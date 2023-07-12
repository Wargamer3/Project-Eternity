using System;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public partial class AnimationClass : GameScreen
    {
        public class AnimationLayerHolder
        {
            private List<AnimationLayer> ListAnimationLayer;
            private GameEngineLayer GameEngineLayer;

            public AnimationLayerHolder()
            {
                ListAnimationLayer = new List<AnimationLayer>();
            }

            public void Add(AnimationLayer NewAnimationLayer)
            {
                ListAnimationLayer.Add(NewAnimationLayer);
            }

            public void Insert(int Index, AnimationLayer NewAnimationLayer)
            {
                ListAnimationLayer.Insert(Index, NewAnimationLayer);
            }

            public void Remove(AnimationLayer AnimationLayerToRemove)
            {
                ListAnimationLayer.Remove(AnimationLayerToRemove);
            }

            public void RemoveAt(int Index)
            {
                ListAnimationLayer.RemoveAt(Index);
            }

            public AnimationLayerHolder Copy()
            {
                AnimationLayerHolder NewAnimationLayerHolder = new AnimationLayerHolder();
                NewAnimationLayerHolder.ListAnimationLayer = new List<AnimationLayer>(ListAnimationLayer.Count);

                foreach (AnimationLayer ActiveLayer in ListAnimationLayer)
                {
                    NewAnimationLayerHolder.ListAnimationLayer.Add(ActiveLayer.Copy());
                }

                NewAnimationLayerHolder.GameEngineLayer = (GameEngineLayer)GameEngineLayer.Copy();

                return NewAnimationLayerHolder;
            }

            public AnimationLayer this[int i]
            {
                get
                {
                    if (i < ListAnimationLayer.Count)
                    {
                        return ListAnimationLayer[i];
                    }
                    else
                    {
                        switch (i - ListAnimationLayer.Count)
                        {
                            case 0:
                                return GameEngineLayer;

                            default:
                                return null;
                        }
                    }
                }
                set
                {
                    ListAnimationLayer[i] = value;
                }
            }

            public int Count
            {
                get
                {
                    return ListAnimationLayer.Count + 1;
                }
            }

            public int BasicLayerCount
            {
                get
                {
                    return ListAnimationLayer.Count;
                }
            }

            public GameEngineLayer EngineLayer
            {
                get
                {
                    return GameEngineLayer;
                }
                set
                {
                    GameEngineLayer = value;
                }
            }
        }

        public class AnimationLayer
        {
            public enum LayerSamplerStates { AnisotropicClamp, AnisotropicWrap, LinearClamp, LinearWrap, PointClamp, PointWrap }
            public enum LayerBlendStates { Add, Substract, Merge };

            private string _Name;

            public List<AnimationLayer> ListChildren;
            public Dictionary<int, List<Timeline>> DicTimelineEvent;//Spawn Frame, Events to spawn.
            public List<VisibleTimeline> ListVisibleObject;//List of active Sprites.
            public List<MarkerTimeline> ListActiveMarker;
            public List<PolygonCutterTimeline> ListPolygonCutter;
            public RenderTarget2D renderTarget;//Buffer used to render the AnimationLayer.
            public Dictionary<uint, GroupTimeline> DicGroupEvent;//List of groups used for the editor.

            private LayerBlendStates _LayerBlendState;
            private LayerSamplerStates _LayerSamplerStates;
            public SamplerState SamplerState;
            private byte _Alpha;
            public bool ShowChildren;
            public bool IsSelected;
            public bool IsVisible;
            public bool IsLocked;
            internal AnimationClass Owner;

            protected AnimationLayer()
            {
            }

            public AnimationLayer(AnimationClass Owner, string Name)
            {
                this.Name = Name;
                this.Owner = Owner;
                ListChildren = new List<AnimationLayer>();
                DicTimelineEvent = new Dictionary<int, List<Timeline>>();
                ListVisibleObject = new List<VisibleTimeline>();
                ListActiveMarker = new List<MarkerTimeline>();
                ListPolygonCutter = new List<PolygonCutterTimeline>();
                DicGroupEvent = new Dictionary<uint, GroupTimeline>();
                _LayerBlendState = LayerBlendStates.Add;
                _LayerSamplerStates = LayerSamplerStates.LinearClamp;
                SamplerState = SamplerState.LinearClamp;
                ShowChildren = true;
                IsVisible = true;
                IsLocked = false;
                IsSelected = false;
            }

            public virtual void LoadLayer(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content, Dictionary<string, Timeline> DicTimeline)
            {
                Name = BR.ReadString();
                LayerBlendState = (LayerBlendStates)BR.ReadByte();
                _LayerSamplerStates = (LayerSamplerStates)BR.ReadByte();
                UpdateSamplerState();

                int DicGroupEventCount = BR.ReadInt32();
                DicGroupEvent = new Dictionary<uint, GroupTimeline>(DicGroupEventCount);
                for (int T = 0; T < DicGroupEventCount; T++)
                {
                    uint Key = BR.ReadUInt32();
                    GroupTimeline NewGroup = new GroupTimeline(BR.ReadString());
                    int GroupIndex = BR.ReadInt32();
                    NewGroup.GroupIndex = GroupIndex;
                    NewGroup.KeyValue = Key;
                    DicGroupEvent.Add(Key, NewGroup);
                }

                int DicTimelineEventCount = BR.ReadInt32();
                DicTimelineEvent = new Dictionary<int, List<Timeline>>(DicTimelineEventCount);
                for (int T = 0; T < DicTimelineEventCount; T++)
                {
                    int ListEventCount = BR.ReadInt32();

                    for (int E = 0; E < ListEventCount; E++)
                    {
                        Timeline ActiveEvent = Timeline.Load(BR, Content, this, DicTimeline);

                        if (!DicTimelineEvent.ContainsKey(ActiveEvent.SpawnFrame))
                            DicTimelineEvent.Add(ActiveEvent.SpawnFrame, new List<Timeline>());

                        DicTimelineEvent[ActiveEvent.SpawnFrame].Add(ActiveEvent);
                    }
                }
                int ChildrenCount = BR.ReadInt32();
                for (int L = 0; L < ChildrenCount; L++)
                {
                    AnimationLayer NewChildrenLayer = new AnimationLayer(Owner, "");
                    NewChildrenLayer.LoadLayer(BR, Content, DicTimeline);
                    ListChildren.Add(NewChildrenLayer);
                }
            }

            public virtual void SaveLayer(BinaryWriter BW)
            {
                BW.Write(Name);
                BW.Write((byte)LayerBlendState);
                BW.Write((byte)_LayerSamplerStates);

                //Save Groups.
                BW.Write(DicGroupEvent.Count);
                foreach (KeyValuePair<uint, GroupTimeline> ActiveEvent in DicGroupEvent)
                {
                    BW.Write(ActiveEvent.Key);
                    BW.Write(ActiveEvent.Value.Name);
                    BW.Write(ActiveEvent.Value.GroupIndex);
                }
                //Save Events.
                BW.Write(DicTimelineEvent.Count);
                foreach (KeyValuePair<int, List<Timeline>> ActiveEvent in DicTimelineEvent)
                {
                    BW.Write(ActiveEvent.Value.Count);
                    for (int E = 0; E < ActiveEvent.Value.Count; E++)
                    {
                        ActiveEvent.Value[E].Save(BW);
                    }
                }

                BW.Write(ListChildren.Count);
                for (int L = 0; L < ListChildren.Count; L++)
                    ListChildren[L].SaveLayer(BW);
            }

            public void AddTimelineEvent(int KeyFrame, Timeline TimelineEventToAdd)
            {
                if (!DicTimelineEvent.ContainsKey(KeyFrame))
                    DicTimelineEvent.Add(KeyFrame, new List<Timeline>());

                DicTimelineEvent[KeyFrame].Add(TimelineEventToAdd);
            }

            public bool RemoveTimelineEvent(int KeyFrame, Timeline TimelineEventToRemove)
            {
                bool RemoveValue = DicTimelineEvent[KeyFrame].Remove(TimelineEventToRemove);
                //If there is no Spawner in the current Key Frame, remove it from the Dictionary.
                if (DicTimelineEvent[KeyFrame].Count == 0)
                    DicTimelineEvent.Remove(KeyFrame);

                return RemoveValue;
            }

            public void GetEndFrame(ref int EndFrame)
            {
                foreach (KeyValuePair<int, List<Timeline>> ActiveEvent in DicTimelineEvent)
                {
                    for (int E = 0; E < ActiveEvent.Value.Count; E++)
                    {
                        if (ActiveEvent.Value[E].DeathFrame > EndFrame)
                            EndFrame = ActiveEvent.Value[E].DeathFrame;
                    }
                }

                for (int L = 0; L < ListChildren.Count; L++)
                    ListChildren[L].GetEndFrame(ref EndFrame);
            }

            public void ResetAnimationLayer()
            {
                ListVisibleObject.Clear();
                ListActiveMarker.Clear();
                ListPolygonCutter.Clear();

                //Reset Spawners.
                foreach (KeyValuePair<int, List<Timeline>> Event in DicTimelineEvent)
                {
                    for (int S = Event.Value.Count - 1; S >= 0; --S)
                    {
                        Event.Value[S].ResetAnimationLayer();
                    }
                }

                for (int C = 0; C < ListChildren.Count; C++)
                {
                    ListChildren[C].ResetAnimationLayer();
                }
            }

            public virtual AnimationLayer Copy()
            {
                AnimationLayer NewAnimationLayer = new AnimationLayer();

                NewAnimationLayer.UpdateFrom(this);

                return NewAnimationLayer;
            }

            protected void UpdateFrom(AnimationLayer Other)
            {
                _Name = Other._Name;
                Owner = Other.Owner;

                ListChildren = new List<AnimationLayer>(Other.ListChildren.Count);
                foreach (AnimationLayer ActiveLayer in Other.ListChildren)
                {
                    ListChildren.Add(ActiveLayer.Copy());
                }

                DicTimelineEvent = new Dictionary<int, List<Timeline>>(Other.DicTimelineEvent.Count);
                foreach (KeyValuePair<int, List<Timeline>> ActiveListTimelineEvent in Other.DicTimelineEvent)
                {
                    List<Timeline> ListTimelineEvent = new List<Timeline>(ActiveListTimelineEvent.Value.Count);

                    foreach (Timeline ActiveTimelineEvent in ActiveListTimelineEvent.Value)
                    {
                        ListTimelineEvent.Add(ActiveTimelineEvent.Copy(this));
                    }

                    DicTimelineEvent.Add(ActiveListTimelineEvent.Key, ListTimelineEvent);
                }
                DicGroupEvent = new Dictionary<uint, GroupTimeline>(Other.DicGroupEvent.Count);
                foreach (KeyValuePair<uint, GroupTimeline> ActiveGroupEvent in Other.DicGroupEvent)
                {
                    DicGroupEvent.Add(ActiveGroupEvent.Key, (GroupTimeline)ActiveGroupEvent.Value.Copy(this));
                }

                ListVisibleObject = new List<VisibleTimeline>();
                ListActiveMarker = new List<MarkerTimeline>();
                ListPolygonCutter = new List<PolygonCutterTimeline>();
                renderTarget = Other.renderTarget;

                _LayerBlendState = Other._LayerBlendState;
                _Alpha = Other._Alpha;
                ShowChildren = Other.ShowChildren;
                IsSelected = Other.IsSelected;
                IsVisible = Other.IsVisible;
                IsLocked = Other.IsLocked;
            }

            public virtual void UpdateKeyFrame(int KeyFrame)
            {
                List<Timeline> ListTimelineEvent;

                if (DicTimelineEvent.TryGetValue(KeyFrame, out ListTimelineEvent))
                {
                    for (int S = 0; S < ListTimelineEvent.Count; S++)
                    {
                        if (!ListTimelineEvent[S].IsUsed)
                        {
                            ListTimelineEvent[S].IsUsed = true;
                            ListTimelineEvent[S].SpawnItem(Owner, this, KeyFrame);
                        }
                    }
                }

                //Update Visible items.
                for (int A = 0; A < ListVisibleObject.Count; A++)
                {
                    if (KeyFrame >= ListVisibleObject[A].DeathFrame)
                    {
                        ListVisibleObject[A].OnDeathFrame(Owner);
                        ListVisibleObject.RemoveAt(A--);
                        continue;
                    }
                    ListVisibleObject[A].UpdateAnimationObject(KeyFrame);
                }
                //Update Markers.
                for (int M = 0; M < ListActiveMarker.Count; M++)
                {
                    if (KeyFrame >= ListActiveMarker[M].DeathFrame)
                    {
                        ListActiveMarker[M].OnDeathFrame(Owner);
                        ListActiveMarker.RemoveAt(M--);
                        continue;
                    }
                    ListActiveMarker[M].UpdateAnimationObject(KeyFrame);
                }
                //Update Polygon Cutters.
                for (int P = 0; P < ListPolygonCutter.Count; P++)
                {
                    if (KeyFrame >= ListPolygonCutter[P].DeathFrame)
                    {
                        ListPolygonCutter[P].OnDeathFrame(Owner);
                        ListPolygonCutter.RemoveAt(P--);
                        continue;
                    }
                    ListPolygonCutter[P].UpdateAnimationObject(KeyFrame);
                }

                for (int L = 0; L < ListChildren.Count; L++)
                    ListChildren[L].UpdateKeyFrame(KeyFrame);
            }

            private void UpdateSamplerState()
            {
                switch (_LayerSamplerStates)
                {
                    case LayerSamplerStates.AnisotropicClamp:
                        SamplerState = SamplerState.AnisotropicClamp;
                        break;
                    case LayerSamplerStates.AnisotropicWrap:
                        SamplerState = SamplerState.AnisotropicWrap;
                        break;
                    case LayerSamplerStates.LinearClamp:
                        SamplerState = SamplerState.LinearClamp;
                        break;
                    case LayerSamplerStates.LinearWrap:
                        SamplerState = SamplerState.LinearWrap;
                        break;
                    case LayerSamplerStates.PointClamp:
                        SamplerState = SamplerState.PointClamp;
                        break;
                    case LayerSamplerStates.PointWrap:
                        SamplerState = SamplerState.PointWrap;
                        break;
                }
            }

            [CategoryAttribute("Animation Layer Attributes"),
            DescriptionAttribute(".")]
            public LayerBlendStates LayerBlendState
            {
                get
                {
                    return _LayerBlendState;
                }
                set
                {
                    _LayerBlendState = value;
                }
            }

            [CategoryAttribute("Animation Layer Attributes"),
            DescriptionAttribute(".")]
            public LayerSamplerStates LayerSamplerState
            {
                get
                {
                    return _LayerSamplerStates;
                }
                set
                {
                    _LayerSamplerStates = value;
                    UpdateSamplerState();
                }
            }

            [CategoryAttribute("Animation Layer Attributes"),
            DescriptionAttribute(".")]
            public byte Alpha
            {
                get
                {
                    return _Alpha;
                }
                set
                {
                    _Alpha = value;
                }
            }

            [CategoryAttribute("Animation Layer Attributes"),
            DescriptionAttribute(".")]
            public string Name
            {
                get
                {
                    return _Name;
                }
                set
                {
                    _Name = value;
                }
            }
        }

        public class GameEngineLayer : AnimationLayer
        {
            public AnimationOriginTimeline AnimationOrigin;

            private GameEngineLayer()
                : this(null)
            {
                AnimationOrigin = new AnimationOriginTimeline();
                AnimationOrigin.DicAnimationKeyFrame.Add(0, new VisibleAnimationObjectKeyFrame(new Vector2(275, 320), true, -1));

                List<Timeline> ListGameEngineTimeline = new List<Timeline>();
                ListGameEngineTimeline.Add(new BackgroundTimeline());
                ListGameEngineTimeline.Add(new QuoteSetTimeline());
                ListGameEngineTimeline.Add(new SFXTimeline());
                ListGameEngineTimeline.Add(AnimationOrigin);
                DicTimelineEvent.Add(0, ListGameEngineTimeline);
            }

            public GameEngineLayer(AnimationClass Owner)
                : base(Owner, "Game Engine")
            {
            }

            public static GameEngineLayer EmptyGameEngineLayer(AnimationClass Owner)
            {
                GameEngineLayer NewGameEngineLayer = new GameEngineLayer();
                NewGameEngineLayer.Owner = Owner;
                return NewGameEngineLayer;
            }

            public override void LoadLayer(BinaryReader BR, Microsoft.Xna.Framework.Content.ContentManager Content, Dictionary<string, Timeline> DicTimeline)
            {
                DicTimelineEvent = new Dictionary<int, List<Timeline>>(5);
                DicTimelineEvent.Add(0, new List<Timeline>());

                int ListEventCount = BR.ReadInt32();
                for (int E = 0; E < ListEventCount; E++)
                {
                    Timeline ActiveEvent = Timeline.Load(BR, Content, this, DicTimeline);

                    DicTimelineEvent[0].Add(ActiveEvent);
                }

                AnimationOrigin = (AnimationOriginTimeline)DicTimelineEvent[0][3];
            }

            public override void SaveLayer(BinaryWriter BW)
            {
                BW.Write(DicTimelineEvent[0].Count);
                foreach (Timeline ActiveEvent in DicTimelineEvent[0])
                {
                    ActiveEvent.Save(BW);
                }
            }

            public override AnimationLayer Copy()
            {
                GameEngineLayer NewGameEngineLayer = new GameEngineLayer();

                NewGameEngineLayer.UpdateFrom(this);

                return NewGameEngineLayer;
            }

            public override void UpdateKeyFrame(int KeyFrame)
            {
                List<Timeline> ListTimelineEvent = DicTimelineEvent[0];
                for (int S = 0; S < ListTimelineEvent.Count; S++)
                {
                    AnimationObjectKeyFrame ActiveKeyFrame;
                    if (ListTimelineEvent[S].TryGetValue(KeyFrame, out ActiveKeyFrame))
                    {
                        ListTimelineEvent[S].SpawnItem(Owner, this, KeyFrame);
                    }
                }
                //Update Bitmap.
                for (int A = 0; A < ListVisibleObject.Count; A++)
                {
                    if (KeyFrame >= ListVisibleObject[A].DeathFrame)
                    {
                        ListVisibleObject[A].OnDeathFrame(Owner);
                        ListVisibleObject.RemoveAt(A--);
                        continue;
                    }
                    ListVisibleObject[A].UpdateAnimationObject(KeyFrame);
                }
            }
        }

        #region Ressources

        protected Texture2D sprTopLeftAttack;
        protected Texture2D sprTopLeftCounter;
        protected Texture2D sprTopLeftDefense;
        protected Texture2D sprTopLeftEvasion;

        #endregion

        public Dictionary<string, Timeline> DicTimeline;

        public int ActiveKeyFrame;

        public AnimationLayerHolder ListAnimationLayer;

        public List<SFX> ListActiveSFX;

        public string ActiveCharacterName;
        public List<string> ActiveQuoteSet;
        public SimpleAnimation ActiveCharacterSprite;

        public const int VNBoxHeight = 128;

        public string CurrentQuote;
        public string AnimationPath;
        public int ScreenWidth;
        public int ScreenHeight;
        public int LoopStart;
        public int LoopEnd;
        protected BasicEffect PolygonEffect;
        protected AlphaTestEffect AlphaEffect;
        public AnimationBackground ActiveAnimationBackground;
        public AnimationBackground ActiveAnimationForeground;
        public Matrix TransformationMatrix2D;
        public Matrix TransformationMatrix3D;

        public static readonly BlendState NegativeBlendState = new BlendState()
        {
            ColorSourceBlend = Blend.Zero,
            ColorDestinationBlend = Blend.InverseSourceAlpha,

            AlphaSourceBlend = Blend.Zero,
            AlphaDestinationBlend = Blend.InverseSourceAlpha
        };
        public static DepthStencilState AlwaysStencilState = new DepthStencilState()
        {

            StencilEnable = true,
            StencilFunction = CompareFunction.Always,
            StencilPass = StencilOperation.Replace,
            ReferenceStencil = 1,
            DepthBufferEnable = false,
        };

        public static DepthStencilState EqualStencilState = new DepthStencilState()
        {
            StencilEnable = true,
            StencilFunction = CompareFunction.Equal,
            StencilPass = StencilOperation.Keep,
            ReferenceStencil = 1,
            DepthBufferEnable = false,
        };

        public AnimationOriginTimeline AnimationOrigin { get { return ListAnimationLayer.EngineLayer.AnimationOrigin; } }

        protected AnimationClass()
            : base()
        {
            ListActiveSFX = new List<SFX>();
            TransformationMatrix2D = Matrix.Identity;
            TransformationMatrix3D = Matrix.Identity;
            DicTimeline = new Dictionary<string, Timeline>();
        }

        public AnimationClass(string AnimationPath)
            : this()
        {
            this.AnimationPath = AnimationPath;
        }

        public override void Load()
        {
            InitMembers();

            LoadFromFile();
        }

        protected void InitMembers()
        {
            if (GraphicsDevice != null)
            {
                PolygonEffect = new BasicEffect(GraphicsDevice);

                PolygonEffect.VertexColorEnabled = true;
                PolygonEffect.TextureEnabled = true;
                PolygonEffect.View = Matrix.Identity;

                PolygonEffect.World = Matrix.Identity;
                Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 0, 1);
                Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                PolygonEffect.Projection = HalfPixelOffset * Projection;

                AlphaEffect = new AlphaTestEffect(GraphicsDevice);
                AlphaEffect.AlphaFunction = CompareFunction.NotEqual;
                AlphaEffect.VertexColorEnabled = true;
                AlphaEffect.ReferenceAlpha = 255;
                AlphaEffect.View = Matrix.Identity;
                AlphaEffect.World = Matrix.Identity;
                AlphaEffect.Projection = HalfPixelOffset * Projection;
            }

            if (!DicTimeline.ContainsKey("Marker"))
            {
                DicTimeline.Add("Marker", new MarkerTimeline());
                DicTimeline.Add("SFX", new SFXTimeline());
                DicTimeline.Add("Background", new BackgroundTimeline());
                DicTimeline.Add("Quote", new QuoteSetTimeline());
                DicTimeline.Add("Origin", new AnimationOriginTimeline());
                DicTimeline.Add("Polygon", new PolygonCutterTimeline());
            }
        }

        public void LoadFromFile()
        {
            FileStream FS = new FileStream("Content/Animations/" + AnimationPath + ".pea", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);
            
            LoopStart = BR.ReadInt32();
            LoopEnd = BR.ReadInt32();

            ScreenWidth = BR.ReadInt32();
            ScreenHeight = BR.ReadInt32();

            ListAnimationLayer = new AnimationLayerHolder();
            int ListAnimationLayerCount = BR.ReadInt32();
            for (int L = 0; L < ListAnimationLayerCount; L++)
            {
                AnimationLayer NewAnimationLayer = new AnimationLayer(this, "");
                NewAnimationLayer.LoadLayer(BR, Content, DicTimeline);
                ListAnimationLayer.Add(NewAnimationLayer);
            }

            GameEngineLayer NewGameEngineLayer = new GameEngineLayer(this);
            NewGameEngineLayer.LoadLayer(BR, Content, DicTimeline);
            ListAnimationLayer.EngineLayer = NewGameEngineLayer;

            FS.Close();
            BR.Close();
        }

        protected static Dictionary<string, Timeline> LoadTimelines(Type TypeOfTimeline, params object[] Args)
        {
            Dictionary<string, Timeline> DicTimelineByType = new Dictionary<string, Timeline>();

            string[] Files = Directory.GetFiles("Timelines", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<Timeline> ListTimeline = ReflectionHelper.GetObjectsFromTypes<Timeline>(TypeOfTimeline, ActiveAssembly.GetTypes(), Args);

                foreach (Timeline Instance in ListTimeline)
                {
                    DicTimelineByType.Add(Instance.TimelineEventType, Instance);
                }
            }

            return DicTimelineByType;
        }

        protected static Dictionary<string, Timeline> LoadTimelines(string RootPath, params object[] Args)
        {
            Dictionary<string, Timeline> DicTimelineByType = new Dictionary<string, Timeline>();

            string[] Files = Directory.GetFiles("Timelines/" + RootPath, "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<Timeline> ListTimeline = ReflectionHelper.GetObjectsFromBaseTypes<Timeline>(typeof(Timeline), ActiveAssembly.GetTypes(), Args);

                foreach (Timeline Instance in ListTimeline)
                {
                    DicTimelineByType.Add(Instance.TimelineEventType, Instance);
                }
            }

            return DicTimelineByType;
        }

        public static Dictionary<string, Timeline> LoadAllTimelines()
        {
            Dictionary<string, Timeline> DicTimelineByType = new Dictionary<string, Timeline>();

            string[] Files = Directory.GetFiles("Timelines", "*.dll", SearchOption.AllDirectories);
            for (int F = 0; F < Files.Length; F++)
            {
                Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<Timeline> ListTimeline = ReflectionHelper.GetObjectsFromBaseTypes<Timeline>(typeof(Timeline), ActiveAssembly.GetTypes());

                foreach (Timeline Instance in ListTimeline)
                {
                    DicTimelineByType.Add(Instance.TimelineEventType, Instance);
                }
            }

            return DicTimelineByType;
        }

        public virtual void Init()
        {
        }

        protected void InitRessources()
        {
            sprTopLeftAttack = Content.Load<Texture2D>("Animations/Ressources/Left Attack");
            sprTopLeftCounter = Content.Load<Texture2D>("Animations/Ressources/Left Counter");
            sprTopLeftDefense = Content.Load<Texture2D>("Animations/Ressources/Left Defense");
            sprTopLeftEvasion = Content.Load<Texture2D>("Animations/Ressources/Left Evasion");
        }

        public virtual AnimationClass Copy()
        {
            AnimationClass NewAnimationClass = new AnimationClass();

            NewAnimationClass.UpdateFrom(this);

            return NewAnimationClass;
        }

        protected void UpdateFrom(AnimationClass Other)
        {
            sprTopLeftAttack = Other.sprTopLeftAttack;
            sprTopLeftCounter = Other.sprTopLeftCounter;
            sprTopLeftDefense = Other.sprTopLeftDefense;
            sprTopLeftEvasion = Other.sprTopLeftEvasion;

            ListAnimationLayer = Other.ListAnimationLayer.Copy();
            AnimationPath = Other.AnimationPath;
            ScreenWidth = Other.ScreenWidth;
            ScreenHeight = Other.ScreenHeight;
            LoopStart = Other.LoopStart;
            LoopEnd = Other.LoopEnd;
            PolygonEffect = Other.PolygonEffect;
        }

        public void UpdateKeyFrame(int KeyFrame)
        {
            //Update engine first to update the Animation Origin first.
            ListAnimationLayer.EngineLayer.UpdateKeyFrame(KeyFrame);

            for (int L = 0; L < ListAnimationLayer.BasicLayerCount; L++)
                ListAnimationLayer[L].UpdateKeyFrame(KeyFrame);

            //Update SFX.
            for (int S = 0; S < ListActiveSFX.Count; S++)
            {
                if (ListActiveSFX[S].Length >= 0)
                {
                    if (KeyFrame >= ListActiveSFX[S].DeathFrame)
                    {
                        ListActiveSFX[S].SFXSound.Stop();
                        ListActiveSFX.RemoveAt(S--);
                        continue;
                    }
                }
                else
                {
                    if (!ListActiveSFX[S].SFXSound.IsPlaying())
                    {
                        ListActiveSFX.RemoveAt(S--);
                    }
                }
            }
        }

        #region Interpolation

        private float CosineInterpolation(float Progression, float y1, float y2)
        {
            float ProgressionValue;

            double mu2;

            mu2 = (1 - Math.Cos(Progression * Math.PI)) / 2;
            ProgressionValue = (float)(y1 * (1 - mu2) + y2 * mu2);

            return ProgressionValue;
        }

        private float CubicInterpolation(float Progression, int y0, int y1, int y2, int y3)
        {
            double a0, a1, a2, a3, mu2;

            mu2 = Progression * Progression;
            a0 = y3 - y2 - y0 + y1;
            a1 = y0 - y1 - a0;
            a2 = y2 - y0;
            a3 = y1;

            return (float)(a0 * Progression * mu2 + a1 * mu2 + a2 * Progression + a3);
        }

        private float CatmullRomInterpolation(float Progression, int y0, int y1, int y2, int y3)
        {
            double a0, a1, a2, a3, mu2;

            mu2 = Progression * Progression;
            a0 = -0.5 * y0 + 1.5 * y1 - 1.5 * y2 + 0.5 * y3;
            a1 = y0 - 2.5 * y1 + 2 * y2 - 0.5 * y3;
            a2 = -0.5 * y0 + 0.5 * y2;
            a3 = y1;

            return (float)(a0 * Progression * mu2 + a1 * mu2 + a2 * Progression + a3);
        }

        //Tension: 1 is high, 0 normal, -1 is low
        //Bias: 0 is even,
        //positive is towards first segment,
        //negative towards the other
        private float HermiteInterpolation(float Progression, int y0, int y1, int y2, int y3, double mu, double tension, double bias)
        {
            double m0, m1, mu2, mu3;
            double a0, a1, a2, a3;

            mu2 = mu * mu;
            mu3 = mu2 * mu;
            m0 = (y1 - y0) * (1 + bias) * (1 - tension) / 2;
            m0 += (y2 - y1) * (1 - bias) * (1 - tension) / 2;
            m1 = (y2 - y1) * (1 + bias) * (1 - tension) / 2;
            m1 += (y3 - y2) * (1 - bias) * (1 - tension) / 2;
            a0 = 2 * mu3 - 3 * mu2 + 1;
            a1 = mu3 - 2 * mu2 + mu;
            a2 = mu3 - mu2;
            a3 = -2 * mu3 + 3 * mu2;

            return (float)(a0 * y1 + a1 * m0 + a2 * m1 + a3 * y2);
        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            if (ActiveAnimationBackground != null)
                ActiveAnimationBackground.Update(gameTime);

            if (ActiveAnimationForeground != null)
                ActiveAnimationForeground.Update(gameTime);

            UpdateKeyFrame(ActiveKeyFrame);
            ActiveKeyFrame++;
        }

        //The methods starting with "On" should only be called directly by the Timelines
        public virtual void OnVisibleTimelineSpawn(AnimationLayer ActiveLayer, VisibleTimeline ActiveTimeline)
        {
            ActiveLayer.ListVisibleObject.Add(ActiveTimeline);
        }

        public virtual void OnMarkerTimelineSpawn(AnimationLayer ActiveLayer, MarkerTimeline ActiveMarker)
        {
            ActiveLayer.ListActiveMarker.Add(ActiveMarker);
        }

        public virtual void OnPolygonCutterTimelineSpawn(AnimationLayer ActiveLayer, PolygonCutterTimeline ActivePolygonCutter)
        {
            ActiveLayer.ListPolygonCutter.Add(ActivePolygonCutter);
        }

        public virtual void OnVisibleTimelineDeath(VisibleTimeline RemovedBitmap)
        {
        }

        public virtual void OnMarkerTimelineDeath(MarkerTimeline RemovedMarker)
        {
        }

        public virtual void OnPolygonCutterTimelineDeath(PolygonCutterTimeline RemovedPolygonCutter)
        {
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            for (int L = 0; L < ListAnimationLayer.Count; L++)
            {
                AnimationLayer ActiveLayer = ListAnimationLayer[L];

                for (int A = 0; A < ActiveLayer.ListVisibleObject.Count; A++)
                {
                    ActiveLayer.ListVisibleObject[A].BeginDraw(g);
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            throw new NotImplementedException();
        }

        public void DrawLayer(CustomSpriteBatch g, AnimationLayer ActiveLayer, bool DrawNestedMarkers, bool DrawMarkerRenderTarget, AnimationLayer Parent, bool BeginDraw)
        {
            if (BeginDraw)
            {
                //TODO: Don't use Begin to use the transformationMatrix as it force the rendering making it impossible to properly use the drawing depth.
                if (ActiveLayer.LayerBlendState == AnimationLayer.LayerBlendStates.Add)
                {
                    g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, ActiveLayer.SamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, TransformationMatrix2D);
                }
                else if (ActiveLayer.LayerBlendState == AnimationLayer.LayerBlendStates.Substract)
                {
                    g.Begin(SpriteSortMode.BackToFront, NegativeBlendState, ActiveLayer.SamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, TransformationMatrix2D);
                }
                else
                {
                    GraphicsDevice.Clear(ClearOptions.Stencil, Color.Black, 0, 0);
                    g.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, AlwaysStencilState, null, null);
                    DrawLayer(g, Parent, DrawNestedMarkers, false, null, false);

                    g.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, EqualStencilState, null, null);
                    for (int A = 0; A < ActiveLayer.ListVisibleObject.Count; A++)
                    {
                        ActiveLayer.ListVisibleObject[A].Draw(g, false);
                    }
                }
            }

            for (int A = 0; A < ActiveLayer.ListVisibleObject.Count; A++)
            {
                ActiveLayer.ListVisibleObject[A].Draw(g, false);
                ActiveLayer.ListVisibleObject[A].Draw3D(g, false, TransformationMatrix2D, TransformationMatrix3D);
            }

            if (DrawNestedMarkers)
            {
                for (int M = 0; M < ActiveLayer.ListActiveMarker.Count; M++)
                {
                    AnimationClass ActiveMarkerAnimation = ActiveLayer.ListActiveMarker[M].AnimationMarker;

                    for (int i = 0; i < ActiveMarkerAnimation.ListAnimationLayer.BasicLayerCount; i++)
                    {
                        DrawLayer(g, ActiveMarkerAnimation.ListAnimationLayer[i], DrawNestedMarkers, DrawMarkerRenderTarget, null, false);
                    }
                }
            }

            if (DrawMarkerRenderTarget)
            {
                for (int M = 0; M < ActiveLayer.ListActiveMarker.Count; M++)
                {
                    AnimationClass ActiveMarkerAnimation = ActiveLayer.ListActiveMarker[M].AnimationMarker;
                    AnimationOriginTimeline ActiveMarkerOrigin = ActiveMarkerAnimation.AnimationOrigin;

                    for (int L = 0; L < ActiveMarkerAnimation.ListAnimationLayer.BasicLayerCount; L++)
                    {
                        SpriteEffects ActiveEffect = SpriteEffects.None;
                        if (ActiveLayer.ListActiveMarker[M].ScaleFactor.X < 0)
                            ActiveEffect = SpriteEffects.FlipHorizontally;
                        if (ActiveLayer.ListActiveMarker[M].ScaleFactor.Y < 0)
                            ActiveEffect |= SpriteEffects.FlipVertically;

                        int OriginX = (int)ActiveMarkerOrigin.Position.X;
                        if ((ActiveEffect & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally)
                            OriginX = ScreenWidth - (int)ActiveMarkerOrigin.Position.X;

                        g.Draw(ActiveMarkerAnimation.ListAnimationLayer[ActiveMarkerAnimation.ListAnimationLayer.BasicLayerCount - L - 1].renderTarget,
                            new Vector2(ActiveLayer.ListActiveMarker[M].Position.X, ActiveLayer.ListActiveMarker[M].Position.Y), null, Color.White, 0,
                            new Vector2(OriginX, ActiveMarkerOrigin.Position.Y),
                            new Vector2(Math.Abs(ActiveLayer.ListActiveMarker[M].ScaleFactor.X), Math.Abs(ActiveLayer.ListActiveMarker[M].ScaleFactor.Y)), ActiveEffect, ActiveLayer.ListActiveMarker[M].DrawingDepth);
                    }
                }
            }

            if (BeginDraw)
            {
                g.End();
            }

            for (int L = 0; L < ActiveLayer.ListChildren.Count; L++)
            {
                DrawLayer(g, ActiveLayer.ListChildren[L], DrawNestedMarkers, DrawMarkerRenderTarget, ActiveLayer, BeginDraw);
            }
        }
    }
}
