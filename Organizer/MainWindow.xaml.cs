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
            if (TextBlock1.Text == "Hello")
            {
                TextBlock1.Text = "World";
                Process[] localByName = Process.GetProcessesByName("notepad");
            }
            else
            {
                TextBlock1.Text = "Hello";
                ListProcesses();

            }
        }
        private void ListProcesses()
        {
            Process[] processCollection = Process.GetProcesses();
            foreach (Process p in processCollection)
            {
                ListBox.Items.Add(p);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GetApplications();
        }

        public static void GetApplications()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Process p in Process.GetProcesses("."))
            {
                try
                {
                    if (p.MainWindowTitle.Length > 0)
                    {
                        Console.WriteLine("Window Title:" + p.MainWindowTitle.ToString());
                        Console.WriteLine("Process Name:" + p.ProcessName.ToString());
                    }
                }
                catch { }
            }
        }
    }
    
}
