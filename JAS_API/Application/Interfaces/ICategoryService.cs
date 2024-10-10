using Application.ServiceReponse;
using Application.ViewModels.CategoryDTOs;
using Application.ViewModels.KeyCharacteristicDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICategoryService 
    {
        public Task<APIResponseModel> CreateCategoryAsync(CategoryDTO categoryDTO);

        public Task<APIResponseModel> getCategoriesAsync();
    }
}
