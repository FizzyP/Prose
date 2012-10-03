using System;
using System.Reflection;

namespace ProseLanguage
{
	public class BindTypeAction : ProseObjectBase, ProseObject, ProseAction
	{
		AssemblyNameWord assemblyWord;
		string typeName;
		RawWord[] rawWords;
		
		public bool IsPure {	get {	return false;	}	}
		
		public BindTypeAction (AssemblyNameWord assemblyWord, string typeName, RawWord[] rawWords)
		{
			this.assemblyWord = assemblyWord;
			this.typeName = typeName;
			this.rawWords = rawWords;
		}
		
		public void performAction(ProseRuntime runtime)
		{
			//	Look up the type.
			Type type = assemblyWord.AssemblyObject.GetType(typeName);
			TypeNameWord typeNameWord = new TypeNameWord(rawWords, runtime, type);
			runtime.addWord(typeNameWord);
		}
		
		public string getReadableString()
		{
			return "BindType{" + assemblyWord.getReadableString() + "." + typeName + " <- " + rawWords[0].AsString + "}";
		}
	}
}
