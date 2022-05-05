using Organizer.Resources;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Organizer
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>

    //public struct AppTimer
    //{
    //    public int Id_prog;
    //    public DateTime TimeStart;
    //    public DateTime TimeEnd;
    //    public AppTimer(int id_prog, DateTime endtime = default(DateTime))
    //    {
    //        Id_prog = id_prog;
    //        TimeStart = DateTime.Now;
    //        TimeEnd = endtime;
    //    }
    //}

    public partial class MainPage : Page
    {
        //public ObservableCollection<AppTimer> Timers { get; set; }
        public ObservableCollection<Process> ProcessList { get; set; }
        public MainPage()
        {
            InitializeComponent();
            listProfilesMain();
            ProcessList = new ObservableCollection<Process>();
            //Timers = new ObservableCollection<AppTimer>();
        }
        public string CurrentProfile = null;
        public DateTime TimeStartProfile;
        public DateTime TimeEndProfile;

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
        public void profileSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (profileListMain.SelectedItem != null)
            {
                using (DataClasses1DataContext DB = new DataClasses1DataContext())
                {
                    var selProf = profileListMain.SelectedItem as Profile;
                    var query = from t1 in DB.Program_desc
                                join t2 in DB.Program on t1.Id_prog equals t2.Id_prog
                                join t3 in DB.Profile on t1.Id_prof equals t3.Id_prof
                                where t3.Name == selProf.Name
                                select t2;
                    List<Program> procdescList = new List<Program>();
                    foreach (var q in query)
                    {
                        procdescList.Add(q);
                    }

                    programList.ItemsSource = procdescList;
                }
            }
        }

        private void BttnActivateProfile_Click(object sender, RoutedEventArgs e)
        {
            var selProf = profileListMain.SelectedItem as Profile;
            if (CurrentProfile == null)
            {
                CurrentProfile = selProf.Name;
                ActivateProfile();
            }
            else if(CurrentProfile == selProf.Name)
            {
                MessageBox.Show("Profile " + CurrentProfile + "is already activated");
            }
            else
            {
                MessageBox.Show("Stopping profile: " + CurrentProfile + ", and activating: " + selProf.Name);
                CurrentProfile = selProf.Name;
                ActivateProfile();
            }
        }

        public void ActivateProfile()
        {
            using (DataClasses1DataContext DB = new DataClasses1DataContext())
            {
                var selProfMain = profileListMain.SelectedItem as Profile;
                var queryMain = from t1 in DB.Program_desc
                                join t2 in DB.Program on t1.Id_prog equals t2.Id_prog
                                join t3 in DB.Profile on t1.Id_prof equals t3.Id_prof
                                where t3.Name == selProfMain.Name
                                select new { t2, t1 };
                List<Program> procdescListMain = new List<Program>();

                foreach (var q in queryMain)
                {
                    try
                    {
                        //MessageBox.Show("Starting: " + q.t2.Path + q.t1.Status);
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

                        TimeStartProfile = DateTime.Now;
                        //AppTimer at = new AppTimer(q.t2.Id_prog);
                        //Timers.Add(at);

                        //GetProcessByName(q.t2.Name);
                        string trim = q.t2.Name.Replace(" ", "");

                        foreach (var process in Process.GetProcessesByName(trim))
                        {
                            process.EnableRaisingEvents = true;
                            process.Exited += new EventHandler(Process_Exited);
                        }
                    }
                    catch { }
                }
            }
        }

        public void DeactivateProfile()
        {
            using (DataClasses1DataContext DB = new DataClasses1DataContext())
            {
                TimeEndProfile = DateTime.Now;
                TimeSpan diff = TimeEndProfile - TimeStartProfile;

                var queryMain = from t1 in DB.Program_desc
                                join t2 in DB.Program on t1.Id_prog equals t2.Id_prog
                                join t3 in DB.Profile on t1.Id_prof equals t3.Id_prof
                                where t3.Name == CurrentProfile
                                select new { t2, t1 };

                foreach (var q in queryMain)
                {
                    try
                    {
                        Time_program tp = new Time_program();
                        tp.Id_prog = q.t2.Id_prog;
                        tp.Time_start = TimeStartProfile;
                        tp.Time_stop = TimeEndProfile;
                        DB.Time_program.InsertOnSubmit(tp);
                        DB.SubmitChanges();

                        MessageBox.Show("Added: " + q.t2.Name + "opened for " + diff.Seconds + " seconds");
                    }
                    catch { }
                }
            }
            CurrentProfile = null;
        }

        private void Process_Exited(object sender, System.EventArgs e)
        {
            MessageBox.Show("process exited");
            if (CurrentProfile != null)
            {
                DeactivateProfile();
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
