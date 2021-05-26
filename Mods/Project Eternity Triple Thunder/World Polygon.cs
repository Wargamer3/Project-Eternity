using System;
using System.ComponentModel;
using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class WorldPolygon : ICollisionObject<WorldPolygon>
    {
        private bool _BlockBullets;
        private bool _IsPlatform;

        CollisionObject<WorldPolygon> CollisionBox;
        public CollisionObject<WorldPolygon> Collision => CollisionBox;

        public WorldPolygon(Vector2[] ArrayVertex, int MaxWidth, int MaxHeight)
        {
            _BlockBullets = true;
            _IsPlatform = false;

            CollisionBox = new CollisionObject<WorldPolygon>(ArrayVertex, MaxWidth, MaxHeight);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(CollisionBox.ListCollisionPolygon[0].ArrayVertex.Length);
            for (int V = 0; V < CollisionBox.ListCollisionPolygon[0].ArrayVertex.Length; V++)
            {
                BW.Write(CollisionBox.ListCollisionPolygon[0].ArrayVertex[V].X);
                BW.Write(CollisionBox.ListCollisionPolygon[0].ArrayVertex[V].Y);
            }

            BW.Write(BlockBullets);
            BW.Write(IsPlatform);
        }

        #region Properties

        [CategoryAttribute("Polygon attributes"),
        DescriptionAttribute("Allow bullets to go through this polygon."),
        DefaultValueAttribute("")]
        public bool BlockBullets { get { return _BlockBullets; } set { _BlockBullets = value; } }

        [CategoryAttribute("Polygon attributes"),
        DescriptionAttribute("Allow players to jump through this polygon."),
        DefaultValueAttribute("")]
        public bool IsPlatform { get { return _IsPlatform; } set { _IsPlatform = value; } }


        #endregion
    }
}
