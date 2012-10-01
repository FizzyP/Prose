using System;

namespace ProseLanguage
{
	public class RuntimeProseLanguageException : Exception
	{
		PNode beginningOfReducedSentence;

		public RuntimeProseLanguageException (string msg, PNode reducedSentence) : base(msg)
		{
			beginningOfReducedSentence = reducedSentence;
		}


		public string Report {
			get {
				return Message + "\n" + beginningOfReducedSentence.getReadableString();
			}
		}
	}
}

