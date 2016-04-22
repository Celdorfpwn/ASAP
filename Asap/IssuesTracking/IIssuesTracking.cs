using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssuesTracking
{
    public interface IIssuesTracking
    {        
        /// <summary>
        /// Assign the given issue to the specified name
        /// </summary>
        /// <param name="issue">Issue to be assigned to newName</param>
        /// <param name="newName">The person that gets the Issue</param>
        void AssignIssue(Issue issue, string newName);

        /// <summary>
        /// Return the Issue object for the given Jira Key
        /// </summary>
        /// <param name="issueKey">Issue key to query for</param>
        /// <returns>Issue jira for the given key</returns>
        Issue GetIssue(string issueKey);

        /// <summary>
        /// Returns all Issues for the given personId
        /// </summary>
        /// <param name="username">The person name (firstName.lastName)</param>
        /// <returns>Search result object</returns>
        SearchResult GetIssues(string username = "", string project = "");

        /// <summary>
        /// Get all the Jira items for the specified project with the specified status
        /// </summary>
        /// <param name="project">The project to query</param>
        /// <param name="status">The status to search for</param>
        /// <param name="type">The type of the issue</param>
        /// <returns>SearchResult with the requested issues</returns>
        SearchResult GetIssues(string project, List<JiraItemStatus> status, JiraIssueType type);

        /// <summary>
        /// Get all available Jira versions
        /// </summary>
        /// <returns>List with all versions</returns>
        IEnumerable<ItVersion> GetVersions();

        /// <summary>
        /// List Jira issues fixed in the given version
        /// </summary>
        /// <param name="version">The version used to search for FixVersion</param>
        SearchResult SearchIssues(ItVersion version);

        /// <summary>
        /// Adds the jira comments to the given issue
        /// </summary>
        /// <param name="issue">Issue to add comments to</param>
        /// <returns>Comments for the given issue</returns>
        Comment AppendCommentsForIssue(Issue issue);

        /// <summary>
        /// Set the next status
        /// </summary>
        /// <param name="issue">The issue to change the status for</param>
        /// <param name="status">The new status (transition)</param>
        /// <param name="message">Message to add</param>
        /// <param name="fixVersion">Issue fixed version</param>
        void SetStatus(Issue issue, JiraTransition status, string message, ItVersion fixVersion);


        Attachment[] AppendAttachmentForIssue(Issue issue);

        void AddMessage(Issue issue,string message);
    }
}
