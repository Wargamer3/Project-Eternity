using System.Threading;
using System.Windows.Forms;

namespace ProjectEternity
{
    public partial class SplashScreen : Form
    {
        private static SplashScreen Splash = null;
        private static Thread SplashThread = null;
        
        private string Status;
        private bool IsOpen;

        public SplashScreen()
        {
            InitializeComponent();
            UpdateTimer.Interval = 50;
            UpdateTimer.Start();
            IsOpen = true;
        }
        
        static public void ShowSplashScreen()
        {
            SplashThread = new Thread(new ThreadStart(ShowForm));
            SplashThread.IsBackground = true;
            SplashThread.SetApartmentState(ApartmentState.STA);
            SplashThread.Start();
            while (Splash == null || Splash.IsHandleCreated == false)
            {
                Thread.Sleep(50);
            }
        }
        
        static public void CloseForm()
        {
            if (Splash != null && Splash.IsDisposed == false)
            {
                Splash.IsOpen = false;
            }
            SplashThread = null;
            Splash = null;
        }
        
        static public void SetStatus(string newStatus)
        {
            Splash.Status = newStatus;
        }
        
        static private void ShowForm()
        {
            Splash = new SplashScreen();
            Application.Run(Splash);
        }

        private void UpdateTimer_Tick(object sender, System.EventArgs e)
        {
            lblStatus.Text = Status;
            
            if (!IsOpen)
            {
                UpdateTimer.Stop();
                Close();
            }
        }
    }
}
