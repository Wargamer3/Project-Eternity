using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchUnit : Unit
    {
        protected DeathmatchMap Map;

        protected DeathmatchUnit(DeathmatchMap Map)
        {
            this.Map = Map;
        }

        protected DeathmatchUnit(string Name, DeathmatchMap Map)
            : base(Name)
        {
            this.Map = Map;
        }
    }
}
