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
        }

    }
}
