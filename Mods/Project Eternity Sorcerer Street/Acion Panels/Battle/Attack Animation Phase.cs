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
            : base(PanelName, Map.ListActionMenuChoice, false)
        {
            this.Map = Map;
        }

        public ActionPanelBattleAttackAnimationPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map, string AttackAnimationPath, bool LeftSideAttackRightSide)
            : base(PanelName, ListActionMenuChoice, false)
        {
            this.Map = Map;
            this.AttackAnimationPath = AttackAnimationPath;
            this.LeftSideAttackRightSide = LeftSideAttackRightSide;
        }

        public override void OnSelect()
        {
            if (LeftSideAttackRightSide)
            {
                AttackAnimation = new AnimationScreen(AttackAnimationPath, Map.GlobalSorcererStreetBattleContext.Invader.sprCard, false);
                AttackAnimation.Load();
                Map.GlobalSorcererStreetBattleContext.DefenderCard = new SimpleAnimation("", "", AttackAnimation);
                Map.GlobalSorcererStreetBattleContext.DefenderCard.Position.X = Constants.Width - 210;
                Map.GlobalSorcererStreetBattleContext.DefenderCard.Position.Y = 30;
                Map.GlobalSorcererStreetBattleContext.DefenderCard.Scale = new Vector2(0.5f);
            }
            else
            {
                AttackAnimation = new AnimationScreen(AttackAnimationPath, Map.GlobalSorcererStreetBattleContext.Defender.sprCard, false);
                AttackAnimation.Load();
                Map.GlobalSorcererStreetBattleContext.InvaderCard = new SimpleAnimation("", "", AttackAnimation);
                Map.GlobalSorcererStreetBattleContext.InvaderCard.Position.X = 10;
                Map.GlobalSorcererStreetBattleContext.InvaderCard.Position.Y = 30;
                Map.GlobalSorcererStreetBattleContext.InvaderCard.Scale = new Vector2(0.5f);
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (AttackAnimation.HasLooped)
            {
                FinishPhase();
            }
        }

        public void FinishPhase()
        {
            RemoveFromPanelList(this);
            Map.GlobalSorcererStreetBattleContext.InvaderCard = new SimpleAnimation("Invader", "Invader", Map.GlobalSorcererStreetBattleContext.Invader.sprCard);
            Map.GlobalSorcererStreetBattleContext.InvaderCard.Position = new Vector2(10, 30);
            Map.GlobalSorcererStreetBattleContext.InvaderCard.Scale = new Vector2(0.5f);
            Map.GlobalSorcererStreetBattleContext.DefenderCard = new SimpleAnimation("Defender", "Defender", Map.GlobalSorcererStreetBattleContext.Defender.sprCard);
            Map.GlobalSorcererStreetBattleContext.DefenderCard.Position = new Vector2(Constants.Width - 210, 30);
            Map.GlobalSorcererStreetBattleContext.DefenderCard.Scale = new Vector2(0.5f);
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
