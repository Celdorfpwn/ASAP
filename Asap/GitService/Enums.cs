namespace GitService
{
    public enum CommandStatus
    {
        UpToDate = 0,
        FastForward = 1,
        NonFastForward = 2,
        Conflicts = 3,
        FAIL,
        OK,
        BranchNotFound,
        BranchAlreadyExists
    }
}