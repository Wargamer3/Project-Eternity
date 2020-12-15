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
        public class ScriptRemoveSquad : BattleMapScript
        {
            private string _SquadName;

            public ScriptRemoveSquad()
                : this(null)
            {
                _SquadName = "";
            }

            public ScriptRemoveSquad(BattleMap Map)
                : base(Map, 140, 70, "Remove Squad", new string[] { "Remove squad" }, new string[] { "Squad removed" })
            {
                _SquadName = "";
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Map.PlayerRoster.TeamSquads.RemoveAll(_SquadName);
                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _SquadName = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_SquadName);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptRemoveSquad(Map);
            }

            #region Properties

            [Editor(typeof(Selectors.UnitSelector), typeof(UITypeEditor)),
            CategoryAttribute("Unit Attributes"),
            DescriptionAttribute(".")]
            public string SquadName
            {
                get
                {
                    return _SquadName;
                }
                set
                {
                    _SquadName = value;
                }
            }

            #endregion
        }
    }
}
