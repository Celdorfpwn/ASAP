using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceControl
{
    public enum ECommandStatus
    {
        UpToDate = 0,       //merge status
        FastForward = 1,    //merge status
        NonFastForward = 2, //merge status
        Conflicts = 3,      //merge status
        FAIL,
        OK,
        BranchNotFound,
        BranchAlreadyExists
    }
}
