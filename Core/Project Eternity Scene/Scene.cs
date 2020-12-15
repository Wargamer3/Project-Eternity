using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens;

namespace ProjectEternity.Core.Scene
{
    public class SceneScreen : GameScreen
    {
        public List<SceneEvent> ListActiveSceneEvent;
        public Dictionary<int, List<SceneEvent>> DicSceneEventByFrame;
        public readonly Dictionary<string, SceneObject> DicSceneObjectByName;
        private int CurrentSceneIndex;
        private bool RestartUpdate;
        private string ScenePath;
        public int MaxSceneEvent;

        public SceneScreen()
        {
            ListActiveSceneEvent = new List<SceneEvent>();
            DicSceneEventByFrame = new Dictionary<int, List<SceneEvent>>();
            DicSceneObjectByName = new Dictionary<string, SceneObject>();
            CurrentSceneIndex = 0;
            RestartUpdate = false;
        }

        public SceneScreen(string ScenePath)
            : this()
        {
            this.ScenePath = ScenePath;
        }

        public override void Load()
        {
            FileStream FS = new FileStream("Content/Scenes/" + ScenePath + ".pes", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            MaxSceneEvent = BR.ReadInt32();
            var DicScene = LoadAllSceneEvents();

            for (int i = 0; i < MaxSceneEvent; ++i)
            {
                int SceneEventCount = BR.ReadInt32();

                DicSceneEventByFrame.Add(i, new List<SceneEvent>(SceneEventCount));
                for (int j = 0; j < SceneEventCount; ++j)
                {
                    var NewSceneEvent = DicScene[BR.ReadString()].Copy();
                    NewSceneEvent.Load(BR);
                    DicSceneEventByFrame[i].Add(NewSceneEvent);
                }
            }

            FS.Close();
            BR.Close();
        }

        public void Save()
        {
            FileStream FS = new FileStream(ScenePath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(MaxSceneEvent);

            for (int i = 0; i < MaxSceneEvent; ++i)
            {
                BW.Write(DicSceneEventByFrame[i].Count);
                for (int j = 0; j < DicSceneEventByFrame[j].Count; ++j)
                {
                    BW.Write(DicSceneEventByFrame[i][j].SceneEventType);
                    DicSceneEventByFrame[i][j].Save(BW);
                }
            }

            FS.Close();
            BW.Close();
        }

        protected Dictionary<string, SceneEvent> LoadSceneEvents(Type TypeOfTimeline, params object[] Args)
        {
            Dictionary<string, SceneEvent> DicSceneEventByType = new Dictionary<string, SceneEvent>();

            string[] Files = Directory.GetFiles("Timelines", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<SceneEvent> Scenes = ReflectionHelper.GetObjectsFromBaseTypes<SceneEvent>(TypeOfTimeline, ActiveAssembly.GetTypes());

                foreach (SceneEvent Instance in Scenes)
                {
                    DicSceneEventByType.Add(Instance.SceneEventType, Instance);
                }
            }

            return DicSceneEventByType;
        }

        public static Dictionary<string, SceneEvent> LoadAllSceneEvents()
        {
            Dictionary<string, SceneEvent> DicSceneEventByType = new Dictionary<string, SceneEvent>();

            Type TypeOfScene = typeof(SceneEvent);

            string[] Files = Directory.GetFiles("Scenes", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<SceneEvent> Scenes = ReflectionHelper.GetObjectsFromBaseTypes<SceneEvent>(TypeOfScene, ActiveAssembly.GetTypes());

                foreach (SceneEvent Instance in Scenes)
                {
                    DicSceneEventByType.Add(Instance.SceneEventType, Instance);
                }
            }
            return DicSceneEventByType;
        }

        public void Goto(int Index)
        {
            CurrentSceneIndex = Index;
            ListActiveSceneEvent = DicSceneEventByFrame[CurrentSceneIndex];
            RestartUpdate = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (ListActiveSceneEvent.Count == 0)
            {
                if (DicSceneEventByFrame.Count < CurrentSceneIndex)
                {
                    ++CurrentSceneIndex;
                    ListActiveSceneEvent = DicSceneEventByFrame[CurrentSceneIndex];
                }
                else
                {
                    RemoveScreen(this);
                }
            }

            for (int S = ListActiveSceneEvent.Count - 1; S >= 0; --S)
            {
                ListActiveSceneEvent[S].Update(gameTime);
                if (RestartUpdate)
                {
                    RestartUpdate = false;
                    S = ListActiveSceneEvent.Count - 1;
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            for (int S = ListActiveSceneEvent.Count - 1; S >= 0; --S)
            {
                ListActiveSceneEvent[S].Draw(g);
            }
        }
    }
}
