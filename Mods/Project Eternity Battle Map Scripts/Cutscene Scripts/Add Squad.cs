using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptAddSquad : BattleMapScript
        {
            private string _SquadName;
            private string _LeaderName;
            private string _WingmanAName;
            private string _WingmanBName;
            private string[] _LeaderCharacterName;
            private string[] _WingmanACharacterName;
            private string[] _WingmanBCharacterName;

            private bool _IsEventSquad;
            private bool _IsNameLocked;
            private bool _IsLeaderLocked;
            private bool _IsWingmanALocked;
            private bool _IsWingmanBLocked;

            private bool _IsPresent;
            private bool _IsEvent;

            public ScriptAddSquad()
                : this(null)
            {
                _SquadName = "";
                _LeaderName = "";
                _WingmanAName = "";
                _WingmanBName = "";

                _LeaderCharacterName = new string[0];
                _WingmanACharacterName = new string[0];
                _WingmanBCharacterName = new string[0];

                _IsEventSquad = false;
                _IsNameLocked = false;
                _IsLeaderLocked = false;
                _IsWingmanALocked = false;
                _IsWingmanBLocked = false;
            }

            public ScriptAddSquad(BattleMap Map)
                : base(Map, 140, 70, "Add Squad", new string[] { "Add squad" }, new string[] { "Squad added" })
            {
                _SquadName = "";
                _LeaderName = "";
                _WingmanAName = "";
                _WingmanBName = "";

                _LeaderCharacterName = new string[0];
                _WingmanACharacterName = new string[0];
                _WingmanBCharacterName = new string[0];

                _IsEventSquad = false;
                _IsNameLocked = false;
                _IsLeaderLocked = false;
                _IsWingmanALocked = false;
                _IsWingmanBLocked = false;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Unit NewLeader = null;
                Unit NewWingmanA = null;
                Unit NewWingmanB = null;

                List<Character> ListCharacterPresent = Map.PlayerRoster.TeamCharacters.GetPresent();

                List<Unit> ListUnitPresent = Map.PlayerRoster.TeamUnits.GetPresent();
                NewLeader = ListUnitPresent.Find(u => u.UnitTypeName + "\\" + u.RelativePath == _LeaderName);
                Map.PlayerRoster.TeamUnits.Remove(NewLeader);

                NewLeader.ArrayCharacterActive = new Character[_LeaderCharacterName.Length];
                for (int C = 0; C < _LeaderCharacterName.Length; C++)
                {
                    NewLeader.ArrayCharacterActive[C] = ListCharacterPresent.Find(c => c.FullName == _LeaderCharacterName[C]);
                    Map.PlayerRoster.TeamCharacters.Remove(NewLeader.ArrayCharacterActive[C]);
                }

                if (!string.IsNullOrEmpty(_WingmanAName))
                {
                    NewWingmanA = ListUnitPresent.Find(u => u.UnitTypeName + "\\" + u.RelativePath == _WingmanAName);
                    Map.PlayerRoster.TeamUnits.Remove(NewWingmanA);

                    NewWingmanA.ArrayCharacterActive = new Character[_WingmanACharacterName.Length];
                    for (int C = 0; C < _WingmanACharacterName.Length; C++)
                    {
                        NewWingmanA.ArrayCharacterActive[C] = ListCharacterPresent.Find(c => c.FullName == _WingmanACharacterName[C]);
                        Map.PlayerRoster.TeamCharacters.Remove(NewWingmanA.ArrayCharacterActive[C]);
                    }
                }

                if (!string.IsNullOrEmpty(_WingmanBName))
                {
                    NewWingmanB = ListUnitPresent.Find(u => u.UnitTypeName + "\\" + u.RelativePath == _WingmanBName);
                    Map.PlayerRoster.TeamUnits.Remove(NewWingmanB);

                    NewWingmanB.ArrayCharacterActive = new Character[_WingmanBCharacterName.Length];
                    for (int C = 0; C < _WingmanBCharacterName.Length; C++)
                    {
                        NewWingmanB.ArrayCharacterActive[C] = ListCharacterPresent.Find(c => c.FullName == _WingmanBCharacterName[C]);
                        Map.PlayerRoster.TeamCharacters.Remove(NewWingmanB.ArrayCharacterActive[C]);
                    }
                }

                Squad NewSquad = new Squad(_SquadName, NewLeader, NewWingmanA, NewWingmanB);
                NewSquad.IsEventSquad = _IsEventSquad;
                NewSquad.IsNameLocked = _IsNameLocked;
                NewSquad.IsLeaderLocked = _IsLeaderLocked;
                NewSquad.IsWingmanALocked = _IsWingmanALocked;
                NewSquad.IsWingmanBLocked = _IsWingmanBLocked;

                if (_IsPresent)
                {
                    NewSquad.TeamTags.AddTag("Present");
                }
                if (_IsEvent)
                {
                    NewSquad.TeamTags.AddTag("Event");
                }

                Map.PlayerRoster.TeamSquads.Add(NewSquad);

                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _SquadName = BR.ReadString();
                _LeaderName = BR.ReadString();
                _WingmanAName = BR.ReadString();
                _WingmanBName = BR.ReadString();

                int LeaderCharacterNameCount = BR.ReadInt32();
                _LeaderCharacterName = new string[LeaderCharacterNameCount];

                for (int C = 0; C < LeaderCharacterNameCount; C++)
                {
                    _LeaderCharacterName[C] = BR.ReadString();
                }

                int WingmanACharacterNameCount = BR.ReadInt32();
                _WingmanACharacterName = new string[WingmanACharacterNameCount];

                for (int C = 0; C < WingmanACharacterNameCount; C++)
                {
                    _WingmanACharacterName[C] = BR.ReadString();
                }

                int WingmanBCharacterNameCount = BR.ReadInt32();
                _WingmanBCharacterName = new string[WingmanBCharacterNameCount];

                for (int C = 0; C < WingmanBCharacterNameCount; C++)
                {
                    _WingmanBCharacterName[C] = BR.ReadString();
                }

                _IsEventSquad = BR.ReadBoolean();
                _IsNameLocked = BR.ReadBoolean();
                _IsLeaderLocked = BR.ReadBoolean();
                _IsWingmanALocked = BR.ReadBoolean();
                _IsWingmanBLocked = BR.ReadBoolean();
                _IsPresent = BR.ReadBoolean();
                _IsEvent = BR.ReadBoolean();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_SquadName);
                BW.Write(_LeaderName);
                BW.Write(_WingmanAName);
                BW.Write(_WingmanBName);

                BW.Write(_LeaderCharacterName.Length);
                for (int C = 0; C < _LeaderCharacterName.Length; C++)
                {
                    BW.Write(_LeaderCharacterName[C]);
                }
                BW.Write(_WingmanACharacterName.Length);
                for (int C = 0; C < _WingmanACharacterName.Length; C++)
                {
                    BW.Write(_WingmanACharacterName[C]);
                }
                BW.Write(_WingmanBCharacterName.Length);
                for (int C = 0; C < _WingmanBCharacterName.Length; C++)
                {
                    BW.Write(_WingmanBCharacterName[C]);
                }

                BW.Write(_IsEventSquad);
                BW.Write(_IsNameLocked);
                BW.Write(_IsLeaderLocked);
                BW.Write(_IsWingmanALocked);
                BW.Write(_IsWingmanBLocked);
                BW.Write(_IsPresent);
                BW.Write(_IsEvent);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptAddSquad(Map);
            }

            #region Properties

            [CategoryAttribute("Squad Attributes"),
            DescriptionAttribute(".")]
            public string SquadName
            {
                get
                {
                    return _SquadName;
                }
                set
                {
                    _SquadName = value;
                }
            }

            [Editor(typeof(Selectors.UnitSelector), typeof(UITypeEditor)),
            CategoryAttribute("Squad Attributes"),
            DescriptionAttribute(".")]
            public string LeaderName
            {
                get
                {
                    return _LeaderName;
                }
                set
                {
                    _LeaderName = value;
                }
            }

            [Editor(typeof(Selectors.UnitSelector), typeof(UITypeEditor)),
            CategoryAttribute("Squad Attributes"),
            DescriptionAttribute(".")]
            public string WingmanAName
            {
                get
                {
                    return _WingmanAName;
                }
                set
                {
                    _WingmanAName = value;
                }
            }

            [Editor(typeof(Selectors.UnitSelector), typeof(UITypeEditor)),
            CategoryAttribute("Squad Attributes"),
            DescriptionAttribute(".")]
            public string WingmanBName
            {
                get
                {
                    return _WingmanBName;
                }
                set
                {
                    _WingmanBName = value;
                }
            }

            [Editor(typeof(Selectors.CharacterSelector), typeof(UITypeEditor)),
            TypeConverter(typeof(CsvConverter)),
            CategoryAttribute("Squad Attributes"),
            DescriptionAttribute(".")]
            public string[] LeaderCharacterName
            {
                get
                {
                    return _LeaderCharacterName;
                }
                set
                {
                    _LeaderCharacterName = value;
                }
            }

            [Editor(typeof(Selectors.CharacterSelector), typeof(UITypeEditor)),
            TypeConverter(typeof(CsvConverter)),
            CategoryAttribute("Squad Attributes"),
            DescriptionAttribute(".")]
            public string[] WingmanACharacterName
            {
                get
                {
                    return _WingmanACharacterName;
                }
                set
                {
                    _WingmanACharacterName = value;
                }
            }

            [Editor(typeof(Selectors.CharacterSelector), typeof(UITypeEditor)),
            TypeConverter(typeof(CsvConverter)),
            CategoryAttribute("Squad Attributes"),
            DescriptionAttribute(".")]
            public string[] WingmanBCharacterName
            {
                get
                {
                    return _WingmanBCharacterName;
                }
                set
                {
                    _WingmanBCharacterName = value;
                }
            }

            [CategoryAttribute("Squad Attributes"),
            DescriptionAttribute(".")]
            public bool IsEventSquad
            {
                get
                {
                    return _IsEventSquad;
                }
                set
                {
                    _IsEventSquad = value;
                }
            }

            [CategoryAttribute("Squad Attributes"),
            DescriptionAttribute(".")]
            public bool IsNameLocked
            {
                get
                {
                    return _IsNameLocked;
                }
                set
                {
                    _IsNameLocked = value;
                }
            }

            [CategoryAttribute("Squad Attributes"),
            DescriptionAttribute(".")]
            public bool IsLeaderLocked
            {
                get
                {
                    return _IsLeaderLocked;
                }
                set
                {
                    _IsLeaderLocked = value;
                }
            }

            [CategoryAttribute("Squad Attributes"),
            DescriptionAttribute(".")]
            public bool IsWingmanALocked
            {
                get
                {
                    return _IsWingmanALocked;
                }
                set
                {
                    _IsWingmanALocked = value;
                }
            }

            [CategoryAttribute("Squad Attributes"),
            DescriptionAttribute(".")]
            public bool IsWingmanBLocked
            {
                get
                {
                    return _IsWingmanBLocked;
                }
                set
                {
                    _IsWingmanBLocked = value;
                }
            }

            [CategoryAttribute("Squad Attributes"),
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

            [CategoryAttribute("Squad Attributes"),
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
