using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using UniTrack.Application.Abstraction.Services.Interceptors;
using UniTrack.Application.Abstraction.Services.IoC;

namespace UniTrack.Application.Abstraction.Services.MemoryCach
{
    public class CacheRemoveAspect : MethodInterception
    {
        private string _pattern;
        private ICacheManager _cacheManager;

        public CacheRemoveAspect(string pattern)
        {
            _pattern = pattern;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }

        protected override void OnSuccess(IInvocation invocation)
        {
            _cacheManager.RemoveByPattern(_pattern);
        }
    }
}
