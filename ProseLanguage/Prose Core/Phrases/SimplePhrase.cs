using System;
using System.Collections.Generic;
using System.Text;

namespace ProseLanguage
{
	public class SimplePhrase : Phrase
	{
		private ProseObject phraseClass;
		private ProseObject[] pattern;
		protected ProseObject[] value;
		protected ProseObject[] argNames;


		public SimplePhrase(ProseObject phraseClass, ProseObject[] pattern, ProseObject[] argNames, ProseObject[] value)
		{
			this.phraseClass = phraseClass;
			this.pattern = (ProseObject[]) pattern.Clone();
			this.argNames = (ProseObject[]) argNames.Clone ();
			this.value = (ProseObject[]) value.Clone();
		}

		//	This is reserved for subclasses who want to set value[] themselves and call
		//	replaceWithValueAt in their own evaluate methods.
		public SimplePhrase(ProseObject phraseClass, ProseObject[] pattern)
		{
			this.phraseClass = phraseClass;
			this.pattern = (ProseObject[]) pattern.Clone();
			this.value = null;
		}


		public ProseObject[] getPattern() {
			return pattern;
		}

		public ProseObject getPhraseClass() {
			return phraseClass;
		}

		public virtual PNode evaluate(PNode evaluateMe, PatternMatcher match)
		{
			return replaceWithValueAt(evaluateMe, match);
		}

		//	Evaluate a list of prose objects into a new list of prose objects.
		public PNode replaceWithValueAt(PNode evaluateMe, PatternMatcher match)
		{
			//	If the phrase only serves to delete the pattern...
			if (value.Length == 0)
			{
//				PNode firstNodeAfterPattern = evaluateMe.forwardSeek(match.NumObjectsMatched);
				PNode firstNodeAfterPattern = match.terminatorNode;
				if (evaluateMe.prev != null)
					evaluateMe.prev.next = firstNodeAfterPattern;
				firstNodeAfterPattern.prev = evaluateMe.prev;
				return firstNodeAfterPattern;
			}

			//	Make a node to write and hook it up the previous part of the list.
			PNode prevWriteHead = evaluateMe.prev;
			PNode firstNodeInNewList = new PNode();
			PNode writeHead = firstNodeInNewList;
			prevWriteHead.next = writeHead;

			foreach (ProseObject obj in value)
			{

				//	Write a value into it
				//	Argrefs get special treatment.
				if (obj is ArgRefObject)
				{
					//	Look up the appropriate argument in "evaluate me" and substitute it.
					int idx = ((ArgRefObject) obj).reffedArgIndex;
					//ProseObject evaluated = evaluateMe.forwardSeek(index).value;

					PNode subStart, subEnd;
					subStart = match.getArgumentBounds(idx, out subEnd);


					//TODO	This needs to support @prose, @text, and @string.
					//writeHead.value = evaluated;
					PNode readHead = subStart;
					//writeHead.initWithPNode(subStart);
					while (readHead != subEnd) {
						//	Hook p into the output
						if (prevWriteHead != null) {
							prevWriteHead.next = writeHead;
						}
						writeHead.prev = prevWriteHead;
						writeHead.initWithPNode(readHead);

						//	Update
						prevWriteHead = writeHead;
						readHead = readHead.next;
						//	Get a new node for the next cycle
						writeHead = new PNode();
					}

				}
				else {
					//	Hook this node into the list
					writeHead.prev = prevWriteHead;
					if (prevWriteHead != null)
						prevWriteHead.next = writeHead;

					writeHead.value = obj;

					//	Grab a new node
					prevWriteHead = writeHead;
					writeHead = new PNode();
				}
			}

			//	When we're done, tie the end of the output list back into the input list.
			//	Note: prevWriteHead is actually the last PNode we wrote.
			if (prevWriteHead != null)
				prevWriteHead.next = match.terminatorNode;
				//prevWriteHead.next = evaluateMe.forwardSeek(match.NumObjectsMatched);


			return firstNodeInNewList;
		}



		public string getReadableString()
		{
			//return "phrase{}";

			
			StringBuilder str = new StringBuilder();
			str.Append(phraseClass.getReadableString());
			str.Append(": ");
			for (int i=0; i < pattern.Length; i++)
			{
				str.Append(pattern[i].getReadableString());
				if (argNames != null && argNames[i] != null)
				{
					str.Append("[");
					str.Append(argNames[i].getReadableString());
					str.Append("]");
				}
				str.Append(" ");
			}
			if (pattern.Length > 0)
				str.Append(" ");

//			if (pattern.Length > 0)
//				str.Remove(str.Length - 1, 1);

			str.Append("-> ");
			str.Append(getValueDescriptionString());

			return str.ToString();

		}

		public string getValueDescriptionString()
		{
			//	If the value hasn't been determined, look up a static description instead.
			if (value == null)
				return getStaticValueDescriptionString();

			StringBuilder str = new StringBuilder();

			for (int i=0; i < value.Length; i++) {
				if (value[i] is ArgRefObject) {
					int argIdx = ((ArgRefObject) value[i]).reffedArgIndex;
					ProseObject argName = argNames[argIdx];
					if (argName != null) {
						str.Append(argName.getReadableString());
					}
					else {
						str.Append(pattern[argIdx].getReadableString());
					}
				}
				else {
					str.Append(value[i].getReadableString());
				}

				str.Append(" ");
			}

			if (value.Length > 0)
				str.Remove(str.Length - 1, 1);

			return str.ToString();
		}

		public virtual string getStaticValueDescriptionString()
		{
			return "Substitute{}";
		}

		public ProseObject[] getIsa() {	return new ProseObject[0];	}

		// Tags
		private UInt64 tag;
		public void setTag(UInt64 tag) {	this.tag = tag;	}
		public UInt64 getTag() {	return tag;	}
	}
}

