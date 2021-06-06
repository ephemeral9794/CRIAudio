using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRIAudio.Utility
{
	public static class Log
	{
		static Log() {
			logWriter = new StreamWriter(new FileStream(@".\debug.log",FileMode.OpenOrCreate, FileAccess.Write));
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
		}
		public static void WriteLine() {
			if (Visible)
			{
				Console.WriteLine();
			}
			logWriter.WriteLine();
		}
	}
}
