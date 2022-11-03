using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.Core.Units.UnitMapComponent;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelChooseDirection : ActionPanelSorcererStreet
    {
        private const string PanelName = "ChooseDirection";

        public TerrainSorcererStreet ChosenTerrain { get { return ActivePlayer.GamePiece.Direction == Directions.None ? null : DicNextTerrain[ActivePlayer.GamePiece.Direction]; } }

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private int Movement;
        private readonly Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain;
        List<List<MovementAlgorithmTile>> ListPath;

        public ActionPanelChooseDirection(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelChooseDirection(SorcererStreetMap Map, int ActivePlayerIndex, int Movement, Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.Movement = Movement;
            this.DicNextTerrain = DicNextTerrain;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            ListPath = new List<List<MovementAlgorithmTile>>();

            foreach (TerrainSorcererStreet ActiveTerrain in DicNextTerrain.Values)
            {
                List<MovementAlgorithmTile> ListArrow = new List<MovementAlgorithmTile>();
                ListArrow.Add(Map.GetTerrain(ActivePlayer.GamePiece));

                ListArrow.Add(ActiveTerrain);
                ListPath.Add(ListArrow);
            }
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            foreach (List<MovementAlgorithmTile> ListArrow in ListPath)
            {
                Map.LayerManager.AddDrawablePath(ListArrow);
            }

            if (InputHelper.InputUpPressed() && DicNextTerrain.ContainsKey(Directions.Up))
            {
                ActivePlayer.GamePiece.Direction = Directions.Up;
            }
            else if (InputHelper.InputDownPressed() && DicNextTerrain.ContainsKey(Directions.Down))
            {
                ActivePlayer.GamePiece.Direction = Directions.Down;
            }
            else if (InputHelper.InputLeftPressed() && DicNextTerrain.ContainsKey(Directions.Left))
            {
                ActivePlayer.GamePiece.Direction = Directions.Left;
            }
            else if (InputHelper.InputRightPressed() && DicNextTerrain.ContainsKey(Directions.Right))
            {
                ActivePlayer.GamePiece.Direction = Directions.Right;
            }
            else if (InputHelper.InputConfirmPressed() && ActivePlayer.GamePiece.Direction != Directions.None)
            {
                FinalizeChoice();
            }

            if (ActivePlayer.GamePiece.Direction != Directions.None)
            {
                Map.LayerManager.AddDrawablePoints(new List<MovementAlgorithmTile>() { DicNextTerrain[ActivePlayer.GamePiece.Direction] }, Color.White);
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelChooseDirection(Map);
        }

        public void FinalizeChoice()
        {
            RemoveFromPanelList(this);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw remaining movement count in the upper left corner
            GameScreen.DrawBox(g, new Vector2(30, 30), 50, 50, Color.Black);
            g.DrawString(Map.fntArial12, Movement.ToString(), new Vector2(37, 35), Color.White);
        }
    }
}
