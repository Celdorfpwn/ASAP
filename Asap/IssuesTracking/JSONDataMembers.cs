using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IssuesTracking;
using System.Collections;

namespace IssuesTracking
{
    [DataContract]
    public class SearchResult
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }

        [DataMember(Name = "issues")]
        public Issue[] Issues { get; set; }
    }

    [DataContract]
    public class CreateResult
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "self")]
        public string self { get; set; }
    }

    [DataContract]
    public class Issue
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "fields")]
        public Field Field { get; set; }

        public string URL { get; set; }
    }

    [DataContract]
    public class Field
    {
        [DataMember(Name = "summary")]
        public string Summary
        {
            get
            {
                return _summary;
            }
            set
            {
                _summary = value;
                ExtractUSName();
            }
        }

        public string USNumber
        {
            get; private set;
        }

        [DataMember(Name = "fixVersions")]
        public ItVersion[] FixVersions { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "issuetype")]
        public IssueType IssueType { get; set; }

        [DataMember(Name = "priority")]
        public Priority Priority { get; set; }

        public Severity Severity { get; set; }

        [DataMember(Name = "status")]
        public Status Status { get; set; }

        [DataMember(Name = "labels")]
        public string[] Labels { get; set; }

        [DataMember(Name = "resolution")]
        public Resolution Resolution { get; set; }

        [DataMember(Name = "assignee")]
        public Person Assignee { get; set; }

        [DataMember(Name = "reporter")]
        public Person Reporter { get; set; }

        [DataMember(Name = "comment")]
        public Comment Comment { get; set; }

        [DataMember(Name = "attachment")]
        public Attachment[] Attachment { get; set; }

        [DataMember(Name = "created")]
        public string CreatedDate
        {
            get
            {
                return _createdDate.Substring(0, 10);
            }
            set
            {
                _createdDate = value;
            }
        }

        [DataMember(Name = "updated")]
        public string UpdatedDate
        {
            get
            {
                return _updatedDate.Substring(0, 10);
            }
            set
            {
                _updatedDate = value;
            }
        }

        [DataMember(Name = "customfield_10322")]
        private object Customfield_10322
        {
            get
            {
                return _customfield_10322;
            }
            set
            {
                _customfield_10322 = value;
                Iteration = ParseToIteration(_customfield_10322);
            }
        }

        /// <summary>
        /// Parses the given object into iteration string
        /// </summary>
        /// <param name="_customfield_10322">The _customfield_10322.</param>
        private string ParseToIteration(object customfield_10322)
        {
            string iteration = string.Empty;

            if (customfield_10322 != null)
            {
                IEnumerable enumerable = customfield_10322 as IEnumerable;
                if (enumerable != null)
                {
                    foreach (object element in enumerable)
                    {
                        if (element != null)
                        {

                            int nameIndex = (element as String).IndexOf("name=");
                            int startDateIndex = (element as String).IndexOf(",startDate=");
                            if (nameIndex > 0 && startDateIndex > 0 && startDateIndex > nameIndex)
                            {
                                return (element as String).Substring(nameIndex + 5, startDateIndex - nameIndex - "name=".Length);
                            }
                        }
                    }
                }
            }


            return iteration;
        }

        /// <summary>
        /// Gets the iteration for this issue
        /// </summary>
        /// <value>
        /// The iteration.
        /// </value>
        public string Iteration
        {
            get; private set;
        }

        private string _createdDate;
        private string _updatedDate;
        private string _summary;
        private object _customfield_10322;

        private void ExtractUSName()
        {
            try
            {
                Match m = _usRegex.Match(_summary);
                if (m.Success)
                {
                    USNumber = m.Value.Substring(1, m.Value.Length - 2);
                }
                else
                {
                    USNumber = string.Empty;
                    return;
                }
            }
            catch (Exception)
            {
                USNumber = string.Empty;
            }
        }

        private static Regex _usRegex = new Regex(@"\[US\w{5}\]+", RegexOptions.Compiled);
    }

    [DataContract]
    public class Attachment
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "filename")]
        public string Filename { get; set; }

        [DataMember(Name = "author")]
        public Person Author { get; set; }

        [DataMember(Name = "mimeType")]
        public string MimeType { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "thumbnail")]
        public string Thumbnail { get; set; }

        [DataMember(Name = "size")]
        public int Size { get; set; }

        [DataMember(Name = "created")]
        public string CreatedDate
        {
            get
            {
                return _createdDate.Substring(0, 10);
            }
            set
            {
                _createdDate = value;
            }
        }

        private string _createdDate;
    }

    [DataContract]
    public class Comment
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }

        [DataMember(Name = "comments")]
        public Comments[] IssueComments { get; set; }
    }

    [DataContract]
    public class Comments
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "author")]
        public Person Author { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }

        [DataMember(Name = "updateAuthor")]
        public Person UpdateAuthor { get; set; }

        [DataMember(Name = "created")]
        public string CreatedDate
        {
            get
            {
                return _createdDate.Substring(0, 10);
            }
            set
            {
                _createdDate = value;
            }
        }

        [DataMember(Name = "updated")]
        public string UpdatedDate
        {
            get
            {
                return _updatedDate.Substring(0, 10);
            }
            set
            {
                _updatedDate = value;
            }
        }

        private string _createdDate;
        private string _updatedDate;
    }

    [DataContract]
    public class Person
    {
        [DataMember(Name = "emailAddress")]
        public string EmailAddress { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }
    }

    [DataContract]
    public class Resolution
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class Status
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class Priority
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "iconUrl")]
        public string IconURL { get; set; }
    }

    [DataContract]
    public class IssueType
    {
        [DataMember(Name = "id")]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                Type = (JiraIssueType)_id;
            }
        }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "iconUrl")]
        public string IconURL { get; set; }

        public JiraIssueType Type { get; private set; }

        private int _id;
    }

    [DataContract]
    public class ItVersion
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }


        [DataMember(Name = "released")]
        public string Released { get; set; }

        [DataMember(Name = "releaseDate")]
        public string ReleasedDate { get; set; }
    }
}
