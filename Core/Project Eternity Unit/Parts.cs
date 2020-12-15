namespace ProjectEternity.Core.Parts
{
    public enum PartTypes { Standard, Consumable }

    public abstract class UnitPart
    {
        public PartTypes PartType;

        public abstract string Name { get; }

        public int Quantity = 1;
        
        public abstract void ActivatePassiveBuffs();
    }
}
