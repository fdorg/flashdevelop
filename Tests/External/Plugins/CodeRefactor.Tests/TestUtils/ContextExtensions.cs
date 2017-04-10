// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using ASCompletion.Context;
using NSubstitute;

namespace CodeRefactor.TestUtils
{
    public static class ContextExtensions
    {
        public static void SetAs3Features(this IASContext context)
        {
            var asContext = new AS3Context.Context(new AS3Context.AS3Settings());
            context.Features.Returns(asContext.Features);
        }

        public static void SetHaxeFeatures(this IASContext context)
        {
            var haxeContext = new HaXeContext.Context(new HaXeContext.HaXeSettings());
            context.Features.Returns(haxeContext.Features);
        }
    }
}
