using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum RelationshipStatus
    {
        Hated,
        Neutral,
        Friendly,
        BestFriend,
        Lover,
        Married,
    }

    public enum RelationshipMood
    {
        Angry,
        Neutral,
        LoveyDovey,
    }

    public class CharacterRelationship
    {
        public const float FullBar = 100f;

        public PlayerCharacter Other;
        public float Progress;
        public RelationshipStatus Status;
        public RelationshipMood Mood;

        public float Bar => Progress / FullBar;

        public CharacterRelationship(PlayerCharacter Other)
        {
            this.Other = Other;
            Status = RelationshipStatus.Neutral;
            Mood = RelationshipMood.Neutral;
        }

        public void AddProgress(float Amount)
        {
            Progress += Amount;
            if (Progress < 0f)
            {
                Progress = 0f;
            }

            if (Progress > FullBar)
            {
                Progress = FullBar;
            }
        }
    }

    public partial class PlayerCharacter
    {
        public Dictionary<PlayerCharacter, CharacterRelationship> DicRelationshipByCharacter = new Dictionary<PlayerCharacter, CharacterRelationship>();

        public CharacterRelationship RelationshipWith(PlayerCharacter Other)
        {
            if (Other == null)
            {
                return null;
            }

            if (!DicRelationshipByCharacter.TryGetValue(Other, out CharacterRelationship Bond))
            {
                Bond = new CharacterRelationship(Other);
                DicRelationshipByCharacter.Add(Other, Bond);
            }

            return Bond;
        }
    }
}
