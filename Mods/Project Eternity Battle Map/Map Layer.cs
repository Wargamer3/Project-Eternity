using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class BaseMapLayer
    {
        public int StartupDelay;
        public int ToggleDelayOn;
        public int ToggleDelayOff;

        public DrawableTile[,] ArrayTile;

        public List<EventPoint> ListCampaignSpawns;
        public List<EventPoint> ListMultiplayerSpawns;
        public List<MapSwitchPoint> ListMapSwitchPoint;
        public List<TeleportPoint> ListTeleportPoint;
        public List<InteractiveProp> ListProp;
        public List<HoldableItem> ListHoldableItem;
        public List<TemporaryAttackPickup> ListAttackPickup;

        public void RemoveTileset(int TilesetIndex)
        {
            for (int X = ArrayTile.GetLength(0) - 1; X >= 0; --X)
            {
                for (int Y = ArrayTile.GetLength(1) - 1; Y >= 0; --Y)
                {
                    if (ArrayTile[X, Y].TilesetIndex > TilesetIndex)
                    {
                        --ArrayTile[X, Y].TilesetIndex;
                    }
                }
            }
        }

        public abstract MovementAlgorithmTile GetTile(int X, int Y);
    }

    public interface ISubMapLayer
    {

    }

    public abstract class LayerHolder
    {
        public ILayerHolderDrawable LayerHolderDrawable;

        public abstract BaseMapLayer this[int i] { get; }

        public abstract void Save(BinaryWriter BW);

        public abstract void Update(GameTime gameTime);

        public abstract void TogglePreview(bool UsePreview);
        
        public abstract void AddDrawablePoints(List<MovementAlgorithmTile> ListPoint, Color PointColor);

        public abstract void AddDrawablePath(List<MovementAlgorithmTile> ListPoint);

        public abstract void AddDamageNumber(string Damage, Vector3 Position);

        public abstract void BeginDraw(CustomSpriteBatch g);

        public abstract void Draw(CustomSpriteBatch g);

        public abstract void EndDraw(CustomSpriteBatch g);
    }

    public interface ILayerHolderDrawable
    {
        Point GetVisiblePosition(Vector3 Position);

        void AddDrawablePath(List<MovementAlgorithmTile> ListPoint);

        void AddDrawablePoints(List<MovementAlgorithmTile> ListPoint, Color PointColor);

        void AddDamageNumber(string Damage, Vector3 Position);

        void SetWorld(Matrix NewWorld);

        void Reset();

        void CursorMoved();

        void UnitMoved(int PlayerIndex);

        void UnitKilled(int PlayerIndex);

        void Update(GameTime gameTime);

        void BeginDraw(CustomSpriteBatch g);

        void Draw(CustomSpriteBatch g);

        void EndDraw(CustomSpriteBatch g);
    }
}
