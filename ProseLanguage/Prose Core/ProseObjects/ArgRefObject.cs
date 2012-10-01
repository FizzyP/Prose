using System;

namespace ProseLanguage
{
	public class ArgRefObject : ProseObject
	{
		public int reffedArgIndex;

		public ArgRefObject (int refedArgIndex)
		{
			this.reffedArgIndex = refedArgIndex;
		}

		public string getReadableString() {
			return "refToArg#" + reffedArgIndex;
		}

		public ProseObject[] getIsa() {	return new ProseObject[0];	}

		// Tags
		private UInt64 tag;
		public void setTag(UInt64 tag) {	this.tag = tag;	}
		public UInt64 getTag() {	return tag;	}
	}
}

