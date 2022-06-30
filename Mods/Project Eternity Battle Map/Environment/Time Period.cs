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
            HeatWave,
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
        public enum ZoneUsages { Global, Relative, Absolute }


        public string Name;

        public float TimeStart;
        public float DayStart;

        public WeatherTypes WeatherType;
        public ZoneUsages WeatherUsage;
        public SkyTypes SkyType;
        public ZoneUsages SkyUsage;
        public VisibilityTypes VisibilityType;
        public ZoneUsages VisibilityUsage;

        public ZoneUsages WindUsage;
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

            WeatherType = WeatherTypes.Regular;
            SkyType = SkyTypes.Regular;
            VisibilityType = VisibilityTypes.Regular;

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
            PassiveSkill = null;

            Load(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget);
        }

        public TimePeriod(TimePeriod Copy, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            Name = Copy.Name;

            TimeStart = Copy.TimeStart;
            DayStart = Copy.DayStart;

            PassiveSkillPath = Copy.PassiveSkillPath;
            if (PassiveSkillPath != "None")
            {
                PassiveSkill = new BaseAutomaticSkill("Content/Characters/Skills/" + PassiveSkillPath + ".pecs", PassiveSkillPath, DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }

            WeatherType = Copy.WeatherType;
            WeatherUsage = Copy.WeatherUsage;

            SkyType = Copy.SkyType;
            SkyUsage = Copy.SkyUsage;

            VisibilityType = Copy.VisibilityType;
            VisibilityUsage = Copy.VisibilityUsage;

            WindUsage = Copy.WindUsage;
            WindSpeed = Copy.WindSpeed;
            WindDirection = Copy.WindDirection;

            TransitionStartLength = Copy.TransitionStartLength;
            TransitionEndLength = Copy.TransitionEndLength;
            CrossfadeLength = Copy.CrossfadeLength;
        }

        protected virtual void Load(BinaryReader BR, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
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

            WeatherType = (WeatherTypes)BR.ReadByte();
            WeatherUsage = (ZoneUsages)BR.ReadByte();

            SkyType = (SkyTypes)BR.ReadByte();
            SkyUsage = (ZoneUsages)BR.ReadByte();

            VisibilityType = (VisibilityTypes)BR.ReadByte();
            VisibilityUsage = (ZoneUsages)BR.ReadByte();

            WindUsage = (ZoneUsages)BR.ReadByte();
            WindSpeed = BR.ReadSingle();
            WindDirection = BR.ReadSingle();

            TransitionStartLength = BR.ReadSingle();
            TransitionEndLength = BR.ReadSingle();
            CrossfadeLength = BR.ReadSingle();
        }

        public virtual void Save(BinaryWriter BW)
        {
            BW.Write(Name);

            BW.Write(TimeStart);
            BW.Write(DayStart);

            BW.Write(PassiveSkillPath);

            BW.Write((byte)WeatherType);
            BW.Write((byte)WeatherUsage);

            BW.Write((byte)SkyType);
            BW.Write((byte)SkyUsage);

            BW.Write((byte)VisibilityType);
            BW.Write((byte)VisibilityUsage);

            BW.Write((byte)WindUsage);
            BW.Write(WindSpeed);
            BW.Write(WindDirection);

            BW.Write(TransitionStartLength);
            BW.Write(TransitionEndLength);
            BW.Write(CrossfadeLength);
        }
    }
}
