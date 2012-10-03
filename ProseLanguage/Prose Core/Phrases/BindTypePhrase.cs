using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProseLanguage
{
	//
	//	Designed to match:
	//
	//	,  assembly : @string[file_name] <- @raw[new_assembly_word] ,
	//
	
	public class BindTypePhrase : SimplePhrase
	{
		public BindTypePhrase(ProseObject phraseClass, ProseObject[] phrasePattern)
			: base(phraseClass, phrasePattern)
		{
		}
		
		public override PNode evaluate(PNode evaluateMe, PatternMatcher successfulMatch)
		{
			PatternMatcher match = successfulMatch;
			
			//	Extract the "arguments" from the PatternMatcher.
			List<PNode> M = match.Matching;			//	The pattern -> prose index from the match
			AssemblyNameWord asmbly = ((AssemblyNameWord) M[1].value);
			string typeName = ((StringLiteralObject) M[4].value).literal;
			RawWord[] newTypeWord;
			if (M[6].value is RawWordObject)	newTypeWord = ((RawWordObject) M[6].value).RawWords;
			else newTypeWord = ((Word) M[6].value).RawWords;
			BindTypeAction action = new BindTypeAction(asmbly, typeName, newTypeWord);
			
			value = new ProseObject[3];
			value[0] = M[0].value;
			value[1] = action;
			value[2] = M[7].value;
			
			return replaceWithValueAt(evaluateMe, successfulMatch);
		}
		
	}
}

