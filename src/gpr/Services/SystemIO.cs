namespace GitPullRequest.Services;

class SystemIO : IO
{
    public void CreateDirectory(string directory) => Directory.CreateDirectory(directory);

    public string GetCurrentDirectory() => Directory.GetCurrentDirectory();

    public TextWriter OpenWrite(string file) => new StreamWriter(file);

    public TextReader OpenRead(string configFile) => File.OpenText(configFile);

    public bool FileExists(string file) => File.Exists(file);

    public void FileDelete(string file) => File.Delete(file);

    public void FileCopy(string source, string target) => File.Copy(source, target);
}
