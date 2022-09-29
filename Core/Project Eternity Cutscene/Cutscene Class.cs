using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProjectEternity.GameScreens;

namespace ProjectEternity.Core.Scripts
{
    public delegate void ExecuteEventDelegate(CutsceneActionScript InputScript, int Index);

    public delegate void AppendCutsceneDelegate(Cutscene CutsceneToAppend);

    public delegate void OnCutsceneEndedDelegate();

    public delegate CutsceneDataContainer GetDataContainerByIDDelegate(uint ID, string ScriptName);

    public abstract class CutsceneScriptHolder : Holder<CutsceneScript>
    {
        public static Dictionary<string, CutsceneScript> LoadAllScripts()
        {
            return LoadAllScripts(typeof(CutsceneScriptHolder));
        }

        public static Dictionary<string, CutsceneScript> LoadAllScripts(Type HolderType, params object[] Args)
        {
            Dictionary<string, List<CutsceneScript>> DicAllScript = GetScriptsByCategory(HolderType, Args);
            Dictionary<string, CutsceneScript> ReturnValue = new Dictionary<string, CutsceneScript>();
            foreach (KeyValuePair<string, List<CutsceneScript>> ActiveListScript in DicAllScript)
            {
                for (int S = ActiveListScript.Value.Count - 1; S >= 0; --S)
                    ReturnValue.Add(ActiveListScript.Value[S].Name, ActiveListScript.Value[S]);
            }

            return ReturnValue;
        }

        public static Dictionary<string, List<CutsceneScript>> GetScriptsByCategory()
        {
            return GetScriptsByCategory(typeof(CutsceneScriptHolder));
        }

        public static Dictionary<string, List<CutsceneScript>> GetScriptsByCategory(Type HolderType, params object[] Args)
        {
            return ReflectionHelper.GetContentByName<CutsceneScript>(HolderType, Directory.GetFiles("Scripts", "*.dll"), Args);
        }
    }

    public unsafe partial class Cutscene : GameScreen
    {
        public Dictionary<string, CutsceneScript> DicCutsceneScript;
        protected string CutscenePath;
        private bool IsInit;
        public bool IsInEditor;
        private double WarmUpSeconds;

        public Dictionary<int, CutsceneActionScript> DicActionScript;
        private int NextActionScriptID;
        public List<CutsceneDataContainer> ListDataContainer;
        public List<ScriptCutsceneBehavior> ListCutsceneBehavior;
        public List<Cutscene> ListSubCutscene;
        private OnCutsceneEndedDelegate OnCutsceneEnded;

        public Cutscene(OnCutsceneEndedDelegate OnCutsceneEnded, Dictionary<string, CutsceneScript> DicCutsceneScript)
            : base()
        {
            this.OnCutsceneEnded = OnCutsceneEnded;
            this.DicCutsceneScript = DicCutsceneScript;

            DicActionScript = new Dictionary<int, CutsceneActionScript>();
            NextActionScriptID = 0;
            ListDataContainer = new List<CutsceneDataContainer>();
            ListCutsceneBehavior = new List<ScriptCutsceneBehavior>();
            ListSubCutscene = new List<Cutscene>();
            WarmUpSeconds = 0;
        }

        public Cutscene(OnCutsceneEndedDelegate OnCutsceneEnded, string CutscenePath, Dictionary<string, CutsceneScript> DicCutsceneScript)
            : base()
        {
            IsInit = false;
            this.OnCutsceneEnded = OnCutsceneEnded;
            this.CutscenePath = CutscenePath;
            this.DicCutsceneScript = DicCutsceneScript;
            RequireFocus = false;
            RequireDrawFocus = false;

            DicActionScript = new Dictionary<int, CutsceneActionScript>();
            NextActionScriptID = 0;
            ListDataContainer = new List<CutsceneDataContainer>();
            ListCutsceneBehavior = new List<ScriptCutsceneBehavior>();
            ListSubCutscene = new List<Cutscene>();
            WarmUpSeconds = 0;
        }

        public Cutscene(OnCutsceneEndedDelegate OnCutsceneEnded, BinaryReader BR, Dictionary<string, CutsceneScript> DicCutsceneScript)
            : base()
        {
            IsInit = false;
            this.OnCutsceneEnded = OnCutsceneEnded;
            this.DicCutsceneScript = DicCutsceneScript;
            RequireFocus = false;
            RequireDrawFocus = false;

            DicActionScript = new Dictionary<int, CutsceneActionScript>();
            NextActionScriptID = 0;
            ListDataContainer = new List<CutsceneDataContainer>();
            ListCutsceneBehavior = new List<ScriptCutsceneBehavior>();
            ListSubCutscene = new List<Cutscene>();
            WarmUpSeconds = 0;

            Load(BR);
        }

        private void Load(BinaryReader BR)
        {
            DicActionScript.Clear();
            int ScriptCount = BR.ReadInt32();
            CutsceneScript NewScript = null;
            int PosX, PosY;
            uint ID = 0;
            string NewScriptType;
            Cutscene ScriptOwner;
            if (IsInEditor)
            {
                ScriptOwner = null;
            }
            else
            {
                ScriptOwner = this;
            }

            #region Action Scripts

            NextActionScriptID = ScriptCount;
            for (int S = 0; S < ScriptCount; S++)
            {
                NewScriptType = BR.ReadString();
                PosX = BR.ReadInt32();
                PosY = BR.ReadInt32();
                NewScript = DicCutsceneScript[NewScriptType].CopyScript(ScriptOwner);

                switch (NewScriptType)
                {
                    case "Cutscene Behavior":
                        ListCutsceneBehavior.Add((ScriptCutsceneBehavior)NewScript);
                        break;
                }

                NewScript.Load(BR);

                if (NewScript is CutsceneActionScript)
                {
                    ((CutsceneActionScript)NewScript).ExecuteEvent = ExecuteEvent;
                    ((CutsceneActionScript)NewScript).GetDataContainerByID = GetDataContainerByID;
                    int ArrayEventCount;
                    //Load the Events
                    for (int E = 0; E < ((CutsceneActionScript)NewScript).ArrayEvents.Count(); E++)
                    {
                        ArrayEventCount = BR.ReadInt32();//Number of Scripts linked to the Event.
                        for (int i = 0; i < ArrayEventCount; i++)
                        {
                            ((CutsceneActionScript)NewScript).ArrayEvents[E].Add(new EventInfo(BR.ReadInt32(), BR.ReadInt32()));
                        }
                    }
                    DicActionScript.Add(S, (CutsceneActionScript)NewScript);
                }
                else
                    throw new Exception("The cutscene failed to load. " + NewScript.Name + "didn't load properly.");

                NewScript.ScriptSize.X = PosX;
                NewScript.ScriptSize.Y = PosY;
            }

            #endregion

            #region Data Container

            int ScriptDataContainer = BR.ReadInt32();
            for (int D = 0; D < ScriptDataContainer; D++)
            {
                NewScriptType = BR.ReadString();
                ID = BR.ReadUInt32();
                PosX = BR.ReadInt32();
                PosY = BR.ReadInt32();
                NewScript = DicCutsceneScript[NewScriptType].CopyScript(this);

                if (NewScript is CutsceneDataContainer)
                {
                    ((CutsceneDataContainer)NewScript).ID = ID;
                    ListDataContainer.Add(((CutsceneDataContainer)NewScript));
                }
                else
                    throw new Exception("The cutscene failed to load. " + NewScript.Name + "didn't load properly.");

                NewScript.Load(BR);
                NewScript.ScriptSize.X = PosX;
                NewScript.ScriptSize.Y = PosY;
            }

            #endregion
        }

        public void LoadForEditor()
        {
            if (!File.Exists("Content/Cutscenes/" + CutscenePath + ".pec"))
                return;

            FileStream FS = new FileStream("Content/Cutscenes/" + CutscenePath + ".pec", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            Load(BR);

            FS.Close();
            BR.Close();
        }

        public override void Load()
        {
            if (!File.Exists("Content/Cutscenes/" + CutscenePath + ".pec"))
                return;

            FileStream FS = new FileStream("Content/Cutscenes/" + CutscenePath + ".pec", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            Load(BR);

            for (int S = 0; S < DicActionScript.Count; S++)
            {
                DicActionScript[S].AfterCutsceneLoad();
            }

            for (int D = 0; D < ListDataContainer.Count; D++)
            {
                ListDataContainer[D].AfterCutsceneLoad();
            }

            FS.Close();
            BR.Close();
        }

        public void Save(string FilePath)
        {
            FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            Save(BW);

            FS.Close();
            BW.Close();
        }

        public void Save(BinaryWriter BW)
        {
            #region Action Scripts

            BW.Write(DicActionScript.Count);

            for (int S = 0; S < DicActionScript.Count; S++)
            {
                BW.Write(DicActionScript[S].Name);
                BW.Write(DicActionScript[S].ScriptSize.X);
                BW.Write(DicActionScript[S].ScriptSize.Y);
                DicActionScript[S].Save(BW);

                //Save the Events
                for (int E = 0; E < DicActionScript[S].ArrayEvents.Count(); E++)
                {
                    BW.Write(DicActionScript[S].ArrayEvents[E].Count);//Number of Scripts linked to the Event.
                    for (int i = 0; i < DicActionScript[S].ArrayEvents[E].Count; i++)
                    {
                        BW.Write(DicActionScript[S].ArrayEvents[E][i].LinkedScriptIndex);
                        BW.Write(DicActionScript[S].ArrayEvents[E][i].LinkedScriptTriggerIndex);
                    }
                }
            }

            #endregion

            #region Data Container

            BW.Write(ListDataContainer.Count);

            for (int D = 0; D < ListDataContainer.Count; D++)
            {
                BW.Write(ListDataContainer[D].Name);
                BW.Write(ListDataContainer[D].ID);
                BW.Write(ListDataContainer[D].ScriptSize.X);
                BW.Write(ListDataContainer[D].ScriptSize.Y);
                ListDataContainer[D].Save(BW);
            }

            #endregion
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!IsInit)
            {
                IsInit = true;
                for (int C = 0; C < ListCutsceneBehavior.Count; C++)
                {
                    ListCutsceneBehavior[C].ExecuteEvent(ListCutsceneBehavior[C], 0);
                }
                return;
            }

            WarmUpSeconds += gameTime.ElapsedGameTime.TotalSeconds;

            //Hack for xna to counter insane increase in FPS after a loading
            if (gameTime.IsRunningSlowly && WarmUpSeconds <= 2)
                return;

            bool CutsceneIsActive = CheckIfCutsceneIsActive();

            for (int C = 0; C < ListSubCutscene.Count && !CutsceneIsActive; C++)
            {
                ListSubCutscene[C].Update(gameTime);
                CutsceneIsActive = ListSubCutscene[C].CheckIfCutsceneIsActive();
            }

            if (!CutsceneIsActive)
            {
                RemoveScreen(this);

                if (OnCutsceneEnded != null)
                    OnCutsceneEnded();
            }

            for (int S = 0; S < DicActionScript.Count; S++)
            {
                if (DicActionScript[S].IsActive && !DicActionScript[S].IsEnded)
                    DicActionScript[S].Update(gameTime);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            for (int C = 0; C < ListSubCutscene.Count; C++)
            {
                ListSubCutscene[C].Draw(g);
            }

            for (int S = 0; S < DicActionScript.Count; S++)
            {
                if (DicActionScript[S].IsDrawn && !DicActionScript[S].IsEnded)
                    DicActionScript[S].Draw(g);
            }
        }

        public void AddActionScript(CutsceneActionScript NewScript)
        {
            DicActionScript.Add(NextActionScriptID++, NewScript);
        }

        public void RemoveActionScript(int ScriptIndex)
        {
            if (DicActionScript.Count == 0 || DicActionScript.Count == 1)
            {
                NextActionScriptID = 0;
                DicActionScript.Clear();
                return;
            }
            for (int S = ScriptIndex; S < DicActionScript.Count - 1; ++S)
            {
                DicActionScript[S] = DicActionScript[S + 1];
            }
            DicActionScript.Remove(DicActionScript.Count - 1);
            NextActionScriptID = DicActionScript.Count;
        }

        public bool CheckIfCutsceneIsActive()
        {
            bool CutsceneIsActive = false;

            for (int C = 0; C < ListCutsceneBehavior.Count; C++)
            {
                ListCutsceneBehavior[C].ExecuteEvent(ListCutsceneBehavior[C], 1);
                if (CheckIfScriptIsActive(ListCutsceneBehavior[C]))
                    CutsceneIsActive = true;
            }

            return CutsceneIsActive;
        }

        /// <summary>
        /// Call every Triggers linked to an Event.
        /// </summary>
        /// <param name="Index">Index of the Event to call.</param>
        public void ExecuteEvent(CutsceneActionScript InputScript, int Index)
        {
            for (int E = 0; E < InputScript.ArrayEvents[Index].Count; E++)
            {
                DicActionScript[InputScript.ArrayEvents[Index][E].LinkedScriptIndex].ExecuteTrigger(InputScript.ArrayEvents[Index][E].LinkedScriptTriggerIndex);
            }
        }

        public CutsceneDataContainer GetDataContainerByID(UInt32 ID, string ScriptName)
        {
            for (int S = 0; S < ListDataContainer.Count; S++)
            {
                if (ListDataContainer[S].Name != ScriptName)
                    continue;
                if (ListDataContainer[S].ID == ID)
                {
                    return ListDataContainer[S];
                }
            }
            return null;
        }

        public void AppendCutscene(Cutscene ActiveCutscene)
        {
            ListSubCutscene.Add(ActiveCutscene);
        }

        public bool CheckIfScriptIsActive(CutsceneActionScript Script)
        {
            if (Script.IsActive && !Script.IsEnded)
                return true;
            else
            {
                for (int E = 0; E < Script.ArrayEvents.Length; E++)
                {
                    for (int S = 0; S < Script.ArrayEvents[E].Count; S++)
                    {
                        if (CheckIfScriptIsActive(DicActionScript[Script.ArrayEvents[E][S].LinkedScriptIndex]))
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
