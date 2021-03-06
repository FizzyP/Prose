using System;
using System.Collections.Generic;
using System.Text;
using ProseLanguage;


namespace ProseLib
{
	public class REPL
	{
		static REPL() {
			string name = typeof(REPL).FullName.ToString();
		}

		public REPL() {}

		private static ProseRuntime runtime;
		public static ProseRuntime Runtime {
			get {	return runtime; }
			set { 	runtime = value; }
		}
		
		
		private static ProseRuntime.OnAmbiguityDelegate onAmbiguityDelegate = new ProseRuntime.OnAmbiguityDelegate(onAmbiguity);
		
		private static ProseRuntime.OnProgressReportDelegate onProgressReportDelegate = new ProseRuntime.OnProgressReportDelegate(onProgressReport);
		private static bool showProgressReport = false;
		private static ProseRuntime.OnParseSentenceDelegate onParseSentenceDelegate = new ProseRuntime.OnParseSentenceDelegate(onParseSentence);
		private static bool showParseSentenceReport = false;
		private static ProseRuntime.BeforePerformingActionDelegate beforePerformingActionDelegate = new ProseRuntime.BeforePerformingActionDelegate(beforePerformingAction);
		private static bool showBeforeActionReport = false;
		private static ProseRuntime.AfterPerformingActionDelegate afterPerformingActionDelegate = new ProseRuntime.AfterPerformingActionDelegate(afterPerformingAction);
		private static bool showAfterActionReport = false;

		private static ProseRuntime.BeforeReductionDelegate beforeReductionDelegate = new ProseRuntime.BeforeReductionDelegate(beforeReduction);
		private static bool showBeforeReductionReport = false;
		private static ProseRuntime.AfterReductionDelegate afterReductionDelegate = new ProseRuntime.AfterReductionDelegate(afterReduction);
		private static bool showAfterReductionReport = false;

		private static bool showOnMatchReport = false;
		private static ProseRuntime.OnMatchDelegate onMatchDelegate = new ProseRuntime.OnMatchDelegate(onMatch);
		private static bool showOnMatcherFailureReport = false;
		private static ProseRuntime.OnMatcherFailureDelegate onMatcherFailureDelegate = new ProseRuntime.OnMatcherFailureDelegate(onMatcherFailure);


		private static int breakPointDepth = 0;
		
		//	Flags controlling behavior of REPL queued from prose
		private static bool shouldContinue = false;
		private static bool shouldExit = false;
		private static bool shouldGetNewREPLRuntime = false;

		
		
		public static void howdy()
		{
			restoreConsoleColor();
			Console.WriteLine("------------------------------------------");
			Console.WriteLine("Welcome to Prose.");
			Console.WriteLine("");
		}
		
		public static void seeYaLater()
		{
			restoreConsoleColor();
			Console.WriteLine("------------------------------------------");
			Console.WriteLine ("Goodbye. Write more Prose soon!");
		}
		
		
		
		
		
		public static void initNewREPLRuntime()
		{
			initNewCleanRuntime ();
			initializeConsole();
			tryRead("read file \"Libraries/REPL/REPL.prose\"");
		}
		
		
		public static void initNewCleanRuntime()
		{
			runtime = new ProseRuntime();
			
			//			runtime.OnProgressReport += new ProseRuntime.OnProgressReportDelegate(onProgressReport);
			//			showProgressReport = true;
			
			//runtime.OnAmbiguity += new ProseRuntime.OnAmbiguityDelegate(onAmbiguity);

			//runtime.BeforePerformingAction += new ProseRuntime.BeforePerformingActionDelegate(beforePerformingAction);
			//runtime.AfterPerformingAction += new ProseRuntime.AfterPerformingActionDelegate(afterPerformingAction);
			
			showParseSentenceReport = false;
			showAfterActionReport = false;
			showBeforeActionReport = false;
			showProgressReport = false;
			showBeforeReductionReport = false;
			showAfterActionReport = false;
			showOnMatchReport = false;
			showOnMatcherFailureReport = false;
			

			runtime.OnParseSentence += onParseSentenceDelegate;
			showParseSentenceReport = true;

			//runtime.AfterReduction += afterReductionDelegate;
			showAfterReductionReport = false;

			runtime.OnBreakPoint += new ProseRuntime.OnBreakPointDelegate(onBreakPoint);
			runtime.OnAmbiguity += new ProseRuntime.OnAmbiguityDelegate(onAmbiguity);
		}

		public static void runRegressionTest()
		{
			initNewCleanRuntime();
			try {
				runtime.read ("read file \"Libraries/REPL/regression.prose\"", runtime.GlobalClient);
				runtime.read ("read file \"adventure.prose\"", runtime.GlobalClient);
			}
			catch (Exception e) {
				Console.WriteLine("Automated regression test failed: " + e.Message);
				Console.WriteLine("Press return to continue.");
				Console.ReadLine();
			}
		}
		
		
		public static void enterProseREPL()
		{
			initNewREPLRuntime();
			howdy();
			
			ProseREPLLoop();
			
		}


		public static void ProseREPLLoop()
		{
			while (true) {
				printPrompt();
				string instring = Console.ReadLine();
				restoreConsoleColor();

				if (instring == "clean.") {
					restoreConsoleColor();
					Console.WriteLine("Building new runtime.");
					initNewCleanRuntime();
					Console.BackgroundColor = ConsoleColor.White;
					Console.ForegroundColor = ConsoleColor.Black;
					Console.WriteLine("Switched to new runtime.");
					continue;
				}
				
				tryRead (instring);

				if (shouldExit) {
					shouldExit = false;
					break;
				}
				if (shouldContinue) {
					shouldContinue = false;
					break;
				}
				if (shouldGetNewREPLRuntime) {
					shouldGetNewREPLRuntime = false;
					restoreConsoleColor();
					Console.WriteLine("Building new runtime.");
					initNewREPLRuntime();
					Console.BackgroundColor = ConsoleColor.White;
					Console.ForegroundColor = ConsoleColor.Black;
					Console.WriteLine("Switched to new runtime.");
					continue;
				}
			}
		}
		
		static void tryRead(string instring)
		{
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
		
		
		public static void initializeConsole()
		{
			restoreConsoleColor();
			Console.Clear();
			Console.Clear();
			Console.Clear();
			Console.Clear();
			Console.Clear();
			Console.Clear();
			//Console.SetBufferSize(120, 100);
		}
		
		static void restoreConsoleColor()
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
		}
		
		public static void reportException(string s)
		{
			Console.ForegroundColor = ConsoleColor.Black;
			Console.BackgroundColor = ConsoleColor.Yellow;
			Console.WriteLine(s);
			restoreConsoleColor();
		}
		
		static void printPrompt()
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.WriteLine();
			
			writeBreakPointDepthMarker();
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Write("> ");
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
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write("progress>      ");
			writePrettyProseWithProgressMark(runtime, beginningOfFragment, progressMark, 0);
		}
		
		static void writeBreakPointDepthMarker()
		{
			int depth = runtime.CallDepth;
			Console.BackgroundColor = ConsoleColor.Red;
			for (int i=0; i < runtime.CallDepth; i++) {
				Console.Write("   ");
			}
		}
		
		static void writeStackDepthMarker(ProseRuntime runtime)
		{
			Console.BackgroundColor = ConsoleColor.Red;
			for (int i=0; i < runtime.CallDepth - 1; i++) {
				Console.Write("   ");
			}
		}
		
		//	Pass null to progress mark to eliminate
		static void writePrettyProseWithProgressMark(ProseRuntime runtime, PNode start, PNode progressMark, int maxNodesToProcess)
		{
			
			writeStackDepthMarker(runtime);
			
			//Stack<ProseObject> parenStack = new Stack<ProseObject>();
			int parenCount = 0;
			int bracketCount = 0;
			int quadQuoteCount = 0;
			int periodCount = 0;
			int nodesProcessed = 0;
			
			PNode p = start;
			do {
				nodesProcessed++;
				
				while (p != null && p.value == null) {
					p = p.next;
				}
				
				if (p.value == null) {
					p = p.next;
					continue;
				}
				p = runtime.filterIncomingPNode(p);
				if (p == null)
					break;
				
				//	PROGRESS MARK
				if (p == progressMark) {
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Write (" ");
					Console.BackgroundColor = ConsoleColor.DarkBlue;
					Console.Write (" ");
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Write (" ");
				}
				
				//	Write a leading space
				Console.BackgroundColor = ConsoleColor.Black;
				Console.Write(" ");
				
				
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
				         || p.value == runtime.ColonMinus
				         || p.value == runtime.Quadquote)
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
				
				//	Inside a text block everything is forced to white unless its in brackets
				//	Deal with different highighting inside of text expressions
				if (quadQuoteCount % 2 == 1) {
					if (p.value == runtime.LeftSquareBracket) {
						bracketCount++;
					}
					
					//	Neutralize colors of words that behave as text
					if (bracketCount == 0) {
						Console.BackgroundColor = ConsoleColor.Black;
						Console.ForegroundColor = ConsoleColor.Cyan;
					}
					
					if (p.value == runtime.RightSquareBracket) {
						bracketCount--;
					}
				}
				
				if (	p.value == runtime.@break
				    ||	p.value is BreakPointObject)
				{
					Console.BackgroundColor = ConsoleColor.White;
					Console.ForegroundColor = ConsoleColor.Black;
				}
				
				
				//	Prewrite logic
				
				
				
				//	Write the output
				//
				
				Console.Write(p.value.getReadableString());
				//	Just so nothing too ugly can go wrong
				Console.BackgroundColor = ConsoleColor.Black;
				
				//	Postwrite logic
				
				if (		p.value == runtime.RightArrow
				    || p.value == runtime.ColonPlus
				    || p.value == runtime.ColonMinus)
				{
					if (	Console.CursorLeft > 60
					    ||	Console.CursorLeft + 40 > Console.BufferWidth)
					{
						restoreConsoleColor();
						Console.WriteLine();
						Console.Write ("               ");
						writeStackDepthMarker(runtime);
						restoreConsoleColor();
						Console.Write ("\t\t");
					}
				}
				
				
				//
				//	Other exits
				//
				if (nodesProcessed == maxNodesToProcess)
					break;
				
				//
				//	Keep track of parentheticals to know if we can quit
				//
				if (	p.value == runtime.LeftParenthesis
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
			Console.BackgroundColor = ConsoleColor.Yellow;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.WriteLine("ambiguity> ");
			foreach (PatternMatcher match in matches)
			{
				foreach (Phrase phrase in match.MatchedPhrases) {
					Console.WriteLine(phrase.getReadableString());
				}
			}
		}
		
		static void onParseSentence(ProseRuntime runtime, PNode source)
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("postparse>     ");
			writePrettyProseWithProgressMark(runtime, source, null, 0);
		}
		
		static void beforePerformingAction(ProseRuntime runtime, PNode source)
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("preaction>     ");
			writePrettyProseWithProgressMark(runtime, source, null, 1);
		}
		
		static void afterPerformingAction(ProseRuntime runtime, PNode source)
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("postaction>    ");
			writePrettyProseWithProgressMark(runtime, source, null, 1);
		}
		
		static void beforeReduction(ProseRuntime runtime, PNode source)
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("prereduction>  ");
			writePrettyProseWithProgressMark(runtime, source, null, 0);
		}

		static void afterReduction(ProseRuntime runtime, PNode source)
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("postreduction> ");
			writePrettyProseWithProgressMark(runtime, source, null, 0);
		}

		public static void onBreakPoint(ProseRuntime runtime, PNode source, BreakPointObject.RuntimeData rtdata, string script)
		{
			breakPointDepth++;
			
			runtime.read(script, runtime.GlobalClient);
			runtime.read("read file \"Libraries/REPL/onbreak.prose\"", runtime.GlobalClient);
			ProseREPLLoop();
			
			breakPointDepth--;
		}

		public static void onMatch(ProseRuntime runtime, PatternMatcher match) {
			restoreConsoleColor();
			foreach (Phrase phrase in match.MatchedPhrases)
				Console.WriteLine("match>          " + phrase.getReadableString());
		}

		public static void onMatcherFailure(ProseRuntime runtime, PatternMatcher match) {
			restoreConsoleColor();
			StringBuilder str = new StringBuilder();
			ProseObject[] partialPattern = match.AssociatedPattern;
			foreach(ProseObject po in partialPattern) {
				str.Append(po.getReadableString());
				str.Append(" ");
			}
			if (str.Length != 0)
				str.Remove(str.Length-1, 1);

			Console.WriteLine("match fail>     " + str.ToString());
		}

		public static void setShowEvent(ProseRuntime runtime, List<ProseObject> args)
		{
			if (args.Count != 2)
				throw new ArgumentException("SetShowEvent requires an event and a yes/no value.");
			
			string eventWord = args[0].getReadableString();
			string yesNoWord = args[1].getReadableString();
			
			//	PROGRESS REPORTS
			if (eventWord == "progress") {
				if (yesNoWord == "yes")	{
					if (!showProgressReport) {
						runtime.OnProgressReport += onProgressReportDelegate;
						showProgressReport = true;
					}
				}
				else {
					runtime.OnProgressReport -= onProgressReportDelegate;
					showProgressReport = false;
				}
			}
			
			//	POSTPARSE REPORT
			if (eventWord == "postparse") {
				if (yesNoWord == "yes")	{
					if (!showParseSentenceReport) {
						runtime.OnParseSentence += onParseSentenceDelegate;
						showParseSentenceReport = true;
					}
				}
				else {
					runtime.OnParseSentence -= onParseSentenceDelegate;
					showParseSentenceReport = false;
				}
			}


			//	POSTACTION REPORTS
			if (eventWord == "postaction") {
				if (yesNoWord == "yes")	{
					if (!showAfterActionReport) {
						runtime.AfterPerformingAction += afterPerformingActionDelegate;
						showAfterActionReport = true;
					}
				}
				else {
					runtime.AfterPerformingAction -= afterPerformingActionDelegate;
					showAfterActionReport = false;
				}
			}


			//	PREACTION REPORTS
			if (eventWord == "preaction") {
				if (yesNoWord == "yes")	{
					if (!showBeforeActionReport) {
						runtime.BeforePerformingAction += beforePerformingActionDelegate;
						showBeforeActionReport = true;
					}
				}
				else {
					runtime.BeforePerformingAction -= beforePerformingActionDelegate;
					showBeforeActionReport = false;
				}
			}

			//	PREREDUCTION REPORTS
			if (eventWord == "prereduction") {
				if (yesNoWord == "yes")	{
					if (!showBeforeReductionReport) {
						runtime.BeforeReduction += beforeReductionDelegate;
						showBeforeReductionReport = true;
					}
				}
				else {
					runtime.BeforeReduction -= beforeReductionDelegate;
					showBeforeReductionReport = false;
				}
			}

			//	POSTREDUCTION REPORTS
			if (eventWord == "postreduction") {
				if (yesNoWord == "yes")	{
					if (!showAfterReductionReport) {
						runtime.AfterReduction += afterReductionDelegate;
						showAfterReductionReport = true;
					}
				}
				else  {
					runtime.AfterReduction -= afterReductionDelegate;
					showAfterReductionReport = false;
				}
			}
		
			//	MATCH REPORTS
			if (eventWord == "matches") {
				if (yesNoWord == "yes")	{
					if (!showOnMatchReport) {
						runtime.OnMatch += onMatchDelegate;
						showOnMatchReport = true;
					}
				}
				else {
					runtime.OnMatch -= onMatchDelegate;
					showOnMatchReport = false;
				}
			}

			//	MATCHER FAILURE REPORTS
			if (eventWord == "matchfails") {
				if (yesNoWord == "yes")	{
					if (!showOnMatcherFailureReport) {
						runtime.OnMatcherFailure += onMatcherFailureDelegate;
						showOnMatcherFailureReport = true;
					}
				}
				else {
					runtime.OnMatcherFailure -= onMatcherFailureDelegate;
					showOnMatcherFailureReport = false;
				}
			}

		}

		public static void WriteMethod(ProseRuntime runtime, List<ProseObject> args)
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(assembleArgumentsIntoString(args, "WriteMethod takes only string arguments."));
		}


		public static void ErrorMethod(ProseRuntime runtime, List<ProseObject> args)
		{
			reportException(assembleArgumentsIntoString(args, "ErrorMethod takes only string arguments."));
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
			setShowEvent(runtime, args);
		}

	
		public static void ContinueFromBreakPoint(ProseRuntime runtime, List<ProseObject> args)
		{
			shouldContinue = true;
		}

		public static void ExitREPL(ProseRuntime runtime, List<ProseObject> args)
		{
			shouldContinue = true;
		}

		public static void SwitchToNewRuntime(ProseRuntime runtime, List<ProseObject> args)
		{
			shouldGetNewREPLRuntime = true;
		}
	}
}

