using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAIAttackBehavior : ActionPanelDeathmatch
    {
        private const string PanelName = "AI Attack Behavior";

        private Squad ActiveSquad;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private SupportSquadHolder ActiveSquadSupport;
        private Squad TargetSquad;
        private SupportSquadHolder TargetSquadSupport;
        private Tuple<int, int> Target;

        private int AITimer;
        private const int AITimerBase = 20;
        private const int AITimerBaseHalf = 10;
        private Vector2 AICursorNextPosition;
        public List<Vector3> ListAttackChoice;
        public List<MovementAlgorithmTile> ListAttackTerrain;

        public ActionPanelAIAttackBehavior(DeathmatchMap Map)
            : base(PanelName, Map)
        {
            ListAttackTerrain = new List<MovementAlgorithmTile>();
        }

        public ActionPanelAIAttackBehavior(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            ListAttackTerrain = new List<MovementAlgorithmTile>();
        }

        public ActionPanelAIAttackBehavior(DeathmatchMap Map,  int ActivePlayerIndex, int ActiveSquadIndex, Tuple<int, int> Target)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.Target = Target;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            ListAttackTerrain = new List<MovementAlgorithmTile>();
        }

        public override void OnSelect()
        {
            Map.CursorPosition = ActiveSquad.Position;
            Map.CursorPositionVisible = Map.CursorPosition;

            if (Target == null)
            {
                SelectTargetAndAttack();
                return;
            }

            TargetSquad = Map.ListPlayer[Target.Item1].ListSquad[Target.Item2];
            Map.TargetPlayerIndex = Target.Item1;

            AITimer = AITimerBase;
            PrepareToAttack();

            ActiveSquadSupport = new SupportSquadHolder();
            ActiveSquadSupport.PrepareAttackSupport(Map, ActivePlayerIndex, ActiveSquad, Target.Item1, Target.Item2);
            TargetSquadSupport = new SupportSquadHolder();
            TargetSquadSupport.PrepareDefenceSupport(Map, Target.Item1, TargetSquad);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
            if (AITimer >= 0)
            {
                AITimer--;
            }
            if (Map.CursorPosition.X != AICursorNextPosition.X || Map.CursorPosition.Y != AICursorNextPosition.Y)
            {
                if (AITimer == AITimerBaseHalf)
                {
                    if (Map.CursorPosition.Y < AICursorNextPosition.Y)
                        Map.CursorPosition.Y += 1;
                    else if (Map.CursorPosition.Y > AICursorNextPosition.Y)
                        Map.CursorPosition.Y -= 1;
                    else if (Map.CursorPosition.X < AICursorNextPosition.X)
                        Map.CursorPosition.X += 1;
                    else if (Map.CursorPosition.X > AICursorNextPosition.X)
                        Map.CursorPosition.X -= 1;
                    else
                        AITimer = -1;
                }
                else if (AITimer == 0)
                {
                    AITimer = AITimerBase;
                    if (Map.CursorPosition.X < AICursorNextPosition.X)
                        Map.CursorPosition.X += 1;
                    else if (Map.CursorPosition.X > AICursorNextPosition.X)
                        Map.CursorPosition.X -= 1;
                }
            }
            else
            {
                //End the unit turn.
                ActiveSquad.EndTurn();

                if (ActiveSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.MAP)
                {
                    ListAttackChoice.Clear();
                    for (int X = 0; X < ActiveSquad.CurrentLeader.CurrentAttack.MAPAttributes.ListChoice.Count; X++)
                    {
                        for (int Y = 0; Y < ActiveSquad.CurrentLeader.CurrentAttack.MAPAttributes.ListChoice[X].Count; Y++)
                        {
                            if (ActiveSquad.CurrentLeader.CurrentAttack.MAPAttributes.ListChoice[X][Y])
                            {
                                ListAttackChoice.Add(new Vector3(Map.CursorPosition.X + X - ActiveSquad.CurrentLeader.CurrentAttack.MAPAttributes.Width,
                                                       Map.CursorPosition.Y + Y - ActiveSquad.CurrentLeader.CurrentAttack.MAPAttributes.Height, Map.CursorPosition.Z));
                            }
                        }
                    }

                    if (ActiveSquad.CurrentLeader.CurrentAttack.MAPAttributes.Delay > 0)
                    {
                        Map.ListDelayedAttack.Add(new DelayedAttack(ActiveSquad.CurrentLeader.CurrentAttack, ActiveSquad, ActivePlayerIndex, ListAttackTerrain));
                        RemoveFromPanelList(this);
                    }
                    else
                    {
                        Stack<Tuple<int, int>> ListMAPAttackTarget = Map.GetEnemies(ActiveSquad.CurrentLeader.CurrentAttack.MAPAttributes.FriendlyFire, ListAttackTerrain);

                        if (ListMAPAttackTarget.Count > 0)
                        {
                            Map.Params.GlobalContext.ArrayAttackPosition = ListAttackTerrain.ToArray();

                            Map.AttackWithMAPAttack(ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack, new List<Vector3>(), ListMAPAttackTarget);

                            //Remove Ammo if needed.
                            if (ActiveSquad.CurrentLeader.CurrentAttack.MaxAmmo > 0)
                                ActiveSquad.CurrentLeader.CurrentAttack.ConsumeAmmo();
                        }
                        else
                        {
                            Map.ComputeTargetPlayerDefence(ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack, ActiveSquadSupport, new List<Vector3>(),
                                Target.Item1, Target.Item2, TargetSquadSupport);
                        }
                    }
                }
                else
                {
                    Map.ComputeTargetPlayerDefence(ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack, ActiveSquadSupport, new List<Vector3>(),
                        Target.Item1, Target.Item2, TargetSquadSupport);
                }
                AITimer = AITimerBase;
            }
        }

        private void SelectTargetAndAttack()
        {
            Map.UpdateAllAttacks(ActiveSquad.CurrentLeader, ActiveSquad.Position, Map.ListPlayer[ActivePlayerIndex].TeamIndex, false);
            IEnumerable<Attack> ListAttackOrderedByPower = ActiveSquad.CurrentLeader.ListAttack.OrderByDescending(Attack => Attack.GetPower(ActiveSquad.CurrentLeader, Map.Params.ActiveParser));
            foreach (Attack ActiveAttack in ListAttackOrderedByPower)
            {
                ActiveSquad.CurrentLeader.CurrentAttack = ActiveAttack;
                List<Tuple<int, int>> ListTargetUnit = new List<Tuple<int, int>>();

                for (int P = 0; P < Map.ListPlayer.Count; P++)
                {
                    if (Map.ListPlayer[P].TeamIndex == Map.ListPlayer[ActivePlayerIndex].TeamIndex)//Don't check your team.
                        continue;

                    for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count; U++)
                    {
                        if (Map.ListPlayer[P].ListSquad[U].CurrentLeader == null)
                            continue;

                        ActiveSquad.CurrentLeader.UpdateAllAttacks(ActiveSquad.Position, Map.ListPlayer[ActivePlayerIndex].TeamIndex, Map.ListPlayer[P].ListSquad[U].Position, Map.ListPlayer[P].TeamIndex,
                                Map.ListPlayer[P].ListSquad[U].ArrayMapSize, Map.ListPlayer[P].ListSquad[U].CurrentTerrainIndex, false);

                        if (ActiveAttack.CanAttack)
                        {
                            ListTargetUnit.Add(new Tuple<int, int>(P, U)); ;
                        }
                    }
                }
                if (ListTargetUnit.Count > 0)
                {
                    //Priority goes to units with higher chances of hitting.
                    IOrderedEnumerable<Tuple<int, int>> ListHitRate = ListTargetUnit.OrderByDescending(Target =>
                        Map.CalculateHitRate(ActiveSquad.CurrentLeader, ActiveAttack, ActiveSquad,
                        Map.ListPlayer[Target.Item1].ListSquad[Target.Item2].CurrentLeader, Map.ListPlayer[Target.Item1].ListSquad[Target.Item2],
                        Unit.BattleDefenseChoices.Attack));

                    this.Target = ListHitRate.First();
                }
                else
                {
                    RemoveFromPanelList(this);
                    return;
                }
            }

            TargetSquad = Map.ListPlayer[Target.Item1].ListSquad[Target.Item2];
            Map.TargetPlayerIndex = Target.Item1;

            AITimer = AITimerBase;
            PrepareToAttack();

            ActiveSquadSupport = new SupportSquadHolder();
            ActiveSquadSupport.PrepareAttackSupport(Map, ActivePlayerIndex, ActiveSquad, Target.Item1, Target.Item2);
            TargetSquadSupport = new SupportSquadHolder();
            TargetSquadSupport.PrepareDefenceSupport(Map, Target.Item1, TargetSquad);
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            Target = new Tuple<int, int>(BR.ReadInt32(), BR.ReadInt32());

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            TargetSquad = Map.ListPlayer[Target.Item1].ListSquad[Target.Item2];
            Map.TargetPlayerIndex = Target.Item1;

            ActiveSquadSupport = new SupportSquadHolder();
            ActiveSquadSupport.PrepareAttackSupport(Map, ActivePlayerIndex, ActiveSquad, Target.Item1, Target.Item2);
            TargetSquadSupport = new SupportSquadHolder();
            TargetSquadSupport.PrepareDefenceSupport(Map, Target.Item1, TargetSquad);
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);

            BW.AppendInt32(Target.Item1);
            BW.AppendInt32(Target.Item2);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAIAttackBehavior(Map);
        }

        private void PrepareToAttack()
        {
            ListAttackTerrain = Map.GetAttackChoice(ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack.RangeMaximum);
            ListAttackChoice = new List<Vector3>();
            foreach (MovementAlgorithmTile ActiveTerrain in ListAttackTerrain)
            {
                ListAttackChoice.Add(ActiveTerrain.WorldPosition);
            }
            Map.LayerManager.AddDrawablePoints(ListAttackTerrain, Color.FromNonPremultiplied(255, 0, 0, 190));
            Map.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;

            if (ActiveSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.ALL)
                Map.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            else
                Map.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;

            //Prepare the Battle Sumary menu.
            Map.BattleMenuCursorIndex = 0;

            //Show the Battle Sumary Menu.
            Map.BattleMenuStage = DeathmatchMap.BattleMenuStages.Default;

            //Prepare the Cursor to move.
            AICursorNextPosition.X = TargetSquad.X;
            AICursorNextPosition.Y = TargetSquad.Y;

            ActiveSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;

            for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 1; --U)
            {
                Unit ActiveWingman = ActiveSquad[U];

                ActiveWingman.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                if (ActiveWingman.PLAAttack != null)
                {
                    ActiveWingman.PLAAttack.UpdateAttack(ActiveWingman, ActiveSquad.Position, Map.ListPlayer[ActivePlayerIndex].TeamIndex, TargetSquad.Position, Map.ListPlayer[Map.TargetPlayerIndex].TeamIndex, TargetSquad.ArrayMapSize, TargetSquad.CurrentTerrainIndex, true);

                    if (ActiveWingman.PLAAttack.CanAttack)
                    {
                        ActiveWingman.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                    }
                }
            }

            //Simulate defense reaction.
            DeathmatchMap.PrepareDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack, Target.Item1, Target.Item2);
            //Compute the Attacker's accuracy and Wingmans reaction.
            DeathmatchMap.PrepareAttackSquadForBattle(Map, ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack, TargetSquad);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
