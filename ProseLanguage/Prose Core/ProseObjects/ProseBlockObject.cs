
//
//	A wrapper for a list of prose objects.
//	The only purpose it to provide a single proseObject to store the data for
//	a pattern match with @prose.
//

using System;
using System.Collections.Generic;
using System.Text;

namespace ProseLanguage
{
	public class ProseBlockObject : ProseObjectBase, ProseObject
	{
		private List<ProseObject> body = new List<ProseObject>();

		public List<ProseObject> Body	{
			get {	return body;	}
		}

		public ProseBlockObject ()
		{
		}

		public string getReadableString() {
			StringBuilder msg = new StringBuilder("{");
			foreach(ProseObject o in body) {
				msg.Append(o.getReadableString());
				msg.Append(" ");
			}
			msg.Remove(msg.Length - 1, 1);
			return msg.ToString();
		}

		public void addProseObject(ProseObject obj)
		{
			body.Add(obj);
		}
	}
}

