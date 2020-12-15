using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptDefineGlobalPilotUnit : CutsceneDataContainer
        {
            internal string _SpawnUnitName;
            internal string[] _SpawnCharacter;
            private Unit _SpawnUnit;
            private bool _DeleteAfterMission;
            private BattleMap Map;

            [Browsable(false)]
            public Unit SpawnUnit { get { return _SpawnUnit; } }

            public static readonly string ScriptName = "Global Unit and Pilot";

            public ScriptDefineGlobalPilotUnit()
                : base(100, 50, ScriptName)
            {
                _SpawnUnitName = "";
                _SpawnCharacter = new string[0];
                DeleteAfterMission = true;
            }

            public ScriptDefineGlobalPilotUnit(BattleMap Map)
                : base(100, 50, ScriptName)
            {
                this.Map = Map;
                _SpawnUnitName = "";
                _SpawnCharacter = new string[0];
                DeleteAfterMission = true;
            }

            public override void Load(BinaryReader BR)
            {
                SpawnUnitName = BR.ReadString();

                int SpawnCharacterCount = BR.ReadInt32();
                _SpawnCharacter = new string[SpawnCharacterCount];

                for (int C = 0; C < SpawnCharacterCount; C++)
                    _SpawnCharacter[C] = BR.ReadString();

                _DeleteAfterMission = BR.ReadBoolean();

                Microsoft.Xna.Framework.Content.ContentManager Content = GameScreen.ContentFallback;

                if (Content != null && !string.IsNullOrEmpty(_SpawnUnitName))
                {
                    string[] UnitInfo = _SpawnUnitName.Split(new[] { "\\" }, StringSplitOptions.None);
                    _SpawnUnit = Unit.FromType(UnitInfo[0], _SpawnUnitName.Remove(0, UnitInfo[0].Length + 1), Content, Map.DicUnitType, Map.DicRequirement, Map.DicEffect);

                    _SpawnUnit.ArrayCharacterActive = new Character[_SpawnCharacter.Length];
                    for (int C = 0; C < _SpawnCharacter.Length; C++)
                    {
                        _SpawnUnit.ArrayCharacterActive[C] = new Character(_SpawnCharacter[C], Content, Map.DicRequirement, Map.DicEffect);
                    }
                }
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_SpawnUnitName);
                BW.Write(_SpawnCharacter.Length);
                for (int C = 0; C < _SpawnCharacter.Length; C++)
                {
                    BW.Write(_SpawnCharacter[C]);
                }
                BW.Write(_DeleteAfterMission);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptDefineGlobalPilotUnit(Map);
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
            DescriptionAttribute("Destroy the Unit after the mission is completed. If true, this Unit will not be allowed to be used outside of the active Battle DeathmatchMap.ActiveDeathmatchMap.")]
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
