﻿using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Ixq.Security.Identity
{
    public abstract class UserStoreBase<TUser, TRole> :
        UserStore<TUser, TRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>,
        IUserStore<TUser>, IUserStore<TUser, string>, IDisposable
        where TUser : IdentityUser
        where TRole : IdentityRole
    {
        protected UserStoreBase(DbContext context) : base(context)
        {
        }
    }
}