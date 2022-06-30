using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using System.Collections.Generic;
using System.IO;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class EnvironmentManager
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

        public MapZone GlobalZone;
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

            ListVolatileSubstance = new List<VolatileSubstance>();

            TimeStart = BR.ReadSingle();
            HoursInDay = BR.ReadSingle();
            TimeLoopType = (TimeLoopTypes)BR.ReadByte();
            TimeMultiplier = BR.ReadSingle();
            TimePeriodType = (TimePeriods)BR.ReadByte();
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(TimeStart);
            BW.Write(HoursInDay);
            BW.Write((byte)TimeLoopType);
            BW.Write(TimeMultiplier);
            BW.Write((byte)TimePeriodType);

            GlobalZone.Save(BW);

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

            GlobalZone.Update(gameTime);

            foreach (MapZone ActiveZone in ListMapZone)
            {
                ActiveZone.Update(gameTime);
            }
        }

        public void OnNewPlayerPhase()
        {
            GlobalZone.OnNewPlayerPhase();

            foreach (MapZone ActiveZone in ListMapZone)
            {
                ActiveZone.OnNewPlayerPhase();
            }
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            GlobalZone.BeginDraw(g);

            foreach (MapZone ActiveZone in ListMapZone)
            {
                ActiveZone.BeginDraw(g);
            }
        }

        public void Draw(CustomSpriteBatch g, int LayerIndex, bool IsSubLayer)
        {
            GlobalZone.Draw(g, LayerIndex, IsSubLayer);

            foreach (MapZone ActiveZone in ListMapZone)
            {
                ActiveZone.Draw(g, LayerIndex, IsSubLayer);
            }
        }

        public void EndDraw(CustomSpriteBatch g)
        {
            GlobalZone.EndDraw(g);

            foreach (MapZone ActiveZone in ListMapZone)
            {
                ActiveZone.EndDraw(g);
            }
        }
    }
}
