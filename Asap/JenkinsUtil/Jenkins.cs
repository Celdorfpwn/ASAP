using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsService
{
    public class Jenkins : IJenkins
    {
        #region Constants

        /// <summary>
        /// http://SERVER:PORT
        /// </summary>
        private const String JENKINS_SERVER = @"http://192.168.9.206:8080";

        /// <summary>
        /// TheChromeEurope
        /// </summary>
        private const String JENKINS_JOB = "TheChromeEurope";

        /// <summary>
        /// /api/json
        /// </summary>
        private const String JENKINS_URL_SUFFIX = "/api/json";
        
        /// <summary>
        /// http://192.168.9.206:8080/job/TheChromeEurope
        /// </summary>
        private readonly String JENKINS_PREFIX = String.Format("{0}/job/{1}/", JENKINS_SERVER, JENKINS_JOB);

        private readonly String LAST_BUILD_URL = String.Format("{0}/job/{1}/lastBuild/api/json", JENKINS_SERVER, JENKINS_JOB); //"http://192.168.9.206:8080/job/TheChromeEurope/lastBuild/api/json";
        #endregion

        #region Constructor

        /// <summary>
        /// The Jenkins constructor
        /// </summary>
        public Jenkins()
        {

        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get the Jenkins build object for the given build number
        /// </summary>
        /// <param name="buildNo">Build no to query</param>
        /// <returns>Build object</returns>
        public Build GetBuild(int buildNo)
        {
            string retData = RunQuery("buildNo");
            return new Build(retData);
        }

        /// <summary>
        /// Start a new build from the given branch
        /// </summary>
        /// <param name="branch">The branch to build</param>
        /// <returns>Build object with the return</returns>
        public Build NewBuild(string branch)
        {
            string ret = RunQuery("build");
            if (!String.IsNullOrEmpty(ret))
            {
                throw new FormatException("Jenkins build may be not started correctly.");
            }
            System.Threading.Thread.Sleep(5000);

            return GetLatest();
        }

        /// <summary>
        /// Get the latest Jenkins job
        /// </summary>
        /// <returns></returns>
        public Build GetLatest()
        {
            string retData = RunQuery("lastBuild");
            return new Build(retData);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the Jenkins API url of the given build no.
        /// </summary>
        /// <param name="buildNo">Build no to get the URL. 0 returns Url for the latest build.</param>
        /// <returns>Jenkins build API url</returns>
        private string GetBuildUrl(int buildNo = 0)
        {
            if (buildNo == 0)
            {
                return JENKINS_SERVER + "/job/" + JENKINS_JOB + "/lastBuild" + JENKINS_URL_SUFFIX;
            }
            return JENKINS_SERVER + "/job/" + JENKINS_JOB + "/" + buildNo + JENKINS_URL_SUFFIX;
        }

        /// <summary>
        /// Execute Jenkins query
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns>JSON string response</returns>
        private String RunQuery(string argument = null, string data = null, string method = "GET")
        {
            string url = String.Empty;

            url = String.Format("{0}{1}{2}", JENKINS_PREFIX, argument, JENKINS_URL_SUFFIX);

            //System.Net.ServicePointManager.Expect100Continue = false;
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

            //string base64Credentials = "dmFsZW50aW4ubWl0YXJ1OkNlZmFjaWF6aTAx";
            //request.Headers.Add("Authorization", "Basic " + base64Credentials);

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            if (argument.Equals("build") && response.StatusCode == HttpStatusCode.Created)
            {
                // We should just receive a response with a status of "201 Created" if jenkins job was created.
                // Build no is get using /latest/ url
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

        #endregion
    }
}
