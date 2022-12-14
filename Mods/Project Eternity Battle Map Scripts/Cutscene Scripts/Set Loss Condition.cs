using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptSetLossCondition : BattleMapScript
        {
            private string _LossCondition;

            public ScriptSetLossCondition()
                : this(null)
            {
                _LossCondition = "";
            }

            public ScriptSetLossCondition(BattleMap Map)
                : base(Map, 100, 50, "Set Loss Condition", new string[] { "Set text" }, new string[] { "Text set" })
            {
                _LossCondition = "";
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Map.LossCondition = _LossCondition;
                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _LossCondition = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_LossCondition);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSetLossCondition(Map);
            }

            #region Properties

            [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor)),
            CategoryAttribute("Objective attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public string LossCondition
            {
                get
                {
                    return _LossCondition;
                }
                set
                {
                    _LossCondition = value;
                }
            }

            #endregion
        }
    }
}
