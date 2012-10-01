using System;

namespace ProseLanguage
{
	public class ProseValidationException : Exception
	{
		public ProseValidationException (string message) : base(message)
		{
		}
	}
}

