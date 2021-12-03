using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class DataLoadScreen : GameScreen
    {
        private SpriteFont fntFinlanderFont;

        public Dictionary<string, Unit> DicUnitType;
        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, BaseEffect> DicEffect;
        public Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget;
        public Dictionary<string, ManualSkillTarget> DicManualSkillTarget;

        private readonly Roster PlayerRoster;
        private double TimeSinceSaveInSeconds;

        public DataLoadScreen(Roster PlayerRoster)
        {
            this.PlayerRoster = PlayerRoster;
            TimeSinceSaveInSeconds = 0;
        }

        public override void Load()
        {
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");

            DicUnitType = Unit.LoadAllUnits();
            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();
            DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();
            DicManualSkillTarget = ManualSkillTarget.LoadAllTargetTypes();
        }

        public override void Update(GameTime gameTime)
        {
            if (TimeSinceSaveInSeconds == 0)
            {
                DataScreen.LoadProgression(PlayerRoster, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                TimeSinceSaveInSeconds = gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                TimeSinceSaveInSeconds += gameTime.ElapsedGameTime.TotalSeconds;

                if (KeyboardHelper.InputConfirmPressed() || KeyboardHelper.InputCancelPressed() || MouseHelper.InputLeftButtonPressed() || MouseHelper.InputRightButtonPressed() || TimeSinceSaveInSeconds > 3)
                {
                    RemoveScreen(this);
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X = Constants.Width * 0.1f;
            float Y = Constants.Height * 0.4f;
            GameScreen.DrawBox(g, new Vector2(X, Y), (int)(Constants.Width * 0.8), (int)(Constants.Height * 0.1), Color.White);

            if (TimeSinceSaveInSeconds == 0)
            {
                g.DrawStringCentered(fntFinlanderFont, "Loading", new Vector2(Constants.Width / 2, Y + 20), Color.White);
            }
            else
            {
                g.DrawStringCentered(fntFinlanderFont, "Game Loaded", new Vector2(Constants.Width / 2, Y + 20), Color.White);
            }
        }
    }
}
