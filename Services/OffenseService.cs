using Microsoft.EntityFrameworkCore;
using webapi.Entities;

namespace webapi.Services
{
    public class OffenseService
    {
        public async Task SetOffense(Offense newOffense)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                await db.Offenses.AddAsync(newOffense);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateOffense(int offenseId, Offense updateOffense)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Offense? offense = await db.Offenses.FirstOrDefaultAsync(p => p.OffenseId == offenseId);
                if (offense != null)
                {
                    offense.Status = updateOffense.Status;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task<Offense?> GetOffense(int offenseId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return await db.Offenses.FirstOrDefaultAsync(p => p.OffenseId == offenseId);
            }
        }

        public async Task<Offense[]> GetOffenses(Guid userId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return await db.Offenses.Where(p => p.ExpertId.Equals(userId)).OrderByDescending(p => p.OffenseId).ToArrayAsync();
            }
        }

        public async Task<Offense[]> GetOffensesByEmployee(Guid userId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return await db.Offenses.Where(p => p.EmployeeId.Equals(userId)).OrderByDescending(p => p.OffenseId).ToArrayAsync();
            }
        }
    }
}
