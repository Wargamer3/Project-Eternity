using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    class SinglePlayerGameRule : IGameRule
    {
        private readonly SorcererStreetMap Owner;

        public string Name => "Single Player";

        public SinglePlayerGameRule(SorcererStreetMap Owner)
        {
            this.Owner = Owner;
        }

        public void Init()
        {
            if (Owner.IsOfflineOrServer)
            {
                for (int P = 0; P < Owner.ListPlayer.Count; P++)
                {
                    Player ActivePlayer = Owner.ListPlayer[P];
                    if (ActivePlayer.Inventory == null)
                        continue;

                    List<MovementAlgorithmTile> ListPossibleSpawnPoint = Owner.GetMultiplayerSpawnLocations(ActivePlayer.Team);
                    int SpawnSquadIndex = 0;
                    foreach (MovementAlgorithmTile ActiveSpawn in ListPossibleSpawnPoint)
                    {
                        ActivePlayer.GamePiece.SetPosition(new Vector3(ActiveSpawn.InternalPosition.X, ActiveSpawn.InternalPosition.Y, ActiveSpawn.LayerIndex));

                        ++SpawnSquadIndex;

                        if (!ActivePlayer.IsPlayerControlled)
                        {
                            //ActivePlayer.GamePiece.AI = new SorcererStreetAIContainer(new SorcererStreetAIInfo(Owner, ActivePlayer));
                            //ActivePlayer.GamePiece.AI.Load("Multiplayer/Easy");
                        }

                        if (Owner != ActiveSpawn.Owner)
                        {
                            ActiveSpawn.Owner.AddUnit(P, ActivePlayer.GamePiece, ActiveSpawn);
                            Owner.RemoveUnit(P, ActivePlayer.GamePiece);
                            Owner.SelectPlatform(Owner.GetPlatform(ActiveSpawn.Owner));
                        }

                        for (int C = 0; C < 4 && ActivePlayer.ListRemainingCardInDeck.Count > 0; ++C)
                        {
                            int RandomCardIndex = RandomHelper.Next(ActivePlayer.ListRemainingCardInDeck.Count);
                            Card DrawnCard = ActivePlayer.ListRemainingCardInDeck[RandomCardIndex];

                            ActivePlayer.ListCardInHand.Add(DrawnCard);
                            ActivePlayer.ListRemainingCardInDeck.RemoveAt(RandomCardIndex);
                        }

                        ActivePlayer.Team = P;
                        ActivePlayer.TotalMagic = ActivePlayer.Gold = Owner.MagicAtStart;
                        break;
                    }
                }
            }
        }

        public int GetRemainingResapwn(int PlayerIndex)
        {
            throw new NotImplementedException();
        }

        public void OnNewTurn(int ActivePlayerIndex)
        {
        }

        public void OnSquadDefeated(int AttackerSquadPlayerIndex, Squad AttackerSquad, int DefeatedSquadPlayerIndex, Squad DefeatedSquad)
        {
        }

        public void OnManualVictory(int EXP, uint Money)
        {
        }

        public void OnManualDefeat(int EXP, uint Money)
        {
        }

        public void Update(GameTime gameTime)
        {
            if (!Owner.IsEditor)
            {
                if (!Owner.ListPlayer[Owner.ActivePlayerIndex].IsOnline)
                {
                    Owner.ListActionMenuChoice.Last().Update(gameTime);
                }
                else if (Owner.ListPlayer[Owner.ActivePlayerIndex].IsOnline)
                {
                    Owner.ListActionMenuChoice.Last().UpdatePassive(gameTime);
                }
            }
        }

        public void BeginDraw(CustomSpriteBatch g)
        {

        }

        public void Draw(CustomSpriteBatch g)
        {

        }

        public List<GameRuleError> Validate(RoomInformations Room)
        {
            return new List<GameRuleError>();
        }
    }
}
