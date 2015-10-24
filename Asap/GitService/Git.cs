using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace GitService
{

    public static class Git
    {
        static string _userName = "ahito";
        static string _password = "testAsap1";
        static string _email = "test@test.com";
        static string _repositoryPath = "";

        public static string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public static string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public static string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public static string RepositoryPath
        {
            get { return _repositoryPath; }
            set { _repositoryPath = value; }
        }

        public static CommandStatus Pull()
        {
            return Pull(_repositoryPath);
        }

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

        public static CommandStatus Push(string branchName)
        {
            return Push(_repositoryPath, branchName);
        }

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

        public static CommandStatus PushToOrigin(string branchName)
        {
            return PushToOrigin(_repositoryPath, branchName);
        }

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

        public static CommandStatus Branch(string branchName)
        {
            return Branch(_repositoryPath, branchName);
        }

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

        public static CommandStatus DeleteBranch(string branchName, bool isRemote = false)
        {
            return DeleteBranch(_repositoryPath, branchName, isRemote);
        }

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

        public static CommandStatus Checkout(string branchName)
        {
            return Checkout(_repositoryPath, branchName);
        }

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

        public static CommandStatus AddAll()
        {
            return AddAll(_repositoryPath);
        }


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


        public static CommandStatus Commit(string message, bool allowEmptyCommit = false)
        {
            return Commit(_repositoryPath, message, allowEmptyCommit);
        }

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

        public static CommandStatus Merge(string branchName)
        {
            return Merge(_repositoryPath, branchName);
        }

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
                        repo.Merge(localBranch, merger);
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

        public static string GetCurrentBranch()
        {
            return GetCurrentBranch(_repositoryPath);
        }

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

    }
}
