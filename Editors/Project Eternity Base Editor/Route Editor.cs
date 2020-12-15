using System.Windows.Forms;

namespace ProjectEternity.Core.Editor
{
    public partial class RouteEditor : Form
    {
        public RouteEditor(string Title, string Summary, string Description)
        {
            InitializeComponent();

            txtTitle.Text = Title;
            txtSummary.Text = Summary;
            txtDescription.Text = Description;
        }
    }
}
