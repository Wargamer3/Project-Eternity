using System;
using System.IO;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptTriggerDefeat : BattleMapScript
        {
            public ScriptTriggerDefeat()
                : this(null)
            {
            }

            public ScriptTriggerDefeat(BattleMap Map)
                : base(Map, 100, 50, "Trigger Defeat", new string[] { "Activate" }, new string[] { })
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Map.GameRule.OnManualDefeat();
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
