using System;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Units.Normal;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Units.MultiForm
{
    public class UnitMultiForm : Unit
    {
        public override string UnitTypeName => "Multi-Form";

        public struct EquipmentInformations
        {
            public UnitNormal UnitForm;
            public string EquipmentUnitPath;
            public string EquipmentName;

            public EquipmentInformations(string EquipmentUnitPath, string EquipmentName)
            {
                UnitForm = null;
                this.EquipmentUnitPath = EquipmentUnitPath;
                this.EquipmentName = EquipmentName;
            }

            public override string ToString()
            {
                return EquipmentUnitPath;
            }
        }

        public EquipmentInformations[] ArrayUnitStat;
        public int ActiveUnitIndex;

        private double HPPercentage;
        private double ENPercentage;
        private string OriginalName;

        public UnitMultiForm()
        { }

        public UnitMultiForm(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
            : base(Name)
        {
            this.OriginalName = Name;
            HPPercentage = 1;
            ENPercentage = 1;
            ActiveUnitIndex = 0;
            ArrayCharacterActive = new Character[0];
            MaxCharacter = 1;
            ArrayParts = new Parts.UnitPart[0];

            FileStream FS = new FileStream("Content/Units/Multi-Form/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            int ListTransformingUnitCount = BR.ReadInt32();

            ArrayUnitStat = new EquipmentInformations[ListTransformingUnitCount];

            for (int U = 0; U < ListTransformingUnitCount; ++U)
            {
                string EquipmentUnitPath = BR.ReadString();
                string EquipmentName = BR.ReadString();

                ArrayUnitStat[U] = new EquipmentInformations();
                ArrayUnitStat[U].EquipmentName = EquipmentName;

                ArrayUnitStat[U].UnitForm = new UnitNormal(EquipmentUnitPath, Content, DicRequirement, DicEffect);
            }

            _UnitStat = ArrayUnitStat[0].UnitForm.UnitStat;
            _HP = ArrayUnitStat[0].UnitForm.MaxHP;
            _EN = ArrayUnitStat[0].UnitForm.MaxEN;
            SpriteMap = ArrayUnitStat[0].UnitForm.SpriteMap;
            SpriteUnit = ArrayUnitStat[0].UnitForm.SpriteUnit;

            FS.Close();
            BR.Close();
        }

        public override void ReinitializeMembers(Unit InitializedUnitBase)
        {
        }

        public void ChangeUnit(int ActiveUnit)
        {//Used to avoid updating HP, EN on Init.
            if (this.ActiveUnitIndex != ActiveUnit)
            {
                HPPercentage = HP / (double)MaxHP;
                ENPercentage = EN / (double)MaxEN;
                this.ActiveUnitIndex = ActiveUnit;
            }
            
            _UnitStat = ArrayUnitStat[ActiveUnit].UnitForm.UnitStat;
            SpriteMap = ArrayUnitStat[ActiveUnit].UnitForm.SpriteMap;
            SpriteUnit = ArrayUnitStat[ActiveUnit].UnitForm.SpriteUnit;

            _HP = (int)(MaxHP * HPPercentage);
            _EN = (int)(MaxEN * ENPercentage);
        }

        public override Unit FromFile(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
        {
            return new UnitMultiForm(Name, Content, DicRequirement, DicEffect);
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
            BW.Write(ActiveUnitIndex);
        }

        protected override void DoQuickLoad(BinaryReader BR, ContentManager Content)
        {
            ActiveUnitIndex = BR.ReadInt32();
            
            ChangeUnit(ActiveUnitIndex);
        }

        public override void DoInit()
        {
            ChangeUnit(ActiveUnitIndex);
        }

        public override GameScreens.GameScreen GetCustomizeScreen()
        {
            return null;
        }
    }
}
