using System.Windows.Controls;

namespace blekenbleu.loaded
{
    /// <summary>
    /// Control.xaml interaction logic
    /// </summary>
    public partial class Control : UserControl
    {
        public Loaded Plugin { get; }

        public Control()
        {
            InitializeComponent();
        }

        public Control(Loaded plugin) : this()
        {
            this.Plugin = plugin;
        }

		public void Change_sv(object sender, System.EventArgs e)
		{
			Plugin.FromSv(sv.Value);
		}

		public void Change_ss(object sender, System.EventArgs e)
		{
			Plugin.FromSs(ss.Value);
		}

		public void Change_sh(object sender, System.EventArgs e)
		{
			Plugin.FromSh(sh.Value);
		}
    }
}
