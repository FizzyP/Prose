using System;
using System.Collections.Generic;

namespace ProseLanguage
{
	public class ProseLanguage
	{

		//	The method signature of foreign actions (C# methods)
		public delegate void ActionDelegate(ProseRuntime runtime, List<ProseObject> args);


		public class Raw {

			//	@words

			private static RawWord @PROSE = RawWord.new_FromString ("@prose");
			private static RawWord @TEXT = RawWord.new_FromString ("@text");
			private static RawWord @STRING = RawWord.new_FromString ("@string");
			private static RawWord @RAW = RawWord.new_FromString ("@raw");
			private static RawWord @PATTERN = RawWord.new_FromString ("@pattern");
			private static RawWord @ACTION = RawWord.new_FromString ("@action");

			//	Core Symbols
			private static RawWord COLON = RawWord.new_FromString(":");
			private static RawWord PERIOD = RawWord.new_FromString(".");
			private static RawWord SEMICOLON = RawWord.new_FromString(";");
			private static RawWord COMMA = RawWord.new_FromString(",");
			private static RawWord QUADQUOTE = RawWord.new_FromString ("\"\"");

			//	Parentheticals
			private static RawWord LEFT_PAREN = RawWord.new_FromString("(");
			private static RawWord RIGHT_PAREN = RawWord.new_FromString(")");
			private static RawWord LEFT_SQ_BRACKET = RawWord.new_FromString("[");
			private static RawWord RIGHT_SQ_BRACKET = RawWord.new_FromString("]");
			private static RawWord LEFT_CURLY_BRACKET = RawWord.new_FromString("{");
			private static RawWord RIGHT_CURLY_BRACKET = RawWord.new_FromString("}");

			//	Inherritance
			private static RawWord PLUS_COLON = RawWord.new_FromString("+:");
			private static RawWord MINUS_COLON = RawWord.new_FromString("-:");
			private static RawWord LEFT_ARROW = RawWord.new_FromString("<-");

			//	Pattern Matching
			private static RawWord COLON_PLUS = RawWord.new_FromString(":+");
			private static RawWord COLON_MINUS = RawWord.new_FromString(":-");
			private static RawWord RIGHT_ARROW = RawWord.new_FromString("->");

			//	Builtin Words
			private static RawWord WORD = RawWord.new_FromString("word");
			private static RawWord PHRASE = RawWord.new_FromString("phrase");

			//
			//	Accessors
			//
			public static RawWord @prose {	get {	return @PROSE;	}	}
			public static RawWord @text {	get {	return @TEXT;	}	}
			public static RawWord @string {	get {	return @STRING;	}	}
			public static RawWord @raw {	get {	return @RAW;	}	}
			public static RawWord @pattern {	get {	return @PATTERN;	}	}
			public static RawWord @action {	get {	return @ACTION;	}	}

			public static RawWord Colon {	get {	return COLON;	}	}
			public static RawWord Period {	get {	return PERIOD;	}	}
			public static RawWord Semicolon {	get {	return SEMICOLON;	}	}
			public static RawWord Comma {	get {	return COMMA;	}	}
			public static RawWord Quadquote {	get {	return QUADQUOTE;	}	}

			public static RawWord LeftParenthesis {	get {	return LEFT_PAREN;	}	}
			public static RawWord RightParenthesis {	get {	return RIGHT_PAREN;	}	}
			public static RawWord LeftSquareBracket {	get {	return LEFT_SQ_BRACKET;	}	}
			public static RawWord RightSquareBracket {	get {	return RIGHT_SQ_BRACKET;	}	}
			public static RawWord LeftCurlyBracket {	get {	return LEFT_CURLY_BRACKET;	}	}
			public static RawWord RightCurlyBracket {	get {	return RIGHT_CURLY_BRACKET;	}	}

			public static RawWord PlusColon {	get {	return PLUS_COLON;	}	}
			public static RawWord MinusColon {	get {	return MINUS_COLON;	}	}
			public static RawWord LeftArrow {	get {	return LEFT_ARROW;	}	}

			public static RawWord ColonPlus {	get {	return COLON_PLUS;	}	}
			public static RawWord ColonMinus {	get {	return COLON_MINUS;	}	}
			public static RawWord RightArrow {	get {	return RIGHT_ARROW;	}	}

			public static RawWord word {	get {	return WORD;	}	}
			public static RawWord phrase {	get {	return PHRASE;	}	}


		}

		//	Build up fundamental words of the language inside the runtime before
		//	it begins processing other commands.
		static public void constructInitialWordTrie(ProseScope scope)
		{
			//	Pack all the data into a list and add each word to the runtime dictionary.
			List<string[]> wordList = buildWordList();
			foreach (string[] wordStrings in wordList) {
				scope.addWordFromStrings(wordStrings);
			}
		}
	



		//	Actually create the list
		static private List<string[]> buildWordList()
		{
			List<string[]> wordList = new List<string[]>();

//			//	Symbols
//			wordList.Add(new string[]{","});
//			wordList.Add(new string[]{";"});
//			wordList.Add(new string[]{"."});
//
//			wordList.Add(new string[]{"("});
//			wordList.Add(new string[]{")"});
//			wordList.Add(new string[]{"["});
//			wordList.Add(new string[]{"]"});
//			wordList.Add(new string[]{"{"});
//			wordList.Add(new string[]{"}"});
//
//			wordList.Add(new string[]{"\"\""});
//
//			wordList.Add(new string[]{"'"});
//			wordList.Add(new string[]{"-"});

			//	Language constructurs
			wordList.Add(new string[]{"word"});
			wordList.Add(new string[]{"phrase"});

			wordList.Add(new string[]{"load"});
			wordList.Add(new string[]{"assembly"});
			wordList.Add(new string[]{"type"});
			wordList.Add(new string[]{"method"});

			//	Pattern matching words
			wordList.Add(new string[]{"@prose"});
			wordList.Add(new string[]{"@text"});
			wordList.Add(new string[]{"@string"});
			wordList.Add(new string[]{"@pattern"});
			wordList.Add(new string[]{"@raw"});
			wordList.Add(new string[]{"@action"});

			wordList.Add(new string[]{"@assembly"});
			wordList.Add(new string[]{"@type"});


			//	IO (minimal necessary to load libraries)
			wordList.Add(new string[]{"read"});
			wordList.Add(new string[]{"contents"});
			wordList.Add(new string[]{"of"});
			wordList.Add(new string[]{"text"});
			wordList.Add(new string[]{"file"});
			wordList.Add(new string[]{"path"});


			//	Variable names
			wordList.Add(new string[]{"x"});

			return wordList;
		}


		static public void constructInitialInheritance(ProseRuntime runtime)
		{
			ProseScope scope = runtime.GlobalScope;

			//runtime.Period.addParent(runtime.Comma);
			runtime.Period.addParent (runtime.Semicolon);
			runtime.Semicolon.addParent(runtime.Comma);


			runtime.Word_phrase.addParent(runtime.Word_word);
		}

		static public void constructInitialPatternTrie(ProseRuntime runtime)
		{
			ProseScope scope = runtime.GlobalScope;
			Trie<ProseObject, List<Phrase>> patternTrie = scope.PatternTree;

			//
			//	Pattern Creation Pattern
			//

			//	The only pattern we need to "force" into the system is the pattern for making patterns:
			//	word[phrase_class] : @pattern[pattern] -> @prose[value] .

//			Phrase phrasePhrase = new SimplePhrase(runtime.Word_phrase,
//			                                       new ProseObject[] { runtime.Word_word, 

			//patternTrie.putObjectString(

			#region Word binding phrases
			//	, word : @raw ,
			{
				ProseObject[] commaDelimitedBindWordsPattern = new ProseObject[]
				{runtime.Comma, runtime.Word_word, runtime.Colon, runtime.@raw, runtime.Comma};
				WordBindingPhrase bindWords_commaDelimited = new WordBindingPhrase( runtime.Word_phrase, commaDelimitedBindWordsPattern );
				scope.addPhrase(bindWords_commaDelimited);                                                                 	
			}
			//	, word +: @raw ,
			{
				ProseObject[] commaDelimitedBindWordsPattern = new ProseObject[]
				{runtime.Comma, runtime.Word_word, runtime.PlusColon, runtime.@raw, runtime.Comma};
				WordBindingPhrase bindWords_commaDelimited = new WordBindingPhrase( runtime.Word_phrase, commaDelimitedBindWordsPattern );
				scope.addPhrase(bindWords_commaDelimited);                                                                 	
			}
			//	, word <- @raw ,
			{
				ProseObject[] commaDelimitedBindWordsPattern = new ProseObject[]
				{runtime.Comma, runtime.Word_word, runtime.LeftArrow, runtime.@raw, runtime.Comma};
				ExclusiveWordBindingPhrase exclusiveBindWords_commaDelimited = new ExclusiveWordBindingPhrase( runtime.Word_phrase, commaDelimitedBindWordsPattern );
				scope.addPhrase(exclusiveBindWords_commaDelimited);  
			}

			#endregion

			#region Phrase creation phrases

			// 	, word[class]: @pattern -> @prose[value] ,
			{
				ProseObject[] phrasePattern = new ProseObject[]
				{runtime.Comma, runtime.Word_word, runtime.Colon, runtime.@pattern, runtime.RightArrow, runtime.@prose, runtime.Comma};
				ExclusivePhraseBindingPhrase phrasePhrase = new ExclusivePhraseBindingPhrase(runtime.Word_phrase, phrasePattern);
				scope.addPhrase(phrasePhrase);
			}

			#endregion

			#region Reading


			//	, read @string[x] ,
			{
				ProseObject[] p = new ProseObject[]
				{runtime.Comma, runtime.word("read"), runtime.word("@string"), runtime.Comma};
				Phrase readPhrase = new ReadPhrase(runtime.Word_phrase, p);
				scope.addPhrase(readPhrase);
			}


			//	contents of text file @string[file_name]
			{
				ProseObject[] p = new ProseObject[]
				{runtime.word("contents"), runtime.word("of"), runtime.word("text"), runtime.word("file"), runtime.word("@string")};
				Phrase readFilePhrase = new ContentsOfTextFilePhrase(runtime.Word_phrase, p);
				scope.addPhrase(readFilePhrase);
			}

			runtime.read("phrase: , read file @string[path] ,  ->  , read contents of text file path ,", runtime.GlobalClient);

			#endregion

			#region Foreign Function Interface

			// 	, load assembly : @string[file_name] <- @raw[new_assembly_word] ,
			{
				ProseObject[] p = new ProseObject[]
				{runtime.Comma, runtime.word("load"), runtime.word("assembly"), runtime.Colon,
					runtime.@string, runtime.LeftArrow, runtime.@raw, runtime.Comma};
				AssemblyPhrase asmPhrase = new AssemblyPhrase(runtime.Word_phrase, p);
				scope.addPhrase(asmPhrase);
			}

			#endregion


			#region Experimental
			//	, -> @pattern -> ,
			{
				ProseObject[] test = new ProseObject[]
				{runtime.Comma, runtime.RightArrow, runtime.@pattern, runtime.RightArrow, runtime.Comma};
				DebugOutputPhrase dbg = new DebugOutputPhrase("Carlybou", runtime.Word_phrase, test);
				scope.addPhrase(dbg);
			}
			#endregion
		}

	}
}




































