using System;
using System.Collections.Generic;
using System.Text;

namespace ProseLanguage
{
	
	
	public class WordBase : ProseObject
	{
		
		private RawWord[] rawWords;
		private string readableString;
		
		public WordBase (RawWord[] words)
		{
			if (words.Length == 0)
				throw new ProseWordParserFailure("Attempt to construct an empty word.");
			
			//	Remeber the words
			rawWords = words;
			
			//	Compute readableString
			StringBuilder x = new StringBuilder();
			foreach (RawWord word in words)
			{
				x.Append(word.AsString);
				x.Append(" ");
			}
			//	Throw away the extra space at the end.
			x.Remove(x.Length - 1, 1);
			readableString = x.ToString();
		}
		
		public string getReadableString() {
			return readableString;
		}
		
		public RawWord[] RawWords {
			get {	return rawWords;	}
		}
		
		
		
		#region Inheritance
		
		//	This word inherits from these words.
		public WordBase[] isa = new Word[0];		//	We assign this at creation time.
		
		
		public ProseObject[] getIsa() {	return isa;	}
		
		//	Get a list of all words descending from this one.
		public List<ProseObject> getAllDescendents(ProseRuntime runtime) {
			return internalGetAllDescendents(runtime.getNewTag());
		}
		
		//	Use the given tag to determine which descendents we've visted before.
		private List<ProseObject> internalGetAllDescendents(UInt64 tag) {
			List<ProseObject> descendents = new List<ProseObject>(isa.Length);
			
			//	Step through the descendents
			for (int currIdx = 0; currIdx < isa.Length; currIdx++)
			{
				//	If we've visted this word before skip it.
				if (isa[currIdx].getTag() == tag)
					continue;
				
				WordBase word = (WordBase) isa[currIdx];
				//	Otherwise, mark it as visited and add it to the list descendents
				word.setTag(tag);
				descendents.Add(word);
				//	Add all the descendents of this descendent using the same tag to elminiate dups.
				descendents.AddRange(word.internalGetAllDescendents(tag));
			}
			
			return descendents;
		}
		
#endregion
		
		public void addParent(WordBase newParent)
		{
			WordBase[] newIsa = new WordBase[isa.Length + 1];
			for (int i=0; i < isa.Length; i++) {
				if (isa[i] == newParent) {
					return;
				}
				else {
					newIsa[i] = isa[i];
				}
			}
			newIsa[isa.Length] = newParent;
			isa = newIsa;
		}
		
		
		// Tags
		private UInt64 tag;
		public void setTag(UInt64 tag) {	this.tag = tag;	}
		public UInt64 getTag() {	return tag;	}
		
	}
}

