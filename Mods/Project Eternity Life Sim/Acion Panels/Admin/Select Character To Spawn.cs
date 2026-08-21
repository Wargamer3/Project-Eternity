using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelSelectCharacterToSpawn : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "SelectCharacterToSpawn";

        private static List<PlayerCharacter> ListCharacter;

        public ActionPanelSelectCharacterToSpawn(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, true)
        {
            ListCharacter = new List<PlayerCharacter>();
        }

        public override void OnSelect()
        {
            if (ListCharacter.Count == 0)
            {
                string[] ArrayCharacterPath = Directory.GetFiles("Content/Life Sim/Characters/", "*.pec");
                foreach (string ActiveCharacterPath in ArrayCharacterPath)
                {
                    string CharacterName = ActiveCharacterPath.Substring(0, ActiveCharacterPath.Length - 4).Substring(28);
                    ListCharacter.Add(new PlayerCharacter(CharacterName, MapManager.Content, MapManager, Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo));
                }
            }
            foreach (PlayerCharacter ActiveCharacter in ListCharacter)
            {
                AddChoiceToCurrentPanel(new ActionPanelSpawnCharacter(Owner, MapManager, ListActionMenuChoice, ActiveCharacter));
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (NavigateThroughNextChoices(MapManager.sndSelection))
            {
            }
            else if (ConfirmNextChoices(MapManager.sndConfirm))
            {
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
            return new ActionPanelSelectCharacterToSpawn(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }
}
