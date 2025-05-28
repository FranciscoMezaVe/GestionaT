using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionaT.Application.Interfaces.Auth
{
    public interface IOAuthServiceFactory
    {
        IOAuthService? GetService(string provider);
    }
}
