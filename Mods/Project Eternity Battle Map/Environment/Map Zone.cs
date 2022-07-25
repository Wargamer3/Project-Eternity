using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class MapZone
    {
        private BattleMap Map;

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

        private BattleMapOverlay CrossfadeOverlayWeather;
        private BattleMapOverlay CrossfadeOverlayTypeOfSky;
        private BattleMapOverlay CrossfadeOverlayVisibility;
        private string _AIPath;
        public AIContainer ZoneAI;

        public MapZone(BattleMap Map, ZoneShape.ZoneShapeTypes ZoneShapeType)
        {
            this.Map = Map;

            _Name = string.Empty;
            _AIPath = string.Empty;

            Shape = new ZoneShape(ZoneShapeType);

            ListTimePeriod = new List<TimePeriod>();
            ListTimePeriod.Add(new TimePeriod("Default"));

            CurrentTimePeriod = ListTimePeriod[0];
        }

        public MapZone(BinaryReader BR, BattleMap Map)
        {
            this.Map = Map;

            _Name = BR.ReadString();
            Shape = new ZoneShape(BR);
            _AIPath = BR.ReadString();

            int ListTimePeriodCount = BR.ReadInt32();
            ListTimePeriod = new List<TimePeriod>(ListTimePeriodCount);
            for (int P = 0; P < ListTimePeriodCount; P++)
            {
                ListTimePeriod.Add(new TimePeriod(BR, Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget));
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
            Map = Copy.Map;

            _Name = Copy._Name;
            Shape = new ZoneShape(Copy.Shape.ZoneShapeType, Copy.Shape.Position, Copy.Shape.Size, Copy.Shape.Radius);
            _AIPath = Copy.AIPath;

            ListTimePeriod = new List<TimePeriod>(Copy.ListTimePeriod.Count);
            for (int P = 0; P < Copy.ListTimePeriod.Count; P++)
            {
                ListTimePeriod.Add(new TimePeriod(Copy.ListTimePeriod[P], Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget));
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

        public void Update(GameTime gameTime)
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

            if (CrossfadeOverlayTypeOfSky != null)
            {
                CrossfadeOverlayTypeOfSky.Update(gameTime);
            }
            if (CrossfadeOverlayWeather != null)
            {
                CrossfadeOverlayWeather.Update(gameTime);
            }
            if (CrossfadeOverlayVisibility != null)
            {
                CrossfadeOverlayVisibility.Update(gameTime);
            }

            UpdateNextTimePeriod();
        }

        public abstract void OnNewPlayerPhase();

        protected abstract void UpdateOverlayTypeOfSky();

        protected abstract void UpdateOverlayWeater();

        protected abstract void UpdateOverlayVisibility();

        public abstract MapZone Copy();

        private void UpdateNextTimePeriod()
        {
            if (NextTimePeriod != null)
            {
                if (Map.MapEnvironment.CurrentHour >= NextTimePeriod.TimeStart)
                {
                    if (CurrentTimePeriod.CrossfadeLength > 0 && NextTimePeriod.TimeStart + CurrentTimePeriod.CrossfadeLength < Map.MapEnvironment.CurrentHour)
                    {
                        CrossfadeOverlayWeather = OverlayWeather;
                        CrossfadeOverlayTypeOfSky = OverlayTypeOfSky;
                        CrossfadeOverlayVisibility = OverlayVisibility;
                    }

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

            if (CrossfadeOverlayTypeOfSky != null)
            {
                CrossfadeOverlayTypeOfSky.BeginDraw(g);
            }
            if (CrossfadeOverlayWeather != null)
            {
                CrossfadeOverlayWeather.BeginDraw(g);
            }
            if (CrossfadeOverlayVisibility != null)
            {
                CrossfadeOverlayVisibility.BeginDraw(g);
            }
        }

        public void Draw(CustomSpriteBatch g, int LayerIndex, bool IsSubLayer)
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

            if (CrossfadeOverlayTypeOfSky != null)
            {
                CrossfadeOverlayTypeOfSky.Draw(g);
            }
            if (CrossfadeOverlayWeather != null)
            {
                CrossfadeOverlayWeather.Draw(g);
            }
            if (CrossfadeOverlayVisibility != null)
            {
                CrossfadeOverlayVisibility.Draw(g);
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

            if (CrossfadeOverlayTypeOfSky != null)
            {
                CrossfadeOverlayTypeOfSky.EndDraw(g);
            }
            if (CrossfadeOverlayWeather != null)
            {
                CrossfadeOverlayWeather.EndDraw(g);
            }
            if (CrossfadeOverlayVisibility != null)
            {
                CrossfadeOverlayVisibility.EndDraw(g);
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
