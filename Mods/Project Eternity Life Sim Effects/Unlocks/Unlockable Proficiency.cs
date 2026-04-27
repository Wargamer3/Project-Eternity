using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class UnlockableProficiency : UnlcokableItemType
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
                    List<string> Items = EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimProficiencies, "Select a proficiency", false);
                    if (Items != null)
                    {
                        value = Items[0].Substring(0, Items[0].Length - 4).Substring(31);
                    }
                }
                return value;
            }
        }


        public const string UnlockableTypeName = "Proficiency";

        private ProficiencyLink Proficiency;

        public UnlockableProficiency()
            : base(UnlockableTypeName)
        {
            Proficiency = new ProficiencyLink(string.Empty);
        }

        protected UnlockableProficiency(BinaryReader BR)
            : this()
        {
            Proficiency = new ProficiencyLink(BR);
        }

        public override void DoWrite(BinaryWriter BW)
        {
            Proficiency.Write(BW);
        }

        public override void Unlock()
        {
            if (Parent.GetType() == typeof(CharacterAncestry))
            {
                Params.Owner.Ancestry.DicProficiencyLevelByName.Add(Proficiency.LinkedProficiencyRelativePath, Proficiency);
            }
            else if (Parent.GetType() == typeof(CharacterBackground))
            {
                Params.Owner.Background.DicProficiencyLevelByName.Add(Proficiency.LinkedProficiencyRelativePath, Proficiency);
            }
            else if (Parent.GetType() == typeof(CharacterClass))
            {
                Params.Owner.Class.DicProficiencyLevelByName.Add(Proficiency.LinkedProficiencyRelativePath, Proficiency);
            }
        }

        public override UnlcokableItemType Copy()
        {
            return new UnlockableProficiency();
        }

        public override UnlcokableItemType LoadCopy(BinaryReader BR)
        {
            return new UnlockableProficiency(BR);
        }

        #region Properties

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Used if the boost is a free type.")]
        public Proficiency.ProficiencyRanks ProficiencyRank
        {
            get { return Proficiency.ProficiencyRank; }
            set { Proficiency.ProficiencyRank = value; }
        }

        [Editor(typeof(CreatureSelector), typeof(UITypeEditor)),
        CategoryAttribute("Effect Attributes"),
        DescriptionAttribute("Used if the boost is a free type.")]
        public string NumberType
        {
            get { return Proficiency.LinkedProficiencyRelativePath; }
            set { Proficiency.LinkedProficiencyRelativePath = value; }
        }

        #endregion
    }
}
