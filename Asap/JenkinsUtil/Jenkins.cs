using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

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
        /// Job config file
        /// </summary>
        private const String JENKINS_CONFIG_FILE = "config.xml";

        /// <summary>
        /// http://192.168.9.206:8080/job/TheChromeEurope
        /// </summary>
        private readonly String JENKINS_PREFIX = String.Format("{0}/job/{1}/", JENKINS_SERVER, JENKINS_JOB);

        /// <summary>
        /// Jenkins URL to the last build
        /// </summary>
        private readonly String LAST_BUILD_URL = String.Format("{0}/job/{1}/lastBuild/api/json", JENKINS_SERVER, JENKINS_JOB); //"http://192.168.9.206:8080/job/TheChromeEurope/lastBuild/api/json";

        /// <summary>
        /// Jenkins job config file URL
        /// </summary>
        private readonly String JENKINS_CONFIG_URL = String.Format("{0}/job/{1}/{2}", JENKINS_SERVER, JENKINS_JOB, JENKINS_CONFIG_FILE);
        #endregion

        #region Fields

        /// <summary>
        /// Background worker that checks for build completion
        /// </summary>
        private BackgroundWorker bw = new BackgroundWorker();

        /// <summary>
        /// Current build associated with this instance
        /// </summary>
        private Build _currentBuild;

        #endregion

        #region Constructor

        /// <summary>
        /// The Jenkins constructor
        /// </summary>
        public Jenkins()
        {
            _currentBuild = new Build();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.ProgressChanged += bw_ProgressChanged;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the current build associated with this instance
        /// </summary>
        /// <returns>Current Build object</returns>
        public Build GetCurrentBuild()
        {
            return _currentBuild;
        }

        /// <summary>
        /// Get the Jenkins build object for the given build number
        /// </summary>
        /// <param name="buildNo">Build no to query</param>
        /// <returns>Build object</returns>
        public Build GetBuild(int buildNo)
        {
            string retData = RunQuery(buildNo.ToString());
            _currentBuild = new Build(retData);
            return _currentBuild;
        }

        /// <summary>
        /// Start a new build from the given branch
        /// </summary>
        /// <param name="branch">The branch to build</param>
        /// <returns>Build object with the return</returns>
        public Build NewBuild(string branch)
        {
            if (!String.IsNullOrWhiteSpace(branch))
            {
                SwitchBranch(branch);
            }
            Build previous = GetLatest();

            Build current = new Build();
            current.Number = previous.Number + 1;

            string ret = RunQuery("build");
            if (!String.IsNullOrEmpty(ret))
            {
                throw new FormatException("Jenkins build may be not started correctly.");
            }

            bw.RunWorkerAsync(current);
            FireBuildStateChange(current);
            return current;
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

        /// <summary>
        /// Swith the current Jenkins build branch
        /// </summary>
        /// <param name="newBranch">The new branch to build</param>
        public void SwitchBranch(string newBranch)
        {
            string configXML = RunQuery(JENKINS_CONFIG_URL);

            XmlDocument conf = new XmlDocument();
            conf.LoadXml(configXML);

            XmlNode branchName = conf.DocumentElement.SelectSingleNode("/project/scm/branches/hudson.plugins.git.BranchSpec/name");
            branchName.InnerText = "*/" + newBranch;

            RunQuery(JENKINS_CONFIG_URL, conf.InnerXml, "POST");
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

            if (argument.EndsWith(JENKINS_CONFIG_URL))
            {
                url = argument;
            }

            //System.Net.ServicePointManager.Expect100Continue = false;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Method = method;

            if (data != null)
            {
                if (String.IsNullOrWhiteSpace(method))
                {
                    request.Method = "PUT";
                }
                else
                {
                    request.Method = method;
                }
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

        #region Events

        /// <summary>
        /// Build state change event
        /// </summary>
        public event Action<Build> BuildStateChange;

        /// <summary>
        /// Fire the Build state change
        /// </summary>
        /// <param name="build">Build object that just changed its state</param>
        private void FireBuildStateChange(Build build)
        {
            if (BuildStateChange != null)
            {
                BuildStateChange(build);
            }
        }

        /// <summary>
        /// Do work event handler for the background worker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(10000);
            do
            {
                
                GetBuild((e.Argument as Build).Number);
                if (_currentBuild.State == BuildState.Failed)
                {
                    e.Result = BuildState.Failed;
                    break;
                }
                bw.ReportProgress(10, BuildState.Building);
                Thread.Sleep(5000);
            } while (_currentBuild.State == BuildState.None || _currentBuild.State == BuildState.Building);

            e.Result = BuildState.Success;
        }

        /// <summary>
        /// Background worker completed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FireBuildStateChange(_currentBuild);
        }

        /// <summary>
        /// Background worker progress changed event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FireBuildStateChange(_currentBuild);
        }

        #endregion
    }
}
