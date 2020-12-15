using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen.Online
{
    public sealed class BattleMapLobyScriptHolder : OnlineScriptHolder
    {
        public override KeyValuePair<string, List<OnlineScript>> GetNameAndContent(params object[] args)
        {
            List<OnlineScript> ListOnlineScript = ReflectionHelper.GetNestedTypes<OnlineScript>(typeof(BattleMapLobyScriptHolder), args);
            return new KeyValuePair<string, List<OnlineScript>>("Deathmatch Loby", ListOnlineScript);
        }

        public abstract class BattleMapScript : OnlineScript
        {
            protected readonly BattleMap Map;

            protected BattleMapScript(string Name, BattleMap Map)
                : base(Name)
            {
                this.Map = Map;
            }
        }

        public class SkipSquadMovementScript : BattleMapScript
        {
            public SkipSquadMovementScript()
                : this(null)
            {
            }

            public SkipSquadMovementScript(BattleMap Map)
                : base("Skip Squad Movement", Map)
            {
            }

            public override OnlineScript Copy()
            {
                return new SkipSquadMovementScript(Map);
            }

            protected override void Execute(IOnlineConnection Host)
            {
                for (int P = Map.MovementAnimation.Count - 1; P >= 0; --P)
                {
                    UnitMapComponent ActiveUnitMap = Map.MovementAnimation.ListMovingMapUnit[P];

                    Map.MovementAnimation.ListPosX[P] = ActiveUnitMap.X;
                    Map.MovementAnimation.ListPosY[P] = ActiveUnitMap.Y;
                }
            }

            protected override void Read(OnlineReader Sender)
            {
            }

            protected override void DoWrite(OnlineWriter WriteBuffer)
            {
            }
        }
    }
}
