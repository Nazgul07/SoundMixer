using System.Windows;
using System.Windows.Input;
using System.Timers;
using System.ComponentModel;  
using System.Windows.Media;

namespace SoundMixer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Mixer _mixer = Mixer.Instance;
		private readonly Timer _timer;
		public MainWindow()
		{
			InitializeComponent();
			Top = Properties.Settings.Default.Top;
			Left = Properties.Settings.Default.Left;
			OuterGrid.DataContext = _mixer;

			_timer = new Timer {Interval = 300};
			bool alternateMuted = false;
			_timer.Elapsed += (o, e) => {
				alternateMuted = !alternateMuted;
				Dispatcher.Invoke(() => {
				
					if (_mixer.InputMuted && alternateMuted)  
						CircleBorder.Background = Brushes.Gray;
					else if (_mixer.InputMuted)
						CircleBorder.Background = Brushes.Red;
					else
						CircleBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#01A2DD"));
				});
			};
			_timer.Start();
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OnDrag(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			_timer.Stop();
			_timer.Dispose();
			Properties.Settings.Default.Top = Top;
			Properties.Settings.Default.Left = Left;
			Properties.Settings.Default.Save();
		}

		private void OuterBorder_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if(e.ClickCount == 2)
			{
				_mixer.ToggleInputMute();
			}
		}

		private void ExpandButton_Click(object sender, RoutedEventArgs e)
		{
			Window_Closing(null, null);
			MixerView mixer = new MixerView();
			mixer.Show();
			Close();
		}
	}
}
