using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoListWeb.Models;

namespace ToDoListWeb.Data
{
    public sealed class ToDoListWebContext : DbContext
    {
        public ToDoListWebContext (DbContextOptions<ToDoListWebContext> options)
            : base(options)
        {
        }

        public DbSet<ToDoListWeb.Models.User> User { get; set; }

        public DbSet<ToDoListWeb.Models.Note> Note { get; set; }
    }
}
