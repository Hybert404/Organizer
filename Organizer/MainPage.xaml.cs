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
                foreach (Profile prof in DB.Profiles)
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
        }
    }
}
