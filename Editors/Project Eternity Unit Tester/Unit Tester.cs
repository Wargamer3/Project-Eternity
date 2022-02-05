using System;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Editors.UnitTester
{
    public partial class UnitTester : BaseEditor
    {
        private enum ItemSelectionChoices { Unit, Pilot, Weapon };

        private ItemSelectionChoices ItemSelectionChoice;
        private StatsBoosts AttackerBoosts;
        private StatsBoosts DefenderBoosts;
        private bool AllowUpdate;

        private Dictionary<string, Unit> DicUnitType = Unit.LoadAllUnits();
        private Dictionary<string, BaseSkillRequirement> DicRequirement;
        private Dictionary<string, BaseEffect> DicEffect;
        private Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget;
        private Dictionary<string, ManualSkillTarget> DicManualSkillTarget;

        public UnitTester()
        {
            InitializeComponent();

            DicUnitType = Unit.LoadAllUnits();
            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();
            DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();
            DicManualSkillTarget = ManualSkillTarget.LoadAllTargetTypes();

            FilePath = null;
            AttackerBoosts = new StatsBoosts();
            DefenderBoosts = new StatsBoosts();
            AllowUpdate = false;
        }

        public override EditorInfo[] LoadEditors()
        {
            return null;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
        }

        private void btnLoadUnit_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Unit;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathUnits));
        }

        private void btnLoadPilot_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Pilot;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacters));
        }

        private void btnLoadWeapon_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Weapon;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathAttacks));
        }

        private void UpdateResults(object sender, EventArgs e)
        {
            if (!AllowUpdate)
                return;
            
            int AttackerSize = 0;
            if (rbAttackerSizeLLL.Checked)
                AttackerSize = 30;
            else if (rbAttackerSizeLL.Checked)
                AttackerSize = 20;
            else if (rbAttackerSizeL.Checked)
                AttackerSize = 10;
            else if (rbAttackerSizeM.Checked)
                AttackerSize = 0;
            else if (rbAttackerSizeS.Checked)
                AttackerSize = -10;
            else if (rbAttackerSizeSS.Checked)
                AttackerSize = -20;

            int DefenderSize = 0;
            if (rbDefenderSizeLLL.Checked)
                DefenderSize = 30;
            else if (rbDefenderSizeLL.Checked)
                DefenderSize = 20;
            else if (rbDefenderSizeL.Checked)
                DefenderSize = 10;
            else if (rbDefenderSizeM.Checked)
                DefenderSize = 0;
            else if (rbDefenderSizeS.Checked)
                DefenderSize = -10;
            else if (rbDefenderSizeSS.Checked)
                DefenderSize = -20;

            int SizeCompare = AttackerSize - DefenderSize;
            int WeaponTerrain = 0;
            int DefenderTerrain = 0;
            int DefenderPilotTerrain = 0;
            int AttackerTerrain = 0;
            int AttackerPilotTerrain = 0;

            #region Terrain Values

            char WeaponTerrainLetter = 'B';
            if (rbDefenderMovementAir.Checked && !String.IsNullOrEmpty(cbWeaponAirRank.Text))
                WeaponTerrainLetter = cbWeaponAirRank.Text[0];
            else if (rbDefenderMovementLand.Checked && !String.IsNullOrEmpty(cbWeaponLandRank.Text))
                WeaponTerrainLetter = cbWeaponLandRank.Text[0];
            else if (rbDefenderMovementSea.Checked && !String.IsNullOrEmpty(cbWeaponSeaRank.Text))
                WeaponTerrainLetter = cbWeaponSeaRank.Text[0];
            else if (rbDefenderMovementSpace.Checked && !String.IsNullOrEmpty(cbWeaponSpaceRank.Text))
                WeaponTerrainLetter = cbWeaponSpaceRank.Text[0];

            switch (WeaponTerrainLetter)
            {
                case 'S':
                    WeaponTerrain = 20;
                    break;

                case 'A':
                    WeaponTerrain = 0;
                    break;

                case 'B':
                    WeaponTerrain = -20;
                    break;

                case 'C':
                    WeaponTerrain = -40;
                    break;

                case 'D':
                    WeaponTerrain = -60;
                    break;
            }

            #region Defender

            char DefenderTerrainLetter = 'B';
            if (rbDefenderMovementAir.Checked && !String.IsNullOrEmpty(cbDefenderTerrainAir.Text))
                DefenderTerrainLetter = cbDefenderTerrainAir.Text[0];
            else if (rbDefenderMovementLand.Checked && !String.IsNullOrEmpty(cbDefenderTerrainLand.Text))
                DefenderTerrainLetter = cbDefenderTerrainLand.Text[0];
            else if (rbDefenderMovementSea.Checked && !String.IsNullOrEmpty(cbDefenderTerrainSea.Text))
                DefenderTerrainLetter = cbDefenderTerrainSea.Text[0];
            else if (rbDefenderMovementSpace.Checked && !String.IsNullOrEmpty(cbDefenderTerrainSpace.Text))
                DefenderTerrainLetter = cbDefenderTerrainSpace.Text[0];

            switch (DefenderTerrainLetter)
            {
                case 'S':
                    DefenderTerrain = 20;
                    break;

                case 'A':
                    DefenderTerrain = 10;
                    break;

                case 'B':
                    DefenderTerrain = 0;
                    break;

                case 'C':
                    DefenderTerrain = -10;
                    break;

                case 'D':
                    DefenderTerrain = -20;
                    break;
            }

            char DefenderPilotTerrainLetter = 'B';
            if (rbDefenderMovementAir.Checked && !String.IsNullOrEmpty(cbDefenderAirRank.Text))
                DefenderPilotTerrainLetter = cbDefenderAirRank.Text[0];
            else if (rbDefenderMovementLand.Checked && !String.IsNullOrEmpty(cbDefenderLandRank.Text))
                DefenderPilotTerrainLetter = cbDefenderLandRank.Text[0];
            else if (rbDefenderMovementSea.Checked && !String.IsNullOrEmpty(cbDefenderSeaRank.Text))
                DefenderPilotTerrainLetter = cbDefenderSeaRank.Text[0];
            else if (rbDefenderMovementSpace.Checked && !String.IsNullOrEmpty(cbDefenderSpaceRank.Text))
                DefenderPilotTerrainLetter = cbDefenderSpaceRank.Text[0];

            switch (DefenderPilotTerrainLetter)
            {
                case 'S':
                    DefenderPilotTerrain = 20;
                    break;

                case 'A':
                    DefenderPilotTerrain = 10;
                    break;

                case 'B':
                    DefenderPilotTerrain = 0;
                    break;

                case 'C':
                    DefenderPilotTerrain = -10;
                    break;

                case 'D':
                    DefenderPilotTerrain = -20;
                    break;
            }

            #endregion

            #region Attacker

            char AttackerTerrainLetter = 'B';
            if (rbAttackerMovementAir.Checked && !String.IsNullOrEmpty(cbAttackerTerrainAir.Text))
                AttackerTerrainLetter = cbAttackerTerrainAir.Text[0];
            else if (rbAttackerMovementLand.Checked && !String.IsNullOrEmpty(cbAttackerTerrainLand.Text))
                AttackerTerrainLetter = cbAttackerTerrainLand.Text[0];
            else if (rbAttackerMovementSea.Checked && !String.IsNullOrEmpty(cbAttackerTerrainSea.Text))
                AttackerTerrainLetter = cbAttackerTerrainSea.Text[0];
            else if (rbAttackerMovementSpace.Checked && !String.IsNullOrEmpty(cbAttackerTerrainSpace.Text))
                AttackerTerrainLetter = cbAttackerTerrainSpace.Text[0];

            switch (AttackerTerrainLetter)
            {
                case 'S':
                    AttackerTerrain = 20;
                    break;

                case 'A':
                    AttackerTerrain = 10;
                    break;

                case 'B':
                    AttackerTerrain = 0;
                    break;

                case 'C':
                    AttackerTerrain = -10;
                    break;

                case 'D':
                    AttackerTerrain = -20;
                    break;
            }

            char AttackerPilotTerrainLetter = 'B';
            if (rbAttackerMovementAir.Checked && !String.IsNullOrEmpty(cbAttackerAirRank.Text))
                AttackerPilotTerrainLetter = cbAttackerAirRank.Text[0];
            else if (rbAttackerMovementLand.Checked && !String.IsNullOrEmpty(cbAttackerLandRank.Text))
                AttackerPilotTerrainLetter = cbAttackerLandRank.Text[0];
            else if (rbAttackerMovementSea.Checked && !String.IsNullOrEmpty(cbAttackerSeaRank.Text))
                AttackerPilotTerrainLetter = cbAttackerSeaRank.Text[0];
            else if (rbAttackerMovementSpace.Checked && !String.IsNullOrEmpty(cbAttackerSpaceRank.Text))
                AttackerPilotTerrainLetter = cbAttackerSpaceRank.Text[0];

            switch (AttackerPilotTerrainLetter)
            {
                case 'S':
                    AttackerPilotTerrain = 20;
                    break;

                case 'A':
                    AttackerPilotTerrain = 10;
                    break;

                case 'B':
                    AttackerPilotTerrain = 0;
                    break;

                case 'C':
                    AttackerPilotTerrain = -10;
                    break;

                case 'D':
                    AttackerPilotTerrain = -20;
                    break;
            }

            #endregion

            int FinalAttackerTerrainMultiplier = 0;

            switch (AttackerTerrain + AttackerPilotTerrain)
            {
                case -40:
                case -30:
                    FinalAttackerTerrainMultiplier = -60;
                    break;

                case -20:
                case -10:
                    FinalAttackerTerrainMultiplier = -40;
                    break;

                case 0:
                case 10:
                    FinalAttackerTerrainMultiplier = -20;
                    break;

                case 20:
                case 30:
                    FinalAttackerTerrainMultiplier = 0;
                    break;

                case 40:
                    FinalAttackerTerrainMultiplier = 20;
                    break;
            }

            int FinalDefenderTerrainMultiplier = 0;

            switch (DefenderTerrain + DefenderPilotTerrain)
            {
                case -40:
                case -30:
                    FinalDefenderTerrainMultiplier = -60;
                    break;

                case -20:
                case -10:
                    FinalDefenderTerrainMultiplier = -40;
                    break;

                case 0:
                case 10:
                    FinalDefenderTerrainMultiplier = -20;
                    break;

                case 20:
                case 30:
                    FinalDefenderTerrainMultiplier = 0;
                    break;

                case 40:
                    FinalDefenderTerrainMultiplier = 20;
                    break;
            }

            #endregion

            #region Attack Formula

            int PilotPower;

            if (rbWeaponMelee.Checked)
                PilotPower = (int)txtAttackerPilotMEL.Value;
            else
                PilotPower = (int)txtAttackerPilotRNG.Value;

            //ATTACK = (((Attack Power * ((Pilot Will + Pilot Stat)/ 200) *Attack Side Terrain Performance) +Additive Base Damage Bonuses) *Base Damage Multiplier Bonuses
            int AttackFormula = (int)((int)txtWeaponDamage.Value * ((int)txtAttackerPilotMorale.Value + PilotPower) / 200f * (1 + WeaponTerrain / 100f));
            txtAttackFormula.Text = AttackFormula.ToString();

            #endregion

            #region Defense Formula

            int Armor = (int)txtDefenderArmor.Value;

            //DEFENSE = ((Robot Armor Stat * ((Pilot Will + Pilot Def)/200) * Defense Side Terrain Performance) + Additive Base Defense Bonuses * Multiplying base defense bonuses) * Tile Bonus
            int DefenseFormula = (int)(Armor * (((int)txtAttackerPilotMorale.Value + (int)txtDefenderPilotDEF.Value) / 200f) * (1 + FinalDefenderTerrainMultiplier / 100f));
            txtDefenseFormula.Text = DefenseFormula.ToString();

            #endregion

            #region Damage Formula

            // FINAL DAMAGE = (((ATTACK - DEFENSE) * (ATTACKED AND DEFENDER SIZE COMPARISON)) + Additive Final Damage Bonuses) * Final Damage Multiplier Bonuses
            int DamageResult;

            if (DefenderBoosts.FinalDamageTakenFixedModifier > 0)
            {
                DamageResult = DefenderBoosts.FinalDamageTakenFixedModifier;
            }
            else
            {
                float Damage = Math.Max(0, (AttackFormula - DefenseFormula)  * (1 + SizeCompare / 100f));

                DamageResult = (int)Damage + AttackerBoosts.FinalDamageModifier;
                if (DefenderBoosts.ShieldModifier)
                    DamageResult = (int)(DamageResult * 0.4);

                DamageResult = (int)(DamageResult * AttackerBoosts.FinalDamageMultiplier);
            }
            txtDamageFormula.Text = DamageResult.ToString();

            #endregion

            #region Accuracy
            //(((Pilot Hit Stat/2 + 130) * Final Terrain Multiplier) + Weapon Hit Rate) + Base Hit Rate Effect
            int Accuracy = (int)(((((int)txtAttackerPilotHIT.Value / 2 + 130) * ((100 + FinalAttackerTerrainMultiplier) / 100.0)) + (int)txtWeaponAccuracy.Value + AttackerBoosts.AccuracyModifier) * AttackerBoosts.AccuracyMultiplier);
            txtAccuracyFormula.Text = Accuracy.ToString();

            #endregion

            #region Evasion

            //((Pilot Evasion/2)+Robot Mobility) * Final Terrain Multiplier) + Base Evasion Effect
            int Evasion = (int)((int)(txtDefenderPilotEVA.Value / 2 + txtDefenderMobility.Value) * ((100 + FinalDefenderTerrainMultiplier) / 100.0)) + DefenderBoosts.EvasionModifier;
            txtEvasionFormula.Text = Evasion.ToString();

            #endregion

            #region Hit Rate

            //(((Attacker Hit Rate + Defender Evasion) * Size Difference Multiplier) + Additive final hit rate effect) * Multiplying final hit rate effect
            int BaseHitRate;
            //If the Attacker have an accuracy modifier, use it.
            if (AttackerBoosts.AccuracyFixedModifier > 0)
                BaseHitRate = AttackerBoosts.AccuracyFixedModifier;
            //If the Defender have an accuracy modifier, use it.
            else if (DefenderBoosts.EvasionFixedModifier > 0)
                BaseHitRate = 100 - DefenderBoosts.EvasionFixedModifier;
            //Force the defender to dodge the attack.
            else if (DefenderBoosts.AutoDodgeModifier)
                BaseHitRate = 0;
            else//No particular modifier, use basic hit rate formula.
            {
                BaseHitRate = (int)((Accuracy - Evasion) * (1 + -SizeCompare / 100f));
            }
            BaseHitRate = Math.Max(0, Math.Min(100, BaseHitRate));

            txtHitRateForumla.Text = BaseHitRate.ToString();

            #endregion
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            AllowUpdate = false;

            List<char> ListGrade = new List<char> { '-', 'S', 'A', 'B', 'C', 'D' };

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    #region Unit

                    case ItemSelectionChoices.Unit:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(14);
                        if (Name != null)
                        {
                            string[] UnitInfo = Name.Split(new[] { "\\", "/" }, StringSplitOptions.None);
                            Unit NewUnit = Unit.FromType(UnitInfo[0], Name.Remove(0, UnitInfo[0].Length + 1), null, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget);

                            if (tabControl1.SelectedIndex == 0)//Attacker
                            {
                                txtAttackerName.Text = NewUnit.RelativePath;

                                txtAttackerHP.Value = NewUnit.MaxHP;
                                txtAttackerEN.Value = NewUnit.MaxEN;
                                txtAttackerArmor.Value = NewUnit.Armor;
                                txtAttackerMobility.Value = NewUnit.Mobility;

                                List<char> Grades = new List<char> { '-', 'S', 'A', 'B', 'C', 'D' };
                                cbAttackerTerrainAir.SelectedIndex = NewUnit.DicTerrainValue[UnitStats.TerrainAir];
                                cbAttackerTerrainLand.SelectedIndex = NewUnit.DicTerrainValue[UnitStats.TerrainLand];
                                cbAttackerTerrainSea.SelectedIndex = NewUnit.DicTerrainValue[UnitStats.TerrainSea];
                                cbAttackerTerrainSpace.SelectedIndex = NewUnit.DicTerrainValue[UnitStats.TerrainSpace];

                                if (NewUnit.ListTerrainChoices.Contains(UnitStats.TerrainAir))
                                    rbAttackerMovementAir.Checked = true;
                                else if (NewUnit.ListTerrainChoices.Contains(UnitStats.TerrainLand))
                                    rbAttackerMovementLand.Checked = true;
                                else if (NewUnit.ListTerrainChoices.Contains(UnitStats.TerrainSea))
                                    rbAttackerMovementSea.Checked = true;
                                else if (NewUnit.ListTerrainChoices.Contains(UnitStats.TerrainSpace))
                                    rbAttackerMovementSpace.Checked = true;

                                if (NewUnit.Size == UnitStats.UnitSizeLLL)
                                    rbAttackerSizeLLL.Checked = true;
                                else if (NewUnit.Size == UnitStats.UnitSizeLL)
                                    rbAttackerSizeLL.Checked = true;
                                else if (NewUnit.Size == UnitStats.UnitSizeL)
                                    rbAttackerSizeL.Checked = true;
                                else if (NewUnit.Size == UnitStats.UnitSizeM)
                                    rbAttackerSizeM.Checked = true;
                                else if (NewUnit.Size == UnitStats.UnitSizeS)
                                    rbAttackerSizeS.Checked = true;
                                else if (NewUnit.Size == UnitStats.UnitSizeSS)
                                    rbAttackerSizeSS.Checked = true;
                            }
                            else if (tabControl1.SelectedIndex == 1)//Defender
                            {
                                txtDefenderName.Text = NewUnit.RelativePath;

                                txtDefenderHP.Value = NewUnit.MaxHP;
                                txtDefenderEN.Value = NewUnit.MaxEN;
                                txtDefenderArmor.Value = NewUnit.Armor;
                                txtDefenderMobility.Value = NewUnit.Mobility;
                                
                                cbDefenderTerrainAir.SelectedIndex = NewUnit.DicTerrainValue[UnitStats.TerrainAir];
                                cbDefenderTerrainLand.SelectedIndex = NewUnit.DicTerrainValue[UnitStats.TerrainLand];
                                cbDefenderTerrainSea.SelectedIndex = NewUnit.DicTerrainValue[UnitStats.TerrainSea];
                                cbDefenderTerrainSpace.SelectedIndex = NewUnit.DicTerrainValue[UnitStats.TerrainSpace];

                                if (NewUnit.ListTerrainChoices.Contains(UnitStats.TerrainAir))
                                    rbDefenderMovementAir.Checked = true;
                                else if (NewUnit.ListTerrainChoices.Contains(UnitStats.TerrainLand))
                                    rbDefenderMovementLand.Checked = true;
                                else if (NewUnit.ListTerrainChoices.Contains(UnitStats.TerrainSea))
                                    rbDefenderMovementSea.Checked = true;
                                else if (NewUnit.ListTerrainChoices.Contains(UnitStats.TerrainSpace))
                                    rbDefenderMovementSpace.Checked = true;

                                if (NewUnit.Size == UnitStats.UnitSizeLLL)
                                    rbDefenderSizeLLL.Checked = true;
                                else if (NewUnit.Size == UnitStats.UnitSizeLL)
                                    rbDefenderSizeLL.Checked = true;
                                else if (NewUnit.Size == UnitStats.UnitSizeL)
                                    rbDefenderSizeL.Checked = true;
                                else if (NewUnit.Size == UnitStats.UnitSizeM)
                                    rbAttackerSizeM.Checked = true;
                                else if (NewUnit.Size == UnitStats.UnitSizeS)
                                    rbDefenderSizeS.Checked = true;
                                else if (NewUnit.Size == UnitStats.UnitSizeSS)
                                    rbDefenderSizeSS.Checked = true;
                            }
                        }
                        break;

                    #endregion

                    #region Pilot

                    case ItemSelectionChoices.Pilot:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(Items[I].LastIndexOf("Characters") + 11);
                        Character NewPilot = new Character(Name, null, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                        NewPilot.Level = 1;
                        NewPilot.Init();

                        if (tabControl1.SelectedIndex == 0)//Attacker
                        {
                            txtAttackerPilotMEL.Value = NewPilot.MEL;
                            txtAttackerPilotRNG.Value = NewPilot.RNG;
                            txtAttackerPilotDEF.Value = NewPilot.DEF;
                            txtAttackerPilotSKL.Value = NewPilot.SKL;
                            txtAttackerPilotEVA.Value = NewPilot.EVA;
                            txtAttackerPilotHIT.Value = NewPilot.HIT;
                            txtAttackerPilotMorale.Value = 100;
                            
                            cbAttackerAirRank.SelectedIndex = ListGrade.IndexOf(NewPilot.TerrainGrade.TerrainGradeAir);
                            cbAttackerLandRank.SelectedIndex = ListGrade.IndexOf(NewPilot.TerrainGrade.TerrainGradeLand);
                            cbAttackerSeaRank.SelectedIndex = ListGrade.IndexOf(NewPilot.TerrainGrade.TerrainGradeSea);
                            cbAttackerSpaceRank.SelectedIndex = ListGrade.IndexOf(NewPilot.TerrainGrade.TerrainGradeSpace);
                        }
                        else if (tabControl1.SelectedIndex == 1)//Defender
                        {
                            txtDefenderPilotMEL.Value = NewPilot.MEL;
                            txtDefenderPilotRNG.Value = NewPilot.RNG;
                            txtDefenderPilotDEF.Value = NewPilot.DEF;
                            txtDefenderPilotSKL.Value = NewPilot.SKL;
                            txtDefenderPilotEVA.Value = NewPilot.EVA;
                            txtDefenderPilotHIT.Value = NewPilot.HIT;


                            cbDefenderAirRank.SelectedIndex = ListGrade.IndexOf(NewPilot.TerrainGrade.TerrainGradeAir);
                            cbDefenderLandRank.SelectedIndex = ListGrade.IndexOf(NewPilot.TerrainGrade.TerrainGradeLand);
                            cbDefenderSeaRank.SelectedIndex = ListGrade.IndexOf(NewPilot.TerrainGrade.TerrainGradeSea);
                            cbDefenderSpaceRank.SelectedIndex = ListGrade.IndexOf(NewPilot.TerrainGrade.TerrainGradeSpace);
                        }
                        break;

                    #endregion

                    #region Weapon

                    case ItemSelectionChoices.Weapon:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(16);
                        Attack NewWeapon = new Attack(Name, null, DicRequirement, DicEffect, DicAutomaticSkillTarget);

                        txtWeaponDamage.Text = NewWeapon.PowerFormula;
                        txtWeaponAccuracy.Value = NewWeapon.Accuracy;

                        if (NewWeapon.Style == WeaponStyle.M)
                            rbWeaponMelee.Checked = true;
                        else if (NewWeapon.Style == WeaponStyle.R)
                            rbWeaponRange.Checked = true;

                        cbWeaponAirRank.SelectedIndex = ListGrade.IndexOf(NewWeapon.DicTerrainAttribute[UnitStats.TerrainAir]);
                        cbWeaponLandRank.SelectedIndex = ListGrade.IndexOf(NewWeapon.DicTerrainAttribute[UnitStats.TerrainLand]);
                        cbWeaponSeaRank.SelectedIndex = ListGrade.IndexOf(NewWeapon.DicTerrainAttribute[UnitStats.TerrainSea]);
                        cbWeaponSpaceRank.SelectedIndex = ListGrade.IndexOf(NewWeapon.DicTerrainAttribute[UnitStats.TerrainSpace]);
                        break;

                        #endregion
                }
            }

            AllowUpdate = true;
            UpdateResults(null, null);
        }
    }
}
