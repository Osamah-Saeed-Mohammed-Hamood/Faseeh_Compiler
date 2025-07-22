using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Faseeh.Presentation;
using Faseeh.IDE.Core.Services;

namespace Faseeh
{
    class Program 
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // تحميل الإعدادات المسبقة إن وجدت
            var themeService = new ThemeService();
            var settings = themeService.LoadSettings();

            // تطبيق الثقافة العربية
            System.Threading.Thread.CurrentThread.CurrentUICulture =new System.Globalization.CultureInfo("ar-SA");

            Application.Run(new MainForm(settings));
        }
    }
}
