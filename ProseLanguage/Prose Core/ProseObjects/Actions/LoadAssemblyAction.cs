using System;
using System.Reflection;

namespace ProseLanguage
{
	public class LoadAssemblyAction : ProseObjectBase, ProseObject, ProseAction
	{
		string dllName;
		RawWordObject rawWordObj;

		public bool IsPure {	get {	return false;	}	}

		public LoadAssemblyAction (string assemblyFileName, RawWordObject rawWordObj)
		{
			dllName = assemblyFileName;
			this.rawWordObj = rawWordObj;
		}

		public void performAction(ProseRuntime runtime)
		{
			//	Load the dll
			Assembly assembly = Assembly.LoadFrom(dllName);
			//	Build an assembly word from it
			AssemblyNameWord asmName = new AssemblyNameWord(rawWordObj.RawWords, runtime, assembly);
			runtime.addWord(asmName);
		}

		public string getReadableString()
		{
			return "LoadAndBindAssembly{\"" + dllName + "\" <- " + rawWordObj.getReadableString() + "}";
		}
	}
}

