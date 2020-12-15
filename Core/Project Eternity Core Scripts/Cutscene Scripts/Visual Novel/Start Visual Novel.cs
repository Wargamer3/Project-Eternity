using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.Core.Scripts
{

    #region Visual Novel

    public sealed partial class VisualNovelCutsceneScriptHolder
    {
        public class ScriptStartVisualNovel : CutsceneActionScript
        {
            private UInt32 _TargetID;
            private VisualNovel ActiveVisualNovel;
            private ScriptVisualNovel scriptVisualNovel;

            public ScriptStartVisualNovel()
                : base(140, 70, "Start Visual Novel", new string[] { "Start" }, new string[] { "Frame Changed", "Paused", "Resumed", "Visual Novel Ended" })
            {
                _TargetID = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                switch (Index)
                {
                    case 0:
                        scriptVisualNovel = (ScriptVisualNovel)GetDataContainerByID(_TargetID, ScriptVisualNovel.ScriptName);
                        if (scriptVisualNovel != null)
                        {
                            ActiveVisualNovel = scriptVisualNovel.ActiveVisualNovel = new VisualNovel(scriptVisualNovel.VisualNovelName);
                            scriptVisualNovel.ActiveVisualNovel.OnVisualNovelFrameChanged += OnVisualNovelFrameChanged;
                            scriptVisualNovel.ActiveVisualNovel.OnVisualNovelPaused = OnVisualNovelPaused;
                            scriptVisualNovel.ActiveVisualNovel.OnVisualNovelResumed = OnVisualNovelResumed;
                            scriptVisualNovel.ActiveVisualNovel.OnVisualNovelEnded = OnVisualNovelEnded;
                            Owner.PushScreen(scriptVisualNovel.ActiveVisualNovel);
                            break;
                        }
                        break;
                }
                IsActive = true;
            }

            public void OnVisualNovelFrameChanged()
            {
                ExecuteEvent(this, 0);
            }

            public void OnVisualNovelPaused()
            {
                ExecuteEvent(this, 1);
            }

            public void OnVisualNovelResumed()
            {
                ExecuteEvent(this, 2);
            }

            public void OnVisualNovelEnded()
            {
                scriptVisualNovel.ActiveVisualNovel.OnVisualNovelFrameChanged -= OnVisualNovelFrameChanged;
                ExecuteEvent(this, 3);
                IsEnded = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
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
                return new ScriptStartVisualNovel();
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
