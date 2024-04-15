using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelMovePart2 : ActionPanelDeathmatch
    {
        private const string PanelName = "Move2";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad ActiveSquad;
        private bool IsPostAttack;
        private Vector3 CursorPosition;
        private List<Vector3> ListMVHoverPoint;
        private bool HasFocus;

        public ActionPanelMovePart2(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelMovePart2(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, bool IsPostAttack, List<Vector3> ListMVHoverPoint)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.IsPostAttack = IsPostAttack;
            this.ListMVHoverPoint = ListMVHoverPoint;

            CursorPosition = Map.CursorPosition;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            HasFocus = true;
        }

        public override void OnSelect()
        {
            ListNextChoice.Clear();
            if (IsPostAttack)
            {
                AddChoiceToCurrentPanel(new ActionPanelWait(Map, ActiveSquad, ListMVHoverPoint));
            }
            else
            {
                Map.UpdateAllAttacks(ActiveSquad.CurrentLeader, Map.CursorPosition, Map.ListPlayer[Map.ActivePlayerIndex].TeamIndex, false);

                if (ActiveSquad.CurrentLeader.CanAttack)
                {
                    AddChoiceToCurrentPanel(new ActionPanelAttackPart1(Map, ActivePlayerIndex, ActiveSquadIndex, false, ListMVHoverPoint));
                }

                if (ActiveSquad.CurrentLeader.Boosts.PostMovementModifier.Spirit)
                {
                    AddChoiceToCurrentPanel(new ActionPanelSpirit(Map, ActiveSquad));
                }

                ActiveSquad.CurrentLeader.OnMenuMovement(ActivePlayerIndex, ActiveSquad, Map.ListActionMenuChoice);

                int SquadIndex = Map.CheckForSquadAtPosition(Map.ActivePlayerIndex, Map.CursorPosition, Vector3.Zero);
                if (SquadIndex >= 0 && Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex] != ActiveSquad)
                {
                    AddChoiceToCurrentPanel(new ActionPanelBoard(Map, Map.ListPlayer[Map.ActivePlayerIndex].ListSquad[SquadIndex], ActiveSquad));
                }
                else
                {
                    AddChoiceToCurrentPanel(new ActionPanelWait(Map, ActiveSquad, ListMVHoverPoint));
                }

                AddPropPanelsOnUnitStop(ActiveSquad);
                ActionPanelRepair.AddIfUsable(Map, this, ActiveSquad);
                ActionPanelGetInVehicle.AddIfUsable(Map, this, ActiveSquad, ListMVHoverPoint);
            }

            if (ActiveSquad.Position != Map.CursorPosition)
            {
                Map.MovementAnimation.Add(ActiveSquad, ActiveSquad.Position, ListMVHoverPoint);

                ActiveSquad.SetPosition(Map.CursorPosition);

                Map.CursorPosition = ActiveSquad.Position;
                Map.CursorPositionVisible = Map.CursorPosition;
            }
        }

        private void AddPropPanelsOnUnitStop(Squad StoppedUnit)
        {
            foreach (InteractiveProp ActiveProp in Map.LayerManager[(int)ActiveSquad.Position.Z].ListProp)
            {
                ActiveProp.OnUnitBeforeStop(this, StoppedUnit, CursorPosition);
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            HasFocus = true;
            if (NavigateThroughNextChoices(Map.sndSelection))
            {
                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }
            else if (ConfirmNextChoices(Map.sndConfirm))
            {
                HasFocus = false;
            }
        }

        public override void DoRead(ByteReader BR)
        {
            HasFocus = BR.ReadBoolean();
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            CursorPosition = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat());
            Map.CursorPosition = CursorPosition;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];

            int ListMVHoverPointCount = BR.ReadInt32();
            ListMVHoverPoint = new List<Vector3>(ListMVHoverPointCount);
            for (int M = 0; M < ListMVHoverPointCount; ++M)
            {
                ListMVHoverPoint.Add(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat()));
            }

            if (HasFocus)
            {
                Map.MovementAnimation.Skip();
                Map.MovementAnimation.Add(ActiveSquad, ActiveSquad.Position, ListMVHoverPoint);
                ActiveSquad.SetPosition(Map.CursorPosition);

                Map.CursorPosition = ActiveSquad.Position;
                Map.CursorPositionVisible = Map.CursorPosition;
            }
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendBoolean(HasFocus);
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendFloat(CursorPosition.X);
            BW.AppendFloat(CursorPosition.Y);
            BW.AppendFloat(CursorPosition.Z);

            BW.AppendInt32(ListMVHoverPoint.Count);
            for (int M = 0; M < ListMVHoverPoint.Count; ++M)
            {
                BW.AppendFloat(ListMVHoverPoint[M].X);
                BW.AppendFloat(ListMVHoverPoint[M].Y);
                BW.AppendFloat(ListMVHoverPoint[M].Z);
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelMovePart2(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
