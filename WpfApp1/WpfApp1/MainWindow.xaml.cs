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
        private ComboBoxItem cbi;
        public SeriesCollection SeriesCollection { get; set; }
        public MainWindow()
        {
            SeriesCollection = new SeriesCollection();
            DataContext = this;
            InitializeComponent();
            ReadDigitalCurrency();
            ReadPhysicalCurrency();
            FillComboBoxes();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
                FillTable(answer);
                Window1 w = new Window1(answer);
                w.Activate();
                w.Show();
                
            }
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
                string dcstring = (string)valutaCb.SelectedItem;
                if (dcstring == null) throw new Exception("Morate selektovati valutu.");
                DigitalCurrency dc = new DigitalCurrency(dcstring);
                string pcstring = (string)trzisteCb.SelectedItem;
                if (pcstring == null) throw new Exception("Morate selektovati tržište.");
                PhysicalCurrency pc = new PhysicalCurrency(pcstring);

                cbi = (ComboBoxItem)intervalCb.SelectedItem;
                if (cbi == null) throw new Exception("Morate selektovati interval.");
                string function = cbi.Content.ToString();
                switch (function)
                {
                    case "U jednom danu": function = "CRYPTO_INTRADAY"; break;
                    case "Dnevno": function = "DIGITAL_CURRENCY_DAILY"; break;
                    case "Nedeljno": function = "DIGITAL_CURRENCY_WEEKLY"; break;
                    case "Mesečno": function = "DIGITAL_CURRENCY_MONTHLY"; break;
                    default:
                        string message = "Interval mora biti selektovan.";
                        Console.WriteLine(message);
                        throw new Exception(message);
                }
                string interval = "";

                if (function == "CRYPTO_INTRADAY")
                {
                    cbi = (ComboBoxItem)vremelCb.SelectedItem;
                    if (cbi == null) throw new Exception("Morate selektovati vreme.");
                    interval = cbi.Content.ToString();
                    interval = interval.Split(" ")[0];
                    interval += "min";
                }
                if (interval == "" && function == "CRYPTO_INTRADAY") 
                {
                    string message = "Vreme mora biti selektovano za Interval 'u jednom danu'.";
                    Console.WriteLine(message);
                    throw new Exception(message);
                }

                string outputsize = "";
                Service service = new Service();

                // return value
                APIAnswer answer = service.PullData(function, dc, pc, interval, outputsize);
                answer.IsIntraday = function == "CRYPTO_INTRADAY";
                cbi = (ComboBoxItem)vrstaCb.SelectedItem;
                if (cbi == null) throw new Exception("Morate selektovati vrstu.");
                answer.Vrsta = cbi.Content.ToString();
                return answer;
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška: " + ex.Message);
                return null;
            }
        }
    }
}
