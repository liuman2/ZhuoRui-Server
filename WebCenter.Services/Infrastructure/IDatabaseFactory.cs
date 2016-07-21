namespace WebCenter.Services.Infrastructure
{
    public interface IDatabaseFactory
    {
         ApplicationDb GetDbSession();

    }
}