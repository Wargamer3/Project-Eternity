using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelAttackMAPSpread : ActionPanelSorcererStreet
    {
        private const string PanelName = "AttackMAPSpread";

        private TerrainSorcererStreet ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        public MAPAttackAttributes MAPAttributes;
        private List<Vector3> ListMVHoverPoints;
        public List<Vector3> ListAttackChoice;
        public List<MovementAlgorithmTile> ListAttackTerrain;

        public ActionPanelAttackMAPSpread(SorcererStreetMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackMAPSpread(SorcererStreetMap Map, int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ListMVHoverPoints = ListMVHoverPoints;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSummonedCreature[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
            ListAttackChoice = new List<Vector3>();
            ListAttackTerrain = new List<MovementAlgorithmTile>();
            for (int X = 0; X < MAPAttributes.ListChoice.Count; X++)
            {
                for (int Y = 0; Y < MAPAttributes.ListChoice[X].Count; Y++)
                {
                    if (MAPAttributes.ListChoice[X][Y])
                    {
                        Vector3 NewPosition = new Vector3(Map.CursorPosition.X +X - MAPAttributes.Width,
                                               Map.CursorPosition.Y + Y - MAPAttributes.Height, Map.CursorPosition.Z);
                        ListAttackChoice.Add(NewPosition);
                        ListAttackTerrain.Add(Map.GetTerrain(NewPosition));
                    }
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ActiveInputManager.InputConfirmPressed())
            {
                if (ListAttackChoice.Contains(Map.CursorPosition))
                {
                    Map.SelectMAPEnemies(ActivePlayerIndex, ActiveSquadIndex, MAPAttributes, ListMVHoverPoints, ListAttackTerrain);
                    Map.sndConfirm.Play();
                }
                else
                {
                    Map.sndDeny.Play();
                }
            }
            else
            {
                Map.CursorControl(ActiveInputManager);//Move the cursor
            }

            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSummonedCreature[ActiveSquadIndex];

            int AttackChoiceCount = BR.ReadInt32();
            ListAttackChoice = new List<Vector3>(AttackChoiceCount);
            ListAttackTerrain = new List<MovementAlgorithmTile>(AttackChoiceCount);
            for (int A = 0; A < AttackChoiceCount; ++A)
            {
                Vector3 NewTerrain = new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadInt32());
                ListAttackChoice.Add(NewTerrain);
                ListAttackTerrain.Add(Map.GetTerrain(NewTerrain));
            }

        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendInt32(ListAttackChoice.Count);

            for (int A = 0; A < ListAttackChoice.Count; ++A)
            {
                BW.AppendFloat(ListAttackChoice[A].X);
                BW.AppendFloat(ListAttackChoice[A].Y);
                BW.AppendInt32((int)ListAttackChoice[A].Z);
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAttackMAPSpread(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
