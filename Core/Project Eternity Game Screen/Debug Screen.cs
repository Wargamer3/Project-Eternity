using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens
{
    public class DebugScreen : GameScreen
    {
        private struct DebugEffect
        {
            public BaseEffect Effect;
            public List<string> ListDebugText;

            public DebugEffect(BaseEffect Effect, List<string> ListDebugText)
            {
                this.Effect = Effect;
                this.ListDebugText = ListDebugText;
            }
        }

        private List<DebugEffect> ListDebugEffect;
        private List<string> ListDebugText;
        private int MinIndex;
        private int LineIndex;
        private bool ShowContext;

        public DebugScreen()
            : base()
        {
            ListDebugText = new List<string>();
            ListDebugEffect = new List<DebugEffect>();
            ShowContext = false;
        }

        public override void Load()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                --LineIndex;
                if (LineIndex < 0)
                    LineIndex = MinIndex;
            }
            else if (InputHelper.InputDownPressed())
            {
                ++LineIndex;
                if (LineIndex > MinIndex)
                    LineIndex = 0;
            }
            else if (InputHelper.InputConfirmPressed() || InputHelper.InputCancelPressed())
            {
                ListGameScreen.Remove(this);
            }
        }

        public void Init()
        {
            ListDebugText = new List<string>();

            for (int i = 0; i < ListDebugEffect.Count; ++i)
            {
                string ActivationInfo = ListDebugEffect[i].Effect.ActivationInfo;

                if (!string.IsNullOrWhiteSpace(ActivationInfo))
                {
                    ListDebugText.Add("The following effect was activated: " + ListDebugEffect[i].Effect.EffectTypeName);
                    ListDebugText.Add(ActivationInfo);
                }
                else
                {
                    ListDebugText.Add("The following effect was activated: " + ListDebugEffect[i].Effect.EffectTypeName);
                }

                if (ShowContext)
                {
                    ListDebugText.AddRange(ListDebugEffect[i].ListDebugText);
                }
            }

            MinIndex = LineIndex = Math.Max(0, ListDebugText.Count - 22);
        }

        public void AddDebugEffect(BaseEffect NewDebugEffect, List<string> ListExtraText)
        {
            ListDebugEffect.Add(new DebugEffect(NewDebugEffect, ListExtraText));
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawBox(g, new Vector2(10, 10), Constants.Width - 20, Constants.Height - 20, Color.White);

            g.Draw(sprPixel, new Rectangle(Constants.Width - 25, 20, 5, 5), Color.Black);
            g.Draw(sprPixel, new Rectangle(Constants.Width - 25, Constants.Height - 25, 5, 5), Color.Black);

            int ScrollbarPercent = (int)(LineIndex / (float)MinIndex * (Constants.Height - 70));
            g.Draw(sprPixel, new Rectangle(Constants.Width - 25, 28 + ScrollbarPercent, 5, 15), Color.Black);

            int StartY = 15;

            for (int i = LineIndex; i < LineIndex + 22 && i < ListDebugText.Count; ++i)
            {
                TextHelper.DrawText(g, ListDebugText[i], new Vector2(15, StartY + (i - LineIndex) * 20), Color.White);
            }
        }
    }
}
