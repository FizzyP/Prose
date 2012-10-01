using System;

namespace ProseLanguage
{
	public class RawWordObject : ProseObjectBase, ProseObject
	{
		private RawWord rawWord;
		private RawWord[] rawWordAsArray;		//	So we don't have to call new later

		public RawWord[] RawWords {
			get {
				return rawWordAsArray;
			}
		}

		public RawWordObject (RawWord rawWord)
		{
			this.rawWord = rawWord;
			this.rawWordAsArray = new RawWord[] { rawWord };		//	So we don't have to call new later
		}

		public string getReadableString()
		{
			return rawWord.AsString;
		}
	}
}

