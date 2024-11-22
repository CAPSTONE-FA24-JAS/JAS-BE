using Application.ServiceReponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDashBoardService
    {
        //DashBoard
        Task<APIResponseModel> TotalInvoice();
        Task<APIResponseModel> TotalInvoiceByMonth(int month);
        Task<APIResponseModel> DashBoardRevenueInYear(int year);
        Task<APIResponseModel> GetRevenueByMonthWithYear(int month, int year);
        Task<APIResponseModel> TotalRevenue();
        Task<APIResponseModel> DashBoardInvoiceInYear(int year);
        Task<APIResponseModel> GetTopFiveJewelryAuctionsAsync();
        Task<APIResponseModel> GetTopFiveSellersAsync();
        Task<APIResponseModel> GetTopFiveBuyersAsync();
        Task<APIResponseModel> TotalUser();
        Task<APIResponseModel> TotalUserActive();
        Task<APIResponseModel> TotalProfit();
    }
}
