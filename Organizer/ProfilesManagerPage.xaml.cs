using Organizer.Resources;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

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
                    MessageBox.Show("New profile named " + NewProfileTextBox.Text + " added.");
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
                Profile p = new Profile();
                p.Name = nazwa;
                DB.Profiles.InsertOnSubmit(p);
                DB.SubmitChanges();
            }
            listProfiles();
        }

        //Zapisywanie grupy do profilu
        private void Przesun_Click(object sender, RoutedEventArgs e)
        {
            List<Process> procList = new List<Process>();
            foreach (Process p in listwyb.Items)
            {
                procList.Add(p);
            }

            {
                var s = processList.SelectedItem as Process;
                procList.Add(s);
            }

            listwyb.ItemsSource = procList;
        }

        private void BttnSaveToProf_Click(object sender, RoutedEventArgs e)
        {
            foreach (Process p in listwyb.Items)
            {
                //try
                //{
                //    //Process.Start(p.ProcessName);
                //    DebugBox.Items.Add(p.MainModule.FileName);

                //}
                //catch { }
                using (DataClasses1DataContext DB = new DataClasses1DataContext())
                {
                    Program pr = new Program
                    {
                        Path = p.MainModule.FileName,
                        Name = p.ProcessName
                    };
                    DB.Programs.InsertOnSubmit(pr);
                    DB.SubmitChanges();
                }
            }
            listwyb.ItemsSource = null;
        }

        void listProfiles()
        {
            using (DataClasses1DataContext DB = new DataClasses1DataContext())
            {
                List<Profile> profList = new List<Profile>();
                foreach (Profile prof in DB.Profiles)
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
                foreach (Program pr in DB.Programs)
                {
                    MessageBox.Show(pr.Name);
                }
            }
        }
    }
}
