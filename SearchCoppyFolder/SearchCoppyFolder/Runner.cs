
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
        private List<string> _folderNotFounds;

        #region Ver1
        /// <summary>
        /// Thực hiện công việc tìm và lưu thư mục khác
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isCoppy"></param>
        public async void DoAction(string name, bool isCoppy = false)
        {
            _config = ConfigExtensions.GetConfig();
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
            var folderNeedSearch = await File.ReadAllLinesAsync(_config.FileCoppy);
            Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Start {(isCoppy ? "coppy" : "read")}...");
            _folderNotFounds = new List<string>();
            Parallel.ForEach(folderNeedSearch, (s, _) =>
            {
                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Search: {s.Trim()}");
                try
                {
                    var pathFolder = dirs.FirstOrDefault(x => x.Contains(s.Trim()));
                    if (pathFolder != null)
                    {
                        if (isCoppy)
                            DirectoryCopy(pathFolder, $"{_config.FolderCoppy}/{s}", true);
                    }
                    else
                    {
                        _folderNotFounds.Add(s);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            });
            if (_folderNotFounds.Any())
                await File.WriteAllLinesAsync($"{_config.PathSaveFileNotFound}/FolderNotFound-{DateTime.Now:ddMMyyyy}.txt", _folderNotFounds);
            _logger.LogDebug(20, $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | SUCCESS | {name}");
            Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | SUCCESS | {name}");
        }

        /// <summary>
        /// Tìm file cần coppy
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="searchName"></param>
        /// <returns></returns>
        public bool SearchFolder(string[] paths, string searchName)
        {
            var folder = paths.FirstOrDefault(x => x.Equals(searchName));
            if (string.IsNullOrEmpty(folder))
                return paths.Select(path => Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
                    .Any(dirs => SearchFolder(dirs, searchName));
            DirectoryCopy(folder, _config.FolderCoppy, true);
            return true;
        }
        /// <summary>
        /// Thực hiện coppy thư mục
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
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
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (!copySubDirs) return;
            {
                foreach (var subdir in dirs)
                {
                    var tempPath = Path.Combine(destDirName, subdir.Name);
                    if (Directory.Exists(tempPath))
                        DirectoryCopy(subdir.FullName, tempPath, true);
                }
            }
        }
        #endregion

        public async void ReadNameAllFiles(string name)
        {
            _config = ConfigExtensions.GetConfig();
            if (_config == null)
            {
                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | ERROR | Can't read file config");
                return;
            }
            Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Read all name file | {_config.PathFileWrite}...");
            try
            {
                var files = Directory.GetFiles(_config.PathFileWrite, "*", SearchOption.AllDirectories);
                if (!Directory.Exists(_config.PathSaveFileWrite))
                {
                    Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Create folder | {_config.PathSaveFileWrite}...");
                    Directory.CreateDirectory(_config.PathSaveFileWrite);
                }

                if (files.Any())
                {
                    await File.WriteAllLinesAsync($"{_config.PathSaveFileWrite}/FileNameWrite-{DateTime.Now:ddMMyyyyhhmmss}.txt", files.Select(x => x[(x.LastIndexOf('\\') + 1)..]));
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | WARNING | not found");
                }
                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | SUCCESS | {name}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.LogError(e, "ReadNameAllFiles");
            }
        }

        public void CoppyAllFile(string name)
        {
            _config = ConfigExtensions.GetConfig();
            if (_config == null)
            {
                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | ERROR | Can't read file config");
                return;
            }
            Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Read all name file | {_config.PathFolderCoppy}...");
            try
            {
                if (!Directory.Exists(_config.PathSaveFolderCoppy))
                {
                    Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Create folder | {_config.PathSaveFolderCoppy}...");
                    Directory.CreateDirectory(_config.PathSaveFolderCoppy);
                }
                CopyAllFile(_config.PathFolderCoppy, _config.PathSaveFolderCoppy);
                Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | SUCCESS | {name}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.LogError(e, "CoppyAllFile");
            }
        }

        private void CopyAllFile(string sourceDir, string targetDir)
        {
            Parallel.ForEach(Directory.GetFiles(sourceDir), (s, _) =>
            {
                try
                {
                    Console.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | Coppy | {s}...");
                    File.Copy(s, Path.Combine(targetDir, Path.GetFileName(s)), true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _logger.LogError(e, "CopyAllFile");
                }

            });
            Parallel.ForEach(Directory.GetDirectories(sourceDir), (s, _) =>
            {
                try
                {
                    CopyAllFile(s, targetDir);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _logger.LogError(e, "CopyAllFile");
                }
            });
        }
    }
}
