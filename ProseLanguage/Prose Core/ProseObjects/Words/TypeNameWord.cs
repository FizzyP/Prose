using System;
using System.Reflection;


namespace ProseLanguage
{
	public class TypeNameWord : Word
	{

		private Type type;
		public Type TypeObject { get { return type; } }
		
		public TypeNameWord (RawWord[] words, ProseRuntime runtime, Type type )
			: base(words)
		{
			this.type = type;
			isa = new Word[] { runtime.word("@type") };
		}
	}
}

