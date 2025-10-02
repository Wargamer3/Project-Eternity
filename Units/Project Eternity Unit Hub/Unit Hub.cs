using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.Core.Units.Hub
{
    public class UnitHub : Unit
    {
        public override string UnitTypeName => "Hub";
        
        private readonly Unit OriginalUnit;
        public readonly string OriginalUnitName;
        private BattleParams Params;
        private int ActiveVisualNovelIndex;
        public List<string> ListVisualNovel;
        
        public UnitHub()
            : base(null)
        {
        }

        public UnitHub(BattleParams Params)
        {
            this.Params = Params;
        }

        public UnitHub(string Name, ContentManager Content, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : this(Name, Content, null, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget)
        {
        }

        public UnitHub(string Name, ContentManager Content, BattleParams Params, Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(Name)
        {
            this.Params = Params;

            ActiveVisualNovelIndex = 0;

            FileStream FS = new FileStream("Content/Deathmatch/Units/Hub/" + Name + ".peu", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            OriginalUnitName = BR.ReadString();
            if (!string.IsNullOrEmpty(OriginalUnitName))
            {
                OriginalUnit = Unit.FromFullName(OriginalUnitName, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
                _UnitStat = OriginalUnit.UnitStat;
            }
            
            int ListVisualNovelCount = BR.ReadInt32();
            ListVisualNovel = new List<string>(ListVisualNovelCount);
            for (int U = 0; U < ListVisualNovelCount; ++U)
            {
                ListVisualNovel.Add(BR.ReadString());
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
            }

            FS.Close();
            BR.Close();
        }

        public override void ReinitializeMembers(Unit InitializedUnitBase)
        {
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
            VisualNovel NewVisualNovel = new VisualNovel(ListVisualNovel[ActiveVisualNovelIndex]);
            NewVisualNovel.ListGameScreen = Params.Map.ListGameScreen;
            NewVisualNovel.Load();

            if (ActiveVisualNovelIndex + 1 < ListVisualNovel.Count)
                ++ActiveVisualNovelIndex;

            return new List<ActionPanel>() { new ActionPanelVisualNovel(ListActionMenuChoice, NewVisualNovel) };
        }

        public override Unit FromFile(string Name, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            return new UnitHub(Name, Content, Params, Params.DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);
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
            throw new NotImplementedException();
        }
    }
}
