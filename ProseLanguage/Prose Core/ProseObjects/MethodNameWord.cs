using System;

namespace ProseLanguage
{
	public class MethodNameWord : Word
	{
		private ProseLanguage.ActionDelegate delegateMethod;

		internal MethodNameWord (RawWord[] words) : base(words)
		{
		}

		internal ProseLanguage.ActionDelegate DelegateMethod {
			get {	return delegateMethod;	}
		}
	}
}

