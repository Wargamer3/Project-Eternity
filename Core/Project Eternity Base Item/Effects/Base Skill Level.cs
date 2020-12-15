using System.IO;
using System.ComponentModel;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public class BaseSkillLevel
    {
        public List<BaseSkillActivation> ListActivation;//List of requirements, each of them activate the Skill when completed.

        private int _ActivationsCount;//Number of times the Skill can be used, -1 for infinite.
        public int Price;

        public BaseSkillLevel()
        {
            ListActivation = new List<BaseSkillActivation>();

            _ActivationsCount = -1;
            Price = 0;
        }

        public BaseSkillLevel(BaseSkillLevel Clone)
        {
            ListActivation = new List<BaseSkillActivation>(Clone.ListActivation.Count);
            foreach (BaseSkillActivation ActiveActivation in Clone.ListActivation)
            {
                //Some effect will need to update global parameters so to clones can copy the right values.
                foreach (BaseEffect ActiveEffect in ActiveActivation.ListEffect)
                {
                    ActiveEffect.SetupParamsBeforeCopy();
                }
            }

            foreach (BaseSkillActivation ActiveActivation in Clone.ListActivation)
            {
                ListActivation.Add(new BaseSkillActivation(ActiveActivation));
            }

            _ActivationsCount = Clone.ActivationsCount;
            Price = Clone.Price;
        }

        public BaseSkillLevel(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
        {
            ActivationsCount = BR.ReadInt32();
            Price = BR.ReadInt32();

            int ListActivationCount = BR.ReadInt32();
            ListActivation = new List<BaseSkillActivation>(ListActivationCount);
            for (int R = 0; R < ListActivationCount; R++)
            {
                ListActivation.Add(new BaseSkillActivation(BR, DicRequirement, DicEffect));
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ActivationsCount);
            BW.Write(Price);

            BW.Write(ListActivation.Count);
            for (int R = 0; R < ListActivation.Count; R++)
            {
                ListActivation[R].Save(BW);
            }
        }

        [CategoryAttribute("Level Attributes"),
        DescriptionAttribute(".")]
        public int ActivationsCount
        {
            get { return _ActivationsCount; }
            set { _ActivationsCount = value; }
        }
    }
}
