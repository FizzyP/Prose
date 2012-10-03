using System;
using System.Collections.Generic;
using System.Text;
using ProseLanguage;
using Prose;


namespace ProseLib
{
	public class REPL
	{
		static REPL() {
			string name = typeof(REPL).FullName.ToString();
		}

		public REPL() {}

		public static void WriteMethod(ProseRuntime runtime, List<ProseObject> args)
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(assembleArgumentsIntoString(args, "WriteMethod takes only string arguments."));
		}


		public static void ErrorMethod(ProseRuntime runtime, List<ProseObject> args)
		{
			ProseREPL.reportException(assembleArgumentsIntoString(args, "ErrorMethod takes only string arguments."));
		}


		private static string assembleArgumentsIntoString(List<ProseObject> args, string errMsg)
		{
			StringBuilder str = new StringBuilder();
			foreach (ProseObject po in args) {
				if (po is StringLiteralObject) {
					str.Append(((StringLiteralObject) po).literal);
				}
				else {
					throw new ArgumentException(errMsg);
				}
			}

			return str.ToString();
		}

		//	Set which events are logged
		public static void SetShowEvent(ProseRuntime runtime, List<ProseObject> args)
		{
			ProseREPL.setShowEvent(runtime, args);
		}
	
	}
}

