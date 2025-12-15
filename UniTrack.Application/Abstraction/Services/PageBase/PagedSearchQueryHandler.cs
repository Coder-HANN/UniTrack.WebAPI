using MediatR;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Application.Common;

namespace UniTrack.Application.Abstraction.Services.PageBase
{

    public abstract class PagedSearchQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
 where TRequest : IRequest<TResponse>
    {
        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

        protected ServiceResponse<IPagingExecutionResult<T>> HandleResult<T>(IPagingExecutionResult<T> paginationResult)
        {
            return new ServiceResponse<IPagingExecutionResult<T>>
            {
                IsSuccess = true,
                Data = paginationResult,
                Message = null
            };
        }

    }
}
