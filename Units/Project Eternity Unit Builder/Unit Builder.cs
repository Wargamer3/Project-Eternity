using System;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Units.Builder
{
    public class UnitBuilder : Unit
    {
        public override string UnitTypeName => "Builder";

        private DeathmatchMap Map;
        private Unit OriginalUnit;
        public readonly string OriginalUnitName;
        public List<String> ListUnitToBuild;

        public UnitBuilder()
            : base(null)
        {
        }

        public UnitBuilder(DeathmatchMap Map)
        {
            this.Map = Map;
        }

        public UnitBuilder(string Name, ContentManager Content, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
            : this(Name, Content, null, DicUnitType, DicRequirement, DicEffect)
        {
        }

        public UnitBuilder(string Name, ContentManager Content, DeathmatchMap Map, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
            : base(Name)
        {
            this.Map = Map;

            FileStream FS = new FileStream("Content/Units/Builder/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            OriginalUnitName = BR.ReadString();
            if (!string.IsNullOrEmpty(OriginalUnitName) && DicUnitType != null)
            {
                OriginalUnit = Unit.FromFullName(OriginalUnitName, Content, DicUnitType, DicRequirement, DicEffect);
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
                string UnitDirectory = Path.GetDirectoryName("Content\\Units\\Normal\\" + Name);
                string XNADirectory = UnitDirectory.Substring(8);

                if (File.Exists(UnitDirectory + "\\Map Sprite\\" + Name + ".xnb"))
                    SpriteMap = Content.Load<Texture2D>(XNADirectory + "\\Map Sprite\\" + this.RelativePath);
                else
                    SpriteMap = Content.Load<Texture2D>("Units/Default");

                if (File.Exists(UnitDirectory + "\\Unit Sprite\\" + Name + ".xnb"))
                    SpriteUnit = Content.Load<Texture2D>(XNADirectory + "\\Unit Sprite\\" + this.RelativePath);
                else
                    SpriteUnit = Content.Load<Texture2D>("Units/Default");
            }

            FS.Close();
            BR.Close();
        }

        public override void ReinitializeMembers(Unit InitializedUnitBase)
        {
            UnitBuilder Other = (UnitBuilder)InitializedUnitBase;
            Map = Other.Map;

            if (OriginalUnit == null)
            {
                OriginalUnit = FromFullName(OriginalUnitName, Map.Content, Map.DicUnitType, Map.DicRequirement, Map.DicEffect);
                _UnitStat = OriginalUnit.UnitStat;
                _HP = OriginalUnit.MaxHP;
                _EN = OriginalUnit.MaxEN;
            }
        }

        public override void OnTurnEnd(Squad ActiveSquad)
        {
        }

        public override List<ActionPanel> OnMenuMovement(Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            return null;
        }

        public override List<ActionPanel> OnMenuSelect(Squad ActiveSquad, ActionPanelHolder ListActionMenuChoice)
        {
            return new List<ActionPanel>() { new ActionPanelBuild(Map, this) };
        }

        public override Unit FromFile(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
        {
            if (Map == null)
            {
                return new UnitBuilder(Name, Content, null, DicRequirement, DicEffect);
            }
            else
            {
                return new UnitBuilder(Name, Content, Map, Map.DicUnitType, DicRequirement, DicEffect);
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

        public override GameScreen GetCustomizeScreen()
        {
            return null;
        }
    }
}
