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
    public sealed class SorcererStreetCreaturesNameRequirement : SorcererStreetRequirement
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

        public enum Targets { Self, Opponent }

        private string _CreatureName;
        private Targets _Target;

        public SorcererStreetCreaturesNameRequirement()
            : this(null)
        {
        }

        public SorcererStreetCreaturesNameRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Creature Name", GlobalContext)
        {
            _CreatureName = string.Empty;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(_CreatureName);
            BW.Write((byte)_Target);
        }

        protected override void Load(BinaryReader BR)
        {
            _CreatureName = BR.ReadString();
            _Target = (Targets)BR.ReadByte();
        }
        
        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetCreaturesNameRequirement NewRequirement = new SorcererStreetCreaturesNameRequirement(GlobalContext);

            NewRequirement._CreatureName = _CreatureName;
            NewRequirement._Target = _Target;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetCreaturesNameRequirement CopyRequirement = (SorcererStreetCreaturesNameRequirement)Copy;

            _CreatureName = CopyRequirement._CreatureName;
            _Target = CopyRequirement._Target;
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

        #endregion
    }
}
