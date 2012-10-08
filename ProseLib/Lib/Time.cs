using System;
using ProseLanguage;
using System.Collections.Generic;
using System.Timers;

namespace ProseLib
{
	public class Time
	{
		public static List<Timer> timerList = new List<Timer>();
		public static List<string> scriptList = new List<string>();
		public static List<ProseRuntime> runtimeList = new List<ProseRuntime>();

		private static Object lockObj = new Object();

		/*
		 * 		Expects all arguments to be strings.
		 * 		
		 * 		@string[delay] @string[script]
		 */
		public static void ReadAfterDelay(ProseRuntime runtime, List<ProseObject> args)
		{
			lock(lockObj) {
				if (args.Count != 2)
					throw new ArgumentException("ReadAfterDelay takes two string arguments: delay, script.");
				foreach (ProseObject arg in args)
					if (!(arg is StringLiteralObject))
						throw new ArgumentException("ReadAfterDelay takes only string arguments.");

				//	Parse the args for the delay and the script
				double seconds = Double.Parse(((StringLiteralObject) args[0]).literal);
				string script = ((StringLiteralObject) args[1]).literal;

				Console.WriteLine("--" + seconds + "--" + script);

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


		private static void onTimeElapsed (object sender, ElapsedEventArgs e)
		{
			lock(lockObj) {
				//	Look up the timer fired
				int idx = timerList.IndexOf((Timer) sender);
				Console.WriteLine(idx);
				//	Run the script
			runtimeList[idx].read(scriptList[idx], runtimeList[idx].GlobalClient);
				//runtimeList[idx].read("write \"Hello Carly\"", runtimeList[idx].GlobalClient);
				//	Remove the timer and the script from the lists
				timerList.RemoveAt(idx);
				scriptList.RemoveAt(idx);
				runtimeList.RemoveAt(idx);
			}
		}

//		public static void DestroyAllTimers(ProseRuntime runtime, List<ProseObject> args);
//		public static void CreateTimer(ProseRuntime runtime, List<ProseObject> args);
	}


}

