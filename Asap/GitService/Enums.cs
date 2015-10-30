namespace GitService
{
    /// <summary>
    /// Enum to represent different statuses for git commands
    /// </summary>
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