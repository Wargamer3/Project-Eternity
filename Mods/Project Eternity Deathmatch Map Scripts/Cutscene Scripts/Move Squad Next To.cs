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
        public class ScriptMoveSquadNextTo : DeathmatchMapScript
        {
            private uint _UnitToMoveID;
            private uint _UnitToMoveNextToID;
            private Point _RelativeMovement;
            private float _MovementSpeed;
            private bool HasMovementFocus;

            public ScriptMoveSquadNextTo()
                : this(null)
            {
                _UnitToMoveID = 0;
                _UnitToMoveNextToID = 0;
                _RelativeMovement = new Point();
                _MovementSpeed = 0.1f;
                IsEnded = false;
                IsActive = false;
            }

            public ScriptMoveSquadNextTo(DeathmatchMap Map)
                : base(Map, 100, 50, "Move Squad Next To", new string[] { "Move" }, new string[] { "Unit Moved" })
            {
                _UnitToMoveID = 0;
                _UnitToMoveNextToID = 0;
                _RelativeMovement = new Point();
                _MovementSpeed = 0.1f;
                IsEnded = false;
                IsActive = false;
            }

            public override void ExecuteTrigger(int Index)
            {
                if (!IsActive && !IsEnded)
                {
                    Squad MovingSquad = null;
                    Squad UnitToMoveNextTo = null;

                    for (int P = 0; P < Map.ListPlayer.Count && !IsActive; P++)
                    {
                        for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count && !IsActive; U++)
                        {
                            if (Map.ListPlayer[P].ListSquad[U].ID == _UnitToMoveID)
                            {
                                MovingSquad = Map.ListPlayer[P].ListSquad[U];
                                Map.MovementAnimation.Add(MovingSquad.X, MovingSquad.Y, MovingSquad);
                            }
                            else if (Map.ListPlayer[P].ListSquad[U].ID == _UnitToMoveNextToID)
                            {
                                UnitToMoveNextTo = Map.ListPlayer[P].ListSquad[U];
                            }
                        }
                    }

                    if (MovingSquad != null && UnitToMoveNextTo != null)
                    {
                        int TargetX = (int)UnitToMoveNextTo.X + _RelativeMovement.X;
                        int TargetY = (int)UnitToMoveNextTo.Y + _RelativeMovement.Y;

                        Microsoft.Xna.Framework.Vector3 FinalPosition;

                        Map.GetEmptyPosition(new Microsoft.Xna.Framework.Vector3(TargetX, TargetY, 0), out FinalPosition);

                        if (Map.MovementAnimation.Count == 0)
                            HasMovementFocus = true;
                        else
                            HasMovementFocus = false;

                        MovingSquad.SetPosition(FinalPosition);
                    }

                    IsActive = true;
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (HasMovementFocus)
                    Map.MoveSquad();

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
                UnitToMoveNextToID = BR.ReadUInt32();
                RelativeMovement = new Point(BR.ReadInt32(), BR.ReadInt32());
                MovementSpeed = BR.ReadSingle();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(UnitToMoveID);
                BW.Write(UnitToMoveNextToID);
                BW.Write(RelativeMovement.X);
                BW.Write(RelativeMovement.Y);
                BW.Write(MovementSpeed);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptMoveSquadNextTo(Map);
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
            DescriptionAttribute("The Identification number of the Unit to move next to.")]
            public UInt32 UnitToMoveNextToID
            {
                get
                {
                    return _UnitToMoveNextToID;
                }
                set
                {
                    _UnitToMoveNextToID = value;
                }
            }

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute(".")]
            public Point RelativeMovement
            {
                get
                {
                    return _RelativeMovement;
                }
                set
                {
                    _RelativeMovement = value;
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
