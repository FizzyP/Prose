using System;

namespace ProseLanguage
{
	public class LexerFailure : Exception
	{
		public LexerFailure (string msg) : base(msg)
		{
		}
	}
}

