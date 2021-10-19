using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class NonDemoScreen
    {
        public class NonDemoBattleFrame
        {
            public struct NonDemoBattleFrameSquad
            {
                public NonDemoBattleUnitFrame LeaderStance;
                public NonDemoBattleUnitFrame WingmanAStance;
                public NonDemoBattleUnitFrame WingmanBStance;
                public NonDemoBattleUnitFrame SupportStance;

                public void Update(GameTime gameTime)
                {
                    LeaderStance.Update(gameTime);
                    if (WingmanAStance != null)
                    {
                        WingmanAStance.Update(gameTime);
                    }

                    if (WingmanBStance != null)
                    {
                        WingmanBStance.Update(gameTime);
                    }

                    if (SupportStance != null)
                    {
                        SupportStance.Update(gameTime);
                    }
                }

                public void OnEnd()
                {
                    LeaderStance.OnEnd();
                    if (WingmanAStance != null)
                    {
                        WingmanAStance.OnEnd();
                    }

                    if (WingmanBStance != null)
                    {
                        WingmanBStance.OnEnd();
                    }

                    if (SupportStance != null)
                    {
                        SupportStance.OnEnd();
                    }
                }

                public void Draw(CustomSpriteBatch g, int NonDemoAnimationTimer)
                {
                    LeaderStance.Draw(g, NonDemoAnimationTimer);
                    if (WingmanAStance != null)
                    {
                        WingmanAStance.Draw(g, NonDemoAnimationTimer);
                    }

                    if (WingmanBStance != null)
                    {
                        WingmanBStance.Draw(g, NonDemoAnimationTimer);
                    }

                    if (SupportStance != null)
                    {
                        SupportStance.Draw(g, NonDemoAnimationTimer);
                    }
                }

                public NonDemoBattleFrameSquad Copy()
                {
                    NonDemoBattleFrameSquad NewCopy = new NonDemoBattleFrameSquad();

                    NewCopy.LeaderStance = LeaderStance;
                    NewCopy.WingmanAStance = WingmanAStance;
                    NewCopy.WingmanBStance = WingmanBStance;
                    NewCopy.SupportStance = SupportStance;

                    return NewCopy;
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
        }
    }
}
