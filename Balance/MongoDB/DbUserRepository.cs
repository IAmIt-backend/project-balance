using RepositoryAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Entities;

namespace MongoDB
{
    public class DbUserRepository : IUserRepository
    {
        private IMongoCollection<Group> _groups;
        private IMongoCollection<UserGroupMembership> _memberships;
        public DbUserRepository()
        {
            var client = new MongoClient();
            var db = client.GetDatabase("balance");
            _groups = db.GetCollection<Group>(nameof(Group));
            _memberships = db.GetCollection<UserGroupMembership>(nameof(UserGroupMembership));
        }
        public async Task<bool> IsUserInGroup(ObjectId userId, ObjectId groupId)
        {
            return await _memberships.Find(m => m.GroupId == groupId && m.UserId == userId).AnyAsync();
        }
        public async Task<ICollection<Group>> GetAllGroupsOfUser(ObjectId userId)
        {
            var ids = (await _memberships.FindAsync(m => m.UserId == userId)).ToList().Select(m => m.GroupId).ToArray();
            var list = _groups.Find(g => ids.Contains(g.Id)).ToList();
            return list;
        }

        public async Task<bool> IsUserAdministrator(ObjectId userId, ObjectId groupId)
        {
            var membership = await _memberships.Find(m => m.GroupId == groupId && m.UserId == userId).FirstOrDefaultAsync();
            if (membership == null)
                return false;
            return (membership.MemberType == Role.Administrator);
        }
    }
}
