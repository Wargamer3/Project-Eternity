using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class UnitUpgradesScreen : UnitListScreen
    {
        private readonly BattleMapPlayer Player;

        public UnitUpgradesScreen(BattleMapPlayer Player, Roster PlayerRoster, FormulaParser ActiveParser)
            : base(PlayerRoster, ActiveParser)
        {
            this.Player = Player;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
            else if (InputHelper.InputConfirmPressed() && ListPresentUnit.Count > 0)
            {
                GameScreen CustomizeScreen = SelectedUnit.GetCustomizeScreen(ListPresentUnit, UnitSelectionMenu.SelectedIndex, ActiveParser);

                if (CustomizeScreen == null)
                {
                    CustomizeScreen = new DefaultUnitUpgradesScreen(Player, ListPresentUnit, UnitSelectionMenu.SelectedIndex, ActiveParser);
                }

                PushScreen(CustomizeScreen);
            }
            else
            {
                UnitSelectionMenu.Update(gameTime);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            DrawMenu(g);
            g.DrawString(fntFinlanderFont, "Unit List", new Vector2(120, 10), Color.White);
        }
    }
}
