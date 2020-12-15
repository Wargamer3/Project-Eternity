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
        public class ScriptSetNextMap : BattleMapScript
        {
            private string _NextMap;
            private string _NextMapType;

            public ScriptSetNextMap()
                : this(null)
            {
                _NextMap = "";
                _NextMapType = "Deathmatch";
            }

            public ScriptSetNextMap(BattleMap Map)
                : base(Map, 100, 50, "Set Next Map", new string[] { "Set map" }, new string[] { "Map set" })
            {
                _NextMap = "";
                _NextMapType = "Deathmatch";
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                BattleMap.NextMapPath = _NextMap;
                BattleMap.NextMapType = _NextMapType;
                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _NextMap = BR.ReadString();
                _NextMapType = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_NextMap);
                BW.Write(_NextMapType);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSetNextMap(Map);
            }

            #region Properties

            [Editor(typeof(Selectors.MapSelector), typeof(UITypeEditor)),
            CategoryAttribute("BattleMap attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public string NextMap
            {
                get
                {
                    return _NextMap;
                }
                set
                {
                    _NextMap = value;
                }
            }

            [CategoryAttribute("BattleMap attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public string NextMapType
            {
                get
                {
                    return _NextMapType;
                }
                set
                {
                    _NextMapType = value;
                }
            }

            #endregion
        }
    }
}
