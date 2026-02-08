using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiMedialityc.Features.Common.Security
{
    public class PasswordHasher
    {
        public static string Hash(string password) 
            => BCrypt.Net.BCrypt.HashPassword(password); 
        public static bool Verify(string password, string hashed) 
            => BCrypt.Net.BCrypt.Verify(password, hashed);
    }
}