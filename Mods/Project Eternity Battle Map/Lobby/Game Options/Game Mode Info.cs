using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class GameModeInfo
    {
        public struct GameModeParameter
        {
            public string Name;
            public string Description;
            public string Category;
            public object Value;

            public GameModeParameter(string Name, string Description, string Category, object Value)
            {
                this.Name = Name;
                this.Description = Description;
                this.Category = Category;
                this.Value = Value;
            }
        }

        public const string CategoryPVE = "PVE";
        public const string CategoryPVP = "PVE";

        public string Name;
        public string Description;
        public string Category;
        public List<string> ListMapFolder;
        public bool IsUnlocked;
        public Texture2D sprPreview;
        public bool UseTeams;

        public GameModeInfo()
        {
            UseTeams = true;
        }

        public GameModeInfo(string Name, string Description, string Category, bool IsUnlocked, Texture2D sprPreview)
        {
            this.Name = Name;
            this.Description = Description;
            this.Category = Category;
            this.IsUnlocked = IsUnlocked;
            this.sprPreview = sprPreview;
            UseTeams = true;
            ListMapFolder = new List<string>();
            ListMapFolder.Add(Name);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(Name);
            DoSave(BW);
        }

        public void Write(OnlineWriter WriteBuffer)
        {
            WriteBuffer.AppendString(Name);
            DoWrite(WriteBuffer);
        }

        protected virtual void DoWrite(OnlineWriter WriteBuffer)
        {
        }

        protected virtual void DoSave(BinaryWriter BW)
        {
        }

        public virtual void Load(BinaryReader BR)
        {
        }

        public virtual void Read(OnlineReader ReadBuffer)
        {
        }

        public virtual IGameRule GetRule(BattleMap Map)
        {
            return null;
        }

        public virtual void OnBotChanged(RoomInformations Room)
        {
            if (Room.MaxNumberOfBots > Room.MaxNumberOfPlayer)
            {
                Room.MaxNumberOfBots = Room.MaxNumberOfPlayer;
            }

            while (Room.ListRoomBot.Count < Room.MaxNumberOfBots)
            {
                BattleMapPlayer NewPlayer = new BattleMapPlayer(PlayerManager.OnlinePlayerID, "Bot", OnlinePlayerBase.PlayerTypes.Player, false, 0, false, Color.Blue);
                NewPlayer.InitFirstTimeInventory();
                NewPlayer.FillLoadout(Room.MaxSquadsPerBot);
                Room.ListRoomBot.Add(NewPlayer);
            }

            while (Room.ListRoomBot.Count > Room.MaxNumberOfBots)
            {
                Room.ListRoomBot.RemoveAt(Room.ListRoomBot.Count - 1);
            }
        }

        public virtual GameModeInfo Copy()
        {
            return new GameModeInfo(Name, Description, Category, IsUnlocked, sprPreview);
        }

        public Dictionary<string, List<GameModeParameter>> GetDescriptionFromEnumValue()
        {
            Dictionary<string, List<GameModeParameter>> DicGameModeParametersByCategory = new Dictionary<string, List<GameModeParameter>>();

            foreach (System.Reflection.PropertyInfo ActiveProperty in this.GetType().GetProperties())
            {
                DisplayNameAttribute ActiveName = (DisplayNameAttribute)ActiveProperty.GetCustomAttributes(typeof(DisplayNameAttribute), false)[0];
                CategoryAttribute ActiveCategory = (CategoryAttribute)ActiveProperty.GetCustomAttributes(typeof(CategoryAttribute), false)[0];
                DescriptionAttribute ActiveDescription = (DescriptionAttribute)ActiveProperty.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
                object PropertyValue = ActiveProperty.GetValue(this);

                if (!DicGameModeParametersByCategory.ContainsKey(ActiveCategory.Category))
                {
                    DicGameModeParametersByCategory.Add(ActiveCategory.Category, new List<GameModeParameter>());
                }

                GameModeParameter NewGameOption = new GameModeParameter(ActiveName.DisplayName, ActiveCategory.Category, ActiveDescription.Description, PropertyValue);
                DicGameModeParametersByCategory[ActiveCategory.Category].Add(NewGameOption);
            }

            return DicGameModeParametersByCategory;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
