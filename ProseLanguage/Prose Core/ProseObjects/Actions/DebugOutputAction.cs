using System;

namespace ProseLanguage
{
	public class DebugOutputAction : ProseObjectBase, ProseAction
	{
		string who, what;

		public DebugOutputAction (string who, string what)
		{
			this.who = who;
			this.what = what;
		}

		public bool IsPure {	get {	return true;	}	}

		public string getReadableString() {
			return "_Debug{" + who + ": " + what + "}";
		}

		public void performAction(ProseRuntime runtime)
		{
			Console.WriteLine("debugAction(" + who +", " + what + ")");
		}
	}
}

