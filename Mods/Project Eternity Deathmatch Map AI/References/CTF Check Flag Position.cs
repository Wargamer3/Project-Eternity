using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class CTFGetFlagPosition : DeathmatchScript, ScriptReference
        {
            public enum FlagTeams { Self, Enemy }

            private FlagTeams _FlagTeam;

            private FlagSpawner FlagSpawnerCache;

            public CTFGetFlagPosition()
                : base(100, 50, "CTF - Get Flag Position", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                if (FlagSpawnerCache == null)
                {
                    FlagSpawnerCache = GetFlagSpawner();
                }

                if (FlagSpawnerCache.IsUsed)
                {

                    Flag DroppedFlag = GetFlag();
                    if (DroppedFlag != null)
                    {
                        return DroppedFlag.Position;
                    }
                }
                else
                {
                    return FlagSpawnerCache.Position;
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
                return new CTFGetFlagPosition();
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
