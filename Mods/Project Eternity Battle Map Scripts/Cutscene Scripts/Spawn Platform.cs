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
        public class ScriptSpawnPlaform : BattleMapScript
        {
            private string _SkillPoint;

            public ScriptSpawnPlaform()
                : this(null)
            {
                _SkillPoint = "";
            }

            public ScriptSpawnPlaform(BattleMap Map)
                : base(Map, 100, 50, "Spawn Platform", new string[] { "Set text" }, new string[] { "Text set" })
            {
                _SkillPoint = "";
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Map.SkillPoint = _SkillPoint;
                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _SkillPoint = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_SkillPoint);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSetSkillPoint(Map);
            }

            #region Properties

            [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor)),
            CategoryAttribute("Objective attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public string SkillPoint
            {
                get
                {
                    return _SkillPoint;
                }
                set
                {
                    _SkillPoint = value;
                }
            }

            #endregion
        }
    }
}
