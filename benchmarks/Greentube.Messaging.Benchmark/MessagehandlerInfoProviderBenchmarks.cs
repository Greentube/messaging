using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using FastExpressionCompiler;

namespace Greentube.Messaging.Benchmarks
{
    //BenchmarkDotNet=v0.10.12, OS=Mac OS X 10.12
    //Unknown processor
    //.NET Core SDK=2.0.3
    //[Host]     : .NET Core 2.0.5 (Framework 4.6.0.0), 64bit RyuJIT
    //DefaultJob : .NET Core 2.0.5 (Framework 4.6.0.0), 64bit RyuJIT
    //
    //
    //                Method |       Mean |     Error |    StdDev | Scaled | ScaledSD |  Gen 0 |  Gen 1 |  Gen 2 | Allocated |
    //---------------------- |-----------:|----------:|----------:|-------:|---------:|-------:|-------:|-------:|----------:|
    //              Overhead |   5.923 us | 0.0595 us | 0.0557 us |   1.00 |     0.00 | 0.6256 |      - |      - |   1.94 KB |
    //     ExpressionCompile | 222.876 us | 1.3839 us | 1.2945 us |  37.63 |     0.40 | 1.9531 | 0.9766 |      - |   6.34 KB |
    // ExpressionCompileFast |  18.170 us | 0.1703 us | 0.1510 us |   3.07 |     0.04 | 1.0986 | 0.5493 | 0.0305 |    3.4 KB |

    [MemoryDiagnoser]
    public class MessagehandlerInfoProviderBenchmarks
    {
        private readonly IEnumerable<Type> Types =  new [] { typeof(int) };

        [Benchmark(Baseline = true)]
        public IEnumerable<(
                Type messageType,
                Type handlerType,
                Expression<Func<object, object, CancellationToken, Task>> handleMethod)>
            Overhead()
            => HandlerInformationProvider().ToList();

        [Benchmark]
        public IEnumerable<(
            Type messageType,
            Type handlerType,
            Expression<Func<object, object, CancellationToken, Task>> handleMethod)> ExpressionCompile()
        {
            var handlerInfos = HandlerInformationProvider();

            foreach (var handlerInfo in handlerInfos)
            {
                handlerInfo.handlerFunction.Compile();
            }

            return handlerInfos;
        }

        [Benchmark]
        public IEnumerable<(
            Type messageType,
            Type handlerType,
            Expression<Func<object, object, CancellationToken, Task>> handleMethod)>
            ExpressionCompileFast()
        {
            var handlerInfos = HandlerInformationProvider();

            foreach (var handlerInfo in handlerInfos)
            {
                handlerInfo.handlerFunction.CompileFast();
            }

            return handlerInfos;
        }

        // Relevant portion of MessagehandlerInfoProvider to isolate the target of the benchmark: Compiling the expression
        private IEnumerable<
                (Type messageType,
                Type handlerType,
                Expression<Func<object, object, CancellationToken, Task>> handlerFunction)>
            HandlerInformationProvider()
        {
            return from messageType in Types
                let handlerType = typeof(IMessageHandler<>).MakeGenericType(messageType)
                let handleMethod = handlerType.GetTypeInfo().GetMethod(nameof(IMessageHandler<object>.Handle))
                let handlerInstance = Expression.Parameter(typeof(object))
                let messageInstance = Expression.Parameter(typeof(object))
                let tokenInstance = Expression.Parameter(typeof(CancellationToken))
                let handleFunc = Expression.Lambda<Func<object, object, CancellationToken, Task>>(
                    Expression.Call(
                        Expression.Convert(
                            handlerInstance,
                            handlerType),
                        handleMethod,
                        Expression.Convert(
                            messageInstance,
                            messageType),
                        tokenInstance),
                    handlerInstance,
                    messageInstance,
                    tokenInstance)
                select (messageType, handlerType, handleFunc);
        }
    }
}