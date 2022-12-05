using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleAttackAnimationPhase : BattleMapActionPanel
    {
        private const string PanelName = "BattleAttackAnimationPhase";

        private readonly SorcererStreetMap Map;
        private string AttackAnimationPath;
        private bool LeftSideAttackRightSide;

        private AnimationScreen AttackAnimation;

        public ActionPanelBattleAttackAnimationPhase(SorcererStreetMap Map)
            : base(PanelName, Map.ListActionMenuChoice, null, false)
        {
            this.Map = Map;
        }

        public ActionPanelBattleAttackAnimationPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map, string AttackAnimationPath, bool LeftSideAttackRightSide)
            : base(PanelName, ListActionMenuChoice, null, false)
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
            if (LeftSideAttackRightSide)
            {
                AttackAnimation = new AnimationScreen(AttackAnimationPath, Map.GlobalSorcererStreetBattleContext.Defender.sprCard, false);
                if (Map.Content != null)
                {
                    AttackAnimation.Load();
                    AttackAnimation.UpdateKeyFrame(0);
                }
                Map.GlobalSorcererStreetBattleContext.DefenderCard = new SimpleAnimation("", "", AttackAnimation);
                Map.GlobalSorcererStreetBattleContext.DefenderCard.Position.X = Constants.Width - 282;
                Map.GlobalSorcererStreetBattleContext.DefenderCard.Position.Y = 48;
                Map.GlobalSorcererStreetBattleContext.DefenderCard.Scale = new Vector2(0.6f);
            }
            else
            {
                AttackAnimation = new AnimationScreen(AttackAnimationPath, Map.GlobalSorcererStreetBattleContext.Invader.sprCard, false);
                if (Map.Content != null)
                {
                    AttackAnimation.Load();
                    AttackAnimation.UpdateKeyFrame(0);
                }

                Map.GlobalSorcererStreetBattleContext.InvaderCard = new SimpleAnimation("", "", AttackAnimation);
                Map.GlobalSorcererStreetBattleContext.InvaderCard.Position.X = 37;
                Map.GlobalSorcererStreetBattleContext.InvaderCard.Position.Y = 48;
                Map.GlobalSorcererStreetBattleContext.InvaderCard.Scale = new Vector2(0.6f);
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Map.GlobalSorcererStreetBattleContext.DefenderFinalHP > Map.GlobalSorcererStreetBattleContext.Defender.CurrentHP)
            {
                --Map.GlobalSorcererStreetBattleContext.DefenderFinalHP;
            }
            if (Map.GlobalSorcererStreetBattleContext.InvaderFinalHP > Map.GlobalSorcererStreetBattleContext.Invader.CurrentHP)
            {
                --Map.GlobalSorcererStreetBattleContext.InvaderFinalHP;
            }

            if (AttackAnimation.HasLooped)
            {
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

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
