using System;
using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptLaunchIntermissionScreen : BattleMapScript
        {
            public ScriptLaunchIntermissionScreen()
                : this(null)
            {
            }

            public ScriptLaunchIntermissionScreen(BattleMap Map)
                : base(Map, 140, 50, "Launch Intermission Screen", new string[] { "Launch screen" }, new string[] { })
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                NewIntermissionScreen NewIntermissionScreen = new NewIntermissionScreen(Map.PlayerRoster);
                Owner.RemoveAllScreens();
                Owner.PushScreen(NewIntermissionScreen);
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
                return new ScriptLaunchIntermissionScreen(Map);
            }
        }
    }
}
