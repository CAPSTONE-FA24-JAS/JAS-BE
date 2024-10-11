using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.ArtistDTOs;
using Application.ViewModels.CategoryDTOs;
using Application.ViewModels.KeyCharacteristicDTOs;
using AutoMapper;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<APIResponseModel> CreateCategoryAsync(CategoryDTO categoryDTO)
        {
            var response = new APIResponseModel();
            try
            {

                var category = _mapper.Map<Category>(categoryDTO);
                if (category == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Mapper failed";
                }
                else
                {
                    await _unitOfWork.CategoryRepository.AddAsync(category);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        response.Message = $"Create Category Successfully";
                        response.Code = 200;
                        response.IsSuccess = true;
                        response.Data = category;
                    }
                    else
                    {
                        response.Message = $"Save DB failed!";
                        response.Code = 500;
                        response.IsSuccess = false;
                    }
                }

            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<APIResponseModel> getCategoriesAsync()
        {
            var response = new APIResponseModel();
            try
            {
                var categories = await _unitOfWork.CategoryRepository.GetAllPaging(filter: null,
                                                                             orderBy: x => x.OrderByDescending(t => t.CreationDate)
                                                                             );
                List<CategoryDTO> listCategoriesDTO = new List<CategoryDTO>();
                if (categories.totalItems > 0)
                {
                    response.Message = $"List categories Successfully";
                    response.Code = 200;
                    response.IsSuccess = true;
                    foreach (var item in categories.data)
                    {
                        var categoriesResponse = _mapper.Map<CategoryDTO>(item);
                        listCategoriesDTO.Add(categoriesResponse);
                    };
                    response.Data = listCategoriesDTO;
                }
                else
                {
                    response.Message = $"Don't have category";
                    response.Code = 404;
                    response.IsSuccess = true;

                }
            }
            catch (Exception ex)
            {
                response.ErrorMessages = ex.Message.Split(',').ToList();
                response.Message = "Exception";
                response.Code = 500;
                response.IsSuccess = false;
            }
            return response;
        }
    }
}
