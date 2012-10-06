
/*
 * 	This class represents a bank of phrases and words.  A ProseRuntime instance
 * 	will read from and write to several of these.
 * 
 */

using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	public class ProseScope
	{
		public ProseScope ()
		{


		}

		#region Words

		//	A Trie allowing us to build up words from raw words.
		private Trie<RawWord, Word> wordTrie = new Trie<RawWord, Word>();

		//	Given a list of contiguous (traditional) words, either look it up
		//	and return it, or create it and return it.
		public Word addWordFromStrings(string[] rawStrings)
		{
			//	Convert to RawWord objects.
			//List<RawWord[]> rawWordList = new List<RawWord[]>();

			RawWord[] rawWords = new RawWord[rawStrings.Length];
			int i=0;
			foreach(string s in rawStrings)
			{
				rawWords[i] = RawWord.new_FromString(s);
				i++;
			}
			// rawWordList.Add(rawWords);


			return addWordFromRawWords(rawWords);
		}

		//	Lookup/add a word to the current scope representing this list of raw words.
		public Word addWordFromRawWords(RawWord[] rawWords)
		{
			Word newWord = new_WordFromWords(rawWords);
			wordTrie.putObjectPath(rawWords, newWord);
			return newWord;
		}

		public Word addWordFromRawWord(RawWord rawWord) {
			return addWordFromRawWords(new RawWord[] { rawWord });
		}

		public Word searchWordFromRawWords(RawWord[] rawWords) {
			return wordTrie.getValue(rawWords);
		}

		public Word word(string s)
		{
			return wordTrie.getValue(new RawWord[] {RawWord.new_FromString(s)});
		}

		internal Trie<RawWord, Word>.Node getWordLookupRoot()
		{
			return wordTrie.Root;
		}

		//	Put a word we already have
		public Word addWord(Word word)
		{
			wordTrie.putObjectPath(word.RawWords, word);
			return word;
		}

		public void addProseLanguageWords()
		{
			ProseLanguage.constructInitialWordTrie(this);
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
				wordTrie.putObjectPath(words, newWord);	//	Put the word into the Trie with address = words
				return newWord;
			}
			else
				return foundWord;
		}

		#endregion

		#region Phrases

		//	Patterns  matched by the phrases, mapped to the list of such phrases.
		private Trie<ProseObject, List<Phrase>> patternTrie = new Trie<ProseObject, List<Phrase>>();

		public Trie<ProseObject, List<Phrase>> PatternTree {
			get {	return patternTrie;	}
		}

		public bool addPhrase(Phrase phrase)
		{
			//	Get the node where the phrase belongs
			bool created;
			Trie<ProseObject, List<Phrase>>.Node phraseNode = patternTrie.getOrCreateNodeAtObjectPath(phrase.getPattern(), out created);

			//	Add the phrase to the node
			if (phraseNode.Value == null)					//	If we have to make a place in the node to put our phrase...
				phraseNode.setValue(new List<Phrase>());
			phraseNode.Value.Add(phrase);
			return created;
		}

		public bool addPhraseAndDeleteExistingPhrases(Phrase phrase)
		{
			//	Get the node where the phrase belongs
			bool created;
			Trie<ProseObject, List<Phrase>>.Node phraseNode = patternTrie.getOrCreateNodeAtObjectPath(phrase.getPattern(), out created);
			
			//	Make this phrase the only one at the node.
			phraseNode.setValue(new List<Phrase>(1));
			phraseNode.Value.Add(phrase);
			return created;
		}


		#endregion

		#region Actions


		#endregion

	}
}

