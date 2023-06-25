using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Effects;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.UnitTests
{
    public partial class DeathmatchTests
    {
        private BaseAutomaticSkill CreateDummySkill(BaseSkillRequirement Requirement, AutomaticSkillTargetType Target, BaseEffect Effect)
        {
            BaseAutomaticSkill NewSkill = new BaseAutomaticSkill();
            NewSkill.Name = "Dummy";
            NewSkill.ListSkillLevel.Add(new BaseSkillLevel());
            NewSkill.CurrentLevel = 1;

            BaseSkillActivation NewActivation = new BaseSkillActivation();
            NewSkill.CurrentSkillLevel.ListActivation.Add(NewActivation);

            NewActivation.ListRequirement.Add(Requirement);

            NewActivation.ListEffect.Add(Effect);
            NewActivation.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>());
            NewActivation.ListEffectTargetReal[0].Add(Target);

            return NewSkill;
        }
        
        [TestMethod]
        public void TestLoad()
        {
            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.Params.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy());
        }

        [TestMethod]
        public void TestSkillActivation()
        {
            FinalDamageEffect NewRequirement = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewRequirement.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.Params.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewRequirement);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
        }

        [TestMethod]
        public void PassiveRequirementSuccess1()
        {
            FinalDamageEffect NewRequirement = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewRequirement.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.Params.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewRequirement);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void PassiveRequirementSuccess2()
        {
            FinalDamageEffect NewRequirement = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewRequirement.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(DummyMap.Params.DicRequirement[PassiveRequirement.Name].Copy(),
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewRequirement);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(PassiveRequirement.Name);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void HPLeftRequirementFail()
        {
            HPLeftRequirement NewRequirement = (HPLeftRequirement)DummyMap.Params.DicRequirement[HPLeftRequirement.Name].Copy();
            NewRequirement.HPLeft = "1000";
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            GlobalDeathmatchContext.EffectOwnerUnit.DamageUnit(9600);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void HPLeftRequirementSuccess()
        {
            HPLeftRequirement NewRequirement = (HPLeftRequirement)DummyMap.Params.DicRequirement[HPLeftRequirement.Name].Copy();
            NewRequirement.HPLeft = "1000";
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void WillReachedRequirementFail()
        {
            WillReachedRequirement NewRequirement = (WillReachedRequirement)DummyMap.Params.DicRequirement[WillReachedRequirement.Name].Copy();
            NewRequirement.Will = "90";
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Will = 50;

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void WillReachedRequirementSuccess()
        {
            WillReachedRequirement NewRequirement = (WillReachedRequirement)DummyMap.Params.DicRequirement[WillReachedRequirement.Name].Copy();
            NewRequirement.Will = "90";
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void PilotedUnitNameRequirementFail()
        {
            PilotedUnitNameRequirement NewRequirement = (PilotedUnitNameRequirement)DummyMap.Params.DicRequirement[PilotedUnitNameRequirement.Name].Copy();
            NewRequirement.UnitName = "";

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
        public void PilotedUnitNameRequirementSuccess()
        {
            PilotedUnitNameRequirement NewRequirement = (PilotedUnitNameRequirement)DummyMap.Params.DicRequirement[PilotedUnitNameRequirement.Name].Copy();
            NewRequirement.UnitName = "Dummy Unit";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void PilotSkillFoundRequirementFail()
        {
            PilotSkillFoundRequirement NewRequirement = (PilotSkillFoundRequirement)DummyMap.Params.DicRequirement[PilotSkillFoundRequirement.Name].Copy();
            NewRequirement.PilotSkill = "";

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
        public void PilotSkillFoundRequirementSuccess()
        {
            PilotSkillFoundRequirement NewRequirement = (PilotSkillFoundRequirement)DummyMap.Params.DicRequirement[PilotSkillFoundRequirement.Name].Copy();
            NewRequirement.PilotSkill = "Dummy";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            GlobalDeathmatchContext.EffectOwnerUnit.Pilot.ArrayPilotSkill = new BaseAutomaticSkill[1] { DummySkill };

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        #region PilotStatRequirement Tests

        [TestMethod]
        public void PilotStatRequirementMELFail()
        {
            PilotStatRequirement NewRequirement = (PilotStatRequirement)DummyMap.Params.DicRequirement[PilotStatRequirement.Name].Copy();
            NewRequirement.StatusType = StatusTypes.MEL;
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;
            NewRequirement.EffectValue = "150";

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
        public void PilotStatRequirementMELSuccess()
        {
            PilotStatRequirement NewRequirement = (PilotStatRequirement)DummyMap.Params.DicRequirement[PilotStatRequirement.Name].Copy();
            NewRequirement.StatusType = StatusTypes.MEL;
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;
            NewRequirement.EffectValue = "50";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void PilotStatRequirementRNGFail()
        {
            PilotStatRequirement NewRequirement = (PilotStatRequirement)DummyMap.Params.DicRequirement[PilotStatRequirement.Name].Copy();
            NewRequirement.StatusType = StatusTypes.RNG;
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;
            NewRequirement.EffectValue = "150";

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
        public void PilotStatRequirementRNGSuccess()
        {
            PilotStatRequirement NewRequirement = (PilotStatRequirement)DummyMap.Params.DicRequirement[PilotStatRequirement.Name].Copy();
            NewRequirement.StatusType = StatusTypes.RNG;
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;
            NewRequirement.EffectValue = "50";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void PilotStatRequirementDEFFail()
        {
            PilotStatRequirement NewRequirement = (PilotStatRequirement)DummyMap.Params.DicRequirement[PilotStatRequirement.Name].Copy();
            NewRequirement.StatusType = StatusTypes.DEF;
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;
            NewRequirement.EffectValue = "150";

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
        public void PilotStatRequirementDEFSuccess()
        {
            PilotStatRequirement NewRequirement = (PilotStatRequirement)DummyMap.Params.DicRequirement[PilotStatRequirement.Name].Copy();
            NewRequirement.StatusType = StatusTypes.DEF;
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;
            NewRequirement.EffectValue = "50";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void PilotStatRequirementSKLFail()
        {
            PilotStatRequirement NewRequirement = (PilotStatRequirement)DummyMap.Params.DicRequirement[PilotStatRequirement.Name].Copy();
            NewRequirement.StatusType = StatusTypes.SKL;
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;
            NewRequirement.EffectValue = "150";

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
        public void PilotStatRequirementSKLSuccess()
        {
            PilotStatRequirement NewRequirement = (PilotStatRequirement)DummyMap.Params.DicRequirement[PilotStatRequirement.Name].Copy();
            NewRequirement.StatusType = StatusTypes.SKL;
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;
            NewRequirement.EffectValue = "50";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void PilotStatRequirementEVAFail()
        {
            PilotStatRequirement NewRequirement = (PilotStatRequirement)DummyMap.Params.DicRequirement[PilotStatRequirement.Name].Copy();
            NewRequirement.StatusType = StatusTypes.EVA;
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;
            NewRequirement.EffectValue = "150";

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
        public void PilotStatRequirementEVASuccess()
        {
            PilotStatRequirement NewRequirement = (PilotStatRequirement)DummyMap.Params.DicRequirement[PilotStatRequirement.Name].Copy();
            NewRequirement.StatusType = StatusTypes.EVA;
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;
            NewRequirement.EffectValue = "50";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        [TestMethod]
        public void PilotStatRequirementHITFail()
        {
            PilotStatRequirement NewRequirement = (PilotStatRequirement)DummyMap.Params.DicRequirement[PilotStatRequirement.Name].Copy();
            NewRequirement.StatusType = StatusTypes.HIT;
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;
            NewRequirement.EffectValue = "250";

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
        public void PilotStatRequirementHITSuccess()
        {
            PilotStatRequirement NewRequirement = (PilotStatRequirement)DummyMap.Params.DicRequirement[PilotStatRequirement.Name].Copy();
            NewRequirement.StatusType = StatusTypes.HIT;
            NewRequirement.LogicOperator = Core.Operators.LogicOperators.GreaterOrEqual;
            NewRequirement.EffectValue = "50";

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(string.Empty);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }

        #endregion

        [TestMethod]
        public void PlayerPhaseStartRequirementFail1()
        {
            PlayerPhaseStartRequirement NewRequirement = (PlayerPhaseStartRequirement)DummyMap.Params.DicRequirement[PlayerPhaseStartRequirement.Name].Copy();

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
        public void PlayerPhaseStartRequirementFail2()
        {
            PlayerPhaseStartRequirement NewRequirement = (PlayerPhaseStartRequirement)DummyMap.Params.DicRequirement[PlayerPhaseStartRequirement.Name].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            DummySkill.AddSkillEffectsToTarget(PassiveRequirement.Name);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(0, ListActiveEffect.Count);
        }

        [TestMethod]
        public void PlayerPhaseStartRequirementSuccess()
        {
            PlayerPhaseStartRequirement NewRequirement = (PlayerPhaseStartRequirement)DummyMap.Params.DicRequirement[PlayerPhaseStartRequirement.Name].Copy();

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.Params.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "1000";

            BaseAutomaticSkill DummySkill = CreateDummySkill(NewRequirement,
                                                            DummyMap.Params.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy(),
                                                            NewEffect);

            Squad DummySquad = CreateDummySquad();
            GlobalDeathmatchContext.SetContext(DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummySquad, DummySquad.CurrentLeader, DummySquad.CurrentLeader.Pilot, DummyMap.Params.ActiveParser);

            GlobalDeathmatchContext.EffectOwnerUnit.Pilot.ArrayPilotSkill = new BaseAutomaticSkill[1] { DummySkill };

            DummySkill.AddSkillEffectsToTarget(PlayerPhaseStartRequirement.Name);
            List<BaseEffect> ListActiveEffect = GlobalDeathmatchContext.EffectOwnerUnit.Pilot.Effects.GetActiveEffects("Dummy");
            Assert.AreEqual(1, ListActiveEffect.Count);
        }
    }
}
