using MediatR;

namespace UniTrack.Application.Abstraction.Services.PageBase
{
    public abstract class PagedSearchQuery<T, TPrimaryKey> : IRequest<T>
    {
        // Request tarafı
        public TPrimaryKey Id { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
