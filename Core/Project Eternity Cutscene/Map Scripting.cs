using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Scripts
{
    public enum MapScriptTypes : byte { Event, Condition, Trigger };

    public abstract class MapScript
    {
        public MapScriptTypes MapScriptType;
        public readonly string Name;

        public System.Drawing.Rectangle ScriptSize;
        public Texture2D ScriptTexture;

        public readonly string[] ArrayNameTrigger;
        public readonly string[] ArrayNameCondition;
        public List<EventInfo>[] ArrayEvents;

        public MapScript(int ScriptWidth, int ScriptHeight, string Name, string[] ArrayNameTrigger, string[] ArrayNameCondition)
        {
            ScriptSize = new System.Drawing.Rectangle(0, 0, ScriptWidth, ScriptHeight);

            this.Name = Name;
            this.ArrayNameTrigger = ArrayNameTrigger;
            this.ArrayNameCondition = ArrayNameCondition;

            ArrayEvents = new List<EventInfo>[ArrayNameCondition.Count()];
            for (int E = ArrayEvents.Count() - 1; E >= 0; --E)
                ArrayEvents[E] = new List<EventInfo>();
        }

        public abstract MapScript CopyScript();

        public abstract void Save(BinaryWriter BW);

        public abstract void Load(BinaryReader BR);

        public abstract void Update(int Index);
        
        public override string ToString()
        {
            return Name;
        }

        public static Dictionary<string, MapCondition> LoadConditions<T>(params object[] Args) where T : MapCondition
        {
            Dictionary<string, MapCondition> DicMapCondition = new Dictionary<string, MapCondition>();
            string[] Files = Directory.GetFiles("Scripts", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                System.Reflection.Assembly ActiveAssembly = System.Reflection.Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<T> ListBattleCondition = ReflectionHelper.GetObjectsFromBaseTypes<T>(typeof(T), ActiveAssembly.GetTypes(), Args);

                foreach (MapCondition Instance in ListBattleCondition)
                {
                    DicMapCondition.Add(Instance.Name, Instance);
                }
            }

            return DicMapCondition;
        }

        public static Dictionary<string, MapTrigger> LoadTriggers<T>(params object[] Args) where T : MapTrigger
        {
            Dictionary<string, MapTrigger> DicMapTrigger = new Dictionary<string, MapTrigger>();
            string[] Files = Directory.GetFiles("Scripts", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                System.Reflection.Assembly ActiveAssembly = System.Reflection.Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<T> ListBattleTrigger = ReflectionHelper.GetObjectsFromBaseTypes<T>(typeof(T), ActiveAssembly.GetTypes(), Args);

                foreach (MapTrigger Instance in ListBattleTrigger)
                {
                    DicMapTrigger.Add(Instance.Name, Instance);
                }
            }

            return DicMapTrigger;
        }

        public static void SaveMapScripts(BinaryWriter BW, List<MapScript> ListMapScript)
        {
            BW.Write(ListMapScript.Count);
            for (int S = 0; S < ListMapScript.Count; S++)
            {
                BW.Write(ListMapScript[S].ScriptSize.X);
                BW.Write(ListMapScript[S].ScriptSize.Y);
                BW.Write((byte)ListMapScript[S].MapScriptType);
                BW.Write(ListMapScript[S].Name);

                ListMapScript[S].Save(BW);

                //Save the Events
                for (int E = 0; E < ListMapScript[S].ArrayEvents.Count(); E++)
                {
                    BW.Write(ListMapScript[S].ArrayEvents[E].Count);//Number of Scripts linked to the Event.
                    for (int i = 0; i < ListMapScript[S].ArrayEvents[E].Count; i++)
                    {
                        BW.Write(ListMapScript[S].ArrayEvents[E][i].LinkedScriptIndex);
                        BW.Write(ListMapScript[S].ArrayEvents[E][i].LinkedScriptTriggerIndex);
                    }
                }
            }
        }

        public static List<MapScript> LoadMapScripts(BinaryReader BR,
                Dictionary<string, MapEvent> DicMapEvent, Dictionary<string, MapCondition> DicMapCondition, Dictionary<string, MapTrigger> DicMapTrigger,
                out List<MapEvent> ListMapEvent)
        {
            ListMapEvent = new List<MapEvent>();
            MapScript NewScript = null;
            int PosX, PosY;
            MapScriptTypes NewScriptType;

            int ListMapScriptCount = BR.ReadInt32();
            List<MapScript> ListMapScript = new List<MapScript>(ListMapScriptCount);
            for (int S = 0; S < ListMapScriptCount; S++)
            {
                PosX = BR.ReadInt32();
                PosY = BR.ReadInt32();
                NewScriptType = (MapScriptTypes)BR.ReadByte();
                string ScriptName = BR.ReadString();

                switch (NewScriptType)
                {
                    #region Event

                    case MapScriptTypes.Event:
                        NewScript = DicMapEvent[ScriptName].CopyScript();
                        ListMapEvent.Add((MapEvent)NewScript);
                        break;

                    #endregion

                    case MapScriptTypes.Condition:
                        NewScript = DicMapCondition[ScriptName].CopyScript();
                        break;

                    case MapScriptTypes.Trigger:
                        NewScript = DicMapTrigger[ScriptName].CopyScript();
                        break;
                }

                NewScript.Load(BR);
                int ArrayEventCount;
                //Load the Events
                for (int E = 0; E < (NewScript).ArrayEvents.Count(); E++)
                {
                    ArrayEventCount = BR.ReadInt32();//Number of Scripts linked to the Event.
                    for (int i = 0; i < ArrayEventCount; i++)
                    {
                        (NewScript).ArrayEvents[E].Add(new EventInfo(BR.ReadInt32(), BR.ReadInt32()));
                    }
                }
                ListMapScript.Add(NewScript);

                NewScript.ScriptSize.X = PosX;
                NewScript.ScriptSize.Y = PosY;
            }

            return ListMapScript;
        }
    }

    public abstract class MapEvent : MapScript
    {
        public MapEvent(int ScriptWidth, int ScriptHeight, string Name, string[] ArrayNameTrigger, string[] ArrayNameCondition)
            : base(ScriptWidth, ScriptHeight, Name, ArrayNameTrigger, ArrayNameCondition)
        {
            MapScriptType = MapScriptTypes.Event;
        }

        public abstract bool IsValid();

        public override void Update(int Index)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class MapCondition : MapScript
    {
        public MapCondition(int ScriptWidth, int ScriptHeight, string Name, string[] ArrayNameTrigger, string[] ArrayNameCondition)
            : base(ScriptWidth, ScriptHeight, Name, ArrayNameTrigger, ArrayNameCondition)
        {
            MapScriptType = MapScriptTypes.Condition;
        }
    }

    public abstract class MapTrigger : MapScript
    {
        public MapTrigger(int ScriptWidth, int ScriptHeight, string Name, string[] ArrayNameTrigger, string[] ArrayNameCondition)
            : base(ScriptWidth, ScriptHeight, Name, ArrayNameTrigger, ArrayNameCondition)
        {
            MapScriptType = MapScriptTypes.Trigger;
        }

        public virtual void Preload() { }
    }

    public class MapScriptGUIHelper
    {
        public delegate void OnSelectDelegate(object SelectedObject, bool RightClick);
        public enum ScriptLinkTypes { None, Trigger, Event };

        public OnSelectDelegate OnSelect;

        private System.Drawing.Point MousePosOld;

        private int ActiveScriptIndex;
        private int ScriptLinkIndex;
        private ScriptLinkTypes ScriptLinkType;
        private int ScriptLinkEventIndex;
        private Point ScriptLinkStartPos;
        private System.Drawing.Point ScriptLinkEndPos;
        private MapScript ScriptLink;
        public Point ScriptStartingPos;//Point from which to start drawing the scripts

        private List<MapScript> ListMapScript;

        private Texture2D sprPixel;
        private SpriteFont fntScriptName;
        private CustomSpriteBatch g;
        private GraphicsDevice GraphicsDevice;

        #region Script Images

        private Texture2D imgScriptTopLeft;
        private Texture2D imgScriptTopMiddle;
        private Texture2D imgScriptTopRight;
        private Texture2D imgScriptMiddleLeft;
        private Texture2D imgScriptMiddleMiddle;
        private Texture2D imgScriptMiddleRight;
        private Texture2D imgScriptBottomLeft;
        private Texture2D imgScriptBottomMiddle;
        private Texture2D imgScriptBottomRight;

        #endregion

        public void Load(ContentManager content, CustomSpriteBatch g, GraphicsDevice GraphicsDevice)
        {
            ActiveScriptIndex = -1;
            ScriptLinkIndex = -1;
            ScriptLinkEventIndex = -1;

            this.g = g;
            this.GraphicsDevice = GraphicsDevice;

            sprPixel = content.Load<Texture2D>("Pixel");
            fntScriptName = content.Load<SpriteFont>("Fonts/Arial8");

            imgScriptTopLeft = content.Load<Texture2D>("Menus/Scripts/ScriptTopLeft2");
            imgScriptTopMiddle = content.Load<Texture2D>("Menus/Scripts/ScriptTopMiddle2");
            imgScriptTopRight = content.Load<Texture2D>("Menus/Scripts/ScriptTopRight2");
            imgScriptMiddleLeft = content.Load<Texture2D>("Menus/Scripts/ScriptMiddleLeft2");
            imgScriptMiddleMiddle = content.Load<Texture2D>("Menus/Scripts/ScriptMiddleMiddle2");
            imgScriptMiddleRight = content.Load<Texture2D>("Menus/Scripts/ScriptMiddleRight2");
            imgScriptBottomLeft = content.Load<Texture2D>("Menus/Scripts/ScriptBottomLeft2");
            imgScriptBottomMiddle = content.Load<Texture2D>("Menus/Scripts/ScriptBottomMiddle2");
            imgScriptBottomRight = content.Load<Texture2D>("Menus/Scripts/ScriptBottomRight2");
        }

        public void SetListMapScript(List<MapScript> ListMapScript)
        {
            this.ListMapScript = ListMapScript;
        }

        public void InitScript(MapScript NewScript)
        {
            RenderTarget2D ScriptBuffer = new RenderTarget2D(GraphicsDevice, NewScript.ScriptSize.Width, NewScript.ScriptSize.Height);
            GraphicsDevice.SetRenderTarget(ScriptBuffer);
            GraphicsDevice.Clear(Color.Transparent);
            g.Begin();

            g.Draw(imgScriptTopLeft, Vector2.Zero, Color.White);
            g.Draw(imgScriptMiddleLeft, new Rectangle(0, imgScriptTopLeft.Height, 1, NewScript.ScriptSize.Height - imgScriptTopMiddle.Height - imgScriptBottomMiddle.Height - 1), Color.White);
            g.Draw(imgScriptBottomLeft, new Vector2(0, NewScript.ScriptSize.Height - imgScriptBottomLeft.Height), Color.White);

            g.Draw(imgScriptTopMiddle, new Rectangle(imgScriptTopLeft.Width, 0, NewScript.ScriptSize.Width - imgScriptTopRight.Width - 1, imgScriptTopMiddle.Height), Color.White);

            g.Draw(imgScriptMiddleMiddle, new Rectangle(imgScriptMiddleLeft.Width,
                                                        imgScriptTopLeft.Height,
                                                        NewScript.ScriptSize.Width - imgScriptMiddleLeft.Width - imgScriptMiddleRight.Width,
                                                        NewScript.ScriptSize.Height - imgScriptTopMiddle.Height - imgScriptBottomMiddle.Height), Color.White);

            g.Draw(imgScriptBottomMiddle, new Rectangle(imgScriptBottomLeft.Width,
                                                        NewScript.ScriptSize.Height - imgScriptBottomMiddle.Height,
                                                        NewScript.ScriptSize.Width - imgScriptBottomLeft.Width - imgScriptBottomRight.Width,
                                                        imgScriptBottomMiddle.Height), Color.White);

            g.Draw(imgScriptTopRight, new Vector2(NewScript.ScriptSize.Width - imgScriptTopRight.Width, 0), Color.White);
            g.Draw(imgScriptMiddleRight, new Rectangle(NewScript.ScriptSize.Width - imgScriptMiddleRight.Width, imgScriptTopRight.Height, 1, NewScript.ScriptSize.Height - imgScriptTopMiddle.Height - imgScriptBottomMiddle.Height), Color.White);
            g.Draw(imgScriptBottomRight, new Vector2(NewScript.ScriptSize.Width - imgScriptBottomRight.Width, NewScript.ScriptSize.Height - imgScriptBottomRight.Height), Color.White);

            g.DrawString(fntScriptName, NewScript.ToString(), new Vector2(3, 0), Color.White);

            for (int T = NewScript.ArrayNameTrigger.Length - 1; T >= 0; --T)
            {
                g.DrawString(fntScriptName, NewScript.ArrayNameTrigger[T], new Vector2(3, 15 + T * 12), Color.White);
            }
            for (int E = NewScript.ArrayNameCondition.Length - 1; E >= 0; --E)
            {
                g.DrawString(fntScriptName, NewScript.ArrayNameCondition[E], new Vector2(
                                                                                         NewScript.ScriptSize.Width - 5 - fntScriptName.MeasureString(NewScript.ArrayNameCondition[E]).X,
                                                                                         NewScript.ScriptSize.Height - NewScript.ArrayNameCondition.Length * 12 - 4 + E * 12), Color.White);
            }
            g.End();

            GraphicsDevice.SetRenderTarget(null);
            Color[] texdata = new Color[NewScript.ScriptSize.Width * NewScript.ScriptSize.Height];
            ScriptBuffer.GetData(texdata);
            NewScript.ScriptTexture = new Texture2D(GraphicsDevice, NewScript.ScriptSize.Width, NewScript.ScriptSize.Height);
            NewScript.ScriptTexture.SetData(texdata);
        }

        public void CreateScript(MapScript OriginalToCopy)
        {
            MapScript NewScript = OriginalToCopy.CopyScript();
            InitScript(NewScript);
            ListMapScript.Add(NewScript);
        }

        public void DeleteSelectedScript()
        {
            for (int S = 0; S < ListMapScript.Count; S++)
            {
                for (int E = 0; E < ListMapScript[S].ArrayEvents.Length; E++)
                {
                    for (int i = 0; i < ListMapScript[S].ArrayEvents[E].Count; i++)
                    {
                        if (ListMapScript[S].ArrayEvents[E][i].LinkedScriptIndex == ActiveScriptIndex)
                            ListMapScript[S].ArrayEvents[E].RemoveAt(i--);
                        else if (ListMapScript[S].ArrayEvents[E][i].LinkedScriptIndex > ActiveScriptIndex)
                        {
                            ListMapScript[S].ArrayEvents[E][i] = new EventInfo(ListMapScript[S].ArrayEvents[E][i].LinkedScriptIndex - 1, ListMapScript[S].ArrayEvents[E][i].LinkedScriptTriggerIndex);
                        }
                    }
                }
            }
            ListMapScript.RemoveAt(ActiveScriptIndex);
            ActiveScriptIndex = -1;
        }

        public void Select(System.Drawing.Point e)
        {
            System.Drawing.Rectangle MouseRec = new System.Drawing.Rectangle(e.X + ScriptStartingPos.X, e.Y + ScriptStartingPos.Y, 1, 1);
            MousePosOld = e;

            for (int S = ListMapScript.Count - 1; S >= 0; --S)
            {
                if (MouseRec.IntersectsWith(ListMapScript[S].ScriptSize))
                {
                    OnSelect(ListMapScript[S], false);
                    ActiveScriptIndex = S;
                    return;
                }
                else
                {
                    //Triggers.
                    for (int T = ListMapScript[S].ArrayNameTrigger.Length - 1; T >= 0; --T)
                    {
                        if (MouseRec.X >= ListMapScript[S].ScriptSize.X - 10 && MouseRec.X <= ListMapScript[S].ScriptSize.X - 5
                             && MouseRec.Y >= ListMapScript[S].ScriptSize.Y + 19 + T * 12 && MouseRec.Y <= ListMapScript[S].ScriptSize.Y + 24 + T * 12)
                        {
                            ScriptLink = ListMapScript[S];
                            ScriptLinkIndex = S;
                            ScriptLinkType = ScriptLinkTypes.Trigger;
                            ScriptLinkEventIndex = T;
                            ScriptLinkStartPos = new Point(ListMapScript[S].ScriptSize.X - 7, ListMapScript[S].ScriptSize.Y + 21 + T * 12);
                            return;
                        }
                    }
                    //Events.
                    for (int E = ListMapScript[S].ArrayNameCondition.Length - 1; E >= 0; --E)
                    {
                        if (MouseRec.X >= ListMapScript[S].ScriptSize.X + ListMapScript[S].ScriptSize.Width + 5
                            && MouseRec.X <= ListMapScript[S].ScriptSize.X + ListMapScript[S].ScriptSize.Width + 10
                            && MouseRec.Y >= ListMapScript[S].ScriptSize.Y + ListMapScript[S].ScriptSize.Height - ListMapScript[S].ArrayNameCondition.Length * 12 + 1 + E * 12
                            && MouseRec.Y <= ListMapScript[S].ScriptSize.Y + ListMapScript[S].ScriptSize.Height - ListMapScript[S].ArrayNameCondition.Length * 12 + 6 + E * 12)
                        {
                            ScriptLink = ListMapScript[S];
                            ScriptLinkIndex = S;
                            ScriptLinkType = ScriptLinkTypes.Event;
                            ScriptLinkEventIndex = E;
                            ScriptLinkStartPos = new Point(ListMapScript[S].ScriptSize.X + ListMapScript[S].ScriptSize.Width + 7, ListMapScript[S].ScriptSize.Y + ListMapScript[S].ScriptSize.Height - ListMapScript[S].ArrayNameCondition.Length * 12 + 3 + E * 12);
                            return;
                        }
                    }
                }
            }
        }

        public void Scripting_MouseUp(System.Drawing.Point e, bool LeftClick, bool RightClick)
        {
            System.Drawing.Rectangle MouseRec = new System.Drawing.Rectangle(e.X + ScriptStartingPos.X, e.Y + ScriptStartingPos.Y, 1, 1);

            #region Action Scripts

            for (int S = ListMapScript.Count - 1; S >= 0; --S)
            {
                if (MouseRec.IntersectsWith(ListMapScript[S].ScriptSize))
                {
                    if (LeftClick)
                    {
                        ActiveScriptIndex = -1;
                        return;
                    }
                    else if (RightClick)
                    {
                        OnSelect(ListMapScript[S], true);
                        return;
                    }
                }

                #region Linking

                else
                {
                    #region Triggers

                    for (int T = ListMapScript[S].ArrayNameTrigger.Length - 1; T >= 0; --T)
                    {
                        if (MouseRec.X >= ListMapScript[S].ScriptSize.X - 10 && MouseRec.X <= ListMapScript[S].ScriptSize.X - 5
                             && MouseRec.Y >= ListMapScript[S].ScriptSize.Y + 19 + T * 12 && MouseRec.Y <= ListMapScript[S].ScriptSize.Y + 24 + T * 12)
                        {
                            EventInfo NewEvent = new EventInfo(S, T);
                            if (LeftClick)
                            {
                                if (ScriptLink != null && ScriptLink != ListMapScript[S])
                                {//Event to Trigger.
                                    if (ScriptLinkType == ScriptLinkTypes.Event && !ScriptLink.ArrayEvents[ScriptLinkEventIndex].Contains(NewEvent))
                                    {
                                        ScriptLink.ArrayEvents[ScriptLinkEventIndex].Add(NewEvent);
                                    }
                                    ScriptLink = null;
                                    ScriptLinkIndex = -1;
                                    ScriptLinkEventIndex = -1;
                                    ScriptLinkType = ScriptLinkTypes.None;
                                    return;
                                }
                            }
                            else if (RightClick)
                            {
                                for (int i = ListMapScript.Count - 1; i >= 0; --i)
                                    for (int E = ListMapScript[i].ArrayEvents.Length - 1; E >= 0; --E)
                                        ListMapScript[i].ArrayEvents[0].Remove(NewEvent);
                            }
                        }
                    }

                    #endregion

                    #region Events

                    for (int E = ListMapScript[S].ArrayNameCondition.Length - 1; E >= 0; --E)
                    {
                        if (MouseRec.X >= ListMapScript[S].ScriptSize.X + ListMapScript[S].ScriptSize.Width + 5 &&
                            MouseRec.X <= ListMapScript[S].ScriptSize.X + ListMapScript[S].ScriptSize.Width + 10 &&
                            MouseRec.Y >= ListMapScript[S].ScriptSize.Y + ListMapScript[S].ScriptSize.Height - ListMapScript[S].ArrayNameCondition.Length * 12 + 1 + E * 12 &&
                            MouseRec.Y <= ListMapScript[S].ScriptSize.Y + ListMapScript[S].ScriptSize.Height - ListMapScript[S].ArrayNameCondition.Length * 12 + 6 + E * 12)
                        {
                            if (LeftClick)
                            {
                                if (ScriptLink != null && ScriptLink != ListMapScript[S])
                                {//Trigger to Event.
                                    if (ScriptLinkType == ScriptLinkTypes.Trigger)
                                    {
                                        ListMapScript[S].ArrayEvents[E].Add(new EventInfo(ScriptLinkIndex, ScriptLinkEventIndex));
                                    }
                                    ScriptLink = null;
                                    ScriptLinkIndex = -1;
                                    ScriptLinkEventIndex = -1;
                                    ScriptLinkType = ScriptLinkTypes.None;
                                    return;
                                }
                            }
                            else if (RightClick)
                            {
                                ListMapScript[S].ArrayEvents[E].Clear();
                            }
                            return;
                        }
                    }

                    #endregion
                }

                #endregion
            }

            #endregion

            ScriptLink = null;
            ScriptLinkIndex = -1;
            ScriptLinkEventIndex = -1;
            OnSelect(null, false);
            ScriptLinkType = ScriptLinkTypes.None;
            ActiveScriptIndex = -1;
        }

        public void MoveScript(System.Drawing.Point e, out int MaxX, out int MaxY)
        {
            MaxX = 0;
            MaxY = 0;

            if (ActiveScriptIndex != -1)
            {
                ListMapScript[ActiveScriptIndex].ScriptSize.X += e.X - MousePosOld.X;
                ListMapScript[ActiveScriptIndex].ScriptSize.Y += e.Y - MousePosOld.Y;

                for (int S = ListMapScript.Count - 1; S >= 0; --S)
                {
                    if (ListMapScript[S].ScriptSize.Right > MaxX)
                        MaxX = ListMapScript[S].ScriptSize.Right;

                    if (ListMapScript[S].ScriptSize.Bottom > MaxY)
                        MaxY = ListMapScript[S].ScriptSize.Bottom;
                }

                MaxX += 10;
                MaxY += 10;
            }
            else if (ScriptLink != null)
            {
                ScriptLinkEndPos = e;
            }
            MousePosOld = e;
        }

        public void DrawScripts()
        {
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            for (int S = ListMapScript.Count - 1; S >= 0; --S)
            {
                int X = ListMapScript[S].ScriptSize.X - ScriptStartingPos.X;
                int Y = ListMapScript[S].ScriptSize.Y - ScriptStartingPos.Y;

                g.Draw(ListMapScript[S].ScriptTexture, new Vector2(X, Y), Color.White);

                for (int T = ListMapScript[S].ArrayNameTrigger.Length - 1; T >= 0; --T)
                {
                    g.Draw(sprPixel, new Rectangle(X - 10, Y + 19 + T * 12, 5, 5), Color.Black);
                }
                for (int E = ListMapScript[S].ArrayNameCondition.Length - 1; E >= 0; --E)
                {
                    g.Draw(sprPixel, new Rectangle(X + ListMapScript[S].ScriptSize.Width + 5,
                                                   Y + ListMapScript[S].ScriptSize.Height - ListMapScript[S].ArrayNameCondition.Length * 12 + 1 + E * 12, 5, 5), Color.Black);

                    for (int i = 0; i < ListMapScript[S].ArrayEvents[E].Count; i++)
                    {
                        DrawLine(g, new Vector2(X + ListMapScript[S].ScriptSize.Width + 7,
                                                Y + ListMapScript[S].ScriptSize.Height - ListMapScript[S].ArrayNameCondition.Length * 12 + 3 + E * 12),
                                    new Vector2(ListMapScript[ListMapScript[S].ArrayEvents[E][i].LinkedScriptIndex].ScriptSize.X - ScriptStartingPos.X - 8,
                                                ListMapScript[ListMapScript[S].ArrayEvents[E][i].LinkedScriptIndex].ScriptSize.Y - ScriptStartingPos.Y + 21 + ListMapScript[S].ArrayEvents[E][i].LinkedScriptTriggerIndex * 12), Color.Black);
                    }
                }
            }

            if (ActiveScriptIndex >= 0 && ActiveScriptIndex < ListMapScript.Count)
                g.Draw(sprPixel, new Rectangle(ListMapScript[ActiveScriptIndex].ScriptSize.X, ListMapScript[ActiveScriptIndex].ScriptSize.Y, ListMapScript[ActiveScriptIndex].ScriptSize.Width, ListMapScript[ActiveScriptIndex].ScriptSize.Height), Color.Black);

            if (ScriptLink != null)
            {
                DrawLine(g, new Vector2(ScriptLinkStartPos.X, ScriptLinkStartPos.Y), new Vector2(ScriptLinkEndPos.X, ScriptLinkEndPos.Y), Color.Black);
            }

            g.End();
        }

        private void DrawLine(CustomSpriteBatch spriteBatch, Vector2 StartPos, Vector2 EndPos, Color color, int width = 1)
        {
            Vector2 v = StartPos - EndPos;
            //Define a line of the length of each small section of the final line.
            Rectangle LineSize = new Rectangle((int)StartPos.X, (int)StartPos.Y, (int)v.Length() + width, width);
            v.Normalize();
            //Get line angle.
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (StartPos.Y > EndPos.Y)
                angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(sprPixel, LineSize, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
