﻿using System.Windows.Controls;
using System.Windows.Navigation;
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
