namespace DockerTest.Data.Infrastructure.Interfaces
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork GetUoW();
    }
}