using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

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
		private short thresh_sv = 3;
		public short Thresh_sv
		{
			get => thresh_sv;
			set { SetField(ref thresh_sv, value, nameof(Thresh_sv)); }
		}

		private short thresh_sh = 15;
		public short Thresh_sh
		{
			get => thresh_sh;
			set { SetField(ref thresh_sh, value, nameof(Thresh_sh)); }
		}

		private short thresh_ss = 15;
		public short Thresh_ss
		{
			get => thresh_ss;
			set { SetField(ref thresh_ss, value, nameof(Thresh_ss)); }
		}

		private short filter_L = 15;
		public short Filter_L
		{
			get => filter_L;
			set { SetField(ref filter_L, value, nameof(Filter_L)); }
		}

		private short steerFact = 70;
		public short SteerFact
		{
			get => steerFact;
			set { SetField(ref steerFact, value, nameof(SteerFact)); }
		}

		private int rrScale = 20;
		public int RRscale
		{
			get => rrScale;
			set { SetField(ref rrScale, value, nameof(RRscale)); }
		}

		private int yawScale = 20;
		public int YawScale
		{
			get => yawScale;
			set { SetField(ref yawScale, value, nameof(YawScale)); }
		}

		private int swayScale = 20;
		public int SwayScale
		{
			get => swayScale;
			set { SetField(ref swayScale, value, nameof(SwayScale)); }
		}

		private Visibility _svis = Visibility.Hidden;
		public Visibility ButtonVisibility		// must be public for XAML Binding
		{
			get => _svis;
			set
			{
				SetField(ref _svis, value, nameof(ButtonVisibility));
				if (Visibility.Hidden == _svis)
				{
					ModeColor = "Green";
					Mode = "Auto";
				}
				else Mode = "Manually";
			}
		}

		private string _color = "Green";
		public string ModeColor
		{
			get => _color;
			set { SetField(ref _color, value, nameof(ModeColor)); }
		}

		private string _mode = "";
		public string Mode
		{
			get => "Press to " + _mode + " scale";
			set { SetField(ref _mode, value, nameof(Mode)); }
		}

		private bool _recal = false;
		public bool Recal
		{
			get => _recal;
			set { SetField(ref _recal, value, nameof(Recal)); }
		}
	}
}
