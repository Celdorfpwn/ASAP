using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsService
{
    public class Jenkins : IJenkins
    {
        #region Constants
        private const String JENKINS_SERVER = @"http://192.168.9.206:8080";
        private const String JENKINS_JOB = "TheChromeEurope";
        private const String JENKINS_URL_SUFFIX = "/api/json";
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

        #endregion
    }
}
