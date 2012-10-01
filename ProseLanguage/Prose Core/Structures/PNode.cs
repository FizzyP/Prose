using System;
using System.Text;

namespace ProseLanguage
{
	public class PNode
	{
		public ProseObject value;
		public PNode next, prev;
		public PNodeDebugData debugData;

		public PNode ()
		{
			next = prev = null;
			value = null;
			debugData = null;
		}

		public PNode(PNode copyMe)
		{
			value = copyMe.value;
			debugData = copyMe.debugData;
		}


		public PNode(ProseObject obj)
		{
			next = prev = null;
			value = obj;
			debugData = null;
		}

		public PNode(ProseObject[] objs)
		{
			debugData = null;
			this.prev = null;
			if (objs.Length == 0)
			{
				this.value = null;
				this.next = null;
				return;
			}

			if (objs.Length == 1)
			{
				this.value = objs[0];
				this.next = null;
				return;
			}

			PNode prevNode = this;
			PNode currNode = null;
			for (int i=2; i < objs.Length; i++)
			{
				currNode = new PNode();
				prevNode.next = currNode;
				currNode.prev = prevNode;
				currNode.value = objs[i];

				prevNode = currNode;
			}

			currNode.next = null;
			return;
		}

		public void initWithPNode(PNode node)
		{
			this.value = node.value;
			this.debugData = node.debugData;
		}

		public static PNode new_PNodeList(ProseObject[] objs, out PNode lastNode)
		{
			//	Create a first object.
			if (objs.Length == 0) {
				lastNode = null;
				return null;
			}

			PNode firstNode = new PNode();
			firstNode.value = objs[0];
			firstNode.prev = null;
			if (objs.Length == 1)
			{
				firstNode.next = null;
				lastNode = firstNode;
				return firstNode;
			}

			PNode prevNode = firstNode;
			PNode currNode = null;
			for (int i=2; i < objs.Length; i++)
			{
				currNode = new PNode();
				prevNode.next = currNode;
				currNode.prev = prevNode;
				currNode.value = objs[i];

				prevNode = currNode;
			}

			currNode.next = null;

			lastNode = currNode;
			return firstNode;
		}



		public PNode forwardSeek(int idx)
		{
			int i=0;
			PNode currNode = this;
			for (; i < idx && currNode != null; i++)
				currNode = currNode.next;
			return currNode;
		}

		public void makeNextEqualTo(PNode newNext)
		{
			next = newNext;
			if (newNext != null)
			{
				newNext.prev = this;
			}
		}

		public void makePrevEqualTo(PNode newPrev)
		{
			prev = newPrev;
			if (newPrev != null)
			{
				newPrev.next = this;
			}
		}

		public void makeTerminal()
		{
			if (next != null)
				next.prev = null;
			next = null;
		}

		public void removeSelf()
		{
			prev.next = next;
			next.prev = prev;
		}


		public string getReadableString()
		{
			StringBuilder outStr = new StringBuilder();
			PNode p = this;
			do {
				if (p.value == null) {
					p = p.next;
					continue;
				}
				outStr.Append(p.value.getReadableString());
				outStr.Append(" ");
				p = p.next;
			} while (p != null);
			outStr.Remove(outStr.Length - 1, 1);
			return outStr.ToString();
		}

		public string getReadableStringWithProgressMark(PNode progressMark)
		{
			StringBuilder outStr = new StringBuilder();
			PNode p = this;
			do {
				if (p.value == null) {
					p = p.next;
					continue;
				}
				if (p == progressMark)
					outStr.Append(" | ");
				outStr.Append(p.value.getReadableString());
				outStr.Append(" ");
				p = p.next;
			} while (p != null);
			outStr.Remove(outStr.Length - 1, 1);
			return outStr.ToString();
		}
	}
}

