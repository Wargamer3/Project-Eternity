using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using System.Windows.Forms.Design;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SummonCreatureEffect : SorcererStreetEffect
    {
        public class CreatureSelector : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                    provider.GetService(typeof(IWindowsFormsEditorService));
                if (svc != null)
                {
                    List<string> Items = EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathSorcererStreetCardsCreatures);
                    if (Items != null)
                    {
                        value = Items[0].Substring(0, Items[0].Length - 4).Substring(39);
                    }
                }
                return value;
            }
        }

        public static string Name = "Sorcerer Street Summon Creature";

        public enum EmplacementTypes { OldMovementPosition, PlayerPosition, Random }
        public enum SummonTypes { Clone, Specific }

        public EmplacementTypes _EmplacementType;
        public SummonTypes _SummonType;
        public string _CreatureName;
        private CreatureCard TransformationCreature;

        public SummonCreatureEffect()
            : base(Name, false)
        {
        }

        public SummonCreatureEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _CreatureName = string.Empty;
        }

        protected override void Load(BinaryReader BR)
        {
            _EmplacementType = (EmplacementTypes)BR.ReadByte();
            _SummonType = (SummonTypes)BR.ReadByte();

            if (_SummonType != SummonTypes.Clone)
            {
                _CreatureName = BR.ReadString();
                if (Params != null && _CreatureName != "Random")
                {
                    TransformationCreature = new CreatureCard(_CreatureName, GameScreen.ContentFallback, Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget);
                }
            }
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_EmplacementType);
            BW.Write((byte)_SummonType);
            if (_SummonType != SummonTypes.Clone)
            {
                BW.Write(_CreatureName);
            }
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            if (_CreatureName == "Random")
            {
                string CreatureFolder = "Content/Sorcerer Street/Creature Cards/";
                string[] ArrayCreatureFile = Directory.GetFiles(CreatureFolder, "*.pec", SearchOption.AllDirectories);
                string SelectedCreature = ArrayCreatureFile[RandomHelper.Next(ArrayCreatureFile.Length)];
                SelectedCreature = SelectedCreature.Remove(SelectedCreature.Length - 4, 4).Remove(0, CreatureFolder.Length);
                TransformationCreature = new CreatureCard(SelectedCreature, GameScreen.ContentFallback, Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget);
            }

            return "Summon " + TransformationCreature.Name;
        }

        protected override BaseEffect DoCopy()
        {
            SummonCreatureEffect NewEffect = new SummonCreatureEffect(Params);

            NewEffect._CreatureName = _CreatureName;
            NewEffect._EmplacementType = _EmplacementType;
            NewEffect._SummonType = _SummonType;

            NewEffect.TransformationCreature = TransformationCreature;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SummonCreatureEffect NewEffect = (SummonCreatureEffect)Copy;

            _CreatureName = NewEffect._CreatureName;
            _EmplacementType = NewEffect._EmplacementType;
            _SummonType = NewEffect._SummonType;

            TransformationCreature = NewEffect.TransformationCreature;
        }

        #region Properties

        [Editor(typeof(CreatureSelector), typeof(UITypeEditor)),
        CategoryAttribute(""),
        DescriptionAttribute("The creature card path."),
        DefaultValueAttribute(0)]
        public string CreatureName
        {
            get
            {
                return _CreatureName;
            }
            set
            {
                _CreatureName = value;
            }
        }

        [CategoryAttribute(""),
        DescriptionAttribute("The Target."),
        DefaultValueAttribute(0)]
        public EmplacementTypes EmplacementType
        {
            get
            {
                return _EmplacementType;
            }
            set
            {
                _EmplacementType = value;
            }
        }

        [CategoryAttribute(""),
        DescriptionAttribute("The Target."),
        DefaultValueAttribute(0)]
        public SummonTypes SummonType
        {
            get
            {
                return _SummonType;
            }
            set
            {
                _SummonType = value;
            }
        }

        #endregion
    }
}
