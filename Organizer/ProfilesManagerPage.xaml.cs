using Organizer.Resources;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System;
using System.Collections.ObjectModel;
//using System.Runtime.InteropServices;

namespace Organizer
{
    /// <summary>
    /// Interaction logic for ProfilesManagerPage.xaml
    /// </summary>
    public partial class ProfilesManagerPage : Page
    {
        public ObservableCollection<Process> ProcessList { get; set; }
        public ObservableCollection<Process> ListGroupToSave { get; set; }
        public ProfilesManagerPage()
        {
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
        public string Status()
        {
            if (Minima.IsChecked == false & Maxima.IsChecked == false)
            {
                string s = "Normal";
                return s;
            }
            else if (Minima.IsChecked == true & Maxima.IsChecked == false)
            {
                string s = "Minimalized";
                return s;
            }
            else 
            {
                string s = "Maximalized";
                return s;
            }
            
        }
        public void GetApplications()
        {
            //List<Process> procList = new List<Process>();
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
                    //MessageBox.Show("New profile named " + NewProfileTextBox.Text + " added.");
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
            //System.Collections.ObjectModel.ObservableCollection<Process> procList = new System.Collections.ObjectModel.ObservableCollection<Process>();
            ListGroupToSave.Clear();
            foreach (Process p in listGrupaToSave.Items)
            {
                ListGroupToSave.Add(p);
            }

            {
                var s = processList.SelectedItem as Process;
                ListGroupToSave.Add(s);
            }

            //listGrupaToSave.ItemsSource = ListGroupToSave;
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
                            Status = Status()
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

        private void BttnlistPrograms_Click(object sender, RoutedEventArgs e)
        {
            using (DataClasses1DataContext DB = new DataClasses1DataContext())
            {
                foreach (Program pr in DB.Program)
                {
                    MessageBox.Show(pr.Name);
                }
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
                                select t2;
                    List<Program> procdescList = new List<Program>();
                    foreach (var q in query)
                    {
                        procdescList.Add(q);
                    }
                    
                    listGrupa.ItemsSource = procdescList;
                }
            }
            //var selectedProfile = profileList.SelectedItem as Profile;
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
                //ObservableCollection<Process> processes = new System.Collections.ObjectModel.ObservableCollection<Process>(lbSource.ItemsSource);
                ListGroupToSave.Add(lbSource.SelectedItem as Process);
            }
        }
    }

}
