using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Distributions;

namespace View
{
    public interface ViewModel
    {
        public PlotModel Model { get; }
    }

    public class NormalDistributionViewModel : ViewModel
    {
        public PlotModel Model { get; } = new PlotModel { Title = "ガウス分布" };
        public NormalDistributionViewModel()
        {
            Model.Series.Add(new FunctionSeries(Normal.WithMeanPrecision(10.0, 2.5).Density, -20.0, 20.0, 0.01, "μ=10, λ=2.5"));
            Model.Series.Add(new FunctionSeries(Normal.WithMeanPrecision(0.0, 0.1).Density, -20.0, 20.0, 0.01, "μ=0, λ=0.1"));
            Model.Series.Add(new FunctionSeries(Normal.WithMeanPrecision(-10.0, 0.8).Density, -20.0, 20.0, 0.01, "μ=-10, λ=0.8"));
        }
    }

    public class GammaDistributionViewModel : ViewModel
    {
        public PlotModel Model { get; } = new PlotModel { Title = "ガンマ分布" };
        public GammaDistributionViewModel()
        {
            Model.Series.Add(new FunctionSeries(Gamma.WithShapeRate(1.0, 1.0).Density, 0.0, 20.0, 0.01, "a=1.0, b=1.0"));
            Model.Series.Add(new FunctionSeries(Gamma.WithShapeRate(1.0, 2.0).Density, 0.0, 20.0, 0.01, "a=1.0, b=2.0"));
            Model.Series.Add(new FunctionSeries(Gamma.WithShapeRate(2.0, 1.0).Density, 0.0, 20.0, 0.01, "a=2.0, b=1.0"));
            Model.Series.Add(new FunctionSeries(Gamma.WithShapeRate(1.0, 2.0).Density, 0.0, 20.0, 0.01, "a=1.0, b=2.0"));
            Model.Series.Add(new FunctionSeries(Gamma.WithShapeRate(9.0, 0.5).Density, 0.0, 20.0, 0.01, "a=9.0, b=0.5"));
        }
    }

    public class StudentTDistributionViewModel : ViewModel
    {
        public PlotModel Model { get; } = new PlotModel { Title = "スチューデントのt分布" };
        public StudentTDistributionViewModel()
        {
            Model.Series.Add(new FunctionSeries(new StudentT(0.0, 1.0, 1.0).Density, -20.0, 20.0, 0.01, "μ=0.0, λ=1.0, ν=1.0"));
            Model.Series.Add(new FunctionSeries(new StudentT(10.0, 1.0, 1.0).Density, -20.0, 20.0, 0.01, "μ=10.0, λ=1.0, ν=1.0"));
            Model.Series.Add(new FunctionSeries(new StudentT(0.0, 3.0, 1.0).Density, -20.0, 20.0, 0.01, "μ=0.0, λ=3.0, ν=1.0"));
            Model.Series.Add(new FunctionSeries(new StudentT(0.0, 3.0, 3.0).Density, -20.0, 20.0, 0.01, "μ=0.0, λ=3.0, ν=3.0"));
            Model.Series.Add(new FunctionSeries(new StudentT(0.0, 3.0, Double.MaxValue).Density, -20.0, 20.0, 0.01, "μ=0.0, λ=3.0, ν=Double.MaxValue"));
        }
    }

    public class LearningViewModel : ViewModel
	{
        public PlotModel Model { get; } = new PlotModel { Title = "学習" };
        private Normal TrueDistribution { get; set; } = new Normal();

        public void SetTrueDistribution(double mean, double precision)
		{
            TrueDistribution = Normal.WithMeanPrecision(mean, precision);
            Model.Series.Clear();
            Model.Series.Add(new FunctionSeries(TrueDistribution.Density, TrueDistribution.Mean - 20, TrueDistribution.Mean + 20, 0.01, "真の分布"));
            Model.InvalidatePlot(true);
        }

        public IEnumerable<double> Samples(int sampleCount)
		{
            return TrueDistribution.Samples().Take(sampleCount).ToArray();
		}

        private NormalGamma PriorDistribution { get; set; }
        public void Init(double m, double beta, double a, double b)
		{
            PriorDistribution = new NormalGamma(m, beta, a, b);
            var normal = Normal.WithMeanPrecision(PriorDistribution.MeanMarginal().Mean, PriorDistribution.PrecisionMarginal().Mean);
            Model.Series.Add(new FunctionSeries(normal.Density, normal.Mean - 20, normal.Mean + 20, 0.01, "学習前（初期）の分布"));
            Model.InvalidatePlot(true);
        }

        public void Learn(IEnumerable<double> samples)
		{
            var beta = samples.Count() + PriorDistribution.MeanScale;
            var m = (samples.Sum() + PriorDistribution.MeanScale * PriorDistribution.MeanLocation) / beta;
            var a = samples.Count() / 2 + PriorDistribution.PrecisionShape;
            var b = (samples.Sum(target => Math.Pow(target, 2)) + PriorDistribution.MeanScale * Math.Pow(PriorDistribution.MeanLocation, 2) - beta * Math.Pow(m, 2)) / 2 + PriorDistribution.PrecisionInverseScale;

            var posteriorDistribution = new NormalGamma(m, beta, a, b);
            //var normal = Normal.WithMeanPrecision(posteriorDistribution.MeanMarginal().Mean, posteriorDistribution.PrecisionMarginal().Mean);
            //Model.Series.Add(new FunctionSeries(normal.Density, normal.Mean - 20, normal.Mean + 20, 0.01, "学習後の分布"));

            var mu = m;
            var lambda = (beta * a) / ((1 + beta) * b);
            var nu = 2 * a;
            // 第2引数が標準偏差なので追加で計算をしている
            var predictedDistribution = new StudentT(mu, 1 / Math.Sqrt(lambda), nu);
            Model.Series.Add(new FunctionSeries(predictedDistribution.Density, predictedDistribution.Mean - 20, predictedDistribution.Mean + 20, 0.01, "予測分布"));
            Model.InvalidatePlot(true);
        }
    }
}
