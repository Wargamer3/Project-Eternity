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
            Vector3 SpawnPosition = CursorGround.From(Owner.InvisibleCharacterAsCursor.Position);
            MapInfo Here = Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo;
            if (Here != null && Here.Heights != null)
            {
                SpawnPosition.Z = HeightField.Stand(Here, SpawnPosition.X, SpawnPosition.Y);
            }
            else
            {
                SpawnPosition.Z += 1f;
            }

            PlayerCharacter Spawned = new PlayerCharacter(CharacterToSpawn.RelativePath, MapManager.Content, MapManager, Here);
            LifeSimMap Map = Here == null || Here.MapMapContainer == null ? null : Here.MapMapContainer.ActiveMap as LifeSimMap;
            if (Map != null)
            {
                Map.SpawnCharacter(Spawned, 0, SpawnPosition);
            }
            else if (Here != null)
            {
                Spawned.SetPosition(SpawnPosition);
                Spawned.AIControlled = true;
                Spawned.Body.CanJump = true;
                Spawned.SeedBelief();
                Spawned.Init();
                Here.Adopt(Spawned);
            }

            TownLife.FileStanding(Spawned);

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
