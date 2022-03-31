using System;
using System.Runtime.Serialization;

namespace OAuthDesktopApp.Utilities
{
	public class OAuthException : Exception
	{
		public OAuthException()
		{
		}

		public OAuthException(string message) : base(message)
		{
		}

		public OAuthException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected OAuthException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
