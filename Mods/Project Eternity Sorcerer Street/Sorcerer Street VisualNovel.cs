using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetVisualNovel : VisualNovel
    {
        private readonly SorcererStreetMap Map;

        public SorcererStreetVisualNovel(SorcererStreetMap Map, string VisualNovelPath)
            : base(VisualNovelPath)
        {
            this.Map = Map;
        }

        public override void Load()
        {
            base.Load();

            fntFinlanderFont = Map.fntMenuText;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            VNBoxHeight = 340;
            //Left Character.
            if (LeftCharacter != null)
            {
                DrawCharacter(g, LeftCharacter, LeftPosition, SpriteEffects.FlipHorizontally, ActiveChar != Dialog.ActiveBustCharacterStates.Left && ActiveChar != Dialog.ActiveBustCharacterStates.Both);
            }
            //Right character.
            if (RightCharacter != null)
            {
                DrawCharacter(g, RightCharacter, RightPosition, SpriteEffects.None, ActiveChar != Dialog.ActiveBustCharacterStates.Right && ActiveChar != Dialog.ActiveBustCharacterStates.Both);
            }

            float Scale = 1.4f;
            int TextboxWidth =  1000;
            
            g.Draw(Map.sprPortraitMiddle, new Rectangle((int)(Map.sprPortraitEnd.Width * Scale + Scale), (int)(Constants.Height - Map.sprPortraitMiddle.Height * Scale), (int)(TextboxWidth * Scale), (int)(Map.sprPortraitMiddle.Height * Scale)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            g.Draw(Map.sprPortraitEnd, new Vector2((Map.sprPortraitEnd.Width + TextboxWidth) * Scale - 1 * Scale, (int)(Constants.Height - Map.sprPortraitEnd.Height * Scale)), null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 1f);
            g.Draw(Map.sprPortraitEnd, new Vector2((5) * Scale - 3 * Scale, (int)(Constants.Height - Map.sprPortraitEnd.Height * Scale)), null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.FlipHorizontally, 1f);
            DrawText(g, new Vector2(105, Constants.Height - VNBoxHeight + 10), CurrentDialog.Text);
        }
    }
}
