
//
//		A Trie mapping Object -> Object
//


using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	public class Trie<KeyT, ValT> 	where ValT : class
									where KeyT : class
	{

		private Trie<KeyT, ValT>.Node root = new Trie<KeyT, ValT>.Node(null);

		internal Trie<KeyT, ValT>.Node Root {	get {	return root;	}	}


		//	Map a given object path to a value in the Trie.
		public bool putObjectPath(KeyT[] characters, ValT value)
		{
//			Node p = root;
//
//			for (int n=0; n < characters.Length; n++)
//			{
//
//				KeyT character = characters[n];
//				//	Try to get a pointer to the next node
//				Node child = p.getChildNode(character);
//				//	If there is no next node, then make it.
//				if (child == null)
//				{
//					if (n == characters.Length - 1)
//					{
//						p.setChildNode(character, new Node(character, value));
//						return false;	//	False: it wasn't here already.
//					}
//					else
//					{
//						p.setChildNode(character, new Node(character, null));
//					}
//				}
//				else
//				{
//					//	Move the pointer down the Trie.
//					p = child;
//				}
//			}
//			//	If we get to this point, then the final node wasn't new (or we would have returned).
//			//	So, we just need to overwrite the value in that location.

			bool created;
			Node p = getOrCreateNodeAtObjectPath(characters, out created);
			p.setValue(value);

			return created;	//	true because we overwrote what was there.
		}

		internal Node getOrCreateNodeAtObjectPath(KeyT[] characters, out bool created) {
			Node p = root;
			created = false;

			for (int n=0; n < characters.Length; n++)
			{
				
				KeyT character = characters[n];
				//	Try to get a pointer to the next node
				Node child = p.getChildNode(character);
				//	If there is no next node, then make it.
				if (child == null)
				{
					child = new Node(character, null);
					p.setChildNode(character, child);
					created = true;
				}

				//	Move the pointer down the Trie.
				p = child;
			}

			//	Return the node we found or created.
			return p;
		}


		//	Returns null if the path doesn't end anywhere or if null is stored there.
		//	We treat these as equivalent.
		public ValT getFirstValue(KeyT[] objectString)
		{
			Node p = root;

			foreach (KeyT character in objectString)
			{
				p = p.getChildNode(character);
				if (p == null)
					return null;
				if (p.Value != null)
					return p.Value;
			}
			return null;
		}


		public ValT getValue(KeyT[] objectString)
		{
			Node p = root;

			foreach (KeyT character in objectString)
			{
				p = p.getChildNode(character);
				if (p == null)
					return null;
			}
			return p.Value;
		}



		//
		//
		//

		internal class Node
		{
			private KeyT character;
			private ValT value;
			private Dictionary<KeyT, Node> childMap = new Dictionary<KeyT, Node>();


			public ValT Value {
				get {	return value;	}
			}

			public Node(KeyT character)
			{
				this.character = character;
				value = null;
			}

			public Node(KeyT character, ValT value)
			{
				this.character = character;
				this.value = value;
			}

			public Node getChildNode(KeyT character)
			{
				Node childNode;
				if (childMap.TryGetValue(character, out childNode))
					return childNode;
				else
					return null;
			}

			public void setChildNode(KeyT character, Node childNode)
			{
				childMap.Add(character, childNode);
			}

			public void setValue(ValT value)
			{
				this.value = value;
			}

			public KeyT getKey()
			{
				return character;
			}

			public bool HasChildren
			{
				get {
					return childMap.Count > 0;
				}
			}


		}


	}
}

