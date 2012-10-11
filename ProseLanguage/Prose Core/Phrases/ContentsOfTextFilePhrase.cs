using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	public class ContentsOfTextFilePhrase : SimplePhrase
	{
		public ContentsOfTextFilePhrase (ProseObject phraseClass, ProseObject[] phrasePattern)
			: base(phraseClass, phrasePattern)
		{
		}


		//	Expects pattern:	contents of text file @string[file_name]
		public override PNode evaluate(PNode evaluateMe, PatternMatcher successfulMatch)
		{
			//	Extract the "arguments" from the PatternMatcher.
			List<PNode> M = successfulMatch.Matching;		//	The pattern -> prose index from the match
			string fileName = ((StringLiteralObject) M[4].value).literal;

			string source = System.IO.File.ReadAllText(fileName);

			//	Wrap the action in whatever punctuation was present and write the list of objects
			//	we want to substitute into our "value" field.
			value = new ProseObject[1];
			value[0] = new StringLiteralObject(source);
			//	Call the super-class to do the substitution.

			PNode ret = replaceWithValueAt(evaluateMe, successfulMatch);
			value = null;
			return ret;
		}

		public override string getStaticValueDescriptionString() {
			return "\"...text file contents...\"";
		}

	}
}

