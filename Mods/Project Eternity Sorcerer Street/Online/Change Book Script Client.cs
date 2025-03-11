using System;
using System.Collections.Generic;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen.Online
{
    public class ChangeBookScriptClient : OnlineScript
    {
        public const string ScriptName = "Change Book";

        private readonly RoomInformations Owner;
        private readonly GamePreparationScreen MissionSelectScreen;

        private string ID;
        private string CharacterModelPath;
        private string BookName;
        private string BookModel;
        private List<Tuple<string, byte>> ListCard;

        public ChangeBookScriptClient(RoomInformations Owner, GamePreparationScreen MissionSelectScreen)
            : base(ScriptName)
        {
            this.Owner = Owner;
            this.MissionSelectScreen = MissionSelectScreen;
            ListCard = new List<Tuple<string, byte>>(50);
        }

        public override OnlineScript Copy()
        {
            return new ChangeBookScriptClient(Owner, MissionSelectScreen);
        }

        protected override void DoWrite(OnlineWriter WriteBuffer)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(IOnlineConnection Sender)
        {
            foreach (Player ActivePlayer in Owner.ListRoomPlayer)
            {
                if (ActivePlayer.ConnectionID == ID)
                {
                    ActivePlayer.Inventory.Character = new PlayerCharacterInfo(new PlayerCharacter(CharacterModelPath, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget));

                    CardBook NewActiveBook = new CardBook(BookName);
                    NewActiveBook.BookModel = BookModel;
                    ActivePlayer.Inventory.ActiveBook = NewActiveBook;
                    for (int C = 0; C < ListCard.Count; ++C)
                    {
                        Card LoadedCard = Card.LoadCard(ListCard[C].Item1, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                        NewActiveBook.AddCard(new CardInfo(LoadedCard, ListCard[C].Item2));
                    }
                }
            }
        }

        protected override void Read(OnlineReader Sender)
        {
            ID = Sender.ReadString();

            CharacterModelPath = Sender.ReadString();

            BookName = Sender.ReadString();
            BookModel = Sender.ReadString();

            int ListCardCount = Sender.ReadInt32();
            for (int C = 0; C < ListCardCount; ++C)
            {
                ListCard.Add(new Tuple<string, byte>(Sender.ReadString(), Sender.ReadByte()));
            }
        }
    }
}
