using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Interfaces.Repositories;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<User> UsersRepository { get; }

        Task SaveChangesAsync();
    }
}
