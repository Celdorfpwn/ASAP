using System;
using System.Runtime.Serialization;

namespace FishEyeService
{
    [DataContract]
    public class ReviewData
    {        
        [DataMember(Name = "projectKey")]
        public string ProjectKey { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "author")]
        public UserData Author { get; set; }

        [DataMember(Name = "moderator")]
        public UserData Moderator { get; set; }

        [DataMember(Name = "creator")]
        public UserData Creator { get; set; }

        [DataMember(Name = "permaId")]
        public IdType PermaId { get; set; }

        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "allowReviewersToJoin")]
        public bool AllowReviewersToJoin { get; set; }

        [DataMember(Name = "createDate")]
        public DateTime CreateDate { get; set; }

        [DataMember(Name = "jiraIssueKey")]
        public string JiraIssueKey { get; set; }
    }

    [DataContract]
    public class ReviewArray
    {
        [DataMember(Name = "reviewData")]
        public ReviewData[] ReviewData { get; set; }
    }

    [DataContract]
    public class UserData
    {
        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "avatarUrl")]
        public string AvatarUrl { get; set; }
    }

    [DataContract]
    public class IdType
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
    }

    [DataContract]
    public class Error
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "stacktrace")]
        public string Stacktrace { get; set; }
    }

    [DataContract]
    public class CreateReview
    {
        [DataMember(Name = "reviewData")]
        public ReviewData ReviewData { get; set; }

        [DataMember(Name = "changesets")]
        public IdType[] ChangeSets { get; set; }

        [DataMember(Name = "repository")]
        public string Repository { get; set; }
    }

    [DataContract]
    public class ReviewItem
    {
        [DataMember(Name = "permId")]
        public IdType PermId { get; set; }

        [DataMember(Name = "participants")]
        public Participant[] Participants { get; set; }

        [DataMember(Name = "repositoryName")]
        public string RepositoryName { get; set; }

        [DataMember(Name = "fromPath")]
        public string FromPath { get; set; }

        [DataMember(Name = "fromRevision")]
        public string FromRevision { get; set; }

        [DataMember(Name = "fromContentUrl")]
        public string FromContentUrl { get; set; }

        [DataMember(Name = "toPath")]
        public string ToPath { get; set; }

        [DataMember(Name = "toRevision")]
        public string ToRevision { get; set; }

        [DataMember(Name = "toContentUrl")]
        public string ToContentUrl { get; set; }

        [DataMember(Name = "fileType")]
        public string FileType { get; set; }

        [DataMember(Name = "commitType")]
        public string CommitType { get; set; }

        [DataMember(Name = "autorName")]
        public string AuthorName { get; set; }

        [DataMember(Name = "showAsDiff")]
        public bool ShowAsDiff { get; set; }

        [DataMember(Name = "commitDate")]
        public string CommitDate { get; set; }
    }

    [DataContract]
    public class Participant
    {
        [DataMember(Name = "user")]
        public UserData User { get; set; }

        [DataMember(Name = "completed")]
        public bool Completed { get; set; }
    }

    [DataContract]
    public class Comment
    {
        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "draft")]
        public bool Draft { get; set; }

        [DataMember(Name = "deleted")]
        public bool Deleted { get; set; }

        [DataMember(Name = "defectRaised")]
        public bool DefectRaised { get; set; }

        [DataMember(Name = "readStatus")]
        public string readStatus { get; set; }

        [DataMember(Name = "user")]
        public UserData User { get; set; }

        [DataMember(Name = "createDate")]
        public string CreateDate { get; set; }

        [DataMember(Name = "messageAsHtml")]
        public string MessageAsHtml { get; set; }

        [DataMember(Name = "reviewItemId")]
        public IdType ReviewItemId { get; set; }

        [DataMember(Name = "fromLineRange")]
        public string FromLineRange { get; set; }

        [DataMember(Name = "toLineRange")]
        public string ToLineStrange { get; set; }

    }

    [DataContract]
    public class NewComment
    {
        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "draft")]
        public bool Draft { get; set; }

        [DataMember(Name = "deleted")]
        public bool Deleted { get; set; }

        [DataMember(Name = "defectRaised")]
        public bool DefectRaised { get; set; }

        [DataMember(Name = "defectApproved")]
        public bool DefectApproved { get; set; }

        [DataMember(Name = "permaId")]
        public IdType PermaId { get; set; }

        [DataMember(Name = "permId")]
        public IdType PermId { get; set; }

        [DataMember(Name = "parrentCommentId")]
        public IdType ParentCommentId { get; set; }
    }

    [DataContract]
    public class MetricsData
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "configVersion")]
        public int ConfigVersion { get; set; }

        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "defaultValue")]
        public MetricValue DefaultValue { get; set; }

        [DataMember(Name = "values")]
        public MetricValue[] Values { get; set; }
    }

    [DataContract]
    public class MetricValue
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "value")]
        public int Value { get; set; }
    }

    [DataContract]
    public class Reviewer
    {
        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "avatarUrl")]
        public string AvatarUrl { get; set; }

        [DataMember(Name = "completed")]
        public bool Completed { get; set; }
    }

    [DataContract]
    public class ReviewerArray
    {
        [DataMember(Name = "reviewer")]
        public Reviewer[] Reviewer { get; set; }
    }
}
