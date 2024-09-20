using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using static ProjectEternity.GameScreens.SorcererStreetScreen.SorcererStreetBattleContext;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class PlayerSelectionScreen : GameScreen
    {
        #region Ressources

        private SpriteFont fntMenuText;

        #endregion

        private int CursorIndex;

        private SorcererStreetBattleContext Context;
        private BattleCreatureInfo Invader;
        private BattleCreatureInfo Defender;
        private SpellCard ActiveSpellCard;

        public PlayerSelectionScreen(SorcererStreetBattleContext Context, BattleCreatureInfo Invader, BattleCreatureInfo Defender, SpellCard ActiveSpellCard)
        {
            this.Context = Context;
            this.Invader = Invader;
            this.Defender = Defender;
            this.ActiveSpellCard = ActiveSpellCard;
        }

        public override void Load()
        {
            fntMenuText = Content.Load<SpriteFont>("Fonts/Arial12");
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                EnchantHelper.ActivateOnPlayer(Context, ActiveSpellCard.Spell, Invader, Defender);
                RemoveScreen(this);
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
            else if (InputHelper.InputDownPressed())
            {
            }
            else if (InputHelper.InputUpPressed())
            {
                CursorIndex -= 1;
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawBox(g, new Vector2(-5, -5), Constants.Width + 10, Constants.Height + 10, Color.White);

        }
    }
}
