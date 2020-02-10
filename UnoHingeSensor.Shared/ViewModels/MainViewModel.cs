using System;
using System.Threading.Tasks;
using System.Windows.Input;
using UnoHingeSensor.Shared.Infrastructure;
using Windows.Devices.Sensors;
using Windows.UI.Core;

namespace UnoHingeSensor.Shared.ViewModels
{
	public class MainViewModel : BindableBase
	{
		private HingeAngleSensor _hinge;
		private bool _readingChangedAttached;
		private string _sensorStatus;
		private double _angle;
		private string _timestamp;
		private readonly CoreDispatcher _dispatcher;

		public MainViewModel(CoreDispatcher dispatcher)
		{
			_dispatcher = dispatcher;
		}

		public async Task InitializeAsync()
		{
			_hinge = await HingeAngleSensor.GetDefaultAsync();
			if (_hinge != null)
			{
				SensorStatus = "HingeAngleSensor created";
			}
			else
			{
				SensorStatus = "HingeAngleSensor not available on this device";
			}
		}

		public ICommand AttachReadingChangedCommand => new Command((p) =>
		{
			_hinge.ReadingChanged += HingeAngleSensor_ReadingChanged;
			ReadingChangedAttached = true;
		});

		public ICommand DetachReadingChangedCommand => new Command((p) =>
		{
			_hinge.ReadingChanged -= HingeAngleSensor_ReadingChanged;
			ReadingChangedAttached = false;
		});

		public bool HingeAngleSensorAvailable => _hinge != null;

		public string SensorStatus
		{
			get => _sensorStatus;
			private set
			{
				_sensorStatus = value;
				RaisePropertyChanged();
			}
		}

		public bool ReadingChangedAttached
		{
			get => _readingChangedAttached;
			private set
			{
				_readingChangedAttached = value;
				RaisePropertyChanged();
			}
		}

		public double Angle
		{
			get => _angle;
			private set
			{
				_angle = value;
				RaisePropertyChanged();
			}
		}

		public string Timestamp
		{
			get => _timestamp;
			private set
			{
				_timestamp = value;
				RaisePropertyChanged();
			}
		}

		private async void HingeAngleSensor_ReadingChanged(HingeAngleSensor sender, HingeAngleSensorReadingChangedEventArgs args)
		{
			await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				Angle = args.Reading.AngleInDegrees;
				Timestamp = args.Reading.Timestamp.ToString("R");
			});
		}
	}
}
