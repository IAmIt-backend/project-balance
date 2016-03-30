﻿using Entities;
using MongoDB.Bson;
using MVCModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelAbstractions
{
    public interface IService
    {
        //Task<User> GetUser(string email);
        Task AddPayment(ObjectId groupId, decimal value, ObjectId userId);
        Task<ICollection<PaymentListItemModel>> GetAllPayments(ObjectId groupId);
        Task AddGroup(AddGroupModel groupModel, ObjectId userId);
        Task AddUserToGroup(ObjectId userId, ObjectId groupId);
        Task<ICollection<UserListItemModel>> GetAllUsersInGroup(ObjectId groupId);
        Task<AddGroupModel> GetGroup(ObjectId id);
        Task<ICollection<GroupListItemModel>> GetAllGroupsOfUser(ObjectId userId);
        Task<bool> IsUserAdministrator(ObjectId userId, ObjectId groupId);
    }
}