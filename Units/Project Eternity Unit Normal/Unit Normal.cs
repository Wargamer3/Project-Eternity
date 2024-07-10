using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Parts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Units.Normal
{
    public unsafe class UnitNormal : Unit
    {
        public override string UnitTypeName => "Normal";

        public UnitNormal()
        {
        }

        public UnitNormal(string RelativePath)
            : base(RelativePath)
        {

        }

        public UnitNormal(string RelativePath, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(RelativePath)
        {
            MaxCharacter = 1;
            ArrayCharacterActive = new Characters.Character[1];
            FileStream FS;
            if (File.Exists("Content/Units/Normal/" + RelativePath + ".peu"))
            {
                FS = new FileStream("Content/Units/Normal/" + RelativePath + ".peu", FileMode.Open, FileAccess.Read);
                BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
                BR.BaseStream.Seek(0, SeekOrigin.Begin);

                //Create the Part file.
                ItemName = BR.ReadString();
                SpriteMapPath = BR.ReadString();
                SpriteUnitPath = BR.ReadString();
                Model3DPath = BR.ReadString();
                UnitTags = BR.ReadString();
                Description = BR.ReadString();
                Price = BR.ReadInt32();

                _UnitStat = new UnitStats(ItemName, BR, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);

                int ListPartSlotCount = BR.ReadInt32();
                ArrayParts = new UnitPart[ListPartSlotCount];

                FS.Close();
                BR.Close();
            }
            else
            {
                IniFile UnitFile = IniFile.ReadFromFile("Content/Units/Normal/" + RelativePath + ".txt");
                Description = UnitFile.ReadField("Unit Stats", "Description");
                Price = Convert.ToInt32(UnitFile.ReadField("Unit Stats", "Price"));
                
                _UnitStat = new UnitStats(RelativePath, Content, UnitFile, DicRequirement, DicEffect, DicAutomaticSkillTarget);

                ArrayParts = new UnitPart[Convert.ToInt32(UnitFile.ReadField("Unit Stats", "Parts Slots"))];
            }

            if (Content != null)
            {
                string FinalSpriteMapPath = "\\Map Sprite\\" + RelativePath;
                if (!string.IsNullOrEmpty(SpriteMapPath))
                    FinalSpriteMapPath = "\\Map Sprite\\" + SpriteMapPath;

                string FinalSpriteUnitPath = "\\Unit Sprite\\" + RelativePath;
                if (!string.IsNullOrEmpty(SpriteUnitPath))
                    FinalSpriteUnitPath = "\\Unit Sprite\\" + SpriteUnitPath;

                string UnitDirectory = Path.GetDirectoryName("Content\\Units\\Normal\\");
                string XNADirectory = UnitDirectory.Substring(8);

                if (File.Exists(UnitDirectory + FinalSpriteMapPath + ".xnb"))
                    SpriteMap = Content.Load<Texture2D>(XNADirectory + FinalSpriteMapPath);
                else
                    SpriteMap = Content.Load<Texture2D>("Units/Default");

                if (File.Exists(UnitDirectory + FinalSpriteUnitPath + ".xnb"))
                    SpriteUnit = Content.Load<Texture2D>(XNADirectory + FinalSpriteUnitPath);
                else
                    SpriteUnit = Content.Load<Texture2D>("Units/Default");

                if (!string.IsNullOrEmpty(Model3DPath))
                {
                    string[] ArrayModelFolder = Model3DPath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    string ModelFolder = Model3DPath.Replace(ArrayModelFolder[ArrayModelFolder.Length - 1], "");
                    Unit3DModel = new AnimatedModel("Units/Normal/Unit Models/" + Model3DPath);
                    Unit3DModel.LoadContent(Content);

                    Unit3DModel.AddAnimation("Units/Normal/Unit Models/" + ModelFolder + "Walking", "Walking", Content);
                    Unit3DModel.AddAnimation("Units/Normal/Unit Models/" + ModelFolder + "Idle", "Idle", Content);
                }
            }
        }

        public override Unit FromFile(string RelativePath, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            return new UnitNormal(RelativePath, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget);
        }

        public override void ReinitializeMembers(Unit InitializedUnitBase)
        {
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

        public override GameScreens.GameScreen GetCustomizeScreen(List<Unit> ListPresentUnit, int SelectedUnitIndex, FormulaParser ActiveParser)
        {
            return null;
        }
    }
}
