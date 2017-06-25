using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SoundMixer
{
	/// <summary>
	/// Interaction logic for MixerView.xaml
	/// </summary>
	public partial class MixerView : Window
	{
		private bool _sliding = false;
		public Mixer _mixer = Mixer.Instance;
		private Timer _timer;
		public MixerView()
		{
			InitializeComponent();
			Top = Properties.Settings.Default.Top;
			Left = Properties.Settings.Default.Left;
			Body.ItemsSource = _mixer.Apps;
			this.DataContext = _mixer;
			_timer = new Timer();
			_timer.Interval = 300;
			bool alternateMuted = false;
			_timer.Elapsed += (o, e) => {
				alternateMuted = !alternateMuted;
				Dispatcher.Invoke(() => {
					if (!_sliding) {
						Body.ItemsSource = null;
						Body.ItemsSource = _mixer.Apps;//reset to keep up to date
					}
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

		private void OnDrag(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void MinimizeButtom_Click(object sender, RoutedEventArgs e)
		{
			Window_Closing(null, null);
			MainWindow main = new MainWindow();
			main.Show();
			Close();
		}

		private void MaximizeButton_Click(object sender, RoutedEventArgs e)
		{
			if (WindowState == WindowState.Normal)
			{
				WindowState = WindowState.Maximized;
				MainGrid.Margin = new Thickness(0);
			}
			else
			{
				MainGrid.Margin = new Thickness(10);
				WindowState = WindowState.Normal;
			}
		}

		private void TitleBar_Click(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				MaximizeButton_Click(null, null);
			}
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			_timer.Stop();
			_timer.Dispose();
			Properties.Settings.Default.Top = Top;
			Properties.Settings.Default.Left = Left;
			Properties.Settings.Default.Save();
		}

		private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			_sliding = true;
		}

		private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			_sliding = false;
		}
	}
}
