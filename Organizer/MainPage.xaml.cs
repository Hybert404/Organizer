using Organizer.Resources;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System;

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
            listProfilesMain();
        }

        ////Uruchamianie procesu
        //private void RunBttn_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var s = ListBox.SelectedItem as Process;
        //        TestBox.Items.Add(s.ProcessName);
        //        Process.Start(s.ProcessName);
        //    }
        //    catch
        //    {
        //        DebugBox.Items.Add("Nie można uruchomiń procesu");
        //    }
        //}

        ////Zamykanie procesu
        //private void KillBttn_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var s = ListBox.SelectedItem as Process;
        //        TestBox.Items.Add(s.ProcessName);
        //        if (s.ProcessName == "devenv")
        //        {
        //            TestBox.Items.Add("Próbujesz zakończyć proces Visual Studio, da się, ale lepiej tego nie robić");
        //        }
        //        else
        //        {
        //            s.Kill();
        //        }
        //    }
        //    catch
        //    {
        //        DebugBox.Items.Add("Nie można zamknąć procesu");
        //    }
        //}
        void listProfilesMain()
        {
            using (DataClasses1DataContext DB = new DataClasses1DataContext())
            {
                List<Profile> profList = new List<Profile>();
                foreach (Profile prof in DB.Profile)
                {
                    try
                    {
                        profList.Add(prof);
                    }
                    catch { }
                }
                profileListMain.ItemsSource = profList;
            }
        }

        private void BttnActivateProfile_Click(object sender, RoutedEventArgs e)
        {
            var selProf = profileListMain.SelectedItem as Profile;
            MessageBox.Show("This button will activate selected profile: " + selProf.Name);

            using (DataClasses1DataContext DB = new DataClasses1DataContext())
            {
                var selProfMain = profileListMain.SelectedItem as Profile;
                var queryMain = from t1 in DB.Program_desc
                                join t2 in DB.Program on t1.Id_prog equals t2.Id_prog
                                join t3 in DB.Profile on t1.Id_prof equals t3.Id_prof
                                where t3.Name == selProfMain.Name
                                select new{t2, t1};
                List<Program> procdescListMain = new List<Program>();
                foreach (var q in queryMain)
                {
                    try
                    {
                        MessageBox.Show("Starting: " + q.t2.Path + q.t1.Status);
                        Process processEditor = Process.Start(q.t2.Path);
                        processEditor.WaitForInputIdle();

                        int left = 100;
                        int top = 100;
                        int width = 400;
                        int height = 450;
                        
                        SetWindowPos(processEditor.MainWindowHandle,
                            HWND_TOP,
                            left, top,
                            width, height, 0);
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
        }

        // HWND Constants
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        // P/Invoke
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
           int Y, int cx, int cy, uint uFlags);
    }
}
