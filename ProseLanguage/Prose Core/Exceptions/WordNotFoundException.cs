using System;

namespace ProseLanguage
{
	public class WordNotFoundException : Exception
	{
		private RawWord[] rawWords;

		public WordNotFoundException (RawWord[] rawWords, string message) : base(message)
		{
			this.rawWords = rawWords;
		}

		public RawWord[] RawWords {
			get {
				return rawWords;
			}
		}
	}
}

