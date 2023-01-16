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

        public static bool Checkout(string respositoryPath, string branchName)
        {
            try
            {
                using Repository localRepo = new Repository(respositoryPath);
                Commit localCommit = localRepo.Lookup<Commit>(branchName);
                Commands.Checkout(localRepo, localCommit);
                return true;
            }
            catch (Exception ex)
            {
                StaticObjects.FireShowExceptionMessage($"Checkout Error, repository= {respositoryPath}, branch= {branchName}: ", ex);
                return false;
            }
        }
    }
}
