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
        public class ScriptCharacterEventFlag : BattleMapScript
        {
            private string _CharacterName;
            private string _EventID;

            public ScriptCharacterEventFlag()
                : this(null)
            {
            }

            public ScriptCharacterEventFlag(BattleMap Map)
                : base(Map, 140, 70, "Character Event Flag", new string[] { "Set flag" }, new string[] { "Flag set" })
            {
                _CharacterName = "";
                _EventID = "";
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

                if (CharacterToAssign == null)
                {
                    foreach (Unit ActiveUnit in Map.PlayerRoster.TeamUnits.GetAll())
                    {
                        foreach (Character ActiveCharacter in ActiveUnit.ArrayCharacterActive)
                        {
                            if (ActiveCharacter.FullName == _CharacterName)
                            {
                                CharacterToAssign = ActiveCharacter;

                                ActiveUnit.ID = EventID;
                                break;
                            }
                        }
                    }
                }

                if (!CharacterToAssign.TeamTags.ContainsTag("Event"))
                {
                    CharacterToAssign.TeamTags.AddTag("Event");
                }

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
                _EventID = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_CharacterName);
                BW.Write(_EventID);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptCharacterEventFlag(Map);
            }

            #region Properties
            
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
