using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PlayerOverseer
    {
        private readonly NavMapGameManager MapManager;

        public List<PlayerCharacter> ListControlledCharacter;
        private KnowledgeContainer Knowledge;
        public Camera3D Camera;

        public PlayerInput ActiveInputManager;
        public ActionPanelHolder ActionHolder;

        public PlayerCharacter ActiveControlledCharacter;
        public readonly PlayerCharacter InvisibleCharacterAsCursor;//Making the cursor is like moving a character to use the same map change code.

        public PlayerOverseer(NavMapGameManager MapManager, MapInfo CurrentMapInfo)
        {
            this.MapManager = MapManager;

            ListControlledCharacter = new List<PlayerCharacter>();
            ActiveInputManager = new KeyboardInput();

            InvisibleCharacterAsCursor = new PlayerCharacter(MapManager, CurrentMapInfo, Vector3.Zero);

            Knowledge = new KnowledgeContainer();

            ActionHolder = new ActionPanelHolder();

            ActionHolder.AddToPanelListAndSelect(new ActionPanelFreeCam(this, MapManager, ActionHolder));
        }

        public void ControlCharacter(PlayerCharacter CharacterToControl)
        {
            ListControlledCharacter.Add(CharacterToControl);
            ActionHolder.AddToPanelListAndSelect(new ActionPanelControlCharacter(this, CharacterToControl, MapManager, ActionHolder));
        }

        public void Update(GameTime gameTime)
        {
            if (ActionHolder.HasSubPanels)
            {
                ActionHolder.Last().Update(gameTime);
            }
            else if (ActionHolder.HasMainPanel)
            {
                ActionHolder.GetMainPanel().Update(gameTime);
            }
        }

        public void Draw(CustomSpriteBatch g)
        {
            if (ActionHolder.HasMainPanel)
            {
                ActionHolder.GetMainPanel().Draw(g);
            }
            if (ActionHolder.HasSubPanels)
            {
                ActionHolder.Last().Draw(g);
            }
        }
    }
}
