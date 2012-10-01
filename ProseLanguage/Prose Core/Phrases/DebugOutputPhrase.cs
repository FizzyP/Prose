using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	public class DebugOutputPhrase : SimplePhrase
	{
		string message;

		public DebugOutputPhrase (string message, ProseObject phraseClass, ProseObject[] pattern)
			:	base(phraseClass, pattern)
		{
			this.message = message;
		}

		//	Expects pattern:    , -> @prose -> ,
		public override PNode evaluate(PNode evaluateMe, PatternMatcher successfulMatch)
		{
			//	Extract the "arguments" from the PatternMatcher.
			List<PNode> M = successfulMatch.Matching;		//	The pattern -> prose index from the match
			//	Create an action that has the desired effect
			DebugOutputAction action = new DebugOutputAction("dbg", message);
			
			//	Wrap the action in whatever punctuation was present and write the list of objects
			//	we want to substitute into our "value" field.
			value = new ProseObject[5];
			value[0] = M[0].value;		//	Initial punctuation
			value[1] = action;			//	Word binding action
			value[2] = M[4].value;		//	Terminal punctuation			
			value[3] = action;			//	Word binding action
			value[4] = M[4].value;		//	Terminal punctuation
			//	Call the super-class to do the substitution.
			return replaceWithValueAt(evaluateMe, successfulMatch);
		}
	}
}

