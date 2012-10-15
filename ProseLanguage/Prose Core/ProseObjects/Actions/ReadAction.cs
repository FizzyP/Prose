using System;
using System.Text.RegularExpressions;

namespace ProseLanguage
{
	public class ReadAction : ProseObjectBase, ProseAction
	{
		string readMe;

		public ReadAction (string proseSourceToRead)
		{
			readMe = proseSourceToRead;
		}

		public void performAction(ProseRuntime runtime)
		{
			runtime.read(readMe, runtime.GlobalClient);
		}

		public bool IsPure {	get {	return false;	}	}

		public string getReadableString()
		{
			int maxLength = 50;

			string stringToDisplay;
			if (readMe.Length < maxLength) {
				stringToDisplay = readMe;
			}
			else {
				stringToDisplay = readMe.Substring(0, maxLength-3) + "...";
			}
			string cleanedString = Regex.Replace(stringToDisplay, "\n|\r", "");

			return "Read{\"" + cleanedString + "\"}";
		}
	}
}

