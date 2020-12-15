using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class AddEffectToUnitPositionTrigger : DeathmatchTrigger
    {
        private System.Drawing.Point _Position;
        private SkillEffect Effect;

        public AddEffectToUnitPositionTrigger()
            : base(140, 70, "Add Effect To Unit Position", new string[] { "Asign Variable" }, new string[] { "Variable asigned" })
        {
        }

        public override void Save(BinaryWriter BW)
        {
        }

        public override void Load(BinaryReader BR)
        {
        }

        public override void Update(int Index)
        {
            throw new NotImplementedException();
        }

        public override MapScript CopyScript()
        {
            return new AddEffectToUnitPositionTrigger();
        }

        #region Properties

        [CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute("")]
        public System.Drawing.Point Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }

        #endregion
    }
}
