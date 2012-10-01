using System;
using System.Collections.Generic;
using ProseLanguage;

namespace Prose
{
	class MainClass
	{


		public static void Main (string[] args)
		{
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
				Console.Write("\n> ");
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
				Console.Write("\n> ");
				string instring = Console.ReadLine();
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
					Console.WriteLine("RuntimeProseLanguageException: " + e.Report);
				}

				catch (LexerSourceException e)
				{
					Console.WriteLine("Lexer Source Exception: " + e.Report);
				}
//				catch (RuntimeLexerSourceException e)
//				{
//					Console.WriteLine("Runtime Lexer Source Exception: " + e.Report);
//				}

			}

		}



	}
}
