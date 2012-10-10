using System;
using ProseLib;
using System.Collections.Generic;

namespace Prose
{
	public class ProseREPL
	{
		public static void Main (string[] args)
		{
			REPL.initializeConsole();
			REPL.runRegressionTest();
			REPL.enterProseREPL();
			REPL.seeYaLater();
		}
	}
}
