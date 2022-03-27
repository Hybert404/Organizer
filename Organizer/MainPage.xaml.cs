using Organizer.Resources;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Organizer
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        //Listowanie aplikacji
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

        //Uruchamianie procesu
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

        //Zamykanie procesu
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

        //Uruchamianie grupy aplikacji
        private void Przesun_Click(object sender, RoutedEventArgs e)
        {
            List<Process> procList = new List<Process>();
            foreach (Process p in listwyb.Items)
            {
                procList.Add(p);
            }

            {
                var s = ListBox.SelectedItem as Process;
                procList.Add(s);
            }


            listwyb.ItemsSource = procList;
        }

        private void uruchombutt_Click(object sender, RoutedEventArgs e)
        {
            foreach (Process p in listwyb.Items)
            {
                try
                {
                    Process.Start(p.ProcessName);
                }
                catch { }
            }
            listwyb.ItemsSource = null;

        }

    }
}
