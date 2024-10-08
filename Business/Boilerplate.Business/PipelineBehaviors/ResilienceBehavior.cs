﻿using MediatR;

using Polly.Retry;
using Polly;

namespace Boilerplate.Business.PipelineBehaviors
{
    // TODO: Farklı Resilience stratejilerine göre farklı behaviorlar yazmak gerekebilir.
    internal class ResilienceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ResiliencePipeline _pipeline;

        public ResilienceBehavior()
        {
            _pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions()
                {
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromSeconds(2),
                    MaxRetryAttempts = 3,
                    ShouldHandle = new PredicateBuilder().Handle<Exception>()
                })
                .AddTimeout(TimeSpan.FromSeconds(15))
                .Build();
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            return await _pipeline.AsAsyncPolicy().ExecuteAsync(async (_next) =>
            {
                return await next();
            }, cancellationToken);
        }
    }
}
