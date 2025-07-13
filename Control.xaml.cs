using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System;

namespace blekenbleu.loaded
{
	// https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-convert-bound-data
	[ValueConversion(typeof(double), typeof(double))]
	public class PerCentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return 100D * (double)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    	{
			return 0.01 * (double)value;
		}
	}

	/// <summary>
	/// Control.xaml interaction logic
	/// </summary>
	public partial class Control : UserControl
	{
		public Model Model { get; }		// for e.g. Binding slider values

		public Control() =>	InitializeComponent();

		public Control(Loaded plugin) : this()
		{
			gl.Title = $"Load gain = {plugin.Settings.Gain:##0.00}";
			DataContext = Model = new Model(plugin);
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
		}

		private void Mode_Click(object sender, RoutedEventArgs e)  // handle button clicks
		{
			
			Model.ButtonVisibility = (Visibility.Visible == Model.ButtonVisibility) ?
									Visibility.Hidden : Visibility.Visible;
		}

		private void Recal_Click(object sender, RoutedEventArgs e)
		{
			Model.Recal = !Model.Recal;
		}
	}
}
