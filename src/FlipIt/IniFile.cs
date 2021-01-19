// Initially from https://stackoverflow.com/a/14906422/1899

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ScreenSaver
{
    public class IniFile 
    {
        private const int BufferSize = 1024;
        private readonly string _path;
        
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string section, string key, string @default, StringBuilder retVal, int size, string filePath);

        public IniFile(string iniPath)
        {
            _path = iniPath;
        }

        public string ReadString(string section, string key)
        {
            var retVal = new StringBuilder(BufferSize);
            GetPrivateProfileString(section, key, "", retVal, BufferSize, _path);
            return retVal.ToString();
        }

        public void WriteBool(string section, string key, bool value)
        {
            WriteString(section, key, value ? "1": "0");
        }

        public bool ReadBool(string section, string key, bool defaultValue)
        {
            var val = ReadString(section, key);
            if (val == "")
                return defaultValue;
            Debug.Assert(val == "1" || val == "0");
            return val == "1";
        }
        
        public int ReadInt(string section, string key, int defaultValue)
        {
            var val = ReadString(section, key);
            if (val == "")
                return defaultValue;
            return Int32.Parse(val);
        }

        public void WriteInt(string section, string key, int value)
        {
            WriteString(section, key, value.ToString());
        }

        public string[] ReadSections()
        {
            var keyNames = ReadString(null, null);
            return keyNames.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public bool SectionExists(string section)
        {
            var keyNames = ReadSection(section);
            return keyNames.Length > 0;
        }

        public string[] ReadSection(string section)
        {
            var keyNames = ReadString(section, null);
            return keyNames.Split(new []{'\n'}, StringSplitOptions.RemoveEmptyEntries);
        }

        public void WriteString(string section, string key, string value)
        {
            var res = WritePrivateProfileString(section, key, value, _path);
        }

        public void DeleteKey(string section, string key)
        {
            WriteString(section, key, null);
        }

        public void DeleteSection(string section = null)
        {
            WriteString(section, null, null);
        }

        public bool KeyExists(string key, string section = null)
        {
            return ReadString(section, key).Length > 0;
        }
    }
}
