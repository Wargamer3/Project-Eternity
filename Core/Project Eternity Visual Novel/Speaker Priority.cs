namespace ProjectEternity.GameScreens.VisualNovelScreen
{
    public struct SpeakerPriority
    {
        public enum PriorityTypes { Character, Location, ID };

        public readonly PriorityTypes PriorityType;
        public readonly string Name;

        public SpeakerPriority(PriorityTypes PriorityType, string Name)
        {
            this.PriorityType = PriorityType;
            this.Name = Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
