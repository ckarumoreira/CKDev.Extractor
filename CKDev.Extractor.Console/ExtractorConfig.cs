public class ExtractorConfig {
    public class Config<T> {
        public Config(string key, T defaultValue) {
            Key = key;
            Value = defaultValue;
        }

        public string Key { get; }
        public T Value { get; set; }
    }

    public ExtractorConfig(string dir) {
        OutputDir = new Config<string>("output_dir", dir);
        OutputFileName = new Config<string>("output_file", "output");
        InputBulkPath = new Config<string>("input_file", $"{dir}\\all-cards.json");
        SaveTimestamp = new Config<bool>("add_timestamp", true);
    }

    public Config<string> OutputDir { get; }
    public Config<string> OutputFileName { get; }
    public Config<string> InputBulkPath { get; }
    public Config<bool> SaveTimestamp { get; }

    public IEnumerable<string> GetKeys() {
        return new[] {
            OutputDir.Key,
            OutputFileName.Key,
            InputBulkPath.Key,
            SaveTimestamp.Key
        };
    }

    public void Load(IDictionary<string, string> values) {
        if (values.ContainsKey(OutputDir.Key)) {
            OutputDir.Value = values[OutputDir.Key];
        }

        if (values.ContainsKey(OutputFileName.Key)) {
            OutputFileName.Value = values[OutputFileName.Key];
        }

        if (values.ContainsKey(InputBulkPath.Key)) {
            InputBulkPath.Value = values[InputBulkPath.Key];
        }

        if (values.ContainsKey(SaveTimestamp.Key)) {
            SaveTimestamp.Value = StringComparer.OrdinalIgnoreCase.Compare(values[SaveTimestamp.Key], "true") == 0;
        }
    }
}
