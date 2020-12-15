using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.Core.AI
{
    public abstract class AIScriptHolder : Holder<AIScript>
    {
        public static Dictionary<string, AIScript> DicAIScripts = new Dictionary<string, AIScript>();

        public static Dictionary<string, AIScript> LoadAllScripts(params object[] Args)
        {
            Dictionary<string, List<AIScript>> DicAllScript = GetAllScriptsByCategory(Args);
            Dictionary<string, AIScript> ReturnValue = new Dictionary<string, AIScript>();
            foreach (KeyValuePair<string, List<AIScript>> ActiveListScript in DicAllScript)
            {
                for (int S = ActiveListScript.Value.Count - 1; S >= 0; --S)
                    ReturnValue.Add(ActiveListScript.Value[S].Name, ActiveListScript.Value[S]);
            }

            return ReturnValue;
        }

        public static Dictionary<string, List<AIScript>> GetAllScriptsByCategory(params object[] Args)
        {
            return ReflectionHelper.GetContentByName<AIScript>(Directory.GetFiles("AI", "*.dll"), Args);
        }
    }

    public class AIContainer
    {
        public List<AIScript> ListScript;
        private List<ScriptEvent> ListEvent;
        public List<object> ListReturnValue;
        public string Path;

        public AIContainer()
        {
            ListScript = new List<AIScript>();
            ListEvent = new List<ScriptEvent>();
            ListReturnValue = new List<object>();
        }

        public void Load(string FilePath)
        {
            this.Path = FilePath;
            FileStream FS = new FileStream("Content/AIs/" + FilePath + ".peai", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            Load(BR);

            FS.Close();
            BR.Close();
        }

        public void Load(BinaryReader BR)
        {
            int ListScriptCount = BR.ReadInt32();
            ListScript = new List<AIScript>(ListScriptCount);

            for (int S = 0; S < ListScriptCount; ++S)
            {
                string NewScriptType = BR.ReadString();
                int PosX = BR.ReadInt32();
                int PosY = BR.ReadInt32();

                AIScript NewScript = AIScriptHolder.DicAIScripts[NewScriptType].CopyScript();
                NewScript.Owner = this;
                NewScript.Load(BR);
                OnScriptLoad(NewScript);

                for (int F = 0; F < NewScript.ArrayFollowingScript.Length; ++F)
                {
                    int ListScriptIndexCount = BR.ReadInt32();
                    for (int i = 0; i < ListScriptIndexCount; ++i)
                    {
                        NewScript.ArrayFollowingScript[F].ListScriptIndex.Add(BR.ReadInt32());
                        NewScript.ArrayFollowingScript[F].ListScript.Add(null);
                    }
                }

                for (int R = 0; R < NewScript.ArrayReferences.Length; ++R)
                {
                    NewScript.ArrayReferences[R].ReferencedScriptIndex = BR.ReadInt32();
                }

                ListScript.Add(NewScript);

                var NewEvent = NewScript as ScriptEvent;

                if (NewEvent != null)
                    ListEvent.Add(NewEvent);

                NewScript.ScriptSize.X = PosX;
                NewScript.ScriptSize.Y = PosY;
            }

            for (int S = 0; S < ListScript.Count; ++S)
            {
                foreach (FollowingScripts FollowingScript in ListScript[S].ArrayFollowingScript)
                {
                    //Load the Events
                    for (int E = 0; E < FollowingScript.ListScriptIndex.Count; E++)
                    {
                        FollowingScript.ListScript[E] = (ScriptEvaluator)ListScript[FollowingScript.ListScriptIndex[E]];
                    }
                }

                foreach (ReferenceToScript Reference in ListScript[S].ArrayReferences)
                {
                    if (Reference.ReferencedScriptIndex >= 0)
                        Reference.ReferencedScript = (ScriptReference)ListScript[Reference.ReferencedScriptIndex];
                }
            }
        }

        public void Save(string FilePath)
        {
            //Create the Part file.
            FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            Save(BW);

            FS.Close();
            BW.Close();
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ListScript.Count);
            for (int S = 0; S < ListScript.Count; ++S)
            {
                var ActiveScript = ListScript[S];
                BW.Write(ActiveScript.Name);
                BW.Write(ActiveScript.ScriptSize.X);
                BW.Write(ActiveScript.ScriptSize.Y);

                ActiveScript.Save(BW);

                for (int F = 0; F < ActiveScript.ArrayFollowingScript.Length; ++F)
                {
                    BW.Write(ActiveScript.ArrayFollowingScript[F].ListScript.Count);
                    for (int i = 0; i < ActiveScript.ArrayFollowingScript[F].ListScript.Count; ++i)
                    {
                        int IndexOf = ListScript.IndexOf((AIScript)ActiveScript.ArrayFollowingScript[F].ListScript[i]);
                        BW.Write(IndexOf);
                    }
                }

                for (int R = 0; R < ActiveScript.ArrayReferences.Length; ++R)
                {
                    int IndexOf = ListScript.IndexOf((AIScript)ActiveScript.ArrayReferences[R].ReferencedScript);
                    BW.Write(IndexOf);
                }
            }
        }

        public void UpdateStep(GameTime gameTime)
        {
            ListReturnValue.Clear();
            List<object> FollowingScriptResult = new List<object>();

            foreach (ScriptEvent ActiveEvent in ListEvent)
            {
                if (ActiveEvent.Name == "On Step" || ActiveEvent.Name == "Custom Event")
                {
                    ActiveEvent.OnCalled(gameTime, out FollowingScriptResult);
                    ListReturnValue.AddRange(FollowingScriptResult);
                }
            }
        }

        public void Update(GameTime gameTime, string EventName)
        {
            ListReturnValue.Clear();
            List<object> FollowingScriptResult = new List<object>();

            foreach (ScriptEvent ActiveEvent in ListEvent)
            {
                if (ActiveEvent.Name == EventName)
                {
                    ActiveEvent.OnCalled(gameTime, out FollowingScriptResult);
                    ListReturnValue.AddRange(FollowingScriptResult);
                }
            }
        }

        // Used by Subroutine Script, if the current Mod is using a different AIContainer, it needs to use the right object.
        public virtual AIContainer Copy()
        {
            return new AIContainer();
        }

        public virtual void OnScriptLoad(AIScript NewScript)
        {
        }
    }

    public abstract class ScriptEvent : AIScript
    {
        protected ScriptEvent(int ScriptWidth, int ScriptHeight, string Name, string[] ArrayFollowingScript, string[] ArrayReferences)
            : base(ScriptWidth, ScriptHeight, Name, ArrayFollowingScript, ArrayReferences)
        {
        }

        public abstract void OnCalled(GameTime gameTime, out List<object> Result);
    }

    public interface ScriptReference
    {
        object GetContent();
    }

    public interface ScriptEvaluator
    {
        void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result);
    }

    public class ReferenceToScript
    {
        public string ReferenceName;
        public ScriptReference ReferencedScript;
        public int ReferencedScriptIndex;

        public ReferenceToScript(string ReferenceName)
        {
            this.ReferenceName = ReferenceName;
            ReferencedScript = null;
            ReferencedScriptIndex = -1;
        }
    }

    public class FollowingScripts
    {
        public string ReferenceName;
        public List<ScriptEvaluator> ListScript;
        public List<int> ListScriptIndex;

        public FollowingScripts(string ReferenceName)
        {
            this.ReferenceName = ReferenceName;
            ListScript = new List<ScriptEvaluator>();
            ListScriptIndex = new List<int>();
        }

        public override string ToString()
        {
            return ReferenceName;
        }
    }

    public abstract class AIScript : BasicScript
    {
        public FollowingScripts[] ArrayFollowingScript;
        public ReferenceToScript[] ArrayReferences;
        private string _Comment;
        public AIContainer Owner;

        protected AIScript(int ScriptWidth, int ScriptHeight, string Name, string[] ArrayFollowingScript, string[] ArrayReferences)
            : base(ScriptWidth, ScriptHeight, Name)
        {
            _Comment = string.Empty;

            this.ArrayFollowingScript = new FollowingScripts[ArrayFollowingScript.Length];
            for (int S = ArrayFollowingScript.Length - 1; S >=0; --S)
            {
                this.ArrayFollowingScript[S] = new FollowingScripts(ArrayFollowingScript[S]);
            }

            this.ArrayReferences = new ReferenceToScript[ArrayReferences.Length];
            for (int S = ArrayReferences.Length - 1; S >= 0; --S)
            {
                this.ArrayReferences[S] = new ReferenceToScript(ArrayReferences[S]);
            }
        }

        public override void Load(BinaryReader BR)
        {
            _Comment = BR.ReadString();
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(_Comment);
        }

        public abstract AIScript CopyScript();

        public void ExecuteFollowingScripts(int Index, GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
        {
            bool IsEverythingCompleted = true;
            IsCompleted = true;
            Result = new List<object>();
            List<object> FollowingScriptResult = new List<object>();

            for (int i = 0; i < ArrayFollowingScript[Index].ListScript.Count && IsEverythingCompleted; ++i)
            {
                ArrayFollowingScript[Index].ListScript[i].Evaluate(gameTime, Input, out IsCompleted, out FollowingScriptResult);
                IsEverythingCompleted &= IsCompleted;
                Result.AddRange(FollowingScriptResult);
            }
        }

        [Editor(typeof(Selectors.FollowingScriptOrderSelector), typeof(UITypeEditor)),
        CategoryAttribute("Followin Scripts Order"),
        DescriptionAttribute("")]
        public FollowingScripts[] FollowingScriptsOrder
        {
            get
            {
                return ArrayFollowingScript;
            }
            set
            {
                ArrayFollowingScript = value;
            }
        }

        [CategoryAttribute("Script Attributes"),
        DescriptionAttribute("")]
        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                _Comment = value;
            }
        }
    }
}
