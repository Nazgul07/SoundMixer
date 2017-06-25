using CoreAudioApi;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SoundMixer
{
	public class AudioApp : INotifyPropertyChanged
	{
		private AudioSessionControl _session;
		public AudioApp(AudioSessionControl session)
		{
			_session = session;
		}

		public string Name
		{
			get {
				if(_session.IsSystemIsSystemSoundsSession)
				{
					return "System Sounds";
				}
				var process = Process.GetProcessById((int)_session.ProcessID);
				return (string.IsNullOrEmpty(process.MainWindowTitle) ? process.ProcessName : process.MainWindowTitle);
			}
		}

		public int Volume
		{
			get => (int)(_session.SimpleAudioVolume.MasterVolume * 100);
			set
			{
				_session.SimpleAudioVolume.MasterVolume = (float)(value * .01);
				OnPropertyChanged(nameof(Volume));
			}
		}

		public bool Muted
		{
			get => _session.SimpleAudioVolume.Mute;
			set
			{
				_session.SimpleAudioVolume.Mute = value;
				OnPropertyChanged(nameof(Muted));
			}
		}

		public ImageSource ImageSource
		{
			get
			{
				if (_session.IsSystemIsSystemSoundsSession)
				{
					var image = new BitmapImage();
					image.BeginInit();
					image.UriSource = new Uri(@"pack://application:,,,/Resources/Windows.png");
					image.DecodePixelHeight = 30;
					image.DecodePixelWidth = 30;
					image.EndInit();
					return image;
				}
				var process = Process.GetProcessById((int)_session.ProcessID);
				return IconHelper.GetAppIcon(process).ToBitmap().ToBitmapImage();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string prop)
		{
			PropertyChangedEventHandler handler = this.PropertyChanged;

			if (handler != null)
			{
				var e = new PropertyChangedEventArgs(prop);
				handler(this, e);
			}
		}
	}
}
