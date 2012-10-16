using System;
using System.Collections.Generic;

namespace ProseLanguage
{


	//
	//	Designed to match:
	//
	//	, @method[method_name] @prose[args] ,
	//
	
	public class ApplyMethodPhrase : SimplePhrase
	{
		public ApplyMethodPhrase(ProseObject phraseClass, ProseObject[] phrasePattern)
			: base(phraseClass, phrasePattern)
		{
		}

		//	Accepts pattersn:
		//	, @method @prose ,
		//	, @method ,
		public override PNode evaluate(PNode evaluateMe, PatternMatcher successfulMatch)
		{
			PatternMatcher match = successfulMatch;
			
			//	Extract the "arguments" from the PatternMatcher.
			List<PNode> M = match.Matching;			//	The pattern -> prose index from the match

			MethodNameWord methodWord = (MethodNameWord) M[1].value;
			List<ProseObject> args;
			ProseObject terminalPunctuation;
			//	Depending on whether or not our method has arguments.
			if (successfulMatch.NumObjectsMatched > 3) {
				args = successfulMatch.getArgumentAsProseAtIndex(2);
				terminalPunctuation = M[3].value;
			}
			else {
				args = new List<ProseObject>();
				terminalPunctuation = M[2].value;
			}

			ProseAction action = new MethodDelegateAction(methodWord, args);

			value = new ProseObject[3];
			value[0] = M[0].value;
			value[1] = action;
			value[2] = terminalPunctuation;

			PNode ret = replaceWithValueAt(evaluateMe, successfulMatch);
			value = null;
			return ret;
		}

		public override string getStaticValueDescriptionString() {
			return "ApplyMethod{}";
		}
		
	}
}

