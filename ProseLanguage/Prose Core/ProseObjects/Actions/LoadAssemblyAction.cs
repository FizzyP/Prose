using System;
using System.Reflection;

namespace ProseLanguage
{
	public class LoadAssemblyAction : ProseObjectBase, ProseObject
	{
		string dllName;
		RawWordObject rawWordObj;

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
			AssemblyWord asmName = new AssemblyWord(rawWordObj.RawWords, runtime, assembly);
			runtime.addWord(asmName);
		}

		public string getReadableString()
		{
			return "LoadAndBindAssembly{\"" + dllName + "\" <- " + rawWordObj.getReadableString() + "}";
		}
	}
}

