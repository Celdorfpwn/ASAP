using System;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace GitService
{
    /// <summary>
    /// Git commands utility class.
    /// </summary>
    public static class Git
    {
        static string _userName = "ahito";
        static string _password = "testAsap1";
        static string _email = "test@test.com";
        static string _repositoryPath = "E:/TestGit/TestRepo/TestRepo";

        /// <summary>
        /// Represents the git UserName.
        /// </summary>
        /// <returns>UserName</returns>
        public static string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        /// <summary>
        /// Represents the password for git.
        /// </summary>
        /// <returns>Password</returns>
        public static string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// Represents the users email.
        /// </summary>
        /// <returns>Email</returns>
        public static string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        /// <summary>
        /// Represents the path to the Repository.
        /// </summary>
        /// <returns>Repository path</returns>
        public static string RepositoryPath
        {
            get { return _repositoryPath; }
            set { _repositoryPath = value; }
        }

        /// <summary>
        /// Git pull command
        /// </summary>
        /// <returns>CommandStatus</returns>
        public static CommandStatus Pull()
        {
            return Pull(_repositoryPath);
        }

        /// <summary>
        /// Git pull command
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus Pull(string repositoryPath)
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
                        Username = _userName,
                        Password = _password
                    });

                    MergeResult result = repo.Network.Pull(new Signature(_userName, _email, new DateTimeOffset(DateTime.Now)), options);
                    return (CommandStatus)result.Status;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return CommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Git push command.
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus Push(string branchName)
        {
            return Push(_repositoryPath, branchName);
        }

        /// <summary>
        /// Git push command.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus Push(string repositoryPath, string branchName)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    PushOptions options = new PushOptions();
                    options.CredentialsProvider = new CredentialsHandler(
                        (url, usernameFromUrl, types) => new UsernamePasswordCredentials()
                    {
                        Username = _userName,
                        Password = _password
                    });

                    var localBranch = repo.Branches[branchName];
                    if (localBranch != null)
                    {
                        repo.Network.Push(repo.Branches[branchName], options);
                        return CommandStatus.OK;
                    }
                    else
                    {
                        return CommandStatus.BranchNotFound;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return CommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Pushes the branch to remote origin.
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus PushToOrigin(string branchName)
        {
            return PushToOrigin(_repositoryPath, branchName);
        }

        /// <summary>
        /// Pushes the branch to remote origin.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus PushToOrigin(string repositoryPath, string branchName)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    Remote remote = repo.Network.Remotes["origin"];

                    var localBranch = repo.Branches[branchName];
                    if (localBranch != null)
                    {
                        repo.Branches.Update(localBranch, b => b.Remote = remote.Name, b => b.UpstreamBranch = localBranch.CanonicalName);
                        var options = new PushOptions();
                        options.CredentialsProvider = new CredentialsHandler(
                            (url, usernameFromUrl, types) => new UsernamePasswordCredentials
                        {
                            Username = _userName,
                            Password = _password
                        });
                        repo.Network.Push(localBranch, options);
                        return CommandStatus.OK;
                    }
                    else
                    {
                        return CommandStatus.BranchNotFound;
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return CommandStatus.FAIL;
            }

        }

        /// <summary>
        /// Creates a new branch with branchName.
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus Branch(string branchName)
        {
            return Branch(_repositoryPath, branchName);
        }

        /// <summary>
        /// Creates a new branch with branchName.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus Branch(string repositoryPath, string branchName)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    var localBranch = repo.Branches[branchName];
                    if (localBranch == null)
                    {
                        Branch branch = repo.CreateBranch(branchName);
                        return CommandStatus.OK;
                    }
                    else
                    {
                        return CommandStatus.BranchAlreadyExists;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return CommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Deletes the branch branchName.
        /// </summary>
        /// <param name="branchName"></param>
        /// <param name="isRemote"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus DeleteBranch(string branchName, bool isRemote = false)
        {
            return DeleteBranch(_repositoryPath, branchName, isRemote);
        }

        /// <summary>
        /// Deletes the branch branchName.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="branchName"></param>
        /// <param name="isRemote"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus DeleteBranch(string repositoryPath, string branchName, bool isRemote = false)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    var localBranch = repo.Branches[branchName];
                    if (localBranch != null)
                    {
                        repo.Branches.Remove(branchName, isRemote);
                        return CommandStatus.OK;
                    }
                    else
                    {
                        return CommandStatus.BranchNotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return CommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Checks out branch branchName.
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus Checkout(string branchName)
        {
            return Checkout(_repositoryPath, branchName);
        }

        /// <summary>
        /// Checks out branch branchName.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus Checkout(string repositoryPath, string branchName)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    var localBranch = repo.Branches[branchName];
                    if (localBranch != null)
                    {                                               
                        Branch branch = repo.Checkout(branchName);
                        return CommandStatus.OK;
                    }
                    else
                    {
                        return CommandStatus.BranchNotFound;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return CommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Stages all files.
        /// </summary>
        /// <returns>CommandStatus</returns>
        public static CommandStatus AddAll()
        {
            return AddAll(_repositoryPath);
        }

        /// <summary>
        /// Stages all files.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus AddAll(string repositoryPath)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {

                    repo.Stage("*");
                    return CommandStatus.OK;
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return CommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Commits all staged files.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="allowEmptyCommit"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus Commit(string message, bool allowEmptyCommit = false)
        {
            return Commit(_repositoryPath, message, allowEmptyCommit);
        }

        /// <summary>
        /// Commits all staged files.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="message"></param>
        /// <param name="allowEmptyCommit"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus Commit(string repositoryPath, string message, bool allowEmptyCommit = false)
        {
            try
            {
                using (var repo = new Repository(repositoryPath))
                {
                    Signature author = new Signature(_userName, _email, DateTime.Now);
                    Signature commiter = author;

                    CommitOptions commitOptions = new CommitOptions();
                    commitOptions.AllowEmptyCommit = allowEmptyCommit;

                    Commit commit = repo.Commit(message, author, commiter, commitOptions); 
                    return CommandStatus.OK;
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return CommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Merges branchName to current Branch.
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus Merge(string branchName)
        {
            return Merge(_repositoryPath, branchName);
        }

        /// <summary>
        /// Merges branchName to current Branch.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="branchName"></param>
        /// <returns>CommandStatus</returns>
        public static CommandStatus Merge(string repositoryPath, string branchName)
        {
            try
            {
                using ( var repo = new Repository(repositoryPath))
                {
                    var localBranch = repo.Branches[branchName];
                    if (localBranch != null)
                    {
                        Signature merger = new Signature(_userName, _email, DateTime.Now);
                        return (CommandStatus)(repo.Merge(localBranch, merger).Status);  
                    }
                    else
                    {
                        return CommandStatus.BranchNotFound;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return CommandStatus.FAIL;
            }
        }

        /// <summary>
        /// Get the current branch name.
        /// </summary>
        /// <returns>Branch name</returns>
        public static string GetCurrentBranch()
        {
            return GetCurrentBranch(_repositoryPath);
        }

        /// <summary>
        /// Gets the current branch name.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <returns>Branch name</returns>
        public static string GetCurrentBranch(string repositoryPath)
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
        /// <returns>Commit message</returns>
        public static string GetLastCommit()
        {
            return GetLastCommit(_repositoryPath);
        }

        /// <summary>
        /// Gets last commit message from current branch.
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <returns>Commit message</returns>
        public static string GetLastCommit(string repositoryPath)
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
    }
}
