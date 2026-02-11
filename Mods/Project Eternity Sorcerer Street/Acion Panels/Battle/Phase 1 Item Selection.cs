using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen.Online;
using static ProjectEternity.GameScreens.SorcererStreetScreen.ActionPanelBattle;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleItemSelectionPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "BattleItemSelection";

        private const float ItemCardScale = 0.4f;

        private bool ItemSelected;
        private double ItemAnimationTime;
        private BattleContent BattleAssets;

        public ActionPanelBattleItemSelectionPhase(SorcererStreetMap Map)
            : base(PanelName, Map, ItemCard.ItemCardType)
        {
            DrawDrawInfo = false;
        }

        public ActionPanelBattleItemSelectionPhase(SorcererStreetMap Map, int ActivePlayerIndex, BattleContent BattleAssets)
            : base(PanelName, Map.ListActionMenuChoice, Map, ActivePlayerIndex, ItemCard.ItemCardType, "End")
        {
            DrawDrawInfo = false;
            this.BattleAssets = BattleAssets;
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
                        AddToPanelListAndSelect(new ActionPanelBattleItemSelectionPhase(Map, Map.GlobalSorcererStreetBattleContext.OpponentCreature.PlayerIndex, BattleAssets));
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
                        AddToPanelListAndSelect(new ActionPanelBattleLandModifierPhase(Map, BattleAssets));
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
                Map.GlobalSorcererStreetBattleContext.SelfCreature.Item = CardSelected;
            }
            else
            {
                Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item = CardSelected;
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

            Map.GlobalSorcererStreetBattleContext.SelfCreature.Animation.Draw(g);
            Map.GlobalSorcererStreetBattleContext.OpponentCreature.Animation.Draw(g);

            if (!ItemSelected)
            {
                float Ratio = Constants.Height / 720f;
                base.Draw(g);

                if (ActivePlayerIndex != Map.ActivePlayerIndex)
                {
                    Card.DrawCardMiniature(g, null, MenuHelper.sprCardBack, Color.White, Constants.Width / 8, Constants.Height / 16, ItemCardScale, ItemCardScale, true);
                }

                MenuHelper.DrawBorderlessBox(g, new Vector2(Constants.Width / 2 - (int)(200 * Ratio), Constants.Height / 12), (int)(400 * Ratio), (int)(40 * Ratio));
                g.DrawStringMiddleAligned(Map.fntMenuText, Map.ListPlayer[ActivePlayerIndex].Name + "'s item selection", new Vector2(Constants.Width - Constants.Width / 2, Constants.Height / 12 + (int)(5 * Ratio)), SorcererStreetMap.TextColor);

                int Y = (int)(165 * Ratio);
                int BoxX = Constants.Width / 16;
                int BoxWidth = Constants.Width - Constants.Width / 8;
                g.Draw(BattleAssets.sprBattle, new Vector2(Constants.Width / 2, Y + 16 * Ratio), null, Color.White, 0f, new Vector2((int)(BattleAssets.sprBattle.Width / 2f), BattleAssets.sprBattle.Height), 1f, SpriteEffects.None, 0f);
                Y += 23;
                MenuHelper.DrawBox(g, new Vector2(BoxX, Y), BoxWidth, (int)(185 * Ratio), 1);
                Y = (int)(190 * Ratio);

                int IconX = BoxX + (int)(30 * Ratio);
                Y = (int)(190 * Ratio);
                foreach (var ActiveIcon in Map.GlobalSorcererStreetBattleContext.InvaderCreature.Creature.GetIcons(CardSymbols.Symbols))
                {
                    g.Draw(ActiveIcon, new Vector2(IconX, Y), Color.White);

                    IconX += ActiveIcon.Width + 10;
                }

                IconX = BoxX + BoxWidth - (int)(30 * Ratio);
                foreach (var ActiveIcon in Map.GlobalSorcererStreetBattleContext.DefenderCreature.Creature.GetIcons(CardSymbols.Symbols))
                {
                    IconX -= ActiveIcon.Width;
                    g.Draw(ActiveIcon, new Vector2(IconX, Y), Color.White);

                    IconX -= 10;
                }
                g.DrawStringMiddleAligned(BattleAssets.fntMenuText, Map.GlobalSorcererStreetBattleContext.InvaderCreature.Owner.Name, new Vector2(Constants.Width / 4, Y), SorcererStreetMap.TextColor);
                g.DrawStringMiddleAligned(BattleAssets.fntMenuText, Map.GlobalSorcererStreetBattleContext.DefenderCreature.Owner.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), SorcererStreetMap.TextColor);
                g.DrawStringMiddleAligned(Map.fntMenuText, "Invasion / Defense", new Vector2(Constants.Width - Constants.Width / 2, Y), SorcererStreetMap.TextColor);
                Y = (int)(225 * Ratio);
                g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 12, Y), new Vector2(Constants.Width - Constants.Width / 12, Y), Color.White);

                //Invader
                Y = (int)(230 * Ratio);
                g.DrawStringMiddleAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.Name, new Vector2(Constants.Width / 4, Y), SorcererStreetMap.TextColor);
                int X = Constants.Width / 4;
                Y += (int)(30 * Ratio);
                g.Draw(Map.Symbols.sprMenuST, new Rectangle(X - (int)(85 * Ratio), Y + (int)(3 * Ratio), 17, 32), Color.White);
                g.DrawStringRightAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.CurrentST.ToString(), new Vector2(X - (int)(25 * Ratio), Y), SorcererStreetMap.TextColor);
                g.Draw(Map.Symbols.sprMenuHP, new Rectangle(X + (int)(25 * Ratio), Y + (int)(3 * Ratio), 17, 32), Color.White);
                g.DrawStringRightAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.CurrentHP.ToString(), new Vector2(X + (int)(85 * Ratio), Y), SorcererStreetMap.TextColor);
                g.DrawStringMiddleAligned(Map.fntMenuText, "Ability Values", new Vector2(Constants.Width / 2, Y), SorcererStreetMap.TextColor);

                Y += (int)(30 * Ratio);
                g.DrawStringRightAligned(Map.fntMenuText, "+" + Map.GlobalSorcererStreetBattleContext.SelfCreature.BonusST, new Vector2(X - (int)(25 * Ratio), Y), SorcererStreetMap.TextColor);
                g.DrawStringRightAligned(Map.fntMenuText, "+" + Map.GlobalSorcererStreetBattleContext.SelfCreature.LandHP, new Vector2(X + (int)(85 * Ratio), Y), SorcererStreetMap.TextColor);
                g.DrawStringMiddleAligned(Map.fntMenuText, "Support / Land", new Vector2(Constants.Width / 2, Y), SorcererStreetMap.TextColor);
                Y += (int)(40 * Ratio);
                g.Draw(Map.Symbols.sprMenuST, new Rectangle(X - (int)(85 * Ratio), Y + (int)(3 * Ratio), 17, 32), Color.White);
                g.DrawStringRightAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.SelfCreature.FinalST.ToString(), new Vector2(X - (int)(25 * Ratio), Y), SorcererStreetMap.TextColor);
                g.Draw(Map.Symbols.sprMenuHP, new Rectangle(X + (int)(25 * Ratio), Y + (int)(3 * Ratio), 17, 32), Color.White);
                g.DrawStringRightAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.SelfCreature.FinalHP.ToString(), new Vector2(X + (int)(85 * Ratio), Y), SorcererStreetMap.TextColor);
                g.DrawStringMiddleAligned(Map.fntMenuText, "Total", new Vector2(Constants.Width / 2, Y), SorcererStreetMap.TextColor);
                Y = (int)(325 * Ratio);
                g.DrawLine(GameScreen.sprPixel, new Vector2(Constants.Width / 12, Y), new Vector2(Constants.Width - Constants.Width / 12, Y), Color.White);

                //Defender
                X = Constants.Width - Constants.Width / 4;
                Y = (int)(230 * Ratio);
                g.DrawStringMiddleAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.Name, new Vector2(Constants.Width - Constants.Width / 4, Y), SorcererStreetMap.TextColor);
                Y += (int)(30 * Ratio);
                g.Draw(Map.Symbols.sprMenuST, new Rectangle(X - (int)(85 * Ratio), Y, 17, 32), Color.White);
                g.DrawStringRightAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.CurrentST.ToString(), new Vector2(X - (int)(25 * Ratio), Y), SorcererStreetMap.TextColor);
                g.Draw(Map.Symbols.sprMenuHP, new Rectangle(X + (int)(25 * Ratio), Y, 17, 32), Color.White);
                g.DrawStringRightAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.CurrentHP.ToString(), new Vector2(X + (int)(85 * Ratio), Y), SorcererStreetMap.TextColor);

                Y += (int)(30 * Ratio);
                g.DrawStringRightAligned(Map.fntMenuText, "+" + Map.GlobalSorcererStreetBattleContext.OpponentCreature.BonusST, new Vector2(X - (int)(25 * Ratio), Y), SorcererStreetMap.TextColor);
                g.DrawStringRightAligned(Map.fntMenuText, "+" + Map.GlobalSorcererStreetBattleContext.OpponentCreature.LandHP, new Vector2(X + (int)(85 * Ratio), Y), SorcererStreetMap.TextColor);
                Y += (int)(40 * Ratio);
                g.Draw(Map.Symbols.sprMenuST, new Rectangle(X - (int)(85 * Ratio), Y + (int)(3 * Ratio), 17, 32), Color.White);
                g.DrawStringRightAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.OpponentCreature.FinalST.ToString(), new Vector2(X - (int)(25 * Ratio), Y), SorcererStreetMap.TextColor);
                g.Draw(Map.Symbols.sprMenuHP, new Rectangle(X + (int)(25 * Ratio), Y + (int)(3 * Ratio), 17, 32), Color.White);
                g.DrawStringRightAligned(Map.fntMenuText, Map.GlobalSorcererStreetBattleContext.OpponentCreature.FinalHP.ToString(), new Vector2(X + (int)(85 * Ratio), Y), SorcererStreetMap.TextColor);
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
                        if (Map.GlobalSorcererStreetBattleContext.SelfCreature.Item == null)
                        {
                            DiscardInvaderItem(g);
                        }
                        else
                        {
                            RevealInvaderItem(g);
                        }

                        if (Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item == null)
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
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.SelfCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, true);
            }
            else
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.SelfCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, false);
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
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, true);
            }
            else
            {
                Card.DrawCardMiniatureCentered(g, Map.GlobalSorcererStreetBattleContext.OpponentCreature.Item.sprCard, MenuHelper.sprCardBack, Color.White, (float)ResultX, (float)ResultY, ScaleX, ScaleY, false);
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
