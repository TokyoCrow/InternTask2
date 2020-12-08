using InternTask2.Website.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace InternTask2.Website.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Sex> Sexes { get; set; }
        public DbSet<Document> Documents { get; set; }

        static ApplicationContext() => Database.SetInitializer(new ApplicationContextInitializer());
        public ApplicationContext() : base("DefaultConnection") { }

        class ApplicationContextInitializer : DropCreateDatabaseIfModelChanges<ApplicationContext>
        {
            protected override void Seed(ApplicationContext db)
            {
                string[] roleNames = { "admin", "user" };
                var roles = new List<Role>();
                string[] sexNames = { "Male", "Female" };
                var sexes = new List<Sex>();

                string adminEmail = "admin@mail.ru";
                string adminName = "Admi";
                string adminSurname = "Adminov";
                string adminPassword = "12345";

                for (var i = 1; i <= roleNames.Length; i++)
                    roles.Add(new Role { Id = i, Name = roleNames[i - 1] });

                for (var i = 1; i <= sexNames.Length; i++)
                    sexes.Add(new Sex { Id = i, Name = sexNames[i - 1] });

                User adminUser = new User
                {
                    Id = 1,
                    RoleId = roles.Where(role => role.Name == "admin").Single().Id,
                    Name = adminName,
                    Surname = adminSurname,
                    Password = PasswordHelper.GetHashedPassword(adminEmail, adminPassword),
                    Email = adminEmail,
                    IsApproved = true
                };

                db.Roles.AddRange(roles.ToArray());
                db.Sexes.AddRange(sexes.ToArray());
                db.Users.AddRange(new User[] { adminUser });
                db.SaveChanges();
            }
        }
    }
}
