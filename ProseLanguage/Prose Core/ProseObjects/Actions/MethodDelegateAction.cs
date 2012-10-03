using System;
using System.Collections.Generic;
using System.Text;

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
			StringBuilder str = new StringBuilder();
			str.Append(methodName.getReadableString());
			str.Append("{");
			foreach (ProseObject arg in args) {
				str.Append(arg.getReadableString());
				str.Append(" ");
			}
			if (args.Count > 0)
				str.Remove(str.Length - 1, 1);

			str.Append("}");

			return str.ToString();
		}

		public void performAction(ProseRuntime runtime)
		{
			try {
				method(runtime, args);
			}
			catch (Exception e) {
				//	If anything goes wrong we "throw an exception" on the side.
				runtime.read ("foreign function exception: \"" + e.Message + "\"", runtime.GlobalClient);
			}
		}

	}
}

