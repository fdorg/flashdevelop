namespace CodeRefactor.Provider
{
    interface IRenameHelper
    {
        bool IncludeComments { get; }
        bool IncludeStrings { get; }
    }
}
