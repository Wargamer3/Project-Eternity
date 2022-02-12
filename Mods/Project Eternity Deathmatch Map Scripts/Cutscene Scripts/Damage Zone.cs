using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class DamageZoneTerrain : DeathmatchMapScript
        {
            private List<Vector2> ListTerrainDamageLocation;
            private int _Damage;

            public DamageZoneTerrain()
                : this(null)
            {
            }

            public DamageZoneTerrain(DeathmatchMap Map)
                : base(Map, 150, 50, "Damage Zone", new string[] { "Damage" }, new string[] { "Damage dealt" })
            {
                ListTerrainDamageLocation = new List<Vector2>();
                _Damage = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(GameTime gameTime)
            {
                for (int P = 0; P < Map.ListPlayer.Count; P++)
                {
                    Player ActivePlayer = Map.ListPlayer[P];
                    foreach (Core.Units.Squad ActiveSquad in ActivePlayer.ListSquad)
                    {
                        if (!ActiveSquad.IsDead && ListTerrainDamageLocation.Contains(new Vector2(ActiveSquad.Position.X, ActiveSquad.Position.Y)))
                        {
                            for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; ++U)
                            {
                                ActiveSquad[U].DamageUnit(_Damage);
                            }

                            ActiveSquad.UpdateSquad();
                            if (ActiveSquad.IsDead)
                            {
                                Map.GameRule.OnSquadDefeated(Map.ActivePlayerIndex, null, P, ActiveSquad);
                            }
                        }
                    }
                }

                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _Damage = BR.ReadInt32();

                int ListTerrainDamageLocationCount = BR.ReadInt32();
                ListTerrainDamageLocation = new List<Vector2>(ListTerrainDamageLocationCount);
                for (int L = 0; L < ListTerrainDamageLocationCount; ++L)
                {
                    ListTerrainDamageLocation.Add(new Vector2(BR.ReadSingle(), BR.ReadSingle()));
                }
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_Damage);

                BW.Write(ListTerrainDamageLocation.Count);
                for (int L = 0; L < ListTerrainDamageLocation.Count; ++L)
                {
                    BW.Write(ListTerrainDamageLocation[L].X);
                    BW.Write(ListTerrainDamageLocation[L].Y);
                }
            }

            protected override CutsceneScript DoCopyScript()
            {
                DamageZoneTerrain NewScript = new DamageZoneTerrain(Map);
                NewScript.MapDestination.AddRange(MapDestination);
                return NewScript;
            }

            #region Properties

            [Editor(typeof(MapDestinationSelector), typeof(UITypeEditor)),
            CategoryAttribute("Terrain change locations"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public List<Vector2> MapDestination
            {
                get
                {
                    return ListTerrainDamageLocation;
                }
                set
                {
                    ListTerrainDamageLocation = value;
                }
            }

            [CategoryAttribute("Damage"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public int Damage
            {
                get
                {
                    return _Damage;
                }
                set
                {
                    _Damage = value;
                }
            }

            #endregion
        }
    }
}
