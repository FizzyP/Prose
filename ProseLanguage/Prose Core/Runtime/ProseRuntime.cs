
/*
 *	Every foreign function gets passed a reference to the runtime object.
 *
 */

using System;
using System.Collections.Generic;
using System.Text;

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

		private int callDepth = 0;
		public int CallDepth {	get {	return callDepth;	}}

		#region Callbacks and Events

		public delegate void OnProgressReportDelegate(ProseRuntime runtime, PNode source, PNode progressMark);
		public event OnProgressReportDelegate OnProgressReport;

		public delegate void OnAmbiguityDelegate(ProseRuntime runtime, PNode source, List<PatternMatcher> matches);
		public event OnAmbiguityDelegate OnAmbiguity;

		public delegate void OnParseSentenceDelegate(ProseRuntime runtime, PNode source);
		public event OnParseSentenceDelegate OnParseSentence;

		public delegate void BeforePerformingActionDelegate(ProseRuntime runtime, PNode source);
		public event BeforePerformingActionDelegate BeforePerformingAction;

		public delegate void AfterPerformingActionDelegate(ProseRuntime runtime, PNode source);
		public event AfterPerformingActionDelegate AfterPerformingAction;

		public delegate void OnBreakPointDelegate(ProseRuntime runtime, PNode source, BreakPointObject.RuntimeData rtdata, string script);
		public event OnBreakPointDelegate OnBreakPoint;

		public delegate void BeforeReductionDelegate(ProseRuntime runtime, PNode source);
		public event BeforeReductionDelegate BeforeReduction;

		public delegate void AfterReductionDelegate(ProseRuntime runtime, PNode source);
		public event AfterReductionDelegate AfterReduction;

		public delegate void OnMatchDelegate(ProseRuntime runtime, PatternMatcher matcher);
		public event OnMatchDelegate OnMatch;

		public delegate void OnMatcherFailureDelegate(ProseRuntime runtime, PatternMatcher matcher);
		public event OnMatcherFailureDelegate OnMatcherFailure;

		#endregion

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
		private Word @BREAK;

		public Word @prose {	get {	return @PROSE;	} }
		public Word @text {		get {	return @TEXT;	} }
		public Word @string {	get {	return @STRING;	} }
		public Word @pattern {	get {	return @PATTERN;	} }
		public Word @raw {		get {	return @RAW;	} }
		public Word @action {	get {	return @ACTION;	} }
		public Word @break {	get {	return @BREAK;	} }

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

		public Word LeftParenthesis {	get {	return LEFT_PAREN;	}	}
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
			@BREAK = global_scope.addWordFromRawWord(ProseLanguage.Raw.@break);

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

		public void addPhraseAndDeleteExistingPhrases(Phrase phrase)
		{
			global_scope.addPhraseAndDeleteExistingPhrases(phrase);
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

			read(parsedSource);
		}


		public void read(List<ProseObject> source, int startIdx, int endIdx, ProseClient who)
		{
			//	Convert source into a linked list structure.
			PNode head = new PNode();
			PNode initPeriod = new PNode(Period);
			head.next = initPeriod;
			initPeriod.prev = head;
			PNode prev = initPeriod;
			for (int i=startIdx; i < endIdx; i++) {
				ProseObject obj = source[i];
				PNode p = new PNode(obj);
				p.prev = prev;
				prev.next = p;
				// Update
				prev = p;
			}

			PNode termPeriod = new PNode(Period);
			prev.next = termPeriod;
			termPeriod.prev = prev;

			//	Read the new format starting with the initial period.
			read(head);
		}

		public void read(PNode source)
		{
			//	Read the source one sentence at a time.
			PNode sourcePtr = source.next.next;	//	Skip the little header PNode and the inserted .
			while (sourcePtr != null)
			{
				sourcePtr = readSentence(sourcePtr);
			}
		}


		//	Synchronize calls to "doReadSentence(...)"
		private PNode readSentence(PNode sourcePtr) {
			lock(sentenceReaderLock) {
				PNode val;
				callDepth++;
				try {
					val = doReadSentence(sourcePtr);
				} finally {
					callDepth--;
				}
				return val;
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


			if (OnParseSentence != null)
				OnParseSentence(this, sourcePtr.prev);



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

					//	Callback Before
					if (BeforePerformingAction != null)
						BeforePerformingAction(this, reduced);

					//	DO THE ACTION!
//					try {
						((ProseAction) reduced.value).performAction(this);
//					}
//					catch (Exception e)
//					{
//						this.read ("language function exception: \"" + e.Message + "\"", GlobalClient);
//					}

					//	Callback After
					if (AfterPerformingAction != null)
						AfterPerformingAction(this, reduced);

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

				//	Make a progress report callback.
				PNode beginningOfFragment = source.prev.next;	//	If source has been changed, this is the new head
				if (OnProgressReport != null)
					OnProgressReport(this, beginningOfFragment, progressMark);

				//	Try to reduce starting at the progressMark
				bool didReduceAtMark;
				PNode reducedAtMark;
				reducedAtMark = reduceSentenceFragmentStartingAtBeginning(progressMark, out didReduceAtMark);

				didReduce = didReduce || didReduceAtMark;		//	Record if we managed to reduce anything.
				//	Possibly this changed the beginning of the fragment we're looking at.
				//	If so, record the change.
				if (progressMark == reducedFragment)	//	Check for the beginning, if we're at the beginning then...
					reducedFragment = reducedAtMark;	//	...need to keep track of the moving beginning.
				if (didReduceAtMark)
				{
					//	If we reduced something, then start over to see if we can match from the beginning.
					progressMark = reducedFragment;

//					//	First check to see if we're moving it past an action.
//					if (progressMark.value is ProseAction) {
//						//	If so, return to allow the action to be performed.
//						return progressMark;
//					}
				}
				else {
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

			List<PatternMatcher> matches = getSuccessfulMatchesStartingAtPNode(source);		//	Get the matches
			List<PatternMatcher> bestMatches = getStrongestMatchesFromMatchList(matches);	//	Select the winners

			//	Deal with ambiguity (too many matches)
			if (bestMatches.Count > 1  ||
			    bestMatches.Count == 1 && bestMatches[0].MatchedPhrases.Count > 1)
			{
				didReduce = false;

				//	Make an ambiguity callback and throw an exception to bail out of this mess.
				if (OnAmbiguity != null) {
					OnAmbiguity(this, source, bestMatches);
					throw new RuntimeProseLanguageException("Sentence is ambiguous.", source);
				}
				else
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

			//	Do any automatic casting necessary to make the arguments match the pattern.
			bestMatcher.autoCastArguments();

			if (BeforeReduction != null)
				BeforeReduction(this, source);

			//	REDUCE!
			PNode reducedSource = bestPhrase.evaluate(source, bestMatcher);

			if (AfterReduction != null)
				AfterReduction(this, reducedSource);

			return reducedSource;
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
				//	If we run into left parenthesis, reduce and eliminate before continuing.
				if (sourceNode.value == LeftParenthesis)
				{
					PNode leftParen = sourceNode;
					//	Reduce the parenthetical
					bool didReduce;
					sourceNode = reduceParentheticalExpression(sourceNode, out didReduce, RightParenthesis);
					//	Some matchers may have retained a reference to the leftParen, so swap it for whatever is there now.
					foreach (PatternMatcher matcher in activeMatchers) {
						if (matcher.terminatorNode == leftParen)
							matcher.terminatorNode = sourceNode;
					}
				}
				sourceNode = debugFilterIncomingPNode(sourceNode);
				if (sourceNode == null)
					break;


				//	Try to extend all of the active matchers.
				List<PatternMatcher> babyMatchers = new List<PatternMatcher>(256);
				foreach(PatternMatcher matcher in activeMatchers)
				{
					//	Try to extend the given matcher and get all the resulting babies.
					List<PatternMatcher> thisMatchersBabies = matcher.matchNextPNode(sourceNode);

					int failCount = 0;	//	Count the baby matchers that arrive dead.
					foreach (PatternMatcher babyMatcher in thisMatchersBabies) {
						//	if the matcher has found a match, remember it.
						if (babyMatcher.IsMatched)
						{
							//	Callback for successful match
							if (OnMatch != null)
								OnMatch(this, babyMatcher);

							successfullMatches.Add(babyMatcher);
							babyMatchers.Add(babyMatcher);
							//							//	Rest the matcher so it can be can be continued
//							babyMatcher.IsMatched = false;
						}
						else if (babyMatcher.IsntFailed)
						{
						//	Regardless, throw it back in the pot to see if it can be extended
						//if (babyMatcher.IsntFailed)
							babyMatchers.Add(babyMatcher);
						}
						else {
							//	This baby matcher was stillborn
							failCount++;
						}
					}

					//	If the matcher produced no good children then that branch is "dead"
					//	Only count it as a "fail" if it isn't a success itself.
					if (	thisMatchersBabies.Count - failCount == 0
					    &&	!matcher.IsMatched)
					{
						if (OnMatcherFailure != null)
							OnMatcherFailure(this, matcher);
					}



				}
				
				//	After one generation, the baby matchers become the new active matchers.
				activeMatchers = babyMatchers;
				//	And now we're looking at the next object in the sequence
				sourceNode = sourceNode.next;
			}

			return successfullMatches;
		}

		private PNode debugFilterIncomingPNode(PNode node)
		{
			node = filterIncomingPNode(node);

			//	Check for breakpoints
			if (node.value is BreakPointObject)
			{
				BreakPointObject bp = (BreakPointObject) node.value;
				BreakPointObject.RuntimeData rtdata = new BreakPointObject.RuntimeData();
				
				//	Do any action associated with the breakpoint and make the default callback if asked
				if (bp.doBreakPoint(this, node, rtdata)) {
					if (OnBreakPoint != null)
						OnBreakPoint(this, node, rtdata, bp.BreakScript);
				}
				
				//	Remove the breakpoint
				if (node.prev != null)
					node.prev.next = node.next;
				if (node.next != null)
					node.next.prev = node.prev;
				//	Skip over the breakpoint
				return node.next;
			}

			return node;
		}

		//	Possibly translate some incoming nodes
		public PNode filterIncomingPNode(PNode node)
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


		
		private PNode reduceParentheticalExpression(PNode source, out bool didReduce, ProseObject rightParentheticalObject)
		{
			//	Find the ending parenthesis
			PNode rightParen = source;
			while (rightParen.value != rightParentheticalObject) {
				rightParen= rightParen.next;
			}
			
			//	Unhook the ending parenthesis from the expression we want to reduce.
			rightParen.prev.next = null;
			
			//	Reduce the expression (starting AFTER the left paren)
			bool didReduceParenthetical;
			PNode reducedExpression;
			
			callDepth++;
			try {
				reducedExpression= reduceSentenceFragment(source.next, out didReduceParenthetical);
			}
			finally {
				callDepth--;
			}
			
			//We splice it in in two ways depending on whether it reduces to anything or not
			if (reducedExpression == null)
			{
				//	If we get nothing back, cut out the parenthesis and continue
				source.prev.next = rightParen.next;
				if (rightParen.next != null)
					rightParen.next.prev = source.prev;
				reducedExpression = rightParen.next;		//	The first thing after the paren is what we look at next.
			}
			else {
				//	If we get something back, splice it in
				//	Cut out the left paren
				source.prev.next = reducedExpression;
				reducedExpression.prev = source.prev;
				
				//	Hook the end of the expression back into the original source (skipping right paren)
				//	First find the new end
				PNode reducedExprEnd = reducedExpression;
				while (reducedExprEnd.next != null)	reducedExprEnd = reducedExprEnd.next;
				reducedExprEnd.next = rightParen.next;
				rightParen.next.prev = reducedExprEnd;
			}
			
			didReduce = didReduceParenthetical;
			
			return reducedExpression;
		}


		//	Read a text expression and reduce it to a string
		//	textStart = opening "", textEnd = object after closing ""
		public string parseTextExpressionIntoString(PNode textStart, PNode textEnd)
		{
			StringBuilder str = new StringBuilder();

			int prevObjSpaceRequest = 0;
			int thisObjSpaceRequest = 0;

			for (PNode node = textStart.next;
			     node != null  &&  node.next != textEnd;
			     node = node.next)
			{
				ProseObject obj = node.value;

				//	WARNING:  MUST COME FIRST BECAUSE IT MAY CHANGE THE VALUE OF node
				//	Left square bracket opens an inline prose expression that must be evaluated
				if (obj == LeftSquareBracket)
				{
					bool didReduce;
					node = reduceParentheticalExpression(node, out didReduce, RightSquareBracket);
					obj = node.value;
				}

				//	String literals substitute their contents directly
				if (obj is StringLiteralObject) {
					str.Append(((StringLiteralObject) obj).literal);
					thisObjSpaceRequest = -1;
				}
				else {
					str.Append(obj.getReadableString());
					thisObjSpaceRequest = 1;
				}

				if (prevObjSpaceRequest != -1 && thisObjSpaceRequest != -1)
					str.Append(" ");

				//	Update
				prevObjSpaceRequest = thisObjSpaceRequest;
			}

			if (str[str.Length-1] == ' ')
				str.Remove(str.Length - 1, 1);

			return str.ToString();
		}


		//	Takes only the best match according to the rules!
		private List<PatternMatcher> getStrongestMatchesFromMatchList(List<PatternMatcher> matches)
		{
			List<PatternMatcher> bestMatches = new List<PatternMatcher>();

			//	First filter based on the class word.  This is the "highest order bit"
			matches = filterMatchesBasedOnClassWord(matches);
			if (matches.Count < 2)
				return matches;

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

			//	If we still have > 1 use inheritence to whittle down the list.
			if (bestMatches.Count > 1)
				return getStrongestMatchesUsingInheritence(bestMatches);
			else
				return bestMatches;
		}

		//	Compare matches against each other and throw out matchers which are "dominated"
		//	according to the inheritence system.
		private List<PatternMatcher> getStrongestMatchesUsingInheritence(List<PatternMatcher> matches)
		{
			List<PatternMatcher> betterMatches = new List<PatternMatcher>();
			int idx = 0;
			do {
				PatternMatcher m = matches[idx];
				betterMatches = filterMatchesUsingInheritenceAgainstOneMatcher(matches, m);

				//	If we didn't manage to shorten the list then we got back the exact same list.
				//	So, we have to look at the next spot in that list.
				if (betterMatches.Count == matches.Count)	idx++;

				matches = betterMatches;
			}
			while (matches.Count > 1  &&  matches.Count > idx);

			return matches;
		}

		private List<PatternMatcher> filterMatchesBasedOnClassWord(List<PatternMatcher> matches)
		{
			//List<PatternMatcher> betterMatches = new List<PatternMatcher>();

			return matches;
		}


		//	Take a pass through the list and throw out everything that loses against m.
		private List<PatternMatcher> filterMatchesUsingInheritenceAgainstOneMatcher(List<PatternMatcher> matches,  PatternMatcher m)
		{
			List<PatternMatcher> winners = new List<PatternMatcher>();

			foreach (PatternMatcher match in matches) {
				int res = compareMatchers(m, match);
				//	As long as it isn't a clear victory for m we keep match
				if (res != 1) {
					winners.Add(match);
				}
			}

			return winners;
		}

		//	Returns 1 if a > b, -1 if a < b, 0 otherwise
		int compareMatchers(PatternMatcher a, PatternMatcher b)
		{
			//	These matches represent exactly the same thing.
			if (a.PhraseTrieNode == b.PhraseTrieNode) {
				return 0;
			}

			if (firstMatcherDominatesSecond(a,b)) {
				if (firstMatcherDominatesSecond(b, a))
					return 0;
				else
					return 1;
			}
			else {
				if (firstMatcherDominatesSecond(b,a))
					return -1;
				else
					return 0;
			}
		}

		bool firstMatcherDominatesSecond(PatternMatcher a, PatternMatcher b)
		{
			//	Compare their class word first

			//	Compare the associated patterns.

			//	Extract the patterns from a, b.
			ProseObject[] pa = a.AssociatedPattern;
			ProseObject[] pb = b.AssociatedPattern;

			for (int i=0; i < pa.Length; i++) {

				//	Most often they'll be the same and then
				if (pa[i] == pb[i])
					continue;

				//	@prose is weaker than everything else
				if (pa[i] == @prose && pb[i] != @prose)
					return false;
				else if (pa[i] != @prose && pb[i] == @prose)
					continue;

				else if (pa[i] == @raw && pb[i] is Word)
					return false;
				else if (pa[i] is Word && pb[i] == @raw)
					continue;

				Word aword = (Word) pa[i];
				List<ProseObject> descendents = aword.getAllDescendents(this);
				//	If there even one piece of b's pattern doesn't descend from a's
				//	then a doesn't dominate.
				if (!descendents.Contains(pb[i]))
					return false;
			}

			return true;
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

























