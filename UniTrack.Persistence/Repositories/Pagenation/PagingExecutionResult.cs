using UniTrack.Application.Abstraction.Repositories.Pagenation;

namespace UniTrack.Persistence.Repositories.Pagenation
{
        public class PagingExecutionResult<T> : IPagingExecutionResult<T>
        {
            public int TotalCount { get; }
            public List<T> Data { get; }
            public bool HasPaging { get; }
            public int CurrentPage { get; }
            public int PageSize { get; }

            public PagingExecutionResult(IEnumerable<T> data, bool hasPaging, int currentPage, int pageSize, int? totalCount = null)
            {
                TotalCount = hasPaging ? totalCount ?? 0 : data.Count();
                Data = data.ToList();
                HasPaging = hasPaging;
                CurrentPage = currentPage;
                PageSize = pageSize;
            }
        }
}
