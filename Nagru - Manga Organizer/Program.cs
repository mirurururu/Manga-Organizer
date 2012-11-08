using System;
using System.Windows.Forms;
using System.Threading;

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
            using (Mutex mutex = new Mutex(false, @"Global\" + Application.ProductName))
            {
                if (!mutex.WaitOne(0, false)) {
                    MessageBox.Show("Instance already running", Application.ProductName, 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                GC.Collect();
                Application.Run(new Main(args));
            }
        }
    }
}
