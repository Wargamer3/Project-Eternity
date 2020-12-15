using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptRoadSelection : BattleMapScript
        {
            private string _RouteName;
            private RouteMenu.RouteParams _RoadA;
            private RouteMenu.RouteParams _RoadB;
            private RouteMenu.RouteParams _RoadC;
            private RouteMenu.RouteParams _RoadD;

            private List<RouteMenu.RouteParams> ListChoice;
            private RouteMenu ActiveRoute;

            public ScriptRoadSelection()
                : this(null)
            {
                _RouteName = "";
                _RoadA = new RouteMenu.RouteParams("", "", "");
                _RoadB = new RouteMenu.RouteParams("", "", "");
                _RoadC = new RouteMenu.RouteParams("", "", "");
                _RoadD = new RouteMenu.RouteParams("", "", "");
            }

            public ScriptRoadSelection(BattleMap Map)
                : base(Map, 140, 80, "Road Selection", new string[] { "Open Selection" }, new string[] { "Road A", "Road B", "Road C", "Road D" })
            {
                _RouteName = "";
                _RoadA = new RouteMenu.RouteParams("", "", "");
                _RoadB = new RouteMenu.RouteParams("", "", "");
                _RoadC = new RouteMenu.RouteParams("", "", "");
                _RoadD = new RouteMenu.RouteParams("", "", "");
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
                IsDrawn = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (ActiveRoute == null)
                {
                    ListChoice = new List<RouteMenu.RouteParams>(4);

                    if (!string.IsNullOrEmpty(_RoadA.Title))
                        ListChoice.Add(_RoadA);

                    if (!string.IsNullOrEmpty(_RoadB.Title))
                        ListChoice.Add(_RoadB);

                    if (!string.IsNullOrEmpty(_RoadC.Title))
                        ListChoice.Add(_RoadC);

                    if (!string.IsNullOrEmpty(_RoadD.Title))
                        ListChoice.Add(_RoadD);

                    ActiveRoute = new RouteMenu(_RouteName, ListChoice);
                    ActiveRoute.Load();
                }
                else
                {
                    ActiveRoute.Update(gameTime);
                    if (ActiveRoute.SelectionFinalized)
                    {
                        ExecuteEvent(this, ActiveRoute.CursorSelection);
                        IsEnded = true;

                        if (BattleMap.DicRouteChoices.ContainsKey(ActiveRoute.RouteName))
                            BattleMap.DicRouteChoices.Add(ActiveRoute.RouteName, ActiveRoute.CursorSelection);
                        else
                            BattleMap.DicRouteChoices[ActiveRoute.RouteName] = ActiveRoute.CursorSelection;

                        ActiveRoute = null;
                    }
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                if (ActiveRoute != null)
                    ActiveRoute.Draw(g);
            }

            public override void Load(BinaryReader BR)
            {
                RouteName = BR.ReadString();
                RoadA = new RouteMenu.RouteParams(BR.ReadString(), BR.ReadString(), BR.ReadString());
                RoadB = new RouteMenu.RouteParams(BR.ReadString(), BR.ReadString(), BR.ReadString());
                RoadC = new RouteMenu.RouteParams(BR.ReadString(), BR.ReadString(), BR.ReadString());
                RoadD = new RouteMenu.RouteParams(BR.ReadString(), BR.ReadString(), BR.ReadString());
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(RouteName);
                BW.Write(RoadA.Title);
                BW.Write(RoadA.Summary);
                BW.Write(RoadA.Description);
                BW.Write(RoadB.Title);
                BW.Write(RoadB.Summary);
                BW.Write(RoadB.Description);
                BW.Write(RoadC.Title);
                BW.Write(RoadC.Summary);
                BW.Write(RoadC.Description);
                BW.Write(RoadD.Title);
                BW.Write(RoadD.Summary);
                BW.Write(RoadD.Description);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptRoadSelection(Map);
            }

            #region Properties

            [CategoryAttribute("Road choices"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public string RouteName
            {
                get
                {
                    return _RouteName;
                }
                set
                {
                    _RouteName = value;
                }
            }

            [Editor(typeof(RouteMenuSelector), typeof(UITypeEditor)),
            CategoryAttribute("Road choices"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public RouteMenu.RouteParams RoadA
            {
                get
                {
                    return _RoadA;
                }
                set
                {
                    _RoadA = value;
                }
            }

            [Editor(typeof(RouteMenuSelector), typeof(UITypeEditor)),
            CategoryAttribute("Road choices"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public RouteMenu.RouteParams RoadB
            {
                get
                {
                    return _RoadB;
                }
                set
                {
                    _RoadB = value;
                }
            }

            [Editor(typeof(RouteMenuSelector), typeof(UITypeEditor)),
            CategoryAttribute("Road choices"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public RouteMenu.RouteParams RoadC
            {
                get
                {
                    return _RoadC;
                }
                set
                {
                    _RoadC = value;
                }
            }

            [Editor(typeof(RouteMenuSelector), typeof(UITypeEditor)),
            CategoryAttribute("Road choices"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public RouteMenu.RouteParams RoadD
            {
                get
                {
                    return _RoadD;
                }
                set
                {
                    _RoadD = value;
                }
            }

            #endregion
        }
    }
}
