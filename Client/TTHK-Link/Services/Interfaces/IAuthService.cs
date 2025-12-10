using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTHK_Link.Models;

namespace TTHK_Link.Services.Interfaces
{
    internal interface IAuthService
    {
        Task<AuthResult> LoginAsync(LoginRequest request);
        Task LogoutAsync();

        User CurrentUser { get; }
    }
}
