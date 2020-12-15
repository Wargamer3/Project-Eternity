using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class WorldPolygon : Polygon
    {
        private bool _BlockBullets;
        private bool _IsPlatform;

        public WorldPolygon(Vector2[] ArrayVertex, float MaxWidth, float MaxHeight)
            : base(ArrayVertex, MaxWidth, MaxHeight)
        {
            _BlockBullets = true;
            _IsPlatform = false;
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
