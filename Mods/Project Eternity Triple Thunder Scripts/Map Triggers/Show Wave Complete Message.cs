using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class ShowWaveCompleteMessageTrigger : FightingZoneTrigger
    {
        public class StageCompleteTextOverlay : ScrollingTextOverlayBase
        {
            private enum AnimationStates { Visible, FadeOut }

            private readonly FightingZone Map;
            private readonly MapScript Owner;
            private AnimationStates AnimationState;
            private const double VisibleTimeInSeconds = 3d;
            private const double FadeOutSeconds = 2d;
            private double ElapsedTime;

            private Texture2D sprWaveComplete;
            private SpriteFont fntWaveNumber;
            private int WaveNumber;
            public string NextLevelPath { get; set; }

            public bool IsFinished { get; private set; }

            public StageCompleteTextOverlay(Texture2D sprWaveComplete, SpriteFont fntWaveNumber, int WaveNumber, FightingZone Map, MapScript Owner)
            {
                this.sprWaveComplete = sprWaveComplete;
                this.fntWaveNumber = fntWaveNumber;
                this.WaveNumber = WaveNumber;
                this.Map = Map;
                this.Owner = Owner;
                AnimationState = AnimationStates.Visible;
            }

            public void Update(GameTime gameTime)
            {
                ElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

                switch (AnimationState)
                {
                    case AnimationStates.Visible:
                        if (ElapsedTime >= VisibleTimeInSeconds)
                        {
                            ElapsedTime -= VisibleTimeInSeconds;
                            AnimationState = AnimationStates.FadeOut;
                            Map.ExecuteFollowingScripts(Owner, 0);
                        }
                        break;

                    case AnimationStates.FadeOut:
                        if (ElapsedTime >= FadeOutSeconds)
                        {
                            ElapsedTime -= FadeOutSeconds;
                            AnimationState = AnimationStates.Visible;
                            IsFinished = true;
                        }
                        break;
                }
            }

            public void Draw(CustomSpriteBatch g)
            {
                switch (AnimationState)
                {
                    case AnimationStates.Visible:
                        g.Draw(sprWaveComplete, new Vector2(Constants.Width / 2 - 40, Constants.Height / 2), null, Color.White, 0f, new Vector2(sprWaveComplete.Width / 2, sprWaveComplete.Height / 2), 1f, SpriteEffects.None, 0f);
                        g.DrawString(fntWaveNumber, WaveNumber.ToString(), new Vector2(Constants.Width / 2 + 40, Constants.Height / 2 - 17), Color.White);
                        break;
                }
            }
        }

        private Texture2D sprWaveComplete;
        private SpriteFont fntWaveNumber;
        private int _WaveNumber;

        public ShowWaveCompleteMessageTrigger()
            : this(null)
        {
        }

        public ShowWaveCompleteMessageTrigger(FightingZone Map)
            : base(Map, 150, 70, "Show Wave Complete Message", new string[] { "Show Message" }, new string[] { "Message ended" })
        {
        }

        public override void Load(BinaryReader BR)
        {
            if (Map.Content != null)
            {
                sprWaveComplete = Map.Content.Load<Texture2D>("Triple Thunder/HUD/Text Stage");
                fntWaveNumber = Map.Content.Load<SpriteFont>("Triple Thunder/HUD/Text Stage Number");
            }

            _WaveNumber = BR.ReadInt32();
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(_WaveNumber);
        }

        public override void Update(int Index)
        {
            Map.SetScrollingTextOverlay(new StageCompleteTextOverlay(sprWaveComplete, fntWaveNumber, _WaveNumber, Map, this));
        }

        public override MapScript CopyScript()
        {
            ShowWaveCompleteMessageTrigger NewEffect = new ShowWaveCompleteMessageTrigger(Map);

            return NewEffect;
        }

        #region Properties


        [CategoryAttribute("Trigger values"),
        DescriptionAttribute("."),
        DefaultValueAttribute(1)]
        public int WaveNumber
        {
            get
            {
                return _WaveNumber;
            }
            set
            {
                _WaveNumber = value;
            }
        }

        #endregion
    }
}
