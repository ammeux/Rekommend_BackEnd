﻿using Rekommend.IDP.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rekommend.IDP.Services
{
    public interface ILocalUserService
    {
        Task<bool> ValidateCredentialsAsync(string userName, string password);
        Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectAsync(string subject);
        Task<User> GetUserByUserNameAsync(string userName);
        Task<User> GetUserBySubjectAsync(string subject);
        void AddUser(User userToAdd, string password);
        Task<bool> IsUserActive(string subject);
        Task<bool> ActivateUser(string securityCode);
        Task<bool> SaveChangesAsync();
        Task<string> InitiatePasswordResetRequest(string email);
        Task<bool> SetPassword(string securityCode, string password);
        Task<User> GetUserByExternalProvider(string provider, string providerIdentityKey);
        User ProvisionUserFromExternalIdentity(string provider, string providerIdentityKey, IEnumerable<Claim> claims, string userName = null);
        //Task AddExternalProviderToUser(
        //    string subject, 
        //    string provider,
        //    string providerIdentityKey);
    }
}
