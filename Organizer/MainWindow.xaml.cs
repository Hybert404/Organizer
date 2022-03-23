using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                BalloonTipText = "The app has been minimised. Click the tray icon to show.",
                BalloonTipTitle = "The App",
                Text = "The App",
                Icon = Properties.Resources.AppIcon
            };
            notifyIcon.Click += new EventHandler(notifyIcon_Click);
            this.notifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(notifyIcon_Click);
        }

        private void ListProcBttn_Click(object sender, RoutedEventArgs e)
        {
            GetApplications();
        }

        public void GetApplications()
        {
            List<Process> procList = new List<Process>();
            foreach (Process p in Process.GetProcesses("."))
            {
                try
                {
                    if (p.MainWindowTitle.Length > 0)
                    {
                        procList.Add(p);
                    }
                }
                catch { }
            }
            ListBox.ItemsSource = procList;
        }

        public void GetWindows()
        {
            foreach (Window window in App.Current.Windows)
            {
                ListBox.Items.Add(window.Title);
            }
        }

        private void RunBttn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var s = ListBox.SelectedItem as Process;
                TestBox.Items.Add(s.ProcessName);
                Process.Start(s.ProcessName);
            }
            catch
            {
                DebugBox.Items.Add("Nie można uruchomiń procesu");
            }
        }

        private void KillBttn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var s = ListBox.SelectedItem as Process;
                TestBox.Items.Add(s.ProcessName);
                if (s.ProcessName == "devenv")
                {
                    TestBox.Items.Add("Próbujesz zakończyć proces Visual Studio, da się, ale lepiej tego nie robić");
                }
                else
                {
                    s.Kill();
                }
            }
            catch
            {
                DebugBox.Items.Add("Nie można zamknąć procesu");
            }
        }

        //void OnClose(object sender, CancelEventArgs args)
        //{
        //    notifyIcon.Dispose();
        //    notifyIcon = null;
        //}

        private WindowState storedWindowState = WindowState.Normal;
        void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (notifyIcon != null)
                    notifyIcon.ShowBalloonTip(2000);
            }
            else
                storedWindowState = WindowState;
        }
        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
        }

        void notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = storedWindowState;
        }
        void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (notifyIcon != null)
                notifyIcon.Visible = show;
        }

    }

}
