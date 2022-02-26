using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class GetNearestWeaponPosition : DeathmatchScript, ScriptReference
        {
            public enum FlagTeams { Self, Enemy }

            private FlagTeams _FlagTeam;

            public GetNearestWeaponPosition()
                : base(100, 50, "Check Nearest Weapon Position", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                int StartingMV = Info.Map.GetSquadMaxMovement(Info.ActiveSquad);//Maximum distance you can reach.
                TemporaryAttackPickup ClosestAttackPickup = GetClosestTemporaryAttack(StartingMV);

                if (ClosestAttackPickup != null)
                {
                    return ClosestAttackPickup.Position;
                }

                WeaponSpawner ClosestWeaponSpawner = GetClosestWeaponSpawner(StartingMV);

                if (ClosestWeaponSpawner != null)
                {
                    return ClosestWeaponSpawner.Position;
                }

                return Info.ActiveSquad.Position;
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _FlagTeam = (FlagTeams)BR.ReadByte();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write((byte)_FlagTeam);
            }

            private TemporaryAttackPickup GetClosestTemporaryAttack(int StartingMV)
            {
                float MinDistance = float.MaxValue;
                TemporaryAttackPickup ClosestAttackPickup = null;

                for (int L = 0; L < Info.Map.LayerManager.ListLayer.Count; L++)
                {
                    for (int A = 0; A < Info.Map.LayerManager.ListLayer[L].ListAttackPickup.Count; A++)
                    {
                        float Distance = (Info.Map.LayerManager.ListLayer[L].ListAttackPickup[A].Position - Info.ActiveSquad.Position).Length();

                        if (Distance < MinDistance)
                        {
                            MinDistance = Distance;
                            ClosestAttackPickup = Info.Map.LayerManager.ListLayer[L].ListAttackPickup[A];
                        }
                    }
                }

                if (MinDistance <= StartingMV)
                {
                    return ClosestAttackPickup;
                }
                else
                {
                    return null;
                }
            }

            private WeaponSpawner GetClosestWeaponSpawner(int StartingMV)
            {
                float MinDistance = float.MaxValue;
                WeaponSpawner ClosestWeaponSpawner = null;

                for (int L = 0; L < Info.Map.LayerManager.ListLayer.Count; L++)
                {
                    for (int P = 0; P < Info.Map.LayerManager.ListLayer[L].ListProp.Count; P++)
                    {
                        WeaponSpawner ActiveFlag = Info.Map.LayerManager.ListLayer[L].ListProp[P] as WeaponSpawner;
                        if (ActiveFlag != null)
                        {
                            float Distance = (ActiveFlag.Position - Info.ActiveSquad.Position).Length();

                            if (Distance < MinDistance)
                            {
                                MinDistance = Distance;
                                ClosestWeaponSpawner = ActiveFlag;
                            }
                        }
                    }
                }

                if (MinDistance <= StartingMV)
                {
                    return ClosestWeaponSpawner;
                }
                else
                {
                    return null;
                }
            }

            public override AIScript CopyScript()
            {
                return new GetNearestWeaponPosition();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public FlagTeams FlagTeam
            {
                get
                {
                    return _FlagTeam;
                }
                set
                {
                    _FlagTeam = value;
                }
            }
        }
    }
}
