using System;

namespace ProseLanguage
{
	public interface ProseObject
	{
		//	Return something to display as the word/description for this object.
		string getReadableString();

		ProseObject[] getIsa();

		void setTag(UInt64 tag);
		UInt64 getTag();
	}
}

