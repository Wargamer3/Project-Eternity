using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptSetVictoryCondition : BattleMapScript
        {
            private string _VictoryCondition;

            public ScriptSetVictoryCondition()
                : this(null)
            {
                _VictoryCondition = "";
            }

            public ScriptSetVictoryCondition(BattleMap Map)
                : base(Map, 100, 50, "Set Victory Condition", new string[] { "Set text" }, new string[] { "Text set" })
            {
                _VictoryCondition = "";
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Map.VictoryCondition = _VictoryCondition;
                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _VictoryCondition = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_VictoryCondition);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSetVictoryCondition(Map);
            }

            #region Properties

            [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor)),
            CategoryAttribute("Objective attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public string VictoryCondition
            {
                get
                {
                    return _VictoryCondition;
                }
                set
                {
                    _VictoryCondition = value;
                }
            }

            #endregion
        }
    }
}
