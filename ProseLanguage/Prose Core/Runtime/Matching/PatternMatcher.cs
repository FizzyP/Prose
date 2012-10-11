using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	public class PatternMatcher
	{
		//	The runtime this pattern matcher is operating inside.
		private ProseRuntime runtime;
		//	Pattern trie for the associated scope
		private Trie<ProseObject, List<Phrase>> patternTrie;
		//	The list of source nodes we've matched so far + an extra one pointing
		//	to the node AFTER the piece of prose matched.
		private List<PNode> patternComponentNodes = new List<PNode>();
		//	A node in the patternTrie that represents the piece of pattern we're working on.
		private Trie<ProseObject, List<Phrase>>.Node currNode;
		//	Keep track of how many objects we've matched
		private int numObjectsMatched = 0;

		private MatcherState state = MatcherState.MATCHING_OBJECT;

		//	Reading Text

		//	Reading Prose
		private ProseBlockObject proseBlock = null;				//	The contents of the prose block being built
		private Stack<ProseObject> parentheticalStack = null;	//	The parentheticals in the block.
		private bool inText = false;							//	Keep track of when we're in a text block inside a prose block.
		private bool isMatched = false;							//	Do we represent a complete match?

		//	Reading Patterns
		private int bracketReaderCount = 0;
		private int prevBracketReaderCount = 0;


		#region Properties

		public bool IsMatched { get { 	return isMatched;	 }
								set {	isMatched = value; 	} }

		public bool IsntFailed { get {	return state != MatcherState.FAILED; } }

		public List<PNode> Matching { get { return patternComponentNodes; } }

		internal Trie<ProseObject, List<Phrase>>.Node PhraseTrieNode {
			get {	return currNode;	}
		}

		public List<Phrase> MatchedPhrases {
			get {
				return currNode.Value;
			}
		}

		public ProseObject[] AssociatedPattern {
			get {
				if (	currNode.Value != null
				    && 	currNode.Value.Count != 0)
				{
					return currNode.Value[0].getPattern();
				}
				else {
					//	If there's no match to use, read it from currNode
					ProseObject[] path = currNode.ReverseCharacterPath.ToArray();
					Array.Reverse(path);
					return path;
				}
			}
		}

		public int NumObjectsMatched {	get {	return numObjectsMatched;	} }

		public ProseRuntime Runtime {	get {	return runtime;	}	}

		#endregion

		public enum MatcherState {
			MATCHING_OBJECT,
			MATCHING_PROSE,
			MATCHING_TEXT,
			MATCHING_PATTERN,

			FAILED,
		}


		//	Start a pattern match at the beginning with one object.
		internal PatternMatcher (ProseRuntime runtime)
		{
			this.runtime = runtime;
			patternTrie = runtime.GlobalScope.PatternTree;
			state = MatcherState.MATCHING_OBJECT;
			currNode = patternTrie.Root;
		}

		public MatcherState State {	get {	return state;	}	}

		//	Clone the pattern matcher and reconfigure it in a new state based on a word.
		private PatternMatcher makeCopyWithStateFromAtWord(ProseObject atWord)
		{
			PatternMatcher newMatcher = new PatternMatcher(runtime);
			newMatcher.patternComponentNodes.AddRange(patternComponentNodes);
			newMatcher.numObjectsMatched = numObjectsMatched;

			if (currPatternObject != null)
				newMatcher.currPatternObject = currPatternObject.clone();


			if (atWord == runtime.@prose) {
				newMatcher.switchToState_MATCHING_PROSE();
				if (parentheticalStack != null) {
					//newMatcher.parentheticalStack = new Stack<ProseObject>(parentheticalStack);
					newMatcher.parentheticalStack = new Stack<ProseObject>();
					ProseObject[] objs = parentheticalStack.ToArray();
					for (int i=objs.Length - 1; i >= 0; i--)
						newMatcher.parentheticalStack.Push(objs[i]);
					
				}
				newMatcher.inText = inText;
			}
			else if (atWord == runtime.@text) {
				newMatcher.switchToState_MATCHING_TEXT();
				newMatcher.textMatchingQQcount = textMatchingQQcount;
			}
			else if (atWord == runtime.@pattern) {
				newMatcher.switchToState_MATCHING_PATTERN();
			}
			else {
				newMatcher.switchToState_MATCHING_OBJECT();
			}

			return newMatcher;
		}
	

		//	Become a successful match and record
		public PNode terminatorNode = null;
		private void becomeMatched(PNode nextNode)
		{
			isMatched = true;
			terminatorNode = nextNode;
		}



		//	Take the current pattern matcher, attempt to extend the match by one object.
		//	Return the list of child matchers spawned.
		//public List<PatternMatcher> matchNextObject(ProseObject nextObject)
		public List<PatternMatcher> matchNextPNode(PNode nextNode)
		{
			ProseObject obj = nextNode.value;

			//	Create a list of ProseObjects that, if they appeared in a pattern, would match with obj.
			//	For each such object, query the pattern tree to see if our pattern can be extended any further.
			//	If it can be extended further
			//		for each such extension (except the first)
			//			Clone this patternMatcher and then advance that matcher to represent the extension.
			//			Add the new matcher to the output children list
			//		for the first such extension:
			//			Modify this matcher toe represent the extension.
			//	If it can't be extended further
			//		isComplete = true;

			//	You're NEVER allowed to match parenthesis
			if (obj == runtime.LeftParenthesis || obj == runtime.RightParenthesis) {
				throw new RuntimeFailure("getListOfMatchingPatternObjects received parenthesis.");
			}

			switch (state)
			{
			case MatcherState.MATCHING_OBJECT:
				return while_MATCHING_OBJECT_matchNextObject(nextNode);

			case MatcherState.MATCHING_PROSE:
				return while_MATCHING_PROSE_matchNextObject(nextNode);

			case MatcherState.MATCHING_TEXT:
				return while_MATCHING_TEXT_matchNextObject(nextNode);

			case MatcherState.MATCHING_PATTERN:
				return while_MATCHING_PATTERN_matchNextObject(nextNode);
			}

			return null;
		}


		#region Object Matching


		private void switchToState_MATCHING_OBJECT()
		{
			state = MatcherState.MATCHING_OBJECT;
		}


		private List<ProseObject> getMatchingPatternWords(ProseObject obj)
		{
			List<ProseObject> matchingPatternWords = new List<ProseObject>(16);

			//
			//	Create a list of ProseObjects that, if they appeared in a pattern, would match with obj.
			//

			//	Objects match themselves
			matchingPatternWords.Add(obj);

			if (obj is ProseAction)
			{
				matchingPatternWords.Add(runtime.@prose);
				matchingPatternWords.Add(runtime.@action);
				//	Pure actions can appear in text.
				if ( ((ProseAction) obj).IsPure ) {
					matchingPatternWords.Add(runtime.@text);
				}
			}
			else if (obj is StringLiteralObject)
			{
				matchingPatternWords.Add(runtime.@prose);
				matchingPatternWords.Add(runtime.@string);
				matchingPatternWords.Add(runtime.@text);
			}
			else if (obj is RawWordObject)
			{
				matchingPatternWords.Add(runtime.@prose);
				matchingPatternWords.Add(runtime.@raw);
				//	Don't need to add @text because "" handles it.
			}
			else if (obj is Word)
			{
				//	If we see an opening "" we could start matching prose or text or string
				if (obj == runtime.Quadquote) {
					matchingPatternWords.Add(runtime.@string);
					matchingPatternWords.Add(runtime.@text);
					matchingPatternWords.Add(runtime.@prose);
				}
				//	If we see an opening {, the ONLY options is that we're matching prose.
				else if (obj == runtime.LeftCurlyBracket) {
					//if (obj != runtime.@prose)
						matchingPatternWords.Add(runtime.@prose);
				}
				else
				{
					//	Arbitrary words can be @prose or @pattern (neither of which require
					//	any parenthetical notation).  @text is left out because "" always tells
					//	you when you're entering a text block.

					//	I DON"T UNDERATND WHAT THESE IF STATEMENTS WERE SUPPOSED TO DO
					//if (obj != runtime.@prose)
						matchingPatternWords.Add(runtime.@prose);
					//if (obj != runtime.@pattern)
						matchingPatternWords.Add(runtime.@pattern);

					matchingPatternWords.Add(runtime.@raw);

					//	If it's a word, then all descendent words are matches.
					Word word = (Word) obj;
					matchingPatternWords.AddRange(word.getAllDescendents(runtime));
				}
			}

			return matchingPatternWords;
		}


		//	Read a new object and maybe switch to reading prose, text, or pattern if necessary.
		List<PatternMatcher> while_MATCHING_OBJECT_matchNextObject(PNode node)
		{
			ProseObject obj = node.value;
			//	Some symbols get special treatment at the beginning or end of 
			if (	obj == runtime.Period
			    ||	obj == runtime.Semicolon
			    &&	currNode == patternTrie.Root)
			{
				
			}

			//	Get a list of words which, if they appeared in a pattern, would match with this one.
			List<ProseObject> matchingPatternWords = getMatchingPatternWords(obj);

			//
			//	For each matchingPatternWord, query the pattern tree to see if
			//	our pattern can be extended any further.
			//

			List<PatternMatcher> babyMatchers = new List<PatternMatcher>(16);

			bool foundAMatch = false;
			//ProseObject myExtensionObject = null;
			Trie<ProseObject, List<Phrase>>.Node myExtensionNode = null;
			foreach(ProseObject objMatch in matchingPatternWords)
			{
				ProseObject match = objMatch;



				//Word match = (Word) objMatch;
				//	Look up the node that would correspond to the pattern word that would match.
				Trie<ProseObject, List<Phrase>>.Node matchNode = currNode.getChildNode(match);
				//	If we can't find it, then there are no patterns with that matching word, so skip it
				if (matchNode == null)
					continue;
				//	If we can find it, then we might need to make a new matcher
				//	Since ONE match has to be reserved for THIS, and the other matches spawn new matchers,
				//	we use "foundAMatch" to decide which to do.
//				if (foundAMatch)
//				{
					//	Spawn off a baby matcher and transform its state to represent the new possibility.
				PatternMatcher babyMatcher = this.makeCopyWithStateFromAtWord(match);
				babyMatcher.while_MATCHING_OBJECT_extendWith(node, matchNode);
				babyMatchers.Add(babyMatcher);		//	List this baby matcher
				foundAMatch = true;
//				}
//				else {
//					//	Cache these two so we can use them later.
//					myExtensionObject = match;
//					myExtensionNode = matchNode;
//					foundAMatch = true;
//				}
			}

			//	If we found at least one, then we need to tweak ourselves and add ourselves to the babies.
//			if (foundAMatch) {
//				this.while_MATCHING_OBJECT_extendWith(node, myExtensionNode);
//				babyMatchers.Add(this);
//			}
//			else {
//				state = MatcherState.FAILED;
//			}

			if (!foundAMatch) {
				state = MatcherState.FAILED;
			}
			return babyMatchers;
		}



		private MatcherState stateFromAtWord(Word word)
		{
			if (word == runtime.@prose) {
				return MatcherState.MATCHING_PROSE;
			}
			else if (word == runtime.@text) {
				return MatcherState.MATCHING_TEXT;
			}
			else if (word == runtime.@pattern) {
				return MatcherState.MATCHING_PATTERN;
			}

			return MatcherState.MATCHING_OBJECT;
		}


		//	Take a single pattern matcher and actually add the next object and node to it
		//	If the pattern object involves @prose, @text, or @pattern then switch to those states.
		//	This is only called while MATCHING_OBJECT.  The @prose, @text, @pattern matching algorithms
		//	do their extensions internally.
//		private void while_MATCHING_OBJECT_extendWith(ProseObject matchedObject, Trie<ProseObject, List<Phrase>>.Node patternNode)
		private void while_MATCHING_OBJECT_extendWith(PNode node, Trie<ProseObject, List<Phrase>>.Node patternNode)
		{


			//	Depending on the contents of patternNode we may need to change state.
			//	If
			//	If we do change state, then re-match
			ProseObject key = patternNode.getKey();
			if (key == runtime.@prose) {
				//	Either this node matches directly with a word in the pattern, or it's the first node in
				//	a match of @prose, @text, @pattern.  Either way, it goes in the patternComponentNodes.
				patternComponentNodes.Add(node);
//				this.currNode = patternNode;
				//this.numObjectsMatched++;
				switchToState_MATCHING_PROSE();
				while_MATCHING_PROSE_extendWith(node, patternNode);
				if (currNode.Value != null && currNode.Value.Count != 0) {
					becomeMatched(node.next);
				}
			} else if (key == runtime.@text) {
				switchToState_MATCHING_TEXT();
			} else if (key == runtime.@pattern) {
				switchToState_MATCHING_PATTERN();
				while_MATCHING_PATTERN_extendWith(node, patternNode);
			}
			//  A text expression masquerading as a string.
			else if (	key == runtime.@string
			         && node.value == runtime.Quadquote)
			{
				switchToState_MATCHING_TEXT();
				while_MATCHING_TEXT_extendWith(node, patternNode);
			}
			else {

				//	Either this node matches directly with a word in the pattern, or it's the first node in
				//	a match of @prose, @text, @pattern.  Either way, it goes in the patternComponentNodes.
				patternComponentNodes.Add(node);
				this.currNode = patternNode;
				this.numObjectsMatched++;

				//	If there's something to match at this node, and we're not entering a
				//	fancy matcher state.  Then, having a value at this patternNode is the same
				//	thing as saying, there's a phrase out there that matches US.  So we're a match.
				if (state == MatcherState.MATCHING_OBJECT &&
				    patternNode.Value != null &&
				    patternNode.Value.Count != 0) {

					//isMatched = true;
					becomeMatched(node.next);
				}
			}

		}

		#endregion

		#region @prose Matching

		int objectsInCurrentProseblock = 0;

		private void switchToState_MATCHING_PROSE()
		{
			state = MatcherState.MATCHING_PROSE;
			
			//	Create a new block to hold the building Prose.
			proseBlock = new ProseBlockObject();
			parentheticalStack = new Stack<ProseObject>();
			objectsInCurrentProseblock = 0;
		}


		List<PatternMatcher> while_MATCHING_PROSE_matchNextObject(PNode node)
		{
			ProseObject obj = node.value;

			//	Return this list of spawned matchers.
			List<PatternMatcher> babyMatchers = new List<PatternMatcher>();

			//	Possibly we can make this prose block longer (in addition to other options where we may leave it)
			bool canExtendThisProseBlock = false;
			//	If we want extend our prose block by adding a right parenthetical, remember to pop the left off the stack.
			bool shouldPopParentheticalStack = false;
			//	If we stay in this prose block, record if we are entering text or leaving text
			bool shouldToggleInText = false;
			//	Should we exit to a different mode?
			bool shouldLeaveMatchingProse = false;
			MatcherState switchToThisState;


			//	Check for parentheticals
			//	No need to check for () because it's eliminated before we're ever called.
			if (obj == runtime.LeftCurlyBracket) {
				canExtendThisProseBlock = true;
				//	Do this in extend
				//parentheticalStack.Push(runtime.LeftCurlyBracket);
			}
			else if (obj == runtime.LeftSquareBracket) {
				canExtendThisProseBlock = true;
				//	Do this in extend
				//parentheticalStack.Push(runtime.LeftSquareBracket);
			}
			else if (obj == runtime.RightCurlyBracket) {
				ProseObject matchingParenthetical = tryPeekParenthetical();
				if (matchingParenthetical == null || matchingParenthetical != runtime.LeftCurlyBracket) {
					throw new PatternMatcherException("Right curly bracket wihtout matching left curly bracket inside @prose block.", this);
				}
				//	At this point, we agree the curly bracket makes sense, so we pop it's counterpart off the stack.
				shouldPopParentheticalStack = true;
				canExtendThisProseBlock = true;
			}
			else if (obj == runtime.RightSquareBracket) {
				ProseObject matchingParenthetical = tryPeekParenthetical();
				if (matchingParenthetical == null || matchingParenthetical != runtime.LeftSquareBracket) {
					throw new PatternMatcherException("Right square bracket without matching left square bracket inside @prose block.", this);
				}
				//	At this point, we agree the square bracket makes sense, so we pop it's counterpart off the stack.
				shouldPopParentheticalStack = true;
				canExtendThisProseBlock = true;
			}
			else if (obj == runtime.Quadquote) {
				//	In this case, we count that we are in a text block, but we don't "read it as text".  Instead, we just
				//	bundle the symbols in with our prose object and pass it along to be interpreted later.
				if (inText) {
					ProseObject matchingParenthetical = tryPeekParenthetical();
					if (matchingParenthetical == null || matchingParenthetical != runtime.Quadquote) {
						//	The "" didn't match up with a previous one!
						throw new PatternMatcherException("Quadquote entagled with other parenthetical.", this);
					} else {
						canExtendThisProseBlock = true;
						shouldToggleInText = true;				//	inText = false;
						shouldPopParentheticalStack = true;		//	Pop off the left ""
					}
				}
				else {
					//	In this case, we treat this as an opening quadquote.
					canExtendThisProseBlock = true;
					parentheticalStack.Push(runtime.Quadquote);		//	Remember the left ""
					shouldToggleInText = true;						//	inText = true;
				}
			}

			//	When the parenthetical stack is empty, we are allowed to exit the prose block if
			//	we can match the next piece of the pattern.
			else if (parentheticalStack.Count == 0) {
//				if (obj == runtime.Period) {
//					//	A period ends a "prose" block unless it is protected by {} or [] or "" ""
//					//	The period does not appear in the block.  It is interpreted as ending the sentence.
//					//	So exit reading prose
//					shouldLeaveMatchingProse = true;
//					//switchToThisState = MatcherState.MATCHING_OBJECT;
//					//CURSOR
//				} else {
				//	Use inheritance to look for an exit!
				//	Look up words which would match if they were in a pattern.
				List<ProseObject> matchingPatternWords = getMatchingPatternWords(obj);
				//	Look these words up to see if actual patterns exist.
				canExtendThisProseBlock = (matchingPatternWords.Count != 0);
				foreach(ProseObject match in matchingPatternWords)
				{
					//	If it's not a period and the attempt to extend @prose immediately failed,
					//	then we continue matching @prose.  If the attempt didn't immediately fail then
					//	the rules say we must leave @prose.
					if (obj == runtime.Period) // && !babyMatcher.IsntFailed)
						canExtendThisProseBlock = false;

					if (match == runtime.@prose) continue;	//	back to back @prose outlawed.

					//	Look up the match
					Trie<ProseObject, List<Phrase>>.Node matchNode = currNode.getChildNode(match);
					if (matchNode == null)	
						continue;

					//	If this pattern can be extended by leaving prose, then do it
					//	Spawn a baby matcher to pursue this pattern
					PatternMatcher babyMatcher = makeCopyWithStateFromAtWord(match);	//	Clone ourselves
					babyMatcher.switchToState_MATCHING_OBJECT();						//	Move babyMatcher into the generic state (can accept anything)
					babyMatcher.while_MATCHING_OBJECT_extendWith(node, matchNode);		//	Append the new node
					babyMatchers.Add(babyMatcher);										//	Eventually return this baby matcher
				
					//	If it's possible to exit an @prose block, the matcher MUST
					canExtendThisProseBlock = false;

				}
			}

			//	In this case the parenthetical stack is not empty and we have some symbol that
			//	isn't parenthetical (or "").
			else {
				//	Add it to this prose block
				canExtendThisProseBlock = true;
			}


			//
			//	Perform updates according to flags
			//
			if (shouldToggleInText) {
				inText = !inText;
			}
			if (shouldPopParentheticalStack) {
				parentheticalStack.Pop();
			}
			if (canExtendThisProseBlock) {
				//	Extend ourselves!
				//	NOTE: We don't change the node we're using!
				PatternMatcher babyMatcher = makeCopyWithStateFromAtWord(runtime.@prose);	//	Clone ourselves
				babyMatcher.objectsInCurrentProseblock = objectsInCurrentProseblock;
				babyMatcher.while_MATCHING_PROSE_extendWith(node, currNode);
				babyMatchers.Add(babyMatcher);
			}
			else if (shouldLeaveMatchingProse) {
				throw new Exception("Failboat.");
//				PatternMatcher babyMatcher = makeCopyWithStateFromAtWord(obj);	//	Clone ourselves
//				//babyMatcher.switchToState_MATCHING_OBJECT();
//				babyMatcher.while_MATCHING_OBJECT_extendWith(node, );
//				babyMatchers.Add(babyMatcher);
			}

			return babyMatchers;
		}


		private ProseObject tryPeekParenthetical() {
			try {
				return parentheticalStack.Peek();
			}
			catch {
				return null;
			}
		}

		private void while_MATCHING_PROSE_extendWith(PNode node, Trie<ProseObject, List<Phrase>>.Node patternNode)
		{
			this.numObjectsMatched++;
			this.currNode = patternNode;
			this.objectsInCurrentProseblock++;

			ProseObject obj = node.value;

			if (obj == runtime.LeftCurlyBracket) {
				parentheticalStack.Push(runtime.LeftCurlyBracket);
			}
			else if (obj == runtime.LeftSquareBracket) {
				parentheticalStack.Push(runtime.LeftSquareBracket);
			}
			//	The first time this is called is from outside fo while_MATCHING_PROSE_matchNext
			else if (obj == runtime.Quadquote &&  objectsInCurrentProseblock == 1) {
				//	In this case, we treat this as an opening quadquote.
				parentheticalStack.Push(runtime.Quadquote);		//	Remember the left ""
				inText = true;
				//shouldToggleInText = true;						//	inText = true;
			}

			//	...we don't really do anything.  The beginning of the @prose block was already noted
			//	If there's something here, then we are a match
			if (currNode.Value != null && currNode.Value.Count != 0) {
				//isMatched = true;
				becomeMatched(node.next);
			}
		}

		#endregion

		#region @text Matching

		int textMatchingQQcount = 0;

		private void switchToState_MATCHING_TEXT()
		{
			state = MatcherState.MATCHING_TEXT;
			textMatchingQQcount = 0;
		}

		List<PatternMatcher>  while_MATCHING_TEXT_matchNextObject(PNode node)
		{
			ProseObject obj = node.value;

			List<PatternMatcher> babyMatchers = new List<PatternMatcher>();

			//	Extend ourselves!
			//	NOTE: We don't change the node we're using!
			PatternMatcher babyMatcher = makeCopyWithStateFromAtWord(runtime.@text);	//	Clone ourselves
			babyMatcher.while_MATCHING_TEXT_extendWith(node, currNode);
			babyMatchers.Add(babyMatcher);

			return babyMatchers;
		}

		private void while_MATCHING_TEXT_extendWith(PNode node, Trie<ProseObject, List<Phrase>>.Node patternNode)
		{
			this.numObjectsMatched++;			
			this.currNode = patternNode;

			if (node.value == runtime.Quadquote) {
				textMatchingQQcount++;
				//	If this is the first time through, record the beginning of the text block
				if (textMatchingQQcount == 1) {
					patternComponentNodes.Add(node);
				}
			}
			
			//	We become matched when the text ends.
			if (	currNode.Value != null && currNode.Value.Count != 0
			    &&	textMatchingQQcount == 2)
			{
				//isMatched = true;
				becomeMatched(node.next);
			}
			else if (textMatchingQQcount > 2)
			{
				state = MatcherState.FAILED;
			}
			else if (textMatchingQQcount == 2)
			{
				switchToState_MATCHING_OBJECT();
			}
		}

		#endregion
		#region @pattern Matching


		PatternObject currPatternObject = null;
		bool justPutArgName = false;

		public PatternObject CurrPatternObject {
			get {	return currPatternObject;	}
		}


		private void switchToState_MATCHING_PATTERN()
		{
			state = MatcherState.MATCHING_PATTERN;

			//	Counts 1 when hits [, increments for each word.  When it gets
			//	to ] it makes sure the count is at 2.  More words is illegal, no words is illegal.
			//	Also, checks that the words are words or RawWords.
			bracketReaderCount = 0;
			prevBracketReaderCount = 0;
			if (currPatternObject == null)
				currPatternObject = new PatternObject();
		}

		private void setBracketReaderCount(int newCount) {
			prevBracketReaderCount = bracketReaderCount;
			bracketReaderCount = newCount;
		}



		List<PatternMatcher>  while_MATCHING_PATTERN_matchNextObject(PNode node)
		{
			List<PatternMatcher> babyMatchers = new List<PatternMatcher>();
			ProseObject obj = node.value;

			//	@Pattern won't match periods
			if (obj == runtime.Period) {
				return babyMatchers;
			}

			//
			switch (bracketReaderCount)
			{
			case 0:
				//	Check to see if obj is -> :+ or :-
				if (	obj == runtime.RightArrow
				    ||	obj == runtime.ColonPlus
				    ||	obj == runtime.ColonMinus)
				{
					//	If so, then we have to leave matching.  See if anything is checking for this symbol
					Trie<ProseObject, List<Phrase>>.Node childNode = currNode.getChildNode(obj);
					if (childNode != null) {
						PatternMatcher babyMatcher = makeCopyWithStateFromAtWord(obj);	//	Clone ourselves
						babyMatcher.while_MATCHING_OBJECT_extendWith(node, childNode);
						babyMatchers.Add(babyMatcher);
					}
				}
				else if (	obj == runtime.Period
				         ||	!(obj is Word))
				{
					return babyMatchers;
				}
				else
				{
					PatternMatcher babyMatcher = makeCopyWithStateFromAtWord(runtime.@pattern);
					babyMatcher.while_MATCHING_PATTERN_extendWith(node, currNode);	//	Use same node
					if (babyMatcher.IsntFailed)
						babyMatchers.Add(babyMatcher);
				}
				break;

			case 1:
			{
				//	In this case, just trust while_..._extendWith to do the right thing.
				PatternMatcher babyMatcher = makeCopyWithStateFromAtWord(runtime.@pattern);
				babyMatcher.bracketReaderCount = bracketReaderCount;
				babyMatcher.while_MATCHING_PATTERN_extendWith(node, currNode);	//	Use same node
				if (babyMatcher.IsntFailed)
					babyMatchers.Add(babyMatcher);
				break;
			}
			}

			return babyMatchers;
		}


		void appendToCurrPatternObject() {
//			if (prevObjInPattern != null)
//				currPatternObject.putPatternElement(prevObjInPattern, prevArgInPattern);
//
//			prevObjInPattern = null;
//			prevArgInPattern = null;
		}

		private void while_MATCHING_PATTERN_extendWith(PNode node, Trie<ProseObject, List<Phrase>>.Node patternNode)
		{
			//	If this is the first node we're pushing in as a "pattern", then add it to patternComponentNodes
			if (currPatternObject.Length == 0) {
				patternComponentNodes.Add(node);
			}

			ProseObject obj = node.value;
			switch (bracketReaderCount)
			{
			case 0:
				if (obj == runtime.LeftSquareBracket) {
					//	Attempting to name an argument when no argument type preceeds it!
					if (currPatternObject.Length == 0) {
						state = MatcherState.FAILED;
						return;
					}
					numObjectsMatched++;
					setBracketReaderCount(1);
				}
				else {
					if (obj is Word) {
						currPatternObject.putPatternElement(obj);
						numObjectsMatched++;
					}
					else {
						state = MatcherState.FAILED;
						return;
					}
				}
				break;

			case 1:
				if (obj == runtime.RightSquareBracket) {
					justPutArgName = false;
					setBracketReaderCount(0);
					numObjectsMatched++;
				}
				//	If its a word or raw word then it should be an argument name
				else if (obj is Word  ||  obj is RawWord) {
					if (!justPutArgName) {
						currPatternObject.replaceLastPutElementNameWith(obj);
						justPutArgName = true;
						numObjectsMatched++;
					}
					else {
						//	Can't supply two arg names.
						state = MatcherState.FAILED;
						return;
					}
				}
				break;
			}

			//	VERY IMPORTANT:  Keep the matcher moving up the tree.
			currNode = patternNode;
		}


		#endregion

		#region Argument Extraction

		//	Just before
		public void autoCastArguments()
		{
			ProseObject[] pattern = AssociatedPattern;
			for (int i=0; i < pattern.Length; i++)
			{
				//	Check for auto-formatting of text expression into string

				//	Auto-cast a text expression into a string by evaluating it.
				if (	pattern[i] == runtime.@string
				    &&	Matching[i].value == runtime.Quadquote)
				{
					PNode textEnd;
					PNode textStart = getArgumentBounds(i, out textEnd);

					//	Create a StringLiteralObject to substitute
					string textAsString = runtime.parseTextExpressionIntoString(textStart, textEnd);
					PNode literal = new PNode(new StringLiteralObject(textAsString));

					//	Splice in this literal
					literal.prev = textStart.prev;
					if (textStart.prev != null)
						textStart.prev.next = literal;
					literal.next = textEnd;
					if (textEnd != null)
						textEnd.prev = literal;

					//	Correct the match list.
					Matching[i] = literal;
				}
			}
		}




		public List<ProseObject> getArgumentAsProseAtIndex(int idx)
		{
			//	Start at the given index
			PNode start = patternComponentNodes[idx];

			//	Slightly different if asked for the last one.
			PNode end;
			if (idx == patternComponentNodes.Count - 1)
			{
				end = terminatorNode;
			}
			else {
				end = patternComponentNodes[idx + 1];
			}

			//	Auto-unwrapping of {}
			if (	start.value == runtime.LeftCurlyBracket
			    &&	end.prev.value == runtime.RightCurlyBracket)
			{
				//	If the prose block starts with { and ends with } then throw them out.
				start = start.next;
				end = end.prev;
			}

			List<ProseObject> proseOut = new List<ProseObject>(12);
			PNode p = start;
			while (p != end)
			{
				proseOut.Add(p.value);
				p = p.next;
			}

			return proseOut;
		}

		public PNode getArgumentBounds(int idx, out PNode terminator)
		{
			//	Start at the given index
			PNode start = patternComponentNodes[idx];
			
			//	Slightly differen if asked for the last one.
			PNode end;
			if (idx == patternComponentNodes.Count - 1)
			{
				end = terminatorNode;
			}
			else {
				end = patternComponentNodes[idx + 1];
			}
			terminator = end;
			return start;
		}

		#endregion

	}
}








































