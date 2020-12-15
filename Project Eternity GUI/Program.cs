using System;
using System.IO;
using System.Windows.Forms;

namespace ProjectEternity.GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.Run(new GUI());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            using (StreamWriter SW = new StreamWriter("Error.txt", true))
            {
                Exception Ex = (Exception)e.ExceptionObject;

                SW.WriteLine("Message :" + Ex.Message + Environment.NewLine + "StackTrace :" + Ex.StackTrace +
                   Environment.NewLine + "Date :" + DateTime.Now.ToString());
                SW.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }
    }
}
