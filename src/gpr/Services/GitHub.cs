namespace GitPullRequest.Services;

public interface IGitHub
{
    Task<string?> GetTokenAsync();
}

public class GitHub : IGitHub
{
    async Task<string> Exec(string cmd, string args)
    {
        var proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = cmd;
        proc.StartInfo.Arguments = args;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();
        var result = await proc.StandardOutput.ReadToEndAsync();
        await proc.WaitForExitAsync();
        return result;
    }

    public async Task<string?> GetTokenAsync()
    {
        var token = (await Exec("gh", "auth token")).Trim();

        return token == string.Empty ? null : token;
    }
}
