using System;
using System.Collections.Generic;
using ProseLanguage;

namespace ProseLib
{
	public class ProseIO
	{

		public static void WriteMethod(ProseRuntime runtime, List<ProseObject> args)
		{
			Console.WriteLine(((StringLiteralObject) args[0]).literal);
		}
	}
}

