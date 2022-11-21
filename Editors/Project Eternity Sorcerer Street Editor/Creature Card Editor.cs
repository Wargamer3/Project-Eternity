using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.SorcererStreetScreen;
using static ProjectEternity.GameScreens.SorcererStreetScreen.ItemCard;
using static ProjectEternity.GameScreens.SorcererStreetScreen.CreatureCard;

namespace ProjectEternity.Editors.CardEditor
{
    public partial class CreatureCardEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Animation, Skill };

        private ItemSelectionChoices ItemSelectionChoice;

        public CreatureCardEditor()
        {
            InitializeComponent();
            FilePath = null;

            cboRarity.SelectedIndex = 0;
        }

        public CreatureCardEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadCard(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathSorcererStreetCardsCreatures, GUIRootPathSorcererStreetCards }, "Sorcerer Street/Creature Cards/", new string[] { ".pec" }, typeof(CreatureCardEditor)),
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            BW.Write((int)txtMagicCost.Value);
            BW.Write((byte)txtCardSacrificed.Value);
            BW.Write((byte)cboRarity.SelectedIndex);

            BW.Write(txtAttackAnimation.Text);

            BW.Write((int)txtMaxHP.Value);
            BW.Write((int)txtMaxST.Value);
            BW.Write(cboSubtype.Text);
            BW.Write(txtSkill.Text);

            #region Affinities

            List<byte> ListAffinity = new List<byte>();
            if (cbAffinityNeutral.Checked)
            {
                ListAffinity.Add((byte)ElementalAffinity.Neutral);
            }
            if (cbAffinityFire.Checked)
            {
                ListAffinity.Add((byte)ElementalAffinity.Fire);
            }
            if (cbAffinityWater.Checked)
            {
                ListAffinity.Add((byte)ElementalAffinity.Water);
            }
            if (cbAffinityEarth.Checked)
            {
                ListAffinity.Add((byte)ElementalAffinity.Earth);
            }
            if (cbAffinityAir.Checked)
            {
                ListAffinity.Add((byte)ElementalAffinity.Air);
            }

            BW.Write(ListAffinity.Count);
            for (int A = 0; A < ListAffinity.Count; ++A)
            {
                BW.Write(ListAffinity[A]);
            }

            #endregion

            #region Land Limits

            List<byte> ListLandLimit = new List<byte>();
            if (cbLandLimitNeutral.Checked)
            {
                ListLandLimit.Add((byte)ElementalAffinity.Neutral);
            }
            if (cbLandLimitFire.Checked)
            {
                ListLandLimit.Add((byte)ElementalAffinity.Fire);
            }
            if (cbLandLimitWater.Checked)
            {
                ListLandLimit.Add((byte)ElementalAffinity.Water);
            }
            if (cbLandLimitEarth.Checked)
            {
                ListLandLimit.Add((byte)ElementalAffinity.Earth);
            }
            if (cbLandLimitAir.Checked)
            {
                ListLandLimit.Add((byte)ElementalAffinity.Air);
            }

            BW.Write(ListLandLimit.Count);
            for (int A = 0; A < ListLandLimit.Count; ++A)
            {
                BW.Write(ListLandLimit[A]);
            }

            #endregion

            #region Item Limits

            List<byte> ListItemLimit = new List<byte>();
            if (cbItemLimitWeapon.Checked)
            {
                ListItemLimit.Add((byte)ItemTypes.Weapon);
            }
            if (cbItemLimitArmor.Checked)
            {
                ListItemLimit.Add((byte)ItemTypes.Armor);
            }
            if (cbItemLimitTools.Checked)
            {
                ListItemLimit.Add((byte)ItemTypes.Tools);
            }
            if (cbItemLimitScrolls.Checked)
            {
                ListItemLimit.Add((byte)ItemTypes.Scrolls);
            }

            BW.Write(ListItemLimit.Count);
            for (int A = 0; A < ListItemLimit.Count; ++A)
            {
                BW.Write(ListItemLimit[A]);
            }

            #endregion

            //Terrain Requirements
            BW.Write(5);
            BW.Write((byte)ElementalAffinity.Neutral);
            BW.Write((int)txtNeutral.Value);
            BW.Write((byte)ElementalAffinity.Fire);
            BW.Write((int)txtFire.Value);
            BW.Write((byte)ElementalAffinity.Water);
            BW.Write((int)txtWater.Value);
            BW.Write((byte)ElementalAffinity.Earth);
            BW.Write((int)txtEarth.Value);
            BW.Write((byte)ElementalAffinity.Air);
            BW.Write((int)txtAir.Value);

            FS.Close();
            BW.Close();
        }

        private void LoadCard(string UnitPath)
        {
            Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(39);
            CreatureCard LoadedCard = new CreatureCard(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);

            this.Text = LoadedCard.Name + " - Project Eternity Creature Card Editor";

            txtName.Text = LoadedCard.Name;
            txtDescription.Text = LoadedCard.Description;

            txtMagicCost.Value = LoadedCard.MagicCost;
            cboRarity.SelectedIndex = (int)LoadedCard.Rarity;
            txtCardSacrificed.Value = LoadedCard.DiscardCardRequired;
            txtAttackAnimation.Text = LoadedCard.AttackAnimationPath;

            txtMaxHP.Value = LoadedCard.MaxHP;
            txtMaxST.Value = LoadedCard.MaxST;
            cboSubtype.SelectedIndex = 0;
            txtSkill.Text = LoadedCard.SkillChainName;

            #region Affinities

            if (LoadedCard.ArrayAffinity.Contains(ElementalAffinity.Neutral))
            {
                cbAffinityNeutral.Checked = true;
            }
            if (LoadedCard.ArrayAffinity.Contains(ElementalAffinity.Fire))
            {
                cbAffinityFire.Checked = true;
            }
            if (LoadedCard.ArrayAffinity.Contains(ElementalAffinity.Water))
            {
                cbAffinityWater.Checked = true;
            }
            if (LoadedCard.ArrayAffinity.Contains(ElementalAffinity.Earth))
            {
                cbAffinityEarth.Checked = true;
            }
            if (LoadedCard.ArrayAffinity.Contains(ElementalAffinity.Air))
            {
                cbAffinityAir.Checked = true;
            }

            #endregion

            #region Land Limits

            if (LoadedCard.ArrayLandLimit.Contains(ElementalAffinity.Neutral))
            {
                cbLandLimitNeutral.Checked = true;
            }
            if (LoadedCard.ArrayLandLimit.Contains(ElementalAffinity.Fire))
            {
                cbLandLimitFire.Checked = true;
            }
            if (LoadedCard.ArrayLandLimit.Contains(ElementalAffinity.Water))
            {
                cbLandLimitWater.Checked = true;
            }
            if (LoadedCard.ArrayLandLimit.Contains(ElementalAffinity.Earth))
            {
                cbLandLimitEarth.Checked = true;
            }
            if (LoadedCard.ArrayLandLimit.Contains(ElementalAffinity.Air))
            {
                cbLandLimitAir.Checked = true;
            }

            #endregion

            #region Item Limits

            if (LoadedCard.ArrayItemLimit.Contains(ItemTypes.Weapon))
            {
                cbItemLimitWeapon.Checked = true;
            }
            if (LoadedCard.ArrayItemLimit.Contains(ItemTypes.Armor))
            {
                cbItemLimitArmor.Checked = true;
            }
            if (LoadedCard.ArrayItemLimit.Contains(ItemTypes.Tools))
            {
                cbItemLimitTools.Checked = true;
            }
            if (LoadedCard.ArrayItemLimit.Contains(ItemTypes.Scrolls))
            {
                cbItemLimitScrolls.Checked = true;
            }

            #endregion

            #region Terrain Requirements

            if (LoadedCard.DicTerrainRequiement.ContainsKey(ElementalAffinity.Neutral))
            {
                txtNeutral.Value = LoadedCard.DicTerrainRequiement[ElementalAffinity.Neutral];
            }
            if (LoadedCard.DicTerrainRequiement.ContainsKey(ElementalAffinity.Fire))
            {
                txtFire.Value = LoadedCard.DicTerrainRequiement[ElementalAffinity.Fire];
            }
            if (LoadedCard.DicTerrainRequiement.ContainsKey(ElementalAffinity.Water))
            {
                txtWater.Value = LoadedCard.DicTerrainRequiement[ElementalAffinity.Water];
            }
            if (LoadedCard.DicTerrainRequiement.ContainsKey(ElementalAffinity.Earth))
            {
                txtEarth.Value = LoadedCard.DicTerrainRequiement[ElementalAffinity.Earth];
            }
            if (LoadedCard.DicTerrainRequiement.ContainsKey(ElementalAffinity.Air))
            {
                txtAir.Value = LoadedCard.DicTerrainRequiement[ElementalAffinity.Air];
            }

            #endregion
        }

        private void btnSetAttackAnimation_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Animation;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathAnimations));
        }

        private void btnSetSkill_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathSorcererStreetSkillChains));
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Animation:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(19);
                        txtAttackAnimation.Text = Name;
                        break;

                    case ItemSelectionChoices.Skill:
                        if (Items[I] == null)
                        {
                            txtSkill.Text = string.Empty;
                        }
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(37);
                            txtSkill.Text = Name;
                        }
                        break;
                }
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }
    }
}
