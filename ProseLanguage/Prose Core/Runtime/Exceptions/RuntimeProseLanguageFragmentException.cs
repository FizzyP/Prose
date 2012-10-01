using System;

namespace ProseLanguage
{
	public class RuntimeProseLanguageFragmentException : Exception
	{
		PNode fragmentStart;

		public RuntimeProseLanguageFragmentException (string msg, PNode fragmentStart) : base(msg)
		{
			this.fragmentStart = fragmentStart;
		}


		public string Report {
			get {
				return Message;
			}
		}
	}
}

