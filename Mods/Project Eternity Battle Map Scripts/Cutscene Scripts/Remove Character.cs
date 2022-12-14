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
        public class ScriptRemoveCharacter : BattleMapScript
        {
            private string _CharacterName;

            public ScriptRemoveCharacter()
                : this(null)
            {
                _CharacterName = "";
            }

            public ScriptRemoveCharacter(BattleMap Map)
                : base(Map, 140, 70, "Remove Character", new string[] { "Remove character" }, new string[] { "Character removed" })
            {
                _CharacterName = "";
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Map.PlayerRoster.TeamCharacters.RemoveAll(_CharacterName);
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
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_CharacterName);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptRemoveCharacter(Map);
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

            #endregion
        }
    }
}
