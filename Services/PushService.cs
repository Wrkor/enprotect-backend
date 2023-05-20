using Microsoft.EntityFrameworkCore;
using webapi.Entities;

namespace webapi.Services
{
    public class PushService
    {
        const string TITLE_CREATE_OFFER = "Поступило новое предложение";
        const string TITLE_CREATE_OFFENSE = "Создан новый инцидент";
        const string TITLE_CHANGE_STATUS_OFFER = "Обновлен статус предложения";
        const string TITLE_CHANGE_STATUS_OFFENSE = "Обновлен статус инцидента";

        const string DESCRIPTION_CREATE_OFFER = "Создано новое предложение на тему:";
        const string DESCRIPTION_CREATE_OFFENSE = "Создан новый инцидент по охране труда на тему:";
        const string DESCRIPTION_CHANGE_STATUS_OFFER = "Обновлен статус Вашего предложения на тему:";
        const string DESCRIPTION_CHANGE_STATUS_OFFENSE = "Обновлен статус инцидента по охране труда на тему:";

        public async Task<Push[]> GetPushes(Guid userId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return await db.Pushs.Where(p => p.UserId.Equals(userId)).OrderByDescending(p => p.PushId).ToArrayAsync();
            }
        }

        public async Task SavePushByCreateOffer(Guid userid, string title)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Push push = new Push
                {
                    UserId = userid,
                    Title = TITLE_CREATE_OFFER,
                    Description = $"{DESCRIPTION_CREATE_OFFER} \"{title}\"" ,
                    Date = DateTime.Now,
                    Checked = false,
                };

                await db.Pushs.AddAsync(push);
                await db.SaveChangesAsync();
            }
        }

        public async Task SavePushByCreateOffense(Guid userid, string title)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Push push = new Push
                {
                    UserId = userid,
                    Title = TITLE_CREATE_OFFENSE,
                    Description = $"{DESCRIPTION_CREATE_OFFENSE} \"{title}\"",
                    Date = DateTime.Now,
                    Checked = false,
                };

                await db.Pushs.AddAsync(push);
                await db.SaveChangesAsync();
            }
        }

        public async Task SavePushByChangeStatusOffense(Guid userid, string title)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Push push = new Push
                {
                    UserId = userid,
                    Title = TITLE_CHANGE_STATUS_OFFENSE,
                    Description = $"{DESCRIPTION_CHANGE_STATUS_OFFENSE} \"{title}\"",
                    Date = DateTime.Now,
                    Checked = false,
                };

                await db.Pushs.AddAsync(push);
                await db.SaveChangesAsync();
            }
        }

        public async Task SavePushByChangeStatusOffer(Guid userid, string title)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Push push = new Push
                {
                    UserId = userid,
                    Title = TITLE_CHANGE_STATUS_OFFER,
                    Description = $"{DESCRIPTION_CHANGE_STATUS_OFFER} \"{title}\"",
                    Date = DateTime.Now,
                    Checked = false,
                };

                await db.Pushs.AddAsync(push);
                await db.SaveChangesAsync();
            }
        }

        public async Task CheckedPushs(Guid userid, int[] pushsid)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Push[] pushs = await db.Pushs.Where(el => pushsid.Contains<int>(el.PushId) && el.UserId.Equals(userid) && !el.Checked).ToArrayAsync();

                for (int i = 0; i < pushs.Length; i++)
                    pushs[i].Checked = true;

                await db.SaveChangesAsync();
            }
        }
    }
}
