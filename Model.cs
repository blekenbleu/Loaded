using System.Collections.Generic;
using System.ComponentModel;

namespace blekenbleu.loaded
{
	// borrowed from Haptics Spec.cs
	/// <summary>
	/// Abstract base class to implement INotifyPropertyChanged interface
	/// https://gist.github.com/itajaja/7439345
	/// </summary>
	public abstract class NotifyPropertyChanged : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		protected void SetField<T>(ref T field, T value, string name)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return;
			field = value;
			OnPropertyChanged(name);
		}

		protected void SetSpec<T>(ref T field, T value, string name)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return;
			field = value;
			OnPropertyChanged(name);
		}
	}

	public class Model : NotifyPropertyChanged	// XAML DataContext
	{
		private double thresh_sv = 3;
		public double Thresh_sv
		{
			get => thresh_sv;
			set { SetField(ref thresh_sv, value, nameof(Thresh_sv)); }
		}

		private double thresh_sh = 15;
		public double Thresh_sh
		{
			get => thresh_sh;
			set { SetField(ref thresh_sh, value, nameof(Thresh_sh)); }
		}

		private double thresh_ss = 15;
		public double Thresh_ss
		{
			get => thresh_ss;
			set { SetField(ref thresh_ss, value, nameof(Thresh_ss)); }
		}

		private double filter_L = 15;
		public double Filter_L
		{
			get => filter_L;
			set { SetField(ref filter_L, value, nameof(Filter_L)); }
		}
	}
}
