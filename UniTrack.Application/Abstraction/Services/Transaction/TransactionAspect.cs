using Castle.DynamicProxy;

namespace UniTrack.Application.Abstraction.Services.Transaction
{
    public class TransactionAspect : IInterceptor
    {
        private readonly ITransactionService transactionService;
        public TransactionAspect(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }
        public void Intercept(IInvocation invocation)
        {
            try
            {
                transactionService.Begin();
                invocation.Proceed();
                transactionService.Commit();
            }
            catch (Exception)
            {
                transactionService.Rollback();
                throw;
            }
        }
    }
}
