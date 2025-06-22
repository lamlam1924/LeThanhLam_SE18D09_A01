using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Services
{
    public interface IAuthenticationService
    {
        bool AuthenticateAdmin(string email, string password);
        Customer AuthenticateCustomer(string email, string password);
    }
}
