using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace ToDoListWeb
{
	public static class AuthOptions
	{
		public const string Issuer = "ToDoListWebServer"; // издатель токена
		public const string Audience = "ToDoListClient"; // потребитель токена
		private const string Key = "mysupersecret_secretkey!123";   // ключ для шифрации
		public const int Lifetime = 20; // время жизни токена - 20 минута
		public static SymmetricSecurityKey GetSymmetricSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
		}
    }
}
