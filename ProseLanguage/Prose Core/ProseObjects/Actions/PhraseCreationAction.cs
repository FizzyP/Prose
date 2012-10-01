using System;

namespace ProseLanguage
{
	public class PhraseCreationAction : ProseAction
	{
		Phrase phrase;

		//	An action that takes a phrase and "creates it" within a scope.
		public PhraseCreationAction (Phrase phraseToCreate)
		{
			phrase = phraseToCreate;
		}

		public bool IsPure {	get {	return false;	}	}

		public string getReadableString() {
			return "XBindPhrase{}";
		}

		public void performAction(ProseRuntime runtime)
		{
			runtime.addPhrase(phrase);
		}

		public ProseObject[] getIsa() {	return new ProseObject[0];	}


		// Tags
		private UInt64 tag;
		public void setTag(UInt64 tag) {	this.tag = tag;	}
		public UInt64 getTag() {	return tag;	}
	}
}

