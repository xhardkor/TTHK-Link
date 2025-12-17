using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTHK_Link.Services.Http
{
    internal class ApiRoute
    {
        // Registreerimine: POST /auth?login=...&password=...&groupid=...
        public const string Register = "/auth";

        // Login: pane siia oma päris login endpoint (näiteks "/login" või "/auth/login")
        public const string Login = "/login";
    }
}
