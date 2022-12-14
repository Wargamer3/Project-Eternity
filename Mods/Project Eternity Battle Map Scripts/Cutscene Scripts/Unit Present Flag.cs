using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptUnitPresentFlag : BattleMapScript
        {
            private string _UnitName;
            private bool _IsPresent;

            public ScriptUnitPresentFlag()
                : this(null)
            {
            }

            public ScriptUnitPresentFlag(BattleMap Map)
                : base(Map, 140, 70, "Unit Present Flag", new string[] { "Set flag" }, new string[] { "Flag set" })
            {
                _UnitName = "";
                _IsPresent = false;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Unit UnitToAssign = null;
                foreach (Unit ActiveUnit in Map.PlayerRoster.TeamUnits.GetAll())
                {
                    if (ActiveUnit.RelativePath == _UnitName)
                    {
                        UnitToAssign = ActiveUnit;
                        break;
                    }
                }

                if (UnitToAssign.TeamTags.ContainsTag("Present") && !_IsPresent)
                {
                    UnitToAssign.TeamTags.RemoveTag("Present");
                }
                else if (!UnitToAssign.TeamTags.ContainsTag("Present") && _IsPresent)
                {
                    UnitToAssign.TeamTags.AddTag("Present");
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
                _UnitName = BR.ReadString();
                _IsPresent = BR.ReadBoolean();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_UnitName);
                BW.Write(_IsPresent);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptUnitPresentFlag(Map);
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

            [CategoryAttribute("Unit Attributes"),
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

            #endregion
        }
    }
}
