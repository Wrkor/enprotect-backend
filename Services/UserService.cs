using Microsoft.EntityFrameworkCore;
using webapi.Entities;
using webapi.Params.HttpRequest;

namespace webapi.Services
{
    public class UserService
    {
        public async Task SaveUserData(Guid userId, UserSaveParams userSaveParams)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                User? user =  await db.Users.FirstOrDefaultAsync(p => p.UserId.Equals(userId));

                if (user != null)
                {
                    user.Push_Offense_Email = userSaveParams.Push_Offense_Email;
                    user.Push_Offer_Email = userSaveParams.Push_Offer_Email;
                    user.Push_Offense_SMS = userSaveParams.Push_Offense_SMS;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task CreateUser(User user)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
            }
        }

        public async Task<User?> FindUserAuth(string login)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return await db.Users.FirstOrDefaultAsync(p => p.Login.Equals(login));
            }
        }

        public async Task<User?> GetUser(Guid userId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return await db.Users.FirstOrDefaultAsync(p => p.UserId.Equals(userId));
            }
        }

        public async Task<User[]> GetUsersBySName(Guid userId, string sname)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return await db.Users.Where(p => p.SName.Contains(sname) && !p.UserId.Equals(userId)).ToArrayAsync();
            }
        }

        public async Task<Role?> GetRole(int roleId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return await db.Roles.FirstOrDefaultAsync(p => p.RoleId == roleId);
            }
        }

        public bool GetFiles(string? paths, out string[] base64Files, int amount = 0)
        {
            if (paths == null)
            {
                base64Files = Array.Empty<string>();
                return false;
            }

            string[] filePaths = paths.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            foreach (string filePath in filePaths)
            {
                if (!Path.Exists(filePath))
                {
                    base64Files = Array.Empty<string>();
                    return false;
                }
            }


            if (amount == 0 || amount > filePaths.Length)
            {
                base64Files = new string[filePaths.Length];

                for (int i = 0; i < filePaths.Length; i++)
                {
                    base64Files[i] = Convert.ToBase64String(File.ReadAllBytes(filePaths[i]));
                }
            }
            
            else
            {
                base64Files = new string[amount];

                for (int i = 0; i < amount; i++)
                {
                    base64Files[i] = Convert.ToBase64String(File.ReadAllBytes(filePaths[i]));
                }
            }

            return true;
        }
    }
}
