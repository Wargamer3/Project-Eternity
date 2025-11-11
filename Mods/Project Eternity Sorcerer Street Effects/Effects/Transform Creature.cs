using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using ProjectEternity.Core;
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
                    List<string> Items = EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathSorcererStreetCardsCreatures);
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
            if (Params != null && _CreatureName != "Random" && _CreatureName != "Opponent")
            {
                TransformationCreature = new CreatureCard(_CreatureName, GameScreen.ContentFallback, Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget, Params.DicManualSkillTarget);
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
            Params.RememberEffects = false;

            if (_CreatureName == "Random")
            {
                string CreatureFolder = "Content/Sorcerer Street/Creature Cards/";
                string[] ArrayCreatureFile = Directory.GetFiles(CreatureFolder, "*.pec", SearchOption.AllDirectories);
                string SelectedCreature = ArrayCreatureFile[RandomHelper.Next(ArrayCreatureFile.Length)];
                SelectedCreature = SelectedCreature.Remove(SelectedCreature.Length - 4, 4).Remove(0, CreatureFolder.Length);
                TransformationCreature = new CreatureCard(SelectedCreature, GameScreen.ContentFallback, Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget, Params.DicManualSkillTarget);
            }
            else if (_CreatureName == "Opponent")
            {
                TransformationCreature = Params.GlobalContext.OpponentCreature.Creature;
            }

            if (_Target == Targets.Self)
            {
                Params.ReplaceSelfCreature(TransformationCreature, _IsTemporary);
            }
            else
            {
                Params.ReplaceOtherCreature(TransformationCreature, _IsTemporary);
            }

            Params.RememberEffects = true;

            return "Transformed into " + TransformationCreature.Name;
        }

        protected override BaseEffect DoCopy()
        {
            TransformCreatureEffect NewEffect = new TransformCreatureEffect(Params);

            NewEffect._CreatureName = _CreatureName;
            NewEffect._Target = _Target;
            NewEffect._IsTemporary = _IsTemporary;

            NewEffect.TransformationCreature = TransformationCreature;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            TransformCreatureEffect NewEffect = (TransformCreatureEffect)Copy;

            _CreatureName = NewEffect._CreatureName;
            _Target = NewEffect._Target;
            _IsTemporary = NewEffect._IsTemporary;

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

        #endregion
    }
}
