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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
                return CommandStatus.FAIL;
            }
        }

        public static CommandStatus PushToOrigin(string branchName)
        {
            return PushToOrigin(_repositoryPath, branchName);
        }

        public static CommandStatus PushToOrigin(string reposiotryPath, string branchName)
        {
            try
            {
                using (var repo = new Repository(reposiotryPath))
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
                return CommandStatus.FAIL;
            }
        }

        public static string Checkout(string repositoryPath, string branchName)
        {
            using (var repo = new Repository(repositoryPath))
            {
                Branch branch = repo.Checkout(branchName);
                return branch.Name;
            }
        }

        public static string Commit(string repositoryPath, string message)
        {
            using (var repo = new Repository(repositoryPath))
            {
                Signature author = new Signature(_userName, _email, DateTime.Now);
                Signature commiter = author;

                Commit commit = repo.Commit(message, author, commiter);
                return commit.Message;
            }
        }

    }
}
