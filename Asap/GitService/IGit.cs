namespace GitService
{
    public interface ISourceControl
    {
        string UserName { get; set; }
        string Password { get; set; }
        string Email { get; set; }
        string RepositoryPath { get; set; }

        ECommandStatus Pull();
        ECommandStatus Push(string remote = "", string branchName = "");
        ECommandStatus Branch(string branchName, bool delete = false, bool isRemote = false);
        ECommandStatus Checkout(string branchName);
        ECommandStatus Add(string fileName, bool all = false);
        ECommandStatus Commit(string message, bool allowEmpty = false);
        ECommandStatus Merge(string branchName);
        string GetCurrentBranch();
        string GetLastCommit();
    }
}
      