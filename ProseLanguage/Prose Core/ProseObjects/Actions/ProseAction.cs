using System;

namespace ProseLanguage
{
	public interface ProseAction : ProseObject
	{
		//	Is the action "side-effect free"?  I.e. it doesn't modify state.
		bool IsPure {
			get;
		}

		void performAction(ProseRuntime runtime);

	}
}

