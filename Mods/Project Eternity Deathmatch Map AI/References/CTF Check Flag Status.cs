using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class CTFGetFlagStatus : DeathmatchScript, ScriptReference
        {
            public enum FlagTeams { Self, Enemy }
            public enum FlagStatuses { AtBase, Captured, CapturedBySelf, Dropped }

            private FlagTeams _FlagTeam;
            private FlagStatuses _FlagStatus;

            private FlagSpawner FlagSpawnerCache;

            public CTFGetFlagStatus()
                : base(100, 50, "CTF - Get Flag Status", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                if (FlagSpawnerCache == null)
                {
                    FlagSpawnerCache = GetFlagSpawner();
                }

                if (FlagStatus == FlagStatuses.AtBase)
                {
                    return !FlagSpawnerCache.IsUsed;
                }
                else if (FlagStatus == FlagStatuses.Captured)
                {
                    return FlagSpawnerCache.IsUsed;
                }
                else if (FlagStatus == FlagStatuses.CapturedBySelf)
                {
                    Flag DroppedFlag = Info.ActiveSquad.ItemHeld as Flag;
                    if (DroppedFlag != null)
                    {
                        return DroppedFlag.Owner.Team != Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].TeamIndex;
                    }
                }
                else if (FlagStatus == FlagStatuses.Dropped)
                {
                    Flag DroppedFlag = GetFlag();
                    if (DroppedFlag != null)
                    {
                        return true;
                    }
                }

                return false;
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _FlagTeam = (FlagTeams)BR.ReadByte();
                _FlagStatus = (FlagStatuses)BR.ReadByte();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write((byte)_FlagTeam);
                BW.Write((byte)_FlagStatus);
            }

            private FlagSpawner GetFlagSpawner()
            {
                int CurrentPlayerTeam = Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].TeamIndex;

                for (int L = 0; L < Info.Map.LayerManager.ListLayer.Count; L++)
                {
                    for (int P = 0; P < Info.Map.LayerManager.ListLayer[L].ListProp.Count; P++)
                    {
                        FlagSpawner ActiveFlagSpawner = Info.Map.LayerManager.ListLayer[L].ListProp[P] as FlagSpawner;
                        if (ActiveFlagSpawner != null)
                        {
                            if (FlagTeam == FlagTeams.Self && ActiveFlagSpawner.Team == CurrentPlayerTeam)
                            {
                                return ActiveFlagSpawner;
                            }
                            else if (FlagTeam == FlagTeams.Enemy && ActiveFlagSpawner.Team != CurrentPlayerTeam)
                            {
                                return ActiveFlagSpawner;
                            }
                        }
                    }
                }

                return null;
            }

            private Flag GetFlag()
            {
                int CurrentPlayerTeam = Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].TeamIndex;

                for (int L = 0; L < Info.Map.LayerManager.ListLayer.Count; L++)
                {
                    for (int I = 0; I < Info.Map.LayerManager.ListLayer[L].ListHoldableItem.Count; I++)
                    {
                        Flag ActiveFlag = Info.Map.LayerManager.ListLayer[L].ListHoldableItem[I] as Flag;
                        if (ActiveFlag != null)
                        {
                            if (FlagTeam == FlagTeams.Self && ActiveFlag.Owner.Team == CurrentPlayerTeam)
                            {
                                return ActiveFlag;
                            }
                            else if (FlagTeam == FlagTeams.Enemy && ActiveFlag.Owner.Team != CurrentPlayerTeam)
                            {
                                return ActiveFlag;
                            }
                        }
                    }
                }

                return null;
            }

            public override AIScript CopyScript()
            {
                return new CTFGetFlagStatus();
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

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public FlagStatuses FlagStatus
            {
                get
                {
                    return _FlagStatus;
                }
                set
                {
                    _FlagStatus = value;
                }
            }
        }
    }
}
