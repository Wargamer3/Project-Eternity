using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using static ProjectEternity.GameScreens.SorcererStreetScreen.Player;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelChooseDirection : ActionPanelSorcererStreet
    {
        private const string PanelName = "ChooseDirection";

        public TerrainSorcererStreet ChosenTerrain { get { return ActivePlayer.CurrentDirection == Directions.None ? null : DicNextTerrain[ActivePlayer.CurrentDirection]; } }

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private readonly Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain;
        List<List<MovementAlgorithmTile>> ListPath;

        public ActionPanelChooseDirection(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelChooseDirection(SorcererStreetMap Map, int ActivePlayerIndex, Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
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
                ActivePlayer.CurrentDirection = Directions.Up;
            }
            else if (InputHelper.InputDownPressed() && DicNextTerrain.ContainsKey(Directions.Down))
            {
                ActivePlayer.CurrentDirection = Directions.Down;
            }
            else if (InputHelper.InputLeftPressed() && DicNextTerrain.ContainsKey(Directions.Left))
            {
                ActivePlayer.CurrentDirection = Directions.Left;
            }
            else if (InputHelper.InputRightPressed() && DicNextTerrain.ContainsKey(Directions.Right))
            {
                ActivePlayer.CurrentDirection = Directions.Right;
            }
            else if (InputHelper.InputConfirmPressed() && ActivePlayer.CurrentDirection != Directions.None)
            {
                FinalizeChoice();
            }

            if (ActivePlayer.CurrentDirection != Directions.None)
            {
                Map.LayerManager.AddDrawablePoints(new List<MovementAlgorithmTile>() { DicNextTerrain[ActivePlayer.CurrentDirection] }, Color.White);
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
        }
    }
}
