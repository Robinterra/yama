namespace LearnCsStuf.CommandLines
{
    public interface ICommandLine
    {
        #region get/set

        string Key
        {
            get;
        }

        bool HasValue
        {
            get;
        }

        string HelpLine
        {
            get;
        }

        string Value
        {
            get;
            set;
        }

        #endregion get/set

        #region methods

        ICommandLine Check ( string command );

        #endregion methods

    }
}