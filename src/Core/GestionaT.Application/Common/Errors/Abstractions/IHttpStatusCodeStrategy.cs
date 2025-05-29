using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionaT.Application.Common.Errors.Abstractions
{
    public interface IHttpStatusCodeStrategy
    {
        bool CanHandle(string errorCode);
        int GetStatusCode();
    }
}
