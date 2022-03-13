using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;


namespace Organizer
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            
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
    }

}
