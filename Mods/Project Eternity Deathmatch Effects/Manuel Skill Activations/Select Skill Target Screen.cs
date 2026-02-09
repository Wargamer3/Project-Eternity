using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Skill
{
    public class SelectSkillTargetScreen : GameScreen
    {
        private readonly DeathmatchMap Map;
        private readonly ManualSkill ActiveSkill;
        private readonly Character SkillPilot;
        private readonly Unit SkillUnit;
        private readonly Squad SkillSquad;
        private readonly DeathmatchParams Params;
        private readonly ManualSkillTarget Owner;
        private BattlePreviewer BattlePreview;

        public SelectSkillTargetScreen(DeathmatchMap Map, ManualSkill ActiveSkill, Character SkillPilot, Unit SkillUnit, Squad SkillSquad, DeathmatchParams Params, ManualSkillTarget Owner)
        {
            this.Map = Map;
            this.ActiveSkill = ActiveSkill;
            this.SkillPilot = SkillPilot;
            this.SkillUnit = SkillUnit;
            this.SkillSquad = SkillSquad;
            this.Params = Params;
            this.Owner = Owner;
        }

        public override void Load()
        {
        }

        public override void Update(GameTime gameTime)
        {
            Map.CursorControl(gameTime, Map.ListPlayer[Map.ActivePlayerIndex].InputManager);
            Map.UpdateCursorVisiblePosition(gameTime);
            //Loop through the players to find a Unit to control.
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                //Find if a current player Unit is under the cursor.
                int CursorSelect = Map.CheckForSquadAtPosition(P, Map.CursorPosition, Vector3.Zero);

                if (CursorSelect >= 0)
                {
                    if (BattlePreview == null)
                    {
                        BattlePreview = new BattlePreviewer(Map, P, CursorSelect, null);
                    }
                    BattlePreview.UpdateUnitDisplay();
                }
            }

            if (Map.ListPlayer[Map.ActivePlayerIndex].InputManager.InputConfirmPressed())
            {
                Squad TargetSquad = null;
                for (int P = Map.ListPlayer.Count - 1; P >= 0 && TargetSquad == null; --P)
                {
                    int SquadIndex = Map.CheckForSquadAtPosition(P, Map.CursorPosition, Vector3.Zero);
                    if (SquadIndex >= 0)
                    {
                        Params.GlobalContext.SetContext(SkillSquad, SkillUnit, SkillPilot,
                            Params.Map.ListPlayer[P].ListSquad[SquadIndex], Params.Map.ListPlayer[P].ListSquad[SquadIndex].CurrentLeader, Params.Map.ListPlayer[P].ListSquad[SquadIndex].CurrentLeader.Pilot, Params.ActiveParser);

                        Params.GlobalContext.EffectTargetUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
                        if (ActiveSkill.CanActivateEffectsOnTarget(Params.GlobalContext.EffectTargetCharacter.Effects))
                        {
                            TargetSquad = Map.ListPlayer[P].ListSquad[SquadIndex];
                        }
                    }
                }

                if (TargetSquad == null)
                    return;

                Params.GlobalContext.SetContext(Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter,
                    TargetSquad, TargetSquad.CurrentLeader, TargetSquad.CurrentLeader.Pilot, Params.ActiveParser);

                Owner.AddAndExecuteEffect(ActiveSkill, Params.GlobalContext.EffectTargetCharacter.Effects, SkillEffect.LifetimeTypeTurns + Params.Map.ActivePlayerIndex);
                Params.GlobalContext.EffectTargetUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
                SkillPilot.SP -= ActiveSkill.ActivationCost;

                Map.CursorPosition = SkillSquad.Position;
                Map.CursorPositionVisible = Map.CursorPosition;

                BattlePreview = null;

                if (SkillPilot.SP < ActiveSkill.ActivationCost)
                {
                    Map.Update(gameTime);//Remove the drawable points if needed
                    Map.RemoveScreen(this);
                }
            }
            else if (Map.ListPlayer[Map.ActivePlayerIndex].InputManager.InputCancelPressed())
            {
                Map.CursorPosition = SkillSquad.Position;
                Map.CursorPositionVisible = Map.CursorPosition;
                Map.Update(gameTime);//Remove the drawable points if needed
                Map.RemoveScreen(this);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawBox(g, new Vector2(0, 0), 120, 40, Color.White);
            g.DrawString(Map.fntArial12, "Select a Unit", new Vector2(10, 10), Color.White);
            if (BattlePreview != null)
            {
                BattlePreview.DrawDisplayUnit(g);
            }
        }
    }
}
