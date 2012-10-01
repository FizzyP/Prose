using System;

namespace ProseLanguage
{
	public class RuntimeFailure : Exception
	{
		public RuntimeFailure (string msg) : base(msg)
		{
		}
	}
}

