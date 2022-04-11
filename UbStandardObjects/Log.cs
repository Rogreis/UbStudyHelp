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


		protected void FireShowMessage(string message, bool isError = false, bool isFatal = false)
        {
			ShowMessage?.Invoke(message, isError, isFatal);
		}

	}
}
