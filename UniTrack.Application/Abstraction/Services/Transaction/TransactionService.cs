using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace UniTrack.Application.Abstraction.Services.Transaction
{
    public class TransactionService : ITransactionService
    {
        private readonly DbContext dbContext;
        private IDbContextTransaction? transaction;
        public TransactionService(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void Begin()
        {
            transaction = dbContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            transaction?.Commit();
            transaction?.Dispose();
        }

        public void Rollback()
        {
            transaction?.Rollback();

        }
    }
}
