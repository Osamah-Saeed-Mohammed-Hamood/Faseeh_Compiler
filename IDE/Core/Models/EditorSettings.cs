using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Faseeh.IDE.Core.Models
{
    [Serializable]
    public class EditorSettings
    {
        public Font Font { get; set; } = new Font("Consolas", 12);
        public bool DarkMode { get; set; } = false;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // تأكد من وجود خط افتراضي في حالة عدم التحميل الصحيح
            if (Font == null)
            {
                Font = new Font("Consolas", 12);
            }
        }
    }
}