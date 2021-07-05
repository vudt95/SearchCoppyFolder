using System.Text.Json;

namespace SearchCoppyFolder
{
    public partial class ConfigJson
    {
        public string FolderSearch { get; set; }
        public string FolderCoppy { get; set; }
        public string FileCoppy { get; set; }
        public string PathSaveFileNotFound { get; set; }
    }

    public partial class ConfigJson
    {
        public static ConfigJson FromJson(string json) => JsonSerializer.Deserialize<ConfigJson>(json);
    }
}
