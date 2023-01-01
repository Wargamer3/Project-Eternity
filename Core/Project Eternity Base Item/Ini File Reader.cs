using System;
using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public class IniFileReader
    {
        private FileStream IniFS;
        private StreamReader InitSR;

        public bool CanRead => IniFS.CanSeek && !InitSR.EndOfStream;

        public IniFileReader(string Path)
        {
            IniFS = new FileStream(Path, FileMode.Open, FileAccess.Read);
            InitSR = new StreamReader(IniFS);
        }

        public Dictionary<string, string> ReadAllHeaders()
        {
            Dictionary<string, string> DicFieldAndValue = new Dictionary<string, string>();
            string CurrentHeader = null;
            while (CanRead)
            {
                string ActiveLine = InitSR.ReadLine();

                if (CurrentHeader == null && ActiveLine.StartsWith("[") && ActiveLine.EndsWith("]"))
                {
                    CurrentHeader = ActiveLine.Remove(ActiveLine.Length - 1, 1).Substring(1);
                }
                else if (string.IsNullOrWhiteSpace(ActiveLine))
                {
                    return DicFieldAndValue;
                }
                else
                {
                    int IndexOfEquals = ActiveLine.IndexOf("=");
                    string Key = ActiveLine.Substring(0, IndexOfEquals);
                    string Value = ActiveLine.Substring(IndexOfEquals + 1);
                    DicFieldAndValue.Add(Key, Value);
                }
            }

            InitSR.Close();
            IniFS.Close();

            return DicFieldAndValue;
        }
    }
}
