using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Nagru___Manga_Organizer
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /* Checks for already running instance of program
               Author: K. Scott Allen (August 20, 2004) */
            using (System.Threading.Mutex mutex = 
                new System.Threading.Mutex(false, @"Global\" + Application.ProductName))
            {
                if (!mutex.WaitOne(0, false)) {
                    MessageBox.Show("Instance already running");
                    return;
                }

                GC.Collect();
                Application.Run(new Main(args));
            }
        }
    }
}
