using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	public class RawWord
	{
		//	Store the unique representatives of raw words.	This  dicitionary is used across
		//	runtimes.  It's irrelevant.
		private static Dictionary<string, RawWord> uniqueRawWordMap = new Dictionary<string, RawWord>();

		//	Instance data
		private string rawWordText;

		private RawWord (string rawWordText)
		{
			this.rawWordText = rawWordText;
		}

		public string AsString {
			get { return rawWordText;	}
		}

		//	Create a new rawWord or fetch a reference
		public static RawWord new_FromString(string rawWordText)
		{
			//	Try to fetch it from the cache.
			RawWord cachedRawWord;
			if (uniqueRawWordMap.TryGetValue(rawWordText, out cachedRawWord))
				return cachedRawWord;

			//	If we can't find it, make it, add it to the cache, and return it.
			RawWord newRawWord = new RawWord(rawWordText);
			uniqueRawWordMap.Add(rawWordText, newRawWord);
			return newRawWord;
		}


	}
}

