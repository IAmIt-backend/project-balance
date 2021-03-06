﻿using RepositoryAbstraction;
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
        public async Task<bool> IsUserInGroup(ObjectId groupId, ObjectId userId)
        {
            return await _memberships.Find(m => m.GroupId == groupId && m.UserId == userId && m.IsVerified).AnyAsync();
        }
        public async Task<ICollection<Group>> GetAllGroupsOfUser(ObjectId userId)
        {
            var ids = (await _memberships.FindAsync(m => m.UserId == userId && m.IsVerified)).ToList().Select(m => m.GroupId).ToArray();
            var list = _groups.Find(g => ids.Contains(g.Id)).ToList();
            return list;
        }

        public async Task<bool> IsUserAdministrator(ObjectId groupId, ObjectId userId)
        {
            var membership = await _memberships.Find(m => m.GroupId == groupId && m.UserId == userId && m.IsVerified).FirstOrDefaultAsync();
            if (membership == null)
                return false;
            return (membership.MemberType == Role.Administrator);
        }

        public async Task VerifyInvitation(ObjectId groupId, ObjectId userId)
        {
            var update = new ObjectUpdateDefinition<UserGroupMembership>(new object());
            var membership = _memberships.Find(m => m.GroupId == groupId && m.UserId == userId).FirstOrDefault();
            if(membership!= null)
            await _memberships.UpdateOneAsync(m => m.GroupId == groupId && m.UserId == userId, update.Set(m => m.IsVerified, true));
        }

        public async Task RejectInvitation(ObjectId groupId, ObjectId userId)
        {
            await _memberships.DeleteOneAsync(m => m.GroupId == groupId && m.UserId == userId);
        }

        public async Task<ICollection<Group>> GetAllInvitations(ObjectId userId)
        {
            var ids = _memberships.Find(m => m.UserId == userId && !m.IsVerified).ToList().Select(m => m.GroupId).ToArray();
            return await _groups.Find(g => ids.Contains(g.Id)).ToListAsync();
        }

        public async Task<bool> IsUserInvitedInGroup(ObjectId groupId, ObjectId userId)
        {
            return await _memberships.Find(m => m.GroupId == groupId && m.UserId == userId && !m.IsVerified).AnyAsync();
        }
    }
}
