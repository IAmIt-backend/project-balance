using RepositoryAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDB
{
    
    public class DbGroupRepository : IGroupRepository
    {
        private IMongoCollection<Group> _groups;
        private IMongoCollection<UserGroupMembership> _memberships;
        public DbGroupRepository()
        {
            var client = new MongoClient();
            var db = client.GetDatabase("balance");
            _groups = db.GetCollection<Group>(nameof(Group));
            _memberships = db.GetCollection<UserGroupMembership>(nameof(UserGroupMembership));    
        }
        public async Task AddGroup(Group group)
        {
            await Task.Run(() => { _groups.InsertOne(group); });
        }

        public async Task AddUserToGroup(Role memberType, ObjectId userId, ObjectId groupId)
        {
            await Task.Run(() => {
                _memberships.InsertOne(new UserGroupMembership {
                    MemberType = memberType,
                    UserId = userId,
                    GroupId = groupId
                });
            });
        }

        public async Task<ICollection<Group>> GetAllGroups()
        {
            return await Task.Run(() =>
            {
                var list = _groups.AsQueryable().ToList();
                return list;
            }
            );
        }

        public async Task<Group> GetGroup(ObjectId id)
        {
            return await _groups.Find(g => g.Id == id).FirstOrDefaultAsync();

        }
    }
}
