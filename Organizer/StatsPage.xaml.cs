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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Organizer.Resources;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Controls.DataVisualization.Charting;

namespace Organizer
{
    /// <summary>
    /// Interaction logic for StatsPage.xaml
    /// </summary>
    public partial class StatsPage : Page
    {
        public StatsPage()
        {
            InitializeComponent();
            listPrograms();
            listProfiles();
        }

        void listPrograms()
        {
            using (DataClasses1DataContext DB = new DataClasses1DataContext())
            {
                List<Program> progList = new List<Program>();
                foreach (Program p in DB.Program)
                {
                    try
                    {
                        progList.Add(p);
                    }
                    catch { }
                }
                programsList.ItemsSource = progList;
            }
        }
        void listProfiles()
        {
            using (DataClasses1DataContext DB = new DataClasses1DataContext())
            {
                List<Profile> profList = new List<Profile>();
                foreach (Profile p in DB.Profile)
                {
                    try
                    {
                        profList.Add(p);
                    }
                    catch { }
                }
                profilesList.ItemsSource = profList;
            }
        }
        public void programSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (programsList.SelectedItem != null)
            {
                using (DataClasses1DataContext DB = new DataClasses1DataContext())
                {
                    var selProg = programsList.SelectedItem as Program;
                    var query = from t1 in DB.Time_program
                                join t2 in DB.Program on t1.Id_prog equals t2.Id_prog
                                where t1.Id_prog == selProg.Id_prog
                                select new { t1, t2 };
                    List<Time_program> times = new List<Time_program>();

                    foreach (var q in query)
                    {
                        times.Add(q.t1);
                        TimeSpan diff = (TimeSpan)(q.t1.Time_stop - q.t1.Time_start);
                        var time = diff.Hours * 3600 + diff.Minutes * 60 + diff.Seconds;
                    }

                    statsList.ItemsSource = times;
                }
            }
        }

        private void profileSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (profilesList.SelectedItem != null)
            {
                using (DataClasses1DataContext DB = new DataClasses1DataContext())
                {
                    var selProf = profilesList.SelectedItem as Profile;
                    var query = from t1 in DB.Time_profile
                                where t1.Id_prof == selProf.Id_prof
                                select t1;
                    List<Time_profile> times = new List<Time_profile>();
                    foreach (var q in query)
                    {
                        times.Add(q);
                    }

                    statsList.ItemsSource = times;
                }
            }
        }
    }

    public partial class PlotElement
    {
        public string Name { get; set; }
        public Int64 Time { get; set; }
    }
    public class PlotCollection : System.Collections.ObjectModel.Collection<PlotElement>
    {
        public PlotCollection()
        {
            using (DataClasses1DataContext DB = new DataClasses1DataContext())
            {
                //var selProg = programsList.SelectedItem as Program;

                var query = from t1 in DB.Time_program
                            join t2 in DB.Program on t1.Id_prog equals t2.Id_prog
                            select new { t1, t2 };

                var dict = new DefaultDictionary<string, int>();

                foreach (var q in query)
                {
                    TimeSpan diff = (TimeSpan)(q.t1.Time_stop - q.t1.Time_start);
                    var time = (int)diff.TotalSeconds;
                    string trim = q.t2.Name.Replace(" ", "");
                    dict[trim] += time;
                }
                foreach (var x in dict)
                {
                    Add(new PlotElement { Name = x.Key, Time = (short)x.Value });
                }

            }
        }
    }
    public class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TValue : new()
    {
        public new TValue this[TKey key]
        {
            get
            {
                TValue val;
                if (!TryGetValue(key, out val))
                {
                    val = new TValue();
                    Add(key, val);
                }
                return val;
            }
            set { base[key] = value; }
        }
    }



}

