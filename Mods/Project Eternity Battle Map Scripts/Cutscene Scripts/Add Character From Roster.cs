using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptAddCharacterFromRoster : BattleMapScript
        {
            private string[] _ArrayCharacterName;

            public ScriptAddCharacterFromRoster()
                : this(null)
            {
            }

            public ScriptAddCharacterFromRoster(BattleMap Map)
                : base(Map, 140, 70, "Add Character From Roster", new string[] { "Add character" }, new string[] { "Character added" })
            {
                _ArrayCharacterName = new string[0];
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                for (int C = 0; C < _ArrayCharacterName.Length; ++C)
                {
                    Map.PlayerRoster.AddCharacterFromRoster(_ArrayCharacterName[C], Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget, Map.Params.DicManualSkillTarget);
                }
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                int ArrayCharacterNameLength = BR.ReadInt32();
                _ArrayCharacterName = new string[ArrayCharacterNameLength];

                for (int C = 0; C < ArrayCharacterNameLength; ++C)
                {
                    _ArrayCharacterName[C] = BR.ReadString();
                }
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_ArrayCharacterName.Length);
                for (int C = 0; C < _ArrayCharacterName.Length; ++C)
                {
                    BW.Write(_ArrayCharacterName[C]);
                }
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptAddCharacterFromRoster(Map);
            }

            #region Properties

            [Editor(typeof(Selectors.CharacterSelector), typeof(UITypeEditor)),
            CategoryAttribute("Character Attributes"),
            DescriptionAttribute(".")]
            public string[] CharacterNames
            {
                get
                {
                    return _ArrayCharacterName;
                }
                set
                {
                    _ArrayCharacterName = value;
                }
            }

            #endregion
        }
    }
}
