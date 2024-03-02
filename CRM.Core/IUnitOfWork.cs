using CRM.Core.Interfaces;
using CRM.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<VerificationCode> VerificationCodes { get; }
        UserManager<ApplicationUser> UserManager { get; }
        RoleManager<IdentityRole> RoleManager { get; }
        IBaseRepository<Source> Sources { get; }
        IBaseRepository<Interest> Interests { get; }
        IBaseRepository<Customer> Customers { get; }
        IBaseRepository<Call> Calls { get; }
        IBaseRepository<Message> Messages { get; }
        IBaseRepository<Meeting>Meetings { get; } 
        IBaseRepository<Deal> Deals { get; }
        int complete();
    }
}
