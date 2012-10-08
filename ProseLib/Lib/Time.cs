using System;
using ProseLanguage;
using System.Collections.Generic;
using System.Timers;

namespace ProseLib
{
	public class Time
	{
		public static List<Timer> timerList = new List<Timer>();
		public static List<RuntimeRunnable> scriptList = new List<RuntimeRunnable>();
		public static List<ProseRuntime> runtimeList = new List<ProseRuntime>();

		private static Object lockObj = new Object();

		/*
		 * 		Expects all arguments to be strings.
		 * 		
		 * 		@string[delay] @string[script]
		 */
		public static void ReadStringAfterDelay(ProseRuntime runtime, List<ProseObject> args)
		{
			lock(lockObj) {
				if (args.Count != 2)
					throw new ArgumentException("ReadAfterDelay takes two string arguments: delay, script.");
				foreach (ProseObject arg in args)
					if (!(arg is StringLiteralObject))
						throw new ArgumentException("ReadAfterDelay takes only string arguments.");

				//	Parse the args for the delay and the script
				double seconds = Double.Parse(((StringLiteralObject) args[0]).literal);
				RuntimeRunnable script = new ProseStringReader(((StringLiteralObject) args[1]).literal);

				//	Configure the timer
				Timer timer = new Timer(seconds * 1000);
				timer.AutoReset = false;
				timer.Elapsed += new ElapsedEventHandler(onTimeElapsed);

//				timer.Elapsed += delegate(object sender, ElapsedEventArgs e)
//					{
//						lock(lockObj) {
//							//	Look up the timer fired
//							int idx = timerList.BinarySearch((Timer) sender);
//						Console.WriteLine(idx);
//							//	Run the script
//							//runtime.read(scriptList[idx], runtime.GlobalClient);
//							runtimeList[idx].read("write \"Hello Carly\"", runtimeList[idx].GlobalClient);
//						//	Remove the timer and the script from the lists
//							timerList.RemoveAt(idx);
//							scriptList.RemoveAt(idx);
//							runtimeList.RemoveAt(idx);
//						}
//					};
				timer.Enabled = true;

				//	Add the timer and the script to the list
				timerList.Add(timer);
				scriptList.Add(script);
				runtimeList.Add(runtime);
			}
		}

		public static void ReadProseAfterDelay(ProseRuntime runtime, List<ProseObject> args)
		{
			lock(lockObj) {
				if (args.Count < 2)
					throw new ArgumentException("ReadProseAfterDelay takes two arguments: @string[delay] @prose[script].");

				//	Parse the args for the delay and the script
				double seconds = Double.Parse(((StringLiteralObject) args[0]).literal);
				RuntimeRunnable script = new ProseObjectListReader(args.GetRange(1, args.Count - 1));

				//	Configure the timer
				Timer timer = new Timer(seconds * 1000);
				timer.AutoReset = false;
				timer.Elapsed += new ElapsedEventHandler(onTimeElapsed);

				timer.Enabled = true;
				
				//	Add the timer and the script to the list
				timerList.Add(timer);
				scriptList.Add(script);
				runtimeList.Add(runtime);
			}
		}

		private static void onTimeElapsed (object sender, ElapsedEventArgs e)
		{
			lock(lockObj) {
				//	Look up the timer fired
				int idx = timerList.IndexOf((Timer) sender);
				//	Run the script
				//runtimeList[idx].read(scriptList[idx], runtimeList[idx].GlobalClient);
				scriptList[idx].run(runtimeList[idx]);
				//	Remove the timer and the script from the lists
				timerList.RemoveAt(idx);
				scriptList.RemoveAt(idx);
				runtimeList.RemoveAt(idx);
			}
		}

//		public static void DestroyAllTimers(ProseRuntime runtime, List<ProseObject> args);
//		public static void CreateTimer(ProseRuntime runtime, List<ProseObject> args);



		private class ProseStringReader : RuntimeRunnable
		{
			string script;

			public ProseStringReader(string script)
			{
				this.script = script;
			}

			public void run(ProseRuntime runtime) {
				runtime.read (script, runtime.GlobalClient);
			}
		}

		private class ProseObjectListReader : RuntimeRunnable
		{
			List<ProseObject> script;

			public ProseObjectListReader(List<ProseObject> script)
			{
				this.script = script;
			}
			
			public void run(ProseRuntime runtime) {
				runtime.read(script, 0, script.Count, runtime.GlobalClient);
			}
		}


	}


}

