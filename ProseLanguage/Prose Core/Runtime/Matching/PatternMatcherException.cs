using System;

namespace ProseLanguage
{
	public class PatternMatcherException : Exception
	{
		private PatternMatcher patternMatcher;
		public PatternMatcher ResponsibleMatcher {	get {	return patternMatcher;	}	}

		public PatternMatcherException (string msg, PatternMatcher matcher) : base(msg)
		{
			patternMatcher = matcher;
		}
	}
}

