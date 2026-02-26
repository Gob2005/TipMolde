using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipMolde.Core.Models;
using TipMolde.Core.Interface;

namespace TipMolde.Core.Interface.IUser
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<IEnumerable<User>> SearchByNameAsync(string searchTerm);

        Task<User?> GetByEmailAsync(string email);
    }
}
