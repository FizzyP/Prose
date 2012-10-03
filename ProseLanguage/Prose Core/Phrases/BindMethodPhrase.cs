using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	//
	//	Designed to match:
	//
	//	, @type[type_name] method : @string[method_name] <- @raw[new_method_word] ,
	//
	
	public class BindMethodPhrase : SimplePhrase
	{
		public BindMethodPhrase(ProseObject phraseClass, ProseObject[] phrasePattern)
			: base(phraseClass, phrasePattern)
		{
		}
		
		public override PNode evaluate(PNode evaluateMe, PatternMatcher successfulMatch)
		{
			PatternMatcher match = successfulMatch;
			
			//	Extract the "arguments" from the PatternMatcher.
			List<PNode> M = match.Matching;			//	The pattern -> prose index from the match
			TypeNameWord typeNameWord = ((TypeNameWord) M[1].value);
			string methodName = ((StringLiteralObject) M[4].value).literal;
			RawWord[] newMethodWord;
			if (M[6].value is RawWordObject)	newMethodWord = ((RawWordObject) M[6].value).RawWords;
			else newMethodWord = ((Word) M[6].value).RawWords;
			BindMethodAction action = new BindMethodAction(typeNameWord, methodName, newMethodWord);
			
			value = new ProseObject[3];
			value[0] = M[0].value;
			value[1] = action;
			value[2] = M[7].value;
			
			return replaceWithValueAt(evaluateMe, successfulMatch);
		}
		
	}
}

