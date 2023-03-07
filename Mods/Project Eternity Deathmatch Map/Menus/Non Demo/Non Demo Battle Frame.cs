using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class NonDemoScreen
    {
        public class NonDemoBattleFrame
        {
            public class NonDemoBattleFrameSquad
            {
                public NonDemoBattleUnitFrame[] ArrayStance;
                public NonDemoBattleUnitFrame SupportStance;

                public NonDemoBattleFrameSquad()
                {
                    ArrayStance = new NonDemoBattleUnitFrame[0];
                }

                public void Update(GameTime gameTime)
                {
                    foreach (NonDemoBattleUnitFrame ActiveStance in ArrayStance)
                    {
                        ActiveStance.Update(gameTime);
                    }

                    if (SupportStance != null)
                    {
                        SupportStance.Update(gameTime);
                    }
                }

                public void OnEnd()
                {
                    foreach (NonDemoBattleUnitFrame ActiveStance in ArrayStance)
                    {
                        ActiveStance.OnEnd();
                    }

                    if (SupportStance != null)
                    {
                        SupportStance.OnEnd();
                    }
                }

                public void Draw(CustomSpriteBatch g, int NonDemoAnimationTimer)
                {
                    foreach (NonDemoBattleUnitFrame ActiveStance in ArrayStance)
                    {
                        ActiveStance.Draw(g, NonDemoAnimationTimer);
                    }

                    if (SupportStance != null)
                    {
                        SupportStance.Draw(g, NonDemoAnimationTimer);
                    }
                }

                public NonDemoBattleFrameSquad Copy()
                {
                    NonDemoBattleFrameSquad NewCopy = new NonDemoBattleFrameSquad();

                    NewCopy.ArrayStance = new NonDemoBattleUnitFrame[ArrayStance.Length];
                    for (int i = 0; i < ArrayStance.Length; ++i)
                    {
                        NewCopy.ArrayStance[i] = ArrayStance[i];
                    }
                    NewCopy.SupportStance = SupportStance;

                    return NewCopy;
                }

                public override string ToString()
                {
                    return ArrayStance[0].ToString();
                }
            }

            public static readonly float SwitchLength = 25f;

            public NonDemoBattleFrameSquad RightStance;
            public NonDemoBattleFrameSquad LeftStance;
            
            public int FrameLength;
            public SquadBattleResult Result;
            protected bool IsRight;
            protected DeathmatchMap Map;

            public NonDemoBattleFrame(int FrameLength, NonDemoBattleFrameSquad RightStance, NonDemoBattleFrameSquad LeftStance)
            {
                this.FrameLength = FrameLength;
                this.RightStance = RightStance;
                this.LeftStance = LeftStance;
            }

            public void Update(GameTime gameTime)
            {
                RightStance.Update(gameTime);
                LeftStance.Update(gameTime);
            }

            public void OnEnd()
            {
                RightStance.OnEnd();
                LeftStance.OnEnd();
            }

            public void Draw(CustomSpriteBatch g, int NonDemoAnimationTimer)
            {
                RightStance.Draw(g, NonDemoAnimationTimer);
                LeftStance.Draw(g, NonDemoAnimationTimer);
            }

            public override string ToString()
            {
                return LeftStance.ToString() + " - " + RightStance.ToString();
            }
        }
    }
}
