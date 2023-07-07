using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleItemSelectionPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "BattleItemSelection";

        private const float ItemCardScale = 0.4f;

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
                        AddToPanelListAndSelect(new ActionPanelBattleItemSelectionPhase(Map, Map.GlobalSorcererStreetBattleContext.Defender.PlayerIndex));
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
                Map.GlobalSorcererStreetBattleContext.Invader.Item = CardSelected;
            }
            else
            {
                Map.GlobalSorcererStreetBattleContext.Defender.Item = CardSelected;
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
            return new byte[] { (byte)AnimationPhase, (byte)ActionMenuCursor, (byte)(ItemSelected ? 1 : 0) };
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleItemSelectionPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1, 0);
            Map.GlobalSorcererStreetBattleContext.Background.Draw(g, Constants.Width, Constants.Height);

            Map.GlobalSorcererStreetBattleContext.Invader.Animation.Draw(g);
            Map.GlobalSorcererStreetBattleContext.Defender.Animation.Draw(g);

            if (!ItemSelected)
            {
                base.Draw(g);

                if (ActivePlayerIndex != Map.ActivePlayerIndex)
                {
                    Card.DrawCardMiniature(g, null, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, Constants.Height / 16, ItemCardScale, ItemCardScale, true);
                }

                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 6, Constants.Height / 12), Constants.Width - Constants.Width / 3, 30);
                g.DrawStringMiddleAligned(Map.fntArial12, Map.ListPlayer[ActivePlayerIndex].Name + "'s item selection", new Vector2(Constants.Width - Constants.Width / 2, Constants.Height / 12 + 5), Color.White);

                int Y = Constants.Height / 4 - 25;
                TextHelper.DrawTextMiddleAligned(g, "BATTLE", new Vector2(Constants.Width / 2, Y), Color.White);
                Y = Constants.Height / 4;
                MenuHelper.DrawBox(g, new Vector2(Constants.Width / 10, Y), Constants.Width - Constants.Width / 8, Constants.Height / 3);
                Y = Constants.Height / 4 + 10;
                g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.Owner.Name, new Vector2(Constants.Width / 4, Y), Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Defender.Owner.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, "Invasion / Defense", new Vector2(Constants.Width - Constants.Width / 2, Y), Color.White);

                //Invader
                Y = Constants.Height / 4 + 35;
                g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 7, Y), new Vector2(Constants.Width - Constants.Width / 7, Y), Color.White);
                Y = Constants.Height / 4 + 40;
                g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.Animation.Name, new Vector2(Constants.Width / 4, Y), Color.White);
                int X = Constants.Width / 4;
                Y += 30;
                g.Draw(Map.Symbols.sprMenuST, new Rectangle(X - 50, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.Creature.CurrentST.ToString(), new Vector2(X - 20, Y), Color.White);
                g.Draw(Map.Symbols.sprMenuHP, new Rectangle(X + 10, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.Creature.MaxHP.ToString(), new Vector2(X + 45, Y), Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, "Ability Values", new Vector2(Constants.Width / 2, Y), Color.White);

                Y += 25;
                g.DrawString(Map.fntArial12, "+0", new Vector2(X - 20, Y), Color.White);
                g.DrawString(Map.fntArial12, "+0", new Vector2(X + 45, Y), Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, "Support / Land", new Vector2(Constants.Width / 2, Y), Color.White);
                Y += 25;
                g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 7, Y), new Vector2(Constants.Width - Constants.Width / 7, Y), Color.White);
                Y += 10;
                g.Draw(Map.Symbols.sprMenuST, new Rectangle(X - 50, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.FinalST.ToString(), new Vector2(X - 20, Y), Color.White);
                g.Draw(Map.Symbols.sprMenuHP, new Rectangle(X + 10, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Invader.FinalHP.ToString(), new Vector2(X + 45, Y), Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, "Total", new Vector2(Constants.Width / 2, Y), Color.White);

                //Defender
                X = Constants.Width - Constants.Width / 4;
                Y = Constants.Height / 4 + 40;
                g.DrawStringMiddleAligned(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Defender.Animation.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), Color.White);
                Y += 30;
                g.Draw(Map.Symbols.sprMenuST, new Rectangle(X - 50, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Defender.Creature.CurrentST.ToString(), new Vector2(X - 20, Y), Color.White);
                g.Draw(Map.Symbols.sprMenuHP, new Rectangle(X + 10, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Defender.Creature.MaxHP.ToString(), new Vector2(X + 45, Y), Color.White);

                Y += 25;
                g.DrawString(Map.fntArial12, "+0", new Vector2(X - 20, Y), Color.White);
                g.DrawString(Map.fntArial12, "+0", new Vector2(X + 45, Y), Color.White);
                Y += 25;
                g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 7, Y), new Vector2(Constants.Width - Constants.Width / 7, Y), Color.White);
                Y += 10;
                g.Draw(Map.Symbols.sprMenuST, new Rectangle(X - 50, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Defender.FinalST.ToString(), new Vector2(X - 20, Y), Color.White);
                g.Draw(Map.Symbols.sprMenuHP, new Rectangle(X + 10, Y, 20, 20), Color.White);
                g.DrawString(Map.fntArial12, Map.GlobalSorcererStreetBattleContext.Defender.FinalHP.ToString(), new Vector2(X + 45, Y), Color.White);
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
                        Card.DrawCardMiniature(g, null, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, Constants.Height / 16, ItemCardScale, ItemCardScale, true);
                        IntroduceDefenderItem(g);
                    }
                    else
                    {
                        if (Map.GlobalSorcererStreetBattleContext.Invader.Item == null)
                        {
                            DiscardInvaderItem(g);
                        }
                        else
                        {
                            RevealInvaderItem(g);
                        }

                        if (Map.GlobalSorcererStreetBattleContext.Defender.Item == null)
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
            float FinalX = Constants.Width / 8;
            float StartX = -50;
            float FinalY = Constants.Height / 16 + (MenuHelper.sprCardBack.Height / 2) * ItemCardScale;
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

            Card.DrawCardMiniatureCentered(g, null, MenuHelper.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, true);
        }

        private void IntroduceDefenderItem(CustomSpriteBatch g)
        {
            float RealRotationTimer = (float)ItemAnimationTime * 2;
            float FinalX = Constants.Width - Constants.Width / 8;
            float StartX = Constants.Width + 50;
            float FinalY = Constants.Height / 16 + (MenuHelper.sprCardBack.Height / 2) * ItemCardScale;
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

            Card.DrawCardMiniatureCentered(g, null, MenuHelper.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, true);
        }

        private void RevealInvaderItem(CustomSpriteBatch g)
        {
            float RealRotationTimer = (float)(ItemAnimationTime - 0.5f) * 2;
            float FinalX = Constants.Width / 8;
            float StartX = Constants.Width / 8;
            float FinalY = Constants.Height / 16 + (MenuHelper.sprCardBack.Height / 2) * ItemCardScale;
            float StartY = Constants.Height / 16 + (MenuHelper.sprCardBack.Height / 2) * ItemCardScale;
            float TransitionX = Constants.Width / 3;
            float TransitionY = -Constants.Height /24;
            float StartScaleY = ItemCardScale;
            float TransitionScaleY = ItemCardScale * 1.5f;
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
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.Invader.Item.sprCard, MenuHelper.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, true);
            }
            else
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.Invader.Item.sprCard, MenuHelper.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, false);
            }
        }

        private void DiscardInvaderItem(CustomSpriteBatch g)
        {
            float RealRotationTimer = (float)(ItemAnimationTime - 0.5f) * 2;
            float StartX = Constants.Width / 8;
            float FinalX = -10;
            float StartY = Constants.Height / 16 + (MenuHelper.sprCardBack.Height / 2) * ItemCardScale;
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

            Card.DrawCardMiniatureCentered(g, null, MenuHelper.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, true);
        }

        private void RevealDefenderItem(CustomSpriteBatch g)
        {
            float RealRotationTimer = (float)(ItemAnimationTime - 0.5f) * 2;
            float FinalX = Constants.Width - Constants.Width / 8;
            float StartX = Constants.Width - Constants.Width / 8;
            float FinalY = Constants.Height / 16 + (MenuHelper.sprCardBack.Height / 2) * ItemCardScale;
            float StartY = Constants.Height / 16 + (MenuHelper.sprCardBack.Height / 2) * ItemCardScale;
            float TransitionX = Constants.Width - Constants.Width / 3;
            float TransitionY = -Constants.Height / 24;
            float StartScaleY = ItemCardScale;
            float TransitionScaleY = ItemCardScale * 1.5f;
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
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.Defender.Item.sprCard, MenuHelper.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, true);
            }
            else
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.Defender.Item.sprCard, MenuHelper.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, false);
            }
        }

        private void DiscardDefenderItem(CustomSpriteBatch g)
        {
            float RealRotationTimer = (float)(ItemAnimationTime - 0.5f) * 2;
            float StartX = Constants.Width - Constants.Width / 8;
            float FinalX = Constants.Width + 290;
            float StartY = Constants.Height / 16 + (MenuHelper.sprCardBack.Height / 2) * ItemCardScale;
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

            Card.DrawCardMiniatureCentered(g, null, MenuHelper.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, true);
        }
    }
}
