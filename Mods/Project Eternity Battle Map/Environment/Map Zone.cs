using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class MapZone
    {
        public string _Name;
        public ZoneShape Shape;

        public List<TimePeriod> ListTimePeriod;
        protected TimePeriod CurrentTimePeriod;
        private TimePeriod NextTimePeriod;
        private int NextTimePeriodIndex;

        public Color TimeOfDayColor;
        public Vector3 SunPosition;
        public Vector2 WindSpeed;

        protected BattleMapOverlay OverlayWeather;
        protected BattleMapOverlay OverlayTypeOfSky;
        protected BattleMapOverlay OverlayVisibility;

        private string _AIPath;
        public AIContainer ZoneAI;

        public MapZone(ZoneShape.ZoneShapeTypes ZoneShapeType)
        {
            _Name = string.Empty;
            _AIPath = string.Empty;

            Shape = new ZoneShape(ZoneShapeType);

            ListTimePeriod = new List<TimePeriod>();
            ListTimePeriod.Add(new TimePeriod("Default"));

            CurrentTimePeriod = ListTimePeriod[0];
        }

        public MapZone(BinaryReader BR)
        {
            _Name = BR.ReadString();
            Shape = new ZoneShape(BR);
            _AIPath = BR.ReadString();

            int ListTimePeriodCount = BR.ReadInt32();
            ListTimePeriod = new List<TimePeriod>(ListTimePeriodCount);
            for (int P = 0; P < ListTimePeriodCount; P++)
            {
                ListTimePeriod.Add(new TimePeriod(BR));
            }

            CurrentTimePeriod = ListTimePeriod[0];

            if (ListTimePeriod.Count > 1)
            {
                NextTimePeriod = ListTimePeriod[1];
                NextTimePeriodIndex = 1;
            }
        }

        public MapZone(MapZone Copy)
        {
            _Name = Copy._Name;
            Shape = new ZoneShape(Copy.Shape.ZoneShapeType, Copy.Shape.Position, Copy.Shape.Size, Copy.Shape.Radius);
            _AIPath = Copy.AIPath;

            ListTimePeriod = new List<TimePeriod>(Copy.ListTimePeriod.Count);
            for (int P = 0; P < Copy.ListTimePeriod.Count; P++)
            {
                ListTimePeriod.Add(new TimePeriod(Copy.ListTimePeriod[P]));
            }

            CurrentTimePeriod = ListTimePeriod[0];

            if (ListTimePeriod.Count > 1)
            {
                NextTimePeriod = ListTimePeriod[1];
                NextTimePeriodIndex = 1;
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(_Name);
            Shape.Save(BW);
            BW.Write(_AIPath);

            BW.Write(ListTimePeriod.Count);
            for (int P = 0; P < ListTimePeriod.Count; P++)
            {
                ListTimePeriod[P].Save(BW);
            }
        }

        public void Reset()
        {
            if (OverlayWeather != null)
            {
                OverlayWeather.Reset();
            }
            if (OverlayTypeOfSky != null)
            {
                OverlayTypeOfSky.Reset();
            }
            if (OverlayVisibility != null)
            {
                OverlayVisibility.Reset();
            }
        }

        public void Update(GameTime gameTime, double CurrentHour)
        {
            if (OverlayWeather != null)
            {
                OverlayWeather.Update(gameTime);
            }
            if (OverlayTypeOfSky != null)
            {
                OverlayTypeOfSky.Update(gameTime);
            }
            if (OverlayVisibility != null)
            {
                OverlayVisibility.Update(gameTime);
            }

            UpdateNextTimePeriod(CurrentHour);
        }

        public abstract void OnNewPlayerPhase();

        protected abstract void UpdateOverlayTypeOfSky();

        protected abstract void UpdateOverlayWeater();

        protected abstract void UpdateOverlayVisibility();

        public abstract MapZone Copy();

        private void UpdateNextTimePeriod(double CurrentHour)
        {
            if (NextTimePeriod != null)
            {
                if (CurrentHour >= NextTimePeriod.TimeStart)
                {
                    CurrentTimePeriod = NextTimePeriod;

                    if (++NextTimePeriodIndex < ListTimePeriod.Count)
                    {
                        NextTimePeriod = ListTimePeriod[NextTimePeriodIndex];
                    }
                    else
                    {
                        NextTimePeriod = null;
                    }

                    UpdateOverlayTypeOfSky();
                    UpdateOverlayWeater();
                    UpdateOverlayVisibility();
                }
            }
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            if (OverlayWeather != null)
            {
                OverlayWeather.BeginDraw(g);
            }
            if (OverlayTypeOfSky != null)
            {
                OverlayTypeOfSky.BeginDraw(g);
            }
            if (OverlayVisibility != null)
            {
                OverlayVisibility.BeginDraw(g);
            }
        }

        public void Draw(CustomSpriteBatch g)
        {
            if (OverlayWeather != null)
            {
                OverlayWeather.Draw(g);
            }
            if (OverlayTypeOfSky != null)
            {
                OverlayTypeOfSky.Draw(g);
            }
            if (OverlayVisibility != null)
            {
                OverlayVisibility.Draw(g);
            }
        }

        public void EndDraw(CustomSpriteBatch g)
        {
            if (OverlayWeather != null)
            {
                OverlayWeather.EndDraw(g);
            }
            if (OverlayTypeOfSky != null)
            {
                OverlayTypeOfSky.EndDraw(g);
            }
            if (OverlayVisibility != null)
            {
                OverlayVisibility.EndDraw(g);
            }
        }

        #region Properties

        [CategoryAttribute("Zone properties"),
        DescriptionAttribute("Name"),
        DefaultValueAttribute(0)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        [Editor(typeof(Selectors.AISelector), typeof(UITypeEditor)),
        CategoryAttribute("Zone properties"),
        DescriptionAttribute("The AI path"),
        DefaultValueAttribute(0)]
        public string AIPath
        {
            get
            {
                return _AIPath;
            }
            set
            {
                _AIPath = value;
            }
        }

        #endregion
    }
}
