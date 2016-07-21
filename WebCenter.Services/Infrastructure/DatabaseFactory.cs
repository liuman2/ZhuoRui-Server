namespace WebCenter.Services.Infrastructure
{
    public class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private ApplicationDb _dataContext;

        public ApplicationDb GetDbSession()
        {
            return _dataContext ?? (_dataContext = new ApplicationDb());
        }
        protected override void DisposeCore()
        {
            if (_dataContext != null)
                _dataContext.Dispose();
        }
    }
}