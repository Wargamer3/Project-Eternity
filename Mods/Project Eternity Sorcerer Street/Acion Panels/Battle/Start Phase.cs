using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleStartPhase : GameScreen
    {
        private readonly SorcererStreetMap Map;
        private readonly int ActivePlayerIndex;
        private readonly Player ActivePlayer;
        private readonly CreatureCard Invader;

        private readonly ActionPanelHolder ListActionMenuChoice;
        private int InvaderHP;
        private int DefenderHP;
        private int InvaderST;
        private int DefenderST;

        private readonly float Scale = 0.5f;
        private readonly SimpleAnimation Background;

        public ActionPanelBattleStartPhase(SorcererStreetMap Map, int ActivePlayerIndex, CreatureCard Invader)
        {
            this.Map = Map;
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            this.Invader = Invader;

            ListActionMenuChoice = new ActionPanelHolder();
        }

        public override void Load()
        {
            TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(ActivePlayer.GamePiece);

            Map.GlobalSorcererStreetBattleContext.Invader = Invader;
            Map.GlobalSorcererStreetBattleContext.Defender = ActiveTerrain.DefendingCreature;

            Map.GlobalSorcererStreetBattleContext.InvaderPlayer = ActivePlayer;
            Map.GlobalSorcererStreetBattleContext.DefenderPlayer = ActiveTerrain.Owner;

            Map.GlobalSorcererStreetBattleContext.Invader.ResetBonuses();
            Map.GlobalSorcererStreetBattleContext.Defender.ResetBonuses();

            Map.GlobalSorcererStreetBattleContext.InvaderFinalHP = Map.GlobalSorcererStreetBattleContext.Invader.CurrentHP;
            Map.GlobalSorcererStreetBattleContext.DefenderFinalHP = Map.GlobalSorcererStreetBattleContext.Defender.CurrentHP;
            Map.GlobalSorcererStreetBattleContext.InvaderFinalST = Map.GlobalSorcererStreetBattleContext.Invader.CurrentST;
            Map.GlobalSorcererStreetBattleContext.DefenderFinalST = Map.GlobalSorcererStreetBattleContext.Defender.CurrentST;

            Map.GlobalSorcererStreetBattleContext.InvaderCard = new SimpleAnimation("Invader", "Invader", Invader.sprCard);
            Map.GlobalSorcererStreetBattleContext.InvaderCard.Position = new Vector2(10, 30);
            Map.GlobalSorcererStreetBattleContext.InvaderCard.Scale = new Vector2(0.5f);
            Map.GlobalSorcererStreetBattleContext.DefenderCard = new SimpleAnimation("Defender", "Defender", ActiveTerrain.DefendingCreature.sprCard);
            Map.GlobalSorcererStreetBattleContext.DefenderCard.Position = new Vector2(Constants.Width - 210, 30);
            Map.GlobalSorcererStreetBattleContext.DefenderCard.Scale = new Vector2(0.5f);

            ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelBattleItemSelectionPhase(ListActionMenuChoice, Map, ActivePlayerIndex));
        }

        public override void Update(GameTime gameTime)
        {
            if (InvaderHP < Map.GlobalSorcererStreetBattleContext.InvaderFinalHP)
            {
                ++InvaderHP;
            }
            else if (InvaderST < Map.GlobalSorcererStreetBattleContext.InvaderFinalST)
            {
                ++InvaderST;
            }
            else if (DefenderHP < Map.GlobalSorcererStreetBattleContext.DefenderFinalHP)
            {
                ++DefenderHP;
            }
            else if (DefenderST < Map.GlobalSorcererStreetBattleContext.DefenderFinalST)
            {
                ++DefenderST;
            }
            else
            {
                ListActionMenuChoice.Last().Update(gameTime);
            }

            if (!ListActionMenuChoice.HasMainPanel)
            {
                RemoveScreen(this);
            }

            Map.GlobalSorcererStreetBattleContext.InvaderCard.Update(gameTime);
            Map.GlobalSorcererStreetBattleContext.DefenderCard.Update(gameTime);
        }

        private void ZoomInOnBackground()
        {

        }

        private void IntroduceInvader()
        {
            //Flip card from the left side
        }

        private void IntroduceDefender()
        {
            //Flip card from the right side
        }

        private void DisplayVersusText()
        {
            //Show creature attribute then the player name on the first line
            //Show the creature name on the second line
            //Show VS in the middle
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            g.GraphicsDevice.Clear(Color.Black);

            Map.GlobalSorcererStreetBattleContext.InvaderCard.BeginDraw(g);
            Map.GlobalSorcererStreetBattleContext.DefenderCard.BeginDraw(g);

            g.End();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Left Card
            Map.GlobalSorcererStreetBattleContext.InvaderCard.Draw(g);

            //Item Card
            if (Map.GlobalSorcererStreetBattleContext.InvaderItem != null)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle(10, 25, 30, 50), Color.White);
            }

            //ST Bar
            float CurrentY = 350;
            GameScreen.DrawBox(g, new Vector2(20, CurrentY), 50, 30, Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderFinalST.ToString(), new Vector2(35, CurrentY + 5), Color.Black);
            g.DrawString(Map.fntArial12, "ST", new Vector2(75, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(100, CurrentY), 200, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(105, (int)CurrentY + 5, (int)(200f * Map.GlobalSorcererStreetBattleContext.InvaderFinalST / 100f), 20), Color.Blue);

            //HP Bar
            CurrentY = 380;
            GameScreen.DrawBox(g, new Vector2(20, CurrentY), 50, 30, Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderFinalHP.ToString(), new Vector2(35, CurrentY + 5), Color.Black);
            g.DrawString(Map.fntArial12, "HP", new Vector2(75, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(100, CurrentY), 200, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(105, (int)CurrentY + 5, (int)(200f * Map.GlobalSorcererStreetBattleContext.InvaderFinalHP / 100f), 20), Color.Green);

            //Right Card
            Map.GlobalSorcererStreetBattleContext.DefenderCard.Draw(g);

            //Item Card
            if (Map.GlobalSorcererStreetBattleContext.DefenderItem != null)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 210, 25, 30, 50), Color.White);
            }

            //ST Bar
            CurrentY = 350;
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 300, CurrentY), 200, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 295, (int)CurrentY + 5, (int)(200f * Map.GlobalSorcererStreetBattleContext.DefenderFinalST / 100f), 20), Color.Blue);
            g.DrawString(Map.fntArial12, "ST", new Vector2(Constants.Width - 95, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 70, CurrentY), 50, 30, Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderFinalST.ToString(), new Vector2(Constants.Width - 55, CurrentY + 5), Color.Black);

            //HP Bar
            CurrentY = 380;
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 300, CurrentY), 200, 30, Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 295, (int)CurrentY + 5, (int)(200f * Map.GlobalSorcererStreetBattleContext.DefenderFinalHP / 100f), 20), Color.Green);
            g.DrawString(Map.fntArial12, "HP", new Vector2(Constants.Width - 95, CurrentY + 5), Color.Black);
            GameScreen.DrawBox(g, new Vector2(Constants.Width - 70, CurrentY), 50, 30, Color.White);
            g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderFinalHP.ToString(), new Vector2(Constants.Width - 55, CurrentY + 5), Color.Black);

            if (ListActionMenuChoice.HasMainPanel)
            {
                ListActionMenuChoice.Last().Draw(g);
            }
        }
    }
}
