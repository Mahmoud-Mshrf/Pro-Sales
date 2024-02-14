using CRM.Core.Interfaces;
using CRM.Core.Models;
using CRM.Core;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBaseRepository<VerificationCode> VerificationCodes { get; private set; }
        public IBaseRepository<Source> Sources { get; private set; }
        public IBaseRepository<Interest> Interests { get; private set; }
        public IBaseRepository<Customer> Customers { get; private set; }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public RoleManager<IdentityRole> RoleManager { get; private set; }

        public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            VerificationCodes = new BaseRepository<VerificationCode>(_context);
            Sources = new BaseRepository<Source>(_context);
            Interests = new BaseRepository<Interest>(_context);
            Customers = new BaseRepository<Customer>(_context);
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public int complete()
        {
            return _context.SaveChanges();// return the number of affected rows
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
