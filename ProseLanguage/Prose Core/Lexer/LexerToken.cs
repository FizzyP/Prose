using System;

namespace ProseLanguage
{
	public class LexerToken
	{
		public RawWord rawWord;
		public int startCharIdx;
		public int endCharIdx;
		public TYPE tokenType;

		public enum TYPE {
			UNCLASSIFIED = 0,
			STRING
		}

		public LexerToken (string rawString, int startCharIdx, int endCharIdx)
		{
			rawWord = RawWord.new_FromString(rawString);
			this.startCharIdx = startCharIdx;
			this.endCharIdx = endCharIdx;
			tokenType = TYPE.UNCLASSIFIED;
		}


		public static LexerToken new_StringToken(string rawString, int startCharIdx, int endCharIdx)
		{
			LexerToken stringToken = new LexerToken(rawString, startCharIdx, endCharIdx);
			stringToken.tokenType = TYPE.STRING;
			return stringToken;
		}

		public string Report {
			get {
				return rawWord.AsString;
			}
		}
	}
}

