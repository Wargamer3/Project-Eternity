using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptAddUnitFromRoster : BattleMapScript
        {
            private string _UnitEventID;
            private string[] ArrayCharacter;

            public ScriptAddUnitFromRoster()
                : this(null)
            {
            }

            public ScriptAddUnitFromRoster(BattleMap Map)
                : base(Map, 140, 70, "Add Unit From Roster", new string[] { "Add unit" }, new string[] { "Unit added" })
            {
                _UnitEventID = "";
                ArrayCharacter = new string[0];
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Map.PlayerRoster.AddUnitFromRoster(_UnitEventID, ArrayCharacter, Map.Params.DicUnitType, Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget, Map.Params.DicManualSkillTarget);
                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _UnitEventID = BR.ReadString();
                int ArrayCharacterNameLength = BR.ReadInt32();
                ArrayCharacter = new string[ArrayCharacterNameLength];

                for (int C = 0; C < ArrayCharacterNameLength; ++C)
                {
                    ArrayCharacter[C] = BR.ReadString();
                }
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_UnitEventID);
                BW.Write(ArrayCharacter.Length);
                for (int C = 0; C < ArrayCharacter.Length; ++C)
                {
                    BW.Write(ArrayCharacter[C]);
                }
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptAddUnitFromRoster(Map);
            }

            #region Properties

            [CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string UnitEventID
            {
                get
                {
                    return _UnitEventID;
                }
                set
                {
                    _UnitEventID = value;
                }
            }

            [Editor(typeof(Selectors.CharacterSelector), typeof(UITypeEditor)),
            TypeConverter(typeof(CsvConverter)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string[] Characters
            {
                get
                {
                    return ArrayCharacter;
                }
                set
                {
                    ArrayCharacter = value;
                }
            }
            #endregion
        }
    }
}
