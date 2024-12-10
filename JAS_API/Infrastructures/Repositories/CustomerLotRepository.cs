using Application.Interfaces;
using Application.Repositories;
using Castle.Core.Resource;
using Domain.Entity;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class CustomerLotRepository : GenericRepository<CustomerLot>, ICustomerLotRepository
    {
        private readonly AppDbContext _dbContext;
        public CustomerLotRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
            : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }

        
       public async Task<CustomerLot> GetCustomerLotByCustomerAndLot(int? customerIId, int? lotId)
        {
            var customerLot = await _dbContext.CustomerLots.Where(x => x.CustomerId == customerIId && x.LotId == lotId).FirstOrDefaultAsync();
            if (customerLot == null)
            {
                throw new Exception("Not found CustomerLot");
            }
            return customerLot;
        }

        //get những thằng thua
        public List<CustomerLot>? GetListCustomerLotByCustomerAndLot(int lotId, int customerLotWinnerId)
        {
            var customerLots = _dbContext.CustomerLots
       .AsEnumerable() // Switch to client-side evaluation
       .Where(x => x.LotId == lotId && x.Id != customerLotWinnerId)
       .ToList();
            if (!customerLots.Any())
            {
                return null;
            }
            return customerLots;
        }

        public async Task<(IEnumerable<CustomerLot> data, int totalItems)> GetBidsOfCustomer(int? customerIId, string? status, int? pageIndex, int? pageSize)
        {
            var customerLots = _dbContext.CustomerLots.Include(x => x.Lot)
                                               .Where(x => x.CustomerId == customerIId && x.Status == status);
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10; // Assuming a default pageSize of 10 if an invalid value is passed

                customerLots = customerLots.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            var products = await customerLots.ToListAsync();

            var totalItems = products.Count;

            if (products != null && products.Any())
            {
                return (products, totalItems);
            }
            else
            {
                throw new Exception("Don't have any Lots");
            }
        }


        public async Task<(IEnumerable<CustomerLot> data, int totalItems)> GetPastBidOfCustomer(int customerIId, IEnumerable<string> status, int? pageIndex, int? pageSize)
        {
            var lots = _dbContext.CustomerLots.Include(x => x.Lot)
                                      .Where(x =>  status.Contains(x.Status) && x.CustomerId == customerIId);

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10; // Assuming a default pageSize of 10 if an invalid value is passed

                lots = lots.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            var products = await lots.ToListAsync();

            var totalItems = products.Count;

            if (products != null && products.Any())
            {
                return (products, totalItems);
            }
            else
            {
                throw new Exception("Don't have any Lots");
            }


        }

        //lay ra het nhung customerLot theo lotId nhung ngoai tru thang thang(customerId)
        public List<CustomerLot>? GetListCustomerLotByLotId(int lotId, int customerLotId)
        {
            var customerLots =  _dbContext.CustomerLots.AsEnumerable().Where(x => x.LotId == lotId && x.Id != customerLotId).ToList();
            if (!customerLots.Any())
            {
                return [];
            }
            return customerLots;
        }

        public async Task<List<CustomerLot>> GetAllCustomerLotAuctioningAsync()
        {
            try
            {
                 return await _dbContext.CustomerLots.AsNoTracking()
                    .Where(x => x.Lot != null && x.Lot.Status == EnumStatusLot.Auctioning.ToString() && x.Lot.LotType == EnumLotType.Public_Auction.ToString())
                    .Select(x => new CustomerLot
                    {
                        Id = x.Id,
                        CustomerId = x.CustomerId,
                        Lot = new Lot
                        {
                            Id = x.Lot.Id,
                            Status = x.Lot.Status,
                            StartPrice = x.Lot.StartPrice,
                            BidIncrement = x.Lot.BidIncrement,
                            IsExtend = x.Lot.IsExtend,
                        },
                        AutoBids = x.AutoBids.Select(autoBid => new AutoBid
                        {
                            Id = autoBid.Id,
                            MinPrice = autoBid.MinPrice,
                            MaxPrice = autoBid.MaxPrice,
                            TimeIncrement = autoBid.TimeIncrement,
                            NumberOfPriceStep = autoBid.NumberOfPriceStep,
                            IsActive = autoBid.IsActive,

                        }).ToList(),
                        Customer = new Customer
                        {
                            Id = x.Customer.Id,
                            PriceLimit = x.Customer.PriceLimit,
                            FirstName = x.Customer.FirstName,
                            LastName = x.Customer.LastName,
                        }
                        
                    })
                    .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error retrieving CustomerLots: {e.Message}");
                return new List<CustomerLot>();
            }
        }


    }
}
