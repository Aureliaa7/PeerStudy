using PeerStudy.Core.DomainEntities;
using PeerStudy.Core.Interfaces.Repositories;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Infrastructure.AppDbContext;
using PeerStudy.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace PeerStudy.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;
        private IRepository<User> usersRepository;
        private IRepository<Course> coursesRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IRepository<User> UsersRepository
        {
            get
            {
                if (usersRepository == null)
                {
                    usersRepository = new Repository<User>(dbContext);
                }
                return usersRepository;
            }
        }

        public IRepository<Course> CoursesRepository
        {
            get
            {
                if (coursesRepository == null)
                {
                    coursesRepository = new Repository<Course>(dbContext);
                }
                return coursesRepository;
            }
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
