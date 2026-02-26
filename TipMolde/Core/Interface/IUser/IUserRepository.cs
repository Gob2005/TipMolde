using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.User
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<IEnumerable<User>> SearchByNameAsync(string searchTerm);
    }
}
