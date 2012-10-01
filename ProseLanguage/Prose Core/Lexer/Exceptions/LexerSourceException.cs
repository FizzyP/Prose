using System;

namespace ProseLanguage
{
	public class LexerSourceException : Exception
	{
		string source;
		int startIdx;
		int endIdx;

		public LexerSourceException (string msg, string source, int sourceErrorStartIdx, int sourceErrorEndIdx) : base(msg)
		{
			this.source = source;
			startIdx = sourceErrorStartIdx;
			endIdx = sourceErrorEndIdx;
		}

		public string Report {
			get {
				return Message + " Indices " + startIdx + "-" + endIdx + ".";
			}
		}
	}
}

