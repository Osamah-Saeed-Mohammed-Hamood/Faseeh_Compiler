using System;
using System.Drawing;
using System.Windows.Forms;
using Faseeh.Presentation.Controls;

namespace Faseeh.IDE.Core.Services
{
    public class EditorService
    {
        public void UpdateStatus(RichTextBoxEx textBox, ToolStripItem statusItem)
        {
            if (textBox.Text.Length > 0)
            {
                char firstChar = textBox.Text[0];
                textBox.RightToLeft = (firstChar >= 'A' && firstChar <= 'Z') ||
                                    (firstChar >= 'a' && firstChar <= 'z') ?
                                    RightToLeft.No : RightToLeft.Yes;
            }

            int wordCount = textBox.Text.Split(new char[] { ' ', '\n', '\r', '\t' },
                             StringSplitOptions.RemoveEmptyEntries).Length;
            int lineCount = textBox.Lines.Length;

            statusItem.Text = $"الكلمات: {wordCount} | الأسطر: {lineCount}";
        }

        public void ToggleFontStyle(RichTextBoxEx textBox, FontStyle style)
        {
            if (textBox.SelectionFont != null)
            {
                Font currentFont = textBox.SelectionFont;
                FontStyle newStyle = currentFont.Style.HasFlag(style) ?
                                    currentFont.Style & ~style :
                                    currentFont.Style | style;

                textBox.SelectionFont = new Font(currentFont, newStyle);
            }
            else
            {
                textBox.SelectionFont = new Font(textBox.Font, style);
            }
        }
    }
}