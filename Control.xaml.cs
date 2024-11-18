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
			tt.Title = "Version " + Plugin.PluginVersion + " Options";
		}
	}
}
