
/*
 * 	ProseLexer does that portion of the parsing which does not depend on the given runtime.
 * 	At reads strings and produces lists of LexerTokens, each of which binds a RawWord to piece
 *  of source.
 * 
 */


using System;
using System.Collections.Generic;
using System.Text;

namespace ProseLanguage
{
	public class ProseLexer
	{
		#region State Data
		private string source;
		private int charIdx;
		private List<LexerToken> output;


		//	Lexer State Data
		bool insideQuadQuoteExpression = false;
		Stack<char> parentheticalStack;
		Stack<int> parentheticalOpenerStack;

		#endregion

		public ProseLexer ()
		{
		}

		#region Public Interface and Helper Functions

		public List<LexerToken> parseStringIntoTokens(string newSource)
		{
			setupForParsing(newSource);

			while (charIdx < source.Length)
			{
				//	Clean up any white space before proceeding
				readWhiteSpace();
				if (charIdx >= source.Length)
					break;

				//	Analyze this character
				char sourceChar = source[charIdx];

				//	Comments
				if (sourceChar == '#') {
					readComment();
					continue;
				}

				//	Double Quotes and Quadruple Quotes
				if (sourceChar == '\"') {
					readQuoteBlock();
					continue;
				}

				//	Read a contiguous word
				if (CharCouldBeginWord(sourceChar)) {
					readRawWord();
					continue;
				}

				if (CharCouldBeginSymbol(sourceChar)) {
					readSymbol();
					continue;
				}

				if (CharIsParenthetical(sourceChar)) {
					readParenthetical();
					continue;
				}

				//	If we make it here, we either have junk, or we have something
				//	inside a text block.
				if (insideQuadQuoteExpression)
				{
					output.Add(new LexerToken(sourceChar.ToString(), charIdx, charIdx+1));
					charIdx++;
				}
				else
					throw new LexerSourceException("Invalid symbol.", source, charIdx, charIdx+1);

			}

			finalCheckAfterParsing();

			return output;
		}

		private void setupForParsing(string newSource)
		{
			source = newSource;
			output = new List<LexerToken>();
			charIdx = 0;
			insideQuadQuoteExpression = false;
			parentheticalStack = new Stack<char>();
			parentheticalOpenerStack = new Stack<int>();
		}

		private void finalCheckAfterParsing()
		{
			if (charIdx != source.Length)
				throw new LexerFailure("Index/source length mismatch: charIdx = " + charIdx + ", and source.Length = " + source.Length +".");
			if (parentheticalStack.Count != 0)
			{
				int openerIdx;
				char opener = peekParentheticalFromStack(out openerIdx);
				throw new LexerSourceException("Source ended before parenthetical or text expression terminal symbol(s).",
				                               source, openerIdx, charIdx);
			}
		}

		#endregion

		//	Protocol for readers:
		//
		//	When a reader is called, it is always called with charIdx at the beginning of the relevant
		//	piece of source.  (e.g. for comment, charIdx points at the #)
		//
		//	When a reader exits, it leaves charIdx pointing at the character after the relevant text.
		//
		//	A reader always double checks: in the case of a comment, the reader checks the # as well as
		//	whichever function decided to call readComment in the first place.
		//

		#region Various Readers

		private void readWhiteSpace()
		{
			while (charIdx < source.Length && char.IsWhiteSpace(source[charIdx]))
				charIdx++;
		}

		private void readComment()
		{
			if ( source[charIdx] != '#')
				throw new ProseWordParserFailure("");

			charIdx++;
			skipSourceUntilNextLine();
		}

		private void skipSourceUntilNextLine()
		{
			bool lfRead = false;
			bool crRead = false;

			while (charIdx < source.Length)
			{
				char c = source[charIdx];
				if (c == '\n') {
					if (lfRead) return;
					lfRead = true;
				}
				else if (c == '\r') {
					if (crRead) return;
					crRead = true;
				}
				else {
					if (lfRead || crRead)
					{
						//	If we read a different character after starting some lf/cr then
						//	we count that as a line feed and quit.
						return;
					}

					//	OTherwise, just advance the charIdx.
					charIdx++;
				}
			}
		}
			
		private void readQuoteBlock()
		{
			int quoteCount = 0;
			while (charIdx < source.Length &&
			       source[charIdx] == '\"')
			{
				quoteCount++;
				charIdx++;
			}

			switch (quoteCount)
			{
			case 0:
				//	This function never should have been called.
				throw new LexerFailure("");
				break;
			
			case 1:
				if (insideQuadQuoteExpression)
				{
					//	In this case a quote is an actual quote character.
					output.Add(new LexerToken("\"", charIdx-1, charIdx));
					return;
				}
				else {
					//	Back up the read head and read a string literal starting at the quote.
					charIdx--;
					readStringLiteral();
					return;
				}
				break;

			case 2:
				//	Add the "" to the output ('T' stands for TEXT and represents the "" symbol in the parenthetical stacks.
				writeParentheticalToken('T', charIdx - quoteCount);
				break;
			
			default:
				throw new LexerSourceException("More then two consecutive double quotes does not constitute a legal symbol.",
				                               source, charIdx - quoteCount, charIdx);
				break;

			}
		}

		private void readStringLiteral()
		{
			if (source[charIdx] != '\"')
				throw new LexerFailure("");

			StringBuilder stringLiteral = new StringBuilder();
			int startStringCharIdx = charIdx;		//	Record the location of the first "

			charIdx++;
			bool isEscaped = false;
			bool endQuoteFound = false;
			for (;charIdx < source.Length; charIdx++)
			{
				char c = source[charIdx];		//	The current character under consideration

				if (isEscaped)
				{
					//TODO	Add support for escaped characters in strings

					//	Just skip the escaped character and leave escaped mode
					isEscaped = false;
					continue;
				}
				else
				{
					if (c == '\"')
					{
						endQuoteFound = true;
						break;
					}
					else if (c == '\\')
					{
						isEscaped = true;
						continue;
					}
					else {
						stringLiteral.Append(c);
						continue;
					}
				}


			}



			if (!endQuoteFound) {
				throw new LexerSourceException("Beginning \" missing end \".",
				                               source, startStringCharIdx - 1, charIdx);
			}


			charIdx++;	//	Move the index to point after the last " in the string.
			int endStringCharIdx = charIdx;

			output.Add(LexerToken.new_StringToken(stringLiteral.ToString(), startStringCharIdx, endStringCharIdx));
		}

		private static bool CharCouldBeginWord(char c) {
			return 	Char.IsLetter(c)
					||	c == '_'		//	Symbols to "glue words together"
					||	c == '@';		//	Private words inside the language begin with @
		}

		private static bool CharCouldLieInWord(char c) {
			return 	Char.IsLetter(c)
					||	c == '_'		//	Symbols to "glue words together"
					||	c == '@';		//	Private words inside the language begin with @
		}

		private void readRawWord() {
			int wordStartIdx = charIdx;
			for (; charIdx < source.Length; charIdx++) {
				if (!CharCouldLieInWord (source[charIdx]))
					break;
			}

			if (charIdx - wordStartIdx == 0)
				throw new LexerFailure("");

			int wordEndIdx = charIdx;
			output.Add(new LexerToken(source.Substring(wordStartIdx, wordEndIdx - wordStartIdx), wordStartIdx, wordEndIdx));
		}

		#endregion

		//	Symbol List
		//
		//
		//	Punctuation:
		//		:	,	;	.	!	?
		//
		//	Inheritance
		//		:	<-	-:	+:
		//
		//	Phrase Binding
		//		:	->	:-	:+
		//
		//	Parentheticals
		//		""	(	)	[	]	{	}		(note, "" doesn't count as a symbol in this sense because it's handled separately).
		//
		//	All Characters Involved
		//		:	,	;	.	!	?
		//		<	-	+
		//		>
		//		(	)	[	]	{	}

		#region Read Symbols

		private static bool CharCouldBeginSymbol(char c)
		{
			//	Punctuation
			if (		c == ':'
			        ||	c == ','
			        ||	c == ';'
			        ||	c == '.'
			        ||	c == '!'
			        ||	c == '?'
			)
				return true;

			//	Inheritance
			if (		c == '<'		//	from <-
			  		||	c == '-'		//	from -:
			    	||	c == '+'		//	from +:
			    						//	note : already done above
			    )
				return true;

			//	Phrase binding
			//	Need :  ->  :-  and :+ which are already  taken care of above

			return false;
		}

		private void readSymbol()
		{
			string sourceCharAsString = source[charIdx].ToString();

			switch(source[charIdx])
			{
				//	All these characters just need to be inserted with no special consideration.
			case ',':
			case ';':
			case '.':
			case '!':
			case '?':
				output.Add(new LexerToken(sourceCharAsString, charIdx, charIdx + 1));
				charIdx++;
				break;

			case ':':
				//	When in quadquotes this is a literal
				if (insideQuadQuoteExpression) {
					output.Add(new LexerToken(sourceCharAsString, charIdx, charIdx + 1));
					charIdx++;
				}
				else {
					readSymbolStartingWithColon();
				}
				break;

			case '<':				//	Read <-
				//	When in quadquotes this is a literal
				if (insideQuadQuoteExpression) {
					output.Add(new LexerToken(sourceCharAsString, charIdx, charIdx + 1));
					charIdx++;
				}
				else
					readLeftArrowSymbol();
				break;

			case '-':				//	Read -> or -:
				//	When in quadquotes this is a literal
				if (insideQuadQuoteExpression) {
					output.Add(new LexerToken(sourceCharAsString, charIdx, charIdx + 1));
					charIdx++;
				}
				else
					readRightArrowOrMinusColon();
				break;

			case '+':				//	Read +:
				//	When in quadquotes this is a literal
				if (insideQuadQuoteExpression) {
					output.Add(new LexerToken(sourceCharAsString, charIdx, charIdx + 1));
					charIdx++;
				}
				else
					readPlusColonSymbol();
				break;
			
			//	Case '>': doesn't need to happen because it never comes first.

			default:
				throw new LexerFailure("Mishandled symbol.");
			}
		}

		private void readLeftArrowSymbol()
		{
			if (source[charIdx] != '<')
				throw new LexerFailure("");

			charIdx++;
			if (charIdx >= source.Length)
				throw new LexerSourceException("Left arrow missing tail.", source, charIdx - 1, charIdx);
			if (source[charIdx] != '-')
				throw new LexerSourceException("Left arrow missing tail.", source, charIdx - 1, charIdx);
			//	Found a left arrow so add the symbl
			output.Add (new LexerToken("<-", charIdx - 1, charIdx + 1));
			charIdx++;
		}

		private void readRightArrowOrMinusColon()
		{
			if (source[charIdx] != '-')
				throw new LexerFailure("");

			charIdx++;
			if (charIdx >= source.Length)
				throw new LexerSourceException("Right arrow missing tail or -: missing : .", source, charIdx - 1, charIdx);

			if (source[charIdx] == '>')
			{
				output.Add (new LexerToken("->", charIdx - 1, charIdx + 1));
			}
			else if (source[charIdx] == ':')
			{
				output.Add (new LexerToken("-:", charIdx - 1, charIdx + 1));
			}
			else
				throw new LexerSourceException("Right arrow missing tail or -: missing : .", source, charIdx - 1, charIdx);

			charIdx++;
		}

		private void readPlusColonSymbol()
		{
			if (source[charIdx] != '+')
				throw new LexerFailure("");

			charIdx++;
			if (charIdx >= source.Length)
				throw new LexerSourceException("+: missing colon.", source, charIdx - 1, charIdx);
			if (source[charIdx] != ':')
				throw new LexerSourceException("+: missing colon.", source, charIdx - 1, charIdx);
			//	Found a +: so add the symbl
			output.Add (new LexerToken("+:", charIdx - 1, charIdx + 1));
			charIdx++;
		}

		private void readSymbolStartingWithColon()
		{
			if (source[charIdx] != ':')
				throw new LexerFailure("");

			//	Check for a lone colon
			charIdx++;
			if (charIdx < source.Length)
			{
				char c = source[charIdx];
				if (c == '-') {
					output.Add (new LexerToken(":-", charIdx - 1, charIdx + 1));
					charIdx++;
				}
				else if (c == '+') {
					output.Add (new LexerToken(":+", charIdx - 1, charIdx + 1));
					charIdx++;
				}
				else {
					//	Just a lone :
					output.Add (new LexerToken(":", charIdx - 1, charIdx));
				}
			}
			else {
				//	Just a lone :
				output.Add (new LexerToken(":", charIdx - 1, charIdx));
			}
		}

		private bool CharIsParenthetical(char c)
		{
			return (	c == '('
			        ||	c == ')'
			        ||	c == '['
			        ||	c == ']'
			        || 	c == '{'
			        ||	c == '}'
			        );
		}

		private void readParenthetical()
		{
			//	Nothing else to do but try to write it.  If there are problems,
			//	with entanglement, this method will throw an exception.
			writeParentheticalToken(source[charIdx], charIdx);
			charIdx++;
		}

		#endregion

		#region Utilities


		#endregion

		#region Output Methods

		private void writeParentheticalToken(char c, int idx)
		{
			switch (c)
			{
				//	All openers all legal
			case '(':
			case '[':
			case '{':
				pushParentheticalOnStack(c, idx);
				break;

			case ')':
			{
				if (parentheticalStack.Count == 0)
					throw new LexerSourceException("Right parentheses missing left parentheses", source, idx, idx+1);
				int openerIdx;
				char opener = popParentheticalFromStack(out openerIdx);
				if (opener != '(')
					throw new LexerSourceException("Parenthetic entanglement.", source, openerIdx, idx+1);
				break;
			}
			
			case ']':
			{
				if (parentheticalStack.Count == 0)
					throw new LexerSourceException("Right square bracket missing left square bracket", source, idx, idx+1);
				int openerIdx;
				char opener = popParentheticalFromStack(out openerIdx);
				if (opener != '[')
					throw new LexerSourceException("Parenthetic entanglement.", source, openerIdx, idx+1);
				break;
			}

			case '}':
			{
				if (parentheticalStack.Count == 0)
					throw new LexerSourceException("Right curly bracket missing left curly bracket", source, idx, idx+1);
				int openerIdx;
				char opener = popParentheticalFromStack(out openerIdx);
				if (opener != '{')
					throw new LexerSourceException("Parenthetic entanglement.", source, openerIdx, idx+1);
				break;
			}

				//	'T' represents "" and stands for "TEXT".  T starts and ends itself.
			case 'T':
			{
				if (insideQuadQuoteExpression)
				{
					//	This is a terminal ""
					if (parentheticalStack.Count == 0)
						throw new LexerFailure("Impossible parenthetical stack underflow.");
					int openerIdx;
					char opener = popParentheticalFromStack(out openerIdx);
					if (opener != 'T')
						throw new LexerSourceException("Quad-quote entangled with parenthetical.", source, openerIdx, idx+1);
					//	Everything's fine, so record that we're out
					insideQuadQuoteExpression = false;
				}
				else
				{
					//	This is an initial ""
					pushParentheticalOnStack(c, idx);
					insideQuadQuoteExpression = true;
				}
				break;
			}
			}

			//	Actually write the token to output
			if (c == 'T')
				output.Add(new LexerToken("\"\"", idx, idx + 2));
			else
				output.Add(new LexerToken(c.ToString(), idx, idx+1));
		}

		#endregion

		#region Parenthetical Stack

		private void pushParentheticalOnStack(char c, int idx)
		{
			parentheticalStack.Push(c);
			parentheticalOpenerStack.Push(idx);
		}

		private char popParentheticalFromStack(out int idx)
		{
			idx = parentheticalOpenerStack.Pop();
			return parentheticalStack.Pop ();
		}

		private char peekParentheticalFromStack(out int idx)
		{
			idx = parentheticalOpenerStack.Peek();
			return parentheticalStack.Peek();
		}


		#endregion
	}
}



































