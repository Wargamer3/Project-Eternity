using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelDialogPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "DialogPhase";

        public enum QuoteEventTypes { TerritoryCommand, BattleEnd, TerritoryClaim }
        public enum QuoteTypes { Introduction, Banter, TerritoryClaim, Chain, TerritoryLevelUp, SuccessfulInvasion, FailedInvasion, Defense, MoneyGain, MoneyLoss, ObjectiveAchieved, Won }

        private List<QuoteSetVersus> QuoteSet;
        private Texture2D sprPortrait;

        private static DynamicText Text;

        private Player ActivePlayer;
        private double AITimer;

        public ActionPanelDialogPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelDialogPhase(SorcererStreetMap Map, List<QuoteSetVersus> ListQuoteSet)
            : base(PanelName, Map, false)
        {
            this.QuoteSet = ListQuoteSet;
            ActivePlayer = Map.ListPlayer[Map.ActivePlayerIndex];

            if (Text == null)
            {
                Text = new DynamicText();
                Text.TextMaxWidthInPixel = Constants.Width;
                Text.LineHeight = 20;
                Text.ListProcessor.Add(new RegularTextProcessor(Text, Map.fntDefaultText));
                Text.ListProcessor.Add(new IconProcessor(Text));
                Text.ListProcessor.Add(new PlayerNameProcessor(Text, Map.fntDefaultText, Map));
                Text.ListProcessor.Add(new DefaultTextProcessor(Text, Map.fntDefaultText));
                Text.SetDefaultProcessor(new DefaultTextProcessor(Text, Map.fntDefaultText));
                Text.Load(Map.Content);
            }

            int TotalQuotes = 0;
            foreach (QuoteSetVersus ActiveQuoteSet in ListQuoteSet)
            {
                TotalQuotes += ActiveQuoteSet.ListQuote.Count;
            }

            if (TotalQuotes > 0)
            {
                int RandomIndex = RandomHelper.Next(TotalQuotes);

                foreach (QuoteSetVersus ActiveQuoteSet in ListQuoteSet)
                {
                    if (RandomIndex >= ActiveQuoteSet.ListQuote.Count)
                    {
                        RandomIndex -= ActiveQuoteSet.ListQuote.Count;
                        continue;
                    }

                    Text.ParseText(ActiveQuoteSet.ListQuote[RandomIndex]);
                }
            }
        }

        public static void AddIntrodctionIfAvailable(SorcererStreetMap Map)
        {
            if (Map.GameTurn == 0)
            {
                Player ActivePlayer = Map.ListPlayer[Map.ActivePlayerIndex];
                TerrainSorcererStreet HoverTerrain = Map.GetTerrain(new Vector3(ActivePlayer.GamePiece.X, ActivePlayer.GamePiece.Y / Map.TileSize.Y, ActivePlayer.GamePiece.Z));

                List<QuoteSetVersus> ListQuoteSet = ExtractQuote(Map, GetQuote(Map, ActivePlayer, HoverTerrain, QuoteTypes.Introduction), ActivePlayer);

                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelDialogPhase(Map, ListQuoteSet));
            }
        }

        //Battle End/Territory Claim/Territory Command end rather.
        public static QuoteSet GetQuote(SorcererStreetMap Map, Player ActivePlayer, TerrainSorcererStreet ActiveTerrain, QuoteTypes QuoteType)
        {
            switch (QuoteType)
            {
                case QuoteTypes.Introduction:
                    if (Map.DicTeam[ActivePlayer.TeamIndex].ListPlayer.Count > 1)
                    {
                        return ActivePlayer.Inventory.Character.Character.AllianceIntroduction;
                    }
                    else
                    {
                        return ActivePlayer.Inventory.Character.Character.Introduction;
                    }

                case QuoteTypes.Banter:
                    if (Map.DicTeam[ActivePlayer.TeamIndex].ListPlayer.Count > 1)
                    {
                        if (Map.DicTeam[ActivePlayer.TeamIndex].Rank == 1)
                        {
                            return ActivePlayer.Inventory.Character.Character.WinningAllianceBanter;
                        }
                        else if (Map.DicTeam[ActivePlayer.TeamIndex].Rank < 1)
                        {
                            return ActivePlayer.Inventory.Character.Character.LosingAllianceBanter;
                        }
                        else if (Map.DicTeam[ActivePlayer.TeamIndex].Rank == Map.DicTeam.Count)
                        {
                            return ActivePlayer.Inventory.Character.Character.MajorLosingBanter;
                        }
                        return ActivePlayer.Inventory.Character.Character.AllianceBanter;
                    }
                    else
                    {
                        if (Map.DicTeam[ActivePlayer.TeamIndex].Rank == 1)
                        {
                            return ActivePlayer.Inventory.Character.Character.WinningBanter;
                        }
                        else if (Map.DicTeam[ActivePlayer.TeamIndex].Rank < 1)
                        {
                            return ActivePlayer.Inventory.Character.Character.LosingBanter;
                        }
                        return ActivePlayer.Inventory.Character.Character.Banter;
                    }

                case QuoteTypes.TerritoryClaim:
                    return ActivePlayer.Inventory.Character.Character.TerritoryClaim;

                case QuoteTypes.Chain:
                    if (Map.DicTeam[ActivePlayer.TeamIndex].DicCreatureCountByElementType[ActiveTerrain.TerrainTypeIndex] > 2)
                    {
                        return ActivePlayer.Inventory.Character.Character.ChainBig;
                    }
                    else
                    {
                        return ActivePlayer.Inventory.Character.Character.ChainSmall;
                    }

                case QuoteTypes.TerritoryLevelUp:
                    if (ActiveTerrain.LandLevel > 3)
                    {
                        return ActivePlayer.Inventory.Character.Character.TerritoryLevelUpBig;
                    }
                    else
                    {
                        return ActivePlayer.Inventory.Character.Character.TerritoryLevelUp;
                    }

                case QuoteTypes.SuccessfulInvasion:
                    return ActivePlayer.Inventory.Character.Character.SuccessfulInvasion;

                case QuoteTypes.FailedInvasion:
                    return ActivePlayer.Inventory.Character.Character.FailedInvasion;

                case QuoteTypes.MoneyGain:
                    return ActivePlayer.Inventory.Character.Character.SmallMoneyGains;
            }

            return null;
        }

        public static QuoteSet GetMoneyQuote(Player ActivePlayer, int MoneyChange)
        {
            if (MoneyChange < -1000)
            {
                return ActivePlayer.Inventory.Character.Character.LargeMoneyLoss;
            }
            else if (MoneyChange < -500)
            {
                return ActivePlayer.Inventory.Character.Character.MediumMoneyLoss;
            }
            else if (MoneyChange < -100)
            {
                return ActivePlayer.Inventory.Character.Character.SmallMoneyLoss;
            }
            else if (MoneyChange > 100)
            {
                return ActivePlayer.Inventory.Character.Character.BigMoneyGains;
            }
            else if (MoneyChange > 0)
            {
                return ActivePlayer.Inventory.Character.Character.SmallMoneyGains;
            }
            return null;
        }

        private static List<QuoteSetVersus> ExtractQuote(SorcererStreetMap Map, QuoteSet ActiveQuoteSet, Player ActivePlayer)
        {
            List<QuoteSetMap> ListQuoteSetVersus = new List<QuoteSetMap>();
            ListQuoteSetVersus.Add(ActiveQuoteSet.ListMapQuote[0]);
            List<QuoteSetVersus> ListReturnQuote = new List<QuoteSetVersus>();

            for (int M = 1; M < ActiveQuoteSet.ListMapQuote.Count; ++M)
            {
                if (ActivePlayer.Inventory.Character.Character.ListQuoteSetMapName[M - 1] == Map.MapName)
                {
                    ListQuoteSetVersus.Add(ActiveQuoteSet.ListMapQuote[M]);
                    break;
                }
            }

            foreach (QuoteSetMap ActiveQuoteSetVersus in ListQuoteSetVersus)
            {
                ListReturnQuote.AddRange(ExtractText(Map, ActiveQuoteSetVersus, ActivePlayer));
            }

            return ListReturnQuote;
        }

        private static List<QuoteSetVersus> ExtractText(SorcererStreetMap Map, QuoteSetMap ActiveQuoteSet, Player ActivePlayer)
        {
            List<QuoteSetVersus> ListReturnQuote = new List<QuoteSetVersus>();
            ListReturnQuote.Add(ActiveQuoteSet.ListQuoteVersus[0]);

            if (Map.GlobalSorcererStreetBattleContext.OpponentCreature != null && Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner != null)
            {
                for (int P = 1; P < ActiveQuoteSet.ListQuoteVersus.Count; ++P)
                {
                    if (ActivePlayer.Inventory.Character.Character.ListQuoteSetVersusName[P - 1] == Map.GlobalSorcererStreetBattleContext.OpponentCreature.Owner.Inventory.Character.Character.Name)
                    {
                        ListReturnQuote.Add(ActiveQuoteSet.ListQuoteVersus[P]);
                        break;
                    }
                }
            }

            return ListReturnQuote;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Text.Update(gameTime);

            if (!ActivePlayer.IsPlayerControlled)
            {
                AITimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (AITimer >= 1)
                {
                    RemoveFromPanelList(this);
                }

                return;
            }

            if (ActiveInputManager.InputConfirmPressed())
            {
                RemoveFromPanelList(this);
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelDialogPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Scale = 1.4f;
            int TextboxWidth = 700;
            g.Draw(Map.sprPortraitStart, new Vector2(0, (int)(Constants.Height - Map.sprPortraitStart.Height * Scale)), null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 1f);
            g.Draw(Map.sprPortraitMiddle, new Rectangle((int)(Map.sprPortraitStart.Width * Scale - Scale), (int)(Constants.Height - Map.sprPortraitMiddle.Height * Scale), (int)(TextboxWidth * Scale), (int)(Map.sprPortraitMiddle.Height * Scale)), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            g.Draw(Map.sprPortraitEnd, new Vector2((Map.sprPortraitStart.Width + TextboxWidth) * Scale - 3 * Scale, (int)(Constants.Height - Map.sprPortraitEnd.Height * Scale)), null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 1f);
            g.DrawString(Map.fntFinlanderFont, "Zeneth", new Vector2(370 * Scale, Constants.Height - 230 * Scale), Color.White);
            Text.Draw(g, new Vector2(350 * Scale, Constants.Height - 170 * Scale));

            g.End();
            g.Begin();

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 300, -300);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Projection = HalfPixelOffset * Projection;

            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            PlayerCharacter ActivePlayer = Map.ListPlayer[Map.ActivePlayerIndex].Inventory.Character.Character;
            ActivePlayer.Unit3DModel.Draw(Matrix.CreateRotationZ(MathHelper.ToRadians(180)) * Matrix.CreateRotationY(MathHelper.ToRadians(180)) * Matrix.CreateScale(7f) * Matrix.CreateTranslation(150, 2000, 0), Projection, Matrix.Identity);

            g.End();
            g.Begin();
        }
    }
}
