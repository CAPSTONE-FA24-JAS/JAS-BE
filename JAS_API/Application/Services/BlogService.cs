using Application.Interfaces;
using Application.ServiceReponse;
using Application.ViewModels.BidLimitDTOs;
using Application.ViewModels.BlogDTOs;
using AutoMapper;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Domain.Entity;
using Domain.Enums;
using Application.ViewModels.LotDTOs;

namespace Application.Services
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private const string Tags = "Backend_FileImageBlog";

        public BlogService(IUnitOfWork unitOfWork, IMapper mapper, Cloudinary cloudinary)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }

        public async Task<APIResponseModel> CreateNewBlog(CreateBlogDTO createBlogDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                var user = await _unitOfWork.AccountRepository.GetByIdAsync(createBlogDTO.AccountId);
                if(user.Role.Name != "Manager")
                {
                    reponse.Message = $"User must have role manager can create blog";
                    reponse.Code = 400;
                    reponse.IsSuccess = false;
                    return reponse;
                }
                var blog = _mapper.Map<Blog>(createBlogDTO);
                blog.ImageBlogs = new List<ImageBlog>();
                if (createBlogDTO.fileImages.Count > 0)
                {
                    foreach (var fileImage in createBlogDTO.fileImages)
                    {
                        var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                        {
                            File = new FileDescription(fileImage.FileName,
                               fileImage.OpenReadStream()),
                            Tags = Tags,
                            Type = "upload"
                        }).ConfigureAwait(false);

                        if (uploadResult == null || uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            reponse.Message = $"File iamge upload failed." + uploadResult.Error.Message + "";
                            reponse.Code = (int)uploadResult.StatusCode;
                            reponse.IsSuccess = false;
                        }
                        blog.ImageBlogs.Add(new ImageBlog() 
                        {
                            ImageLink = uploadResult.SecureUrl.AbsoluteUri,
                            BlogId = blog.Id,
                        });
                    }
                }
                await _unitOfWork.BlogRepository.AddAsync(blog);
                if (await _unitOfWork.SaveChangeAsync() > 0)
                {
                    reponse.Message = $"File blog created Successfull";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
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

        public async Task<APIResponseModel> GetBlogs()
        {
            var reponse = new APIResponseModel();
            try
            {
                var blogs = await _unitOfWork.BlogRepository.GetAllAsync();
                if (blogs.Count()> 0)
                {
                    reponse.Message = $"Received Blogs Successfull";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Data = _mapper.Map<IEnumerable<ViewBlogDTO>>(blogs);
                    return reponse;
                }
                reponse.Message = $"Received Blogs Faild";
                reponse.Code = 400;
                reponse.IsSuccess = true;
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

        public async Task<APIResponseModel> GetDetailBlog(int blogId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var blogs = await _unitOfWork.BlogRepository.GetByIdAsync(blogId);
                if (blogs != null)
                {
                    reponse.Message = $"Received Blog Successfull";
                    reponse.Code = 200;
                    reponse.IsSuccess = true;
                    reponse.Data = _mapper.Map<ViewBlogDTO>(blogs);
                    return reponse;
                }
                reponse.Message = $"Received Blogs Faild";
                reponse.Code = 400;
                reponse.IsSuccess = true;
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

        public async Task<APIResponseModel> RemoveBlog(int blogId)
        {
            var reponse = new APIResponseModel();
            try
            {
                var blog = await _unitOfWork.BlogRepository.GetByIdAsync(blogId);
                if (blog != null)
                {
                    foreach (var image in blog.ImageBlogs)
                    {
                        _unitOfWork.ImageBlogRepository.Remove(image);
                    }
                    _unitOfWork.BlogRepository.Remove(blog);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.Message = $"Removbe Blog Successfull";
                        reponse.Code = 200;
                        reponse.IsSuccess = true;
                        return reponse;
                    }
                    else
                    {
                        reponse.Message = $"Removbe Blog Faild";
                        reponse.Code = 400;
                        reponse.IsSuccess = false;
                        return reponse;
                    }
                }
                reponse.Message = $"Received Blogs Faild";
                reponse.Code = 400;
                reponse.IsSuccess = true;
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

        public async Task<APIResponseModel> UpdateBlog(UpdateBlogDTO updateBlogDTO)
        {
            var reponse = new APIResponseModel();
            try
            {
                var blog = await _unitOfWork.BlogRepository.GetByIdAsync(updateBlogDTO.BlogId);
                if (blog != null)
                {
                    _mapper.Map(updateBlogDTO, blog);
                    _unitOfWork.BlogRepository.Update(blog);
                    if (await _unitOfWork.SaveChangeAsync() > 0)
                    {
                        reponse.Message = $"Update Blog Successfull";
                        reponse.Code = 200;
                        reponse.IsSuccess = true;
                        return reponse;
                    }
                    else
                    {
                        reponse.Message = $"update Blog Faild";
                        reponse.Code = 400;
                        reponse.IsSuccess = false;
                        return reponse;
                    }
                }
                reponse.Message = $"Received Blogs Faild";
                reponse.Code = 400;
                reponse.IsSuccess = true;
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
    }
}
