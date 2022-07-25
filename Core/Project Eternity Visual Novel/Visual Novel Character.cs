using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.VisualNovelScreen
{
    public class VisualNovelCharacter
    {
        public string CharacterName;
        private Character _LoadedCharacter;
        public Character LoadedCharacter { get { return _LoadedCharacter; } }
        public readonly List<SpeakerPriority> ListSpeakerPriority;//Used to center camera if the character is talking

        public VisualNovelCharacter(string CharacterName)
        {
            this.CharacterName = CharacterName;

            ListSpeakerPriority = new List<SpeakerPriority>();

            _LoadedCharacter = new Character(CharacterName, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);
        }

        public VisualNovelCharacter(BinaryReader BR)
        {
            this.CharacterName = BR.ReadString();

            int ListSpeakerPriorityCounter = BR.ReadInt32();
            ListSpeakerPriority = new List<SpeakerPriority>(ListSpeakerPriorityCounter);
            for (int S = 0; S < ListSpeakerPriorityCounter; ++S)
            {
                ListSpeakerPriority.Add(new SpeakerPriority((SpeakerPriority.PriorityTypes)BR.ReadByte(), BR.ReadString()));
            }

            _LoadedCharacter = new Character(CharacterName, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(CharacterName);
            BW.Write(ListSpeakerPriority.Count);
            for (int S = 0; S < ListSpeakerPriority.Count; S++)
            {
                BW.Write((byte)ListSpeakerPriority[S].PriorityType);
                BW.Write(ListSpeakerPriority[S].Name);
            }
        }

        public override string ToString()
        {
            return CharacterName;
        }
    }
}
