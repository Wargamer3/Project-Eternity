using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FMOD;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class ShopCharactersScreen : ShopItemBaseScreen
    {
        #region Ressources

        private AnimatedSprite BuyCharacterIcon;

        #endregion

        private readonly List<CharacterMenuEquipment> ListShopCharacter;

        public ShopCharactersScreen(Player Owner)
            : base(Owner)
        {
            ListShopCharacter = new List<CharacterMenuEquipment>();
        }

        public override void Load()
        {
            base.Load();

            ListShopCharacter.Add(new CharacterMenuEquipment("Soul", 100, null, Content.Load<Texture2D>("Triple Thunder/Menus/Shop/Icons/Player Soul Portrait")));

            #region Ressources

            BuyCharacterIcon = new AnimatedSprite(Content, "Triple Thunder/Menus/Shop/Buy Character Icon", Vector2.Zero, 0, 1, 4);

            #endregion
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!IsDragDropActive)
            {
                CharacterMenuEquipment SelectedCharacter = GetShopCharacterUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
                if (SelectedCharacter != null && InputHelper.InputConfirmPressed())
                {
                    PushScreen(new BuyCharacter(SelectedCharacter, Owner));
                }

            }
        }

        #region Buttons Callback


        #endregion

        private CharacterMenuEquipment GetShopCharacterUnderMouse(int X, int Y)
        {
            if (X >= 34 && X <= 312 && Y >= 127)
            {
                int CharacterIndex = (Y - 152) / 60;
                if (CharacterIndex < ListShopCharacter.Count)
                {
                    return ListShopCharacter[CharacterIndex];
                }
            }

            return null;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);

            for (int C = 0; C < ListShopCharacter.Count; ++C)
            {
                CharacterMenuEquipment SelectedCharacter = GetShopCharacterUnderMouse(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);
                if (SelectedCharacter == ListShopCharacter[C])
                {
                    BuyCharacterIcon.SetFrame(2);
                }
                else
                {

                    BuyCharacterIcon.SetFrame(0);
                }
                BuyCharacterIcon.Draw(g, new Vector2(172, 152 + C * 60), Color.White);
                g.DrawString(fntText, ListShopCharacter[C].Name, new Vector2(102, 131 + C * 60), Color.White);
                g.DrawString(fntText, "5", new Vector2(113, 155 + C * 60), Color.White);
                g.DrawString(fntText, "5", new Vector2(145, 155 + C * 60), Color.White);
                g.DrawString(fntText, "4", new Vector2(177, 155 + C * 60), Color.White);
                g.DrawStringRightAligned(fntText, ListShopCharacter[C].Price + " CR", new Vector2(297, 155 + C * 60), Color.White);
            }
        }
    }
}
