using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Units.Builder
{
    public class UnitBuilder : Unit
    {
        public override string UnitTypeName => "Builder";

        private DeathmatchParams Params;
        private Unit OriginalUnit;
        public readonly string OriginalUnitName;
        public List<String> ListUnitToBuild;

        public UnitBuilder()
            : base(null)
        {
        }

        public UnitBuilder(DeathmatchParams Params)
        {
            this.Params = Params;
        }

        public UnitBuilder(string Name, ContentManager Content, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : this(Name, Content, null, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget)
        {
        }

        public UnitBuilder(string Name, ContentManager Content, DeathmatchParams Params, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(Name)
        {
            this.Params = Params;
            this.ItemName = Name;

            FileStream FS = new FileStream("Content/Deathmatch/Units/Builder/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            OriginalUnitName = BR.ReadString();
            if (!string.IsNullOrEmpty(OriginalUnitName) && DicUnitType != null)
            {
                OriginalUnit = Unit.FromFullName(OriginalUnitName, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                _UnitStat = OriginalUnit.UnitStat;
                _HP = OriginalUnit.MaxHP;
                _EN = OriginalUnit.MaxEN;
            }

            int UnitsToBuildCount = BR.ReadInt32();
            ListUnitToBuild = new List<String>(UnitsToBuildCount);
            for (int U = 0; U < UnitsToBuildCount; ++U)
            {
                ListUnitToBuild.Add(BR.ReadString());
            }

            if (Content != null)
            {
                string UnitDirectory = Path.GetDirectoryName("Content/Deathmatch/Units/Normal/" + Name);
                string XNADirectory = UnitDirectory.Substring(8);

                if (File.Exists(UnitDirectory + "\\Map Sprite\\" + Name + ".xnb"))
                    SpriteMap = Content.Load<Texture2D>(XNADirectory + "\\Map Sprite\\" + this.RelativePath);
                else
                    SpriteMap = Content.Load<Texture2D>("Deathmatch/Units/Default");

                if (File.Exists(UnitDirectory + "\\Unit Sprite\\" + Name + ".xnb"))
                    SpriteUnit = Content.Load<Texture2D>(XNADirectory + "\\Unit Sprite\\" + this.RelativePath);
                else
                    SpriteUnit = Content.Load<Texture2D>("Deathmatch/Units/Default");
            }

            FS.Close();
            BR.Close();
        }

        public override void ReinitializeMembers(Unit InitializedUnitBase)
        {
            UnitBuilder Other = (UnitBuilder)InitializedUnitBase;
            Params = Other.Params;

            if (OriginalUnit == null)
            {
                OriginalUnit = FromFullName(OriginalUnitName, Params.Map.Content, Params.DicUnitType, Params.DicRequirement, Params.DicEffect, Params.DicAutomaticSkillTarget);
                _UnitStat = OriginalUnit.UnitStat;
                _HP = OriginalUnit.MaxHP;
                _EN = OriginalUnit.MaxEN;
            }
        }

        public override void OnTurnEnd(int ActivePlayerIndex, Squad ActiveSquad)
        {
        }

        public override List<ActionPanel> OnMenuMovement(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            return null;
        }

        public override List<ActionPanel> OnMenuSelect(int ActivePlayerIndex, Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            return new List<ActionPanel>() { new ActionPanelBuild(Params.Map, this) };
        }

        public override Unit FromFile(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            if (Params == null || Params.Map == null)
            {
                return new UnitBuilder(Name, Content, null, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
            else
            {
                return new UnitBuilder(Name, Content, Params, Params.DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected override void DoQuickLoad(BinaryReader BR, ContentManager Content)
        {
        }

        public override void DoInit()
        {
        }

        public override GameScreen GetCustomizeScreen(List<Unit> ListPresentUnit, int SelectedUnitIndex, FormulaParser ActiveParser)
        {
            return null;
        }
    }
}
