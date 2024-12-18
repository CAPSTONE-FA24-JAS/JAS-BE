using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.AccountDTOs;
using Application.ViewModels.CustomerLotDTOs;
using Application.ViewModels.InvoiceDTOs;
using Application.ViewModels.JewelryDTOs;
using Application.ViewModels.TransactionDTOs;
using AutoMapper;
using Azure;
using Domain.Entity;
using Domain.Enums;
using System.Linq.Expressions;

namespace Application.Services
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DashBoardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponseModel> TotalInvoice()
        {
            var response = new APIResponseModel();
            try
            {
                var totalInvoice = await _unitOfWork.InvoiceRepository.GetAllAsync();
                if (totalInvoice.Count > 0)
                {
                    response.Code = 200;
                    response.Data = totalInvoice.Count;
                    response.IsSuccess = true;
                    response.Message = $"Received Successfully Total Invoice: {totalInvoice.Count}.";
                }
                else
                {
                    response.Code = 200;
                    response.Data = totalInvoice.Count;
                    response.IsSuccess = true;
                    response.Message = $"Current Time System Haven't Invoice.";
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        public async Task<APIResponseModel> TotalInvoiceByMonth(int month)
        {
            var response = new APIResponseModel();
            try
            {
                var totalInvoiceByMonth = await _unitOfWork.InvoiceRepository.GetAllAsync(x => x.CreationDate.Month == month);
                if (totalInvoiceByMonth.Count > 0)
                {
                    response.Code = 200;
                    response.Data = totalInvoiceByMonth.Count;
                    response.IsSuccess = true;
                    response.Message = $"Received Successfully Total Invoice By Month {month}: {totalInvoiceByMonth.Count}.";
                }
                else
                {
                    response.Code = 200;
                    response.Data = totalInvoiceByMonth.Count;
                    response.IsSuccess = true;
                    response.Message = $"In Month {month}, System Haven't Invoice.";
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        public async Task<APIResponseModel> DashBoardRevenueInYear(int year)
        {
            var response = new APIResponseModel();
            try
            {
                var invoiceInYears = await _unitOfWork.InvoiceRepository.GetAllAsync(x => x.Status == EnumCustomerLot.Finished.ToString() && x.CreationDate.Year == year);

                var revenues = new[]
                {
                    new { Month = "January", Revenue = await GetRevenueByMonth(invoiceInYears,1)},
                    new { Month = "Febraury", Revenue = await GetRevenueByMonth(invoiceInYears,2)},
                    new { Month = "March", Revenue = await GetRevenueByMonth(invoiceInYears,3)},
                    new { Month = "April", Revenue = await GetRevenueByMonth(invoiceInYears,4)},
                    new { Month = "May", Revenue = await GetRevenueByMonth(invoiceInYears,5)},
                    new { Month = "June", Revenue = await GetRevenueByMonth(invoiceInYears,6)},
                    new { Month = "July", Revenue = await GetRevenueByMonth(invoiceInYears,7)},
                    new { Month = "August", Revenue = await GetRevenueByMonth(invoiceInYears,8)},
                    new { Month = "September", Revenue = await GetRevenueByMonth(invoiceInYears,9)},
                    new { Month = "October", Revenue = await GetRevenueByMonth(invoiceInYears,10)},
                    new { Month = "November", Revenue = await GetRevenueByMonth(invoiceInYears,11)},
                    new { Month = "December", Revenue = await GetRevenueByMonth(invoiceInYears,12)},
                };
                response.Code = 200;
                response.Data = revenues;
                response.IsSuccess = true;
                response.Message = $"Received Successfully Revenue In Year {year}";

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        internal async Task<float?> GetRevenueByMonth(List<Invoice> invoices, int month)
        {
            var Revenue = invoices.Where(x => x.CreationDate.Month == month).ToList();
            if (Revenue.Count > 0)
            {
                return Revenue.Sum(x => (x.FeeShip??0 - x.Free??0));
            }
            return 0;
        }

        internal async Task<float?> GetTotalInvoiceByMonth(List<Invoice> invoices, int month)
        {
            var invoice = invoices.Where(x => x.CreationDate.Month == month).ToList();
            if (invoice.Count > 0)
            {
                return invoice.Count;
            }
            return 0;
        }

        public async Task<APIResponseModel> GetRevenueByMonthWithYear(int month, int year)
        {
            var reponse = new APIResponseModel();
            try
            {
                var totalRevenueOfCompanyByMonth = await _unitOfWork.InvoiceRepository
                                                        .GetAllAsync(x => x.CreationDate.Month == month
                                                                      && x.Status == EnumCustomerLot.Finished.ToString());

                if (!totalRevenueOfCompanyByMonth.Any())
                {
                    reponse.Message = $"Revenue of month {month} have nothing.";
                    reponse.Code = 404;
                    reponse.IsSuccess = true;
                }
                else
                {
                    reponse.Message = $"Received Revenue of month {month} successfully";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Data = _mapper.Map<ViewRevenueOfConpanyDTO>(totalRevenueOfCompanyByMonth);
                }
            }
            catch (Exception ex)
            {
                reponse.ErrorMessages = ex.Message.Split(',').ToList();
                reponse.Message = "Exception";
                reponse.Code = 500;
                reponse.IsSuccess = false;
            }
            return reponse;
        }

        public async Task<APIResponseModel> TotalRevenue()
        {
            var response = new APIResponseModel();
            try
            {
                var totalRevenue = await _unitOfWork.InvoiceRepository.GetAllAsync(x => x.Status == EnumCustomerLot.Finished.ToString());
                if (totalRevenue.Count > 0)
                {
                    var total= totalRevenue.Sum(x => x.FeeShip + x.Free);
                    response.Code = 200;
                    response.Data = total;
                    response.IsSuccess = true;
                    response.Message = $"Received Successfully Total Revenue: {total}.";
                }
                else
                {
                    response.Code = 200;
                    response.Data = 0;
                    response.IsSuccess = true;
                    response.Message = $"Current Time System Haven't Revenue.";
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        public async Task<APIResponseModel> DashBoardInvoiceInYear(int year)
        {
            var response = new APIResponseModel();
            try
            {
                var invoiceInYears = await _unitOfWork.InvoiceRepository.GetAllAsync(x => x.Status == EnumCustomerLot.Finished.ToString() && x.CreationDate.Year == year);

                var revenues = new[]
                {
                    new { Month = "January", Revenue = await GetTotalInvoiceByMonth(invoiceInYears,1)},
                    new { Month = "Febraury", Revenue = await GetTotalInvoiceByMonth(invoiceInYears,2)},
                    new { Month = "March", Revenue = await GetTotalInvoiceByMonth(invoiceInYears,3)},
                    new { Month = "April", Revenue = await GetTotalInvoiceByMonth(invoiceInYears,4)},
                    new { Month = "May", Revenue = await GetTotalInvoiceByMonth(invoiceInYears,5)},
                    new { Month = "June", Revenue = await GetTotalInvoiceByMonth(invoiceInYears,6)},
                    new { Month = "July", Revenue = await GetTotalInvoiceByMonth(invoiceInYears,7)},
                    new { Month = "August", Revenue = await GetTotalInvoiceByMonth(invoiceInYears,8)},
                    new { Month = "September", Revenue = await GetTotalInvoiceByMonth(invoiceInYears,9)},
                    new { Month = "October", Revenue = await GetTotalInvoiceByMonth(invoiceInYears,10)},
                    new { Month = "November", Revenue = await GetTotalInvoiceByMonth(invoiceInYears,11)},
                    new { Month = "December", Revenue = await GetTotalInvoiceByMonth(invoiceInYears,12)},
                };
                response.Code = 200;
                response.Data = revenues;
                response.IsSuccess = true;
                response.Message = $"Received Successfully Revenue In Year {year}";

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        public async Task<APIResponseModel> GetTopFiveJewelryAuctionsAsync()
        {
            var response = new APIResponseModel();
            try
            {
                var jewelrys = await _unitOfWork.JewelryRepository.GetAllAsync(x => x.Status == "Added"
                                                                                     && x.Lots.Any(lot => lot.CustomerLots.Any(customerLot => customerLot.Invoice != null)));

                var sortedJewelry = jewelrys
                    .Select(jewelry => new ViewTopJewelryDTO
                    {
                        JewelryDTO = _mapper.Map<JewelryDTO>(jewelry),
                        MaxPriceAuctionEnd = jewelry.Lots
                                          .SelectMany(lot => lot.CustomerLots)
                                          .Where(customerLot => customerLot.Invoice != null)
                                          .Max(customerLot => customerLot.CurrentPrice)
                    })
                    .OrderByDescending(j => j.MaxPriceAuctionEnd)
                    .Take(5)
                    .ToList();


                if (sortedJewelry.Count > 0)
                {

                    response.Code = 200;
                    response.Data = sortedJewelry;
                    response.IsSuccess = true;
                    response.Message = "Received Successfully Jewelrey Top";
                }
                else
                {
                    response.Code = 200;
                    response.Data = 0;
                    response.IsSuccess = true;
                    response.Message = "Current Time System Haven't Jewelrey Top.";
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        public async Task<APIResponseModel> GetTopFiveSellersAsync()
        {
            var response = new APIResponseModel();
            try
            {
                var sellers = await _unitOfWork.CustomerRepository.GetAllAsync(x => x.SellerValuations.Count() > 0
                                                                                     && x.SellerValuations.Any(x => x.Status == EnumStatusValuation.ManagerApproved.ToString()));

                var sortedseller = sellers
                    .Select(seller => new ViewTopSellerDTO
                    {
                        customerDTO = _mapper.Map<CustomerDTO>(seller),
                        TotalSellerValuation = seller.SellerValuations.Count(x => x.Status == EnumStatusValuation.ManagerApproved.ToString()),
                    })
                    .OrderByDescending(j => j.TotalSellerValuation)
                    .Take(5)
                    .ToList();


                if (sortedseller.Count > 0)
                {

                    response.Code = 200;
                    response.Data = sortedseller;
                    response.IsSuccess = true;
                    response.Message = "Received Successfully Seller Valuation Top";
                }
                else
                {
                    response.Code = 200;
                    response.Data = 0;
                    response.IsSuccess = true;
                    response.Message = "Current Time System Haven't Seller Valuation Top.";
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        public async Task<APIResponseModel> GetTopFiveBuyersAsync()
        {
            var response = new APIResponseModel();
            try
            {
                var buyers = await _unitOfWork.CustomerRepository.GetAllAsync(x => x.CustomerLots.Any(x => x.Status == EnumCustomerLot.Paid.ToString()));

                var sortedbuyer = buyers
                    .Select(buyer => new ViewTopBuyerDTO
                    {
                        customerDTO = _mapper.Map<CustomerDTO>(buyer),
                        TotalBuyerJewelry = buyer.CustomerLots.Count(x => x.Status == EnumCustomerLot.Paid.ToString()),
                    })
                    .OrderByDescending(j => j.TotalBuyerJewelry)
                    .Take(5)
                    .ToList();


                if (sortedbuyer.Count > 0)
                {

                    response.Code = 200;
                    response.Data = sortedbuyer;
                    response.IsSuccess = true;
                    response.Message = "Received Successfully Buyer Top";
                }
                else
                {
                    response.Code = 200;
                    response.Data = 0;
                    response.IsSuccess = true;
                    response.Message = "Current Time System Haven't Buyer Top.";
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        public async Task<APIResponseModel> TotalCustomer()
        {
            var response = new APIResponseModel();
            try
            {
                var accounts = await _unitOfWork.AccountRepository.GetAllAsync(x => x.Role.Name == "Customer" && x.IsConfirmed == true);

                if (accounts.Count > 0)
                {

                    response.Code = 200;
                    response.Data = accounts.Count;
                    response.IsSuccess = true;
                    response.Message = "Received Successfully Customer In System";
                }
                else
                {
                    response.Code = 200;
                    response.Data = 0;
                    response.IsSuccess = true;
                    response.Message = "Current Time System Haven't Customer.";
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        public async Task<APIResponseModel> TotalCustomerActive()
        {
            var response = new APIResponseModel();
            try
            {
                var accounts = await _unitOfWork.AccountRepository.GetAllAsync(x => x.Role.Name == "Customer" && x.IsConfirmed == true && x.Status == true);

                if (accounts.Count > 0)
                {

                    response.Code = 200;
                    response.Data = accounts.Count;
                    response.IsSuccess = true;
                    response.Message = "Received Successfully Customer In System";
                }
                else
                {
                    response.Code = 200;
                    response.Data = 0;
                    response.IsSuccess = true;
                    response.Message = "Current Time System Haven't Customer.";
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        public async Task<APIResponseModel> TotalProfit()
        {
            var response = new APIResponseModel();
            try
            {
                float? totalProfit = 0f;

                var invoices = await _unitOfWork.InvoiceRepository.GetAllAsync(x => x.Status == EnumCustomerLot.Finished.ToString());

                foreach (var invoice in invoices ?? Enumerable.Empty<Invoice>())
                {
                    var free = invoice.Free ?? 0f; 
                    var feeShipPrice = invoice.FeeShip ?? 0f;

                    totalProfit += (feeShipPrice + free);
                }

                response.Code = 200;
                response.Data = totalProfit;
                response.IsSuccess = true;
                response.Message = "Received Successfully Total Profit In System";
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processing: {ex.Message}";
            }
            return response;
        }

        public async Task<APIResponseModel> TotalRevenueInvoice()
        {
            var response = new APIResponseModel();
            try
            {
                var totalRevenue = await _unitOfWork.InvoiceRepository.GetAllAsync(x => x.Status == EnumCustomerLot.Finished.ToString());
                if (totalRevenue.Count > 0)
                {
                    var total = totalRevenue.Sum(x => x.TotalPrice);
                    response.Code = 200;
                    response.Data = total;
                    response.IsSuccess = true;
                    response.Message = $"Received Successfully Total Revenue: {total}.";
                }
                else
                {
                    response.Code = 200;
                    response.Data = 0;
                    response.IsSuccess = true;
                    response.Message = $"Current Time System Haven't Revenue.";
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        public async Task<APIResponseModel> TotalUser()
        {
            var response = new APIResponseModel();
            try
            {
                var accounts = await _unitOfWork.AccountRepository.GetAllAsync(x => x.IsConfirmed == true);

                if (accounts.Count > 0)
                {

                    response.Code = 200;
                    response.Data = accounts.Count;
                    response.IsSuccess = true;
                    response.Message = "Received Successfully Account In System";
                }
                else
                {
                    response.Code = 200;
                    response.Data = 0;
                    response.IsSuccess = true;
                    response.Message = "Current Time System Haven't Account.";
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }

        public async Task<APIResponseModel> TotalUserActive()
        {
            var response = new APIResponseModel();
            try
            {
                var accounts = await _unitOfWork.AccountRepository.GetAllAsync(x => x.IsConfirmed == true && x.Status == true);

                if (accounts.Count > 0)
                {

                    response.Code = 200;
                    response.Data = accounts.Count;
                    response.IsSuccess = true;
                    response.Message = "Received Successfully Account In System";
                }
                else
                {
                    response.Code = 200;
                    response.Data = 0;
                    response.IsSuccess = true;
                    response.Message = "Current Time System Haven't Account.";
                }

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.IsSuccess = false;
                response.Message = $"Exception When System Processcing";
            }
            return response;
        }
    }
}
