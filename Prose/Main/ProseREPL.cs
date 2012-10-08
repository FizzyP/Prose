using System;
using ProseLib;

namespace Prose
{
	public class ProseREPL
	{
		public static void Main (string[] args)
		{

			REPL.initializeConsole();
			REPL.enterProseREPL();
			REPL.seeYaLater();
		}
	}
}
