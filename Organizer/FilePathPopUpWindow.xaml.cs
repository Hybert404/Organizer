using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using Organizer.Resources;


namespace Organizer
{
    /// <summary>
    /// Interaction logic for FilePathPopUpWindow.xaml
    /// </summary>
    public partial class FilePathPopUpWindow : Window
    {
        //create variable which will hold passed value of selected profile
        Profile passProf;
        public FilePathPopUpWindow(Profile _passProf)
        {
            InitializeComponent();
            //declare global variable of passed over profile
            this.passProf = _passProf;
        }

        private void BttnAddFilePathOK_Click(object sender, RoutedEventArgs e)
        {
            //same viod as in ProfilesManagerPage, but changed for this need
            if (passProf != null && FileName.Text != string.Empty && FilePath.Text != string.Empty)
            {
                using (DataClasses1DataContext DB = new DataClasses1DataContext())
                {
                    // check if program is already in database, if not, insert it as new row
                    if (!DB.Program.Any(prog => prog.Name == FileName.Text))
                    {
                        Program program = new Program
                        {
                            Path = FilePath.Text,
                            Name = FileName.Text
                        };
                        DB.Program.InsertOnSubmit(program);
                        DB.SubmitChanges();
                    }
                }

                using (DataClasses1DataContext DB = new DataClasses1DataContext())
                {
                    var progFromQuery = from x in DB.Program
                                        where x.Name == FileName.Text
                                        select x;
                    var resultProg = new Program();
                    foreach (var s in progFromQuery)
                    {
                        resultProg = s;
                    }

                    var selProfile = passProf as Profile;
                    Program_desc programdesc = new Program_desc
                    {
                        Id_prof = selProfile.Id_prof,
                        Id_prog = resultProg.Id_prog,
                        Status = StatusComboBox.SelectedValue.ToString()
                    };
                    DB.Program_desc.InsertOnSubmit(programdesc);
                    DB.SubmitChanges();
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Please fill the fields", "Error");
            }
            
        }

        private void BttnAddFilePathCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
