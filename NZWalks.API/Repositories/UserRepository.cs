﻿using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        

        private readonly NZWalksDbContext _context;
        public UserRepository(NZWalksDbContext context)
        {
            _context = context;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == username.ToLower()
            && x.Password == password);

            if(user == null)
            {
                return null;
            }

            var userRoles = await _context.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();

            if (userRoles.Any())
            {
                user.Roles = new List<string>();
                foreach (var userRole in userRoles)
                {
                    var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == userRole.RoleId);
                    if (role != null)
                    {
                        user.Roles.Add(role.Name);
                    }
                }
            }

            user.Password = null;

            return user;
        }
    }
}
