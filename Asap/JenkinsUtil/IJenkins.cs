using System;
namespace JenkinsService
{
    public interface IJenkins
    {
        /// <summary>
        /// Get the Jenkins build object for the given build number
        /// </summary>
        /// <param name="buildNo">Build no to query</param>
        /// <returns>Build object</returns>
        Build GetBuild(int buildNo);

        /// <summary>
        /// Start a new build from the given branch
        /// </summary>
        /// <param name="branch">The branch to build</param>
        /// <returns>Build object with the return</returns>
        Build NewBuild(string branch);

        /// <summary>
        /// Get the latest Jenkins job
        /// </summary>
        /// <returns></returns>
        Build GetLatest();

        /// <summary>
        /// Gets the current build associated with this instance
        /// </summary>
        /// <returns>Current Build object</returns>
        Build GetCurrentBuild();

        /// <summary>
        /// Build state change event
        /// </summary>
        event Action<Build> BuildStateChange;
    }
}
