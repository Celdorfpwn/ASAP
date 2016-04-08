using System;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using SourceControl;
using ToolsConfiguration;

namespace GitService
{
    /// <summary>
    /// Git commands utility class.
    /// </summary>
    public class Git : ISourceControl
    {

        #region Private Variables

        private ISourceControlConfig Config { get; set; }

        #endregion

        #region Public Properties

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods


        /// <summary>
        /// Sets the user details 
        /// </summary>
        /// <param name="user">The user details</param>
        public Git(ISourceControlConfig config)
        {
            Config = config;
        }

        /// <summary>
        /// Git pull command
        /// </summary>
        /// <returns>CommandStatus</returns>
        public ECommandStatus Pull()
        {
            return Pull(Config.Path);
        }

        /// <summary>
        /// Git push command.
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public ECommandStatus Push(string remote = "",  string branchName = "")
        {
            return Push(Config.Path, remote, branchName);
        }

        /// <summary>
        /// Creates a new branch with branchName.
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public ECommandStatus Branch(string branchName, bool delete = false, bool isRemote = false)
        {
            return Branch(Config.Path, branchName, delete, isRemote);
        }

        /// <summary>
        /// Checks out branch branchName.
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public ECommandStatus Checkout(string branchName)
        {
            return Checkout(Config.Path, branchName);
        }

        /// <summary>
        /// Stages all files.
        /// </summary>
        /// <returns>CommandStatus</returns>
        public ECommandStatus Add(string fileName, bool all = false)
        {
            return Add(Config.Path, fileName, all);
        }

        /// <summary>
        /// Commits all staged files.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="allowEmptyCommit"></param>
        /// <returns>CommandStatus</returns>
        public ECommandStatus Commit(string message, bool allowEmptyCommit = false)
        {
            
            return Commit(Config.Path, message, allowEmptyCommit);
        }

        /// <summary>
        /// Merges branchName to current Branch.
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public ECommandStatus Merge(string branchName)
        {
            return Merge(Config.Path, branchName);
        }

        /// <summary>
        /// Get the current branch name.
        /// </summary>
        /// <returns>Branch name</returns>
        public string GetCurrentBranch()
        {
            return GetCurrentBranch(Config.Path);
        }

        /// <summary>
        /// Gets last commit message from current branch.
        /// </summary>
        /// <returns>Commit message</returns>
        public string GetLastCommit()
        {
            return GetLastCommit(Config.Path);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Git pull command
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <returns>CommandStatus</returns>
        private ECommandStatus Pull(string repositoryPath)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    PullOptions options = new PullOptions();
                    options.FetchOptions = new FetchOptions();
                    options.FetchOptions.CredentialsProvider = new CredentialsHandler(
                        (url, usernameFromUrl, types) => new UsernamePasswordCredentials()
                    {
                        Username = Config.Username,
                        Password = Config.Password
                    });

                    MergeResult result = repo.Network.Pull(new Signature(Config.Username, Config.Email, new DateTimeOffset(DateTime.Now)), options);
                    return (ECommandStatus)result.Status;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return ECommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Git push command.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        private ECommandStatus Push(string repositoryPath, string origin, string branchName)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    Branch localBranch;                 
                    if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(branchName))
                    {
                        localBranch = repo.Head;
                    }
                    else
                    {
                        localBranch = repo.Branches[branchName];
                        var remote = repo.Network.Remotes[origin];
                        if (localBranch !=null)
                        {
                            repo.Branches.Update(localBranch, b => b.Remote = remote.Name, b => b.UpstreamBranch = localBranch.CanonicalName);
                        }
                    }

                    PushOptions options = new PushOptions();
                    options.CredentialsProvider = new CredentialsHandler(
                        (url, usernameFromUrl, types) => new UsernamePasswordCredentials()
                    {
                        Username = Config.Username,
                        Password = Config.Password
                    });

                    if (localBranch != null)
                    {
                        repo.Network.Push(localBranch, options);
                        return ECommandStatus.OK;
                    }
                    else
                    {
                        return ECommandStatus.BranchNotFound;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return ECommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Creates a new branch with branchName.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        private ECommandStatus Branch(string repositoryPath, string branchName, bool delete, bool isRemote)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    var localBranch = repo.Branches[branchName];
                    if (localBranch == null)
                    {
                        if (delete)
                        {
                            return ECommandStatus.BranchNotFound;
                        }
                        else
                        {
                            Branch branch = repo.CreateBranch(branchName);
                            return ECommandStatus.OK;
                        }
                    }
                    else
                    {
                        if (delete)
                        {
                            repo.Branches.Remove(branchName, isRemote);
                            return ECommandStatus.OK;
                        }
                        else
                        {
                            return ECommandStatus.BranchAlreadyExists;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return ECommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Checks out branch branchName.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        private ECommandStatus Checkout(string repositoryPath, string branchName)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    var localBranch = repo.Branches[branchName];
                    if (localBranch != null)
                    {                                               
                        Branch branch = repo.Checkout(branchName);
                        return ECommandStatus.OK;
                    }
                    else
                    {
                        return ECommandStatus.BranchNotFound;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return ECommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Stages all files.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <returns>CommandStatus</returns>
        private ECommandStatus Add(string repositoryPath, string fileName, bool all)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    if (all)
                    {
                        repo.Stage("*");
                        return ECommandStatus.OK;
                    }
                    else
                    {
                        repo.Stage(fileName);
                        return ECommandStatus.OK;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return ECommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Commits all staged files.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="message"></param>
        /// <param name="allowEmptyCommit"></param>
        /// <returns>CommandStatus</returns>
        private ECommandStatus Commit(string repositoryPath, string message, bool allowEmptyCommit)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    Signature author = new Signature(Config.Username, Config.Email, DateTime.Now);
                    Signature commiter = author;

                    CommitOptions commitOptions = new CommitOptions();
                    commitOptions.AllowEmptyCommit = allowEmptyCommit;
                    repo.Stage("*");
                    Commit commit = repo.Commit(message, author, commiter, commitOptions); 
                    return ECommandStatus.OK;
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return ECommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Merges branchName to current Branch.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        private ECommandStatus Merge(string repositoryPath, string branchName)
        {
            try
            {
                using ( var repo = new Repository(repositoryPath))
                {
                    var localBranch = repo.Branches[branchName];
                    if (localBranch != null)
                    {
                        Signature merger = new Signature(Config.Username, Config.Email, DateTime.Now);
                        return (ECommandStatus)(repo.Merge(localBranch, merger).Status);  
                    }
                    else
                    {
                        return ECommandStatus.BranchNotFound;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return ECommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Gets the current branch name.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <returns>Branch name</returns>
        private string GetCurrentBranch(string repositoryPath)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    return repo.Head.Name.ToString();
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets last commit message from current branch.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <returns>Commit message</returns>
        private string GetLastCommit(string repositoryPath)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    return repo.Head.Tip.Message;
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        #endregion

    }
}
