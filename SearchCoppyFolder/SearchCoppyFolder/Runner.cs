
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SearchCoppyFolder
{
    public class Runner
    {
        private readonly ILogger<Runner> _logger;

        public Runner(ILogger<Runner> logger)
        {
            _logger = logger;
        }

        private ConfigJson _config;
        private List<string> FolderNotFounds;
        public async void DoAction(string name, bool isCoppy = false)
        {
            _config = ConfigExtentions.GetConfig();
            Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Read All folder and subfolder | {_config.FolderSearch}...");
            var dirs = Directory.GetDirectories(_config.FolderSearch, "*", SearchOption.AllDirectories);
            if (!dirs.Any())
            {
                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | WARNING | Folder search empty {_config.FolderSearch}...");
                return;
            }
            Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Read all folder name need coppy | {_config.FileCoppy}...");
            if (!File.Exists(_config.FileCoppy))
            {
                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | WARNING | Not found file {_config.FileCoppy}...");
                return;
            }
            var folderNeedSearch = File.ReadAllLines(_config.FileCoppy);
            Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Start coppy...");
            FolderNotFounds = new List<string>();
            Parallel.ForEach(folderNeedSearch, (s, state) =>
            {
                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Search: {s.Trim()}");
                var pathFolder = dirs.FirstOrDefault(x => x.Contains(s.Trim()));
                if (pathFolder != null)
                {
                    if (isCoppy)
                        DirectoryCopy(pathFolder, $"{_config.FolderCoppy}/{s}", true);
                }
                else
                {
                    FolderNotFounds.Add(s);
                }
            });
            if (FolderNotFounds.Any())
                await File.WriteAllLinesAsync($"{_config.PathSaveFileNotFound}/FolderNotFound-{DateTime.Now:ddMMyyyy}.txt", FolderNotFounds);
            _logger.LogDebug(20, $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | SUCCESS | {name}");
        }

        public bool SearchFolder(string[] paths, string searchName)
        {
            var folder = paths.FirstOrDefault(x => x.Equals(searchName));
            if (string.IsNullOrEmpty(folder))
                return paths.Select(path => Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
                    .Any(dirs => SearchFolder(dirs, searchName));
            DirectoryCopy(folder, _config.FolderCoppy, true);
            return true;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (Directory.Exists(destDirName))
            {
                return;
            }
            else
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    if (Directory.Exists(tempPath))
                        DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}
