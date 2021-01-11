using AutoMapper;
using InternTask2.BLL.Models;
using InternTask2.BLL.Services.Abstract;
using InternTask2.Core.Helpers;
using InternTask2.Core.Models;
using InternTask2.Core.Services.Abstract;
using InternTask2.DAL.Services.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace InternTask2.BLL.Services.Concrete
{
    public class AdminService : IAdminService
    {
        IUnitOfWork db;
        ISPManager spManager;
        ISendEmail mail;

        public AdminService(IUnitOfWork db, ISPManager spm, ISendEmail se)
        {
            this.db = db;
            spManager = spm;
            mail = se;
        }

        public void Dispose() => db.Dispose();

        public IEnumerable<RoleDTO> GetAllRoles()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Role, RoleDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Role>, List<RoleDTO>>(db.Roles.GetAll());
        }

        public IEnumerable<UserDTO> GetPageUsers(int page, int pageSize)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<User>, List<UserDTO>>(
                db.Users
                    .GetAll()
                    .OrderBy(u => u.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                );
        }

        public IEnumerable<UserDTO> GetUnChecktedPageUser(int page, int pageSize)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<User>, List<UserDTO>>(
                db.Users
                   .Find(u => u.IsApproved == false)
                   .OrderBy(u => u.Id)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                );
        }

        public UserDTO GetUserById(int id)
        {
            var user = db.Users.Get(id);
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

        public bool IsUserExists(int id) => db.Users.Get(id) != null;

        public void SendApproveEmail(UserDTO user)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
            string password = PasswordHelper.GetRandomPassword();
            var userForEdit = mapper.Map<UserDTO, User>(user);
            if (mail.Send($"Your password: {password}", userForEdit.Email, "You were approved!"))
            {
                user.Password = PasswordHelper.GetHashedPassword(userForEdit.Email, password);
                user.IsApproved = true;
                spManager.Update(userForEdit);
                db.Users.Update(userForEdit);
                db.Save();
            }
            else
                throw new ValidationException("Email sending fall", "");
        }

        public void SendRejectEmail(UserDTO user)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
            var userForEdit = mapper.Map<UserDTO, User>(user);
            if (mail.Send($"Sorry.", user.Email, "You were rejected!"))
            {
                spManager.DeleteUser(user.SPId);
                db.Users.Delete(user.Id);
                db.Save();
            }
            else
                throw new ValidationException("Email sending fall", "");
        }

        public void UpdateUserRole(UserDTO user)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<RoleDTO, Role>()).CreateMapper();
            var userForEdit = db.Users.Get(user.Id);
            userForEdit.Role = user.Role;
            userForEdit.RoleId = user.RoleId;
            db.Users.Update(userForEdit);
            db.Save();
        }

        public int UsersCount() => db.Users.Count();
    }
}
