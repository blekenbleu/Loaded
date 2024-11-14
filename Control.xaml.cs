using System.Windows.Controls;

namespace blekenbleu.loaded
{
    /// <summary>
    /// Control.xaml interaction logic
    /// </summary>
    public partial class Control : UserControl
    {
        public Loaded Model { get; }

        public Control()
        {
            InitializeComponent();
        }

        public Control(Loaded plugin) : this()
        {
            this.Model = plugin;
        }


    }
}
