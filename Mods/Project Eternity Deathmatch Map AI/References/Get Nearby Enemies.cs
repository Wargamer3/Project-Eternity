using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class GetNearbyEnemies : DeathmatchScript, ScriptReference
        {
            private int _Range;
            private bool _ConsiderWalls;

            public GetNearbyEnemies()
                : base(100, 50, "Get Nearby Enemies Deathmatch", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                List<object> ListEnemy = new List<object>();
                int CurrentTeam = Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].TeamIndex;

                for (int P = Info.Map.ListPlayer.Count - 1; P >= 0; --P)
                {
                    if (Info.Map.ListPlayer[P].TeamIndex != CurrentTeam)
                    {
                        for (int S = Info.Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                        {
                            if (Math.Abs(Info.Map.ListPlayer[P].ListSquad[S].X - Info.ActiveSquad.X) <= _Range && Math.Abs(Info.Map.ListPlayer[P].ListSquad[S].Y - Info.ActiveSquad.Y) <= _Range)
                            {
                                ListEnemy.Add(Info.Map.ListPlayer[P].ListSquad[S]);
                            }
                        }
                    }
                }


                if (_ConsiderWalls)
                {
                    for (int i = ListEnemy.Count - 1; i >= 0; --i)
                    {
                        Squad ActiveSquad = (Squad)ListEnemy[i];
                        int x1 = (int)Info.ActiveSquad.X;
                        int y1 = (int)Info.ActiveSquad.Y;
                        int x2 = (int)ActiveSquad.X;
                        int y2 = (int)ActiveSquad.Y;
                        int dx = x2 - x1;
                        int dy = y2 - y1;
                        for (int x = x1; x < x2; ++x)
                        {
                            int y = y1 + dy * (x - x1) / dx;

                            Terrain ActiveTerrain = Info.Map.GetTerrain(new Microsoft.Xna.Framework.Vector3(x, y, (int)Info.ActiveSquad.Z));

                            if (!Info.Map.TerrainRestrictions.CanMove(ActiveSquad, ActiveSquad.CurrentLeader.UnitStat, ActiveTerrain.TerrainTypeIndex))
                            {
                                ListEnemy.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }

                return ListEnemy;
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _Range = BR.ReadInt32();
                _ConsiderWalls = BR.ReadBoolean();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_Range);
                BW.Write(_ConsiderWalls);
            }

            public override AIScript CopyScript()
            {
                return new GetNearbyEnemies();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public int Range
            {
                get
                {
                    return _Range;
                }
                set
                {
                    _Range = value;
                }
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public bool ConsiderWalls
            {
                get
                {
                    return _ConsiderWalls;
                }
                set
                {
                    _ConsiderWalls = value;
                }
            }
        }
    }
}
