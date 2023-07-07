using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class TransformCreatureEffect : SorcererStreetEffect
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
                    List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathSorcererStreetCardsCreatures);
                    if (Items != null)
                    {
                        value = Items[0].Substring(0, Items[0].Length - 4).Substring(39);
                    }
                }
                return value;
            }
        }

        public static string Name = "Sorcerer Street Transform Creature";

        public enum Targets { Self, Opponent }

        private string _CreatureName;
        private Targets _Target;
        private bool _IsTemporary;//Transform back after battle
        private CreatureCard TransformationCreature;

        public TransformCreatureEffect()
            : base(Name, false)
        {
            _CreatureName = string.Empty;
        }

        public TransformCreatureEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _CreatureName = string.Empty;
        }

        protected override void Load(BinaryReader BR)
        {
            _CreatureName = BR.ReadString();
            _Target = (Targets)BR.ReadByte();
            _IsTemporary = BR.ReadBoolean();
            if (Params != null && _CreatureName != "Random")
            {
                TransformationCreature = new CreatureCard(_CreatureName, GameScreen.ContentFallback, Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget);
            }
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_CreatureName);
            BW.Write((byte)_Target);
            BW.Write(_IsTemporary);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            if (_Target == Targets.Self)
            {
                Params.ReplaceSelfCreature(TransformationCreature);
            }
            else
            {
                Params.ReplaceOtherCreature(TransformationCreature);
            }

            return "Transformed into " + TransformationCreature.Name;
        }

        protected override BaseEffect DoCopy()
        {
            TransformCreatureEffect NewEffect = new TransformCreatureEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

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
        public Targets Target
        {
            get
            {
                return _Target;
            }
            set
            {
                _Target = value;
            }
        }

        [CategoryAttribute(""),
        DescriptionAttribute("The Target."),
        DefaultValueAttribute(0)]
        public bool IsTemporary
        {
            get
            {
                return _IsTemporary;
            }
            set
            {
                _IsTemporary = value;
            }
        }
    }
}
