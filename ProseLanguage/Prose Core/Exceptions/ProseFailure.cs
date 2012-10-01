using System;

namespace ProseLanguage
{
	public class ProseFailure : Exception
	{
		public ProseFailure (string message) : base(message)
		{
		}
	}
}

