using System;
using System.IO;
using System.Collections.Generic;

namespace TwitchDeathCount
{
    static class IO
    {
        public static void UpdateMainVariableFile(List<Tuple<string, int>> Variables)
        {
            string Output = "";
            foreach (Tuple<string, int> Var in Variables)
                Output += Var.Item1 + ": " + Var.Item2 + "\n";
            File.WriteAllText("Stream Files\\AllCounters.txt", Output);
        }

    }
}
