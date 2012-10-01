using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	public class WordBindingPhrase : SimplePhrase
	{
		//
		//	The constructor expects a pattern like:
		//
		//		, existing_word : word_or_raw_word ,
		//
		//	Possibly we will use different punctuation so, we allow the possibility of creating an
		//	instance of this phrase with a differnet pattern.
		//

		public WordBindingPhrase(ProseObject phraseClass, ProseObject[] pattern)
			: base(phraseClass, pattern)
		{
		}

		//	Replaces a "parent : child" expression with an action that effects the
		//	word creation/inheritance assertion.
		//override
		public override PNode evaluate(PNode evaluateMe, PatternMatcher successfulMatch)
		{
			//	Extract the "arguments" from the PatternMatcher.
			List<PNode> M = successfulMatch.Matching;		//	The pattern -> prose index from the match
			ProseObject parentObject = M[1].value;
			ProseObject childObject = M[3].value;

			//	The childObject must be either a word or raw-word.  Either way we just use the raw word
			//	data because it will result in looking up the correct word in the end anyway.
			RawWord[] childObjectRawWords;
			if (childObject is RawWordObject) {
				childObjectRawWords = ((RawWordObject) childObject).RawWords;
			}
			else if (childObject is Word) {
				childObjectRawWords = ((Word) childObject).RawWords;
			}
			else {
				throw new RuntimeFailure("WordBindingPhrase passed something other than a word or raw word to bind.");
			}

			//	Create an action that has the desired effect
			WordBindingAction action = new WordBindingAction(childObjectRawWords, (Word) parentObject);

			//	Wrap the action in whatever punctuation was present and write the list of objects
			//	we want to substitute into our "value" field.
			value = new ProseObject[3];
			value[0] = M[0].value;		//	Initial punctuation
			value[1] = action;			//	Word binding action
			value[2] = M[4].value;		//	Terminal punctuation

			//	Call the super-class to do the substitution.
			return replaceWithValueAt(evaluateMe, successfulMatch);
		}

	}
}

