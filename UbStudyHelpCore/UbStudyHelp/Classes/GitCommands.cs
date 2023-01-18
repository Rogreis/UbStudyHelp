using CommonMark.Syntax;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UbStandardObjects.Objects;
using UbStandardObjects;
using Lucene.Net.QueryParsers.Flexible.Messages;
using Lucene.Net.Documents;

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

        public static bool Checkout(string repositoryPath, string branchName)
        {
            try
            {
                using Repository localRepo = new Repository(repositoryPath);
                Commit localCommit = localRepo.Lookup<Commit>(branchName);
                Commands.Checkout(localRepo, localCommit);
                return true;
            }
            catch (Exception ex)
            {
                StaticObjects.FireShowExceptionMessage($"Checkout Error, repository= {repositoryPath}, branch= {branchName}: ", ex);
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
                return false;
            }
        }

    }
}
