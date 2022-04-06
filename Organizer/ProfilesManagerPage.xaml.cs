using Organizer.Resources;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace Organizer
{
    /// <summary>
    /// Interaction logic for ProfilesManagerPage.xaml
    /// </summary>
    public partial class ProfilesManagerPage : Page
    {
        public ProfilesManagerPage()
        {
            InitializeComponent();
            listProfiles();
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
            processList.ItemsSource = procList;
        }

        private void Bttc_Click_Add_profile(object sender, RoutedEventArgs e)
        {
            if (NewProfileTextBox.Text.Length > 0)
            {
                if (MessageBox.Show("Do you want to add a new profile?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Save_profile(NewProfileTextBox.Text);
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
                    MessageBox.Show("New profile named " + NewProfileTextBox.Text + " added.");
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
            List<Process> procList = new List<Process>();
            foreach (Process p in listGrupa.Items)
            {
                procList.Add(p);
            }

            {
                var s = processList.SelectedItem as Process;
                procList.Add(s);
            }

            listGrupa.ItemsSource = procList;
        }

        private void BttnSaveToProf_Click(object sender, RoutedEventArgs e)
        {
            if (profileList.SelectedItem != null)
            {
                foreach (Process proc in listGrupa.Items)
                {
                    //try
                    //{
                    //    //Process.Start(p.ProcessName);
                    //    DebugBox.Items.Add(p.MainModule.FileName);

                    //}
                    //catch { }
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
                foreach (Process proc in listGrupa.Items)
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
                        //MessageBox.Show("Zapisano nowy program "+proc.ProcessName);

                        //foreach (Program_desc test in DB.Program_desc)
                        //{
                        //    try
                        //    {
                        //        MessageBox.Show(test.Id_prof + " " + test.Id_prog);
                        //    }
                        //    catch { }
                        //}
                    }

                }
                //listGrupa.ItemsSource = null;
            }
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

        private void profileSelectionChange(object sender, SelectionChangedEventArgs e)
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
        }

        private void Max_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void Min_Checked(object sender, RoutedEventArgs e)
        {

        }
    }

}
