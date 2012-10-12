using System;
using System.Reflection;

namespace ProseLanguage
{
	public class LoadAssemblyAction : ProseObjectBase, ProseObject, ProseAction
	{
		string dllName;
		ProseObject rawWord;
		RawWord[] rawWords;

		public bool IsPure {	get {	return false;	}	}

		public LoadAssemblyAction (string assemblyFileName, ProseObject rawWord)
		{
			dllName = assemblyFileName;
			this.rawWord = rawWord;
			if (rawWord is RawWordObject)
				rawWords = ((RawWordObject) rawWord).RawWords;
			else if (rawWord is Word)
				rawWords = ((Word) rawWord).RawWords;
			else
				throw new Exception("Assembly handles must be words or raw words.");
		}

		public void performAction(ProseRuntime runtime)
		{
			//	Load the dll
			Assembly assembly = Assembly.LoadFrom(dllName);
			//	Build an assembly word from it
			AssemblyNameWord asmName = new AssemblyNameWord(rawWords, runtime, assembly);
			runtime.addWord(asmName);
		}

		public string getReadableString()
		{
			return "LoadAndBindAssembly{\"" + dllName + "\" <- " + rawWord.getReadableString() + "}";
		}
	}
}

