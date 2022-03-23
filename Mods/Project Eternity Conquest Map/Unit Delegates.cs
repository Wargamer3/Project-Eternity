using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    partial class ConquestMap
    {
        /// <summary>
        /// Try to attack with Weapon 1.
        /// </summary>
        /// <returns>Returns true if success.</returns>
        public bool AIAttackWithWeapon1(UnitConquest ActiveUnit)
        {
            int PosX = (int)ActiveUnit.X;
            int PosY = (int)ActiveUnit.Y;
            Attack ActiveWeapon = ActiveUnit.ListAttack[0];
            List<Tuple<int, int>> ListDefendingSquad = CanSquadAttackWeapon1(PosX, PosY, ActivePlayerIndex, ActiveUnit.RelativePath, ActiveUnit.ListAttack[1]);

            if (!ActiveUnit.CanMove && !ActiveWeapon.IsPostMovement(ActiveUnit))
                return false;

            //Define the minimum and maximum value of the attack range.
            int MinRange = ActiveWeapon.RangeMinimum;
            int MaxRange = ActiveWeapon.RangeMaximum;
            float Distance;

            if (MaxRange > 1)
                MaxRange += ActiveUnit.Boosts.RangeModifier;

            #region Can attack

            if (ListDefendingSquad.Count > 0)
            {
                UnitConquest TargetSquad = ListPlayer[ListDefendingSquad[0].Item1].ListUnit[ListDefendingSquad[0].Item2];

                //Prepare the Cursor to move.
                CursorPosition.X = ActiveUnit.X;
                CursorPosition.Y = ActiveUnit.Y;
                GetAttackChoice(ActiveUnit, ActiveUnit.Position);
                ActiveUnit.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;

                GetAttackDamageWithWeapon1(ActiveUnit, TargetSquad, ActiveUnit.HP);
                return true;//Exit the weapon loop.
            }

            #endregion

            #region Can't attack directly

            //If it's a post-movement weapon or you can still move.
            else if (ActiveUnit.CanMove && ActiveWeapon.IsPostMovement(ActiveUnit))
            {//check if there is an enemy too close to be attacked but that could be attacked after moving.
                for (int P = 0; P < ListPlayer.Count; P++)
                {
                    //If the player is from the same team as the current player or is dead, skip it.
                    if (ListPlayer[P].Team == ListPlayer[ActivePlayerIndex].Team
                        || !ListPlayer[P].IsAlive)
                        continue;
                    for (int TargetSelect = 0; TargetSelect < ListPlayer[P].ListUnit.Count; TargetSelect++)
                    {
                        UnitConquest TargetSquad = ListPlayer[P].ListUnit[TargetSelect];
                        Distance = Math.Abs(PosX - TargetSquad.X) + Math.Abs(PosY - TargetSquad.Y);
                        //Check if you can attack it if you moved.
                        if (Distance >= MinRange - GetSquadMaxMovement(ActiveUnit) && Distance <= MaxRange + GetSquadMaxMovement(ActiveUnit))
                            ListDefendingSquad.Add(new Tuple<int, int>(P, TargetSelect));
                    }
                }
                //If something was found.
                if (ListDefendingSquad.Count > 0)
                {
                    int RandomNumber = RandomHelper.Next(ListDefendingSquad.Count);
                    //Select a target.
                    UnitConquest TargetSquad = ListPlayer[ListDefendingSquad[RandomNumber].Item1].ListUnit[ListDefendingSquad[RandomNumber].Item2];
                    float DistanceUnit = Math.Abs(PosX - TargetSquad.X) + Math.Abs(PosY - TargetSquad.Y);
                    //Move to be in range.
                    List<MovementAlgorithmTile> ListRealChoice = new List<MovementAlgorithmTile>(GetMVChoice(ActiveUnit));
                    for (int M = 0; M < ListRealChoice.Count; M++)
                    {//Remove every MV that would make it impossible to attack.
                        Distance = Math.Abs(ListRealChoice[M].WorldPosition.X - TargetSquad.X) + Math.Abs(ListRealChoice[M].WorldPosition.Y - TargetSquad.Y);
                        //Remove every MV that would bring the Unit too close to use its weapon.
                        if (DistanceUnit <= MinRange)
                        {
                            if (Distance <= MinRange)
                                ListRealChoice.RemoveAt(M--);
                        }
                        //Check if you can attack it if you moved.
                        else if (Distance < MinRange || Distance > MaxRange)
                            ListRealChoice.RemoveAt(M--);
                    }
                    //Must find a spot to move if got there, just to make sure it won't crash in case of logic error.
                    if (ListRealChoice.Count != 0)
                    {
                        int Choice = RandomHelper.Next(ListRealChoice.Count);

                        //Movement initialisation.
                        MovementAnimation.Add(ActiveUnit.Components, ActiveUnit.Components.Position, ListRealChoice[Choice].WorldPosition);

                        //Prepare the Cursor to move.
                        CursorPosition.X = ListRealChoice[Choice].WorldPosition.X;
                        CursorPosition.Y = ListRealChoice[Choice].WorldPosition.Y;
                        CursorPosition.Z = ListRealChoice[Choice].WorldPosition.Z;
                        ActiveUnit.SetPosition(CursorPosition);
                        ActiveUnit.SetPosition(ListRealChoice[Choice].WorldPosition);

                        FinalizeMovement(ActiveUnit);
                    }
                    else
                    {
                        //Something is blocking the path.
                        ActiveUnit.EndTurn();
                    }
                    //Unit should be in attack range next time the AI is called.
                    return true;//Exit the weapon loop.
                }
            }

            #endregion

            return false;//Can't attack at all.
        }

        /// <summary>
        /// Try to attack with Weapon 2.
        /// </summary>
        /// <returns>Returns true if success.</returns>
        public bool AIAttackWithWeapon2(UnitConquest ActiveUnit)
        {
            int PosX = (int)ActiveUnit.X;
            int PosY = (int)ActiveUnit.Y;
            Attack ActiveWeapon = ActiveUnit.ListAttack[1];
            List<Tuple<int, int>> ListDefendingSquad = CanSquadAttackWeapon2(PosX, PosY, ActivePlayerIndex, ActiveUnit.RelativePath, ActiveUnit.ListAttack[1]);
            if (ListDefendingSquad == null)
                return false;

            if (!ActiveUnit.CanMove && !ActiveWeapon.IsPostMovement(ActiveUnit))
                return false;

            //Define the minimum and maximum value of the attack range.
            int MinRange = ActiveWeapon.RangeMinimum;
            int MaxRange = ActiveWeapon.RangeMaximum;
            float Distance;

            if (MaxRange > 1)
                MaxRange += ActiveUnit.Boosts.RangeModifier;

            #region Can attack

            if (ListDefendingSquad.Count > 0)
            {
                UnitConquest TargetSquad = ListPlayer[ListDefendingSquad[0].Item1].ListUnit[ListDefendingSquad[0].Item2];

                //Prepare the Cursor to move.
                CursorPosition.X = ActiveUnit.X;
                CursorPosition.Y = ActiveUnit.Y;
                GetAttackChoice(ActiveUnit, ActiveUnit.Position);

                ActiveUnit.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;

                GetAttackDamageWithWeapon2(ActiveUnit, TargetSquad, ActiveUnit.HP);
                return true;//Exit the weapon loop.
            }

            #endregion

            #region Can't attack directly

            //If it's a post-movement weapon or you can still move.
            else if (ActiveUnit.CanMove && ActiveWeapon.IsPostMovement(ActiveUnit))
            {//check if there is an enemy too close to be attacked but that could be attacked after moving.
                for (int P = 0; P < ListPlayer.Count; P++)
                {
                    //If the player is from the same team as the current player or is dead, skip it.
                    if (ListPlayer[P].Team == ListPlayer[ActivePlayerIndex].Team
                        || !ListPlayer[P].IsAlive)
                        continue;
                    for (int TargetSelect = 0; TargetSelect < ListPlayer[P].ListUnit.Count; TargetSelect++)
                    {
                        UnitConquest TargetSquad = ListPlayer[P].ListUnit[TargetSelect];
                        Distance = Math.Abs(PosX - TargetSquad.X) + Math.Abs(PosY - TargetSquad.Y);
                        //Check if you can attack it if you moved.
                        if (Distance >= MinRange - GetSquadMaxMovement(ActiveUnit) && Distance <= MaxRange + GetSquadMaxMovement(ActiveUnit))
                            ListDefendingSquad.Add(new Tuple<int, int>(P, TargetSelect));
                    }
                }
                //If something was found.
                if (ListDefendingSquad.Count > 0)
                {
                    int RandomNumber = RandomHelper.Next(ListDefendingSquad.Count);
                    //Select a target.
                    UnitConquest TargetSquad = ListPlayer[ListDefendingSquad[RandomNumber].Item1].ListUnit[ListDefendingSquad[RandomNumber].Item2];
                    float DistanceUnit = Math.Abs(PosX - TargetSquad.X) + Math.Abs(PosY - TargetSquad.Y);
                    //Move to be in range.
                    List<MovementAlgorithmTile> ListRealChoice = new List<MovementAlgorithmTile>(GetMVChoice(ActiveUnit));
                    for (int M = 0; M < ListRealChoice.Count; M++)
                    {//Remove every MV that would make it impossible to attack.
                        Distance = Math.Abs(ListRealChoice[M].WorldPosition.X - TargetSquad.X) + Math.Abs(ListRealChoice[M].WorldPosition.Y - TargetSquad.Y);
                        //Remove every MV that would bring the Unit too close to use its weapon.
                        if (DistanceUnit <= MinRange)
                        {
                            if (Distance <= MinRange)
                                ListRealChoice.RemoveAt(M--);
                        }
                        //Check if you can attack it if you moved.
                        else if (Distance < MinRange || Distance > MaxRange)
                            ListRealChoice.RemoveAt(M--);
                    }
                    //Must find a spot to move if got there, just to make sure it won't crash in case of logic error.
                    if (ListRealChoice.Count != 0)
                    {
                        int Choice = RandomHelper.Next(ListRealChoice.Count);

                        //Movement initialisation.
                        MovementAnimation.Add(ActiveUnit.Components, ActiveUnit.Components.Position, ListRealChoice[Choice].WorldPosition);

                        //Prepare the Cursor to move.
                        CursorPosition.X = ListRealChoice[Choice].WorldPosition.X;
                        CursorPosition.Y = ListRealChoice[Choice].WorldPosition.Y;
                        CursorPosition.Z = ListRealChoice[Choice].WorldPosition.Z;
                        ActiveUnit.SetPosition(ListRealChoice[Choice].WorldPosition);

                        FinalizeMovement(ActiveUnit);
                    }
                    else
                    {
                        //Something is blocking the path.
                        ActiveUnit.EndTurn();
                    }
                    //Unit should be in attack range next time the AI is called.
                    return true;//Exit the weapon loop.
                }
            }

            #endregion

            return false;//Can't attack at all.
        }
    }
}
