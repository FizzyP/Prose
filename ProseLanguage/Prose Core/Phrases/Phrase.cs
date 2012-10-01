using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	public interface Phrase : ProseObject
	{
		//	Take a node pointing at the beginning of a list and "evaluate the phrase"
		//	i.e. replace the beginning part of the list with what it evaluates to.
		//	Return a pointer to the new beginning of this list.
		//	The PatternMatcher is the matcher that matched this phrase.
		//	It is used to determine the "arguments" which have already been
		//	parsed out of the code starting at "evaluateMe".
		PNode evaluate(PNode evaluateMe, PatternMatcher successfulMatch);

		ProseObject[] getPattern();

		ProseObject getPhraseClass();

	}
}

