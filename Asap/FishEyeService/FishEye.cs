using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace FishEyeService
{
    public class FishEye 
    {   
        public readonly string FISHEYE_PROJECT = "SOCHEU";
    //    private readonly string BASE_FISHEYE_URL = "https://fisheye.softvision.ro/rest-service-fe/";
        private readonly string BASE_CRUCIBLE_URL = "https://fisheye.softvision.ro/rest-service/";
    //    private readonly string BASE_FECRU_URL = "https://fisheye.softvision.ro/rest-service-fecru/";

        private string _userName = "";
        private string _password = "";

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public void CreateReview(CreateReview newReview)
        {
            string query = "reviews-v1";

            string data = Newtonsoft.Json.JsonConvert.SerializeObject(newReview);

            string response = RunQuery(query, data, "POST");           
        }

        /// <summary>
        /// Get the review with the provided id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Review</returns>
        public ReviewData GetReview(int id)
        {
            string query = string.Format("reviews-v1/{0}-{1}", FISHEYE_PROJECT, id);
            string response = RunQuery(query);

            var review = Newtonsoft.Json.JsonConvert.DeserializeObject<ReviewData>(response);
            return review;
        }

        /// <summary>
        /// Get the review with the provided permaId.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Review</returns>
        public ReviewData GetReview(string id)
        {
            string query = string.Format("reviews-v1/{0}", id);
            string response = RunQuery(query);

            var review = Newtonsoft.Json.JsonConvert.DeserializeObject<ReviewData>(response);
            return review;
        }

        /// <summary>
        /// Returns an array of all reviews out for review for the current user.
        /// </summary>
        /// <returns>Reviews array</returns>
        public ReviewArray GetReviewsOutForReview()
        {
            string query = string.Format("reviews-v1/filter/outForReview");
            string response = RunQuery(query);

            var reviews = Newtonsoft.Json.JsonConvert.DeserializeObject<ReviewArray>(response);

            return reviews;
        }

        /// <summary>
        /// Returns an array of all reviewers who completed the review with permaID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reviewers array</returns>
        public ReviewerArray GetCompletedReviewers(string id)
        {
            string query = string.Format("reviews-v1/{0}/reviewers/completed", id);
            string response = RunQuery(query);

            var reviewers = Newtonsoft.Json.JsonConvert.DeserializeObject<ReviewerArray>(response);

            return reviewers;
        }

        /// <summary>
        /// Returns an array of all reviewers who completed the review with ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reviewers array</returns>
        public ReviewerArray GetCompletedReviewers(int id)
        {
            string query = string.Format("reviews-v1/{0}-{1}/reviewers/completed",FISHEYE_PROJECT, id);
            string response = RunQuery(query);

            var reviewers = Newtonsoft.Json.JsonConvert.DeserializeObject<ReviewerArray>(response);

            return reviewers;
        }

        /// <summary>
        /// Returns an array of all reviewers who did not complete the review with permaID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reviewers array</returns>
        public ReviewerArray GetUncompletedReviewers(string id)
        {
            string query = string.Format("reviews-v1/{0}/reviewers/uncompleted", id);
            string response = RunQuery(query);

            var reviewers = Newtonsoft.Json.JsonConvert.DeserializeObject<ReviewerArray>(response);

            return reviewers;
        }

        /// <summary>
        /// Returns an array of all reviewers who did not complete the review with ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Reviewers array</returns>
        public ReviewerArray GetUncompletedReviewers(int id)
        {
            string query = string.Format("reviews-v1/{0}-{1}/reviewers/uncompleted", FISHEYE_PROJECT, id);
            string response = RunQuery(query);

            var reviewers = Newtonsoft.Json.JsonConvert.DeserializeObject<ReviewerArray>(response);

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

            url = string.Format("{0}{1}", BASE_CRUCIBLE_URL, argument);

            ServicePointManager.Expect100Continue = false;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = method;

            string base64Credentials = Base64Encode(_userName + ":" + _password);
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


            HttpWebResponse response = request.GetResponse() as HttpWebResponse;


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

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
