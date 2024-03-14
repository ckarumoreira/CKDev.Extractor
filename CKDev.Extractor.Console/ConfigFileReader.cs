using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CKDev.Extractor.Console {
    public class ConfigFileReader {
        private readonly string _directory;
        private readonly string _filename;
        private readonly HashSet<string> _fields;

        public ConfigFileReader(string directory, string filename, IEnumerable<string> fields) {
            _directory = directory;
            _filename = filename;
            _fields = new HashSet<string>(fields, StringComparer.Ordinal);
        }

        public IDictionary<string, string> Read() {
            var configStream = File.Open(Path.Combine(_directory, _filename), FileMode.Open);
            var streamReader = new StreamReader(configStream);
            var allConfigs = streamReader.ReadToEnd();
            var configLines = allConfigs
                .Replace("\r", string.Empty)
                .Replace("\t", string.Empty)
                .Split("\n");

            var dict = new Dictionary<string, string>(StringComparer.Ordinal);

            foreach (var line in configLines) {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var pair = line.Split("=");
                if (pair.Length is 2) {
                    string key = pair[0];

                    if (_fields.Contains(key) && !dict.ContainsKey(key)) {
                        dict.TryAdd(key, pair[1]);
                    }
                }
            }

            return dict;
        }
    }
}
