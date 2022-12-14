using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Scripts
{

    #region Visual Novel

    public sealed partial class VisualNovelCutsceneScriptHolder
    {
        public class ScriptVisualNovelFrameReached : CutsceneActionScript
        {
            private UInt32 _TargetID;
            private int _FrameReached;

            public ScriptVisualNovelFrameReached()
                : base(160, 50, "Visual Novel Frame Reached", new string[] { "Check Frame" }, new string[] { "Frame Reached" })
            {
                _TargetID = 0;
                _FrameReached = 1;
            }

            public override void ExecuteTrigger(int Index)
            {
                switch (Index)
                {
                    case 0:
                        ScriptVisualNovel scriptUnit = (ScriptVisualNovel)GetDataContainerByID(_TargetID, ScriptVisualNovel.ScriptName);
                        if (scriptUnit != null)
                        {
                            if (scriptUnit.ActiveVisualNovel.TimelineIndex == _FrameReached - 1)
                            {
                                ExecuteEvent(this, 0);
                                IsEnded = true;
                            }
                            break;
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
                FrameReached = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(TargetID);
                BW.Write(FrameReached);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptVisualNovelFrameReached();
            }

            #region Properties

            [CategoryAttribute("Visual Novel"),
            DescriptionAttribute("The targeted Visual Novel."),
            DefaultValueAttribute("0")]
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

            [CategoryAttribute("Visual Novel"),
            DescriptionAttribute("The Frame number to check (Starting at 0)."),
            DefaultValueAttribute("1")]
            public int FrameReached
            {
                get
                {
                    return _FrameReached;
                }
                set
                {
                    _FrameReached = value;
                }
            }

            #endregion
        }
    }

    #endregion
}
