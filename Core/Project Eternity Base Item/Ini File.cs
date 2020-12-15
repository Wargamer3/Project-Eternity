using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProjectEternity.Core.Item
{
    public class IniFile
    {
        private Dictionary<string, Dictionary<string, string>> DicFieldValueByHeader;

        public IniFile()
        {
            DicFieldValueByHeader = new Dictionary<string, Dictionary<string, string>>();
        }

        private IniFile(string[] Content)
            : this()
        {
            string CurrentHeader = null;
            foreach (string ActiveLine in Content)
            {
                if (CurrentHeader == null && ActiveLine.StartsWith("[") && ActiveLine.EndsWith("]"))
                {
                    CurrentHeader = ActiveLine.Remove(ActiveLine.Length - 1, 1).Substring(1);

                    DicFieldValueByHeader.Add(CurrentHeader, new Dictionary<string, string>());
                }
                else if (string.IsNullOrWhiteSpace(ActiveLine))
                {
                    CurrentHeader = null;
                }
                else
                {
                    int IndexOfEquals = ActiveLine.IndexOf("=");
                    string Key = ActiveLine.Substring(0, IndexOfEquals);
                    string Value = ActiveLine.Substring(IndexOfEquals + 1);
                    DicFieldValueByHeader[CurrentHeader].Add(Key, Value);
                }
            }
        }

        public static IniFile ReadFromFile(string FilePath)
        {
            return new IniFile(File.ReadAllLines(FilePath, Encoding.UTF8));
        }

        public string ReadField(string Header, string Field)
        {
            return DicFieldValueByHeader[Header][Field];
        }

        public List<string> ReadAllValues(string Header)
        {
            return new List<string>(DicFieldValueByHeader[Header].Values);
        }

        public Dictionary<string, string> ReadHeader(string Header)
        {
            if (DicFieldValueByHeader.ContainsKey(Header))
            {
                return DicFieldValueByHeader[Header];
            }
            else
            {
                return new Dictionary<string, string>();
            }
        }

        public void AddValue(string Header, string Field, string Value)
        {
            if (!DicFieldValueByHeader.ContainsKey(Header))
            {
                DicFieldValueByHeader.Add(Header, new Dictionary<string, string>());
            }

            DicFieldValueByHeader[Header].Add(Field, Value);
        }

        public void SaveToFile(string FilePath)
        {
            FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            StreamWriter SW = new StreamWriter(FS, Encoding.UTF8);

            foreach (KeyValuePair<string, Dictionary<string, string>> ActiveHeader in DicFieldValueByHeader)
            {
                SW.WriteLine("[" + ActiveHeader.Key + "]");
                foreach (KeyValuePair<string, string> ActiveField in ActiveHeader.Value)
                {
                    if (string.IsNullOrEmpty(ActiveField.Value))
                    {
                        SW.WriteLine(ActiveField.Key);
                    }
                    else
                    {
                        SW.WriteLine(ActiveField.Key + "=" + ActiveField.Value);
                    }
                }
                SW.WriteLine();
            }

            SW.Close();
            FS.Close();
        }
    }
}
