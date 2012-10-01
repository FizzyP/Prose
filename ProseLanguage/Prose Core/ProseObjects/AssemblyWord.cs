using System;
using System.Reflection;

namespace ProseLanguage
{
	public class AssemblyWord : Word
	{
		private Assembly assembly;
		public Assembly AssemblyObject { get { return assembly; } }

		public AssemblyWord (RawWord[] words, ProseRuntime runtime, Assembly assembly )
			: base(words)
		{
			this.assembly = assembly;
			isa = new Word[] { runtime.word("@assembly") };
		}
	}
}

