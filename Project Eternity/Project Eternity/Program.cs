using System;
using System.IO;

namespace ProjectEternity
{
#if WINDOWS || XBOX
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            using (Game game = new Game())
            {
                game.Run();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            using (StreamWriter SW = new StreamWriter("Error.txt", true))
            {
                WriteException(SW, (Exception)e.ExceptionObject);
                SW.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }

        private static void WriteException(StreamWriter SW, Exception Ex)
        {
            SW.WriteLine("Message :" + Ex.Message + Environment.NewLine + "StackTrace :" + Ex.StackTrace +
               Environment.NewLine + "Date :" + DateTime.Now.ToString());
            if (Ex.InnerException != null)
            {
                SW.WriteLine("Inner:");
                WriteException(SW, Ex.InnerException);
            }
        }
    }

#endif
}

