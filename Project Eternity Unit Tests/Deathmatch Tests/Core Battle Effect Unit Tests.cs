using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using ProjectEternity.Core.Units;

namespace ProjectEternity.UnitTests
{
    public partial class DeathmatchTests
    {
        [TestMethod]
        public void TestAutoDodgeEffect()
        {
            AutoDodgeEffect NewEffect = (AutoDodgeEffect)DummyMap.DicEffect[AutoDodgeEffect.Name].Copy();

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot);

            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.AutoDodgeModifier);

            DummySkill.AddSkillEffectsToTarget(string.Empty);

            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.AutoDodgeModifier);
        }

        [TestMethod]
        public void TestNullifyDamageEffect()
        {
            NullifyDamageEffect NewEffect = (NullifyDamageEffect)DummyMap.DicEffect[NullifyDamageEffect.Name].Copy();

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot);

            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.NullifyDamageModifier);

            DummySkill.AddSkillEffectsToTarget(string.Empty);

            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.NullifyDamageModifier);
        }

        [TestMethod]
        public void TestPostMovementEffect()
        {
            PostMovementEffect NewEffect = (PostMovementEffect)DummyMap.DicEffect[PostMovementEffect.Name].Copy();
            NewEffect.Attack = true;
            NewEffect.Transform = true;
            NewEffect.Spirit = true;
            NewEffect.Move = true;

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot);

            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostMovementModifier.Attack);
            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostMovementModifier.Transform);
            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostMovementModifier.Spirit);
            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostMovementModifier.Move);

            DummySkill.AddSkillEffectsToTarget(string.Empty);

            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostMovementModifier.Attack);
            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostMovementModifier.Transform);
            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostMovementModifier.Spirit);
            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostMovementModifier.Move);
        }

        [TestMethod]
        public void TestPostAttackEffect()
        {
            PostAttackEffect NewEffect = (PostAttackEffect)DummyMap.DicEffect[PostAttackEffect.Name].Copy();
            NewEffect.Attack = true;
            NewEffect.Transform = true;
            NewEffect.Spirit = true;
            NewEffect.Move = true;

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot);

            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostAttackModifier.Attack);
            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostAttackModifier.Transform);
            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostAttackModifier.Spirit);
            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostAttackModifier.Move);

            DummySkill.AddSkillEffectsToTarget(string.Empty);

            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostAttackModifier.Attack);
            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostAttackModifier.Transform);
            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostAttackModifier.Spirit);
            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.PostAttackModifier.Move);
        }

        [TestMethod]
        public void TestExtraMovementsPerTurnEffect()
        {
            ExtraMovementsPerTurnEffect NewEffect = (ExtraMovementsPerTurnEffect)DummyMap.DicEffect[ExtraMovementsPerTurnEffect.Name].Copy();
            NewEffect.ExtraMovementsPerTurn = 1;
            NewEffect.MaxExtraMovementsPerTurn = 5;

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot);

            Assert.AreEqual(0, GlobalDeathmatchContext.EffectOwnerUnit.Boosts.ExtraActionsPerTurn);

            DummySkill.AddSkillEffectsToTarget(string.Empty);

            Assert.AreEqual(1, GlobalDeathmatchContext.EffectOwnerUnit.Boosts.ExtraActionsPerTurn);
        }

        [TestMethod]
        public void TestActivateSpiritEffect()
        {
            ActivateSpiritEffect NewEffect = (ActivateSpiritEffect)DummyMap.DicEffect[ActivateSpiritEffect.Name].Copy();
            NewEffect.Value = "";

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
        }

        [TestMethod]
        public void TestIgnoreEnemySkillEffect()
        {
            IgnoreEnemySkillEffect NewEffect = (IgnoreEnemySkillEffect)DummyMap.DicEffect[IgnoreEnemySkillEffect.Name].Copy();
            NewEffect.Value = "test";

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot);

            Assert.AreEqual(0, GlobalDeathmatchContext.EffectOwnerUnit.ListIgnoreSkill.Count);

            DummySkill.AddSkillEffectsToTarget(string.Empty);

            Assert.AreEqual("test", GlobalDeathmatchContext.EffectOwnerUnit.ListIgnoreSkill[0]);
        }

        [TestMethod]
        public void TestRepairEffect()
        {
            RepairEffect NewEffect = (RepairEffect)DummyMap.DicEffect[RepairEffect.Name].Copy();

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot);

            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.RepairModifier);

            DummySkill.AddSkillEffectsToTarget(string.Empty);

            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.RepairModifier);
        }

        [TestMethod]
        public void TestResupplyEffect()
        {
            ResupplyEffect NewEffect = (ResupplyEffect)DummyMap.DicEffect[ResupplyEffect.Name].Copy();

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot);

            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.ResupplyModifier);

            DummySkill.AddSkillEffectsToTarget(string.Empty);

            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.ResupplyModifier);
        }

        [TestMethod]
        public void TestShieldEffect()
        {
            ShieldEffect NewEffect = (ShieldEffect)DummyMap.DicEffect[ShieldEffect.Name].Copy();

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot);

            Assert.IsFalse(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.ShieldModifier);

            DummySkill.AddSkillEffectsToTarget(string.Empty);

            Assert.IsTrue(GlobalDeathmatchContext.EffectOwnerUnit.Boosts.ShieldModifier);
        }

        [TestMethod]
        public void TestParryEffect()
        {
            ParryEffect NewEffect = (ParryEffect)DummyMap.DicEffect[ParryEffect.Name].Copy();
            NewEffect.Attacks.Add("Test");

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot);

            Assert.AreEqual(0, GlobalDeathmatchContext.EffectOwnerUnit.Boosts.ParryModifier.Count);

            DummySkill.AddSkillEffectsToTarget(string.Empty);

            Assert.AreEqual("Test", GlobalDeathmatchContext.EffectOwnerUnit.Boosts.ParryModifier[0]);
        }

        [TestMethod]
        public void TestNullifyAttackEffect()
        {
            NullifyAttackEffect NewEffect = (NullifyAttackEffect)DummyMap.DicEffect[NullifyAttackEffect.Name].Copy();
            NewEffect.Attacks.Add("Test");

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot);

            Assert.AreEqual(0, GlobalDeathmatchContext.EffectOwnerUnit.Boosts.NullifyAttackModifier.Count);

            DummySkill.AddSkillEffectsToTarget(string.Empty);

            Assert.AreEqual("Test", GlobalDeathmatchContext.EffectOwnerUnit.Boosts.NullifyAttackModifier[0]);
        }
    }
}
