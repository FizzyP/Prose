using System;

namespace ProseLanguage
{
	public class ProseObjectBase
	{
		ProseObject[] isa = null;
		UInt64 tag;

		public ProseObjectBase ()
		{
		}

		public ProseObject[] getIsa() {		return isa;		}

		public void setTag(UInt64 tag) {	this.tag = tag;		}
		public UInt64 getTag()		{		return tag;		}
	}
}

