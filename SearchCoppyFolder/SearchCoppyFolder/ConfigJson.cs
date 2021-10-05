using System.Text.Json;

namespace SearchCoppyFolder
{
    public partial class ConfigJson
    {
        public string FolderSearch { get; set; }
        public string FolderCoppy { get; set; }
        public string FileCoppy { get; set; }
        public string PathSaveFileNotFound { get; set; }
        // Thư mục sau khi hoàn thành đọc tên file thì lưu vào
        public string PathSaveFileWrite { get; set; }
        // Thư mục chứa các tên file cần đọc tên file
        public string PathFileWrite { get; set; }
        // Thư mục cần coppy
        public string PathFolderCoppy { get; set; }
        // Thư mục chứa file coppy
        public string PathSaveFolderCoppy { get; set; }
    }

    public partial class ConfigJson
    {
        public static ConfigJson FromJson(string json) => JsonSerializer.Deserialize<ConfigJson>(json);
    }
}
