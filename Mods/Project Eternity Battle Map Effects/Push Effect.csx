using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed class PushSquadEffect : BattleMapEffect
    {
        private string _PushBackwardValue;
        private string _PushLateralValue;

        public PushSquadEffect()
            : this(null)
        {
        }

        public PushSquadEffect(BattleMap Map)
            : base("Push Squad Effect", Map)
        {
            _PushBackwardValue = "0";
            _PushLateralValue = "0";
        }

        protected override void Load(BinaryReader BR)
        {
            _PushBackwardValue = BR.ReadString();
            _PushLateralValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_PushBackwardValue);
            BW.Write(_PushLateralValue);
        }

        protected override void DoExecuteSkillEffect(Squad TargetSquad, Unit TargetUnit, Character TargetCharacter)
        {
            IsUsed = true;
            Vector2 PositionDifference = new Vector2(OwnerSquad.Position.X - TargetSquad.Position.X, OwnerSquad.Position.Y - TargetSquad.Position.Y);
            PositionDifference.Normalize();

            int FinalPushBackwarValue = int.Parse(Map.Parser.Evaluate(_PushBackwardValue), CultureInfo.InvariantCulture);
            int FinalPushLateralValue = int.Parse(Map.Parser.Evaluate(_PushLateralValue), CultureInfo.InvariantCulture);
            Vector2 FinalBackwardPush = PositionDifference * FinalPushBackwarValue;
            Vector2 FinalLateralPush = PositionDifference * FinalPushLateralValue;
            FinalLateralPush = new Vector2(-FinalLateralPush.Y, FinalLateralPush.X);

            Point FinalPosition = MoveTowardPosition(TargetSquad, new Vector2(TargetSquad.Position.X + FinalBackwardPush.X + FinalLateralPush.X, TargetSquad.Y + FinalBackwardPush.Y + FinalLateralPush.Y));
            TargetSquad.SetPosition(new Vector2(FinalPosition.X, FinalPosition.Y), 0);
        }

        private Point MoveTowardPosition(Squad TargetSquad, Vector2 Destination)
        {
            int X0 = TargetSquad.X;
            int X1 = (int)Destination.X;
            int Y0 = TargetSquad.Y;
            int Y1 = (int)Destination.Y;

            bool Upward = Math.Abs(Y1 - Y0) > Math.Abs(X1 - X0);
            if (Upward)
            {
                int temp;
                temp = X0;
                X0 = Y0;
                Y0 = temp;

                temp = X1;
                X1 = Y1;
                Y1 = temp;
            }
            if (X0 > X1)
            {
                int temp;
                temp = X0;
                X0 = X1;
                X1 = temp;

                temp = Y0;
                Y0 = Y1;
                Y1 = temp;
            }

            int DeltaX = (X1 - X0);
            int DeltaY = Math.Abs(Y1 - Y0);
            int Error = (DeltaX / 2);
            int YStep = (Y0 < Y1 ? 1 : -1);
            int Y = Y0;
            Point CurrentPosition = TargetSquad.Position;
            Point LastPosition = TargetSquad.Position;

            for (int X = X0; X <= X1; ++X)
            {
                if (Upward)
                    CurrentPosition = new Point(Y, X);
                else
                    CurrentPosition = new Point(X, Y);

                if (ObstacleAtPosition(CurrentPosition.X, CurrentPosition.Y))
                    break;

                LastPosition = CurrentPosition;

                Error = Error - DeltaY;

                if (Error < 0)
                {
                    Y += YStep;
                    Error += DeltaX;
                }
            }
            return LastPosition;
        }

        private bool ObstacleAtPosition(int TargetPositionX, int TargetPositionY)
        {
            bool ObstacleFound = false;
            for (int P = 0; P < Map.ListPlayer.Count && !ObstacleFound; P++)
                ObstacleFound = Map.CheckForObstacleAtPosition(P, TargetPositionX, TargetPositionY);

            return ObstacleFound;
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string PushBackwardValue
        {
            get { return _PushBackwardValue; }
            set { _PushBackwardValue = value; }
        }

        [CategoryAttribute("Effect Attributes"),
        DescriptionAttribute(".")]
        public string PushLateralValue
        {
            get { return _PushLateralValue; }
            set { _PushLateralValue = value; }
        }

        protected override SkillEffect Copy()
        {
            PushSquadEffect NewSkillEffect = new PushSquadEffect(Map);
            NewSkillEffect._PushBackwardValue = _PushBackwardValue;
            NewSkillEffect._PushLateralValue = _PushLateralValue;

            NewSkillEffect.Lifetime = 0;
            NewSkillEffect.AffectedType = AffectedType;
            NewSkillEffect.IsStacking = IsStacking;
            NewSkillEffect.MaximumStack = MaximumStack;
            NewSkillEffect.LifetimeType = LifetimeType;
            NewSkillEffect.LifetimeTypeValue = LifetimeTypeValue;

            return NewSkillEffect;
        }
    }
}