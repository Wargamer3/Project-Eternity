using System;
using System.IO;
using System.Windows.Forms;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.Editors.SorcererStreetCharacterEditor
{
    public partial class AIEditor : Form
    {
        public AIEditor()
        {
            InitializeComponent();

            txtInvasionAggressiveness.MouseHover += new EventHandler(txtInvasionAggressiveness_MouseHover);
            txtInvasionElement.MouseHover += new EventHandler(txtInvasionElement_MouseHover);
            txtDefenceElement.MouseHover += new EventHandler(txtDefenceElement_MouseHover);
            txtSummonElement.MouseHover += new EventHandler(txtSummonElement_MouseHover);
            txtCreatureSummonCost.MouseHover += new EventHandler(txtCreatureSummonCost_MouseHover);

            txtRemainingMagic.MouseHover += new EventHandler(txtRemainingMagic_MouseHover);
            txtSpellEffectiveness.MouseHover += new EventHandler(txtSpellEffectiveness_MouseHover);
            txtSpellDamage.MouseHover += new EventHandler(txtSpellDamage_MouseHover);
            txtCardsInHand.MouseHover += new EventHandler(txtCardsInHand_MouseHover);
            txtSpellsOnCepters.MouseHover += new EventHandler(txtSpellsOnCepters_MouseHover);
            txtSymbols.MouseHover += new EventHandler(txtSymbols_MouseHover);
            txtSymbolChainBuy.MouseHover += new EventHandler(txtSymbolChainBuy_MouseHover);
            txtSymbolBuy.MouseHover += new EventHandler(txtSymbolBuy_MouseHover);
            txtSymbolSell.MouseHover += new EventHandler(txtSymbolSell_MouseHover);

            txtCreatureCardsImportance.MouseHover += new EventHandler(txtCreatureCardsImportance_MouseHover);
            txtItemCardsImportance.MouseHover += new EventHandler(txtItemCardsImportance_MouseHover);
            txtSpellCardsImportance.MouseHover += new EventHandler(txtSpellCardsImportance_MouseHover);
            txtLevelingUpLand.MouseHover += new EventHandler(txtLevelingUpLand_MouseHover);
            txtLandLevelUpCommand.MouseHover += new EventHandler(txtLandLevelUpCommand_MouseHover);
            txtCreatureExchangeCommand.MouseHover += new EventHandler(txtCreatureExchangeCommand_MouseHover);
            txtCreatureMovement.MouseHover += new EventHandler(txtCreatureMovement_MouseHover);
            txtCreatureAbility.MouseHover += new EventHandler(txtCreatureAbility_MouseHover);
            txtElementToStopOn.MouseHover += new EventHandler(txtElementToStopOn_MouseHover);
            txtAvoidExpensiveLand.MouseHover += new EventHandler(txtAvoidExpensiveLand_MouseHover);
            txtPlayerAlliances.MouseHover += new EventHandler(txtPlayerAlliances_MouseHover);
        }

        public AIEditor(PlayerCharacter LoadedCharacter)
            : this()
        {
            txtInvasionAggressiveness.Value = LoadedCharacter.PlayerCharacterAIParameter.InvasionAggressiveness;
            txtInvasionElement.Value = LoadedCharacter.PlayerCharacterAIParameter.InvasionElement;
            txtDefenceElement.Value = LoadedCharacter.PlayerCharacterAIParameter.DefenceElement;
            txtSummonElement.Value = LoadedCharacter.PlayerCharacterAIParameter.SummonElement;
            txtCreatureSummonCost.Value = LoadedCharacter.PlayerCharacterAIParameter.CreatureSummonCost;

            txtRemainingMagic.Value = LoadedCharacter.PlayerCharacterAIParameter.RemainingMagic;
            txtSpellEffectiveness.Value = LoadedCharacter.PlayerCharacterAIParameter.SpellEffectiveness;
            txtSpellDamage.Value = LoadedCharacter.PlayerCharacterAIParameter.SpellDamage;
            txtCardsInHand.Value = LoadedCharacter.PlayerCharacterAIParameter.CardsInHand;
            txtSpellsOnCepters.Value = LoadedCharacter.PlayerCharacterAIParameter.SpellsOnCepters;
            txtSymbols.Value = LoadedCharacter.PlayerCharacterAIParameter.Symbols;
            txtSymbolChainBuy.Value = LoadedCharacter.PlayerCharacterAIParameter.SymbolChainBuy;
            txtSymbolBuy.Value = LoadedCharacter.PlayerCharacterAIParameter.SymbolBuy;
            txtSymbolSell.Value = LoadedCharacter.PlayerCharacterAIParameter.SymbolSell;

            txtCreatureCardsImportance.Value = LoadedCharacter.PlayerCharacterAIParameter.CreatureCardsImportance;
            txtItemCardsImportance.Value = LoadedCharacter.PlayerCharacterAIParameter.ItemCardsImportance;
            txtSpellCardsImportance.Value = LoadedCharacter.PlayerCharacterAIParameter.SpellCardsImportance;
            txtLevelingUpLand.Value = LoadedCharacter.PlayerCharacterAIParameter.LevelingUpLand;
            txtLandLevelUpCommand.Value = LoadedCharacter.PlayerCharacterAIParameter.LandLevelUpCommand;
            txtCreatureExchangeCommand.Value = LoadedCharacter.PlayerCharacterAIParameter.CreatureExchangeCommand;
            txtCreatureMovement.Value = LoadedCharacter.PlayerCharacterAIParameter.CreatureMovement;
            txtCreatureAbility.Value = LoadedCharacter.PlayerCharacterAIParameter.CreatureAbility;
            txtElementToStopOn.Value = LoadedCharacter.PlayerCharacterAIParameter.ElementToStopOn;
            txtAvoidExpensiveLand.Value = LoadedCharacter.PlayerCharacterAIParameter.AvoidExpensiveLand;
            txtPlayerAlliances.Value = LoadedCharacter.PlayerCharacterAIParameter.PlayerAlliances;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write((byte)txtInvasionAggressiveness.Value);
            BW.Write((byte)txtInvasionElement.Value);
            BW.Write((byte)txtDefenceElement.Value);
            BW.Write((byte)txtSummonElement.Value);
            BW.Write((byte)txtCreatureSummonCost.Value);

            BW.Write((byte)txtRemainingMagic.Value);
            BW.Write((byte)txtSpellEffectiveness.Value);
            BW.Write((byte)txtSpellDamage.Value);
            BW.Write((byte)txtCardsInHand.Value);
            BW.Write((byte)txtSpellsOnCepters.Value);
            BW.Write((byte)txtSymbols.Value);
            BW.Write((byte)txtSymbolChainBuy.Value);
            BW.Write((byte)txtSymbolBuy.Value);
            BW.Write((byte)txtSymbolSell.Value);

            BW.Write((byte)txtCreatureCardsImportance.Value);
            BW.Write((byte)txtItemCardsImportance.Value);
            BW.Write((byte)txtSpellCardsImportance.Value);
            BW.Write((byte)txtLevelingUpLand.Value);
            BW.Write((byte)txtLandLevelUpCommand.Value);
            BW.Write((byte)txtCreatureExchangeCommand.Value);
            BW.Write((byte)txtCreatureMovement.Value);
            BW.Write((byte)txtCreatureAbility.Value);
            BW.Write((byte)txtElementToStopOn.Value);
            BW.Write((byte)txtAvoidExpensiveLand.Value);
            BW.Write((byte)txtPlayerAlliances.Value);
        }

        #region Battle

        private void txtInvasionAggressiveness_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Aggressive 1 - 9 Careful";
        }

        private void txtInvasionElement_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Anywhere 1 - 9 Elemental-focused";
        }

        private void txtDefenceElement_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Anywhere 1 - 9 Elemental-focused";
        }

        private void txtSummonElement_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Anywhere 1 - 9 Elemental-focused";
        }

        private void txtCreatureSummonCost_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Anything 1 - 9 Endurance-focused";
        }

        #endregion

        #region Magic

        private void txtRemainingMagic_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Don't worry 1 - 9 Worry";
        }

        private void txtSpellEffectiveness_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Don't worry 1 - 9 Worry";
        }

        private void txtSpellDamage_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Don't worry 1 - 9 Worry";
        }

        private void txtCardsInHand_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Don't worry 1 - 6 Worry";
        }

        private void txtSpellsOnCepters_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Emotional 1 - 3 Calm";
        }

        private void txtSymbols_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Don't worry 1 - 9 Worry";
        }

        private void txtSymbolChainBuy_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Little 1 - 9 A lot";
        }

        private void txtSymbolBuy_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Just a little 1 - 9 Lots";
        }

        private void txtSymbolSell_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Just a little 1 - 9 Lots";
        }

        #endregion

        #region Cards

        private void txtCreatureCardsImportance_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Unimportant 1 - 9 Important";
        }

        private void txtItemCardsImportance_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Unimportant 1 - 9 Important";
        }

        private void txtSpellCardsImportance_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Unimportant 1 - 9 Important";
        }

        private void txtLevelingUpLand_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Just a little 1 - 9 Lots";
        }

        private void txtLandLevelUpCommand_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Just a little 1 - 9 Lots";
        }

        private void txtCreatureExchangeCommand_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Sometimes 1 - 9 All the time";
        }

        private void txtCreatureMovement_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Sometimes 1 - 9 All the time";
        }

        private void txtCreatureAbility_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Sometimes 1 - 9 All the time";
        }

        private void txtElementToStopOn_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Anywhere 1 - 9 Element-focused";
        }

        private void txtAvoidExpensiveLand_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Don't worry 1 - 9 Avoid";
        }

        private void txtPlayerAlliances_MouseHover(object sender, EventArgs e)
        {
            tssTooltip.Text = "Ignore 1 - 9 Take good care of";
        }

        #endregion
    }
}
