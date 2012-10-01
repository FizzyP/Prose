using System;
using System.Collections.Generic;


namespace ProseLanguage
{
	public class WordTrie
	{
		private WordTrie.Node root = new WordTrie.Node(null);
		UInt64 visitationTag = 0;


		public WordTrie ()
		{
		}





		//	Map a given object path to a value in the Trie.
		public bool putObjectString(Word[] characters, Phrase phrase)
		{
			Node p = root;

			for (int n=0; n < characters.Length; n++)
			{

				Word character = characters[n];
				//	Try to get a pointer to the next node
				Node child = p.getChildNode(character);
				//	If there is no next node, then make it.
				if (child == null)
				{
					if (n == characters.Length - 1)
					{
						p.setChildNode(character, new Node(character, phrase));
						return false;	//	False: it wasn't here already.
					}
					else
					{
						p.setChildNode(character, new Node(character));
					}
				}
				else
				{
					//	Move the pointer down the Trie.
					p = child;
				}
			}
			//	If we get to this point, then the final node wasn't new (or we would have returned).
			//	So, we just need to overwrite the value in that location.
			p.addPhrase(phrase);
			return true;	//	true because we overwrote what was there.
		}



		//	Returns null if the path doesn't end anywhere or if null is stored there.
		//	We treat these as equivalent.
		public Phrase[] getFirstValue(Word[] objectString)
		{
			Node p = root;

			foreach (Word character in objectString)
			{
				p = p.getChildNode(character);
				if (p == null)
					return null;
				if (p.Value != null)
					return p.Value;
			}
			return null;
		}


		public Phrase[] getValue(Word[] objectString)
		{
			Node p = root;

			foreach (Word character in objectString)
			{
				p = p.getChildNode(character);
				if (p == null)
					return null;
			}
			return p.Value;
		}

		private UInt64 getNewTag() {
			visitationTag++;
			return visitationTag;
		}

		//	Search for optimal phrase matches and return them as an array.
		//	Return only matches that have maximum length and maximal specificity.
//		public Phrase[] matchPhrases(PNode source)
//		{
//			if (source == null)
//				return new Phrase[0];
//
//			UInt64 tag = getNewTag();
//
//			//WordTrie.Node[] 
//			//WordTrie.Node matchHead = root;
//			PNode readHead = source;
//
//			//	Do the first step differently: find all the matches for the first word
//			//	and put them into the phrase list.
//			PNode phraseEnd;
//			PNode phraseRoot = PNode.new_List(readHead.value.getIsa(), out phraseEnd);
//			//	Mark readHead.value as visited.
//			phraseRoot.value.setTag(tag);
//
//
//			readHead = readHead.next;
//			while (readHead != null)
//			{
//				//	
//				readHead = readHead.next;
//			}
//		}



		//
		//
		//

		protected class Node
		{
			private Word character;
			private Phrase[] value;
			private Dictionary<Word, Node> childMap = new Dictionary<Word, Node>();


			public Phrase[] Value {
				get {	return value;	}
			}

			public Node(Word character)
			{
				this.character = character;
				value = new Phrase[0];
			}

			public Node(Word character, Phrase phrase)
			{
				this.character = character;
				value = new Phrase[0];
				addPhrase (phrase);
			}

			public Node getChildNode(Word character)
			{
				Node childNode;
				if (childMap.TryGetValue(character, out childNode))
					return childNode;
				else
					return null;
			}

			public void setChildNode(Word character, Node childNode)
			{
				childMap.Add(character, childNode);
			}

			public void setValue(Phrase[] value)
			{
				this.value = value;
			}

			public void addPhrase(Phrase phrase)
			{
				Phrase[] newPhraseList = new Phrase[value.Length + 1];
				Array.Copy(value, newPhraseList, value.Length);
				newPhraseList[value.Length] = phrase;
			}
		}
	}
}

