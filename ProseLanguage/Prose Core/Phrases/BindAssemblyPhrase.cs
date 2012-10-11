using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	//
	//	Designed to match:
	//
	//	, load assembly : @string[file_name] <- @raw[new_assembly_word] ,
	//

	public class BindAssemblyPhrase : SimplePhrase
	{
		public BindAssemblyPhrase(ProseObject phraseClass, ProseObject[] phrasePattern)
			: base(phraseClass, phrasePattern)
		{
		}

		public override PNode evaluate(PNode evaluateMe, PatternMatcher successfulMatch)
		{
			PatternMatcher match = successfulMatch;

			//	Extract the "arguments" from the PatternMatcher.
			List<PNode> M = match.Matching;			//	The pattern -> prose index from the match
			string dllFileName = ((StringLiteralObject) M[4].value).literal;
			RawWordObject newAssemblyWord = (RawWordObject) M[6].value;
			LoadAssemblyAction action = new LoadAssemblyAction(dllFileName, newAssemblyWord);

			value = new ProseObject[3];
			value[0] = M[0].value;
			value[1] = action;
			value[2] = M[7].value;
			
			PNode ret = replaceWithValueAt(evaluateMe, successfulMatch);
			value = null;
			return ret;
		}

		public override string getStaticValueDescriptionString() {
			return "BindAssembly{}";
		}

	}
}

