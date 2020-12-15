using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.AdventureScreen
{
    public class Prop
    {
        public Vector2 Position;

        public Prop Copy()
        {
            return new Prop();
        }

        public static List<Prop> GetAllProps()
        {
            return new List<Prop>();
        }
    }
}
