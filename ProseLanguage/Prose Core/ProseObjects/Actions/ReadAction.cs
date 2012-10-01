using System;

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
			int maxLength = 30;

			string stringToDisplay;
			if (readMe.Length < maxLength) {
				stringToDisplay = readMe;
			}
			else {
				stringToDisplay = readMe.Substring(0, maxLength-3) + "...";
			}

			return "Read{\"" + stringToDisplay + "\"}";
		}
	}
}

