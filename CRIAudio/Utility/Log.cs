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
		}
		private static StreamWriter logWriter;

		public static void WriteLine(string s) => logWriter.WriteLine(s);
		public static void WriteLine() => logWriter.WriteLine();
	}
}
