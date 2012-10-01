
/*
 * 	The RuntimeLexer parses LexerTokens (which are essentially raw words) into ProseObjects.
 * 	Those ProseObjects are stored in PNodes assembled into a linked list structure with
 * 	attached debug data.
 * 
 * 	The RuntimeLexer uses runtime information, whereas the ProseLexer explicitly does not.
 */

using System;
using System.Collections.Generic;


namespace ProseLanguage
{
	public class RuntimeLexer
	{

		//	The Runtime and Client being used for parsing at the moment.
		private ProseRuntime runtime;
		private ProseClient who;
		//	Store the beginning of the output and the output head.
		private PNode outputRoot;
		private PNode output;


		private List<LexerToken> sourceTokenList = null;
		private LexerToken[] sourceTokens;
		int tokenIdx = 0;


		//	Parsing data
		private bool insideQuadquoteExpression = false;
		private int lastQuadquoteIdx = -1;

		public RuntimeLexer ()
		{
		}


		public PNode parseTokensIntoPNodes(ProseRuntime runtime, ProseClient who, List<LexerToken> tokens)
		{
			//	Save the parameters
			this.who = who;
			this.runtime = runtime;
			sourceTokenList = tokens;

			setupForParsing();

			Trie<RawWord, Word>.Node currWordLookupNode, wordLookupRoot, lastGoodNode;
			wordLookupRoot = runtime.getWordLookupRoot();
			currWordLookupNode = wordLookupRoot;
			lastGoodNode = null;
			int lastGoodNodeTokenIdx = -1;
			//	Record whether or not we're in the process of building up a word.
			bool isBuildingWord = false;
			int lastProcessedTokenIdx = -1;

			while (tokenIdx < tokens.Count)
			{
				LexerToken token = tokens[tokenIdx];

				//
				//	Deal With Quadquotes.
				//
				if (token.rawWord == ProseLanguage.Raw.Quadquote) {
					//	First clean up any word we may be building and write it to output.
					if (isBuildingWord)
					{
						if (lastGoodNode == null)
						{
//							//	If we're inside a quadquote block then this is fine: even if we don't have
//							//	a legitimate word we can still wrap rawwords.  Otherwise it's an error.
//							if (insideQuadquoteExpression)
							{
								//	If there is no last good match, then take the raw words we've passed
								//	and dump them all into raw word objects.
								for (int i=lastProcessedTokenIdx + 1; i < tokenIdx; i++) {
									writePNode(new PNode(new RawWordObject(tokens[i].rawWord)));
								}
								//	Update everything so we continue after this point
								lastGoodNodeTokenIdx = tokenIdx - 1;
								lastProcessedTokenIdx = lastGoodNodeTokenIdx;
								currWordLookupNode = wordLookupRoot;
								isBuildingWord = false;
								//	Don't bother updating tokenIdx because we need to look at the word again.
							}
//							else {
//								throw new RuntimeLexerSourceException("Unrecognized word or symbol.", tokens[tokenIdx-1]);
//							}
						}
						else {
							writePNode(new PNode(lastGoodNode.Value));
							//	Reset everything so we're looking for a new word again.
							lastGoodNode = null;
							currWordLookupNode = wordLookupRoot;
							isBuildingWord = false;
							lastGoodNodeTokenIdx = tokenIdx;
							lastProcessedTokenIdx = lastGoodNodeTokenIdx;
						}

					}

					//	Output a quadquote
					writePNode(new PNode(runtime.Quadquote));
					//	Toggle our quad-quote-state.
					insideQuadquoteExpression = !insideQuadquoteExpression;
					lastQuadquoteIdx = tokenIdx;
					lastProcessedTokenIdx = tokenIdx;
					//	Continue
					tokenIdx++;
					continue;
				}



				if (insideQuadquoteExpression)
				{
					if (token.tokenType != LexerToken.TYPE.UNCLASSIFIED)
						throw new RuntimeLexerFailure("Static Lexer Failed Token Classification.");

					//
					//	This code is essentially copied from the LexerToken.TYPE.UNCLASSIFIED block below.
					//	The only major difference is that instead of throwing an exception we wrap unknown
					//	text inside raw word objects.
					//
					isBuildingWord = true;
					//	Try to continue the current word matching.
					Trie<RawWord, Word>.Node nodeForThisRawWord = currWordLookupNode.getChildNode(token.rawWord);
					//	If we can't continue this way...
					if (nodeForThisRawWord == null)
					{
						//...then whatever our last good match was is the correct word.
						if (lastGoodNode == null) {
							//	If there is no last good match, then take the raw words we've passed
							//	and dump them all into raw word objects.
							for (int i=lastProcessedTokenIdx + 1; i < tokenIdx; i++) {
								writePNode(new PNode(new RawWordObject(tokens[i].rawWord)));
							}
							lastProcessedTokenIdx = tokenIdx - 1;
							//	Update everything so we continue after this point
							//	Don't bother updating tokenIdx because we need to look at the word again.
							//	Do update currWordLookupNode
							currWordLookupNode = wordLookupRoot.getChildNode(token.rawWord);
							//	If there's no node at all, we have to deal with it now
							if (currWordLookupNode == null)
							{
								writePNode(new PNode(new RawWordObject(token.rawWord)));
								lastGoodNodeTokenIdx = tokenIdx;
								isBuildingWord = false;
								currWordLookupNode = wordLookupRoot;
								lastProcessedTokenIdx = tokenIdx;
								tokenIdx++;
							}
							else {
								isBuildingWord = true;
								lastGoodNodeTokenIdx = tokenIdx - 1;
								tokenIdx++;
							}
							continue;
						}

						writePNode(new PNode(lastGoodNode.Value));
						//	Reset everything so we're looking for a new word again.
						lastGoodNode = null;
						currWordLookupNode = wordLookupRoot;
						isBuildingWord = false;
						lastProcessedTokenIdx = lastGoodNodeTokenIdx;
						//	Move the head back to the spot after the last token in the word
						tokenIdx = lastGoodNodeTokenIdx + 1;
						continue;
					}

					//	If adding this raw word makes a word, then record it as good
					if (nodeForThisRawWord.Value != null)
					{
						lastGoodNode = nodeForThisRawWord;
						lastGoodNodeTokenIdx = tokenIdx;
					}
					currWordLookupNode = nodeForThisRawWord;
					continue;
				}
				else {
					switch (token.tokenType)
					{
					case LexerToken.TYPE.UNCLASSIFIED:
					{
						isBuildingWord = true;
						//	Try to continue the current word matching.
						Trie<RawWord, Word>.Node nodeForThisRawWord = currWordLookupNode.getChildNode(token.rawWord);
						//	If we can't continue this way...
						if (nodeForThisRawWord == null)
						{
							//...then whatever our last good match was is the correct word.
							if (lastGoodNode == null) {
								//throw new RuntimeLexerSourceException("Unrecognized word or symbol.", token);
								//	Dump everything into raw words.
								tokenIdx++;	//	Include this word
								for (int i=lastProcessedTokenIdx + 1; i < tokenIdx; i++) {
									writePNode(new PNode(new RawWordObject(tokens[i].rawWord)));
								}
								//	Update everything so we continue after this point
								lastGoodNodeTokenIdx = tokenIdx - 1;
								lastProcessedTokenIdx = lastGoodNodeTokenIdx;
								currWordLookupNode = wordLookupRoot;
								isBuildingWord = false;
								continue;
							}
							writePNode(new PNode(lastGoodNode.Value));
							//	Reset everything so we're looking for a new word again.
							lastGoodNode = null;
							currWordLookupNode = wordLookupRoot;
							isBuildingWord = false;
							lastProcessedTokenIdx = lastGoodNodeTokenIdx;
							//	Move the head back to the spot after the last token in the word
							tokenIdx = lastGoodNodeTokenIdx + 1;
							continue;
						}

						//	If adding this raw word makes a word, then record it as good
						if (nodeForThisRawWord.Value != null)
						{
							lastGoodNode = nodeForThisRawWord;
							lastGoodNodeTokenIdx = tokenIdx;
						}
						currWordLookupNode = nodeForThisRawWord;
						continue;
					}
					break;

					case LexerToken.TYPE.STRING:
					{
						//	First clean up any word we may be building and write it to output.
						if (isBuildingWord)
						{
							if (lastGoodNode == null || lastGoodNodeTokenIdx != tokenIdx - 1) {
								//throw new RuntimeLexerSourceException("Unrecognized word or symbol.", tokens[tokenIdx-1]);
								//	Just take all the words up until now and dump them into raw words.
								for (int i=lastProcessedTokenIdx + 1; i < tokenIdx; i++) {
									writePNode(new PNode(new RawWordObject(tokens[i].rawWord)));
								}
								//	Update everything so we continue after this point
								lastGoodNodeTokenIdx = tokenIdx - 1;
								lastProcessedTokenIdx = lastGoodNodeTokenIdx;
								currWordLookupNode = wordLookupRoot;
								isBuildingWord = false;
							}
							else {
								writePNode(new PNode(lastGoodNode.Value));
								//	Reset everything so we're looking for a new word again.
								lastGoodNode = null;
								currWordLookupNode = wordLookupRoot;
								isBuildingWord = false;
								lastProcessedTokenIdx = lastGoodNodeTokenIdx;
							}
						}

						//	Now write the string literal object to output
						writePNode(new PNode(new StringLiteralObject(token.rawWord.AsString)));
						lastProcessedTokenIdx = tokenIdx;
						//	Continue
						tokenIdx++;
						continue;
					}
					break;
					}
				}
			}


			finalCheckAfterParsing();
			return outputRoot;
		}


//		public PNode parseTokensIntoPNodes2(ProseRuntime runtime, ProseClient who, List<LexerToken> tokens)
//		{
//			//	Save the parameters
//			this.who = who;
//			this.runtime = runtime;
//			sourceTokenList = tokens;
//
//			setupForParsing();
//
//
//			//	This tracks a word if we're trying to build up a word longer than one rawword
//
//			Trie<RawWord, Word>.Node currWordLookupNode, wordLookupRoot, lastGoodNode;
//			wordLookupRoot = runtime.getWordLookupRoot();
//			currWordLookupNode = wordLookupRoot;
//			lastGoodNode = null;
//
//			for (LexerToken token = getNextToken();
//			     token != null;
//			     token = getNextToken())
//			{
//				if (token.rawWord == ProseLanguage.Raw.Quadquote) {
//					//	Output a quadquote
//					writePNode(new PNode(runtime.Quadquote));
//					//	Toggle our quad-quote-state.
//					insideQuadquoteExpression = !insideQuadquoteExpression;
//					continue;
//				}
//
//
//				if (insideQuadquoteExpression)
//				{
//					//	Test to see if the raw word is already a word.
//					//	If it is, include that word.
//					//	If it isn't, wrap it as a RawWordObject.
//				}
//				else {
//					switch (token.tokenType)
//					{
//					case LexerToken.TYPE.UNCLASSIFIED:
//						//	If we've made no progress looking up any word so far...
//						if (currWordLookupNode == wordLookupRoot) {
//							//	Is it ia word?
//							Trie<RawWord, Word>.Node nodeForThisRawWord = currWordLookupNode.getChildNode(token.rawWord);
//
//							//	If we didn't find anything at all, then this is a bogus word
//							if (nodeForThisRawWord == null)
//								throw new RuntimeLexerSourceException("Unrecognized word or symbol.", token);
//							//	Start searching here
//							currWordLookupNode = nodeForThisRawWord;
//							//	Is this actually a word?  Maybe we're done.
//							if (currWordLookupNode.Value != null)
//							{
//								//	If no children, then this HAS to be the word
//								if (!currWordLookupNode.HasChildren) {
//									writePNode(new PNode(currWordLookupNode.Value));
//									//	Reset our search
//									currWordLookupNode = wordLookupRoot;	//	Start looking at the bottom
//									lastGoodNode = null;					//	We haven't found anything yet.
//									continue;
//								}
//								else {
//									//	This is an acceptable word so cache the node
//									lastGoodNode = currWordLookupNode;
//									continue;
//								}
//							}
//							continue;
//						}
//
//						//	If we have made some progress looking up the word
//						else {
//							Trie<RawWord, Word>.Node nodeForThisRawWord = currWordLookupNode.getChildNode(token.rawWord);
//							//	If there is no appropriate child, this means that we MUST have found the end of our word already.
//							if (nodeForThisRawWord == null)
//							{
//								//	Make sure there's really a word here!
//								if (currWordLookupNode.Value == null) {
//									//	This is an error
//								}
//							}
//						}
//
//						//	Is it an action?
//						break;
//
//					case LexerToken.TYPE.STRING:
//						break;
//					}
//				}
//			}
//
//
//
//			finalCheckAfterParsing();
//			return outputRoot;
//		}

		private void setupForParsing()
		{
			sourceTokens = sourceTokenList.ToArray();

			//	Setup the output
			outputRoot = new PNode();
			output = outputRoot;

			insideQuadquoteExpression = false;
			tokenIdx = 0;
		}


		private void finalCheckAfterParsing()
		{
			if (insideQuadquoteExpression)
				throw new RuntimeLexerSourceException("Source ended before closing quad-quote.", sourceTokens[lastQuadquoteIdx]);
		}

		private void writePNode(PNode node)
		{
			output.makeNextEqualTo(node);	//	Add the node ot the right of output.
			//node.makeTerminal();
			output = output.next;
		}




	}
}

