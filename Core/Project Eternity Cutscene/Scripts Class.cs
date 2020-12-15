using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ProjectEternity.Core.Scripts
{
    public struct EventInfo
    {
        public int LinkedScriptIndex;
        public int LinkedScriptTriggerIndex;

        public EventInfo(int LinkedScriptIndex, int LinkedScriptTriggerIndex)
        {
            this.LinkedScriptIndex = LinkedScriptIndex;
            this.LinkedScriptTriggerIndex = LinkedScriptTriggerIndex;
        }
    }

    public struct PixelData
    {
        public byte Blue;
        public byte Green;
        public byte Red;
        public byte Alpha;

        public override string ToString()
        {
            return "(" + Alpha.ToString() + ", " + Red.ToString() + ", " + Green.ToString() + ", " + Blue.ToString() + ")";
        }
    }

    public unsafe class UnsafeScriptImage : IDisposable
    {
        private bool disposed = false;

        public Bitmap ImageBase;
        public int ImageWidth;
        public System.Drawing.Imaging.BitmapData ImageData;
        public Byte* ImagePointer;

        public UnsafeScriptImage(string ImageName)
        {
            ImageBase = (Bitmap)Bitmap.FromFile(ImageName);
            ImageWidth = ImageBase.Width * sizeof(PixelData);
            Rectangle ImageRect = new Rectangle(0, 0, ImageBase.Width, ImageBase.Height);

            ImageData = ImageBase.LockBits(ImageRect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            ImagePointer = (Byte*)ImageData.Scan0.ToPointer();
        }

        #region IDisposable code

        // Implement IDisposable.

        // Do not make this method virtual.

        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.

            // Therefore, you should call GC.SupressFinalize to

            // take this object off the finalization queue

            // and prevent finalization code for this object

            // from executing a second time.

            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.

        // If disposing equals true, the method has been called directly

        // or indirectly by a user's code. Managed and unmanaged resources

        // can be disposed.

        // If disposing equals false, the method has been called by the

        // runtime from inside the finalizer and you should not reference

        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.

            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed

                // and unmanaged resources.

                ImageData = null;
                ImagePointer = null;

                if (disposing)
                {
                    // Dispose managed resources.
                    ImageBase.UnlockBits(ImageData);
                }

                // Note disposing has been done.

                disposed = true;
            }
        }

        ~UnsafeScriptImage()
        {
            // Do not re-create Dispose clean-up code here.

            // Calling Dispose(false) is optimal in terms of

            // readability and maintainability.

            Dispose(false);
        }

        #endregion
    }

    public abstract unsafe class BasicScript
    {
        public readonly string Name;

        public Rectangle ScriptSize;
        public Bitmap ScriptImage;
        public int ImageWidth;
        public System.Drawing.Imaging.BitmapData ImageData;
        public Byte* ImagePointer;

        protected BasicScript(int ScriptWidth, int ScriptHeight, string Name)
        {
            this.Name = Name;

            ScriptSize = new Rectangle(0, 0, ScriptWidth, ScriptHeight);
            ScriptImage = new Bitmap(ScriptWidth, ScriptHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            ImageWidth = ScriptImage.Width * sizeof(PixelData);
        }

        public abstract void Load(BinaryReader BR);

        public virtual void AfterCutsceneLoad() { }

        public abstract void Save(BinaryWriter BW);

        public void Lock()
        {
            Rectangle ImageSize = new Rectangle(0, 0, ScriptSize.Width, ScriptSize.Height);
            ImageData = ScriptImage.LockBits(ImageSize, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            ImagePointer = (Byte*)ImageData.Scan0.ToPointer();
        }

        public void Unlock()
        {
            ScriptImage.UnlockBits(ImageData);
            ImageData = null;
            ImagePointer = null;
        }

        public void DrawImage(UnsafeScriptImage InputImage, int StartX, int StartY, int EndX = 0, int EndY = 0)
        {
            PixelData* BufferData;
            PixelData* ScriptData;
            for (int Y = InputImage.ImageBase.Height - 1 + EndY; Y >= 0; --Y)
                for (int X = InputImage.ImageBase.Width - 1 + EndX; X >= 0; --X)
                {
                    BufferData = (PixelData*)(this.ImagePointer + (StartY + Y) * this.ImageWidth + (StartX + X) * sizeof(PixelData));
                    ScriptData = (PixelData*)(InputImage.ImagePointer + (Y % InputImage.ImageBase.Height) * InputImage.ImageWidth + (X % InputImage.ImageBase.Width) * sizeof(PixelData));
                    BufferData->Alpha = ScriptData->Alpha;
                    BufferData->Red = ScriptData->Red;
                    BufferData->Green = ScriptData->Green;
                    BufferData->Blue = ScriptData->Blue;
                }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public abstract unsafe class CutsceneScript : BasicScript
    {
        protected Cutscene Owner;

        protected CutsceneScript(int ScriptWidth, int ScriptHeight, string Name)
            :base(ScriptWidth, ScriptHeight, Name)
        {
        }

        protected abstract CutsceneScript DoCopyScript();

        public CutsceneScript CopyScript(Cutscene Owner)
        {
            CutsceneScript ReturnScript = DoCopyScript();
            ReturnScript.Owner = Owner;

            return ReturnScript;
        }
    }

    public abstract class CutsceneActionScript : CutsceneScript
    {
        public ExecuteEventDelegate ExecuteEvent;
        public GetDataContainerByIDDelegate GetDataContainerByID;

        public bool IsActive;
        public bool IsEnded;
        public bool IsDrawn;

        public readonly string[] NameTriggers;
        public readonly string[] NameEvents;
        public List<EventInfo>[] ArrayEvents;

        protected CutsceneActionScript(int ScriptWidth, int ScriptHeight, string Name, string[] NameTriggers, string[] NameEvents)
            : base(ScriptWidth, ScriptHeight, Name)
        {
            this.NameTriggers = NameTriggers;
            this.NameEvents = NameEvents;

            ArrayEvents = new List<EventInfo>[NameEvents.Count()];
            for (int E = ArrayEvents.Count() - 1; E >= 0; --E)
                ArrayEvents[E] = new List<EventInfo>();
        }

        public abstract void ExecuteTrigger(int Index);

        public abstract void Update(Microsoft.Xna.Framework.GameTime gameTime);

        public abstract void Draw(CustomSpriteBatch g);
    }

    public abstract class CutsceneDataContainer : CutsceneScript
    {
        protected UInt32 _ID;

        protected CutsceneDataContainer(int ScriptWidth, int ScriptHeight, string Name)
            : base(ScriptWidth, ScriptHeight, Name)
        {
        }

        #region Properties

        [CategoryAttribute("Base Informations"),
        DescriptionAttribute("Identification Number used for tracking Scripts."),
        DefaultValueAttribute(0)]
        public UInt32 ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        #endregion
    }

    public class ScriptCutsceneBehavior : CutsceneActionScript
    {
        public ScriptCutsceneBehavior()
            : base(140, 70, "Cutscene Behavior", new string[0], new string[] { "Cutscene Start", "Cutscene Update" })
        {
            IsEnded = true;
        }

        public override void ExecuteTrigger(int Index)
        {
            throw new NotImplementedException();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            throw new NotImplementedException();
        }

        public override void Load(BinaryReader BR)
        {
        }

        public override void Save(BinaryWriter BW)
        {
        }

        protected override CutsceneScript DoCopyScript()
        {
            return new ScriptCutsceneBehavior();
        }
    }
}
