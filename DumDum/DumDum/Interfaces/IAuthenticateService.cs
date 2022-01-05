using DumDum.Models.JsonEntities.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DumDum.Interfaces
{
    public interface IAuthenticateService
    {
        AuthResponse GetUserInfo(AuthRequest request);
    }
}
