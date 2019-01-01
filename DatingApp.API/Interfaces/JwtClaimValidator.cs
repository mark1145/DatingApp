using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Interfaces
{
    public interface IJwtClaimValidator
    {
        /// <summary>
        /// It reads the name_id in the Jwt token and checks if their userId matches the userId contained within the JwT token
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool UserIdConfirmed(int id, Claim claim);
    }
}
