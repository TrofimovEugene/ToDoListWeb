using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListWeb.Models
{
	public class User
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string SecondName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Organization { get; set; }
		public string Role { get; set; }
		public DateTime DateOfBirth { get; set; }
	}
}
