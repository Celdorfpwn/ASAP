using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace JiraService
{
    public class Jira : IJira
    {
        /// <summary>
        /// The Chrome Europe Jira project key
        /// </summary>
        public readonly string JIRA_PROJECT = "CHREU";

        private readonly string BASE_URL = "https://jira.softvision.ro/rest/api/latest/";

        private readonly string BROWSER_URL = "https://jira.softvision.ro/browse/";

        /// <summary>
        /// Get all available Jira versions
        /// </summary>
        /// <returns>List with all versions</returns>
        public List<Version> GetVersions()
        {
            string response = RunQuery("versions");

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<Version>));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response));
            var vers = (List<Version>)jsonSerializer.ReadObject(stream);

            return vers;
        }

        /// <summary>
        /// Search the Jira for issues that were fixed in the given version
        /// </summary>
        /// <param name="version">The version used to search for FixVersion</param>
        public SearchResult SearchIssues(Version version)
        {
            string query = String.Format("search?jql=FixVersion='{0}'+order+by+priority", version.Name);
            string response = RunQuery(query);

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SearchResult));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response));
            SearchResult vers = (SearchResult)jsonSerializer.ReadObject(stream);

            // Fill-up URL property
            foreach (Issue issue in vers.Issues)
            {
                issue.URL = BROWSER_URL + issue.Key;
                issue.Field.Severity = ParseSeverityFromTheme(issue.Field.Labels);
            }
            return vers;
        }

        public SearchResult GetIssuesForPerson(String persionID)
        {
            string query = String.Format("search?jql=status%20in%20(Open%2C%20'In%20Progress'%2C%20Reopened)%20AND%20assignee%20in%20('{0}')+order+by+priority", persionID);
            string response = RunQuery(query);

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SearchResult));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response));
            SearchResult vers = (SearchResult)jsonSerializer.ReadObject(stream);

            // Fill-up extra stuff
            foreach (Issue issue in vers.Issues)
            {
                issue.URL = BROWSER_URL + issue.Key;
                issue.Field.Severity = ParseSeverityFromTheme(issue.Field.Labels);
                AppendCommentsForIssue(issue);
            }

            return vers;
        }

        /// <summary>
        /// Adds the jira comments to the given issue
        /// </summary>
        /// <param name="issue">Issue to add comments to</param>
        /// <returns>Comments for the given issue</returns>
        public Comment AppendCommentsForIssue(Issue issue)
        {
            issue.Field.Comment = GetIssue(issue.Key).Field.Comment;

            return issue.Field.Comment;
        }

        /// <summary>
        /// Parse the Issue epic theme to Severity. Always return the 1st severity from Labels
        /// </summary>
        /// <param name="issueLabels">Issue labels</param>
        /// <returns>Issue severity</returns>
        private Severity ParseSeverityFromTheme(string[] issueLabels)
        {
            foreach (string label in issueLabels)
            {
                try
                {
                    return (Severity)Enum.Parse(typeof(Severity), label.Trim(), true);
                }
                catch
                {
                    continue;
                }
            }
            return Severity.None;
        }

        /// <summary>
        /// Assign the given issue to the specified name
        /// </summary>
        /// <param name="issue">Issue to be assigned to newName</param>
        /// <param name="newName">The person that gets the Issue</param>
        public void AssignIssue(Issue issue, string newName)
        {
            string query = String.Format("issue/{0}", issue.Key);
            //string data = "{\"update\" : {\"components\" : [{\"set\" : [{\"assignee\" : \"" + email + "\"}]}]}}";
            //string data = "{\"update\" : {\"assignee\" :[\"set\" : {\"name\" : \"" + email + "\"}]}}";
            string data = "{\"fields\" : {\"assignee\" : {\"name\" : \"" + newName + "\"}}}";
            //string data = String.Format("{\"update\":{\"components\":[{\"set\":[{\"assignee\":\"{0}\"}]]}}",email);
            string response = RunQuery(query, data);
            if (!String.IsNullOrEmpty(response))
            {
                throw new InvalidDataException("Reassign flow return not expected data");
            }
        }

        /// <summary>
        /// Gets the Issue object based on the given issueKey
        /// </summary>
        /// <param name="issueKey">Issue key</param>
        public Issue GetIssue(string issueKey)
        {
            string query = String.Format("issue/{0}", issueKey);

            string response = RunQuery(query);

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Issue));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response));
            Issue issue = (Issue)jsonSerializer.ReadObject(stream);

            // Fill-up URL property
            issue.URL = BROWSER_URL + issue.Key;

            // Add the severity field
            issue.Field.Severity = ParseSeverityFromTheme(issue.Field.Labels);

            return issue;
        }


        /// <summary>
        /// Execute Jira query
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns>JSON string response</returns>
        private String RunQuery(string argument = null, string data = null, string method = "GET")
        {
            string url = String.Empty;
            if (!argument.StartsWith("search") && !argument.StartsWith("issue"))
            {
                url = string.Format("{0}project/{1}/", BASE_URL, JIRA_PROJECT);

                if (argument != null)
                {
                    url = string.Format("{0}{1}/", url, argument);
                }
            }
            else
            {
                // It's a issue search, don't add the project name to the address
                url = String.Format("{0}{1}", BASE_URL, argument);

            }

            System.Net.ServicePointManager.Expect100Continue = false;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = method;

            if (data != null)
            {
                request.Method = "PUT";
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(data);
                }
            }

            string base64Credentials = "dmFsZW50aW4ubWl0YXJ1OkNlZmFjaWF6aTAx";
            request.Headers.Add("Authorization", "Basic " + base64Credentials);

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            if (data != null && response.StatusCode == HttpStatusCode.NoContent)
            {
                // You should just receive a response with a status of "204 No Content"
                return string.Empty;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
            }

            string result = string.Empty;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}
