using AutoMapper;
using Library.Application.Contracts.Authors;
using Library.Application.Contracts.Books;
using Library.Application.DTOs;
using Library.Core.Common;
using Library.Core.Entities;

namespace Library.Application.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<Book, GetBookResponse>().ReverseMap();
            CreateMap<Author, GetAuthorResponse>().ReverseMap();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>)).ConvertUsing(typeof(PagedResultConverter<,>));
        }
    }

    public class PagedResultConverter<TSource, TDestination> : ITypeConverter<PagedResult<TSource>, PagedResult<TDestination>>
    {
        public PagedResult<TDestination> Convert(PagedResult<TSource> source, PagedResult<TDestination> destination, ResolutionContext context)
        {
            var mappedItems = context.Mapper.Map<IEnumerable<TDestination>>(source.Items);
            return new PagedResult<TDestination>(mappedItems, source.TotalItems, source.Page, source.PageSize);
        }
    }
}