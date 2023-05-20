using Microsoft.EntityFrameworkCore;
using webapi.Entities;

namespace webapi.Services
{
    public class OfferService
    {
        public async Task SetOffer(Offer newOffer)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                await db.Offers.AddAsync(newOffer);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateOffer(int offerId, Offer updateOffer)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Offer? offer = await db.Offers.FirstOrDefaultAsync(p => p.OfferId == offerId);
                if (offer != null)
                {
                    offer.Status = updateOffer.Status;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task<Offer?> GetOffer(int offerId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return await db.Offers.FirstOrDefaultAsync(p => p.OfferId == offerId);
            }
        }

        public async Task<Offer[]> GetOffersByExpert(Guid userId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                    return await db.Offers.Where(p => p.ExpertId.Equals(userId)).OrderByDescending(p => p.OfferId).ToArrayAsync();
            }
        }

        public async Task<Offer[]> GetOffersByEmployee(Guid userId)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return await db.Offers.Where(p => p.EmployeeId.Equals(userId)).OrderByDescending(p => p.OfferId).ToArrayAsync();
            }
        }
    }
}
