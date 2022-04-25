using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<DigitalCurrency> digitalCurrencyList = new List<DigitalCurrency>();
        private List<PhysicalCurrency> physicalCurrencyList = new List<PhysicalCurrency>();
        private DataTable dt = new DataTable();
        public SeriesCollection SeriesCollection { get; set; }
        public MainWindow()
        {
            SeriesCollection = new SeriesCollection();
            DataContext = this;
            InitializeComponent();
            ReadDigitalCurrency();
            ReadPhysicalCurrency();
            FillComboBoxes();
            DrawGraph_Example();
            
        }

        private void FillComboBoxes()
        {
            foreach (DigitalCurrency digitalCurrency in digitalCurrencyList)
            {
                valutaCb.Items.Add(digitalCurrency.ToString());
            }
            foreach (PhysicalCurrency physicalCurrency in physicalCurrencyList)
            {
                trzisteCb.Items.Add(physicalCurrency.ToString());
            }
        }
        private void ReadDigitalCurrency()
        {
            StreamReader sr = new StreamReader("../../../digital_currency_list.csv");
            string line = sr.ReadLine(); // ignore first line
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                var data = line.Split(',');
                DigitalCurrency digitalCurrency = new DigitalCurrency
                {
                    CurrencyCode = data[0],
                    CurrencyName = data[1]
                };
                digitalCurrencyList.Add(digitalCurrency);
            }
            sr.Close();
        }
        private void ReadPhysicalCurrency()
        {
            StreamReader sr = new StreamReader("../../../physical_currency_list.csv");
            string line = sr.ReadLine(); // ignore first line
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                var data = line.Split(',');
                PhysicalCurrency physicalCurrency = new PhysicalCurrency
                {
                    CurrencyCode = data[0],
                    CurrencyName = data[1]
                };
                physicalCurrencyList.Add(physicalCurrency);
            }
            sr.Close();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            APIAnswer answer = GetAnswer();
            if (answer != null)
            {
                answer.SortAnswers();
                DrawGraph(answer);
                FillTable(answer);
            }
        }
        private void DrawGraph_Example()
        {
            ChartValues<double> values = new ChartValues<double> { 3, 5, 7, 9 };

            var lineSeries1 = new LineSeries()
            {
                Title = "Ime",
                Fill = Brushes.Transparent,
                Values = values,
                PointGeometrySize = 5
            };

            SeriesCollection.Clear();
            SeriesCollection.Add(lineSeries1);
            dataChart.Update();
        }

        private void DrawGraph(APIAnswer answer)
        {
            SeriesCollection.Clear();
            var lineSeries1 = new LineSeries
            {
                Title = "Open",
                Values = answer.GetOpenFirstValues(),
                Stroke = Brushes.Green,
                Fill = Brushes.Transparent,
                Focusable = true
            };
            var lineSeries2 = new LineSeries
            {
                Title = "Close",
                Values = answer.GetClosedFirstValues(),
                Stroke = Brushes.Yellow,
                Fill = Brushes.Transparent,
                Focusable = true
            };
            var lineSeries3 = new LineSeries
            {
                Title = "Low",
                Values = answer.GetLowFirstValues(),
                DataLabels = true,
                Stroke = Brushes.Red,
                Fill = Brushes.Transparent,
                ScalesYAt = 1,
                Focusable = true
            };
            var lineSeries4 = new LineSeries
            {
                Title = "High",
                Values = answer.GetHighFirstValues(),
                DataLabels = true,
                Stroke = Brushes.Blue,
                Fill = Brushes.Transparent,
                ScalesYAt = 1,
                Focusable = true
            };

            SeriesCollection.Add(lineSeries1);
            SeriesCollection.Add(lineSeries2);
            SeriesCollection.Add(lineSeries3);
            SeriesCollection.Add(lineSeries4);


            //dataChart.AxisX.Clear();
            //dataChart.AxisY.Clear();
            //dataChart.AxisY.Add(new Axis());
            //dataChart.AxisX.Add(new Axis());

            //dataChart.Series = SeriesCollection;
        }

        private void FillTable(APIAnswer answer)
        {
            dt.Clear();
            dt.Columns.Clear();

            dt.Columns.Add("Timestamp");
            dt.Columns.Add(answer.Vrsta);
            dt.Columns.Add("Volume");

            foreach (MarketCapInfo mci in answer.marketCapInfos)
            {
                DataRow dr = dt.NewRow();
                dr["Timestamp"] = mci.Timestamp.ToString();
                switch (answer.Vrsta)
                {
                    case "Open": dr[answer.Vrsta] = mci.OpenFirstCurrency.ToString(); break;
                    case "Close": dr[answer.Vrsta] = mci.CloseFirstCurrency.ToString(); break;
                    case "Low": dr[answer.Vrsta] = mci.LowFirstCurrency.ToString(); break;
                    case "High": dr[answer.Vrsta] = mci.HighFirstCurrency.ToString(); break;
                    default: break;
                }
                dr["Volume"] = mci.Volume.ToString();
                dt.Rows.Add(dr);
            }
            dataGrid.ItemsSource = dt.DefaultView;
            dataGrid.IsReadOnly = true;
        }

        private APIAnswer GetAnswer()
        {
            try
            {
                string pcstring = (string)trzisteCb.SelectedItem;
                PhysicalCurrency pc = new PhysicalCurrency(pcstring);
                string dcstring = (string)valutaCb.SelectedItem;
                DigitalCurrency dc = new DigitalCurrency(dcstring);
                
                // TODO: NULL CHECK
                string function = ((ComboBoxItem)intervalCb.SelectedItem).Content.ToString();
                switch (function)
                {
                    case "U jednom danu": function = "CRYPTO_INTRADAY"; break;
                    case "Dnevno": function = "DIGITAL_CURRENCY_DAILY"; break;
                    case "Nedeljno": function = "DIGITAL_CURRENCY_WEEKLY"; break;
                    case "Mesečno": function = "DIGITAL_CURRENCY_MONTHLY"; break;
                    default: Console.WriteLine("Interval mora biti selektovan."); return null;
                }
                string interval = "";

                // TODO: NULL CHECK 
                if (function == "CRYPTO_INTRADAY")
                {
                    interval = ((ComboBoxItem)vremelCb.SelectedItem).Content.ToString();
                    interval = interval.Split(" ")[0];
                    interval += "min";
                }
                // TODO: PROPER ERROR POP-UP
                if (interval == "" && function == "CRYPTO_INTRADAY") { Console.WriteLine("Vreme mora biti selektovano za Interval 'u jednom danu'."); return null; }

                string outputsize = "";
                Service service = new Service();

                // return value
                APIAnswer answer = service.PullData(function, dc, pc, interval, outputsize);
                answer.IsIntraday = function == "CRYPTO_INTRADAY";
                answer.Vrsta = ((ComboBoxItem)vrstaCb.SelectedItem).Content.ToString();
                return answer;
               
            }
            catch (Exception ex)
            {
                // TODO: PROPER ERROR POP-UP + (DRUGA PORUKA)
                Console.WriteLine("Valuta i Trziste moraju biti selektovani");
                return null;
            }
        }
    }
}
