using CodeReview;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using ToolsConfiguration;

namespace FishEyeService
{
    public class FishEye : ICodeReview
    {   


        private ICodeReviewConfig Config { get; set; }

        public FishEye(ICodeReviewConfig config)
        {
            Config = config;
        }


        public ReviewData CreateReview(string commit, string name, string description, string summary, string jiraKey)
        {
            if(CheckChangeset(commit))
            {

         
            var newReview = new CreateReview();
            newReview.ChangeSets = new ChangeSet { 
                    ChangeSets = new IdType[] { 
                    new IdType { Id = commit} 
                },
                    Repository = Config.Repository 
            };
              
            var user = FishEyeUser;

            newReview.ReviewData = new ReviewData
            {
                Author = user,
                Moderator = user,
                Creator = user,
                AllowReviewersToJoin = true,
                Name = name,
                ProjectKey = Config.Project,
                Description = description,
                State = "Review",
                PermaId = new IdType { Id = ""},
                Type = "REVIEW",
                CreateDate = CurrentDate,
                JiraIssueKey = jiraKey,
                Summary = summary,
            };


            return JsonConvert.DeserializeObject<ReviewData>(CreateReview(newReview));
            }
            else
            {
                return null;
            }
        }

        private bool CheckChangeset(string commit)
        {
            string query = string.Format("revisionData-v1/changeset/{0}/{1}", Config.Repository, commit);
            var result = RunFeQuery(query);
            return result.Contains(commit);
        }

        public int CountReviewers(string id)
        {
            return GetCompletedReviewers(id).Reviewer.Length;
        }

        private string CreateReview(CreateReview newReview)
        {
            string query = "reviews-v1";

           string data = JsonConvert.SerializeObject(newReview);

           return RunQuery(query, data, "POST");           
        }

        /// <summary>
        /// Get the review with the provided id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Review</returns>
        private ReviewData GetReview(int id)
        {
            string query = string.Format("reviews-v1/{0}-{1}", Config.Project, id);
            string response = RunQuery(query);

            var review = JsonConvert.DeserializeObject<ReviewData>(response);
            return review;
        }

        /// <summary>
        /// Get the review with the provided permaId.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Review</returns>
        private ReviewData GetReview(string id)
        {
            string query = string.Format("reviews-v1/{0}", id);
            string response = RunQuery(query);
            return JsonConvert.DeserializeObject<ReviewData>(response);
        }

        /// <summary>
        /// Returns an array of all reviews out for review for the current user.
        /// </summary>
        /// <returns>Reviews array</returns>
        private ReviewArray GetReviewsOutForReview()
        {
            string query = string.Format("reviews-v1/filter/outForReview");
            string response = RunQuery(query);
            var reviews = JsonConvert.DeserializeObject<ReviewArray>(response);

            return reviews;
        }

        /// <summary>
        /// Returns an array of all reviewers who completed the review with permaID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reviewers array</returns>
        private ReviewerArray GetCompletedReviewers(string id)
        {
            string query = string.Format("reviews-v1/{0}/reviewers/completed", id);
            string response = RunQuery(query);

            var reviewers = JsonConvert.DeserializeObject<ReviewerArray>(response);

            return reviewers;
        }

        /// <summary>
        /// Returns an array of all reviewers who completed the review with ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reviewers array</returns>
        private ReviewerArray GetCompletedReviewers(int id)
        {
            string query = string.Format("reviews-v1/{0}-{1}/reviewers/completed",Config.Project, id);
            string response = RunQuery(query);

            var reviewers = JsonConvert.DeserializeObject<ReviewerArray>(response);

            return reviewers;
        }

        /// <summary>
        /// Returns an array of all reviewers who did not complete the review with permaID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reviewers array</returns>
        private ReviewerArray GetUncompletedReviewers(string id)
        {
            string query = string.Format("reviews-v1/{0}/reviewers/uncompleted", id);
            string response = RunQuery(query);

            var reviewers = JsonConvert.DeserializeObject<ReviewerArray>(response);

            return reviewers;
        }

        /// <summary>
        /// Returns an array of all reviewers who did not complete the review with ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reviewers array</returns>
        private ReviewerArray GetUncompletedReviewers(int id)
        {
            string query = string.Format("reviews-v1/{0}-{1}/reviewers/uncompleted", Config.Project, id);
            string response = RunQuery(query);

            var reviewers = JsonConvert.DeserializeObject<ReviewerArray>(response);

            return reviewers;
        }

        /// <summary>
        /// Runs http request query based on arguments.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <returns>Response string.</returns>
        private string RunQuery(string argument = null, string data = null, string method = "GET")
        {
            string url = string.Empty;

            url = string.Format("{0}{1}", Config.BaseUrl, argument);

            ServicePointManager.Expect100Continue = false;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = method;

            string base64Credentials = Base64Encode(Config.Username + ":" + Config.Password);
            request.Headers.Add("Authorization", "Basic " + base64Credentials);

            if (data != null)
            {
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytes = encoding.GetBytes(data);
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
            }
            HttpWebResponse response = null;
            string pageContent = String.Empty;
            try
            {

                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException wex)
            {
                pageContent = new StreamReader(wex.Response.GetResponseStream())
                                      .ReadToEnd();
                
            }


            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
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

        private string RunFeQuery(string argument = null, string data = null, string method = "GET")
        {
            string url = string.Empty;

            url = string.Format("{0}{1}", Config.BaseUrl.Replace("rest-service", "rest-service-fe"), argument);

            ServicePointManager.Expect100Continue = false;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = method;

            string base64Credentials = Base64Encode(Config.Username + ":" + Config.Password);
            request.Headers.Add("Authorization", "Basic " + base64Credentials);

            if (data != null)
            {
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytes = encoding.GetBytes(data);
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
            }
            HttpWebResponse response = null;
            string pageContent = String.Empty;
            try
            {

                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException wex)
            {
                pageContent = new StreamReader(wex.Response.GetResponseStream())
                                      .ReadToEnd();

            }


            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
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



        private UserData FishEyeUser
        {
            get
            {
                return JsonConvert.DeserializeObject<User>(RunQuery("users-v1?username=" + Config.Username)).Data[0];
            }
        }

        private string CurrentDate
        {
            get
            {
                var splitt = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddThh:mm:sszzz").Split('+');

                return splitt[0]+".123" + "+" + splitt[1].Replace(":", "");
            }
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

    }
}
