using System;

/*
 * 		An immutable linked list type.
 */

namespace Prose
{
	public class ProseList
	{
		public Node root;
		public int length;


		public ProseList (Node root, int length)
		{
			this.root = root;
			this.length = length;
		}


		public class Node
		{
			public ProseObject value;
			public Node next, prev;
		}


		public Node LastNode
		{
			get {
				Node p = this.root;
				while (p.next != null)
					p = p.next;
				return p;
			}
		}
	}
}

