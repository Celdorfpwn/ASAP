using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JiraService
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
        public string Summary { get; set; }

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
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "iconUrl")]
        public string IconURL { get; set; }
    }

    [DataContract]
    public class Version
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
