using System;

namespace ProseLanguage
{
	public class MethodNameWord : Word
	{
		private ProseLanguage.ActionDelegate delegateMethod;

		public ProseLanguage.ActionDelegate DelegateMethod {
			get {	return delegateMethod;	}
		}


		public MethodNameWord (RawWord[] words, ProseRuntime runtime, ProseLanguage.ActionDelegate delegateMethod )
			: base(words)
		{
			this.delegateMethod = delegateMethod;
			isa = new Word[] { runtime.word("@method") };
		}

	}
}

