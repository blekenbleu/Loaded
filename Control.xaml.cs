using System.Windows.Controls;

namespace blekenbleu.loaded
{
	/// <summary>
	/// Control.xaml interaction logic
	/// </summary>
	public partial class Control : UserControl
	{
		public Loaded Plugin { get; }
		public Model Model { get; }		// for e.g. Binding slider values

		public Control() =>	InitializeComponent();

		public Control(Loaded plugin) : this()
		{
			Plugin = plugin;
			DataContext = Model = new Model();
			Model.SlipGain = plugin.Settings.SlipGain;
			Model.MatchGain = plugin.Settings.MatchGain;
			Model.Thresh_sv = plugin.Settings.Thresh_sv;
			Model.Thresh_sh = plugin.Settings.Thresh_sh;
			Model.Thresh_ss = plugin.Settings.Thresh_ss;
			Model.Filter_L = plugin.Settings.Filter_L;
			tt.Title = "Version " + Plugin.PluginVersion + " Options";
			gl.Title = $"Load gain = {Plugin.Settings.Gain:##0.00}";
		}
	}
}
