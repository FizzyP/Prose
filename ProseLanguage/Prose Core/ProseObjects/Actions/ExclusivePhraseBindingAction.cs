using System;
using System.Text;

namespace ProseLanguage
{
	public class ExclusivePhraseBindingAction : ProseAction
	{
		Phrase phrase;

		//	An action that takes a phrase and "creates it" within a scope.
		public ExclusivePhraseBindingAction (Phrase phraseToCreate)
		{
			phrase = phraseToCreate;
		}

		public bool IsPure {	get {	return false;	}	}

		public string getReadableString() {

			return "XBindPhrase{" + phrase.getReadableString() + "}";
		}



		public void performAction(ProseRuntime runtime)
		{
			runtime.addPhraseAndDeleteExistingPhrases(phrase);
		}

		public ProseObject[] getIsa() {	return new ProseObject[0];	}


		// Tags
		private UInt64 tag;
		public void setTag(UInt64 tag) {	this.tag = tag;	}
		public UInt64 getTag() {	return tag;	}
	}
}

