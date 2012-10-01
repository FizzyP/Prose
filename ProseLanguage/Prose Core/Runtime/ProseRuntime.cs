
/*
 *	Every foreign function gets passed a reference to the runtime object.
 *
 */

using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	public class ProseRuntime
	{

		private ProseLexer staticLexer = new ProseLexer();
		private RuntimeLexer runtimeLexer = new RuntimeLexer();

		private Object sentenceReaderLock = new Object();			//	Synchronize reading sentences.

		//	These are just place holders until we put something meaningful here
		private ProseScope global_scope = new ProseScope();
		public ProseScope GlobalScope {		get {	return global_scope;	}	}
		private ProseClient global_client = new ProseClient();
		public ProseClient GlobalClient {	get {	return global_client;	}	}



		#region Constant Words/Symbols/Actions

		#region Builtin Words

		private Word word_word;
		private Word word_phrase;

		public Word Word_word {		get {	return word_word;	}	}
		public Word Word_phrase {	get {	return word_phrase;	}	}

		#endregion

		#region Pattern matching @words
		private Word @PROSE;
		private Word @TEXT;
		private Word @STRING;
		private Word @PATTERN;
		private Word @RAW;
		private Word @ACTION;

		public Word @prose {	get {	return @PROSE;	} }
		public Word @text {		get {	return @TEXT;	} }
		public Word @string {	get {	return @STRING;	} }
		public Word @pattern {	get {	return @PATTERN;	} }
		public Word @raw {		get {	return @RAW;	} }
		public Word @action {	get {	return @ACTION;	} }

		#endregion

		#region Symbols
		//	Private Symbols
		private Word COLON; 
		private Word PERIOD; 
		private Word SEMICOLON; 
		private Word COMMA;
		private Word QUADQUOTE; 

		private Word LEFT_PAREN; 
		private Word RIGHT_PAREN; 
		private Word LEFT_SQ_BRACKET; 
		private Word RIGHT_SQ_BRACKET; 
		private Word LEFT_CURLY_BRACKET; 
		private Word RIGHT_CURLY_BRACKET;

		private Word PLUS_COLON;
		private Word MINUS_COLON;
		private Word LEFT_ARROW;

		private Word COLON_PLUS;
		private Word COLON_MINUS;
		private Word RIGHT_ARROW;

		//	Symbol Accesors
		public Word Colon {	get {	return COLON;	}	}
		public Word Period {	get {	return PERIOD;	}	}
		public Word Semicolon {	get {	return SEMICOLON;	}	}
		public Word Comma {	get {	return COMMA;	}	}
		public Word Quadquote {	get {	return QUADQUOTE;	}	}

		public Word LefetParenthesis {	get {	return LEFT_PAREN;	}	}
		public Word RightParenthesis {	get {	return RIGHT_PAREN;	}	}
		public Word LeftSquareBracket {	get {	return LEFT_SQ_BRACKET;	}	}
		public Word RightSquareBracket {	get {	return RIGHT_SQ_BRACKET;	}	}
		public Word LeftCurlyBracket {	get {	return LEFT_CURLY_BRACKET;	}	}
		public Word RightCurlyBracket {	get {	return RIGHT_CURLY_BRACKET;	}	}

		public Word PlusColon {	get {	return PLUS_COLON;	}	}
		public Word MinusColon {	get {	return MINUS_COLON;	}	}
		public Word LeftArrow {	get {	return LEFT_ARROW;	}	}

		public Word ColonPlus {	get {	return COLON_PLUS;	}	}
		public Word ColonMinus {	get {	return COLON_MINUS;	}	}
		public Word RightArrow {	get {	return RIGHT_ARROW;	}	}
		#endregion


		#endregion


		#region Constructors

		public ProseRuntime ()
		{
			global_scope.addProseLanguageWords();

			//	Built-in Words
			word_word = global_scope.addWordFromRawWord(ProseLanguage.Raw.word);
			word_phrase = global_scope.addWordFromRawWord(ProseLanguage.Raw.phrase);

			//@ words
			@PROSE = global_scope.addWordFromRawWord(ProseLanguage.Raw.@prose);
			@TEXT = global_scope.addWordFromRawWord(ProseLanguage.Raw.@text);
			@STRING = global_scope.addWordFromRawWord(ProseLanguage.Raw.@string);
			@RAW = global_scope.addWordFromRawWord(ProseLanguage.Raw.@raw);
			@PATTERN = global_scope.addWordFromRawWord(ProseLanguage.Raw.@pattern);
			@ACTION = global_scope.addWordFromRawWord(ProseLanguage.Raw.@action);

			//	Assign the private symbols at construction time
			COLON = global_scope.addWordFromRawWord(ProseLanguage.Raw.Colon);
			PERIOD = global_scope.addWordFromRawWord(ProseLanguage.Raw.Period);
			SEMICOLON = global_scope.addWordFromRawWord(ProseLanguage.Raw.Semicolon);
			COMMA = global_scope.addWordFromRawWord(ProseLanguage.Raw.Comma);
			QUADQUOTE = global_scope.addWordFromRawWord(ProseLanguage.Raw.Quadquote);

			LEFT_PAREN = global_scope.addWordFromRawWord(ProseLanguage.Raw.LeftParenthesis);
			RIGHT_PAREN = global_scope.addWordFromRawWord(ProseLanguage.Raw.RightParenthesis);
			LEFT_SQ_BRACKET = global_scope.addWordFromRawWord(ProseLanguage.Raw.LeftSquareBracket);
			RIGHT_SQ_BRACKET = global_scope.addWordFromRawWord(ProseLanguage.Raw.RightSquareBracket);
			LEFT_CURLY_BRACKET = global_scope.addWordFromRawWord(ProseLanguage.Raw.LeftCurlyBracket);
			RIGHT_CURLY_BRACKET = global_scope.addWordFromRawWord(ProseLanguage.Raw.RightCurlyBracket);

			PLUS_COLON = global_scope.addWordFromRawWord(ProseLanguage.Raw.PlusColon);
			MINUS_COLON = global_scope.addWordFromRawWord(ProseLanguage.Raw.MinusColon);
			LEFT_ARROW = global_scope.addWordFromRawWord(ProseLanguage.Raw.LeftArrow);

			COLON_PLUS = global_scope.addWordFromRawWord(ProseLanguage.Raw.ColonPlus);
			COLON_MINUS = global_scope.addWordFromRawWord(ProseLanguage.Raw.ColonMinus);
			RIGHT_ARROW = global_scope.addWordFromRawWord(ProseLanguage.Raw.RightArrow);


			//	Build the built-in inheritence
			ProseLanguage.constructInitialInheritance(this);

			//	Build the built-in phrases
			ProseLanguage.constructInitialPatternTrie(this);
		}

		#endregion

		#region Words

		public Word addWordFromStrings(string[] subWords)
		{
			return global_scope.addWordFromStrings(subWords);
		}

		public Word addWordFromRawWords(RawWord[] rawWords)
		{
			return global_scope.addWordFromRawWords(rawWords);
		}

		public Word addWordFromRawWord(RawWord rawWord)
		{
			return addWordFromRawWords(new RawWord[] { rawWord });
		}

		public void addWord(Word word)
		{
			global_scope.addWord(word);
		}

		public Word searchWordFromRawWords(RawWord[] rawWords) {
			return global_scope.searchWordFromRawWords(rawWords);
		}

		public Word searchWordFromRawWord(RawWord rawWord) {
			return searchWordFromRawWords(new RawWord[] {rawWord});
		}

		public Word word(string s) {
			return global_scope.word(s);
		}

		internal Trie<RawWord, Word>.Node getWordLookupRoot()
		{
			return global_scope.getWordLookupRoot();
		}

		#endregion

		#region Phrases

		public void addPhrase(Phrase phrase)
		{
			global_scope.addPhrase(phrase);
		}

		#endregion

		#region "read" methods and synchronization


		public void read(string source, ProseClient who)
		{
			//	Insert a leading period because the parser expects the previous terminal symbol.
			source = "." + source + ".";

			//	Apply lexical analysis that requires no knowledge of the runtime words/phrases
			List<LexerToken> statLexSrc = staticLexer.parseStringIntoTokens(source);
			//	Parse LexerTokens into runtime code.
			PNode parsedSource = parseTokensIntoPNodes(global_client, statLexSrc);

			//	Debug
			//Console.WriteLine("After runtime lexer: " + parsedSource.getReadableString() );


			//	Read the source one sentence at a time.
			PNode sourcePtr = parsedSource.next.next;	//	Skip the little header PNode and the inserted .
			while (sourcePtr != null)
			{
				sourcePtr = readSentence(sourcePtr);
			}
		}


		//	Synchronize calls to "doReadSentence(...)"
		private PNode readSentence(PNode sourcePtr) {
			lock(sentenceReaderLock) {
				return doReadSentence(sourcePtr);
			}
		}

		//	Synchronize access to the runtime lexer
		private PNode parseTokensIntoPNodes(ProseClient client, List<LexerToken> statLexSrc)
		{
			lock(sentenceReaderLock) {
				return runtimeLexer.parseTokensIntoPNodes(this, global_client, statLexSrc);
			}
		}



		#endregion


		#region Reduce Prose / Perform Actions


		//	Do the work of reading the sentence (not threadsafe)
		private PNode doReadSentence(PNode sourcePtr)
		{
//			//	If the sentence is just a period, skip it.
//			if (sourcePtr.value == Period)
//				return sourcePtr.next;

			//	Reduce the sentence.
			bool didReduce;
			PNode reducedSource = reduceSentence(sourcePtr, out didReduce);

			//	If there are actions on the left, do them. and then try to reduce again.
			while (true)
			{
				bool didSomeActions, sentenceHasBeenExecuted;
				PNode afterActions = doActionsOnLeftOfSentence(reducedSource, out didSomeActions, out sentenceHasBeenExecuted);

				if (sentenceHasBeenExecuted) {
					return afterActions;
				}

				if (didSomeActions) {
					//	If we're at a terminal symbol, then finish, otherwise reduce more.
					if (afterActions.value == Period) {
						reducedSource = afterActions;
						break;
					}
				}
				else if (afterActions == reducedSource) {
					//	If we didn't even do any actions and didn't move over any punctuation then...
					//	If we reduced the sentence and didn't end up with any actions on the left, then
					//	reducing again won't do anything and we have nothing to do.  We have to crash.
					throw new RuntimeProseLanguageException("Sentence reduced to non-actionable code.", reducedSource);
				}

				//	Try again to reduce the source
				reducedSource = reduceSentence(afterActions, out didReduce);
			}

			//	Move the source pointer past the terminal symbol.
			return reducedSource.next;
		}

		private bool symbolEndsSentence(PNode p)
		{
			return proseObjectEndsSentence(p.value);
		}

		private bool proseObjectEndsSentence(ProseObject p)
		{
			return (p == this.PERIOD);
		}


		//	Start on the left side of the sentence and perform actions.  Eliminate ; along the way if
		//	it's sandwhiched between actions.
		//	NOTE:  EXPECTS INITIAL PUNCTUATION
		private PNode doActionsOnLeftOfSentence(PNode sourcePtr, out bool didSomeActions, out bool sentenceHasBeenExecuted)
		{
			didSomeActions = false;
			sentenceHasBeenExecuted = false;
			PNode reduced = sourcePtr.next;	//	Skip initial punctuation
			while (reduced != null)
			{
				if (reduced.value is ProseAction) {
					//	DO THE ACTION!
					((ProseAction) reduced.value).performAction(this);
					didSomeActions = true;
					reduced.removeSelf();

					reduced = reduced.next;
				}
				else if (reduced.value == Semicolon || reduced.value == Comma) {
					//	Check to see if we should leave this semicolon in (as punctuation for the remaining
					//	prose to match) or remove it because it's surrounded in actions.
					if (reduced.next != null)
					{
						//	Skip over the semicolon and move on to the next action we found
						reduced = reduced.next;
					}
					else {
						//	We've run into a the end, so leave the symbol there.
						break;
					}
				}
				else if(reduced.value == Period) {
					//	We've run into the end of the whole sentence, so skip the period.
					sentenceHasBeenExecuted = true;
					return reduced.next;
				}
				else {
					break;
				}
			}

			if (didSomeActions)
				return reduced;
			else
				return sourcePtr;
		}


		//	Reads a sentence from the source and returns the new
		//	begining of the source. 
		//	didReduce:		Whether or not it actually succeeded in reducing anything.
		private PNode reduceSentence(PNode source, out bool didReduce)
		{
			//	We back up the read head to read from the previous terminal symbol.
			try {
				return reduceSentenceFragment(source.prev, out didReduce);
			}
			catch (RuntimeProseLanguageFragmentException e) {
				//	Forward this message and fill in the beginning of the sentence.
				throw new RuntimeProseLanguageException(e.Message, source);
			}
		}

		//	throws RuntimeProseLanguageFragmentException
		private PNode reduceSentenceFragment(PNode source, out bool didReduce)
		{
			//	Preserve this so we can figure out where the reduced source begins.
			PNode prevNode = source.prev;

			//	When we can't undestand the beginning of a sentence, we move on.  progressMark
			//	stores the location of this new "virtual beginning" we try to analyze.
			PNode reducedFragment = source;
			PNode progressMark = source;
			didReduce = false;				//	start assuming we didn't do anything.
			while(true)
			{
				if (progressMark == null)
					throw new RuntimeProseLanguageFragmentException("Source ended before sentence.", source);

				//	Try to reduce starting at the progressMark
				PNode beginningOfFragment = source.prev.next;
				Console.WriteLine("- " + beginningOfFragment.getReadableStringWithProgressMark(progressMark));
				bool didReduceAtMark;
				PNode reducedAtMark = reduceSentenceFragmentStartingAtBeginning(progressMark, out didReduceAtMark);
				didReduce = didReduce || didReduceAtMark;		//	Record if we managed to reduce anything.
				//	Possibly this changed the beginning of the fragment we're looking at.
				//	If so, record the change.
				if (progressMark == reducedFragment)
					reducedFragment = reducedAtMark;
				if (didReduceAtMark)
				{
					//	If we reduced something, then start over to see if we can match from the beginning.
					progressMark = reducedFragment;
				} else
				{
					//	We didn't reduce anything, so move the progress mark forward.
					progressMark = progressMark.next;
					//	If we run into a sentence ending symbol like this, then we're finished.
					if (progressMark == null || symbolEndsSentence(progressMark))
					{
						//INCORRECT:
						//  Return the ending punctation (beginning punctuation depending on how you think about it)
						//return progressMark;

						//	Returning this should give us the beginning of the source fragment whether we
						//  replaced anything or not.
						return prevNode.next;

					}
				}

			}


		}


		//	Match must start at the beginning
		private PNode reduceSentenceFragmentStartingAtBeginning(PNode source, out bool didReduce)
		{
			//	Create a matcher
			//	While there are still "matchers in progress"
			//		Feed each PNode in the source to each matcher in progress.
			//		If the matcher indicats it is done then
			//			Ask for it's list of best matches.
			//			Add that list to the complete list of best matches.
			//			Remove the completed matcher from the list of matchers in progress.
			//		Otherwise,
			//			Add the list of child matchers it returns to the list of matchers in progress

			List<PatternMatcher> matches = getSuccessfulMatchesStartingAtPNode(source);
			List<PatternMatcher> bestMatches = getStrongestMatchesFromMatchList(matches);

			//	Deal with ambiguity (too many matches)
			if (bestMatches.Count > 1  ||
			    bestMatches.Count == 1 && bestMatches[0].MatchedPhrases.Count > 1)
			{
				didReduce = false;
				throw new RuntimeProseLanguageException("Sentence is ambiguous.", source);
			}
			else if (bestMatches.Count == 0) {
				//	We couldn't reduce anything
				didReduce = false;
				return source;
			}


			//	Apply the phrase
			PatternMatcher bestMatcher = bestMatches[0];
			Phrase bestPhrase = bestMatcher.MatchedPhrases[0];
			didReduce = true;

			return bestPhrase.evaluate(source, bestMatcher);
		}

		private List<PatternMatcher> getSuccessfulMatchesStartingAtPNode(PNode source)
		{
			PatternMatcher seedMatcher = new PatternMatcher(this);
			List<PatternMatcher> activeMatchers = new List<PatternMatcher>(64);
			activeMatchers.Add(seedMatcher);
			List<PatternMatcher> successfullMatches = new List<PatternMatcher>();
			
			//	Start out looking at the first node in the source.
			PNode sourceNode = source;
			
			//	As long as there are still active matchers...
			while(activeMatchers.Count != 0 && sourceNode != null)
			{
				//	Try to extend all of the active matchers.
				List<PatternMatcher> babyMatchers = new List<PatternMatcher>(256);
				foreach(PatternMatcher matcher in activeMatchers)
				{
					sourceNode = filterIncomingPNode(sourceNode);
					//	Try to extend the given matcher and get all the resulting babies.
					List<PatternMatcher> thisMatchersBabies = matcher.matchNextPNode(sourceNode);

					//	Keep the matcher
					foreach (PatternMatcher babyMatcher in thisMatchersBabies) {
						//	if the matcher has found a match, remember it.
						if (babyMatcher.IsMatched)
						{
							successfullMatches.Add(babyMatcher);
//							//	Rest the matcher so it can be can be continued
//							babyMatcher.IsMatched = false;
						}
						//	Regardless, throw it back in the pot to see if it can be extended
						//if (babyMatcher.IsntFailed)
							babyMatchers.Add(babyMatcher);
					}


				}
				
				//	After one generation, the baby matchers become the new active matchers.
				activeMatchers = babyMatchers;
				//	And now we're looking at the next object in the sequence
				sourceNode = sourceNode.next;
			}

			return successfullMatches;
		}

		//	Possibly translate some incoming nodes
		private PNode filterIncomingPNode(PNode node)
		{
			//	Raw nodes get free upgrades to words if the word is defined.
			if (node.value is RawWordObject)
			{
				RawWordObject raw = (RawWordObject) node.value;
				Word word = global_scope.searchWordFromRawWords(raw.RawWords);
				//	If it's a real word, then upgrade it
				if (word != null) {
					PNode newNode = new PNode(word);
					//	Hook up the new word node in place if this raw word node
					node.prev.next = newNode;
					newNode.prev = node.prev;
					node.next.prev = newNode;
					newNode.next = node.next;
					return newNode;
				}
			}

			return node;
		}

		//	Takes only the best match according to the rules!
		private List<PatternMatcher> getStrongestMatchesFromMatchList(List<PatternMatcher> matches)
		{
			List<PatternMatcher> bestMatches = new List<PatternMatcher>();

			//	Figure out the longest length
			int longestMatchLength = 0;
			foreach (PatternMatcher matcher in matches) {
				int l = matcher.NumObjectsMatched;
				if (l > longestMatchLength)
					longestMatchLength = l;
			}

			//	Only take the longest matches
			foreach (PatternMatcher matcher in matches) {
				if (longestMatchLength == matcher.NumObjectsMatched)
					bestMatches.Add(matcher);
			}

			return bestMatches;
		}

		#endregion


		#region Tags

		private UInt64 nextTag = 1;		//	All tags are initialized to be 0
		public UInt64 getNewTag() {
			return nextTag++;
		}

		#endregion




	}
}

























