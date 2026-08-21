using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelSpawnCharacter : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "SelectCharacterToSpawn";

        private PlayerCharacter CharacterToSpawn;

        public ActionPanelSpawnCharacter(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, PlayerCharacter CharacterToSpawn)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, true)
        {
            this.CharacterToSpawn = CharacterToSpawn;
        }

        public override void OnSelect()
        {
            Vector3 SpawnPosition = new Vector3(Owner.InvisibleCharacterAsCursor.Position.X, Owner.InvisibleCharacterAsCursor.Position.Z, Owner.InvisibleCharacterAsCursor.Position.Y);
            SpawnPosition.X += Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.TileSize.X / 2;
            SpawnPosition.Y += Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.TileSize.Y / 2;
            SpawnPosition.Z += CharacterToSpawn.SpriteMap.Height * 0.1f;

            Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.SpawnCharacter(new PlayerCharacter(CharacterToSpawn.RelativePath, MapManager.Content, MapManager, Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo), 0, SpawnPosition);

            RemoveAllSubActionPanels();
        }

        public override void DoUpdate(GameTime gameTime)
        {
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
            return new ActionPanelSpawnCharacter(Owner, MapManager, ListActionMenuChoice, CharacterToSpawn);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override string ToString()
        {
            return CharacterToSpawn.Name;
        }
    }
}
