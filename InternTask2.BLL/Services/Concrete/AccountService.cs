using AutoMapper;
using InternTask2.BLL.Models;
using InternTask2.BLL.Services.Abstract;
using InternTask2.Core.Helpers;
using InternTask2.Core.Models;
using InternTask2.DAL.Services.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace InternTask2.BLL.Services.Concrete
{
    public class AccountService : IAccountService
    {
        IUnitOfWork db;
        ISPManager spManager;

        public AccountService(IUnitOfWork db, ISPManager spm)
        {
            this.db = db;
            spManager = spm;
        }

        public void Dispose() => db.Dispose();

        public UserDTO FindUser(string email, string password)
        {
            string hashedPassword = PasswordHelper.GetHashedPassword(email, password);
            var user = db.Users.Find(u => u.Email == email && u.Password == hashedPassword).FirstOrDefault();
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Patronymic = user.Patronymic,
                BirthDate = user.BirthDate,
                SexId = user.SexId,
                Workplace = user.Workplace,
                Position = user.Position,
                Country = user.Country,
                City = user.City,
                SPId = user.SPId,
                IsApproved = user.IsApproved
            };
        }

        public UserDTO FindUser(string email)
        {
            var user = db.Users.Find(u => u.Email == email).FirstOrDefault();
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Patronymic = user.Patronymic,
                BirthDate = user.BirthDate,
                SexId = user.SexId,
                Workplace = user.Workplace,
                Position = user.Position,
                Country = user.Country,
                City = user.City,
                SPId = user.SPId,
                IsApproved = user.IsApproved
            };
        }

        public IEnumerable<SexDTO> GetAllSexes()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Sex, SexDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Sex>, List<SexDTO>>(db.Sexes.GetAll());
        }

        public void Registration(UserDTO userDTO)
        {
            var user = db.Users.Find(u => u.Email == userDTO.Email).FirstOrDefault();
            if (user == null)
            {
                user = new User
                {
                    Name = userDTO.Name,
                    Surname = userDTO.Surname,
                    Email = userDTO.Email,
                    Patronymic = userDTO.Patronymic,
                    BirthDate = userDTO.BirthDate,
                    SexId = userDTO.SexId,
                    Workplace = userDTO.Workplace,
                    Position = userDTO.Position,
                    Country = userDTO.Country,
                    City = userDTO.City
                };
                Role userRole = db.Roles.Find(r => r.Name == "user").FirstOrDefault();
                if (userRole != null)
                    user.Role = userRole;
                Sex userSex = db.Sexes.Find(s => s.Id == userDTO.SexId).FirstOrDefault();
                if (userSex != null)
                    user.Sex = userSex;
                user.SPId = spManager.Create(user);
                if (user.SPId > 0)
                {
                    db.Users.Create(user);
                    db.Save();
                }
                else
                    throw new ValidationException("Registration fall", "");
            }
            else
                throw new ValidationException("Email already used", "");
        }
    }
}
