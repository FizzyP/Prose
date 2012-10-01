using System;

namespace Prose
{
	public class MutableProseList : ProseList
	{
		public MutableProseList (ProseList list, int length) : base(list, length)
		{
		}

		public void replaceSublistWith(Node startNode, Node endNode, ProseList newSublist)
		{
			ProseList.Node lowBound = startNode.prev;
			ProseList.Node highBound = endNode.next;

			if (lowBound == null)
			{
				newSublist.root.prev = null;
			}
			else
			{
				lowBound.next = newSublist.root;
				newSublist.root.prev = lowBound;
			}

			ProseList.Node lastSublistNode = newSublist.LastNode;
			if (highBound == null)
			{
				lastSublistNode.next = null;
			}
			else
			{
				lastSublistNode.next = highBound;
				highBound.prev = lastSublistNode;
			}
		}
	}
}

