using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Effects
{
    public sealed class IgnoreEnemySkillEffect : SkillEffect
    {
        public static string Name = "Ignore Enemy Skill Effect";

        private string _IgnoreEnemySkillValue;

        public IgnoreEnemySkillEffect()
            : base(Name, true)
        {
            _IgnoreEnemySkillValue = string.Empty;
        }

        public IgnoreEnemySkillEffect(UnitEffectParams Params)
            : base(Name, true, Params)
        {
            _IgnoreEnemySkillValue = string.Empty;
        }

        protected override void Load(BinaryReader BR)
        {
            _IgnoreEnemySkillValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_IgnoreEnemySkillValue);
        }

        protected override string DoExecuteEffect()
        {
            Params.LocalContext.EffectTargetUnit.ListIgnoreSkill.Add(_IgnoreEnemySkillValue);

            return _IgnoreEnemySkillValue;
        }

        protected override BaseEffect DoCopy()
        {
            IgnoreEnemySkillEffect NewEffect = new IgnoreEnemySkillEffect(Params);

            NewEffect._IgnoreEnemySkillValue = _IgnoreEnemySkillValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            IgnoreEnemySkillEffect NewEffect = (IgnoreEnemySkillEffect)Copy;

            _IgnoreEnemySkillValue = NewEffect._IgnoreEnemySkillValue;
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string Value
        {
            get { return _IgnoreEnemySkillValue; }
            set { _IgnoreEnemySkillValue = value; }
        }

        #endregion
    }
}
