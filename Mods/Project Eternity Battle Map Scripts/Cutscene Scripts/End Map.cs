using System;
using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptEndMap : BattleMapScript
        {
            public ScriptEndMap()
                : this(null)
            {
            }

            public ScriptEndMap(BattleMap Map)
                : base(Map, 100, 50, "End Map", new string[] { "End" }, new string[] { })
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                BattleMap.ClearedStages++;
                Map.RemoveScreen(Map);
                GameScreen.FMODSystem.sndActiveBGM.Stop();
                Map.PushScreen(new NewIntermissionScreen(Map.PlayerRoster));
                IsEnded = true;
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
                return new ScriptEndMap(Map);
            }
        }
    }
}
