using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	public class BreakPointPhrase : SimplePhrase
	{
		public BreakPointPhrase(ProseObject phraseClass, ProseObject[] phrasePattern)
			: base(phraseClass, phrasePattern)
		{
		}

		//
		//	Meant to match:
		//
		//	@break
		//

		public override PNode evaluate(PNode evaluateMe, PatternMatcher successfulMatch)
		{
			PatternMatcher match = successfulMatch;
			
			//	Extract the "arguments" from the PatternMatcher.
			List<PNode> M = match.Matching;			//	The pattern -> prose index from the match

			value = new ProseObject[1];
			//value[0] = new BreakPointObject("");
			value[0] = new BreakPointObject("# Breakpoint script - \n");

			PNode ret = replaceWithValueAt(evaluateMe, successfulMatch);
			value = null;
			return ret;
		}

		public override string getStaticValueDescriptionString() {
			return "$BREAK$";
		}

	}

}

