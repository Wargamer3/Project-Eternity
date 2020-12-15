using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class BounceEffect : TripleThunderRobotEffect
    {
        public static string Name = "Bounce Robot";

        public BounceEffect()
            : base(Name, false)
        {
        }

        public BounceEffect(TripleThunderRobotParams Params)
            : base(Name, false, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            string MinHeight;
            RobotAnimation ActiveRobot = Params.LocalContext.Target;

            if (ActiveRobot.Speed.Y < ActiveRobot.GravityMax)
            {
                if (Params.LocalContext.Target.DicStoredVariable.TryGetValue("MinHeight", out MinHeight))
                {
                    float RealMinHeight = float.Parse(MinHeight);
                    float CurrentHeight = Params.LocalContext.Target.Position.Y;

                    if (CurrentHeight < RealMinHeight + 50)
                    {
                        if (ActiveRobot.Speed.Y < 0 && ActiveRobot.Speed.Y + ActiveRobot.Gravity > 0)
                        {
                            if (Params.LocalContext.Target.DicStoredVariable.ContainsKey("SpeedY"))
                            {
                                float ExpectedSpeedY = float.Parse(Params.LocalContext.Target.DicStoredVariable["SpeedY"]);
                                Params.LocalContext.Target.DicStoredVariable["SpeedY"] = (-ExpectedSpeedY).ToString();
                            }
                        }

                        ActiveRobot.Speed.Y += ActiveRobot.Gravity;
                    }
                    else
                    {
                        if (Params.LocalContext.Target.DicStoredVariable.ContainsKey("SpeedX"))
                        {
                            float ExpectedSpeedX = float.Parse(Params.LocalContext.Target.DicStoredVariable["SpeedX"]);

                            if (ActiveRobot.Speed.X < ExpectedSpeedX)
                            {
                                ActiveRobot.Speed.X += 0.05f;

                                if (ActiveRobot.Speed.X >= ExpectedSpeedX)
                                {
                                    ActiveRobot.Speed.X = ExpectedSpeedX;
                                }
                            }
                            else if (ActiveRobot.Speed.X > ExpectedSpeedX)
                            {
                                ActiveRobot.Speed.X -= 0.05f;

                                if (ActiveRobot.Speed.X <= ExpectedSpeedX)
                                {
                                    ActiveRobot.Speed.X = ExpectedSpeedX;
                                }
                            }
                        }
                        if (Params.LocalContext.Target.DicStoredVariable.ContainsKey("SpeedY"))
                        {
                            float ExpectedSpeedY = float.Parse(Params.LocalContext.Target.DicStoredVariable["SpeedY"]);

                            if (ActiveRobot.Speed.Y < ExpectedSpeedY)
                            {
                                ActiveRobot.Speed.Y += 0.05f;

                                if (ActiveRobot.Speed.Y >= ExpectedSpeedY)
                                {
                                    ActiveRobot.Speed.Y = ExpectedSpeedY;
                                }
                            }
                            else if (ActiveRobot.Speed.Y > ExpectedSpeedY)
                            {
                                ActiveRobot.Speed.Y -= 0.05f;

                                if (ActiveRobot.Speed.Y <= ExpectedSpeedY)
                                {
                                    ActiveRobot.Speed.Y = ExpectedSpeedY;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            BounceEffect NewEffect = new BounceEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        #region Properties

        #endregion
    }
}
