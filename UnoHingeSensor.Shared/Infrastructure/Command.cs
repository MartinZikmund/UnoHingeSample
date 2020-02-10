using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UnoHingeSensor.Shared.Infrastructure
{
	public class Command : ICommand
	{
		private readonly Action<object> _action;
		private readonly Func<object, Task> _actionTask;
		private readonly Func<object, bool> _canExecute;
		private bool _manualCanExecute = true;

		public bool ManualCanExecute
		{
			get => _manualCanExecute;
			set
			{
				_manualCanExecute = value;
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private object _isExecuting = null;
		private static object _isExecutingNull = new object(); // this represent a null parameter when something is executing

		public Command(Action<object> action, Func<object, bool> canExecute = null)
		{
			_action = action ?? throw new ArgumentNullException(nameof(action));
			_canExecute = canExecute;
		}

		public Command(Func<object, Task> actionTask, Func<object, bool> canExecute = null)
		{
			_actionTask = actionTask ?? throw new ArgumentNullException(nameof(actionTask));
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			var canExecuteParameter = parameter ?? _isExecutingNull;

			return (_isExecuting != canExecuteParameter)
				&& (_canExecute?.Invoke(parameter) ?? true)
				&& _manualCanExecute;
		}

		public async void Execute(object parameter)
		{
			var isExecutingParameter = parameter ?? _isExecutingNull;

			if (_isExecuting == isExecutingParameter)
			{
				// This parameter is executing
				return;
			}

			try
			{
				_isExecuting = isExecutingParameter;
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
				if (_action != null)
				{
					_action(parameter);
				}
				else
				{
					await _actionTask(parameter);
				}
			}
			finally
			{
				_isExecuting = null;
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public event EventHandler CanExecuteChanged;
	}
}
