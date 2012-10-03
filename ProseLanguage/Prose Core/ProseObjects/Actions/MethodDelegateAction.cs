using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	public class MethodDelegateAction : ProseObjectBase, ProseAction
	{

		private bool isPure = false;
		MethodNameWord methodName;
		List<ProseObject> args;
		ProseLanguage.ActionDelegate method;

		public MethodDelegateAction (MethodNameWord methodName, List<ProseObject> args)
		{
			this.methodName = methodName;
			this.args = args;
			this.method = methodName.DelegateMethod;
		}

		public bool IsPure {	get {	return isPure;	}	}

		public string getReadableString() {
			return methodName.getReadableString() + "{}";
		}

		public void performAction(ProseRuntime runtime)
		{
			method(runtime, args);
		}

	}
}

