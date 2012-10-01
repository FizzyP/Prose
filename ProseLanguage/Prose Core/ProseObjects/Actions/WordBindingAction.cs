using System;
using System.Text;

namespace ProseLanguage
{
	public class WordBindingAction : ProseAction
	{
		protected RawWord[] rawWordsToBindAsWord;
		protected Word parent;

		//	The symbol used to describe this action
		protected string bindingSymbol = "+:";

		public WordBindingAction (RawWord[] rawWordsToBindAsWord, Word wordToInheritFrom)
		{

			this.rawWordsToBindAsWord = rawWordsToBindAsWord;
			parent = wordToInheritFrom;
		}

		public bool IsPure {	get {	return false;	}	}

		public string getReadableString() {
			//	Put together the raw words
			StringBuilder rawWordsAsString = new StringBuilder();
			foreach (RawWord rawWord in rawWordsToBindAsWord)
			{
				rawWordsAsString.Append(rawWord.AsString);
				rawWordsAsString.Append(" ");
			}
			if (rawWordsToBindAsWord.Length != 0)
				rawWordsAsString.Remove(rawWordsAsString.Length - 1, 1);

			return bindingSymbol + "{" + parent.getReadableString() + " " + rawWordsAsString + "}";
		}

		virtual public void performAction(ProseRuntime runtime)
		{
			//	Create/fetc a possibly new word object representing the words.
			Word newWord = runtime.addWordFromRawWords(rawWordsToBindAsWord);
			//	Bind the word using inheritence
			Word[] newIsa = new Word[newWord.isa.Length + 1];		//	Copy all the old inherited words.
			Array.Copy(newWord.isa, newIsa, newWord.isa.Length);
			newIsa[newWord.isa.Length] = parent;					//	Add this new one.
			newWord.isa = newIsa;
		}

		public ProseObject[] getIsa() {	return new ProseObject[0];	}

		// Tags
		private UInt64 tag;
		public void setTag(UInt64 tag) {	this.tag = tag;	}
		public UInt64 getTag() {	return tag;	}

	}
}

