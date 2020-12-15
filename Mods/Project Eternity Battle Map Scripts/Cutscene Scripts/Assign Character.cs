using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptAssignCharacter : BattleMapScript
        {
            private string _UnitName;
            private string _CharacterName;

            public ScriptAssignCharacter()
                : this(null)
            {
            }

            public ScriptAssignCharacter(BattleMap Map)
                : base(Map, 140, 70, "Assign Character", new string[] { "Assign character" }, new string[] { "Character assigned" })
            {
                _UnitName = "";
                _CharacterName = "";
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Character CharacterToAssign = null;
                foreach (Character ActiveCharacter in Map.PlayerRoster.TeamCharacters.GetAll())
                {
                    if (ActiveCharacter.FullName == _CharacterName)
                    {
                        CharacterToAssign = ActiveCharacter;
                        break;
                    }
                }

                Unit UnitToAssign = null;
                foreach (Unit ActiveUnit in Map.PlayerRoster.TeamUnits.GetAll())
                {
                    if (ActiveUnit.FullName == _UnitName)
                    {
                        UnitToAssign = ActiveUnit;
                        break;
                    }
                }

                Map.PlayerRoster.TeamCharacters.Add(UnitToAssign.ArrayCharacterActive[0]);
                UnitToAssign.ArrayCharacterActive[0] = CharacterToAssign;
                Map.PlayerRoster.TeamCharacters.Remove(CharacterToAssign);

                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _UnitName = BR.ReadString();
                _CharacterName = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_UnitName);
                BW.Write(_CharacterName);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptAssignCharacter(Map);
            }

            #region Properties

            [Editor(typeof(Selectors.UnitSelector), typeof(UITypeEditor)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string UnitName
            {
                get
                {
                    return _UnitName;
                }
                set
                {
                    _UnitName = value;
                }
            }

            [Editor(typeof(Selectors.CharacterSelector), typeof(UITypeEditor)),
            CategoryAttribute("Character Attributes"),
            DescriptionAttribute(".")]
            public string CharacterName
            {
                get
                {
                    return _CharacterName;
                }
                set
                {
                    _CharacterName = value;
                }
            }

            #endregion
        }
    }
}
