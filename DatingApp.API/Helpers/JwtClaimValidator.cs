using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DatingApp.API.Helpers
{
    public class JwtClaimValidator : IJwtClaimValidator
    {
        public bool UserIdConfirmed(int id, Claim claim)
        {
            if (claim == null)
                return false;

            //1. Make sure that user trying to update the profile is matching the profile their trying to update
            if (id != int.Parse(claim.Value))
                return false;

            return true;
        }
    }
}
