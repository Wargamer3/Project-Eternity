using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptStartDeathmatchVisualNovel : DeathmatchMapScript
        {
            private UInt32 _TargetID;
            private VisualNovel ActiveVisualNovel;
            private VisualNovelCutsceneScriptHolder.ScriptVisualNovel scriptVisualNovel;

            public ScriptStartDeathmatchVisualNovel()
                : this(null)
            {
            }

            public ScriptStartDeathmatchVisualNovel(DeathmatchMap Map)
                : base(Map, 170, 70, "Start Deathmatch Visual Novel", new string[] { "Start" }, new string[] { "Frame Changed", "Paused", "Resumed", "Visual Novel Ended" })
            {
                _TargetID = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                switch (Index)
                {
                    case 0:
                        scriptVisualNovel = (VisualNovelCutsceneScriptHolder.ScriptVisualNovel)GetDataContainerByID(_TargetID, VisualNovelCutsceneScriptHolder.ScriptVisualNovel.ScriptName);
                        if (scriptVisualNovel != null)
                        {
                            ActiveVisualNovel = scriptVisualNovel.ActiveVisualNovel = new VisualNovel(scriptVisualNovel.VisualNovelName, Map.DicCutsceneScript);
                            scriptVisualNovel.ActiveVisualNovel.OnVisualNovelFrameChanged = OnVisualNovelFrameChanged;
                            scriptVisualNovel.ActiveVisualNovel.OnVisualNovelPaused = OnVisualNovelPaused;
                            scriptVisualNovel.ActiveVisualNovel.OnVisualNovelResumed = OnVisualNovelResumed;
                            scriptVisualNovel.ActiveVisualNovel.OnVisualNovelEnded = OnVisualNovelEnded;
                            Owner.PushScreen(scriptVisualNovel.ActiveVisualNovel);
                            CenterCameraOnSpeaker();
                            break;
                        }
                        break;
                }

                IsActive = true;
            }

            public void OnVisualNovelFrameChanged()
            {
                CenterCameraOnSpeaker();
                ExecuteEvent(this, 0);
            }

            private List<SpeakerPriority> GetSpeakerPriorities()
            {
                List<SpeakerPriority> ListSpeakerPriority = new List<SpeakerPriority>();

                Dialog ActiveDialog = scriptVisualNovel.ActiveVisualNovel.CurrentDialog;
                if (scriptVisualNovel.ActiveVisualNovel.CurrentDialog.OverrideCharacterPriority)
                {
                    ListSpeakerPriority = scriptVisualNovel.ActiveVisualNovel.CurrentDialog.ListSpeakerPriority;
                }
                else
                {
                    int CharacterIndex = 0;
                    if (ActiveDialog.ActiveBustCharacterState == Dialog.ActiveBustCharacterStates.Left && ActiveDialog.LeftCharacter != null)
                    {
                        scriptVisualNovel.ActiveVisualNovel.GetBustPortraitIndices(ActiveDialog.LeftCharacter, out CharacterIndex, out _);
                    }
                    else if (ActiveDialog.ActiveBustCharacterState == Dialog.ActiveBustCharacterStates.Right && ActiveDialog.RightCharacter != null)
                    {
                        scriptVisualNovel.ActiveVisualNovel.GetBustPortraitIndices(ActiveDialog.RightCharacter, out CharacterIndex, out _);
                    }
                    else if (ActiveDialog.TopPortaitVisibleState == Dialog.PortaitVisibleStates.Visible && ActiveDialog.TopCharacter != null)
                    {
                        scriptVisualNovel.ActiveVisualNovel.GetBoxPortraitIndices(ActiveDialog.TopCharacter, out CharacterIndex, out _);
                    }
                    else if (ActiveDialog.BottomPortaitVisibleState == Dialog.PortaitVisibleStates.Visible && ActiveDialog.BottomCharacter != null)
                    {
                        scriptVisualNovel.ActiveVisualNovel.GetBoxPortraitIndices(ActiveDialog.BottomCharacter, out CharacterIndex, out _);
                    }

                    if (CharacterIndex > 0)
                    {
                        ListSpeakerPriority = scriptVisualNovel.ActiveVisualNovel.ListCharacter[CharacterIndex - 1].ListSpeakerPriority;
                    }
                }

                return ListSpeakerPriority;
            }

            private void CenterCameraOnSpeaker()
            {
                List<SpeakerPriority> ListSpeakerPriority = GetSpeakerPriorities();
                CenterOnSquadCutscene CenterCamera = new CenterOnSquadCutscene(null, Map, new Microsoft.Xna.Framework.Vector3());
                foreach (SpeakerPriority ActiveSpeaker in ListSpeakerPriority)
                {
                    if (ActiveSpeaker.PriorityType == SpeakerPriority.PriorityTypes.Character)
                    {
                        foreach (Player ActivePlayer in Map.ListPlayer)
                        {
                            foreach (Squad ActiveSquad in ActivePlayer.ListSquad)
                            {
                                for (int U = 0; U < ActiveSquad.UnitsInSquad; ++U)
                                {
                                    foreach (Character ActiveCharacter in ActiveSquad.At(U).ArrayCharacterActive)
                                    {
                                        if (ActiveCharacter.FullName == ActiveSpeaker.Name)
                                        {
                                            Map.PushScreen(new CenterOnSquadCutscene(null, Map, ActiveSquad.Position));
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (ActiveSpeaker.PriorityType == SpeakerPriority.PriorityTypes.Location)
                    {
                        string[] ArrayPosition = ActiveSpeaker.Name.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        float X = float.Parse(ArrayPosition[0]);
                        float Y = float.Parse(ArrayPosition[1]);
                        float Z = 0;
                        if (ArrayPosition.Length > 2)
                        {
                            Z = float.Parse(ArrayPosition[2]);
                        }
                        Map.PushScreen(new CenterOnSquadCutscene(null, Map, new Microsoft.Xna.Framework.Vector3(X, Y, Z)));
                        return;
                    }
                    else if (ActiveSpeaker.PriorityType == SpeakerPriority.PriorityTypes.ID)
                    {
                        foreach (Player ActivePlayer in Map.ListPlayer)
                        {
                            foreach (Squad ActiveSquad in ActivePlayer.ListSquad)
                            {
                                uint RealID = uint.Parse(ActiveSpeaker.Name);
                                if (ActiveSquad.ID == RealID)
                                {
                                    Map.PushScreen(new CenterOnSquadCutscene(null, Map, ActiveSquad.Position));
                                    return;
                                }
                            }
                        }
                    }
                }
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
                return new ScriptStartDeathmatchVisualNovel(Map);
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
}
