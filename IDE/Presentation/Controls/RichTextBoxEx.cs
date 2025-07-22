using System;
using System.Drawing;
using System.Windows.Forms;

namespace Faseeh.Presentation.Controls
{
    public class RichTextBoxEx : RichTextBox
    {
        public (int Line, int Column) GetPosition()
        {   
            int index = this.SelectionStart;
            int line = this.GetLineFromCharIndex(index);
            int column = index - this.GetFirstCharIndexFromLine(line);

            return (line, column);
        }

        public void Redo()
        {
            if (CanRedo)
            {
                base.Redo();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // تحويل Tab إلى مسافات
            if (e.KeyCode == Keys.Tab)
            {
                this.SelectedText = "    ";
                e.SuppressKeyPress = true;
            }

            base.OnKeyDown(e);
        }
    }
}