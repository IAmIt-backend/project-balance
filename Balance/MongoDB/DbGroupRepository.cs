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
            await _groups.InsertOneAsync(group); 
        }

        public async Task AddPayment(ObjectId groupId, Payment payment)
        {
            var payments = _groups.Find(g => g.Id == groupId).First().Payments;
            payments.Add(payment);
            var update = new ObjectUpdateDefinition<Group>(new object());
            await _groups.UpdateOneAsync(g => g.Id == groupId, update.Set(g => g.Payments, payments));
        }

        public async Task AddUserToGroup(Role memberType, ObjectId userId, ObjectId groupId)
        {
            await _memberships.InsertOneAsync(new UserGroupMembership {
                    MemberType = memberType,
                    UserId = userId,
                    GroupId = groupId
                });
            
        }


        public async Task<ICollection<Payment>> GetAllPayments(ObjectId groupId)
        {
            return (await _groups.Find(g => g.Id == groupId).FirstOrDefaultAsync()).Payments;
        }

        public async Task<ICollection<ObjectId>> GetAllUsersInGroup(ObjectId groupId)
        {
            return  (await _memberships.FindAsync(m => m.GroupId == groupId)).ToList().Select(m => m.UserId).ToList();
        }

        public async Task<Group> GetGroup(ObjectId id)
        {
            return await _groups.Find(g => g.Id == id).FirstOrDefaultAsync();

        }

        public async Task<bool> IsUserInGroup(ObjectId userId, ObjectId groupId)
        {
            return await _memberships.Find(m => m.GroupId == groupId && m.UserId == userId).AnyAsync();
        }
    }
}
