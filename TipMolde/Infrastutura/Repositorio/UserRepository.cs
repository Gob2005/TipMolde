using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipMolde.Core.Interface.IUser;
using TipMolde.Core.Models;
using TipMolde.Infrastutura.DB;

namespace TipMolde.Infrastutura.Repositorio
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public Task<IEnumerable<User>> SearchByNameAsync(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
