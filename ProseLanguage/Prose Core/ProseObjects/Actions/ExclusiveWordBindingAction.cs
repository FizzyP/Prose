using System;

namespace ProseLanguage
{
	public class ExclusiveWordBindingAction : WordBindingAction
	{
		public ExclusiveWordBindingAction(RawWord[] rawWordsToBindAsWord, Word wordToInheritFrom)
			: base(rawWordsToBindAsWord, wordToInheritFrom)
		{
			bindingSymbol = "<-";
		}

		public override void performAction (ProseRuntime runtime)
		{
			//	Create/fetc a possibly new word object representing the words.
			Word newWord = runtime.addWordFromRawWords(rawWordsToBindAsWord);
			//	Bind the word using inheritence
			newWord.isa = new Word[] { parent };
		}
	}
}

