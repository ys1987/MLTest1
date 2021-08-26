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

namespace View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public ViewModel MainViewModel { get; private set; } = new NormalDistributionViewModel();

		public List<KeyValuePair<Distribution, string>> Distributions { get; } = new List<KeyValuePair<Distribution, string>>()
		{
			new KeyValuePair<Distribution, string>(Distribution.Normal, "ガウス分布（正規分布）"),
			new KeyValuePair<Distribution, string>(Distribution.Gamma, "ガンマ分布"),
			new KeyValuePair<Distribution, string>(Distribution.StudentT, "スチューデントのt分布"),
			new KeyValuePair<Distribution, string>(Distribution.Learning, "学習テスト"),
		};

		public MainWindow()
		{
			InitializeComponent();

			DataContext = this;
			DistributionComboBox.SelectedIndex = 0;
		}

		private void DistributionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var distribution = (Distribution)DistributionComboBox.SelectedValue;
			SetTextBoxVisible((distribution == Distribution.Learning) ? Visibility.Visible : Visibility.Hidden);

			MainViewModel = distribution switch
			{
				Distribution.Normal => new NormalDistributionViewModel(),
				Distribution.Gamma => new GammaDistributionViewModel(),
				Distribution.StudentT => new StudentTDistributionViewModel(),
				Distribution.Learning => new LearningViewModel(),
				_ => throw new Exception(),
			};

			if (distribution == Distribution.Learning)
			{
				SetTrueDistribution();
			}

			MainPlotView.Model = MainViewModel.Model;
			MainPlotView.InvalidatePlot(true);
			MainViewModel.Model.InvalidatePlot(true);
		}

		private void SetTextBoxVisible(Visibility visibility)
		{
			TrueDistibutionLabel.Visibility = visibility;
			MeanLabel.Visibility = visibility;
			MeanTextBox.Visibility = visibility;
			PrecisionLabel.Visibility = visibility;
			PrecisionTextBox.Visibility = visibility;
			TrueDistributionButton.Visibility = visibility;
			PriorDistributionLabel.Visibility = visibility;
			MLabel.Visibility = visibility;
			MTextBox.Visibility = visibility;
			BetaLabel.Visibility = visibility;
			BetaTextBox.Visibility = visibility;
			ALabel.Visibility = visibility;
			ATextBox.Visibility = visibility;
			BLabel.Visibility = visibility;
			BTextBox.Visibility = visibility;
			SampleLabel.Visibility = visibility;
			SampleTextBox.Visibility = visibility;
			LearningButton.Visibility = visibility;
			SampleViewTextBox.Visibility = visibility;
		}

		private void TrueDistributionButton_Click(object sender, RoutedEventArgs e)
		{
			SetTrueDistribution();
		}

		private void SetTrueDistribution()
		{
			var learning = (LearningViewModel)MainViewModel;
			learning.SetTrueDistribution(double.Parse(MeanTextBox.Text), double.Parse(PrecisionTextBox.Text));
		}

		private void LearningButton_Click(object sender, RoutedEventArgs e)
		{
			Learning();
		}

		private void Learning()
		{
			var learning = (LearningViewModel)MainViewModel;
			learning.Init(double.Parse(MTextBox.Text), double.Parse(BetaTextBox.Text), double.Parse(ATextBox.Text), double.Parse(BTextBox.Text));
			var sample = learning.Samples(int.Parse(SampleTextBox.Text));
			SampleViewTextBox.Text = string.Join(Environment.NewLine, sample);
			learning.Learn(sample);
		}

		
	}

	public enum Distribution
	{
		Normal,
		Gamma,
		StudentT,
		Learning,
	}
}
