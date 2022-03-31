using System;

namespace OAuthDesktopApp.GoogleApi
{
	public abstract class GoogleApiBase
	{
		private readonly Action<string> outputCallback;

		protected GoogleApiBase(Action<string> outputCallback)
		{
			this.outputCallback = outputCallback;
		}

		protected void Output(string text)
		{
			this.outputCallback?.Invoke(text);
		}
	}
}
