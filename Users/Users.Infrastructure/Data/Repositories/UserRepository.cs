using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;
using static Users.Infrastructure.Data.UserContext;

namespace Users.Infrastructure.Data.Repositories
{
    public class UserRepository:RepositoryBase<User>,IUserRepository
    {
        public UserRepository(UserContext context) : base(context) { }
    }
}
