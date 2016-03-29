using Balance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryAbstraction
{
    public interface IGroupRepository
    {
        Task<bool> AddGroup(GroupModel groupModel);
        Task<bool> AddUserToGroup(string userEmail, string GroupName, string GroupDescription);
        Task<GroupModel> GetGroup(string GroupName, string GroupDescription);
        Task<ICollection<GroupModel>> GetAllGroups();
    }
}
