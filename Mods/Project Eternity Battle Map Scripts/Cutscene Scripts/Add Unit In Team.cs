using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptCreateUnitInTeam : BattleMapScript
        {
            private string _SpawnUnitName;
            private bool _IsPresent;
            private bool _IsEvent;

            public ScriptCreateUnitInTeam()
                : this(null)
            {
            }

            public ScriptCreateUnitInTeam(BattleMap Map)
                : base(Map, 140, 70, "Create Unit In Team", new string[] { "Add unit" }, new string[] { "Unit added" })
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (!string.IsNullOrEmpty(_SpawnUnitName))
                {
                    string[] UnitInfo = _SpawnUnitName.Split(new[] { "/" }, StringSplitOptions.None);
                    Unit NewUnit = Unit.FromType(UnitInfo[0], _SpawnUnitName.Remove(0, UnitInfo[0].Length + 1), GameScreen.ContentFallback, Map.Params.DicUnitType, Map.Params.DicRequirement,
                        Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget);

                    if (_IsPresent)
                    {
                        NewUnit.TeamTags.AddTag("Present");
                    }
                    if (_IsEvent)
                    {
                        NewUnit.TeamTags.AddTag("Event");
                    }
                    Map.PlayerRoster.TeamUnits.Add(NewUnit);

                    ExecuteEvent(this, 0);
                    IsEnded = true;
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _SpawnUnitName = BR.ReadString();
                _IsPresent = BR.ReadBoolean();
                _IsEvent = BR.ReadBoolean();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_SpawnUnitName);
                BW.Write(_IsPresent);
                BW.Write(_IsEvent);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptCreateUnitInTeam(Map);
            }

            #region Properties


            [Editor(typeof(Selectors.UnitSelector), typeof(UITypeEditor)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string SpawnUnitName
            {
                get
                {
                    return _SpawnUnitName;
                }
                set
                {
                    _SpawnUnitName = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public bool IsPresent
            {
                get
                {
                    return _IsPresent;
                }
                set
                {
                    _IsPresent = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public bool IsEvent
            {
                get
                {
                    return _IsEvent;
                }
                set
                {
                    _IsEvent = value;
                }
            }

            #endregion
        }
    }
}
