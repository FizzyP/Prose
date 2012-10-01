using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	//
	//	Expects:  , read @string[source] ,
	//
	public class ReadPhrase : SimplePhrase
	{
		public ReadPhrase (ProseObject phraseClass, ProseObject[] phrasePattern)
			: base(phraseClass, phrasePattern)
		{
		}


		public override PNode evaluate(PNode evaluateMe, PatternMatcher successfulMatch)
		{
			PatternMatcher match = successfulMatch;
			
			//	Extract the "arguments" from the PatternMatcher.
			List<PNode> M = match.Matching;			//	The pattern -> prose index from the match
			string sourceString = ((StringLiteralObject) M[2].value).literal;
			ProseAction action = new ReadAction(sourceString);

			value = new ProseObject[3];
			value[0] = M[0].value;
			value[1] = action;
			value[2] = M[3].value;
			
			return replaceWithValueAt(evaluateMe, successfulMatch);
		}

	}
}

