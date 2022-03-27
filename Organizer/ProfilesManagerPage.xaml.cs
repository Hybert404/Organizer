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
            if (MessageBox.Show("Czy na pewno chcesz dodać nowy profil?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Save_profile(NewProfileTextBox.Text);
                MessageBox.Show("Dodano nowy profil o nazwie " + NewProfileTextBox.Text);
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
