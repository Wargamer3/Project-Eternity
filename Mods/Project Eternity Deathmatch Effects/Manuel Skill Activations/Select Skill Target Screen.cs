using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.Core.Skill
{
    public class SelectSkillTargetScreen : GameScreen
    {
        private readonly DeathmatchMap Map;
        private readonly ManualSkill ActiveSkill;
        private readonly Character SkillPilot;
        private readonly Unit SkillUnit;
        private readonly Squad SkillSquad;
        private readonly DeathmatchContext Context;
        private readonly ManualSkillTarget Owner;
        private BattlePreviewer BattlePreview;

        public SelectSkillTargetScreen(DeathmatchMap Map, ManualSkill ActiveSkill, Character SkillPilot, Unit SkillUnit, Squad SkillSquad, DeathmatchContext Context, ManualSkillTarget Owner)
        {
            this.Map = Map;
            this.ActiveSkill = ActiveSkill;
            this.SkillPilot = SkillPilot;
            this.SkillUnit = SkillUnit;
            this.SkillSquad = SkillSquad;
            this.Context = Context;
            this.Owner = Owner;
        }

        public override void Load()
        {
        }

        public override void Update(GameTime gameTime)
        {
            Map.CursorControl();
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

            if (InputHelper.InputConfirmPressed())
            {
                Squad TargetSquad = null;
                for (int P = Map.ListPlayer.Count - 1; P >= 0 && TargetSquad == null; --P)
                {
                    int SquadIndex = Map.CheckForSquadAtPosition(P, Map.CursorPosition, Vector3.Zero);
                    if (SquadIndex >= 0)
                    {
                        Context.SetContext(SkillSquad, SkillUnit, SkillPilot,
                            Context.Map.ListPlayer[P].ListSquad[SquadIndex], Context.Map.ListPlayer[P].ListSquad[SquadIndex].CurrentLeader, Context.Map.ListPlayer[P].ListSquad[SquadIndex].CurrentLeader.Pilot, Context.Map.ActiveParser);

                        Context.EffectTargetUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
                        if (ActiveSkill.CanActivateEffectsOnTarget(Context.EffectTargetCharacter.Effects))
                        {
                            TargetSquad = Map.ListPlayer[P].ListSquad[SquadIndex];
                        }
                    }
                }

                if (TargetSquad == null)
                    return;

                Context.SetContext(Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerCharacter,
                    TargetSquad, TargetSquad.CurrentLeader, TargetSquad.CurrentLeader.Pilot, Context.Map.ActiveParser);

                Owner.AddAndExecuteEffect(ActiveSkill, Context.EffectTargetCharacter.Effects);
                Context.EffectTargetUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
                SkillPilot.SP -= ActiveSkill.SPCost;

                Map.CursorPosition = SkillSquad.Position;
                Map.CursorPositionVisible = Map.CursorPosition;

                BattlePreview = null;

                if (SkillPilot.SP < ActiveSkill.SPCost)
                {
                    Map.Update(gameTime);//Remove the drawable points if needed
                    Map.RemoveScreen(this);
                }
            }
            else if (InputHelper.InputCancelPressed())
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
