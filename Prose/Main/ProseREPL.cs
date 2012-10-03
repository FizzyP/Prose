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



		static ProseRuntime initNewREPLRuntime()
		{
			ProseRuntime runtime = initNewCleanRuntime ();
			runtime.read("read file \"Libraries/REPL/REPL.prose\"", runtime.GlobalClient);
			initializeConsole();
			return runtime;
		}


		static ProseRuntime initNewCleanRuntime()
		{
			ProseRuntime runtime = new ProseRuntime();

			//runtime.OnProgressReport += new ProseRuntime.OnProgressReportDelegate(onProgressReport);
			//runtime.OnAmbiguity += new ProseRuntime.OnAmbiguityDelegate(onAmbiguity);
			runtime.OnParseSentence += new ProseRuntime.OnParseSentenceDelegate(onParseSentence);
			//runtime.BeforePerformingAction += new ProseRuntime.BeforePerformingActionDelegate(beforePerformingAction);
			//runtime.AfterPerformingAction += new ProseRuntime.AfterPerformingActionDelegate(afterPerformingAction);
			return runtime;

		}



		public static void testRuntimeLexer()
		{
			ProseRuntime runtime = initNewRuntime();

			while (true) {
				printPrompt();
				string instring = Console.ReadLine();
				restoreConsoleColor();

				if (instring == "exit.")
					break;
				if (instring == "new.") {
					Console.WriteLine("Building new runtime.");
					runtime = initNewRuntime();
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
			Console.Clear();
			Console.Clear();
			Console.Clear();
			Console.Clear();
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

		static void debugOutput(string s)
		{
//			Console.ForegroundColor = ConsoleColor.Green;
//			Console.BackgroundColor = ConsoleColor.Black;
			Console.WriteLine(s);
//			restoreConsoleColor();
		}

		static void onProgressReport(ProseRuntime runtime, PNode beginningOfFragment, PNode progressMark)
		{
			//debugOutput("- " + beginningOfFragment.getReadableStringWithProgressMark(progressMark));
			writePrettyProseWithProgressMark(runtime, beginningOfFragment, progressMark, 0);
		}

		//	Pass null to progress mark to eliminate
		static void writePrettyProseWithProgressMark(ProseRuntime runtime, PNode start, PNode progressMark, int maxNodesToProcess)
		{
			//Stack<ProseObject> parenStack = new Stack<ProseObject>();
			int parenCount = 0;
			int quadQuoteCount = 0;
			int periodCount = 0;
			int nodesProcessed = 0;

			PNode p = start;
			do {
				nodesProcessed++;
				if (p.value == null) {
					p = p.next;
					continue;
				}

				p = runtime.filterIncomingPNode(p);

				//	PROGRESS MARK
				if (p == progressMark) {
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Write (" ");
					Console.BackgroundColor = ConsoleColor.DarkBlue;
					Console.Write (" ");
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Write (" ");
				}

				//	OBJECTS
				if (p.value is ProseAction) {
					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.Green;
				}
				else if (p.value == runtime.Comma		||
				         p.value == runtime.Semicolon	||
				         p.value == runtime.Period)
				{
					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.Magenta;
				}
				else if (p.value is RawWordObject) {
					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.Red;
				}
//				else if (	p.value == runtime.LefetParenthesis
//				         ||	p.value == runtime.LeftSquareBracket
//				         ||	p.value == runtime.LeftCurlyBracket
//				         || p.value == runtime.RightParenthesis
//				         || p.value == r
				else if (	p.value == runtime.Colon
				         ||	p.value == runtime.LeftArrow
				         || p.value == runtime.PlusColon
				         || p.value == runtime.MinusColon
				         || p.value == runtime.RightArrow
				         || p.value == runtime.ColonPlus
				         || p.value == runtime.ColonMinus)
				{
					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.Cyan;
				}
				else if (	p.value is AssemblyNameWord
				         ||	p.value is TypeNameWord
				         ||	p.value is MethodNameWord)
				{
					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.Yellow;
				}
				else
				{
					Console.BackgroundColor = ConsoleColor.Black;
					Console.ForegroundColor = ConsoleColor.Gray;
				}


				//	Write the output


				Console.Write(" ");

				Console.Write(p.value.getReadableString());
//				outStr.Append(p.value.getReadableString());

				//	Just so nothing too ugly can go wrong
				Console.BackgroundColor = ConsoleColor.Black;


				//
				//	Other exits
				//
				if (nodesProcessed == maxNodesToProcess)
					break;

				//
				//	Keep track of parentheticals to know if we can quit
				//
				if (	p.value == runtime.LefetParenthesis
				    ||	p.value == runtime.LeftCurlyBracket
				    ||	p.value == runtime.LeftSquareBracket)
				{
					parenCount++;
					//parenStack.Push(p.value);
				}
				else if (	p.value == runtime.RightParenthesis
				         || p.value == runtime.RightCurlyBracket
				         ||	p.value == runtime.RightSquareBracket)
				{
					parenCount--;
				}
				else if (p.value == runtime.Quadquote) {
					quadQuoteCount++;
				}
				else if (	p.value == runtime.Period
				         && parenCount == 0
				         &&	quadQuoteCount % 2 == 0)
				{
					periodCount++;
					//	If parens and quadquotes are done and we have period then bail
					if (periodCount == 2)
						break;
				}

				//	Update
				p = p.next;
			} while (p != null);
			//outStr.Remove(outStr.Length - 1, 1);
			//return outStr.ToString();

			Console.WriteLine();

		}

		static void onAmbiguity(ProseRuntime runtime, PNode source, List<PatternMatcher> matches)
		{
		}

		static void onParseSentence(ProseRuntime runtime, PNode source)
		{
			writePrettyProseWithProgressMark(runtime, source, null, 0);
		}

		static void beforePerformingAction(ProseRuntime runtime, PNode source)
		{
		}

		static void afterPerformingAction(ProseRuntime runtime, PNode source)
		{
			writePrettyProseWithProgressMark(runtime, source, null, 1);
		}

	}
}
