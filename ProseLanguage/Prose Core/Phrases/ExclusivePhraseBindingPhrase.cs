using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	//
	//		Pattern:
	//
	//		, word[class] : @pattern[p] -> @prose[v] ,
	//
	//		Replaces with
	//
	//		, XBindPhrase{class: p -> v} ,
	//

	public class ExclusivePhraseBindingPhrase : SimplePhrase
	{

		//	Create a phrase which binds phrases.  The phrase itself uses phraseClass and
		//	phrasePattern.  The phrase to be bound is entirely described by the PatternMatcher.
		public ExclusivePhraseBindingPhrase(ProseObject phraseClass, ProseObject[] phrasePattern)
			: base(phraseClass, phrasePattern)
		{
		}

		public override PNode evaluate(PNode evaluateMe, PatternMatcher successfulMatch)
		{
			PatternMatcher match = successfulMatch;
			//	Create the phrase represented by the user's code
			//	Extract the pattern from the match
			ProseObject[] pattern = match.CurrPatternObject.Pattern;
			ProseObject[] argNames = match.CurrPatternObject.elementNames.ToArray();
			//	Create a value[] array for the new phrase
			ProseObject[] pvalue = match.getArgumentAsProseAtIndex(5).ToArray();
			//	Go through and make substitutions for the arguments
			PatternObject po = match.CurrPatternObject;
			for(int i=0; i < po.Length; i++) {
				ProseObject argName = po.elementNames[i];
				if (argName == null)
					continue;

				//	If we actually have an argument name, then scan through
				//	the value array and replace instances of that variable name
				//	with ArgRefObjects.
				for(int j=0; j < pvalue.Length; j++) {
					if (pvalue[j] == argName) {
						//	Replace this object with a reference to the pattern.
						pvalue[j] = new ArgRefObject(i);
					}
				}
			}

			//	Follow rules regarding , ; .
			//	1.	When matching these on the beginning or end of a pattern, they automatically
			//		generate ArgRefObjects on the beginning/end of the corresponding value. This
			//		means punctuation never gets downgraded.
			//	2.	Shouldn't allow stronger punctuation on the inside than on the outside, but
			//		this is a little tougher to check.
			ProseRuntime runtime = successfulMatch.Runtime;
			if (pattern[0] == runtime.Comma || pattern[0] == runtime.Semicolon) {
				//	Add a ref to the beginning
				ProseObject[] newValue = new ProseObject[pvalue.Length + 1];
				Array.Copy(pvalue, 0, newValue, 1, pvalue.Length);
				newValue[0] = new ArgRefObject(0);
				pvalue = newValue;
			}
			int patternEnd = pattern.Length - 1;
			if (pattern[patternEnd] == runtime.Comma || pattern[patternEnd] == runtime.Semicolon) {
				//	Add a ref to the beginning
				ProseObject[] newValue = new ProseObject[pvalue.Length + 1];
				Array.Copy(pvalue, 0, newValue, 0, pvalue.Length);
				newValue[pvalue.Length] = new ArgRefObject(patternEnd);
				pvalue = newValue;
			}


			
			//	Create a simple phrase from these ingredients
			SimplePhrase newPhrase = new SimplePhrase(match.Matching[1].value, pattern, argNames, pvalue);
			
			//	Extract the "arguments" from the PatternMatcher.
			List<PNode> M = successfulMatch.Matching;		//	The pattern -> prose index from the match

			value = new ProseObject[3];
			value[0] = M[0].value;
			value[1] = new ExclusivePhraseBindingAction(newPhrase);
			value[2] = M[6].value;

			PNode ret = replaceWithValueAt(evaluateMe, successfulMatch);
			value = null;
			return ret;
		}

		public override string getStaticValueDescriptionString() {
			return "XBindPhrase{}";
		}

	}
}

