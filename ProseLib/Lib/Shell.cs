using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ProseLanguage;

namespace ProseLib
{
	public class Shell
	{
		public Shell ()
		{
		}



		//	First argument = application file name
		//	Further arguments = command line arguments
		public static void WriteToShell(ProseRuntime runtime, List<ProseObject> args)
		{
			string appName;
			string argString;

			if (args.Count == 0)
				throw new ArgumentException("Shell command missing executable file name.");

			if (args[0] is StringLiteralObject) {
				appName = ((StringLiteralObject) args[0]).literal;
			}
			else
				appName = args[0].getReadableString();

			StringBuilder argBuilder = new StringBuilder();
			for (int i=1; i < args.Count; i++) {
				if (args[i] is StringLiteralObject) {
					argBuilder.Append(((StringLiteralObject) args[i]).literal);
				}
				else
					argBuilder.Append(args[i].getReadableString());

				argBuilder.Append(" ");
			}
			argString = argBuilder.ToString();

			PrivateWriteToShell(runtime, appName, argString);
		}

		private static void PrivateWriteToShell(ProseRuntime runtime, string appName, string argString)
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine();

			ProcessStartInfo startInfo = new ProcessStartInfo(appName, argString);
			Process.Start(startInfo);
		}

		public static void OpenURL(ProseRuntime runtime, List<ProseObject> args)
		{
			string siteName = "http://";
			if (args.Count == 1 && args[0] is StringLiteralObject) {
				siteName += ((StringLiteralObject) args[0]).literal;
			}
			else
				throw new ArgumentException("OpenURL expects a single string argument.");

			PrivateWriteToShell(runtime, siteName, "");
		}
	}
}

