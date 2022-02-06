using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptMoveSquad : DeathmatchMapScript
        {
            private uint _UnitToMoveID;
            private Point _TargetEndPosition;
            private float _MovementSpeed = 0.1f;
            private bool HasMovementFocus;

            public ScriptMoveSquad()
                : this(null)
            {
                _UnitToMoveID = 0;
                _TargetEndPosition = new Point();
                IsEnded = false;
                IsActive = false;
            }

            public ScriptMoveSquad(DeathmatchMap Map)
                : base(Map, 100, 50, "Move Squad", new string[] { "Move" }, new string[] { "Squad Moved" })
            {
                _UnitToMoveID = 0;
                _TargetEndPosition = new Point();
                IsEnded = false;
                IsActive = false;
            }

            public override void ExecuteTrigger(int Index)
            {
                if (!IsActive && !IsEnded)
                {
                    Squad MovingSquad = null;

                    for (int P = 0; P < Map.ListPlayer.Count && MovingSquad == null; P++)
                    {
                        for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count && MovingSquad == null; U++)
                        {
                            if (Map.ListPlayer[P].ListSquad[U].ID != _UnitToMoveID || Map.ListPlayer[P].ListSquad[U].CurrentLeader.HP <= 0)
                                continue;

                            MovingSquad = Map.ListPlayer[P].ListSquad[U];
                        }
                    }

                    if (MovingSquad != null)
                    {
                        int TargetX = _TargetEndPosition.X;
                        int TargetY = _TargetEndPosition.Y;
                        Microsoft.Xna.Framework.Vector3 FinalPosition;

                        Map.GetEmptyPosition(new Microsoft.Xna.Framework.Vector3(TargetX, TargetY, 0), out FinalPosition);

                        if (Map.MovementAnimation.Count == 1)
                            HasMovementFocus = true;
                        else
                            HasMovementFocus = false;

                        Map.MovementAnimation.Add(MovingSquad, MovingSquad.Position, FinalPosition);
                        MovingSquad.SetPosition(FinalPosition);
                    }

                    IsActive = true;
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (HasMovementFocus)
                    Map.MovementAnimation.MoveSquad(Map);

                if (Map.MovementAnimation.Count == 0)
                {
                    ExecuteEvent(this, 0);
                    IsEnded = true;
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                UnitToMoveID = BR.ReadUInt32();
                TargetEndPosition = new Point(BR.ReadInt32(), BR.ReadInt32());
                MovementSpeed = BR.ReadSingle();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(UnitToMoveID);
                BW.Write(TargetEndPosition.X);
                BW.Write(TargetEndPosition.Y);
                BW.Write(MovementSpeed);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptMoveSquad(Map);
            }

            #region Properties

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute("The Identification number of the targeted Unit.")]
            public UInt32 UnitToMoveID
            {
                get
                {
                    return _UnitToMoveID;
                }
                set
                {
                    _UnitToMoveID = value;
                }
            }

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute(".")]
            public Point TargetEndPosition
            {
                get
                {
                    return _TargetEndPosition;
                }
                set
                {
                    _TargetEndPosition = value;
                }
            }

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute(".")]
            public float MovementSpeed
            {
                get
                {
                    return _MovementSpeed;
                }
                set
                {
                    _MovementSpeed = value;
                }
            }

            #endregion
        }
    }
}
