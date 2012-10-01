using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Prose
{
	public class ProseRuntime
	{
		//	Converts raw text into arrays of RawWords for us.
		ProseSourcePreParser preParser = new ProseSourcePreParser();

		public ProseRuntime ()
		{
			//	Initialize the word trie
			ProseLanguage.constructInitialWordTrie(this);
		}


		//	Read the Prose source text and change state accordingly.
		public void read(string sourceText)
		{
			//	Parse words from the source text and repackage it as a linked list of prose objects.
			Word[] words = parseWordsFromString(sourceText);
			//	Wrap the parsed words as linked list.
			PNode expression = new PNode(words);
			//	Apply patterns until we have nothing but actions left.
			PNode actions = reduceWordsToActions(expression);
			//	Do the actions
			performActions(actions);
		}


		public Word[] parseWordsFromString(string sourceText)
		{
			//
			//	Convert the string into a list of words
			//
			RawWord[] rawWords = preParser.parseSourceIntoRawWords(sourceText);
			Word[] words = new Word[rawWords.Length];
			int i=0;
			foreach(RawWord r in rawWords) {
				//TODO:	We're just using one word words right now.
				RawWord[] oneWordWord = { r };

				//	Look up the word.  If it isn't there, complain.
				Word word = lookupWordFromRawWords(oneWordWord);
				if (word == null)
					failBecauseWordNotFound(oneWordWord);

				words[i] = word;	//	Save the word

				//DEBUG:
//				string readable = words[i].getReadableString();
//				Console.WriteLine(readable);

				//	Next
				i++;
			}
			return words;
		}


		//	Reduces the list of ProseObjects to a list containing only ProseActions.
		//	If it can't, it throws an exception.
		public PNode reduceWordsToActions(PNode expression)
		{
			return null;
		}


		public void performActions(PNode actionList)
		{
			PNode currNode = actionList;
			while (currNode != null)
			{
				if (!(currNode is ProseAction))
					throw new ProseFailure("A non-action prose object leaked into the 'performActions' method.");
				((ProseAction) currNode).performAction(this);
				currNode = currNode.next;
			}
		}


		#region Failure


		private void failBecauseWordNotFound(RawWord[] rawWords)
		{
			throw new WordNotFoundException(rawWords, "");
		}


		#endregion

		#region Words

		//
		//	Since which "words" are legal and which aren't is determined at runtime.  This data must
		//	be put here.  RawWords are not runtime objects.  Words are.
		//
	

		//	A Trie allowing us to build up words from raw words.
		internal Trie<RawWord, Word> wordTrie = new Trie<RawWord, Word>();

		public Word lookupWordFromRawWords(RawWord[] words)
		{
			return wordTrie.getValue(words);
		}

		//	Return an object depending only on the contents of the words.
		public Word new_WordFromWords(RawWord[] words)
		{
			//	Look for the word in the trie
			Word foundWord = wordTrie.getValue(words);
			//	If it isn't there, add it
			if (foundWord == null)
			{
				Word newWord = new Word(words);				//	Create the word
				wordTrie.putObjectString(words, newWord);	//	Put the word into the Trie with address = words
				return newWord;
			}
			else
				return foundWord;
		}


		public bool addWordFromStrings(string[] rawStrings)
		{		
			//	Convert to RawWord objects.
			List<RawWord[]> rawWordList = new List<RawWord[]>();

			RawWord[] rawWords = new RawWord[rawStrings.Length];
			int i=0;
			foreach(string s in rawStrings)
			{
				rawWords[i] = RawWord.new_FromString(s);
				i++;
			}
			rawWordList.Add(rawWords);

			//	Add the
			return wordTrie.putObjectString(rawWords, new_WordFromWords(rawWords));
		}

		#endregion

		#region Inheritance

		WordTrie phraseBank = new WordTrie();

		#endregion

	}
}




































