using System;
using System.IO;
using System.Windows.Forms;
using Faseeh.Presentation.Controls;

namespace Faseeh.IDE.Core.Services
{
    public class FileService
    {
        private string _currentFilePath = string.Empty;
        
        public void NewFile(RichTextBoxEx textBox)
        {
            if (textBox.TextLength > 0)
            {
                var result = MessageBox.Show("هل تريد حفظ التغييرات قبل إنشاء ملف جديد؟", 
                    "محرر فصيح", 
                    MessageBoxButtons.YesNoCancel, 
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    SaveFile(textBox);
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            
            textBox.Clear();
            _currentFilePath = string.Empty;
        }
        
        public void OpenFile(RichTextBoxEx textBox)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "ملفات النصوص (*.txt)|*.txt|ملفات فصيح (*.faseeh)|*.faseeh|كل الملفات (*.*)|*.*";
                ofd.Title = "فتح ملف";
                
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        textBox.Text = File.ReadAllText(ofd.FileName);
                        _currentFilePath = ofd.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"خطأ في فتح الملف: {ex.Message}", "خطأ", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        public void SaveFile(RichTextBoxEx textBox)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                SaveFileAs(textBox);
                return;
            }
            
            try
            {
                File.WriteAllText(_currentFilePath, textBox.Text);
                MessageBox.Show("تم حفظ الملف بنجاح", "حفظ", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ الملف: {ex.Message}", "خطأ", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        public void SaveFileAs(RichTextBoxEx textBox)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "ملفات النصوص (*.txt)|*.txt|ملفات فصيح (*.faseeh)|*.faseeh|كل الملفات (*.*)|*.*";
                sfd.Title = "حفظ الملف باسم";
                
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _currentFilePath = sfd.FileName;
                    SaveFile(textBox);
                }
            }
        }
    }
}