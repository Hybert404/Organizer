using Organizer.Resources;
using Organizer.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System;
using System.Collections.ObjectModel;

namespace Organizer
{
    /// <summary>
    /// Interaction logic for ProfilesManagerPage.xaml
    /// </summary>
    public partial class ProfilesManagerPage : Page
    {
        private DbService _dbService;
        public ObservableCollection<Process> ProcessList { get; set; }
        public ObservableCollection<Process> ListGroupToSave { get; set; }
        public ProfilesManagerPage()
        {
            _dbService = new DbService();
            ProcessList = new ObservableCollection<Process>();
            ListGroupToSave = new ObservableCollection<Process>();
            InitializeComponent();
            listProfiles();
            listGrupaToSave.ItemsSource = ListGroupToSave;
        }

        //Listowanie aplikacji
        private void ListProcBttn_Click(object sender, RoutedEventArgs e)
        {
            GetApplications();
        }
        void Program_desc_create(Process p)
        {
            try
            {
                var s = p.MainModule.FileName;
                ProcessStartInfo startInfo = new ProcessStartInfo(s);
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                Process.Start(startInfo);
            }
            catch { }

        }
        //public string Status()
        //{
        //    if (Minima.IsChecked == false & Maxima.IsChecked == false)
        //    {
        //        string s = "Normal";
        //        return s;
        //    }
        //    else if (Minima.IsChecked == true & Maxima.IsChecked == false)
        //    {
        //        string s = "Minimalized";
        //        return s;
        //    }
        //    else 
        //    {
        //        string s = "Maximalized";
        //        return s;
        //    }
            
        //}
        public void GetApplications()
        {
            ProcessList.Clear();
            foreach (Process p in Process.GetProcesses("."))
            {
                try
                {
                    if (p.MainWindowTitle.Length > 0)
                    {
                        ProcessList.Add(p);
                    }
                }
                catch { }
            }
            processList.ItemsSource = ProcessList;// procList;
        }

        private void Bttc_Click_Add_profile(object sender, RoutedEventArgs e)
        {
            if (NewProfileTextBox.Text.Length > 0)
            {
                if (MessageBox.Show("Do you want to add a new profile?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Save_profile(NewProfileTextBox.Text);
                    NewProfileTextBox.Clear();
                }
            }
            else
            {
                MessageBox.Show("Please enter a profile name");
            }
        }
        private void Bttc_Click_Delete_profile(object sender, RoutedEventArgs e)
        {
            using (DataClasses1DataContext DB = new DataClasses1DataContext())
            {
                if (profileList.SelectedItem != null)
                {
                    var selProfile = profileList.SelectedItem as Profile;
                    var prof = _dbService.GetProfileByName(selProfile.Name);
                    var profTimes = _dbService.GetAllProfileTimeListByProfileId(prof.Id_prof);

                    foreach(var p in profTimes)
                    {
                        DB.Time_profile.DeleteOnSubmit(p);
                        //DB.SubmitChanges();
                    }

                    var profProgramDesc = _dbService.GetProgramDescByProfileId(prof.Id_prof);
                    foreach (var p in profProgramDesc)
                    {
                        DB.Program_desc.DeleteOnSubmit(p);
                        //DB.SubmitChanges();
                    }


                    var deleteProfileQuery = from x in DB.Profile
                                      where x.Name == selProfile.Name
                                      select x;
                    MessageBox.Show(selProfile.Name);
                    foreach(var q in deleteProfileQuery)
                    {
                        DB.Profile.DeleteOnSubmit(q);
                        //DB.SubmitChanges();
                    }
                    DB.SubmitChanges();
                }
            }
            listProfiles();
        }

        void Save_profile(string nazwa)
        {
            using (DataClasses1DataContext DB = new DataClasses1DataContext())
            {
                if (!DB.Profile.Any(prof => prof.Name == nazwa))
                {
                    Profile p = new Profile();
                    p.Name = nazwa;
                    DB.Profile.InsertOnSubmit(p);
                    DB.SubmitChanges();
                }
                else
                {
                    MessageBox.Show("Profile named " + nazwa + " already exist", "Error");
                }
            }
            listProfiles();
        }

        //Zapisywanie grupy do profilu
        private void Przesun_Click(object sender, RoutedEventArgs e)
        {
            if (processList.SelectedItem != null)
            {
                //ListGroupToSave.Clear();
                //foreach (Process p in listGrupaToSave.Items)
                //{
                //    ListGroupToSave.Add(p);
                //}

                //{
                //    var s = processList.SelectedItem as Process;
                //    ListGroupToSave.Add(s);
                //}
                ListGroupToSave.Add(processList.SelectedItem as Process);
            }
            
        }

        private void BttnSaveToProf_Click(object sender, RoutedEventArgs e)
        {
            if (profileList.SelectedItem != null)
            {
                foreach (Process proc in listGrupaToSave.Items)
                {
                    using (DataClasses1DataContext DB = new DataClasses1DataContext())
                    {
                        // check if program is already in database, if not, insert it as new row
                        if (!DB.Program.Any(prog => prog.Name == proc.ProcessName))
                        {
                            Program program = new Program
                            {
                                Path = proc.MainModule.FileName,
                                Name = proc.ProcessName
                            };
                            DB.Program.InsertOnSubmit(program);
                            DB.SubmitChanges();
                        }
                    }
                }
                foreach (Process proc in listGrupaToSave.Items)
                {
                    using (DataClasses1DataContext DB = new DataClasses1DataContext())
                    {
                        var progFromQuery = from x in DB.Program
                                            where x.Name == proc.ProcessName
                                            select x;
                        var resultProg = new Program();
                        foreach (var s in progFromQuery)
                        {
                            resultProg = s;
                        }

                        var selProfile = profileList.SelectedItem as Profile;
                        Program_desc programdesc = new Program_desc
                        {
                            Id_prof = selProfile.Id_prof,
                            Id_prog = resultProg.Id_prog,
                            Status = "Maximized",
                            X = 500,
                            Y = 500,
                            Height = 500,
                            Width = 500
                        };
                        DB.Program_desc.InsertOnSubmit(programdesc);
                        DB.SubmitChanges();
                    }

                }
                ListGroupToSave.Clear();
            }
            listProfiles();
        }

        void listProfiles()
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
                profileList.ItemsSource = profList;
            }
        }

        public void profileSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (profileList.SelectedItem != null)
            {
                using (DataClasses1DataContext DB = new DataClasses1DataContext())
                {
                    var selProf = profileList.SelectedItem as Profile;
                    var query = from t1 in DB.Program_desc
                                join t2 in DB.Program on t1.Id_prog equals t2.Id_prog
                                join t3 in DB.Profile on t1.Id_prof equals t3.Id_prof
                                where t3.Name == selProf.Name
                                select t1;
                    // tymczasowa zmiana z t2 na t1, program na program_desc
                    List<Program_desc> procdescList = new List<Program_desc>();
                    foreach (var q in query)
                    {
                        procdescList.Add(q);
                    }
                    
                    listGrupa.ItemsSource = procdescList;
                }
            }
        }

        private IntPtr FindWindow(string title, int index)
        {
            List<Process> l = new List<Process>();

            Process[] tempProcesses;
            tempProcesses = Process.GetProcesses();
            foreach (Process proc in tempProcesses)
            {
                if (proc.MainWindowTitle == title)
                {
                    l.Add(proc);
                }
            }

            if (l.Count > index) return l[index].MainWindowHandle;
            return (IntPtr)0;
        }

        private void BttnAddPathManual_Click(object sender, RoutedEventArgs e)
        {
            if (profileList.SelectedItem != null)
            {
                FilePathPopUpWindow popup = new FilePathPopUpWindow(profileList.SelectedItem as Profile);
                popup.Show();
                popup.Activate();
                popup.Focus();
                popup.Topmost = true;
            }
            else
            {
                MessageBox.Show("Select a profile first", "Error");
            }
        }

        private void processList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(sender is ListBox lbSource && lbSource.SelectedItem != null)
            {
                ListGroupToSave.Add(lbSource.SelectedItem as Process);
            }
        }

    }

    // Watermark for textboxes
    public class TextInputToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Always test MultiValueConverter inputs for non-null
            // (to avoid crash bugs for views in the designer)
            if (values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool)values[0];
                bool hasFocus = (bool)values[1];

                if (hasFocus || hasText)
                    return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}