using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class ClassicMPGameRule : IGameRule
    {
        private readonly DeathmatchMap Owner;

        public ClassicMPGameRule(DeathmatchMap Owner)
        {
            this.Owner = Owner;
        }

        public void Init()
        {
            for (int P = 0; P < Owner.ListPlayer.Count; P++)
            {
                Owner.ListPlayer[P].Color = Owner.ListMultiplayerColor[P];

                for (int S = 0; S < Owner.ListPlayer[P].ListSpawnPoint.Count; S++)
                {
                    if (string.IsNullOrEmpty(Owner.ListPlayer[P].ListSpawnPoint[S].LeaderTypeName))
                        continue;

                    Unit NewLeaderUnit = Unit.FromType(Owner.ListPlayer[P].ListSpawnPoint[S].LeaderTypeName, Owner.ListPlayer[P].ListSpawnPoint[S].LeaderName, Owner.Content, Owner.Params.DicUnitType, Owner.Params.DicRequirement, Owner.Params.DicEffect, Owner.Params.DicAutomaticSkillTarget);
                    Character NewLeaderPilot = new Character(Owner.ListPlayer[P].ListSpawnPoint[S].LeaderPilot, Owner.Content, Owner.Params.DicRequirement, Owner.Params.DicEffect, Owner.Params.DicAutomaticSkillTarget, Owner.Params.DicManualSkillTarget);
                    NewLeaderPilot.Level = 1;
                    NewLeaderUnit.ArrayCharacterActive = new Character[1] { NewLeaderPilot };

                    Unit NewWingmanAUnit = null;
                    Unit NewWingmanBUnit = null;

                    if (!string.IsNullOrEmpty(Owner.ListPlayer[P].ListSpawnPoint[S].WingmanAName))
                    {
                        NewWingmanAUnit = Unit.FromType(Owner.ListPlayer[P].ListSpawnPoint[S].WingmanATypeName, Owner.ListPlayer[P].ListSpawnPoint[S].WingmanAName, Owner.Content, Owner.Params.DicUnitType, Owner.Params.DicRequirement, Owner.Params.DicEffect, Owner.Params.DicAutomaticSkillTarget);
                        Character NewWingmanAPilot = new Character(Owner.ListPlayer[P].ListSpawnPoint[S].WingmanAPilot, Owner.Content, Owner.Params.DicRequirement, Owner.Params.DicEffect, Owner.Params.DicAutomaticSkillTarget, Owner.Params.DicManualSkillTarget);
                        NewWingmanAPilot.Level = 1;
                        NewWingmanAUnit.ArrayCharacterActive = new Character[1] { NewWingmanAPilot };
                    }

                    if (!string.IsNullOrEmpty(Owner.ListPlayer[P].ListSpawnPoint[S].WingmanBName))
                    {
                        NewWingmanBUnit = Unit.FromType(Owner.ListPlayer[P].ListSpawnPoint[S].WingmanBTypeName, Owner.ListPlayer[P].ListSpawnPoint[S].WingmanBName, Owner.Content, Owner.Params.DicUnitType, Owner.Params.DicRequirement, Owner.Params.DicEffect, Owner.Params.DicAutomaticSkillTarget);
                        Character NewWingmanBPilot = new Character(Owner.ListPlayer[P].ListSpawnPoint[S].WingmanBPilot, Owner.Content, Owner.Params.DicRequirement, Owner.Params.DicEffect, Owner.Params.DicAutomaticSkillTarget, Owner.Params.DicManualSkillTarget);
                        NewWingmanBPilot.Level = 1;
                        NewWingmanBUnit.ArrayCharacterActive = new Character[1] { NewWingmanBPilot };
                    }

                    Squad NewSquad = new Squad("", NewLeaderUnit, NewWingmanAUnit, NewWingmanBUnit);

                    if (!Owner.ListPlayer[P].IsPlayerControlled)
                    {
                        NewSquad.SquadAI = new DeathmatchScripAIContainer(new DeathmatchAIInfo(Owner, NewSquad));
                        NewSquad.SquadAI.Load("SRWE Enemy AI");
                    }
                    else
                    {
                        NewSquad.IsPlayerControlled = true;
                    }

                    Owner.SpawnSquad(P, NewSquad, 0, new Vector2(Owner.ListPlayer[P].ListSpawnPoint[S].Position.X, Owner.ListPlayer[P].ListSpawnPoint[S].Position.Y), 0);
                }
            }
        }

        public void OnNewTurn(int ActivePlayerIndex)
        {
        }


        public void OnSquadDefeated(int AttackerSquadPlayerIndex, Squad AttackerSquad, int DefeatedSquadPlayerIndex, Squad DefeatedSquad)
        {
        }

        public void OnManualVictory()
        {
        }

        public void OnManualDefeat()
        {
        }

        public void Update(GameTime gameTime)
        {
            Owner.OnlinePlayers.Update();
        }

        public void BeginDraw(CustomSpriteBatch g)
        {

        }

        public void Draw(CustomSpriteBatch g)
        {

        }
    }
}
