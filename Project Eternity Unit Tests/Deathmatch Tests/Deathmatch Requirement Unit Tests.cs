using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.UnitTests
{
    public partial class DeathmatchTests
    {
        [TestMethod]
        public void BattleStartRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BattleStartRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BattleStartRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BattleStartRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.BattleStartRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeAttackRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeAttackRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeAttackRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeAttackRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.BeforeAttackRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeGettingAttackedRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeGettingAttackedRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeGettingAttackedRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeGettingAttackedRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.BeforeGettingAttackedRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeHitRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeHitRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeHitRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeHitRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.BeforeHitRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeGettingHitRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeGettingHitRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeGettingHitRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeGettingHitRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.BeforeGettingHitRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeMissRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeMissRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeMissRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeMissRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.BeforeMissRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeGettingMissedRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeGettingMissedRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeGettingMissedRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeGettingMissedRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.BeforeGettingMissedRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeGettingDestroyedRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeGettingDestroyedRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void BeforeGettingDestroyedRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.BeforeGettingDestroyedRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.BeforeGettingDestroyedRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterAttackRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterAttackRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterAttackRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterAttackRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.AfterAttackRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterGettingAttackedRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterGettingAttackedRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterGettingAttackedRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterGettingAttackedRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.AfterGettingAttackedRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterHitRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterHitRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterHitRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterHitRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.AfterHitRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterGettingHitRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterGettingHitRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterGettingHitRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterGettingHitRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.AfterGettingHitRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterMissRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterMissRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterMissRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterMissRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.AfterMissRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterGettingMissedRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterGettingMissedRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterGettingMissedRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterGettingMissedRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.AfterGettingMissedRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterGettingDestroyRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterGettingDestroyRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AfterGettingDestroyRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AfterGettingDestroyRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.AfterGettingDestroyRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void VSPilotRequirementFail()
        {
            VSPilotRequirement NewRequirement = (VSPilotRequirement)DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.VSPilotRequirementName].Copy();
            NewRequirement.PilotName = "";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            Squad DummyTargetSquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyTargetSquad, DummyTargetSquad.CurrentLeader, DummyTargetSquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void VSPilotRequirementSuccess()
        {
            VSPilotRequirement NewRequirement = (VSPilotRequirement)DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.VSPilotRequirementName].Copy();
            NewRequirement.PilotName = "Dummy Pilot";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            Squad DummyTargetSquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyTargetSquad, DummyTargetSquad.CurrentLeader, DummyTargetSquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void VSUnitRequirementFail()
        {
            VSUnitRequirement NewRequirement = (VSUnitRequirement)DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.VSUnitRequirementName].Copy();
            NewRequirement.UnitName = "";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            Squad DummyTargetSquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyTargetSquad, DummyTargetSquad.CurrentLeader, DummyTargetSquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);
            
            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void VSUnitRequirementSuccess()
        {
            VSUnitRequirement NewRequirement = (VSUnitRequirement)DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.VSUnitRequirementName].Copy();
            NewRequirement.UnitName = "Dummy Unit";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            Squad DummyTargetSquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyTargetSquad, DummyTargetSquad.CurrentLeader, DummyTargetSquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AttackUsedRequirementFail()
        {
            AttackUsedRequirement NewRequirement = (AttackUsedRequirement)DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AttackUsedRequirementName].Copy();
            NewRequirement.AttackName = "";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            Squad DummyTargetSquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyTargetSquad, DummyTargetSquad.CurrentLeader, DummyTargetSquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AttackUsedRequirementSuccess()
        {
            AttackUsedRequirement NewRequirement = (AttackUsedRequirement)DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AttackUsedRequirementName].Copy();
            NewRequirement.AttackName = "Dummy Attack";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            Squad DummyTargetSquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyTargetSquad, DummyTargetSquad.CurrentLeader, DummyTargetSquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AttackDefendedRequirementFail()
        {
            AttackDefendedRequirement NewRequirement = (AttackDefendedRequirement)DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AttackDefendedRequirementName].Copy();
            NewRequirement.AttackName = "";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            Squad DummyTargetSquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyTargetSquad, DummyTargetSquad.CurrentLeader, DummyTargetSquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void AttackDefendedRequirementSuccess()
        {
            AttackDefendedRequirement NewRequirement = (AttackDefendedRequirement)DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.AttackDefendedRequirementName].Copy();
            NewRequirement.AttackName = "Dummy Attack";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            Squad DummyTargetSquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyTargetSquad, DummyTargetSquad.CurrentLeader, DummyTargetSquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void SupportAttackRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.SupportAttackRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);


            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void SupportAttackRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.SupportAttackRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.SupportAttackRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void SupportDefendRequirementFail()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.SupportDefendRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void SupportDefendRequirementSuccess()
        {
            BaseSkillRequirement NewRequirement = DummyMap.Params.DicRequirement[DeathmatchSkillRequirement.SupportDefendRequirementName].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(DeathmatchSkillRequirement.SupportDefendRequirementName);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }
    }
}
