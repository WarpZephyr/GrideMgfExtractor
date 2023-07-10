using SoulsFormats.Dreamcast;

namespace GrideMgfExtractor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Message("This program extracts and repacks Frame Gride MGF archives.\n" +
                    "If a txt file from the game is in the same folder, it can add names to the files.\n" +
                    "Txt files will automatically be generated with local paths when repacking.\n\n" +
                    "The program is not meant to be opened this way,\n" +
                    "Drag and drop the file onto the program, or associate the extension with the program.");
                return;
            }

            foreach (string arg in args)
            {
                try
                {
                    if (Directory.Exists(arg))
                        Repack(arg);
                    else if (File.Exists(arg) && MGF.Is(arg))
                        Unpack(arg);
                }
                catch (Exception ex)
                {
                    Message($"An error has occurred.\n Exception: {ex.Message}\nStacktrace:\n {ex.StackTrace}");
                }
            }
        }

        #region Unpack

        static void Unpack(string path)
        {
            string? dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir))
                return;

            string outDir = Path.Combine(dir, $"{Path.GetFileNameWithoutExtension(path)}-mgf");
            Directory.CreateDirectory(outDir);
            string[] names = UnpackNames(path);

            var mgf = MGF.Read(path);
            if (mgf.Files.Count == names.Length)
                UnpackNamed(outDir, mgf, names);
            else
                UnpackUnnamed(outDir, mgf);
        }

        static void UnpackNamed(string outDir, MGF mgf, string[] names)
        {
            if (mgf.Files.Count != names.Length)
                return;

            for (int i = 0; i < mgf.Files.Count; i++)
            {
                
                string outPath = Path.Combine(outDir, names[i]);
                string? dir = Path.GetDirectoryName(outPath);
                if (string.IsNullOrEmpty(dir))
                    throw new Exception("Directory name of output path was null or empty.");

                Directory.CreateDirectory(dir);
                File.WriteAllBytes(outPath, mgf.Files[i].Bytes);
            }
        }

        static void UnpackUnnamed(string outDir, MGF mgf)
        {
            for (int i = 0; i < mgf.Files.Count; i++)
                File.WriteAllBytes(Path.Combine(outDir, $"{i}"), mgf.Files[i].Bytes);
        }

        static string[] UnpackNames(string path)
        {
            string? dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir))
                return Array.Empty<string>();

            string txtpath = Path.Combine(dir, $"{Path.GetFileNameWithoutExtension(path)}.txt");
            if (File.Exists(txtpath))
            {
                var lines = new List<string>();
                lines.AddRange(File.ReadAllLines(txtpath));
                lines.RemoveAll(a => a == string.Empty || a == "\u001a");
                for (int i = 0; i < lines.Count; i++)
                {
                    lines[i] = ValidatePathString(RemoveComments(lines[i].Between("\"", "\""))).ToLower();
                }
                return lines.ToArray();
            }
            else
            {
                return Array.Empty<string>();
            }
        }

        #endregion

        #region Repack

        static void Repack(string dir)
        {
            string outName = GetRepackOutName(dir);
            string? parentdir = Path.GetDirectoryName(dir);
            if (string.IsNullOrEmpty(parentdir))
                return;

            string outPath = Path.Combine(parentdir, outName);
            if (string.IsNullOrEmpty(outPath))
                return;

            var mgf = new MGF();
            mgf.Files = new List<MGF.File>();

            var names = new List<string>();
            string[] paths = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            foreach (string path in paths)
            {
                names.Add($"\"{path.Remove(0, dir.Length)}\"");
                mgf.Files.Add(new MGF.File(File.ReadAllBytes(path)));
            }

            Backup(outPath);
            mgf.Write(outPath);

            string txtOutName = Path.GetFileNameWithoutExtension(outName);
            string txtOutPath = Path.Combine(parentdir, $"{txtOutName}.txt");
            Backup(txtOutPath);
            File.WriteAllLines(txtOutPath, names);
        }

        #endregion

        #region Pathing

        static void Backup(string path)
        {
            if (File.Exists(path) && !File.Exists($"{path}.bak"))
                File.Move(path, $"{path}.bak");
            else if (Directory.Exists(path) && !Directory.Exists($"{path}-bak"))
                Directory.Move(path, $"{path}-bak");
        }

        static string RemoveComments(string str)
        {
            string newStr = new string(str);
            if (newStr.Contains("//"))
                newStr = newStr.Substring(0, "//").Trim();
            return newStr;
        }

        static string ValidatePathString(string str)
        {
            string newStr = new string(str);
            if (newStr.Contains(':'))
                newStr = newStr.Substring(newStr.IndexOf(':') + 1);
            newStr = newStr.Replace("\"", "");
            newStr = newStr.Trim();
            if (newStr.StartsWith("\\") || newStr.StartsWith("/"))
                newStr = newStr.Substring(1);
            return newStr;
        }

        static string GetRepackOutName(string dir)
        {
            if (dir.Contains('-'))
                return dir.Replace("-", ".");
            else
                return $"{dir}.mgf";
        }

        #endregion

        #region Console

        static void Message(string text)
        {
            Console.WriteLine(text);
            Pause();
        }

        static void Pause() => Console.Read();

        #endregion
    }
}