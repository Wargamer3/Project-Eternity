using System.Reflection;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.UnitTests.SorcererStreetTests
{
    static class SorcererStreetTestHelper
    {
        public static CreatureCard CreateCreature(string Path, int MaxHP, int MaxST)
        {
            CreatureCard Card = new CreatureCard(MaxHP, MaxST);
            typeof(Card).GetField("Path").SetValue(Card, Path);
            PrepareCreature(Card);
            return Card;
        }

        public static void PrepareCreature(CreatureCard Card)
        {
            CardAbilities Abilities = Card.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.None);
            Abilities.ArrayElementAffinity = new CreatureCard.ElementalAffinity[] { CreatureCard.ElementalAffinity.Neutral };
        }

        public static void UseDeathmatchRule(SorcererStreetMap Map)
        {
            Map.GameRule = new DeathmatchGameInfo(true, null).GetRule(Map);
        }

        public static void GiveCharacter(Player ActivePlayer)
        {
            PlayerCharacter LoadedCharacter = new PlayerCharacter("Begins/Gantz", null, null, null, null, null);
            ActivePlayer.Inventory.Character = new PlayerCharacterInfo(LoadedCharacter);
        }
    }
}
