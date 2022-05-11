using System;
using System.Windows;
using System.Windows.Controls;

namespace Organizer
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon notifyIcon;
        public MainWindow()
        {
            InitializeComponent();
            Main.Content = new MainPage();
            //DataClasses1DataContext DB = new DataClasses1DataContext();

            //using (DataClasses1DataContext dbContext = new DataClasses1DataContext())
            //{
            //    IEnumerable<Profile> profiles = dbContext.ExecuteQuery<Profile>("Select * from Profile");
            //    foreach (Profile profile in profiles)
            //    {
            //        MessageBox.Show(profile.Id_prof + " " + profile.Name);
            //    }
            //}

            // Ikona w zasobniku
            notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                BalloonTipText = "The app has been minimised. Click the tray icon to show.",
                BalloonTipTitle = "The App",
                Text = "The App",
                Icon = Properties.Resources.AppIcon
            };
            this.notifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(notifyIcon_Click);
        }

        //Menu główne, sterowanie stronami
        private void Bttn_Click_ManageProfiles(object sender, RoutedEventArgs e)
        {
            Main.Content = new ProfilesManagerPage();
        }

        private void Bttn_Click_MainWindow(object sender, RoutedEventArgs e)
        {
            Main.Content = new MainPage();
        }
        private void Button_Click_Stats(object sender, RoutedEventArgs e)
        {
            Main.Content = new StatsPage();
        }

        //Funkcje ikony w zasobniku
        void OnClose(object sender, EventArgs args)
        {
            notifyIcon.Dispose();
            notifyIcon = null;
        }

        private WindowState storedWindowState = WindowState.Normal;
        void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
            else
                storedWindowState = WindowState;
        }
        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (notifyIcon != null)
                notifyIcon.Visible = !IsVisible;
        }

        void notifyIcon_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenu menu = (ContextMenu)this.FindResource("NotifierContextMenu");
                menu.IsOpen = true;
            }
        }

        //Funkcje menu kontekstowe
        private void Menu_Close(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Closing app");
            System.Windows.Application.Current.Shutdown();
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            Show();
            Activate();
            Focus();
            Topmost = true;
            WindowState = storedWindowState;
        }

    }
}