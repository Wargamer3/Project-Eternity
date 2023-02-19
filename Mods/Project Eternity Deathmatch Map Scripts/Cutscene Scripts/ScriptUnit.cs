using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptUnit : DeathmatchMapDataContainer
        {
            internal string _SpawnUnitName;
            internal int _SpawnUnitStatsUpgrade;
            internal int _SpawnUnitAttackUpgrade;
            internal int _SpawnCharacterLevel;
            internal string[] _SpawnCharacter;
            private Unit _SpawnUnit;
            private bool _DeleteAfterMission;

            [Browsable(false)]
            public Unit SpawnUnit { get { return _SpawnUnit; } }

            public static readonly string ScriptName = "Unit";

            public ScriptUnit()
                : this(null)
            {
            }

            public ScriptUnit(DeathmatchMap Map)
                : base(Map, 100, 50, ScriptName)
            {
                _SpawnUnitName = "";
                _SpawnUnitStatsUpgrade = 0;
                _SpawnUnitAttackUpgrade = 0;
                _SpawnCharacterLevel = 1;
                _SpawnCharacter = new string[0];
                DeleteAfterMission = true;
            }

            public void Init()
            {
                ContentManager Content = null;

                if (DeleteAfterMission)
                {
                    if (Map != null)
                        Content = Map.Content;
                }
                else
                {
                    Content = GameScreen.ContentFallback;
                }

                if (!string.IsNullOrEmpty(_SpawnUnitName))
                {
                    string[] UnitInfo = _SpawnUnitName.Split(new[] { "/" }, StringSplitOptions.None);
                    _SpawnUnit = Unit.FromType(UnitInfo[0], _SpawnUnitName.Remove(0, UnitInfo[0].Length + 1), Content, Map.Params.DicUnitType, Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget);

                    List<Character> ListUnitCharacter = new List<Character>();
                    for (int C = 0; C < _SpawnCharacter.Length; C++)
                    {
                        Character NewCharacter = new Character(_SpawnCharacter[C], Content, Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget, Map.Params.DicManualSkillTarget);
                        NewCharacter.Level = _SpawnCharacterLevel;
                        ListUnitCharacter.Add(NewCharacter);
                        if (NewCharacter.Slave != null)
                        {
                            NewCharacter.Slave.Level = _SpawnCharacterLevel;
                            ListUnitCharacter.Add(NewCharacter.Slave);
                        }
                    }

                    _SpawnUnit.ArrayCharacterActive = ListUnitCharacter.ToArray();
                    if (_SpawnUnit.Pilot != null)
                    {
                        _SpawnUnit.Pilot.Level = _SpawnCharacterLevel;
                    }
                    _SpawnUnit.UnitStat.HPUpgrades.Value = _SpawnUnitStatsUpgrade;
                    _SpawnUnit.UnitStat.ENUpgrades.Value = _SpawnUnitStatsUpgrade;
                    _SpawnUnit.UnitStat.ArmorUpgrades.Value = _SpawnUnitStatsUpgrade;
                    _SpawnUnit.UnitStat.MobilityUpgrades.Value = _SpawnUnitStatsUpgrade;

                    _SpawnUnit.UnitStat.AttackUpgrades.Value = SpawnUnitAttackUpgrade;
                }
            }

            public void OverrideUnit(Unit NewUnit)
            {
                _SpawnUnit = NewUnit;
            }

            public override void Load(BinaryReader BR)
            {
                SpawnUnitName = BR.ReadString();
                _SpawnUnitStatsUpgrade = BR.ReadInt32();
                _SpawnUnitAttackUpgrade = BR.ReadInt32();
                _SpawnCharacterLevel = BR.ReadInt32();

                int SpawnCharacterCount = BR.ReadInt32();
                _SpawnCharacter = new string[SpawnCharacterCount];

                for (int C = 0; C < SpawnCharacterCount; C++)
                    _SpawnCharacter[C] = BR.ReadString();

                _DeleteAfterMission = BR.ReadBoolean();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_SpawnUnitName);
                BW.Write(_SpawnUnitStatsUpgrade);
                BW.Write(_SpawnUnitAttackUpgrade);
                BW.Write(_SpawnCharacterLevel);

                BW.Write(_SpawnCharacter.Length);
                for (int C = 0; C < _SpawnCharacter.Length; C++)
                {
                    BW.Write(_SpawnCharacter[C]);
                }
                BW.Write(_DeleteAfterMission);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptUnit(Map);
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
            public int SpawnUnitStatsUpgrade
            {
                get
                {
                    return _SpawnUnitStatsUpgrade;
                }
                set
                {
                    _SpawnUnitStatsUpgrade = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public int SpawnUnitAttackUpgrade
            {
                get
                {
                    return _SpawnUnitAttackUpgrade;
                }
                set
                {
                    _SpawnUnitAttackUpgrade = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public int SpawnCharacterLevel
            {
                get
                {
                    return _SpawnCharacterLevel;
                }
                set
                {
                    if (value < 1)
                        _SpawnCharacterLevel = 1;
                    else
                        _SpawnCharacterLevel = value;
                }
            }

            [Editor(typeof(Selectors.CharacterSelector), typeof(UITypeEditor)),
            TypeConverter(typeof(CsvConverter)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string[] SpawnCharacter
            {
                get
                {
                    return _SpawnCharacter;
                }
                set
                {
                    _SpawnCharacter = value;
                }
            }

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute("Destroy the Unit after the mission is completed. If true, this Unit will not be allowed to be used outside of the active Battle Map.")]
            public bool DeleteAfterMission
            {
                get
                {
                    return _DeleteAfterMission;
                }
                set
                {
                    _DeleteAfterMission = value;
                }
            }

            #endregion
        }
    }
}
