using System;
using System.Collections.Generic;
using ProseLanguage;
using System.Runtime.InteropServices;

namespace Prose
{
	class MainClass
	{


		public static void Main (string[] args)
		{
			initializeConsole();

			howdy();

			// testPreParser();
			//testRuntimeWordParser();

			//testLexer();

			testRuntimeLexer();

			seeYaLater();
		}


		public static void howdy()
		{
			Console.WriteLine("------------------------------------------");
			Console.WriteLine("Welcome to Prose.");
			Console.WriteLine("");
		}

		public static string getLineFromPrompt()
		{
			writePrompt();
			return Console.ReadLine();
		}

		public static void writePrompt()
		{
			Console.Write("prose: ");
		}

		public static void seeYaLater()
		{
			Console.WriteLine("------------------------------------------");
			Console.WriteLine ("Goodbye. Write more Prose soon!");
		}





//		public static void testPreParser()
//		{
//			string source = getLineFromPrompt();
//			ProseSourcePreParser preParser = new ProseSourcePreParser();
//			RawWord[] rawWords = preParser.parseSourceIntoRawWords(source);
//			foreach(RawWord rawWord in rawWords)
//			{
//				Console.WriteLine(rawWord.AsString);
//			}
//		}
//
//
//		public static void testRuntimeWordParser()
//		{
//			ProseRuntime runtime = new ProseRuntime();
//			try {
//				runtime.read(getLineFromPrompt());
//			}
//			catch (WordNotFoundException e)
//			{
//				Console.WriteLine("I don't know the word \"" + e.RawWords[0].AsString + "\"");
//			}
//		}



		public static void testLexer()
		{
			ProseLexer lexer = new ProseLexer();
			while (true) {
				//printPrompt();
				string instring = Console.ReadLine();
				if (instring == "exit.")
					break;
				try {
					List<LexerToken> tokenized = lexer.parseStringIntoTokens(instring);
					foreach (LexerToken token in tokenized)
						Console.WriteLine(token.rawWord.AsString);
				}
				catch (LexerSourceException e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}




		public static void testRuntimeLexer()
		{
			ProseRuntime runtime = new ProseRuntime();
			while (true) {
				printPrompt();
				string instring = Console.ReadLine();
				restoreConsoleColor();

				if (instring == "exit.")
					break;
				if (instring == "new.") {
					Console.WriteLine("Building new runtime.");
					runtime = new ProseRuntime();
					Console.WriteLine("Switched to new runtime.");
					continue;
				}

				try {
					runtime.read(instring, runtime.GlobalClient);
				}
				catch (RuntimeProseLanguageException e) {
					reportException("\r\nRuntimeProseLanguageException: " + e.Report);
				}

				catch (LexerSourceException e)
				{
					reportException("\r\nLexer Source Exception: " + e.Report);
				}
//				catch (RuntimeLexerSourceException e)
//				{
//					Console.WriteLine("Runtime Lexer Source Exception: " + e.Report);
//				}

			}

		}


		static void initializeConsole()
		{
			restoreConsoleColor();
			Console.Clear();
		}

		static void restoreConsoleColor()
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		static void reportException(string s)
		{
			Console.ForegroundColor = ConsoleColor.Black;
			Console.BackgroundColor = ConsoleColor.Yellow;
			Console.WriteLine(s);
			restoreConsoleColor();
		}

		static void printPrompt()
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write("\r\n> ");
		}

	}
}
