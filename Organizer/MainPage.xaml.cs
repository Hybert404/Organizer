using Organizer.Resources;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using Organizer.Services;

namespace Organizer
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>

    public partial class MainPage : Page
    {
        private DbService _dbService;
        private List<Program> _currentProfilePrograms;
        public ObservableCollection<Process> ProcessList { get; set; }
       
        public MainPage()
        {
            _dbService = new DbService();
            InitializeComponent();
            listProfilesMain();
            ProcessList = new ObservableCollection<Process>();
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
                if ((profileListMain.SelectedItem as Profile).Name == CurrentProfile)
                    BttnActivateProfile.Content = "Deactivate Profile";
                else
                    BttnActivateProfile.Content = "Activate Profile";

                var selProf = profileListMain.SelectedItem as Profile;
                _currentProfilePrograms = _dbService.GetProgramListByProfileName(selProf.Name);

                programList.ItemsSource = _currentProfilePrograms;
            }
        }

        private void BttnActivateProfile_Click(object sender, RoutedEventArgs e)
        {
            var selProf = profileListMain.SelectedItem as Profile;
            if (CurrentProfile == null)
            {
                CurrentProfile = selProf.Name;
                ActivateProfile();
                BttnActivateProfile.Content = "Deactivate Profile";
            }
            else if(CurrentProfile == selProf.Name)
            {
                DeactivateProfile();
                BttnActivateProfile.Content = "Activate Profile";
            }
            else
            {
                MessageBox.Show("Stopping profile: " + CurrentProfile + ", and activating: " + selProf.Name);
                CurrentProfile = selProf.Name;
                ActivateProfile();
                this.BttnActivateProfile.Content = "Deactivate Profile";
            }
        }

        public void ActivateProfile()
        {
            using (DataClasses1DataContext DB = new DataClasses1DataContext())
            {
                var selProfMain = profileListMain.SelectedItem as Profile;
                List<Program> procdescListMain = new List<Program>();

                foreach (var q in _currentProfilePrograms)
                {
                    try
                    {
                        Process processEditor = Process.Start(q.Path);
                        processEditor.WaitForInputIdle();

                        var query = from t1 in DB.Program_desc
                                    join t2 in DB.Program on t1.Id_prog equals t2.Id_prog
                                    join t3 in DB.Profile on t1.Id_prof equals t3.Id_prof
                                    where t3.Name == selProfMain.Name
                                    && t2.Name == q.Name
                                    select t1;

                        int left = query.First().X ?? 0;
                        int top = query.First().Y ?? 0;
                        int width = query.First().Width ?? 0;
                        int height = query.First().Height ?? 0;

                        Thread.Sleep(2000);
                        SetWindowPos(processEditor.MainWindowHandle,
                            HWND_TOP,
                            left, top,
                            width, height, 0);

                        TimeStartProfile = DateTime.Now;

                        string trim = q.Name.Replace(" ", "");

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
                // save apps runtime
                TimeEndProfile = DateTime.Now;
                TimeSpan diff = TimeEndProfile - TimeStartProfile;

                var queryTimeApps = from t1 in DB.Program_desc
                                    join t2 in DB.Program on t1.Id_prog equals t2.Id_prog
                                    join t3 in DB.Profile on t1.Id_prof equals t3.Id_prof
                                    where t3.Name == CurrentProfile
                                    select new { t2, t1 };

                foreach (var q in queryTimeApps)
                {
                    try
                    {
                        Time_program tp = new Time_program();
                        tp.Id_prog = q.t2.Id_prog;
                        tp.Time_start = TimeStartProfile;
                        tp.Time_stop = TimeEndProfile;
                        DB.Time_program.InsertOnSubmit(tp);
                        DB.SubmitChanges();

                        this.DebugBox.Items.Add($"Added: {q.t2.Name} opened for {diff.Seconds} seconds");
                    }
                    catch { }
                }


                // save profile runtime
                var queryTimeProfile = from t1 in DB.Profile
                                       where t1.Name == CurrentProfile
                                       select t1;

                foreach (var q in queryTimeProfile)
                {
                    try
                    {
                        Time_profile tp = new Time_profile();
                        tp.Id_prof = q.Id_prof;
                        tp.Time_start = TimeStartProfile;
                        tp.Time_stop = TimeEndProfile;
                        DB.Time_profile.InsertOnSubmit(tp);
                        DB.SubmitChanges();

                        this.DebugBox.Items.Add($"Added profile: {q.Name} opened for {diff.Seconds} seconds");
                    }
                    catch { }
                }
            }
            CurrentProfile = null;
        }

        private void Process_Exited(object sender, System.EventArgs e)
        {
            //MessageBox.Show("process exited");
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
