using System;
using System.Reflection;


namespace ProseLanguage
{
	public class BindMethodAction : ProseObjectBase, ProseObject, ProseAction
	{
		public BindMethodAction ()
		{
		}

		TypeNameWord typeWord;
		string methodName;
		RawWord[] rawWords;
		
		public bool IsPure {	get {	return false;	}	}
		
		public BindMethodAction (TypeNameWord typeWord, string methodName, RawWord[] rawWords)
		{
			this.typeWord = typeWord;
			this.methodName = methodName;
			this.rawWords = rawWords;
		}
		
		public void performAction(ProseRuntime runtime)
		{
			//	Look up the type.

			MethodInfo methodInfo = typeWord.TypeObject.GetMethod(methodName, BindingFlags.Public|BindingFlags.Static);
			ProseLanguage.ActionDelegate delegateMethod = (ProseLanguage.ActionDelegate) Delegate.CreateDelegate(typeof(ProseLanguage.ActionDelegate), methodInfo);
			MethodNameWord methodNameWord = new MethodNameWord(rawWords, runtime, delegateMethod);
			runtime.addWord(methodNameWord);
		}
		
		public string getReadableString()
		{
			return "BindMethod{" + typeWord.getReadableString() + "." + methodName + " <- " + rawWords[0].AsString + "}";
		}
	}
}

