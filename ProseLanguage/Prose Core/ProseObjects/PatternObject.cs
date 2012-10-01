using System;
using System.Collections.Generic;
using System.Text;

namespace ProseLanguage
{
	public class PatternObject : ProseObjectBase, ProseObject
	{
		internal List<ProseObject> patternElements;	//	The pieces of the pattern
		internal List<ProseObject> elementNames;	//	The "argument" names of those pieces (possibly null)

		public int Length { get { return patternElements.Count;	} }

		public PatternObject ()
		{
			patternElements = new List<ProseObject>(4);
			elementNames = new List<ProseObject>(4);
		}

		public string getReadableString() {
			StringBuilder s = new StringBuilder(40);
			for (int i=0; i < patternElements.Count; i++) {
				s.Append(patternElements[i].getReadableString());
				ProseObject eltName = elementNames[i];
				if (eltName != null) {
					s.Append("[");
					s.Append(eltName.getReadableString());
					s.Append("]");
				}
				s.Append(" ");
			}
			//	Throw out the extra space at the end
			if (patternElements.Count > 0)
				s.Remove(s.Length - 1, 1);

			return s.ToString();
		}

		//	Put an a new element into the pattern.  If it has an argument name, put that name, otherwise second arg = null.
		public void putPatternElement(ProseObject element, ProseObject name)
		{
			patternElements.Add(element);
			elementNames.Add (name);
		}

		//	Add an element without a name
		public void putPatternElement(ProseObject element)
		{
			patternElements.Add (element);
			elementNames.Add(null);
		}

		//	Overwrite the name written for the previous element
		public void replaceLastPutElementNameWith(ProseObject newName)
		{
			elementNames[elementNames.Count - 1] = newName;
		}

		public PatternObject clone()
		{
			PatternObject p = new PatternObject();
			p.patternElements = new List<ProseObject>(patternElements);
			p.elementNames = new List<ProseObject>(elementNames);
			return p;
		}

		public ProseObject[] Pattern {
			get {
				return patternElements.ToArray();
			}
		}
	}
}

