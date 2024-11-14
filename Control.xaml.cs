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


    }
}
