using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class TimePeriod
    {
        public enum WeatherTypes
        {
            Regular,
            Windy,
            SomeClouds,
            Cloudy,
            SmallRain,
            HeavyRain,
            Thunderstorm,
            LightSnow,
            HeavySnow,
            Sandstorm,
            Starfall,
            Distortion,
            CherryBlosom,
            FallingLeaves,
            Fireflies,
            Minovsky,
            Getter,
        }

        public enum SkyTypes { Disabled, Regular, Arctic, Warm, }
        public enum VisibilityTypes { Regular, FogOfWar }

        public string Name;

        public float TimeStart;
        public float DayStart;

        public WeatherTypes Weather;
        public SkyTypes TypeOfSky;
        public VisibilityTypes Visibility;

        public float WindSpeed;
        public float WindDirection;

        public float TransitionStartLength;
        public float TransitionEndLength;
        public float CrossfadeLength;
        public string PassiveSkillPath;
        public BaseAutomaticSkill PassiveSkill;

        public TimePeriod(string Name)
        {
            this.Name = Name;

            TimeStart = 0;
            DayStart = 0;

            PassiveSkillPath = "None";

            Weather = WeatherTypes.Regular;
            TypeOfSky = SkyTypes.Regular;
            Visibility = VisibilityTypes.Regular;

            WindSpeed = 0;
            WindDirection = 0;

            TransitionStartLength = 0;
            TransitionEndLength = 0;
            CrossfadeLength = 0;
            PassiveSkill = null;
        }

        public TimePeriod(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            Name = BR.ReadString();

            TimeStart = BR.ReadSingle();
            DayStart = BR.ReadSingle();

            PassiveSkillPath = BR.ReadString();
            if (PassiveSkillPath != "None")
            {
                PassiveSkill = new BaseAutomaticSkill("Content/Characters/Skills/" + PassiveSkillPath + ".pecs", PassiveSkillPath, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }

            Weather = (WeatherTypes)BR.ReadByte();
            TypeOfSky = (SkyTypes)BR.ReadByte();
            Visibility = (VisibilityTypes)BR.ReadByte();

            WindSpeed = BR.ReadSingle();
            WindDirection = BR.ReadSingle();

            TransitionStartLength = BR.ReadSingle();
            TransitionEndLength = BR.ReadSingle();
            CrossfadeLength = BR.ReadSingle();
            PassiveSkill = null;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Name);

            BW.Write(TimeStart);
            BW.Write(DayStart);

            BW.Write(PassiveSkillPath);

            BW.Write((byte)Weather);
            BW.Write((byte)TypeOfSky);
            BW.Write((byte)Visibility);

            BW.Write(WindSpeed);
            BW.Write(WindDirection);

            BW.Write(TransitionStartLength);
            BW.Write(TransitionEndLength);
            BW.Write(CrossfadeLength);
        }
    }
}
