﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListWeb.Models
{
	public class Note
	{
		public int Id { get; set; }
		public string Header { get; set; }
		public DateTime DateAdded { get; set; }
		public string Text { get; set; }
		public User User { get; set; }
	}
}
