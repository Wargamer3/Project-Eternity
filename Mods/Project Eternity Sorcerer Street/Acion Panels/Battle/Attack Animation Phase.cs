using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.AnimationScreen;
using static ProjectEternity.GameScreens.SorcererStreetScreen.SorcererStreetBattleContext;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleAttackAnimationPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleAttackAnimationPhase";

        private BattleCreatureInfo Defender;
        private string AttackAnimationPath;
        private bool LeftSideAttackRightSide;

        private SimpleAnimation AttackAnimation;

        public ActionPanelBattleAttackAnimationPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
            this.Map = Map;
        }

        public ActionPanelBattleAttackAnimationPhase(SorcererStreetMap Map, BattleCreatureInfo Defender, string AttackAnimationPath, bool LeftSideAttackRightSide)
            : base(Map, PanelName)
        {
            this.Map = Map;
            this.Defender = Defender;
            this.AttackAnimationPath = AttackAnimationPath;
            this.LeftSideAttackRightSide = LeftSideAttackRightSide;

            if (string.IsNullOrEmpty(AttackAnimationPath))
            {
                this.AttackAnimationPath = "Sorcerer Street/Default";
            }
        }

        public override void OnSelect()
        {
            AttackAnimation = InitAnimation(LeftSideAttackRightSide, AttackAnimationPath, Defender, Map.Content != null);
            if (LeftSideAttackRightSide)
            {
                Map.GlobalSorcererStreetBattleContext.DefenderCreature.Animation = AttackAnimation;
                Map.GlobalSorcererStreetBattleContext.DefenderCreature.Animation.Position = new Vector2(Constants.Width - Map.GlobalSorcererStreetBattleContext.DefenderCreature.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
                Map.GlobalSorcererStreetBattleContext.DefenderCreature.Animation.Scale = new Vector2(1f);
            }
            else
            {
                Map.GlobalSorcererStreetBattleContext.InvaderCreature.Animation = AttackAnimation;
                Map.GlobalSorcererStreetBattleContext.InvaderCreature.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
                Map.GlobalSorcererStreetBattleContext.InvaderCreature.Animation.Scale = new Vector2(1f);
            }
        }

        public static SimpleAnimation InitAnimation(bool AnimationOnRight, string AttackAnimationPath, BattleCreatureInfo User, bool PreloadAnimation)
        {
            AnimationScreen AttackAnimation;

            AttackAnimation = new AnimationScreen(AttackAnimationPath, User, false);
            if (!User.Creature.UseCardAnimation)
            {
                AttackAnimation.HorizontalMirror = AnimationOnRight;
            }
            if (PreloadAnimation)
            {
                AttackAnimation.Load();
                AttackAnimation.UpdateKeyFrame(0);
            }
            SimpleAnimation NewAnimation = new SimpleAnimation("", "", AttackAnimation);
            NewAnimation.Scale = new Vector2(1f);

            if (AnimationOnRight)
            {
                NewAnimation.Position = new Vector2(Constants.Width - User.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
            }
            else
            {
                NewAnimation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
            }

            return NewAnimation;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!HasFinishedUpdatingBars(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            if (Map.GlobalSorcererStreetBattleContext.SelfCreature != null && !Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation.HasEnded)
            {
                Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation.Update(gameTime);
            }
            if (Map.GlobalSorcererStreetBattleContext.OpponentCreature != null && !Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation.HasEnded)
            {
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation.Update(gameTime);
            }

            if (AttackAnimation.HasEnded)
            {
                if (Defender.FinalHP < Defender.Creature.CurrentHP)
                {
                    Defender.Creature.CurrentHP = Defender.FinalHP;
                }

                FinishPhase();
            }
        }

        public void FinishPhase()
        {
            RemoveFromPanelList(this);
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            AttackAnimationPath = BR.ReadString();
            LeftSideAttackRightSide = BR.ReadBoolean();
            OnSelect();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendString(AttackAnimationPath);
            BW.AppendBoolean(LeftSideAttackRightSide);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleAttackAnimationPhase(Map);
        }
    }
}
