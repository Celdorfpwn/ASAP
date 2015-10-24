using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsService
{
    internal class Caus
    {
        public string shortDescription { get; set; }
        public object userId { get; set; }
        public string userName { get; set; }
    }

    internal class Branch
    {
        public string SHA1 { get; set; }
        public string name { get; set; }
    }

    internal class Marked
    {
        public string SHA1 { get; set; }
        public Branch[] branch { get; set; }
    }

    internal class Branch2
    {
        public string SHA1 { get; set; }
        public string name { get; set; }
    }

    internal class Revision
    {
        public string SHA1 { get; set; }
        public Branch2[] branch { get; set; }
    }

    internal class RefsRemotesOriginMaster
    {
        public int buildNumber { get; set; }
        public object buildResult { get; set; }
        public Marked marked { get; set; }
        public Revision revision { get; set; }
    }

    internal class BuildsByBranchName
    {
        public RefsRemotesOriginMaster branchName { get; set; }
    }

    internal class Branch3
    {
        public string SHA1 { get; set; }
        public string name { get; set; }
    }

    internal class LastBuiltRevision
    {
        public string SHA1 { get; set; }
        public Branch3[] branch { get; set; }
    }

    internal class Action
    {
        public Caus[] causes { get; set; }
        public BuildsByBranchName buildsByBranchName { get; set; }
        public LastBuiltRevision lastBuiltRevision { get; set; }
        public string[] remoteUrls { get; set; }
        public string scmName { get; set; }
    }

    internal class Author
    {
        public string absoluteUrl { get; set; }
        public string fullName { get; set; }
    }

    internal class Path
    {
        public string editType { get; set; }
        public string file { get; set; }
    }

    internal class Item
    {
        public string[] affectedPaths { get; set; }
        public string commitId { get; set; }
        public long timestamp { get; set; }
        public Author author { get; set; }
        public string comment { get; set; }
        public string date { get; set; }
        public string id { get; set; }
        public string msg { get; set; }
        public Path[] paths { get; set; }
    }

    internal class ChangeSet
    {
        public Item[] items { get; set; }
        public string kind { get; set; }
    }

    internal class Culprit
    {
        public string absoluteUrl { get; set; }
        public string fullName { get; set; }
    }

    internal class JkBuild
    {
        public Action[] actions { get; set; }
        public object[] artifacts { get; set; }
        public bool building { get; set; }
        public object description { get; set; }
        public int duration { get; set; }
        public int estimatedDuration { get; set; }
        public object executor { get; set; }
        public string fullDisplayName { get; set; }
        public string id { get; set; }
        public bool keepLog { get; set; }
        public int number { get; set; }
        public string result { get; set; }
        public long timestamp { get; set; }
        public string url { get; set; }
        public string builtOn { get; set; }
        public ChangeSet changeSet { get; set; }
        public Culprit[] culprits { get; set; }
    }
}
