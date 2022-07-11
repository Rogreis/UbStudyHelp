using System;

namespace UbStandardObjects
{

	public abstract class Log
	{
		public event ShowMessage ShowMessage = null;

		public abstract void Initialize(string path, bool append = false);

		public abstract void Clear();

		public abstract void Warn(string message);

		public abstract void Info(string message);

		public abstract void Error(string message);

		public abstract void Error(string message, Exception ex);

		public abstract void FatalError(string message);

		public abstract void NonFatalError(string message);

		public abstract void ShowLog();

		public void IsNull(object obj, string message)
		{
			if (obj == null)
				FatalError(message);
		}

		public void InInterval(short value, short minValue, short maxValue, string message)
		{
			if (value < minValue || value > maxValue )
				FatalError(message);
		}

		protected void FireShowMessage(string message, bool isError = false, bool isFatal = false)
        {
			ShowMessage?.Invoke(message, isError, isFatal);
		}

	}
}
