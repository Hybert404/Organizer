using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Organizer.Resources;
using System.Collections.ObjectModel;
using Organizer.Services;
using Organizer.Enums;
using System.Windows.Controls.DataVisualization.Charting;

namespace Organizer
{
    /// <summary>
    /// Interaction logic for StatsPage.xaml
    /// </summary>
    public partial class StatsPage : Page
    {
        private DbService _dbService;
        private PlotCollectionType _currentType;
        private int _selectedProfileId;
        private int _selectedProgramId;
        private string _selectedItemName;
        private int _refreshItemId;

        public PlotCollection ChartData { get; set; }

        public StatsPage()
        {
            _dbService = new DbService();
            ChartData = new PlotCollection();

            InitializeComponent();

            dp_chartDateFrom.Text = DateTime.Now.Date.ToShortDateString();
            dp_chartDateTo.Text = DateTime.Now.Date.ToShortDateString();

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
                var selProg = programsList.SelectedItem as Program;
                _currentType = PlotCollectionType.Program;
                _selectedProgramId = selProg.Id_prog;
                _selectedItemName = selProg.Name;
                _refreshItemId = _selectedProgramId;
                loadChart(_currentType, _selectedProgramId, _selectedItemName);

                var dateFrom = DateTime.Parse(dp_chartDateFrom.Text).Date;
                var dateTo = DateTime.Parse(dp_chartDateTo.Text).Date.AddHours(24).AddSeconds(-1);
                statsList.ItemsSource = _dbService.GetProgramTimeListByProgramId(selProg.Id_prog, dateFrom, dateTo);
            }
        }

        private void profileSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (profilesList.SelectedItem != null)
            {
                var selProf = profilesList.SelectedItem as Profile;
                _currentType = PlotCollectionType.Profile;
                _selectedProfileId = selProf.Id_prof;
                _selectedItemName = selProf.Name;
                _refreshItemId = _selectedProfileId;
                loadChart(_currentType, _selectedProfileId, _selectedItemName);

                var dateFrom = DateTime.Parse(dp_chartDateFrom.Text).Date;
                var dateTo = DateTime.Parse(dp_chartDateTo.Text).Date.AddHours(24).AddSeconds(-1);
                statsList.ItemsSource = _dbService.GetProfileTimeListByProfileId(selProf.Id_prof, dateFrom, dateTo);
            }
        }

        private void loadChart(PlotCollectionType type, int itemId, string itemName)
        {
            var dateFrom = DateTime.Parse(dp_chartDateFrom.Text).Date;
            var dateTo = DateTime.Parse(dp_chartDateTo.Text).Date.AddHours(24).AddSeconds(-1);
            ChartData.LoadPlotCollection(type, itemId, dateFrom, dateTo);
            var columnSerie = (ColumnSeries)runTimeChart.Series[0];
            columnSerie.Title = $"{_selectedItemName} {type} Time Chart(sec)";
            columnSerie.ItemsSource = ChartData;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            loadChart(_currentType, _refreshItemId, _selectedItemName);
        }
    }

    public partial class PlotElement
    {
        public string Name { get; set; }
        public Int64 Time { get; set; }
    }
    public class PlotCollection : ObservableCollection<PlotElement>
    {
        private DbService _dbService;
        public PlotCollection()
        {
            _dbService = new DbService();
        }

        public void LoadPlotCollection(PlotCollectionType type, int id, DateTime startDate, DateTime endDate)
        {
            Clear();
            var data = type == PlotCollectionType.Program ?
                _dbService.GetProgramTimeInfoById(id, startDate, endDate):
                _dbService.GetProfileTimeInfoById(id, startDate, endDate);

            if(startDate.Date == endDate.Date)
            {
                for(int hour=0; hour < 24; hour++)
                {
                    var totalTimeSec = 0;
                    foreach(var row in data.Where(d=>d.StartTime.Value.Hour == hour || d.StopTime.Value.Hour == hour))
                    {
                        var stopDateTime = GetStopDateTime(row.StopTime.Value, hour, UnitType.Hours);
                        var startDateTime = GetStartDateTime(row.StartTime.Value, hour, UnitType.Hours);
                        totalTimeSec += (int)(stopDateTime - startDateTime).TotalSeconds;
                    }

                    var name = $"0{hour}:00";
                    name = name.Substring(name.Length - 5, 5);
                    Add(new PlotElement()
                    {
                        Name = name,
                        Time = totalTimeSec
                    });
                }
            }
            else{
                for(var iter = 0; iter < (endDate - startDate).TotalDays; iter++)
                {
                    var day = startDate.AddDays(iter);
                    var rows = data.Where(d => d.StartTime.Value.Day == day.Day);
                    var totalTime = 0;
                    foreach(var row in rows)
                    {
                        totalTime += (int)(row.StopTime.Value - row.StartTime.Value).TotalSeconds;
                    };

                    var dayString = $"0{day.Day}";
                    dayString = dayString.Substring(dayString.Length-2,2);
                    var monthString = $"0{day.Month}";
                    monthString = monthString.Substring(monthString.Length - 2, 2);

                    Add(new PlotElement()
                    {
                        Name = $"{dayString}-{monthString}",
                        Time = totalTime
                    });
                }
            }
        }

        private DateTime GetStopDateTime(DateTime stopDate, int hour, UnitType unit)
        {
            if (stopDate.Hour > hour)
            {
                return stopDate.Date.AddHours(hour + 1).AddSeconds(-1);
            }

            return stopDate;
        }

        private DateTime GetStartDateTime(DateTime startDate, int hour, UnitType unit)
        {
            if (startDate.Hour < hour)
            {
                return startDate.Date;
            }

            return startDate;
        }

        private string GetDayLabel(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd");
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

