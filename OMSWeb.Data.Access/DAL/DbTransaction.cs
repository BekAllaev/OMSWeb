using Microsoft.EntityFrameworkCore.Storage;

namespace OMSWeb.Data.Access.DAL
{
    internal class DbTransaction : ITransaction
    {
        private IDbContextTransaction dbContextTransaction;

        public DbTransaction(IDbContextTransaction dbContextTransaction)
        {
            this.dbContextTransaction = dbContextTransaction;
        }
    }
}