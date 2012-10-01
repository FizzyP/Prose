using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace Prose
{
	public class ProseSourcePreParser
	{
		public ProseSourcePreParser ()
		{
		}


		public RawWord[] parseSourceIntoRawWords(string source)
		{
			StringBuilder strippedSource = stripAndCleanSource(source);
			string[] wordStrings = parseCleanedSourceIntoWordStrings(strippedSource);
			//	Convert to RawWord objects.
			RawWord[] outRawWords = new RawWord[wordStrings.Length];
			int i=0;
			foreach (string wordString in wordStrings)
			{
				outRawWords[i] = RawWord.new_FromString(wordString);
				i++;
			}
			return outRawWords;
		}


		//	Remove comments or other non-code content from the source.
		StringBuilder stripAndCleanSource(string source)
		{
			return new StringBuilder(source);
		}



		string[] parseCleanedSourceIntoWordStrings(StringBuilder strippedSource)
		{            
			string script = strippedSource.ToString();

			List<string> words = new List<string>();

            int lastIdx = 0;
            bool inStringLiteral = false;

            string subString;
            string inflatedSubString;
            string[] newWords;

            for (int idx = 0; idx < script.Length; idx++)
            {
                if (script[idx] == '\"')
                {
                    if (inStringLiteral)
                    {
                        //  Add the literal as its own word
                        words.Add(script.Substring(lastIdx, idx - lastIdx));
                        words.Add("\"");
                        inStringLiteral = false;
                        lastIdx = idx + 1;
                    }
                    else
                    {
                        //  Add the words from this block of code
                        subString = script.Substring(lastIdx, idx - lastIdx);
                        
						//DEBUG
						inflatedSubString = inflate(subString);
						//inflatedSubString = subString;


						newWords = inflatedSubString.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string newWord in newWords)
                            words.Add(newWord);
                        words.Add("\"");
                        inStringLiteral = true;
                        lastIdx = idx + 1;
                    }
                }
            }

            //  If there are any remaining words after the final quote, add them.
            //  Add the words from this block of code
            subString = script.Substring(lastIdx, script.Length - lastIdx);
            inflatedSubString = inflate(subString);
			newWords = inflatedSubString.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
            foreach (string newWord in newWords)
                words.Add(newWord);

            //  Dump the results to an array.
            string[] wordArray = words.ToArray<string>();

            //  Make sure that quotes are done by the time the file is done!
            if (inStringLiteral)
                throw new ProseValidationException("File ended before string literal."); 

			return wordArray;
		}



		static string inflate(string text)
		{
            StringBuilder formattedString = new StringBuilder();

            foreach (char x in text)
            {
                if (inflaterChars.Contains<char>(x))
                {
                    formattedString.Append(' ' + x.ToString() + ' ');
                }
                else
                {
                    formattedString.Append(x);
                }
            }

            return formattedString.ToString();
		}



		///////////////////////////////////////////////////////////////////////////////////////////////
        #region  Static Data

        //  We add spaces on either side of these characters
        private static char[] inflaterChars = { '\"', '(', ')', '.', ':' };

        //  We break words at these characters
        private static char[] delimiterChars = { ' ', '\t', '\n', '\r' };

        #endregion
	}
}

