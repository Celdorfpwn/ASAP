using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceControl
{
    public interface ISourceControl
    {
        ECommandStatus Pull();
        ECommandStatus Push(string remote = "", string branchName = "");
        ECommandStatus Branch(string branchName, bool delete = false, bool isRemote = false);
        ECommandStatus Checkout(string branchName);
        ECommandStatus Add(string fileName, bool all = false);
        ECommandStatus Commit(string message, bool allowEmpty = false);
        ECommandStatus Merge(string branchName);
        string GetCurrentBranch();
        string GetLastCommit();
    }
}
