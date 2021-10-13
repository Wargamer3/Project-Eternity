using System;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen.Server
{
    public class AskChangeLoadoutScriptServer : OnlineScript
    {
        private struct SquadLoadout
        {
            public UnitLoadout[] ArrayUnit;

            public SquadLoadout(UnitLoadout[] ArrayUnit)
            {
                this.ArrayUnit = ArrayUnit;
            }
        }

        private struct UnitLoadout
        {
            public string UnitTypeName;
            public string RelativePath;

            public string[] ArrayCharacter;

            public UnitLoadout(string UnitTypeName, string RelativePath, string[] ArrayCharacter)
            {
                this.UnitTypeName = UnitTypeName;
                this.RelativePath = RelativePath;
                this.ArrayCharacter = ArrayCharacter;
            }
        }

        public const string ScriptName = "Ask Change Loadout";

        private readonly RoomInformations Owner;

        private byte LocalPlayerIndex;
        private SquadLoadout[] ArrayNewSquad;

        public AskChangeLoadoutScriptServer(RoomInformations Owner)
            : base(ScriptName)
        {
            this.Owner = Owner;
        }

        public override OnlineScript Copy()
        {
            return new AskChangeLoadoutScriptServer(Owner);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            foreach (BattleMapPlayer ActivePlayer in Owner.ListRoomPlayer)
            {
                if (ActivePlayer.ConnectionID == Sender.ID && ActivePlayer.LocalPlayerIndex == LocalPlayerIndex)
                {
                    ActivePlayer.ListSquadToSpawn.Clear();

                    foreach (SquadLoadout ActiveSquad in ArrayNewSquad)
                    {
                        Unit Leader = PlayerManager.DicUnitType[ActiveSquad.ArrayUnit[0].UnitTypeName].FromFile(ActiveSquad.ArrayUnit[0].RelativePath, null, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                        Leader.ArrayCharacterActive = new Core.Characters.Character[ActiveSquad.ArrayUnit[0].ArrayCharacter.Length];
                        
                        for (int C = 0; C < ActiveSquad.ArrayUnit[0].ArrayCharacter.Length; ++C)
                        {
                            Leader.ArrayCharacterActive[C] = new Core.Characters.Character(ActiveSquad.ArrayUnit[0].ArrayCharacter[C], null, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                            Leader.ArrayCharacterActive[C].Level = 1;
                        }

                        Unit WingmanA = null;
                        if (ActiveSquad.ArrayUnit.Length > 1)
                        {
                            WingmanA = PlayerManager.DicUnitType[ActiveSquad.ArrayUnit[1].UnitTypeName].FromFile(ActiveSquad.ArrayUnit[1].RelativePath, null, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                            WingmanA.ArrayCharacterActive = new Core.Characters.Character[ActiveSquad.ArrayUnit[1].ArrayCharacter.Length];

                            for (int C = 0; C < ActiveSquad.ArrayUnit[1].ArrayCharacter.Length; ++C)
                            {
                                WingmanA.ArrayCharacterActive[C] = new Core.Characters.Character(ActiveSquad.ArrayUnit[1].ArrayCharacter[C], null, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                            }
                        }

                        Unit WingmanB = null;
                        if (ActiveSquad.ArrayUnit.Length > 2)
                        {
                            WingmanB = PlayerManager.DicUnitType[ActiveSquad.ArrayUnit[2].UnitTypeName].FromFile(ActiveSquad.ArrayUnit[2].RelativePath, null, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                            WingmanB.ArrayCharacterActive = new Core.Characters.Character[ActiveSquad.ArrayUnit[2].ArrayCharacter.Length];

                            for (int C = 0; C < ActiveSquad.ArrayUnit[2].ArrayCharacter.Length; ++C)
                            {
                                WingmanB.ArrayCharacterActive[C] = new Core.Characters.Character(ActiveSquad.ArrayUnit[2].ArrayCharacter[C], null, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                            }
                        }

                        Squad NewSquad = new Squad("", Leader, WingmanA, WingmanB);
                        NewSquad.IsPlayerControlled = true;
                        ActivePlayer.ListSquadToSpawn.Add(NewSquad);
                    }
                    
                    for (int P = 0; P < Owner.ListOnlinePlayer.Count; P++)
                    {
                        IOnlineConnection ActiveOnlinePlayer = Owner.ListOnlinePlayer[P];

                        ActiveOnlinePlayer.Send(new ChangeLoadoutScriptServer(ActivePlayer));
                    }
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            LocalPlayerIndex = Sender.ReadByte();

            int ArraySquadLength = Sender.ReadInt32();
            ArrayNewSquad = new SquadLoadout[ArraySquadLength];
            for (int S = 0; S < ArraySquadLength; ++S)
            {
                int UnitsInSquad = Sender.ReadInt32();
                UnitLoadout[] ArrayNewUnit = new UnitLoadout[UnitsInSquad];
                for (int U = 0; U < UnitsInSquad; ++U)
                {
                    string UnitTypeName = Sender.ReadString();
                    string RelativePath = Sender.ReadString();

                    int ArrayCharacterLength = Sender.ReadInt32();
                    string[] ArrayNewCharacter = new string[UnitsInSquad];
                    for (int C = 0; C < ArrayCharacterLength; ++C)
                    {
                        ArrayNewCharacter[C] = Sender.ReadString();
                    }

                    ArrayNewUnit[S] = new UnitLoadout(UnitTypeName, RelativePath, ArrayNewCharacter);
                }

                ArrayNewSquad[S] = new SquadLoadout(ArrayNewUnit);
            }
        }
    }
}
