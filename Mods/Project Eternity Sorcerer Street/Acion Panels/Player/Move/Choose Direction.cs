using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.Core.Units.UnitMapComponent;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelChooseDirection : ActionPanelSorcererStreet
    {
        private const string PanelName = "ChooseDirection";

        public TerrainSorcererStreet ChosenTerrain { get { return ActivePlayer.GamePiece.Direction == DirectionNone ? null : DicNextTerrain[ActivePlayer.GamePiece.Direction]; } }

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private int Movement;
        private readonly Dictionary<float, TerrainSorcererStreet> DicNextTerrain;
        private readonly List<List<MovementAlgorithmTile>> ListPath;

        public ActionPanelChooseDirection(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
            DicNextTerrain = new Dictionary<float, TerrainSorcererStreet>();
            ListPath = new List<List<MovementAlgorithmTile>>();
        }

        public ActionPanelChooseDirection(SorcererStreetMap Map, int ActivePlayerIndex, int Movement, Dictionary<float, TerrainSorcererStreet> DicNextTerrain)
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
                ListArrow.Add(Map.GetTerrain(ActivePlayer.GamePiece.Position));

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

            if (ActiveInputManager.InputUpPressed() && DicNextTerrain.ContainsKey(DirectionUp))
            {
                ActivePlayer.GamePiece.Direction = DirectionUp;

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }
            else if (ActiveInputManager.InputDownPressed() && DicNextTerrain.ContainsKey(DirectionDown))
            {
                ActivePlayer.GamePiece.Direction = DirectionDown;

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }
            else if (ActiveInputManager.InputLeftPressed() && DicNextTerrain.ContainsKey(DirectionLeft))
            {
                ActivePlayer.GamePiece.Direction = DirectionLeft;

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }
            else if (ActiveInputManager.InputRightPressed() && DicNextTerrain.ContainsKey(DirectionRight))
            {
                ActivePlayer.GamePiece.Direction = DirectionRight;

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }
            else if (ActiveInputManager.InputConfirmPressed() && ActivePlayer.GamePiece.Direction != DirectionNone)
            {
                FinalizeChoice();
            }

            if (ActivePlayer.GamePiece.Direction != DirectionNone)
            {
                Map.LayerManager.AddDrawablePoints(new List<MovementAlgorithmTile>() { DicNextTerrain[ActivePlayer.GamePiece.Direction] }, Color.White);
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            foreach (List<MovementAlgorithmTile> ListArrow in ListPath)
            {
                Map.LayerManager.AddDrawablePath(ListArrow);
            }

            if (ActivePlayer.GamePiece.Direction != DirectionNone)
            {
                Map.LayerManager.AddDrawablePoints(new List<MovementAlgorithmTile>() { DicNextTerrain[ActivePlayer.GamePiece.Direction] }, Color.White);
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            Movement = BR.ReadInt32();

            int DicNextTerrainCount = BR.ReadInt32();
            DicNextTerrain.Clear();
            for (int T = 0; T < DicNextTerrainCount; ++T)
            {
                DicNextTerrain.Add(BR.ReadFloat(), Map.GetTerrain(new Vector3(BR.ReadFloat(), BR.ReadFloat(), BR.ReadFloat())));
            }

            ListPath.Clear();

            foreach (TerrainSorcererStreet ActiveTerrain in DicNextTerrain.Values)
            {
                List<MovementAlgorithmTile> ListArrow = new List<MovementAlgorithmTile>();
                ListArrow.Add(Map.GetTerrain(ActivePlayer.GamePiece.Position));

                ListArrow.Add(ActiveTerrain);
                ListPath.Add(ListArrow);
            }
        }

        public override void ExecuteUpdate(byte[] ArrayUpdateData)
        {
            ByteReader BR = new ByteReader(ArrayUpdateData);
            ActivePlayer.GamePiece.Direction = BR.ReadFloat();
            BR.Clear();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(Movement);

            BW.AppendInt32(DicNextTerrain.Count);
            foreach (KeyValuePair<float, TerrainSorcererStreet> NextTerrain in DicNextTerrain)
            {
                BW.AppendFloat(NextTerrain.Key);
                BW.AppendFloat(NextTerrain.Value.GridPosition.X);
                BW.AppendFloat(NextTerrain.Value.GridPosition.Y);
                BW.AppendFloat(NextTerrain.Value.LayerIndex);
            }
        }

        public override byte[] DoWriteUpdate()
        {
            ByteWriter BW = new ByteWriter();

            BW.AppendFloat(ActivePlayer.GamePiece.Direction);

            byte[] ArrayUpdateData = BW.GetBytes();
            BW.ClearWriteBuffer();

            return ArrayUpdateData;
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
            MenuHelper.DrawDiceHolder(g,new Vector2(Constants.Width / 8, Constants.Height / 4), Movement);
        }
    }
}
