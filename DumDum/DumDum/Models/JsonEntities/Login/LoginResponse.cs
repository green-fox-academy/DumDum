using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DumDum.Models.JsonEntities.Login
{
    public class LoginResponse
    {
        public string Status { get; set; }
        public string Token { get; set; }
    }
}
