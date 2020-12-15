using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class UnitUpgradesScreen : UnitListScreen
    {
        private List<Unit> ListPresentUnit;

        public UnitUpgradesScreen(Roster PlayerRoster)
            : base(PlayerRoster)
        {
        }

        public override void Load()
        {
            base.Load();

            ListPresentUnit = PlayerRoster.TeamUnits.GetPresent();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputCancelPressed())
            {
                RemoveScreen(this);
            }
            else if (InputHelper.InputConfirmPressed() && ListPresentUnit.Count > 0)
            {
                GameScreen CustomizeScreen = SelectedUnit.GetCustomizeScreen();

                if (CustomizeScreen == null)
                    CustomizeScreen = new DefaultUnitUpgradesScreen(SelectedUnit);

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
