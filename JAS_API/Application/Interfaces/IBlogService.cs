using Application.ServiceReponse;
using Application.ViewModels.BlogDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBlogService
    {
        Task<APIResponseModel> CreateNewBlog(CreateBlogDTO createBlogDTO);
        Task<APIResponseModel> UpdateBlog(UpdateBlogDTO updateBlogDTO);
        Task<APIResponseModel> RemoveBlog(int blogId);
        Task<APIResponseModel> GetBlogs();
        Task<APIResponseModel> GetDetailBlog(int blogId);


    }
}
