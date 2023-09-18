using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;


namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ActivateSpellEffect : SorcererStreetEffect
    {
        public class SpellSelector : UITypeEditor
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
                    List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathSorcererStreetCardsSpells);
                    if (Items != null)
                    {
                        value = Items[0].Substring(0, Items[0].Length - 4).Substring(39);
                    }
                }
                return value;
            }
        }

        public static string Name = "Sorcerer Street Activate Spell";

        private string _SpellName;

        public ActivateSpellEffect()
            : base(Name, false)
        {
            _SpellName = string.Empty;
        }

        public ActivateSpellEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _SpellName = string.Empty;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _SpellName = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_SpellName);
        }

        public override bool CanActivate()
        {
            return false;
        }

        protected override string DoExecuteEffect()
        {
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            ActivateSpellEffect NewEffect = new ActivateSpellEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        #region Properties

        [Editor(typeof(SpellSelector), typeof(UITypeEditor)),
        CategoryAttribute(""),
        DescriptionAttribute("The spell card path."),
        DefaultValueAttribute(0)]
        public string SpellName
        {
            get
            {
                return _SpellName;
            }
            set
            {
                _SpellName = value;
            }
        }

        #endregion
    }
}
