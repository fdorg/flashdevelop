﻿using ASCompletion.Context;
using NSubstitute;

namespace ASCompletion.TestUtils
{
    public static class ContextExtensions
    {
        public static void SetAs3Features(this object context)
        {
            //var asContext = new AS3Context.Context(new AS3Context.AS3Settings());
            //context.Features.Returns(asContext.Features);
        }

        public static void SetHaxeFeatures(this object context)
        {
            //var haxeContext = new HaXeContext.Context(new HaXeContext.HaXeSettings());
            //context.Features.Returns(haxeContext.Features);
        }
    }
}
