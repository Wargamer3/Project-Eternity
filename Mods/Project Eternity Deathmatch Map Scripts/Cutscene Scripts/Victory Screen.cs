using System;
using System.IO;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptVictoryScreen : DeathmatchMapScript
        {
            public ScriptVictoryScreen()
                : this(null)
            {
            }

            public ScriptVictoryScreen(DeathmatchMap Map)
                : base(Map, 100, 50, "Victory Screen", new string[] { "Start Screen" }, new string[0])
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsEnded = true;
                Owner.PushScreen(new VictoryMenu());
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
                return new ScriptVictoryScreen(Map);
            }

            #region Properties

            #endregion
        }
    }
}
