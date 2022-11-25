using Grpc.Core;
using GrpcExample;

namespace GrpcGreeter.Services
{
    public class ExampleService : Example.ExampleBase
    {
        /// <summary>
        /// 一元方法
        /// 一元方法将请求消息作为参数，并返回响应。 返回响应时，一元调用完成
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ExampleResponse> UnaryCall(ExampleRequest request,
        ServerCallContext context)
        {
            //访问 gRPC 请求标头
            var userAgent = context.RequestHeaders.GetValue("user-agent");

            var response = new ExampleResponse();
            return Task.FromResult(response);
        }

        /// <summary>
        /// 服务器流式处理方法
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task StreamingFromServer(ExampleRequest request,IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            for (var i = 0; i < 5; i++)
            {
                await responseStream.WriteAsync(new ExampleResponse());
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
        /// <summary>
        /// 服务器流式处理方法2
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task StreamingFromServer2(ExampleRequest request,IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await responseStream.WriteAsync(new ExampleResponse());
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
        }

        /// <summary>
        /// 客户端流式处理方法
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ExampleResponse> StreamingFromClient(IAsyncStreamReader<ExampleRequest> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var message = requestStream.Current;
                // ...
            }
            return new ExampleResponse();
        }
        /// <summary>
        /// 客户端流式处理方法2
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ExampleResponse> StreamingFromClient2(IAsyncStreamReader<ExampleRequest> requestStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                // ...
            }
            return new ExampleResponse();
        }

        /// <summary>
        /// 双向流式处理方法
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task StreamingBothWays(IAsyncStreamReader<ExampleRequest> requestStream,IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                await responseStream.WriteAsync(new ExampleResponse());
            }
        }
        /// <summary>
        /// 双向流式处理方法2
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task StreamingBothWays2(IAsyncStreamReader<ExampleRequest> requestStream,IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            // Read requests in a background task.
            var readTask = Task.Run(async () =>
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    // Process request.
                }
            });

            // Send responses until the client signals that it is complete.
            while (!readTask.IsCompleted)
            {
                await responseStream.WriteAsync(new ExampleResponse());
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
        }
    }
}
