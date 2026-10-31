using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimSalamanderTests
    {
        [TestMethod]
        public void ASalamanderUsesTheBestiaryBlock()
        {
            PlayerCharacter Who = SalamanderField.Make(1);

            Assert.AreEqual("Salamander", Who.Name);
            Assert.AreEqual(SkirmishSide.Monster, Who.Side);
            Assert.AreEqual(SalamanderField.Level, Who.Level);
            Assert.AreEqual(4, Who.STR);
            Assert.AreEqual(3, Who.DEX);
            Assert.AreEqual(4, Who.CON);
            Assert.AreEqual(2, Who.INT);
            Assert.AreEqual(2, Who.WIS);
            Assert.AreEqual(1, Who.CHA);
            Assert.AreEqual(SalamanderField.HitPoints, Who.MaxHP);
            Assert.AreEqual(SalamanderField.HitPoints, Who.CurrentHP);
            Assert.AreEqual(SalamanderField.ArmorClass, Who.AC);
            Assert.IsTrue(Who.PrintedArmor);
            Assert.AreEqual(15, Who.Fortitude);
            Assert.AreEqual(16, Who.Reflex);
            Assert.AreEqual(13, Who.Will);
            Assert.AreEqual(15, Who.Perception);
            Assert.AreEqual(17, Who.Athletics);
            Assert.AreEqual(20, Who.SpeedFeet);
            Assert.AreEqual("Fire", Who.CreatureTrait);
            Assert.AreEqual(SalamanderField.FireImmunity, Who.ResistanceFire);
            Assert.AreEqual("+1 Ranseur", Who.Weapon.Name);
            Assert.AreEqual("2d10+3", Who.Weapon.Damage);
            Assert.AreEqual("Tail", Who.OtherWeapon.Name);
            Assert.AreEqual("1d8+3", Who.OtherWeapon.Damage);
            Assert.AreEqual(17, PracticeMath.AttackBonus(Who, Who.Weapon));
            Assert.AreEqual(17, PracticeMath.AttackBonus(Who, Who.OtherWeapon));
        }

        [TestMethod]
        public void InitDoesNotEraseTheBlock()
        {
            PlayerCharacter Who = SalamanderField.Make(2);
            Who.Init();
            SalamanderField.Finish(Who);

            Assert.AreEqual(SalamanderField.HitPoints, Who.MaxHP);
            Assert.AreEqual(SalamanderField.ArmorClass, Who.AC);
            Assert.AreEqual(4, Who.STR);
            Assert.AreEqual(16, Who.Reflex);
            Assert.AreEqual(17, PracticeMath.AttackBonus(Who, Who.Weapon));
            Assert.IsFalse(Who.AIControlled);
        }
    }
}
