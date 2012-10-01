using System;

namespace ProseLanguage
{
	public class MethodObject : ProseObjectBase, ProseObject
	{

		RawWord methodName;


		public MethodObject (RawWord methodName)
		{
			this.methodName = methodName;
		}

		public string getReadableString()
		{
			return methodName.AsString;
		}
	}
}

