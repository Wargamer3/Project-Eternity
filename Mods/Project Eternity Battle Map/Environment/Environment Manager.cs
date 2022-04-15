using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using System.Collections.Generic;
using System.IO;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class EnvironmentManager
    {
        public enum TimePeriods { Turns, RealTime }
        public enum TimeLoopTypes { FirstDay, LastDay, Stop }

        private BattleMap Map;

        public float TimeStart;
        public float HoursInDay;
        public double CurrentHour { get; private set; }
        public TimeLoopTypes TimeLoopType;

        public float TimeMultiplier;
        public TimePeriods TimePeriodType;

        public List<VolatileSubstance> ListVolatileSubstance;

        public List<MapZone> ListMapZone;

        public EnvironmentManager(BattleMap Map)
        {
            this.Map = Map;

            TimeLoopType = TimeLoopTypes.FirstDay;
            HoursInDay = 24;

            TimePeriodType = TimePeriods.RealTime;
            TimeMultiplier = 1;

            ListMapZone = new List<MapZone>();
            ListVolatileSubstance = new List<VolatileSubstance>();
        }

        public EnvironmentManager(BinaryReader BR, BattleMap Map)
        {
            this.Map = Map;

            ListMapZone = new List<MapZone>();
            ListVolatileSubstance = new List<VolatileSubstance>();

            TimeStart = BR.ReadSingle();
            HoursInDay = BR.ReadSingle();
            TimeLoopType = (TimeLoopTypes)BR.ReadByte();
            TimeMultiplier = BR.ReadSingle();
            TimePeriodType = (TimePeriods)BR.ReadByte();

            int ListMapZoneCount = BR.ReadInt32();
            for (int Z = 0; Z < ListMapZoneCount; ++Z)
            {
                ListMapZone.Add(new MapZone(BR, Map));
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(TimeStart);
            BW.Write(HoursInDay);
            BW.Write((byte)TimeLoopType);
            BW.Write(TimeMultiplier);
            BW.Write((byte)TimePeriodType);

            BW.Write(ListMapZone.Count);
            for (int Z = 0; Z < ListMapZone.Count; ++Z)
            {
                ListMapZone[Z].Save(BW);
            }
        }

        public void Update(GameTime gameTime)
        {
            double EllapsedMinute = gameTime.ElapsedGameTime.TotalHours * 5d;
            CurrentHour += EllapsedMinute;

            foreach (MapZone ActiveZone in ListMapZone)
            {
                ActiveZone.Update(gameTime);
            }
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            foreach (MapZone ActiveZone in ListMapZone)
            {
                ActiveZone.BeginDraw(g);
            }
        }

        public void Draw(CustomSpriteBatch g)
        {
            foreach (MapZone ActiveZone in ListMapZone)
            {
                ActiveZone.Draw(g);
            }
        }

        public void EndDraw(CustomSpriteBatch g)
        {
            foreach (MapZone ActiveZone in ListMapZone)
            {
                ActiveZone.EndDraw(g);
            }
        }
    }
}
