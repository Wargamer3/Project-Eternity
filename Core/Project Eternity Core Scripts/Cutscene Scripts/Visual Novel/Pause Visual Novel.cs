using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Scripts
{

    #region Visual Novel

    public sealed partial class VisualNovelCutsceneScriptHolder
    {
        public class ScriptPauseVisualNovel : CutsceneActionScript
        {
            private UInt32 _TargetID;

            public ScriptPauseVisualNovel()
                : base(120, 50, "Pause Visual Novel", new string[] { "Pause Visual Novel" }, new string[0])
            {
                _TargetID = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                switch (Index)
                {
                    case 0:
                        ScriptVisualNovel scriptUnit = (ScriptVisualNovel)GetDataContainerByID(_TargetID, ScriptVisualNovel.ScriptName);
                        if (scriptUnit != null)
                        {
                            scriptUnit.ActiveVisualNovel.IsPaused = true;
                            scriptUnit.ActiveVisualNovel.OnVisualNovelPaused();
                            IsEnded = true;
                        }
                        else
                        {
                            throw new Exception("Visual Novel " + _TargetID + " not found.");
                        }
                        break;
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                throw new NotImplementedException();
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                TargetID = BR.ReadUInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(TargetID);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptPauseVisualNovel();
            }

            #region Properties

            [CategoryAttribute("Visual Novel"),
            DescriptionAttribute("The targeted Visual Novel."),
            DefaultValueAttribute("")]
            public UInt32 TargetID
            {
                get
                {
                    return _TargetID;
                }
                set
                {
                    _TargetID = value;
                }
            }

            #endregion
        }
    }

    #endregion
}
