using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptAddCharacter : BattleMapScript
        {
            private string[] _CharacterName;
            private bool _IsPresent;
            private bool _IsEvent;

            public ScriptAddCharacter()
                : this(null)
            {
            }

            public ScriptAddCharacter(BattleMap Map)
                : base(Map, 140, 70, "Add Character", new string[] { "Add character" }, new string[] { "Character added" })
            {
                _CharacterName = new string[1];
                _IsPresent = true;
                _IsEvent = false;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                string[] ArrayCharacterName = _CharacterName[0].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int C = 0; C < ArrayCharacterName.Length; C++)
                {
                    Character NewCharacter = new Character(ArrayCharacterName[C], GameScreen.ContentFallback, Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget, Map.Params.DicManualSkillTarget);
                    if (_IsPresent)
                    {
                        NewCharacter.TeamTags.AddTag("Present");
                    }
                    if (_IsEvent)
                    {
                        NewCharacter.TeamTags.AddTag("Event");
                    }
                    Map.PlayerRoster.TeamCharacters.Add(NewCharacter);
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
                _CharacterName[0] = BR.ReadString();
                _IsPresent = BR.ReadBoolean();
                _IsEvent = BR.ReadBoolean();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_CharacterName[0]);
                BW.Write(_IsPresent);
                BW.Write(_IsEvent);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptAddCharacter(Map);
            }

            #region Properties

            [Editor(typeof(Selectors.CharacterSelector), typeof(UITypeEditor)),
            CategoryAttribute("Character Attributes"),
            DescriptionAttribute(".")]
            public string[] CharacterName
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

            [CategoryAttribute("Character Attributes"),
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

            [CategoryAttribute("Character Attributes"),
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
