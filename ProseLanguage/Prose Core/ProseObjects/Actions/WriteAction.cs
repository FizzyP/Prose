using System;

namespace ProseLanguage
{
	public class WriteAction : ProseAction
	{

		string writeMe;

		public WriteAction (string writeMe)
		{
			this.writeMe = writeMe;
		}

		public string getReadableString()
		{
			return "write{\"" + writeMe + "\"}";
		}

		public bool IsPure {
			get {	return true;	}
		}

		public void performAction(ProseRuntime runtime)
		{
			//runtime.StdOut.Write(writeMe);
		}

		public ProseObject[] getIsa() {
			return new ProseObject[0];
		}

		// Tags
		private UInt64 tag;
		public void setTag(UInt64 tag) {	this.tag = tag;	}
		public UInt64 getTag() {	return tag;	}
	}
}

