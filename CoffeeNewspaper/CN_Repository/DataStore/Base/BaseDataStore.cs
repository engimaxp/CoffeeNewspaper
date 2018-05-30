namespace CN_Repository
{
    public class BaseDataStore
    {
        #region Protected Members

        /// <summary>
        ///     The database context for the client data store
        /// </summary>
        protected CNDbContext mDbContext;

        #endregion

        #region Constructor

        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <param name="dbContext">The database to use</param>
        public BaseDataStore(CNDbContext dbContext)
        {
            // Set local member
            mDbContext = dbContext;
        }

        #endregion
    }
}
