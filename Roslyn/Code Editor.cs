using Microsoft.CodeAnalysis.CSharp;
using System.Drawing;
using System.Windows.Forms;

namespace Roslyn
{
    public partial class CodeEditor : UserControl
    {
        public string Code { get { return scCodeEditor.Text; } }

        public CodeEditor()
        {
            InitializeComponent();
        }

        private void scCodeEditor_TextChanged(object sender, System.EventArgs e)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(scCodeEditor.Text);
            var diags = syntaxTree.GetDiagnostics();

            lvErrors.Items.Clear();

            foreach (var d in diags)
            {
                lvErrors.Items.Add(d.ToString());
                
                if (d.Location.IsInSource)
                {
                    int lineEndOffset = d.Location.GetLineSpan().StartLinePosition.Line;
                }
            }
        }
    }
}
