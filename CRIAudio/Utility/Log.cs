using System;
using System.IO;

namespace CRIAudio.Utility
{
    public static class Log
	{
		static Log() {
			logWriter = new StreamWriter(new FileStream(@".\debug.log",FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 2 ^ 18));
			Visible = false;
		}
		private static StreamWriter logWriter;
		public static bool Visible { get; set; }

		public static void WriteLine(string s) { 
			if (Visible)
			{
				Console.WriteLine(s);
			}
			logWriter.WriteLine(s); 
			logWriter.Flush();
		}
		public static void WriteLine() {
			if (Visible)
			{
				Console.WriteLine();
			}
			logWriter.WriteLine();
            logWriter.Flush();
        }
	}
}
