﻿using Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryAbstraction
{
    public interface IGroupRepository
    {
        Task AddGroup(Group group);
        Task AddUserToGroup(Role memberType, ObjectId userId, ObjectId groupId);
        Task AddPayment(ObjectId groupId, Payment payment);
        Task<bool> IsUserInGroup(ObjectId userId, ObjectId groupId);
        Task<Group> GetGroup(ObjectId id);
        Task<ICollection<Group>> GetAllGroups();
        Task<ICollection<Payment>> GetAllPayments(ObjectId groupId);
    }
}
