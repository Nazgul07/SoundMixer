using CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;

namespace SoundMixer
{
	public class Mixer : INotifyPropertyChanged
	{
		public ObservableCollection<AudioApp> Apps { get {
				ObservableCollection<AudioApp> list = new ObservableCollection<AudioApp>();
				for (int i = 0; i < _outputDevice.AudioSessionManager.Sessions.Count; i++)
				{
					var session = _outputDevice.AudioSessionManager.Sessions[i];
					if(session.State == AudioSessionState.AudioSessionStateActive || session.IsSystemIsSystemSoundsSession)
						list.Add(new AudioApp(session));
				}
				return list;
			}
		}

		private readonly MMDevice _inputDevice;
		private readonly MMDevice _outputDevice;
		private readonly Timer _timer;
		private static readonly object SyncRoot = new object();
		private static Mixer _instance;

		public static Mixer Instance
		{
			get
			{
				if (_instance == null)
				{
					lock(SyncRoot)
						if (_instance == null)
							_instance = new Mixer();
				}
				return _instance;
			}
		}

		private Mixer()
		{
			MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
			_inputDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eCommunications);
			_outputDevice = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
			InputMuted = _inputDevice.AudioEndpointVolume.Mute || _inputDevice.AudioMeterInformation.MasterPeakValue == 0;

			_timer = new Timer {Interval = 10};
			_timer.Elapsed += _timer_Elapsed;
			_timer.Start();
		}

		private void _timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			OnPropertyChanged(nameof(InputMuted));
			OnPropertyChanged(nameof(MasterVolume));
		}

		public bool InputMuted {
			get => _inputDevice.AudioEndpointVolume.Mute || _inputDevice.AudioMeterInformation.MasterPeakValue == 0;
			set {
				_inputDevice.AudioEndpointVolume.Mute = value;
				OnPropertyChanged(nameof(InputMuted));
			}
		}
			 
		public int MasterVolume
		{
			get => (int)(_outputDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
			set {
				_outputDevice.AudioEndpointVolume.MasterVolumeLevelScalar = (float)(value * .01);
				OnPropertyChanged(nameof(MasterVolume));
			}
		}

		public bool MasterVolumeMuted
		{
			get => _outputDevice.AudioEndpointVolume.Mute;
			set
			{
				_outputDevice.AudioEndpointVolume.Mute = value;
				OnPropertyChanged(nameof(MasterVolumeMuted));
			}
		}

		internal void ToggleInputMute()
		{
			_inputDevice.AudioEndpointVolume.Mute = !_inputDevice.AudioEndpointVolume.Mute;
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

		~Mixer()
		{
			//be sure to unmute!!
			_inputDevice.AudioEndpointVolume.Mute = false;
			_timer.Stop();
			_timer.Dispose();
		}
	}
}
