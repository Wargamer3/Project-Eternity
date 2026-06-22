using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelSpawnCharacter : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "SpawnCharacter";

        private static List<PlayerCharacter> ListCharacter;

        public ActionPanelSpawnCharacter(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, true)
        {
            ListCharacter = new List<PlayerCharacter>();
        }

        public override void OnSelect()
        {
            string[] ArrayCharacterPath = Directory.GetFiles("Content/Life Sim/Characters/", "*.pec");
            foreach (string ActiveCharacterPath in ArrayCharacterPath)
            {
                string CharacterName = ActiveCharacterPath.Substring(0, ActiveCharacterPath.Length - 4).Substring(28);
                ListCharacter.Add(new PlayerCharacter(CharacterName, MapManager.Content, MapManager, Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo));
            }
            MenuHeight = (ListCharacter.Count) * PannelHeight + 6 * 2;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ActiveInputManager.InputConfirmPressed())
            {
                Vector3 SpawnPosition = Owner.InvisibleCharacterAsCursor.Position;
                SpawnPosition.X += Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.TileSize.X / 2;
                SpawnPosition.Z += Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.TileSize.Y / 2;
                SpawnPosition.Y += ListCharacter[ActionMenuCursor].SpriteMap.Height * 0.1f;

                Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.SpawnCharacter(new PlayerCharacter(ListCharacter[ActionMenuCursor].RelativePath, MapManager.Content, MapManager, Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo), 0, SpawnPosition);

                RemoveAllSubActionPanels();
            }
            else if (ActiveInputManager.InputUpPressed())
            {
                if (--ActionMenuCursor < 0)
                {
                    ActionMenuCursor = ListCharacter.Count - 1;
                }
            }
            if (ActiveInputManager.InputDownPressed())
            {
                if (++ActionMenuCursor >= ListCharacter.Count)
                {
                    ActionMenuCursor = 0;
                }
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
            return new ActionPanelSpawnCharacter(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw the action panel.
            int X = FinalMenuX;
            int Y = FinalMenuY;

            X += 20;
            GameScreen.DrawBox(g, new Vector2(X, Y), ActionMenuWidth, MenuHeight, Color.White);
            X += 10;
            Y += 14;

            //Draw the choices.
            foreach (PlayerCharacter ActiveCharacter in ListCharacter)
            {
                TextHelper.DrawText(g, ActiveCharacter.Name.ToString(), new Vector2(X, Y), Color.White);
                Y += PannelHeight;
            }

            Y = BaseMenuY;
            if (Y + MenuHeight >= Constants.Height)
                Y = Constants.Height - MenuHeight;
            //Draw the menu cursor.
            g.Draw(GameScreen.sprPixel, new Rectangle(X, 9 + Y + ActionMenuCursor * PannelHeight, ActionMenuWidth - 20, PannelHeight - 5), Color.FromNonPremultiplied(255, 255, 255, 200));
        }
    }
}
