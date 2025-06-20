using System.Windows.Controls;
using System.Windows;

namespace blekenbleu.loaded
{
	/// <summary>
	/// Control.xaml interaction logic
	/// </summary>
	public partial class Control : UserControl
	{
		public Model Model { get; }		// for e.g. Binding slider values

		public Control() =>	InitializeComponent();

		public Control(Loaded plugin) : this()
		{
			DataContext = Model = new Model();
			Model.YawScale = plugin.Settings.SlipGain;
			Model.SwayScale = plugin.Settings.SwayGain;
			Model.RRfactor = plugin.Settings.MatchGain;
			Model.Thresh_sv = plugin.Settings.Thresh_sv;
			Model.Thresh_sh = plugin.Settings.Thresh_sh;
			Model.Thresh_ss = plugin.Settings.Thresh_ss;
			Model.Filter_L = plugin.Settings.Filter_L;
			Model.ButtonVisibility = Visibility.Visible;
			gl.Title = $"Load gain = {plugin.Settings.Gain:##0.00}";
		}

		private void Hyperlink_RequestNavigate(object sender,
									System.Windows.Navigation.RequestNavigateEventArgs e)
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
