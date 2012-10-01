using System;

namespace ProseLanguage
{
	public class RuntimeLexerFailure : Exception
	{
		public RuntimeLexerFailure (string msg) : base(msg)
		{
		}
	}
}

