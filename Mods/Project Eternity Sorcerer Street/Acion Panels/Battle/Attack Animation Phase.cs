using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleAttackAnimationPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleAttackAnimationPhase";

        private string AttackAnimationPath;
        private bool LeftSideAttackRightSide;

        private AnimationScreen AttackAnimation;

        public ActionPanelBattleAttackAnimationPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
            this.Map = Map;
        }

        public ActionPanelBattleAttackAnimationPhase(SorcererStreetMap Map, string AttackAnimationPath, bool LeftSideAttackRightSide)
            : base(Map, PanelName)
        {
            this.Map = Map;
            this.AttackAnimationPath = AttackAnimationPath;
            this.LeftSideAttackRightSide = LeftSideAttackRightSide;

            if (string.IsNullOrEmpty(AttackAnimationPath))
            {
                this.AttackAnimationPath = "Sorcerer Street/Default";
            }
        }

        public override void OnSelect()
        {
            AttackAnimation = InitAnimation(LeftSideAttackRightSide, AttackAnimationPath, Map.GlobalSorcererStreetBattleContext, Map.Content);
        }

        public static AnimationScreen InitAnimation(bool LeftSideAttackRightSide, string AttackAnimationPath, SorcererStreetBattleContext GlobalSorcererStreetBattleContext, ContentManager Content)
        {
            AnimationScreen AttackAnimation;

            if (LeftSideAttackRightSide)
            {
                AttackAnimation = new AnimationScreen(AttackAnimationPath, GlobalSorcererStreetBattleContext.Defender.Creature.sprCard, false);
                if (Content != null)
                {
                    AttackAnimation.Load();
                    AttackAnimation.UpdateKeyFrame(0);
                }
                GlobalSorcererStreetBattleContext.Defender.Animation = new SimpleAnimation("", "", AttackAnimation);
                GlobalSorcererStreetBattleContext.Defender.Animation.Position = new Vector2(Constants.Width - GlobalSorcererStreetBattleContext.Defender.Creature.sprCard.Width - Constants.Width / 9, Constants.Height / 12);
                GlobalSorcererStreetBattleContext.Defender.Animation.Scale = new Vector2(1f);
            }
            else
            {
                AttackAnimation = new AnimationScreen(AttackAnimationPath, GlobalSorcererStreetBattleContext.Invader.Creature.sprCard, false);
                if (Content != null)
                {
                    AttackAnimation.Load();
                    AttackAnimation.UpdateKeyFrame(0);
                }

                GlobalSorcererStreetBattleContext.Invader.Animation = new SimpleAnimation("", "", AttackAnimation);
                GlobalSorcererStreetBattleContext.Invader.Animation.Position = new Vector2(Constants.Width / 9, Constants.Height / 12);
                GlobalSorcererStreetBattleContext.Invader.Animation.Scale = new Vector2(1f);
            }

            return AttackAnimation;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!UpdateAnimations(gameTime, Map.GlobalSorcererStreetBattleContext, AttackAnimation))
            {
                FinishPhase();
            }
        }

        public static bool UpdateAnimations(GameTime gameTime, SorcererStreetBattleContext GlobalSorcererStreetBattleContext, AnimationScreen AttackAnimation)
        {
            if (!CanUpdate(gameTime, GlobalSorcererStreetBattleContext))
                return true;

            if (GlobalSorcererStreetBattleContext.Defender.FinalHP > GlobalSorcererStreetBattleContext.Defender.Creature.CurrentHP)
            {
                --GlobalSorcererStreetBattleContext.Defender.FinalHP;
            }
            if (GlobalSorcererStreetBattleContext.Invader.FinalHP > GlobalSorcererStreetBattleContext.Invader.Creature.CurrentHP)
            {
                --GlobalSorcererStreetBattleContext.Invader.FinalHP;
            }

            if (AttackAnimation.HasLooped)
            {
                return false;
            }

            return true;
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
