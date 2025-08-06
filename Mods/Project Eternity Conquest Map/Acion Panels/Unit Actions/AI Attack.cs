using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelAIAttackBehavior : ActionPanelConquest
    {
        private const string PanelName = "AIAttack";

        private int ActivePlayerIndex;
        private int ActiveUnitIndex;
        private Tuple<int, int> Target;
        private UnitConquest ActiveUnit;

        public ActionPanelAIAttackBehavior(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAIAttackBehavior(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
        }

        public ActionPanelAIAttackBehavior(ConquestMap Map, int ActivePlayerIndex, int ActiveSquadIndex, Tuple<int, int> Target)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveSquadIndex;
            this.Target = Target;

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveSquadIndex];
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            // Can Move, try capturing a building.
            if (ActiveUnit.CanMove)
            {
                List<MovementAlgorithmTile> ListMVChoice = Map.GetMVChoice(ActiveUnit, Map);

                for (int M = 0; M < ListMVChoice.Count; M++)
                {
                    TerrainConquest ActiveTerrain = Map.GetTerrain((int)ListMVChoice[M].WorldPosition.X, (int)ListMVChoice[M].WorldPosition.Y, (int)ListMVChoice[M].WorldPosition.Z);

                    //Check if the Terrain is a building.
                    if (ActiveTerrain.CapturedPlayerIndex != Map.ActivePlayerIndex && ActiveTerrain.TerrainTypeIndex >= 13)
                    {
                        //Movement initialisation.
                        Map.MovementAnimation.Add(ActiveUnit.Components, ActiveUnit.Components.Position, ListMVChoice[M].WorldPosition);
                        //Prepare the Cursor to move.
                        Map.CursorPosition.X = ListMVChoice[M].WorldPosition.X;
                        Map.CursorPosition.Y = ListMVChoice[M].WorldPosition.Y;
                        Map.CursorPosition.Z = ListMVChoice[M].WorldPosition.Z;
                        //Move the Unit to the target position;
                        ActiveUnit.SetPosition(ListMVChoice[M].WorldPosition);

                        Map.FinalizeMovement(ActiveUnit, 0, Map.GetPathToTerrain(ActiveTerrain, ActiveUnit.Position));
                        return;
                    }
                }
            }
            //If it didn't attacked yet.
            if (ActiveUnit.CanMove)
            {
                TerrainConquest ActiveTerrain = Map.GetTerrain(ActiveUnit.Components);

                // Can't move and on a building not owned by the current Player, try to capture it
                if (!ActiveUnit.CanMove && ActiveTerrain.CapturedPlayerIndex != Map.ActivePlayerIndex && ActiveTerrain.TerrainTypeIndex >= 13)
                {
                    ActiveTerrain.CapturePoints = Math.Max(0, ActiveTerrain.CapturePoints - ActiveUnit.HP);
                    if (ActiveTerrain.CapturePoints == 0)
                        ActiveTerrain.CapturedPlayerIndex = Map.ActivePlayerIndex;

                    ActiveUnit.EndTurn();
                }

                if (ActiveUnit.X < Map.Camera2DPosition.X || ActiveUnit.Y < Map.Camera2DPosition.Y ||
                    ActiveUnit.X >= Map.Camera2DPosition.X + Map.ScreenSize.X || ActiveUnit.Y >= Map.Camera2DPosition.Y + Map.ScreenSize.Y)
                {
                    Map.PushScreen(new CenterOnSquadCutscene(null, Map, ActiveUnit.Position));
                }

                bool AttackSuccess = false;
                //Try to attack.
                AttackSuccess = Map.AIAttackWithWeapon1(ActiveUnit) || Map.AIAttackWithWeapon2(ActiveUnit);

                //All weapon are used, if he had to attack at this point it's already done.
                if (!AttackSuccess)
                    ActiveUnit.EndTurn();

                RemoveFromPanelList(this);
            }

            //If the Unit can't attack at all, move toward the nearest enemy.
            else if (ActiveUnit.CanMove)
            {
                AddToPanelListAndSelect(new ActionPanelAIMoveTowardEnemy(Map, ActiveUnit));
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveUnitIndex = BR.ReadInt32();
            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveUnitIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAIAttackBehavior(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
