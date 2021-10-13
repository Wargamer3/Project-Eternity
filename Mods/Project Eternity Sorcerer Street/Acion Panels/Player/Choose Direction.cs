using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using static ProjectEternity.GameScreens.SorcererStreetScreen.Player;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelChooseDirection : ActionPanelSorcererStreet
    {
        private const string PanelName = "ChooseDirection";

        public TerrainSorcererStreet ChosenTerrain { get { return ActivePlayer.CurrentDirection == Directions.None ? null : DicNextTerrain[ActivePlayer.CurrentDirection]; } }

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private readonly Dictionary<Directions, TerrainSorcererStreet> DicNextTerrain;

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
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
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
            if (DicNextTerrain.Keys.Contains(Directions.Right))
            {
                if (ActivePlayer.CurrentDirection == Directions.Right)
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle((int)ActivePlayer.GamePiece.X + 32, (int)ActivePlayer.GamePiece.Y, 20, 20), Color.Red);
                }
                else
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle((int)ActivePlayer.GamePiece.X + 32, (int)ActivePlayer.GamePiece.Y, 20, 20), Color.AliceBlue);
                }
            }
            if (DicNextTerrain.Keys.Contains(Directions.Down))
            {
                if (ActivePlayer.CurrentDirection == Directions.Down)
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle((int)ActivePlayer.GamePiece.X, (int)ActivePlayer.GamePiece.Y + 32, 20, 20), Color.Red);
                }
                else
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle((int)ActivePlayer.GamePiece.X, (int)ActivePlayer.GamePiece.Y + 32, 20, 20), Color.AliceBlue);
                }
            }
        }
    }
}
