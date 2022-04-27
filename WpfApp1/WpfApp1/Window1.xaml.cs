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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public SeriesCollection SeriesCollection { get; set; }
        public Window1(APIAnswer answer)
        {
            SeriesCollection = new();
            DataContext = this;
            InitializeComponent();
            DrawGraph(answer);
        }
        private void DrawGraph(APIAnswer answer)
        {
            SeriesCollection.Clear();
            var lineSeries1 = new LineSeries
            {
                Title = answer.Vrsta,
                Values = answer.GetValues(),
                Stroke = Brushes.Green,
                Fill = Brushes.Transparent,
                Focusable = true
            };

            SeriesCollection.Add(lineSeries1);


            dataChart.AxisX.Clear();
            dataChart.AxisY.Clear();
            Axis X = new Axis();
            X.Title = "Dates";
            X.Labels = new List<string>();
            foreach (var a in answer.marketCapInfos)
            {
                X.Labels.Add(a.Timestamp.ToString());
            }
            Axis Y = new Axis();
            Y.Title = "Values";
            dataChart.AxisX.Add(X);
            dataChart.AxisY.Add(Y);

            dataChart.Series = SeriesCollection;
        }
    }
}
