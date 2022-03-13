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
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        public MainWindow()
        {
            InitializeComponent();
            m_notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                BalloonTipText = "The app has been minimised. Click the tray icon to show.",
                BalloonTipTitle = "The App",
                Text = "The App",
                Icon = new System.Drawing.Icon("TheAppIcon.ico")
            };
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TextBlock1.Text == "Hello" || TextBlock1.Text == "Nie klikać")
            {
                TextBlock1.Text = "World";
                //Process[] localByName = Process.GetProcessesByName("notepad");
            }
            else
            {
                TextBlock1.Text = "Hello";
                //ListProcesses();

            }
        }
        //private void ListProcesses()
        //{
        //    Process[] processCollection = Process.GetProcesses();
        //    foreach (Process p in processCollection)
        //    {
        //        ListBox.Items.Add(p);
        //    }
        //}

        private void ListProcBttn_Click(object sender, RoutedEventArgs e)
        {
            GetApplications();
            //GetWindows();
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
        //    m_notifyIcon.Dispose();
        //    m_notifyIcon = null;
        //}

        private WindowState m_storedWindowState = WindowState.Normal;
        void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (m_notifyIcon != null)
                    m_notifyIcon.ShowBalloonTip(2000);
            }
            else
                m_storedWindowState = WindowState;
        }
        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = m_storedWindowState;
        }
        void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = show;
        }
    }

}
