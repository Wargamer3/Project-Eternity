using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptRemoveUnit : BattleMapScript
        {
            private string _UnitName;

            public ScriptRemoveUnit()
                : this(null)
            {
                _UnitName = "";
            }

            public ScriptRemoveUnit(BattleMap Map)
                : base(Map, 140, 70, "Remove Unit", new string[] { "Remove unit" }, new string[] { "Unit removed" })
            {
                _UnitName = "";
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Map.PlayerRoster.TeamUnits.RemoveAll(_UnitName);
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
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_UnitName);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptRemoveUnit(Map);
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

            #endregion
        }
    }
}
