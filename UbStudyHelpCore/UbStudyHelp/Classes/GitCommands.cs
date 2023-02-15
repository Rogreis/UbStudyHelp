using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using UbStandardObjects;

namespace UbStudyHelp.Classes
{
    internal class GitCommands
    {

        /// <summary>
        /// Check is a path is a valid repository
        /// </summary>
        /// <param name="respositoryPath"></param>
        /// <returns></returns>
        public static bool IsValid(string respositoryPath)
        {
            return Repository.IsValid(respositoryPath);
        }


        private CredentialsHandler Credentials(string username, string password)
        {
            return new CredentialsHandler(
                    (url, usernameFromUrl, types) =>
                        new UsernamePasswordCredentials()
                        {
                            Username = "username",
                            Password = "password"
                        });
        }

        private static void CheckoutProgress(string path, int completedSteps, int totalSteps)
        {
            EventsControl.FireSendMessage($"Checkout progress: {completedSteps} of {totalSteps}");
        }


        private static bool Fetch(Repository repo)
        {
            try
            {
                var options = new FetchOptions();
                options.Prune = true;
                options.TagFetchMode = TagFetchMode.Auto;
                //options.CredentialsProvider = Credentials(string username, string password)
                Remote remote = repo.Network.Remotes["origin"];
                string msg = "Fetching remote";
                IEnumerable<string> refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                Commands.Fetch(repo, remote.Name, refSpecs, options, msg);
                return true;
            }
            catch (Exception ex)
            {
                StaticObjects.FireShowExceptionMessage($"Fetch Error, repository= {repo.Head.FriendlyName}: ", ex);
                return false;
            }
        }


        public static bool Pull(string repositoryPath)
        {
            try
            {
                using Repository localRepo = new Repository(repositoryPath);
                PullOptions pullOptions = new PullOptions();
                pullOptions.FetchOptions = new FetchOptions();
                //options.CredentialsProvider = Credentials(string username, string password)
                Commands.Pull(localRepo, new Signature("username", "rogreis@gmail.com", new DateTimeOffset(DateTime.Now)), pullOptions);
                return true;
            }
            catch (Exception ex)
            {
                StaticObjects.FireShowExceptionMessage($"Pull Error, repository= {repositoryPath}: ", ex);
                StaticObjects.Logger.Error($"Pull Error, repository= {repositoryPath}: ", ex);
                return false;
            }
        }


        /// <summary>
        /// Checkout a branch overwriting local changes
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="branchName"></param>
        /// <returns></returns>
        public static bool Checkout(string repositoryPath, string branchName)
        {
            try
            {
                using Repository localRepo = new Repository(repositoryPath);
                var branchNameFull = $@"refs/heads/{branchName}";
                Branch branch = localRepo.Branches.ToList().Find(b => b.CanonicalName == branchNameFull);
                if (branch == null)
                {
                    // Let's get a reference on the remote tracking branch...
                    string trackedBranchName = $"origin/{branchName}";
                    Branch trackedBranch = localRepo.Branches[trackedBranchName];

                    // ...and create a local branch pointing at the same Commit
                    branch = localRepo.CreateBranch(branchName, trackedBranch.Tip);

                    // The local branch is not configured to track anything
                    if (!branch.IsTracking)
                    {
                        // So, let's configure the local branch to track the remote one.
                        Branch updatedBranch = localRepo.Branches.Update(branch, b => b.TrackedBranch = trackedBranch.CanonicalName);
                    }

                }
                CheckoutOptions options = new CheckoutOptions() { CheckoutModifiers = CheckoutModifiers.Force, OnCheckoutProgress = CheckoutProgress };
                Branch currentBranch = Commands.Checkout(localRepo, branchName, options);
                return true;
            }
            catch (Exception ex)
            {
                StaticObjects.FireShowExceptionMessage($"Checkout Error, repository= {repositoryPath}, branch= {branchName}: ", ex);
                StaticObjects.Logger.Error($"Checkout Error, repository= {repositoryPath}, branch= {branchName}: ", ex);
                return false;
            }
        }

        public static bool Clone(string sourceUrl, string repositoryPath)
        {
            try
            {
                var cloneOptions = new CloneOptions { BranchName = "master", Checkout = true };
                var cloneResult = Repository.Clone(sourceUrl, repositoryPath);
                return true;
            }
            catch (Exception ex)
            {
                StaticObjects.FireShowExceptionMessage($"Clone Error, repository= {repositoryPath}, sourceUrl= {sourceUrl}: ", ex);
                StaticObjects.Logger.Error($"Clone Error, repository= {repositoryPath}, sourceUrl= {sourceUrl}: ", ex);
                return false;
            }
        }

        /// <summary>
        /// Commit a list of files to the repository
        /// </summary>
        /// <param name="authorName"></param>
        /// <param name="email"></param>
        /// <param name="relativeFilesList"></param>
        /// <param name="repositoryPath"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool CommitFiles(string authorName, string email, List<string> relativeFilesList, string repositoryPath, string message)
        {
            try
            {
                using Repository localRepo = new Repository(repositoryPath);
                if (localRepo.Info.IsHeadDetached)
                {
                    StaticObjects.FireSendMessage("Cannot commit files, repository is in detached head state");
                    return false;
                }

                Signature author = new Signature(authorName, email, DateTime.Now);
                Signature committer = author;
                Commands.Stage(localRepo, relativeFilesList);
                // Commit the changes
                localRepo.Commit(message, author, committer);
                return true;
            }
            catch (Exception ex)
            {
                StaticObjects.FireShowExceptionMessage("Commit files error", ex);
                StaticObjects.Logger.Error("Commit files error", ex);
                return false;
            }
        }

        /// <summary>
        /// Push the repository to the remote
        /// </summary>
        /// <param name="repositoryPath"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool Push(string repositoryPath, string username, string password, string branch)
        {
            try
            {
                //using (var repo = new Repository(repositoryPath))
                //{

                //    var remote = repo.Network.Remotes[branch];
                //    PushOptions pushOptions = new PushOptions
                //    {
                //        CredentialsProvider = (_url, _user, _cred) =>
                //            new UsernamePasswordCredentials { Username = username, Password = password }
                //    };
                //    var pushRefSpec = @"refs/heads/master";
                //    repo.Network.Push(remote, pushRefSpec, pushOptions, new Signature(username, email, DateTimeOffset.Now),
                //        "pushed changes");

                //    // Fetch the remote repository to ensure it's up-to-date
                //    Remote remote = repo.Network.Remotes["origin"];
                //    var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                //    Commands.Fetch(repo, remote.Name, refSpecs, new FetchOptions(), "");

                //    // Push the changes to the remote repository
                //    repo.Network.Push(repo.Branches["main"], pushOptions);
                //}
                return true;
            }
            catch (Exception ex)
            {
                StaticObjects.FireShowExceptionMessage("Push error", ex);
                StaticObjects.Logger.Error("Push error", ex);
                return false;
            }
        }



    }
}
