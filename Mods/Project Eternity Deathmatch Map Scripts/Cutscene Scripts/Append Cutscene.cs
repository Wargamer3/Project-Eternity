using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptAppendCutscene : DeathmatchMapScript
        {
            private string _CutsceneName;

            public ScriptAppendCutscene()
                : this(null)
            {
                _CutsceneName = "";
            }

            public ScriptAppendCutscene(DeathmatchMap Map)
                : base(Map, 140, 70, "Append Cutscene", new string[] { "Append cutscene" }, new string[] { "Cutscene loaded" })
            {
                _CutsceneName = "";
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                IsEnded = true;
                Cutscene NewCutscene = new Cutscene(Map.CenterCamera, _CutsceneName, Owner.DicCutsceneScript);
                NewCutscene.Load();
                Owner.AppendCutscene(NewCutscene);
                ExecuteEvent(this, 0);
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                CutsceneName = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(CutsceneName);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptAppendCutscene(Map);
            }

            #region Properties

            [Editor(typeof(Selectors.CutsceneSelector), typeof(UITypeEditor)),
            CategoryAttribute("Cutscene behavior"),
            DescriptionAttribute("The cutscene path"),
            DefaultValueAttribute("")]
            public string CutsceneName
            {
                get
                {
                    return _CutsceneName;
                }
                set
                {
                    _CutsceneName = value;
                }
            }

            #endregion
        }
    }
}
