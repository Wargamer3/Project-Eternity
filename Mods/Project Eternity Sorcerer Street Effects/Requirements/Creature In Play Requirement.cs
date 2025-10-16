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
    public sealed class SorcererStreetCreaturesInPlayrRequirement : SorcererStreetRequirement
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
                    List<string> Items = EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathSorcererStreetCardsCreatures, "Select Creatures", true);
                    if (Items != null)
                    {
                        string[] ArrayCreatureName = new string[Items.Count];

                        for (int C = 0; C < Items.Count; C++)
                        {
                            ArrayCreatureName[C] = Items[C].Substring(0, Items[C].Length - 4).Substring(39);
                        }

                        value = ArrayCreatureName;
                    }
                }
                return value;
            }
        }


        private string[] _ArrayCreatureName;
        private Operators.LogicOperators _LogicOperator;
        private byte _Value;

        public SorcererStreetCreaturesInPlayrRequirement()
            : this(null)
        {
        }

        public SorcererStreetCreaturesInPlayrRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street Creatures In Play", GlobalContext)
        {
            _ArrayCreatureName = new string[0];
            _LogicOperator = Operators.LogicOperators.GreaterOrEqual;
            _Value = 1;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write((byte)_ArrayCreatureName.Length);
            for (int C = 0; C < _ArrayCreatureName.Length; ++C)
            {
                BW.Write(_ArrayCreatureName[C]);
            }

            BW.Write((byte)_LogicOperator);
            BW.Write(_Value);
        }

        protected override void Load(BinaryReader BR)
        {
            byte ArrayCreatureNameCount = BR.ReadByte();
            _ArrayCreatureName = new string[ArrayCreatureNameCount];
            for (int C = 0; C < ArrayCreatureNameCount; ++C)
            {
                _ArrayCreatureName[C] = BR.ReadString();
            }

            _LogicOperator = (Operators.LogicOperators)BR.ReadByte();
            _Value = BR.ReadByte();
        }
        
        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            SorcererStreetCreaturesInPlayrRequirement NewRequirement = new SorcererStreetCreaturesInPlayrRequirement(GlobalContext);

            NewRequirement._ArrayCreatureName = new string[_ArrayCreatureName.Length];
            _ArrayCreatureName.CopyTo(NewRequirement._ArrayCreatureName, 0);
            NewRequirement._LogicOperator = _LogicOperator;
            NewRequirement._Value = _Value;

            return NewRequirement;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
            SorcererStreetCreaturesInPlayrRequirement CopyRequirement = (SorcererStreetCreaturesInPlayrRequirement)Copy;

            _ArrayCreatureName = new string[CopyRequirement._ArrayCreatureName.Length];
            CopyRequirement._ArrayCreatureName.CopyTo(_ArrayCreatureName, 0);
            _LogicOperator = CopyRequirement._LogicOperator;
            _Value = CopyRequirement._Value;
        }

        #region Properties

        [Editor(typeof(CreatureSelector), typeof(UITypeEditor)),
        CategoryAttribute(""),
        DescriptionAttribute("The creature card path."),
        DefaultValueAttribute(0)]
        public string[] CreatureNames
        {
            get
            {
                return _ArrayCreatureName;
            }
            set
            {
                _ArrayCreatureName = value;
            }
        }

        #endregion
    }
}
