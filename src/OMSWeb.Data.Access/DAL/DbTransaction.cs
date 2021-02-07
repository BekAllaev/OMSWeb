using Microsoft.EntityFrameworkCore.Storage;

namespace OMSWeb.Data.Access.DAL
{
    public class DbTransaction : ITransaction
    {
        #region Declarations
        private IDbContextTransaction _efContextTransaction;
        #endregion

        #region Constructor
        public DbTransaction(IDbContextTransaction dbContextTransaction)
        {
            _efContextTransaction = dbContextTransaction;
        }
        #endregion

        #region ITransaction implementation
        public void Commit()
        {
            _efContextTransaction.Commit();
        }

        public void Dispose()
        {
            _efContextTransaction.Dispose();
        }

        public void RollBack()
        {
            _efContextTransaction.Rollback();
        }
        #endregion
    }
}