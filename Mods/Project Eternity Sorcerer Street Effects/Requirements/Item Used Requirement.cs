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
    public sealed class SorcererStreetUsedRequirement : SorcererStreetBattleRequirement
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
                    List<string> Items = BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathSorcererStreetCards);
                    if (Items != null)
                    {
                        value = Items[0].Substring(0, Items[0].Length - 4).Substring(24);
                    }
                }
                return value;
            }
        }

        public enum Targets { Self, Opponent }

        private string _CardName;
        private Targets _Target;

        public SorcererStreetUsedRequirement()
            : this(null)
        {
        }

        public SorcererStreetUsedRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Item Used", GlobalContext)
        {
            _CardName = string.Empty;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_CardName);
            BW.Write((byte)_Target);
        }

        protected override void Load(BinaryReader BR)
        {
            _CardName = BR.ReadString();
            _Target = (Targets)BR.ReadByte();
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetUsedRequirement NewRequirement = new SorcererStreetUsedRequirement(GlobalContext);

            NewRequirement._CardName = _CardName;
            NewRequirement._Target = _Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetUsedRequirement CopyRequirement = (SorcererStreetUsedRequirement)Copy;

            _CardName = CopyRequirement._CardName;
            _Target = CopyRequirement._Target;
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

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
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

        #endregion
    }
}
