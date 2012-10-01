using System;

namespace ProseLanguage
{
	public class StringLiteralObject : ProseObjectBase, ProseObject
	{
		public string literal;

		public StringLiteralObject(string literal)
		{
			this.literal = literal;
		}

		public string getReadableString()
		{
			string readMe = literal;

		    int maxLength = 30;
		    
		    string stringToDisplay;
		    if (readMe.Length < maxLength) {
				stringToDisplay = readMe;
			}
			else {
				stringToDisplay = readMe.Substring(0, maxLength-3) + "...";
			}
			
			return "\"" + stringToDisplay + "\"";
		}
	}
}

