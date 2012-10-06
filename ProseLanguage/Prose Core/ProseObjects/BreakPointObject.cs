using System;

namespace ProseLanguage
{
	public class BreakPointObject : ProseObjectBase, ProseObject
	{

		string script;

		public string BreakScript {	get {	return script;	}	}

		//	Create a breakpoint object with an attached script to run.
		//	When the interpreter hits a breakpoint it will recursively call the
		public BreakPointObject (string script)
		{
			this.script = script;
		}
		
		public string getReadableString()
		{
			return "$BREAK$";
		}

		public bool doBreakPoint(ProseRuntime runtime, PNode source, BreakPointObject.RuntimeData rtdata)
		{
			return onBreak (runtime, source, rtdata);
		}

		//	Override this if you want persistant breakpoints.
		public bool ShouldEliminateOnContinue {
			get {	return true;	}
		}

		//	Overriding instructions:
		//	Return true if you want the runtime to make the standard breakpoint callback.
		//	Return false if you prefer to handle the event yourself in the body of onBreak()
		public bool onBreak(ProseRuntime runtime, PNode source, BreakPointObject.RuntimeData rtdata)
		{
			//	Handle the breakpoint in a non-standard way if you want
			//	by putting something in the body of this function.
			return true;
		}



		public class RuntimeData
		{
		}

	
	}
}

