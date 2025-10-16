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
    public sealed class AddCardEffect : SorcererStreetEffect
    {
        public class CardSelector : UITypeEditor
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
                    List<string> Items = EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathSorcererStreetCards);
                    if (Items != null)
                    {
                        value = Items[0].Substring(0, Items[0].Length - 4).Substring(24);
                    }
                }
                return value;
            }
        }

        public static string Name = "Sorcerer Street Add Card";

        private string _CardName;
        private Card CardToAdd;

        public AddCardEffect()
            : base(Name, false)
        {
            _CardName = string.Empty;
        }

        public AddCardEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _CardName = string.Empty;
        }

        protected override void Load(BinaryReader BR)
        {
            _CardName = BR.ReadString();
            if (Params != null && !string.IsNullOrEmpty(_CardName))
            {
                CardToAdd = Card.LoadCard(_CardName, GameScreen.ContentFallback, Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget, Params.DicManualSkillTarget);
            }
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_CardName);
        }


        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            return "Draw " + _CardName;
        }
        protected override BaseEffect DoCopy()
        {
            AddCardEffect NewEffect = new AddCardEffect(Params);

            NewEffect._CardName = _CardName;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            AddCardEffect CopyRequirement = (AddCardEffect)Copy;

            _CardName = CopyRequirement._CardName;
        }

        #region Properties

        [Editor(typeof(CardSelector), typeof(UITypeEditor)),
        CategoryAttribute(""),
        DescriptionAttribute("The creature card path."),
        DefaultValueAttribute(0)]
        public string CardName
        {
            get
            {
                return _CardName;
            }
            set
            {
                _CardName = value;
            }
        }

        #endregion
    }
}
