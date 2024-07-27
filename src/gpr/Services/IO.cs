namespace GitPullRequest.Services;

public interface IO
{
    void CreateDirectory(string directory);
    string GetCurrentDirectory();

    TextWriter OpenWrite(string file);
    TextReader OpenRead(string configFile);
    bool FileExists(string file);
    void FileDelete(string file);
    void FileCopy(string source, string target);
}
