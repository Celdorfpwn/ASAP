namespace JiraService
{
    interface IJira
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
        /// <param name="persionID">The person name (firstName.lastName)</param>
        /// <returns>Search result object</returns>
        SearchResult GetIssuesForPerson(string persionID);

        /// <summary>
        /// Get all available Jira versions
        /// </summary>
        /// <returns>List with all versions</returns>
        System.Collections.Generic.List<Version> GetVersions();

        /// <summary>
        /// List Jira issues fixed in the given version
        /// </summary>
        /// <param name="version">The version used to search for FixVersion</param>
        SearchResult SearchIssues(Version version);

        /// <summary>
        /// Adds the jira comments to the given issue
        /// </summary>
        /// <param name="issue">Issue to add comments to</param>
        /// <returns>Comments for the given issue</returns>
        Comment AppendCommentsForIssue(Issue issue);
    }
}
