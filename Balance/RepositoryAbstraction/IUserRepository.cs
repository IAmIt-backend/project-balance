using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryAbstraction
{
    public interface IUserRepository
    {
        Task<ICollection<ObjectId>> GetAllGroupsOfUser(ObjectId userId);
    }
}
