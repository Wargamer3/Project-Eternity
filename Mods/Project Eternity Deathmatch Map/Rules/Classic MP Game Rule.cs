using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ClassicGameInfo : GameModeInfo
    {
        public const string ModeName = "Classic";

        private int _MaximumResapwn;
        private int MaximumUnitPriceAllowed;

        public ClassicGameInfo()
            : base(ModeName, null, CategoryPVP, true, null)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override void Load(BinaryReader BR)
        {
        }

        public override IGameRule GetRule(BattleMap Map)
        {
            return new ClassicMPGameRule((DeathmatchMap)Map, this);
        }

        public override GameModeInfo Copy()
        {
            return new ClassicGameInfo();
        }

        [DisplayNameAttribute("Maximum Resapwn"),
        CategoryAttribute("Respawn"),
        DescriptionAttribute("How many points are allowed to respawn."),
        DefaultValueAttribute(3)]
        public int MaximumResapwn
        {
            get
            {
                return _MaximumResapwn;
            }
            set
            {
                _MaximumResapwn = value;
            }
        }
    }

    class ClassicMPGameRule : IGameRule
    {
        private readonly DeathmatchMap Owner;
        private readonly ClassicGameInfo GameInfo;

        public string Name => GameInfo.Name;

        public ClassicMPGameRule(DeathmatchMap Owner, ClassicGameInfo GameInfo)
        {
            this.Owner = Owner;
            this.GameInfo = GameInfo;
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

        public int GetRemainingResapwn(int PlayerIndex)
        {
            throw new NotImplementedException();
        }

        public void OnTurnEnd(int ActivePlayerIndex)
        {
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
            Owner.OnlinePlayers.Update();
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
