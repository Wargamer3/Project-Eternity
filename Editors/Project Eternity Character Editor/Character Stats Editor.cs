using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.Editors.CharacterEditor
{
    public partial class CharacterStatsEditor : Form
    {
        private bool UseEvent;

        public CharacterStatsEditor(Character NewCharacter)
        {
            UseEvent = false;
            InitializeComponent();
            if (NewCharacter.CanPilot)
            {
                txtMaxLevel.Value = NewCharacter.MaxLevel;
                for (int L = 0; L < NewCharacter.MaxLevel; ++L)
                {
                    dgvStats.Rows.Add(L + 1,
                        NewCharacter.ArrayLevelMEL[L],
                        NewCharacter.ArrayLevelRNG[L],
                        NewCharacter.ArrayLevelDEF[L],
                        NewCharacter.ArrayLevelSKL[L],
                        NewCharacter.ArrayLevelEVA[L],
                        NewCharacter.ArrayLevelHIT[L],
                        NewCharacter.ArrayLevelMaxSP[L]);
                }
            }
            UseEvent = true;
        }

        private void dgvStats_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (UseEvent)
            {
                dgvTotalStatsIncrease.Rows.Clear();
                DataGridViewRow FirstRow = dgvStats.Rows[0];
                DataGridViewRow LastRow = dgvStats.Rows[dgvStats.Rows.GetLastRow(DataGridViewElementStates.None) - 1];
                dgvTotalStatsIncrease.Rows.Add(
                    Convert.ToInt32(LastRow.Cells[0].Value) - Convert.ToInt32(FirstRow.Cells[0].Value),// Level
                    Convert.ToInt32(LastRow.Cells[1].Value) - Convert.ToInt32(FirstRow.Cells[1].Value),// MEL
                    Convert.ToInt32(LastRow.Cells[2].Value) - Convert.ToInt32(FirstRow.Cells[2].Value),// RNG
                    Convert.ToInt32(LastRow.Cells[3].Value) - Convert.ToInt32(FirstRow.Cells[3].Value),// DEF
                    Convert.ToInt32(LastRow.Cells[4].Value) - Convert.ToInt32(FirstRow.Cells[4].Value),// SKL
                    Convert.ToInt32(LastRow.Cells[5].Value) - Convert.ToInt32(FirstRow.Cells[5].Value),// EVA
                    Convert.ToInt32(LastRow.Cells[6].Value) - Convert.ToInt32(FirstRow.Cells[6].Value),// HIT
                    Convert.ToInt32(LastRow.Cells[7].Value) - Convert.ToInt32(FirstRow.Cells[7].Value));// SP
            }
        }

        private void txtMaxLevel_ValueChanged(object sender, EventArgs e)
        {
            if (UseEvent)
            {
                while (dgvStats.Rows.Count > txtMaxLevel.Value)
                {
                    dgvStats.Rows.RemoveAt(dgvStats.Rows.Count - 1);
                }

                DataGridViewRow LastRow = dgvStats.Rows[dgvStats.Rows.GetLastRow(DataGridViewElementStates.None)];
                while (dgvStats.Rows.Count < txtMaxLevel.Value)
                {
                    dgvStats.Rows.Add(
                        LastRow.Cells[0].Value,
                        LastRow.Cells[1].Value,
                        LastRow.Cells[2].Value,
                        LastRow.Cells[3].Value,
                        LastRow.Cells[4].Value,
                        LastRow.Cells[5].Value,
                        LastRow.Cells[6].Value,
                        LastRow.Cells[7].Value);
                }
            }
        }

        private void btnFillTheBlanks_Click(object sender, EventArgs e)
        {
            FillBlankForColumn(1);
            FillBlankForColumn(2);
            FillBlankForColumn(3);
            FillBlankForColumn(4);
            FillBlankForColumn(5);
            FillBlankForColumn(6);
            FillBlankForColumn(7);
        }

        private void btnResetStats_Click(object sender, EventArgs e)
        {
            dgvStats.Rows.Clear();
            for (int L = 0; L < txtMaxLevel.Value; ++L)
            {
                dgvStats.Rows.Add(L + 1,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0,
                    0);
            }
        }

        private void FillBlankForColumn(int Index)
        {
            List<int> ListOldCell = new List<int>();
            int OldValue = 0;
            
            for (int L = 0; L < (int)txtMaxLevel.Value; ++L)
            {
                if (dgvStats.Rows[L].Cells[Index].Value == null || string.IsNullOrWhiteSpace(dgvStats.Rows[L].Cells[Index].Value.ToString())
                    || Convert.ToInt32(dgvStats.Rows[L].Cells[Index].Value) == 0)
                {
                    ListOldCell.Add(L);
                }
                else
                {
                    int NextValue = Convert.ToInt32(dgvStats.Rows[L].Cells[Index].Value);

                    if (ListOldCell.Count > 0)
                    {
                        double Difference = (NextValue - OldValue) / (double)(ListOldCell.Count + 1);
                        for (int C = 0; C < ListOldCell.Count; ++C)
                        {
                            dgvStats.Rows[ListOldCell[C]].Cells[Index].Value = OldValue + (int)(Difference * (C + 1));
                        }

                        ListOldCell.Clear();
                    }
                    OldValue = NextValue;
                }
            }
        }
    }
}
