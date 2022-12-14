using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptUnitEventFlag : BattleMapScript
        {
            private string _CharacterName;
            private string _UnitName;
            private string _EventID;

            public ScriptUnitEventFlag()
                : this(null)
            {
            }

            public ScriptUnitEventFlag(BattleMap Map)
                : base(Map, 140, 70, "Unit Event Flag", new string[] { "Set flag" }, new string[] { "Flag set" })
            {
                _CharacterName = "";
                _UnitName = "";
                _EventID = "";
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Character CharacterToAssign = null;

                Unit UnitToAssign = null;
                foreach (Unit ActiveUnit in Map.PlayerRoster.TeamUnits.GetAll())
                {
                    if (ActiveUnit.RelativePath == _UnitName)
                    {
                        UnitToAssign = ActiveUnit;
                        break;
                    }
                }
                
                if (!UnitToAssign.TeamTags.ContainsTag("Event"))
                {
                    UnitToAssign.TeamTags.AddTag("Event");
                }

                for(int C = 0; C < UnitToAssign.ArrayCharacterActive.Length; ++C)
                {
                    if (UnitToAssign.ArrayCharacterActive[C].FullName == _CharacterName)
                    {
                        CharacterToAssign = UnitToAssign.ArrayCharacterActive[C];
                        break;
                    }
                }

                if (CharacterToAssign == null)
                {
                    foreach (Character ActiveCharacter in Map.PlayerRoster.TeamCharacters.GetAll())
                    {
                        if (ActiveCharacter.FullName == _CharacterName)
                        {
                            CharacterToAssign = ActiveCharacter;

                            Map.PlayerRoster.TeamCharacters.Add(UnitToAssign.ArrayCharacterActive[0]);
                            UnitToAssign.ArrayCharacterActive[0] = CharacterToAssign;
                            Map.PlayerRoster.TeamCharacters.Remove(CharacterToAssign);
                            break;
                        }
                    }
                }

                UnitToAssign.ID = EventID;

                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _CharacterName = BR.ReadString();
                _UnitName = BR.ReadString();
                _EventID = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_CharacterName);
                BW.Write(_UnitName);
                BW.Write(_EventID);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptUnitEventFlag(Map);
            }

            #region Properties

            [Editor(typeof(Selectors.UnitSelector), typeof(UITypeEditor)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string SpawnUnitName
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

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string EventID
            {
                get
                {
                    return _EventID;
                }
                set
                {
                    _EventID = value;
                }
            }

            #endregion
        }
    }
}
