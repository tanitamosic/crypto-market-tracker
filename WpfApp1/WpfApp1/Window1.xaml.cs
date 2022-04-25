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
                Stroke = Brushes.Red,
                Fill = Brushes.Transparent,
                Focusable = true
            };
            var lineSeries4 = new LineSeries
            {
                Title = "High",
                Values = answer.GetHighFirstValues(),
                Stroke = Brushes.Blue,
                Fill = Brushes.Transparent,
                Focusable = true
            };

            SeriesCollection.Add(lineSeries1);
            SeriesCollection.Add(lineSeries2);
            SeriesCollection.Add(lineSeries3);
            SeriesCollection.Add(lineSeries4);


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
