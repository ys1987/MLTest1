using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace View
{
    public class ViewModel
    {

        public PlotModel Model { get; } = new PlotModel();
        public PlotController Controller { get; } = new PlotController();

        public OxyPlot.Axes.TimeSpanAxis X { get; } = new OxyPlot.Axes.TimeSpanAxis();
        public OxyPlot.Axes.LinearAxis Y { get; } = new OxyPlot.Axes.LinearAxis();
        public OxyPlot.Series.LineSeries LineSeries { get; private set; }
        public OxyPlot.Series.FunctionSeries FunctionSeries { get; private set; }

        public ObservableCollection<TestData> Samples { get; private set; }

        public void Init()
        {
            Samples = new ObservableCollection<TestData>
            {
                new TestData{ Time= new TimeSpan(0,0,0), Value=0, Tag="A" },
                new TestData{ Time= new TimeSpan(0,0,1), Value=2, Tag="B" },
                new TestData{ Time= new TimeSpan(0,0,2), Value=4, Tag="C" },
                new TestData{ Time= new TimeSpan(0,0,3), Value=6, Tag="D" },
                new TestData{ Time= new TimeSpan(0,0,4), Value=0, Tag="E" },
                new TestData{ Time= new TimeSpan(0,0,5), Value=2, Tag="F" },
            };

            Model.Title = "PlotView";

            // 軸の初期化
            X.Position = OxyPlot.Axes.AxisPosition.Bottom;
            Y.Position = OxyPlot.Axes.AxisPosition.Left;

            // 線グラフ
            LineSeries = new OxyPlot.Series.LineSeries();
            LineSeries.Title = "Custom";
            LineSeries.ItemsSource = Samples;
            LineSeries.DataFieldX = nameof(TestData.Time);
            LineSeries.DataFieldY = nameof(TestData.Value);

            var a = 1;
            var b = 2;

            // 関数グラフ
            FunctionSeries = new OxyPlot.Series.FunctionSeries
            (
                x => a * x + b, 0, 30, 5, "Y = ax + b"
            );

            Model.Axes.Add(X);
            Model.Axes.Add(Y);
            Model.Series.Add(LineSeries);
            Model.Series.Add(FunctionSeries);

            Model.InvalidatePlot(true);
        }

        public class TestData
        {
            public TimeSpan Time { get; set; }
            public double Value { get; set; }
            public string Tag { get; set; }
        }
    }
}
