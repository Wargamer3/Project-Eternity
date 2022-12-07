using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleItemSelectionPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "BattleItemSelection";

        private const float ItemCardScale = 0.2f;
        private const float FaceDownValue = 0;

        private bool ItemSelected;
        private double ItemAnimationTime;

        public ActionPanelBattleItemSelectionPhase(SorcererStreetMap Map)
            : base(PanelName, Map, ItemCard.ItemCardType)
        {
            DrawDrawInfo = false;
        }

        public ActionPanelBattleItemSelectionPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map.ListActionMenuChoice, Map, ActivePlayerIndex, ItemCard.ItemCardType, "End")
        {
            DrawDrawInfo = false;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!ItemSelected)
            {
                base.DoUpdate(gameTime);
            }
            else
            {
                if (ActivePlayerIndex == Map.ActivePlayerIndex)
                {
                    if (ItemAnimationTime < 0.5f)
                    {
                        ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        RemoveFromPanelList(this);
                        AddToPanelListAndSelect(new ActionPanelBattleItemSelectionPhase(Map, Map.GlobalSorcererStreetBattleContext.DefenderPlayerIndex));
                    }
                }
                else
                {
                    if (ItemAnimationTime < 0.5f)
                    {
                        ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else if (ItemAnimationTime < 1)//Reveal both cards
                    {
                        ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        RemoveFromPanelList(this);
                        AddToPanelListAndSelect(new ActionPanelBattleLandModifierPhase(Map));
                    }
                }
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            if (!ItemSelected)
            {
                base.UpdatePassive(gameTime);
            }
            else
            {
                if (ActivePlayerIndex == Map.ActivePlayerIndex)
                {
                    if (ItemAnimationTime < 0.5f)
                    {
                        ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
                else
                {
                    if (ItemAnimationTime < 0.5f)
                    {
                        ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else if (ItemAnimationTime < 1)//Reveal both cards
                    {
                        ItemAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }
        }

        public override void OnCardSelected(Card CardSelected)
        {
            ItemSelected = true;
            if (ActivePlayerIndex == Map.ActivePlayerIndex)
            {
                Map.GlobalSorcererStreetBattleContext.InvaderItem = CardSelected;
            }
            else
            {
                Map.GlobalSorcererStreetBattleContext.DefenderItem = CardSelected;
            }

            if (Map.OnlineClient != null)
            {
                Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
            }
        }

        public override void OnEndCardSelected()
        {
            ItemSelected = true;

            if (Map.OnlineClient != null)
            {
                Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
            }
        }

        public override void DoRead(ByteReader BR)
        {
            base.DoRead(BR);
            ActionPanelBattle.ReadPlayerInfo(BR, Map);
        }

        public override void ExecuteUpdate(byte[] ArrayUpdateData)
        {
            base.ExecuteUpdate(ArrayUpdateData);
            ItemSelected = ArrayUpdateData[2] == 1 ? true : false;
        }

        public override void DoWrite(ByteWriter BW)
        {
            base.DoWrite(BW);
            ActionPanelBattle.WritePlayerInfo(BW, Map);
        }

        public override byte[] DoWriteUpdate()
        {
            return new byte[] { (byte)AnimationPhase, (byte)CardCursorIndex, (byte)(ItemSelected ? 1 : 0) };
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleItemSelectionPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1, 0);
            Map.GlobalSorcererStreetBattleContext.Background.Draw(g, Constants.Width, Constants.Height);

            Map.GlobalSorcererStreetBattleContext.InvaderCard.Draw(g);
            Map.GlobalSorcererStreetBattleContext.DefenderCard.Draw(g);

            if (ItemAnimationTime == 0)
            {
                base.Draw(g);

                if (ActivePlayerIndex != Map.ActivePlayerIndex)
                {
                    float FaceDownValue = 0;
                    Card.DrawCardMiniature(g, null, Map.sprCardBack, Color.White, (float)Constants.Width / 12, (float)Constants.Height / 12, 0.2f, 0.2f, FaceDownValue);
                }

                GameScreen.DrawBox(g, new Vector2(Constants.Width / 6, Constants.Height / 12), Constants.Width - Constants.Width / 3, 30, Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, Map.ListPlayer[ActivePlayerIndex].Name + "'s item selection", new Vector2(Constants.Width - Constants.Width / 2, Constants.Height / 12 + 5), Color.White);

                int Y = Constants.Height / 4 - 25;
                TextHelper.DrawTextMiddleAligned(g, "BATTLE", new Vector2(Constants.Width / 2, Y), Color.White);
                Y = Constants.Height / 4;
                GameScreen.DrawBox(g, new Vector2(Constants.Width / 16, Y), Constants.Width - Constants.Width / 8, Constants.Height / 3, Color.White);
                Y = Constants.Height / 4 + 10;
                g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderPlayer.Name, new Vector2(Constants.Width / 4, Y), Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderPlayer.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, "Invasion / Defense", new Vector2(Constants.Width - Constants.Width / 2, Y), Color.White);

                //Invader
                Y = Constants.Height / 4 + 35;
                g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 7, Y), new Vector2(Constants.Width - Constants.Width / 7, Y), Color.White);
                Y = Constants.Height / 4 + 40;
                g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderCard.Name, new Vector2(Constants.Width / 4, Y), Color.White);
                int X = Constants.Width / 4;
                Y += 30;
                g.Draw(Map.Symbols.sprMenuST, new Rectangle(X - 50, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.MaxST.ToString(), new Vector2(X - 20, Y), Color.White);
                g.Draw(Map.Symbols.sprMenuHP, new Rectangle(X + 10, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.MaxHP.ToString(), new Vector2(X + 45, Y), Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, "Ability Values", new Vector2(Constants.Width / 2, Y), Color.White);

                Y += 25;
                g.DrawString(Map.fntArial12, "+0", new Vector2(X - 20, Y), Color.White);
                g.DrawString(Map.fntArial12, "+0", new Vector2(X + 45, Y), Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, "Support / Land", new Vector2(Constants.Width / 2, Y), Color.White);
                Y += 25;
                g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 7, Y), new Vector2(Constants.Width - Constants.Width / 7, Y), Color.White);
                Y += 10;
                g.Draw(Map.Symbols.sprMenuST, new Rectangle(X - 50, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderFinalST.ToString(), new Vector2(X - 20, Y), Color.White);
                g.Draw(Map.Symbols.sprMenuHP, new Rectangle(X + 10, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.InvaderFinalHP.ToString(), new Vector2(X + 45, Y), Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, "Total", new Vector2(Constants.Width / 2, Y), Color.White);

                //Defender
                X = Constants.Width - Constants.Width / 4;
                Y = Constants.Height / 4 + 40;
                g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderCard.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
                Y += 30;
                g.Draw(Map.Symbols.sprMenuST, new Rectangle(X - 50, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Defender.MaxST.ToString(), new Vector2(X - 20, Y), Color.White);
                g.Draw(Map.Symbols.sprMenuHP, new Rectangle(X + 10, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Defender.MaxHP.ToString(), new Vector2(X + 45, Y), Color.White);

                Y += 25;
                g.DrawString(Map.fntArial12, "+0", new Vector2(X - 20, Y), Color.White);
                g.DrawString(Map.fntArial12, "+0", new Vector2(X + 45, Y), Color.White);
                Y += 25;
                g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 7, Y), new Vector2(Constants.Width - Constants.Width / 7, Y), Color.White);
                Y += 10;
                g.Draw(Map.Symbols.sprMenuST, new Rectangle(X - 50, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderFinalST.ToString(), new Vector2(X - 20, Y), Color.White);
                g.Draw(Map.Symbols.sprMenuHP, new Rectangle(X + 10, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.DefenderFinalHP.ToString(), new Vector2(X + 45, Y), Color.White);
            }
            else
            {
                DrawOutro(g);
                if (ActivePlayerIndex == Map.ActivePlayerIndex)
                {
                    IntroduceInvaderItem(g);
                }
                else
                {
                    if (ItemAnimationTime < 0.5f)
                    {
                        Card.DrawCardMiniature(g, null, Map.sprCardBack, Color.White, (float)Constants.Width / 12, (float)Constants.Height / 12, ItemCardScale, ItemCardScale, FaceDownValue);
                        IntroduceDefenderItem(g);
                    }
                    else
                    {
                        if (Map.GlobalSorcererStreetBattleContext.InvaderItem == null)
                        {
                            DiscardInvaderItem(g);
                        }
                        else
                        {
                            RevealInvaderItem(g);
                        }

                        if (Map.GlobalSorcererStreetBattleContext.DefenderItem == null)
                        {
                            DiscardDefenderItem(g);
                        }
                        else
                        {
                            RevealDefenderItem(g);
                        }
                    }
                }
            }
        }

        private void IntroduceInvaderItem(CustomSpriteBatch g)
        {
            float RealRotationTimer = (float)ItemAnimationTime * 2;
            float FinalX = Constants.Width / 12;
            float StartX = -50;
            float FinalY = Constants.Height / 5.3f;
            float StartY = Constants.Height / 2;
            float TransitionX = Constants.Width / 4;
            float TransitionY = Constants.Height / 4;

            float t = RealRotationTimer;

            int n = 2;
            double ResultX = Math.Pow(1 - t, n) * StartX;
            double ResultY = Math.Pow(1 - t, n) * StartY;

            //n! / ((n-i)!*i!) * ((1 - t)^(n-1)) * (t ^ i) * Point[i]
            ResultX += n * (1 - t) * t * TransitionX;
            ResultY += n * (1 - t) * t * TransitionY;
            ResultX += Math.Pow(t, n) * FinalX;
            ResultY += Math.Pow(t, n) * FinalY;

            float StartScale = ItemCardScale * 1.5f;
            float FinalScale = ItemCardScale;
            float ScaleDifference = FinalScale - StartScale;
            float ScaleX = StartScale + ScaleDifference * RealRotationTimer;
            float ScaleY = StartScale + ScaleDifference * RealRotationTimer;

            if (RealRotationTimer < 0.5f)
            {
                ScaleX -= (0.5f - RealRotationTimer) / 4;
            }

            Card.DrawCardMiniatureCentered(g, null, Map.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, FaceDownValue);
        }

        private void IntroduceDefenderItem(CustomSpriteBatch g)
        {
            float RealRotationTimer = (float)ItemAnimationTime * 2;
            float FinalX = Constants.Width - Constants.Width / 12;
            float StartX = Constants.Width + 50;
            float FinalY = Constants.Height / 5.3f;
            float StartY = Constants.Height / 2;
            float TransitionX = Constants.Width - Constants.Width / 4;
            float TransitionY = Constants.Height / 4;

            float t = RealRotationTimer;

            int n = 2;
            double ResultX = Math.Pow(1 - t, n) * StartX;
            double ResultY = Math.Pow(1 - t, n) * StartY;

            //n! / ((n-i)!*i!) * ((1 - t)^(n-1)) * (t ^ i) * Point[i]
            ResultX += n * (1 - t) * t * TransitionX;
            ResultY += n * (1 - t) * t * TransitionY;
            ResultX += Math.Pow(t, n) * FinalX;
            ResultY += Math.Pow(t, n) * FinalY;

            float StartScale = ItemCardScale * 1.5f;
            float FinalScale = ItemCardScale;
            float ScaleDifference = FinalScale - StartScale;
            float ScaleX = StartScale + ScaleDifference * RealRotationTimer;
            float ScaleY = StartScale + ScaleDifference * RealRotationTimer;

            if (RealRotationTimer < 0.5f)
            {
                ScaleX -= (0.5f - RealRotationTimer) / 4;
            }

            Card.DrawCardMiniatureCentered(g, null, Map.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, FaceDownValue);
        }

        private void RevealInvaderItem(CustomSpriteBatch g)
        {
            float RealRotationTimer = (float)(ItemAnimationTime - 0.5f) * 2;
            float FinalX = Constants.Width / 12;
            float StartX = Constants.Width / 12;
            float FinalY = Constants.Height / 5.3f;
            float StartY = Constants.Height / 5.3f;
            float TransitionX = Constants.Width / 3;
            float TransitionY = -Constants.Height /24;
            float StartScaleY = ItemCardScale;
            float TransitionScaleY = ItemCardScale * 3;
            float FinalScaleY = ItemCardScale;

            float t = RealRotationTimer;

            int n = 2;
            double ResultX = Math.Pow(1 - t, n) * StartX;
            double ResultY = Math.Pow(1 - t, n) * StartY;
            double ResultScaleY = Math.Pow(1 - t, n) * StartScaleY;

            //n! / ((n-i)!*i!) * ((1 - t)^(n-1)) * (t ^ i) * Point[i]
            ResultX += n * (1 - t) * t * TransitionX;
            ResultY += n * (1 - t) * t * TransitionY;
            ResultScaleY += n * (1 - t) * t * TransitionScaleY;
            ResultX += Math.Pow(t, n) * FinalX;
            ResultY += Math.Pow(t, n) * FinalY;
            ResultScaleY += Math.Pow(t, n) * FinalScaleY;

            float StartScale = ItemCardScale;
            float FinalScale = -ItemCardScale;
            float ScaleDifference = FinalScale - StartScale;
            float ScaleX = StartScale + ScaleDifference * RealRotationTimer;
            float ScaleY = (float)ResultScaleY;

            if (ScaleX > 0)
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.InvaderItem.sprCard, Map.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, FaceDownValue);
            }
            else
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.InvaderItem.sprCard, Map.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, MathHelper.Pi);
            }
        }

        private void DiscardInvaderItem(CustomSpriteBatch g)
        {
            float RealRotationTimer = (float)(ItemAnimationTime - 0.5f) * 2;
            float StartX = Constants.Width / 12;
            float FinalX = -10;
            float StartY = Constants.Height / 5.3f;
            float FinalY = Constants.Height / 2;
            float TransitionX = Constants.Width / 4;
            float TransitionY = Constants.Height / 4;

            float t = RealRotationTimer;

            int n = 2;
            double ResultX = Math.Pow(1 - t, n) * StartX;
            double ResultY = Math.Pow(1 - t, n) * StartY;

            //n! / ((n-i)!*i!) * ((1 - t)^(n-1)) * (t ^ i) * Point[i]
            ResultX += n * (1 - t) * t * TransitionX;
            ResultY += n * (1 - t) * t * TransitionY;
            ResultX += Math.Pow(t, n) * FinalX;
            ResultY += Math.Pow(t, n) * FinalY;

            float StartScale = ItemCardScale;
            float FinalScale = ItemCardScale * 2f;
            float ScaleDifference = FinalScale - StartScale;
            float ScaleX = StartScale + ScaleDifference * RealRotationTimer;
            float ScaleY = StartScale + ScaleDifference * RealRotationTimer;

            if (RealRotationTimer > 0.5f)
            {
                ScaleX -= ((RealRotationTimer - 0.5f)) * 0.5f;
            }

            Card.DrawCardMiniatureCentered(g, null, Map.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, FaceDownValue);
        }

        private void RevealDefenderItem(CustomSpriteBatch g)
        {
            float RealRotationTimer = (float)(ItemAnimationTime - 0.5f) * 2;
            float FinalX = Constants.Width - Constants.Width / 12;
            float StartX = Constants.Width - Constants.Width / 12;
            float FinalY = Constants.Height / 5.3f;
            float StartY = Constants.Height / 5.3f;
            float TransitionX = Constants.Width - Constants.Width / 3;
            float TransitionY = -Constants.Height / 24;
            float StartScaleY = ItemCardScale;
            float TransitionScaleY = ItemCardScale * 3;
            float FinalScaleY = ItemCardScale;

            float t = RealRotationTimer;

            int n = 2;
            double ResultX = Math.Pow(1 - t, n) * StartX;
            double ResultY = Math.Pow(1 - t, n) * StartY;
            double ResultScaleY = Math.Pow(1 - t, n) * StartScaleY;

            //n! / ((n-i)!*i!) * ((1 - t)^(n-1)) * (t ^ i) * Point[i]
            ResultX += n * (1 - t) * t * TransitionX;
            ResultY += n * (1 - t) * t * TransitionY;
            ResultScaleY += n * (1 - t) * t * TransitionScaleY;
            ResultX += Math.Pow(t, n) * FinalX;
            ResultY += Math.Pow(t, n) * FinalY;
            ResultScaleY += Math.Pow(t, n) * FinalScaleY;

            float StartScale = ItemCardScale;
            float FinalScale = -ItemCardScale;
            float ScaleDifference = FinalScale - StartScale;
            float ScaleX = StartScale + ScaleDifference * RealRotationTimer;
            float ScaleY = (float)ResultScaleY;

            if (ScaleX > 0)
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.DefenderItem.sprCard, Map.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, FaceDownValue);
            }
            else
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.DefenderItem.sprCard, Map.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, MathHelper.Pi);
            }
        }

        private void DiscardDefenderItem(CustomSpriteBatch g)
        {
            float RealRotationTimer = (float)(ItemAnimationTime - 0.5f) * 2;
            float StartX = Constants.Width - Constants.Width / 12;
            float FinalX = Constants.Width + 290;
            float StartY = Constants.Height / 5.3f;
            float FinalY = Constants.Height / 2;
            float TransitionX = Constants.Width - Constants.Width / 4;
            float TransitionY = Constants.Height / 4;

            float t = RealRotationTimer;

            int n = 2;
            double ResultX = Math.Pow(1 - t, n) * StartX;
            double ResultY = Math.Pow(1 - t, n) * StartY;

            //n! / ((n-i)!*i!) * ((1 - t)^(n-1)) * (t ^ i) * Point[i]
            ResultX += n * (1 - t) * t * TransitionX;
            ResultY += n * (1 - t) * t * TransitionY;
            ResultX += Math.Pow(t, n) * FinalX;
            ResultY += Math.Pow(t, n) * FinalY;

            float StartScale = ItemCardScale;
            float FinalScale = ItemCardScale * 2f;
            float ScaleDifference = FinalScale - StartScale;
            float ScaleX = StartScale + ScaleDifference * RealRotationTimer;
            float ScaleY = StartScale + ScaleDifference * RealRotationTimer;

            if (RealRotationTimer > 0.5f)
            {
                ScaleX -= ((RealRotationTimer - 0.5f)) * 0.5f;
            }

            Card.DrawCardMiniatureCentered(g, null, Map.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, FaceDownValue);
        }
    }
}
