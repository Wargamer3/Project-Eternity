using System;
using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptTriggerVictory : BattleMapScript
        {
            public ScriptTriggerVictory()
                : this(null)
            {
            }

            public ScriptTriggerVictory(BattleMap Map)
                : base(Map, 100, 50, "Trigger Victory", new string[] { "Activate" }, new string[] { })
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Map.GameRule.OnManualVictory();
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
                return new ScriptTriggerVictory(Map);
            }
        }
    }
}
