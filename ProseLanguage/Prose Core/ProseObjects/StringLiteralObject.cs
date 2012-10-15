using System;
using System.Text.RegularExpressions;

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

		    int maxLength = 50;
		    
		    string stringToDisplay;
		    if (readMe.Length < maxLength) {
				stringToDisplay = readMe;
			}
			else {
				stringToDisplay = readMe.Substring(0, maxLength-3) + "...";
			}

			string cleanedString = Regex.Replace(stringToDisplay, "\n|\r", "");
			
			return "\"" + cleanedString + "\"";
		}
	}
}

