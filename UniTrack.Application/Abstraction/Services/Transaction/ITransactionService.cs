namespace UniTrack.Application.Abstraction.Services.Transaction
{
    public interface ITransactionService
    {
        public void Begin();
        public void Commit();
        public void Rollback();
    }
}
