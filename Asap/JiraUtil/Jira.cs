using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using IssuesTracking;
using ToolsConfiguration;

namespace JiraService
{
    public class Jira : IIssuesTracking
    {
        private IIssuesTrackingConfig Config { get; set; }

        #region Constructor
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="credentials">Jira credentials user:pass</param>
        public Jira(IIssuesTrackingConfig config)
        {
            Config = config;
        }
        #endregion

        /// <summary>
        /// Get all available Jira versions
        /// </summary>
        /// <returns>List with all versions</returns>
        public IEnumerable<ItVersion> GetVersions()
        {
            string response = RunQuery("versions");

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<ItVersion>));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response));
            var vers = (List<ItVersion>)jsonSerializer.ReadObject(stream);

            return vers;
        }

        /// <summary>
        /// Search the Jira for issues that were fixed in the given version
        /// </summary>
        /// <param name="version">The version used to search for FixVersion</param>
        public SearchResult SearchIssues(ItVersion version)
        {
            string query = String.Format("search?jql=FixVersion='{0}'+order+by+priority", version.Name);
            string response = RunQuery(query);

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SearchResult));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response));
            SearchResult vers = (SearchResult)jsonSerializer.ReadObject(stream);

            // Fill-up URL property
            foreach (Issue issue in vers.Issues)
            {
                issue.URL = Config.BrowserUrl + issue.Key;
                issue.Field.Severity = ParseSeverityFromTheme(issue.Field.Labels);
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
        /// Adds the jira attachments to the given issue
        /// </summary>
        /// <param name="issue">Issue to add attachments to </param>
        /// <returns>Jira attachments for the given issue</returns>
        public Attachment[] AppendAttachmentForIssue(Issue issue)
        {
            Issue i = GetIssue(issue.Key);

            issue.Field.Attachment = i.Field.Attachment;

            return issue.Field.Attachment;
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
                Severity s = Severity.None;
                if (Enum.TryParse(label.Trim().ToUpper(), out s))
                {
                    return s;
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

            string logMsg = String.Format("{0} assigned to {1}", issue.Key, newName);
            LoggerService.Log.Instance.AddLog(new LoggerService.LogEntry(LoggerService.LogType.Info, logMsg));
        }

        /// <summary>
        /// Creates the Jira issue.
        /// </summary>
        /// <param name="issue">The issue.</param>
        /// <param name="project">The project.</param>
        /// <returns>CreateResult object</returns>
        /// <exception cref="InvalidDataException">Create item return not expected data</exception>
        public CreateResult CreateIssue(Issue issue)
        {
            issue.Field.Assignee = new Person();
            issue.Field.Assignee.Name = "valentin.mitaru";
            string query = String.Format("issue/");
            string data = "{\"fields\" : {\"project\" : {\"key\" : \"" + Config.Project + "\"}, \"summary\":\"" + issue.Field.Summary + "\", \"description\":\"" + issue.Field.Description + "\", \"issuetype\": {\"name\":\"" + issue.Field.IssueType.Name + "\"}, \"assignee\": {\"name\":\"" + issue.Field.Assignee.Name + "\"}}}";
            string response = RunQuery(query, data, create:true);
            if (String.IsNullOrEmpty(response))
            {
                throw new InvalidDataException("Create item return not expected data");
            }

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(CreateResult));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response));
            CreateResult createdItem = (CreateResult)jsonSerializer.ReadObject(stream);
            return createdItem;
        }

        private string JiraJQLConverter(JiraItemStatus status)
        {
            switch(status)
            {
                case JiraItemStatus.InProgress:
                    return "'In%20Progress'";
                default:
                    return status.ToString();
            }
        }

        /// <summary>
        /// Returns all Issues for the given personId
        /// </summary>
        /// <param name="username">The person name (firstName.lastName)</param>
        /// <returns>Search result object</returns>
        public SearchResult GetIssues(string username = "", string project = "")
        {
            username = String.IsNullOrWhiteSpace(username) ? Config.Username : username;
            project = String.IsNullOrWhiteSpace(project) ? Config.Project : project;
            
            List<JiraItemStatus> status = new List<JiraItemStatus>() { JiraItemStatus.Open, JiraItemStatus.Reopened, JiraItemStatus.InProgress, JiraItemStatus.Resolved  };
            string statusMsg = "status%20in%20(";
            for (int i = 0; i < status.Count; i++)
            {
                if (i < (status.Count - 1))
                {
                    statusMsg += JiraJQLConverter(status[i]) + "%2C%20";
                }
                else
                {
                    statusMsg += JiraJQLConverter(status[i]) + ")";
                }
            }
            string query = String.Format("search?jql={0}%20AND%20assignee%20in%20('{1}')%20AND%20project%20%3D{2}+order+by+priority", statusMsg, username, project);
            string response = RunQuery(query);

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SearchResult));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response));
            SearchResult vers = (SearchResult)jsonSerializer.ReadObject(stream);

            // Fill-up extra stuff
            foreach (Issue issue in vers.Issues)
            {
                issue.URL = Config.BrowserUrl + issue.Key;
                issue.Field.Severity = ParseSeverityFromTheme(issue.Field.Labels);
                //AppendCommentsForIssue(issue);
                //Attachment[] a = AppendAttachmentForIssue(issue);
            }

            return vers;
        }

        /// <summary>
        /// Get all the Jira items for the specified project with the specified status
        /// </summary>
        /// <param name="project">The project to query</param>
        /// <param name="status">The status to search for</param>
        /// <param name="type">The type of the issue</param>
        /// <returns>SearchResult with the requested issues</returns>
        public SearchResult GetIssues(string project, List<JiraItemStatus> status, JiraIssueType type)
        {
            string statusMsg = "status%20in%20(";
            for (int i = 0; i < status.Count; i++)
            {
                if (i < (status.Count - 1))
                {
                    statusMsg += JiraJQLConverter(status[i]) + "%2C%20";
                }
                else
                {
                    statusMsg += JiraJQLConverter(status[i]) + ")";
                }
            }

            string query = String.Format("search?jql={0}%20AND%20issuetype%3D{1}%20AND%20project%20%3D{2}+order+by+priority", statusMsg, (int)type, project);
            string response = RunQuery(query);

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SearchResult));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response));
            SearchResult res = (SearchResult)jsonSerializer.ReadObject(stream);

            // Fill-up extra stuff
            foreach (Issue issue in res.Issues)
            {
                issue.URL = Config.BrowserUrl + issue.Key;
                issue.Field.Severity = ParseSeverityFromTheme(issue.Field.Labels);
                //AppendCommentsForIssue(issue);
                //Attachment[] a = AppendAttachmentForIssue(issue);
            }

            return res;
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
            issue.URL = Config.BrowserUrl + issue.Key;

            // Add the severity field
            issue.Field.Severity = ParseSeverityFromTheme(issue.Field.Labels);

            return issue;
        }

        /// <summary>
        /// Set the next status
        /// </summary>
        /// <param name="issue">The issue to change the status for</param>
        /// <param name="newStatus">The new status (transition)</param>
        /// <param name="message">Message to add</param>
        public void SetStatus(Issue issue, JiraTransition newStatus, string message, ItVersion fixVersion)
        {
            string query = String.Format("issue/{0}", issue.Key);
            string data = "{\"transition\" : {\"id\" : \"" + (int)newStatus + "\"}}";
            if (!String.IsNullOrWhiteSpace(message))
            {
                data = "{\"update\" : {\"comment\" : [{\"add\" : {\"body\" : \"" + message + "\"}}]}, \"transition\": {\"id\": \"" + (int)newStatus + "\"}}";
            }

            //TODO: Implement setting-up fix fixVersion

            string response = RunQuery(query, data, "POST");

            if (!String.IsNullOrEmpty(response))
            {
                throw new InvalidDataException("Set status return not expected data");
            }

            string logMsg = String.Format("{0} changed into {1}", issue.Key, newStatus);
            LoggerService.Log.Instance.AddLog(new LoggerService.LogEntry(LoggerService.LogType.Info, logMsg));
        }

        public void AddMessage(Issue issue, string message)
        {
            string query = String.Format("issue/{0}", issue.Key);
            string data = "{\"update\" : {\"comment\" : [{\"add\" : {\"body\" : \"" + message + "\"}}]}}";
            string response = RunQuery(query, data, "POST");
        }

        /// <summary>
        /// Add the provided files as attachement for the proviced Issue
        /// </summary>
        /// <param name="issueKey">Jira key (CHREU-?*)</param>
        /// <param name="filePaths">IEnumarable with full paths as strings</param>
        /// <returns>True if the files were attached</returns>
        public bool AddAttachments(string issueKey, IEnumerable<string> filePaths)
        {
            if (filePaths == null)
            {
                return false;
            }
            string query = String.Format("issue/{0}/attachments", issueKey);
            // It's a issue search, don't add the project name to the address
            string issueLinkUrl = String.Format("{0}{1}", Config.BaseUrl, query);

            //string restUrl = Jira.FormatRestUrl(m_JiraId, true);
            //string issueLinkUrl = String.Format("{0}/issue/{1}/attachments", restUrl, issueKey);

            var filesToUpload = new List<FileInfo>();
            foreach (var filePath in filePaths)
            {
                if (!File.Exists(filePath))
                {
                    //Jira.LogError("File '{0}' doesn't exist", filePath);
                    return false;
                }

                var file = new FileInfo(filePath);
                if (file.Length > 10485760) // TODO Get Actual Limit
                {
                    //Jira.LogError("Attachment too large");
                    return false;

                }

                filesToUpload.Add(file);
            }

            if (filesToUpload.Count <= 0)
            {
                //Jira.LogWarning("No file to Upload");
                return false;
            }

            return PostMultiPart(issueLinkUrl, filesToUpload);
        }

        /// <summary>
        /// Attached the provided files to the given REST URL
        /// </summary>
        /// <param name="restUrl">REST url</param>
        /// <param name="filePaths">enumerable for file paths to include</param>
        /// <returns>true if the upload was successfull, otherwise false</returns>
        private bool PostMultiPart(string restUrl, IEnumerable<FileInfo> filePaths)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            bool success = true;
            try
            {
                var boundary = string.Format("----------{0:N}", Guid.NewGuid());
                var content = new MemoryStream();
                var writer = new StreamWriter(content);

                foreach (var filePath in filePaths)
                {
                    var fs = new FileStream(filePath.FullName, FileMode.Open, FileAccess.Read);
                    var data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                    fs.Close();

                    writer.WriteLine("--{0}", boundary);
                    writer.WriteLine("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"", filePath.Name);
                    writer.WriteLine("Content-Type: application/octet-stream");
                    writer.WriteLine();
                    writer.Flush();

                    content.Write(data, 0, data.Length);

                    writer.WriteLine();
                    Console.WriteLine("Attaching " + filePath.Name);
                }

                writer.WriteLine("--" + boundary + "--");
                writer.Flush();
                content.Seek(0, SeekOrigin.Begin);

                System.Net.ServicePointManager.Expect100Continue = false;
                request = WebRequest.Create(restUrl) as HttpWebRequest;
                if (request == null)
                {
                    throw new WebException("Unable to create REST query: " + restUrl);
                }

                request.Method = "POST";
                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                request.Accept = "application/json";
                request.Headers.Add("Authorization", "Basic " + CredentialsAsBase64());
                request.Headers.Add("X-Atlassian-Token", "nocheck");
                request.ContentLength = content.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    content.WriteTo(requestStream);
                    requestStream.Close();
                }

                using (response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var reader = new StreamReader(response.GetResponseStream());
                        throw new WebException(String.Format("The server returned '{0}'\n{1}", response.StatusCode, reader.ReadToEnd()));
                    }

                    Console.WriteLine("File(s) uploaded successfully.");
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        var reader = new StreamReader(errorResponse.GetResponseStream());
                        throw new WebException(String.Format("The server returned '{0}'\n{1}", response.StatusCode, reader.ReadToEnd()));
                    }
                }

                if (request != null)
                {
                    request.Abort();
                }
                Console.WriteLine("File(s) upload failed.");
                success = false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return success;
        }

        /// <summary>
        /// Execute Jira query
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <param name="create">True if the query is to create a new jira item, otherwise false.</param>
        /// <returns>JSON string response</returns>
        private String RunQuery(string argument = null, string data = null, string method = "GET", bool create = false)
        {
            string url = String.Empty;
            if (!argument.StartsWith("search") && !argument.StartsWith("issue"))
            {
                url = string.Format("{0}project/{1}/", Config.BaseUrl, Config.Project);

                if (argument != null)
                {
                    url = string.Format("{0}{1}/", url, argument);
                }
            }
            else if (!String.IsNullOrEmpty(data) && data.Contains("transition"))
            {
                url = String.Format("{0}{1}/transitions?expand=transitions.fields", Config.BaseUrl, argument);
            }
            else
            {
                // It's a issue search, don't add the project name to the address
                url = String.Format("{0}{1}", Config.BaseUrl, argument);
            }

            System.Net.ServicePointManager.Expect100Continue = false;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = method;

            if (data != null)
            {
                if (data.Contains("transition"))
                {
                    request.Method = "POST";
                }
                else
                {
                    request.Method = "PUT";
                }
				if (create)
                {
                    request.Method = "POST";
                }
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(data);
                }
            }

            request.Headers.Add("Authorization", "Basic " + CredentialsAsBase64());
            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                LoggerService.Log.Instance.AddException(e);
				throw e;
            }

            #region Handle specific commands reqponse

            // 204 for transition and reassign
            if (data != null && response.StatusCode == HttpStatusCode.NoContent)
            {
                // You should just receive a response with a status of "204 No Content"
                return string.Empty;
            }

            // If none of above, throw exception if not OK
            if ((!create && response.StatusCode != HttpStatusCode.OK) ||
                (create && response.StatusCode != HttpStatusCode.Created))
            {
                Exception e = new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                LoggerService.Log.Instance.AddException(e);
                throw e;
            }

            #endregion

            string result = string.Empty;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        private string CredentialsAsBase64()
        {
            var bytes = Encoding.UTF8.GetBytes(Config.Username + ":" + Config.Password);
            return Convert.ToBase64String(bytes);
        }


    }
}
