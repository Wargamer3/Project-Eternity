using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace ProjectEternity.Editors.AttackEditor
{
    public partial class QuoteEditor : Form
    {
        public QuoteEditor()
        {
            InitializeComponent();
        }

        private void tsmInsertLine_Click(object sender, EventArgs e)
        {
            if (dgvQuotes.CurrentRow != null)
                dgvQuotes.Rows.Insert(dgvQuotes.CurrentRow.Index, 1);
            else
                dgvQuotes.Rows.Add();
        }

        private void tsmDeleteLine_Click(object sender, EventArgs e)
        {
            if (dgvQuotes.CurrentRow != null && !dgvQuotes.CurrentRow.IsNewRow)
                dgvQuotes.Rows.RemoveAt(dgvQuotes.CurrentRow.Index);
        }

        private void cmsDataGridView_Opening(object sender, CancelEventArgs e)
        {
            if (dgvQuotes.CurrentRow != null && !dgvQuotes.CurrentRow.IsNewRow)
                tsmDeleteLine.Visible = true;
            else
                tsmDeleteLine.Visible = false;
        }

        private void dgvQuotes_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvQuotes.Rows[e.RowIndex].Selected = true;

            cmsDataGridView.Show(dgvQuotes, e.X, e.Y);
        }
    }
}
