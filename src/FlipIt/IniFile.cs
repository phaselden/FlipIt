using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScreenSaver
{
    public class IniFile
    {
        private readonly string _iniFilePath;
        private readonly List<Entry> _entries = new List<Entry>();
        
        private class Entry
        {
            public Entry(string section, string key, string value)
            {
                Section = section;
                Key = key;
                Value = value;
            }

            public string Section { get; }
            public string Key { get; }
            public string Value { get; set; }
        }

        public IniFile(string iniPath)
        {
            _iniFilePath = iniPath;

            string currentSection = null;

            var lines = File.ReadAllLines(iniPath);
            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (line == "")
                    continue;
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line.Substring(1, line.Length - 2);
                }
                else
                {
                    var keyPair = line.Split(new char[] {'='}, 2);
                    if (keyPair.Length < 2)
                    {
                        // it could be a comment
                        continue;
                    }

                    if (currentSection == null)
                    {
                        currentSection = "ROOT";
                    }

                    var entry = new Entry(currentSection, keyPair[0], keyPair[1]);
                    _entries.Add(entry);
                }
            }
        }

        public bool SectionExists(string section)
        {
            return _entries.Exists(e => e.Section == section);
        }

        public string[] GetKeys(string section)
        {
            return _entries.Where(e => e.Section == section).Select(e => e.Key).ToArray();
        }

        public void DeleteSection(string section)
        {
            _entries.RemoveAll(e => e.Section == section);
        }

        public string GetString(string section, string key)
        {
            return GetEntry(section, key)?.Value;
        }

        public int GetInt(string section, string key, int defaultValue)
        {
            var value = GetString(section, key);
            return value != null ? Convert.ToInt32(value) : defaultValue ;
        }

        public bool GetBool(string section, string key, bool defaultValue)
        {
            var value = GetInt(section, key, defaultValue ? 1 : 0);
            return value == 1;
        }

        public void SetString(string section, string key, string value)
        {
            var entry = GetEntry(section, key);
            if (entry != null)
            {
                entry.Value = value;
            }
            else
            {
                _entries.Add(new Entry(section, key, value));
            }
        }

        public void SetBool(string section, string key, bool value)
        {
            SetString(section, key, value ? "1" : "0");
        }

        public void SetInt(string section, string key, int value)
        {
            SetString(section, key, value.ToString());
        }

        private Entry GetEntry(string section, string key)
        {
            return _entries.SingleOrDefault(e => e.Section == section && e.Key == key);
        }

        public void DeleteKey(String section, String key)
        {
            _entries.RemoveAll(e => e.Section == section && e.Key == key);
        }

        public void Save(string filePath)
        {
            var sb = new StringBuilder();
            var sectionNames = _entries.Select(e => e.Section).Distinct();
            foreach (var section in sectionNames)
            {
                sb.AppendLine($"[{section}]");

                var sectionEntries = _entries.Where(e => e.Section == section);
                foreach (var entry in sectionEntries)
                {
                    if (entry.Value == null)
                        continue;
                    
                    sb.AppendLine($"{entry.Key}={entry.Value}");
                }
                sb.AppendLine();
            }
            File.WriteAllText(filePath, sb.ToString());
        }

        public void Save()
        {
            Save(_iniFilePath);
        }
    }
}