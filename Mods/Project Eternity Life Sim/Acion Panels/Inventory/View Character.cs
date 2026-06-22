using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelViewCharacter : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "ViewCharacter";

        private readonly PlayerCharacter ActiveCharacter;

        public ActionPanelViewCharacter(PlayerOverseer Owner, PlayerCharacter ActiveCharacter, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, true)
        {
            this.ActiveCharacter = ActiveCharacter;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ActiveInputManager.InputRightPressed())
            {
                AddToPanelListAndSelect(new ActionPanelViewCharacterActions(Owner, ActiveCharacter, MapManager, ListActionMenuChoice));
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelViewCharacter(Owner, ActiveCharacter, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawHeader(g, ActiveCharacter);
            DrawLeftPart(g, ActiveCharacter);

            int MenuWidth = 600;
            int MenuHeight = 150;
            int MenuX = 200;
            int MenuY = 0;
            int TextX = 200;
            int TextY = 0;
            DrawHeader(g, ActiveCharacter);

            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY + 30), MenuWidth, MenuHeight, Color.White);
            MenuY = 40;
            g.Draw(ActiveCharacter.SpriteMap, new Rectangle(MenuX + 10, MenuY, 55, 125), Color.White);
            MenuX += 100;
            TextX = MenuX;
            TextY = MenuY;
            foreach (Trait ActiveTrait in ActiveCharacter.Ancestry.ListTraits)
            {
                TextHelper.DrawText(g, ActiveTrait.Name, new Vector2(TextX, TextY), Color.White);
                TextY += (int)TextHelper.fntShadowFont.MeasureString(ActiveTrait.Name).X;
            }

            TextX = MenuX;
            TextY += 30;
            TextHelper.DrawText(g, "Ancestry: " + (ActiveCharacter.Ancestry != null ? ActiveCharacter.Ancestry.Name : ""), new Vector2(TextX, TextY), Color.White);
            TextX += 250;
            TextHelper.DrawText(g, "Heritage: " + (ActiveCharacter.Heritage != null ? ActiveCharacter.Background.Name : ""), new Vector2(TextX, TextY), Color.White);
            TextX = MenuX;
            TextY += 30;
            TextHelper.DrawText(g, "Background: " + (ActiveCharacter.Background != null ? ActiveCharacter.Background.Name : ""), new Vector2(TextX, TextY), Color.White);
            TextX += 250;
            TextHelper.DrawText(g, "Class: " + (ActiveCharacter.Class != null ? ActiveCharacter.Class.Name : ""), new Vector2(TextX, TextY), Color.White);
            TextX = MenuX;
            TextY += 30;
            TextHelper.DrawText(g, "Deity: " + (ActiveCharacter.Deity != null ? ActiveCharacter.Deity.Name : ""), new Vector2(TextX, TextY), Color.White);

            MenuX = 200;
            MenuY = 180;
            MenuWidth = 600;
            MenuHeight = 40;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            MenuX += 10;
            MenuY += 10;
            TextX = MenuX;
            TextY = MenuY;
            TextHelper.DrawText(g, "Sex: " + ActiveCharacter.CharacterSex, new Vector2(TextX, TextY), Color.White);
            TextX += 250;
            TextHelper.DrawText(g, "Age: " + ActiveCharacter.Age, new Vector2(TextX, TextY), Color.White);

            MenuX = 200;
            MenuY = 220;
            MenuWidth = 600;
            MenuHeight = 70;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            MenuX += 10;
            MenuY += 10;
            TextX = MenuX;
            TextY = MenuY;
            TextHelper.DrawText(g, "Languages", new Vector2(TextX, TextY), Color.White);
            TextY += 30;
            foreach (Language ActiveLanguage in ActiveCharacter.Ancestry.ListLanguage)
            {
                TextHelper.DrawText(g, ActiveLanguage.Name, new Vector2(TextX, TextY), Color.White);
                TextY += (int)TextHelper.fntShadowFont.MeasureString(ActiveLanguage.Name).X;
            }

            MenuX = 200;
            MenuY = 290;
            MenuWidth = 600;
            MenuHeight = 70;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            MenuX += 10;
            MenuY += 10;
            TextX = MenuX;
            TextY = MenuY;
            TextHelper.DrawText(g, "Speeds", new Vector2(TextX, TextY), Color.White);
            TextY += 30;
            TextHelper.DrawText(g, "Land: " + (ActiveCharacter.Ancestry != null ? ActiveCharacter.Ancestry.Speed.ToString() : ""), new Vector2(TextX, TextY), Color.White);

            MenuX = 200;
            MenuY = 360;
            MenuWidth = 600;
            MenuHeight = 70;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            MenuX += 10;
            MenuY += 10;
            TextX = MenuX;
            TextY = MenuY;
            TextHelper.DrawText(g, "Attribute Modifiers", new Vector2(TextX, TextY), Color.White);
            TextY += 30;
            TextHelper.DrawText(g, "STR: " + ActiveCharacter.STR, new Vector2(TextX, TextY), Color.White);
            TextX += 100;
            TextHelper.DrawText(g, "DEX: " + ActiveCharacter.DEX, new Vector2(TextX, TextY), Color.White);
            TextX += 100;
            TextHelper.DrawText(g, "CON: " + ActiveCharacter.CON, new Vector2(TextX, TextY), Color.White);
            TextX += 100;
            TextHelper.DrawText(g, "INT: " + ActiveCharacter.INT, new Vector2(TextX, TextY), Color.White);
            TextX += 100;
            TextHelper.DrawText(g, "WIS: " + ActiveCharacter.WIS, new Vector2(TextX, TextY), Color.White);
            TextX += 100;
            TextHelper.DrawText(g, "CHA: " + ActiveCharacter.CHA, new Vector2(TextX, TextY), Color.White);
        }

        public static void DrawPreview(CustomSpriteBatch g, PlayerCharacter ActiveCharacter)
        {
            int MenuWidth = 200;
            int MenuHeight = 200;
            int MenuX = Constants.Width - MenuWidth;
            int MenuY = 0;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, 30, Color.White);
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY + 30), MenuWidth, MenuHeight, Color.White);
            TextHelper.DrawTextMiddleAligned(g, ActiveCharacter.Name, new Vector2(MenuX + MenuWidth / 2, MenuY + 5), Color.White);
            int TextX = MenuX + 10;
            int TextY = MenuY;
            TextY += 40;
            TextHelper.DrawText(g, "HP: 10", new Vector2(TextX, TextY), Color.White);
            TextX += 100;
            TextHelper.DrawText(g, "Level: 10", new Vector2(TextX, TextY), Color.White);
            TextX = MenuX + 10;
            TextY += 30;
            TextHelper.DrawText(g, "STR: 10", new Vector2(TextX, TextY), Color.White);
            TextX += 100;
            TextHelper.DrawText(g, "DEX: 10", new Vector2(TextX, TextY), Color.White);
            TextX = MenuX + 10;
            TextY += 30;
            TextHelper.DrawText(g, "CON: 10", new Vector2(TextX, TextY), Color.White);
            TextX += 100;
            TextHelper.DrawText(g, "INT: 10", new Vector2(TextX, TextY), Color.White);
            TextX = MenuX + 10;
            TextY += 30;
            TextHelper.DrawText(g, "WIS: 10", new Vector2(TextX, TextY), Color.White);
            TextX += 100;
            TextHelper.DrawText(g, "CHA: 10", new Vector2(TextX, TextY), Color.White);
        }

        public static void DrawHeader(CustomSpriteBatch g, PlayerCharacter ActiveCharacter)
        {
            int MenuWidth = 600;
            int MenuHeight = 150;
            int MenuX = 200;
            int MenuY = 0;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, 30, Color.White);
            TextHelper.DrawText(g, ActiveCharacter.Name, new Vector2(MenuX + 10, MenuY + 5), Color.White);
            TextHelper.DrawText(g, "Level: 10", new Vector2(MenuX + 200, MenuY + 5), Color.White);
        }

        public static void DrawLeftPart(CustomSpriteBatch g, PlayerCharacter ActiveCharacter)
        {
            int MenuWidth = 200;
            int MenuHeight = 100;
            int MenuX = 0;
            int MenuY = 0;
            int TextX = MenuX + 5;
            int TextY = MenuY + 5;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            TextHelper.DrawText(g, "Hit Points", new Vector2(TextX, TextY), Color.White);
            TextY = MenuY + 30;
            TextX = 30;
            TextHelper.DrawTextMiddleAligned(g, "Temp", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            TextHelper.DrawTextMiddleAligned(g, "HP", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            TextHelper.DrawTextMiddleAligned(g, ActiveCharacter.BonusHP.ToString(), new Vector2(TextX, TextY), Color.White);
            TextY = MenuY + 30;
            TextX = 90;
            TextHelper.DrawTextMiddleAligned(g, "Current", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            TextHelper.DrawTextMiddleAligned(g, "HP", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            TextHelper.DrawTextMiddleAligned(g, ActiveCharacter.CurrentHP.ToString(), new Vector2(TextX, TextY), Color.White);
            TextY = MenuY + 30;
            TextX = 160;
            TextHelper.DrawTextMiddleAligned(g, "Max", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            TextHelper.DrawTextMiddleAligned(g, "HP", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            TextHelper.DrawTextMiddleAligned(g, ActiveCharacter.MaxHP.ToString(), new Vector2(TextX, TextY), Color.White);

            MenuY += 100;
            TextX = MenuX + 5;
            TextY = MenuY + 5;
            MenuHeight = 100;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            TextHelper.DrawText(g, "Armor Class", new Vector2(TextX, TextY), Color.White);
            TextY = MenuY + 30;
            TextX = 30;
            TextHelper.DrawTextMiddleAligned(g, ActiveCharacter.AC.ToString(), new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            TextHelper.DrawTextMiddleAligned(g, "AC", new Vector2(TextX, TextY), Color.White);
            TextY = MenuY + 30;
            TextX = 90;
            TextHelper.DrawTextMiddleAligned(g, "0", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            TextHelper.DrawTextMiddleAligned(g, "Shield", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            TextHelper.DrawTextMiddleAligned(g, "HP", new Vector2(TextX, TextY), Color.White);
            TextY = MenuY + 30;
            TextX = 160;
            TextHelper.DrawTextMiddleAligned(g, "0", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            TextHelper.DrawTextMiddleAligned(g, "Shield", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            TextHelper.DrawTextMiddleAligned(g, "Max HP", new Vector2(TextX, TextY), Color.White);

            MenuY += 100;
            TextX = MenuX + 5;
            TextY = MenuY + 5;
            MenuHeight = 60;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            TextHelper.DrawText(g, "Perception", new Vector2(TextX, TextY), Color.White);
            TextY = MenuY + 30;
            TextX = 30;
            ProficiencyLink FoundPerception;
            ActiveCharacter.DicProficiencyLevelByName.TryGetValue("Perception", out FoundPerception);
            TextHelper.DrawText(g, FoundPerception != null ? FoundPerception.GetValue().ToString() : "0", new Vector2(TextX, TextY), Color.White);
            TextX = MenuWidth - 10;
            TextHelper.DrawTextRightAligned(g, FoundPerception != null ? FoundPerception.ProficiencyRank.ToString() : "-", new Vector2(TextX, TextY), Color.White);

            MenuY += 60;
            TextX = MenuX + 5;
            TextY = MenuY + 5;
            MenuHeight = 60;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            TextHelper.DrawText(g, "Initiative", new Vector2(TextX, TextY), Color.White);
            TextY = MenuY + 30;
            TextX = 30;
            TextHelper.DrawText(g, ActiveCharacter.GetInitiative("Perception").ToString(), new Vector2(TextX, TextY), Color.White);
            TextX = MenuWidth - 10;
            TextHelper.DrawTextRightAligned(g, "Perception", new Vector2(TextX, TextY), Color.White);

            MenuY += 60;
            TextX = MenuX + 5;
            TextY = MenuY + 5;
            MenuHeight = 180;
            GameScreen.DrawBox(g, new Vector2(MenuX, MenuY), MenuWidth, MenuHeight, Color.White);
            TextHelper.DrawText(g, "Saves", new Vector2(TextX, TextY), Color.White);
            TextY = MenuY + 30;
            TextX = 30;
            TextHelper.DrawText(g, "Fortitude", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            ProficiencyLink FoundFortitude;
            ActiveCharacter.DicProficiencyLevelByName.TryGetValue("Fortitude", out FoundFortitude);
            TextHelper.DrawText(g, FoundFortitude != null ? FoundFortitude.GetValue().ToString() : "0", new Vector2(TextX, TextY), Color.White);
            TextX = MenuWidth - 10;
            TextHelper.DrawTextRightAligned(g, FoundFortitude != null ? FoundFortitude.ProficiencyRank.ToString() : "-", new Vector2(TextX, TextY), Color.White);
            TextX = 30;
            TextY += 30;
            TextHelper.DrawText(g, "Reflex", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            ProficiencyLink FoundReflex;
            ActiveCharacter.DicProficiencyLevelByName.TryGetValue("Reflex", out FoundReflex);
            TextHelper.DrawText(g, FoundReflex != null ? FoundReflex.GetValue().ToString() : "0", new Vector2(TextX, TextY), Color.White);
            TextX = MenuWidth - 10;
            TextHelper.DrawTextRightAligned(g, FoundReflex != null ? FoundReflex.ProficiencyRank.ToString() : "-", new Vector2(TextX, TextY), Color.White);
            TextX = 30;
            TextY += 30;
            TextHelper.DrawText(g, "Will", new Vector2(TextX, TextY), Color.White);
            TextY += 20;
            ProficiencyLink FoundWill;
            ActiveCharacter.DicProficiencyLevelByName.TryGetValue("Will", out FoundWill);
            TextHelper.DrawText(g, FoundWill != null ? FoundWill.GetValue().ToString() : "0", new Vector2(TextX, TextY), Color.White);
            TextX = MenuWidth - 10;
            TextHelper.DrawTextRightAligned(g, FoundWill != null ? FoundWill.ProficiencyRank.ToString() : "-", new Vector2(TextX, TextY), Color.White);
        }
    }
}
