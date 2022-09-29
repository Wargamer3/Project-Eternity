using System;
using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.Core.Units
{
    public abstract class ITerrainRestriction
    {
        public readonly List<ITerrainRestriction> ListRestriction;

        public readonly string Type;

        public float EntryCost;
        public float MovementCost;
        public float ENCostToMove;
        public int ENCostPerTurn;

        protected ITerrainRestriction(string Type)
        {
            this.Type = Type;
            ListRestriction = new List<ITerrainRestriction>();
        }

        public static ITerrainRestriction LoadCopy(BinaryReader BR, UnitAndTerrainValues Parent)
        {
            string Type = BR.ReadString();

            ITerrainRestriction LoadedTerrainRestriction = null;

            if (Type == UnitTypeRestriction.RestrictionName)
            {
                LoadedTerrainRestriction = new UnitTypeRestriction(BR, Parent);
            }
            else if (Type == UnitSizeRestriction.RestrictionName)
            {
                LoadedTerrainRestriction = new UnitSizeRestriction(BR, Parent);
            }
            else if (Type == UnitRankRestriction.RestrictionName)
            {
                LoadedTerrainRestriction = new UnitRankRestriction(BR, Parent);
            }
            else if (Type == TagRestriction.RestrictionName)
            {
                LoadedTerrainRestriction = new TagRestriction(BR, Parent);
            }

            LoadedTerrainRestriction.EntryCost = BR.ReadSingle();
            LoadedTerrainRestriction.MovementCost = BR.ReadSingle();
            LoadedTerrainRestriction.ENCostToMove = BR.ReadSingle();
            LoadedTerrainRestriction.ENCostPerTurn = BR.ReadInt32();

            return LoadedTerrainRestriction;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Type);

            DoSave(BW);

            BW.Write(EntryCost);
            BW.Write(MovementCost);
            BW.Write(ENCostToMove);
            BW.Write(ENCostPerTurn);
        }

        public abstract void DoSave(BinaryWriter BW);

        public abstract bool IsValid(UnitMapComponent Unit, UnitStats Stats);

        public ITerrainRestriction Copy()
        {
            ITerrainRestriction NewCopy = DoCopy();

            foreach (ITerrainRestriction ActiveRestriction in ListRestriction)
            {
                NewCopy.ListRestriction.Add(ActiveRestriction);
            }

            return NewCopy;
        }

        public abstract void SetValues(List<List<string>> ListValuePerRow);

        public abstract DataGridValue GetColumns();

        public abstract ITerrainRestriction DoCopy();
    }
}
