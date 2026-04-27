using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PlayerOverseer
    {
        private List<MapContainer> ListActiveMap;//Loaded, updated and visible

        public List<PlayerCharacter> ListControlledCharacter;
        private KnowledgeContainer Knowledge;

        private Vector3 CameraPosition;
        public ActionPanel ActiveActionPanel;

        public PlayerCharacter ActiveControlledCharacter;

        public PlayerOverseer()
        {
            ListActiveMap = new List<MapContainer>();

            ListControlledCharacter = new List<PlayerCharacter>();

            Knowledge = new KnowledgeContainer();
        }

        public void ControlCharacter(PlayerCharacter CharacterToControl)
        {
            ListControlledCharacter.Add(CharacterToControl);
        }

        public void Update(GameTime gameTime)
        {
            ActiveActionPanel.Update(gameTime);
        }

        public void Draw(CustomSpriteBatch g)
        {
            ActiveActionPanel.Draw(g);

            foreach (MapContainer ActiveMap in ListActiveMap)
            {
                ActiveMap.ActiveMap.Draw(g);
            }
        }
    }
}
