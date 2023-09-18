using System;
using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.Core.Units
{
    public class TerrainType
    {
        public string Name;

        public string ActivationName;
        public string AnnulatioName;
        public byte WallHardness;

        public List<ITerrainRestriction> ListRestriction;

        public TerrainType()
            : this("New Terrain")
        {
            ActivationName = string.Empty;
            AnnulatioName = string.Empty;
            ListRestriction = new List<ITerrainRestriction>();
        }

        public TerrainType(string Name)
        {
            this.Name = Name;
            ActivationName = string.Empty;
            AnnulatioName = string.Empty;
            ListRestriction = new List<ITerrainRestriction>();
        }

        public TerrainType(BinaryReader BR, UnitAndTerrainValues Parent)
        {
            Name = BR.ReadString();
            ActivationName = BR.ReadString();
            AnnulatioName = BR.ReadString();
            WallHardness = BR.ReadByte();

            int ListRestrictionCount = BR.ReadInt32();

            ListRestriction = new List<ITerrainRestriction>(ListRestrictionCount);
            for (int R = 0; R < ListRestrictionCount; ++R)
            {
                ListRestriction.Add(ITerrainRestriction.LoadCopy(BR, Parent));
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Name);
            BW.Write(ActivationName);
            BW.Write(AnnulatioName);
            BW.Write(WallHardness);

            BW.Write(ListRestriction.Count);
            for (int R = 0; R < ListRestriction.Count; ++R)
            {
                ListRestriction[R].Save(BW);
            }
        }

        public bool CanMove(UnitMapComponent Unit, UnitStats Stats)
        {
            foreach (ITerrainRestriction ActiveRestriction in ListRestriction)
            {
                if (ActiveRestriction.IsValid(Unit, Stats))
                {
                    return true;
                }
            }

            return false;
        }

        public float GetMovementCost(UnitMapComponent Unit, UnitStats Stats)
        {
            foreach (ITerrainRestriction ActiveRestriction in ListRestriction)
            {
                if (ActiveRestriction.IsValid(Unit, Stats))
                {
                    return ActiveRestriction.MovementCost;
                }
            }

            return -1;
        }

        public float GetEntryCost(UnitMapComponent Unit, UnitStats Stats)
        {
            foreach (ITerrainRestriction ActiveRestriction in ListRestriction)
            {
                if (ActiveRestriction.IsValid(Unit, Stats))
                {
                    return ActiveRestriction.EntryCost;
                }
            }

            return -1;
        }

        public float GetENCost(UnitMapComponent Unit, UnitStats Stats)
        {
            foreach (ITerrainRestriction ActiveRestriction in ListRestriction)
            {
                if (ActiveRestriction.IsValid(Unit, Stats))
                {
                    return ActiveRestriction.ENCostToMove;
                }
            }

            return -1;
        }

        public int GetENUsedPerTurnCost(UnitMapComponent Unit, UnitStats Stats)
        {
            foreach (ITerrainRestriction ActiveRestriction in ListRestriction)
            {
                if (ActiveRestriction.IsValid(Unit, Stats))
                {
                    return ActiveRestriction.ENCostPerTurn;
                }
            }

            return -1;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
