using System;

namespace Prose
{
	public class LNode
	{
		public LNode next;
		public Object value;

		public LNode (Object value)
		{
			this.value = value;
			this.next = null;
		}

		public static LNode new_List(Object[] values, out LNode lastNode)
		{
			LNode firstNode = new LNode(values[0]);
			firstNode.next = null;

			if (values.Length == 0)
			{
				firstNode.value = null;
				lastNode = firstNode;
				return firstNode;
			}

			LNode prevNode = firstNode;
			LNode currNode = null;
			for(int i=1; i < values.Length; i++)
			{
				currNode = new LNode(values[i]);
				prevNode.next = newNode;
				prevNode = currNode;
			}

			currNode.next = null;
			lastNode = currNode;
			return firstNode;
		}
	}
}

