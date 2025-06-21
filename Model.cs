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
        readonly Settings Set;

		public Model(Loaded plugin)
		{
			Set = plugin.Settings;
		}

		public short Thresh_sv
		{
			get => Set.thresh_sv;
			set { SetField(ref Set.thresh_sv, value, nameof(Thresh_sv)); }
		}

		public short Thresh_sh
		{
			get => Set.thresh_sh;
			set { SetField(ref Set.thresh_sh, value, nameof(Thresh_sh)); }
		}

		public short Thresh_ss
		{
			get => Set.thresh_ss;
			set { SetField(ref Set.thresh_ss, value, nameof(Thresh_ss)); }
		}

		public short Filter_L
		{
			get => Set.filter_L;
			set { SetField(ref Set.filter_L, value, nameof(Filter_L)); }
		}

		public short SteerFact
		{
			get => Set.steerFact;
			set { SetField(ref Set.steerFact, value, nameof(SteerFact)); }
		}

		public int RRscale
		{
			get => Set.rrScale;
			set { SetField(ref Set.rrScale, value, nameof(RRscale)); }
		}

		public int YawScale
		{
			get => Set.yawScale;
			set { SetField(ref Set.yawScale, value, nameof(YawScale)); }
		}

		public int SwayScale
		{
			get => Set.swayScale;
			set { SetField(ref Set.swayScale, value, nameof(SwayScale)); }
		}

		public Visibility ButtonVisibility		// must be public for XAML Binding
		{
			get => Set.svis;
			set
			{
				SetField(ref Set.svis, value, nameof(ButtonVisibility));
				if (Visibility.Hidden == Set.svis)
				{
					ModeColor = "Green";
					Mode = "Auto";
				}
				else Mode = "Manually";
			}
		}

		public string ModeColor
		{
			get => Set.color;
			set { SetField(ref Set.color, value, nameof(ModeColor)); }
		}

		public string Mode
		{
			get => "Press to " + Set.mode + " scale";
			set { SetField(ref Set.mode, value, nameof(Mode)); }
		}

		public bool Recal
		{
			get => Set.recal;
			set { SetField(ref Set.recal, value, nameof(Recal)); }
		}
	}
}
