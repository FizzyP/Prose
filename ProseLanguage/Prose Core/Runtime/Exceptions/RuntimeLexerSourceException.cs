using System;

namespace ProseLanguage
{
	public class RuntimeLexerSourceException : Exception
	{
		LexerToken badToken;

		public RuntimeLexerSourceException (string msg, LexerToken badToken) : base(msg)
		{
			this.badToken = badToken;
		}


		public string Report {
			get {
				return Message + "  Bad word: " + badToken.Report + ".";
			}
		}


	}
}

