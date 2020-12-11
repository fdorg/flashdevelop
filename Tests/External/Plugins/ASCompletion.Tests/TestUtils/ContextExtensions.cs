// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using NSubstitute;
using PluginCore;
using PluginCore.Helpers;
using ProjectManager.Projects.AS3;
using ProjectManager.Projects.Haxe;
using ScintillaNet;

namespace ASCompletion.TestUtils
{
    public static class ContextExtensions
    {
        public static void SetAs3Features(this IASContext mock)
        {
            var ctx = new AS3Context.Context(new AS3Context.AS3Settings());
            ASContext.RegisterLanguage(ctx, "as3");
            BuildClassPath(ctx);
            ctx.CurrentModel = new FileModel {Context = mock, Version = 3};
            SetFeatures(mock, ctx);
            mock.GetTopLevelElements()
                .Returns(it =>
                {
                    ctx.completionCache.IsDirty = true;
                    return ctx.GetTopLevelElements();
                });
            mock.When(it => it.ResolveTopLevelElement(Arg.Any<string>(), Arg.Any<ASResult>()))
                .Do(it => ctx.ResolveTopLevelElement(it.ArgAt<string>(0), it.ArgAt<ASResult>(1)));
        }

        public static void SetHaxeFeatures(this IASContext mock)
        {
            var ctx = new HaXeContext.Context(new HaXeContext.HaXeSettings());
            ASContext.RegisterLanguage(ctx, "haxe");
            BuildClassPath(ctx);
            ctx.CurrentModel = new FileModel {Context = mock, Version = 4, haXe = true};
            SetFeatures(mock, ctx);
            mock.GetTopLevelElements()
                .Returns(it =>
                {
                    ctx.completionCache.IsDirty = true;
                    return ctx.GetTopLevelElements();
                });
            mock.When(it => it.ResolveTopLevelElement(Arg.Any<string>(), Arg.Any<ASResult>()))
                .Do(it =>
                {
                    var topLevel = new FileModel();
                    topLevel.Members.Add(new MemberModel("this", mock.CurrentClass.Name, FlagType.Variable, Visibility.Public) {InFile = mock.CurrentModel});
                    topLevel.Members.Add(new MemberModel("super", mock.CurrentClass.ExtendsType, FlagType.Variable, Visibility.Public) {InFile = mock.CurrentModel});
                    ctx.TopLevel = topLevel;
                    ctx.completionCache.IsDirty = true;
                    ctx.GetTopLevelElements();
                    ctx.ResolveTopLevelElement(it.ArgAt<string>(0), it.ArgAt<ASResult>(1));
                });
        }

        static void SetFeatures(IASContext mock, IASContext ctx)
        {
            ClassModel.VoidClass.Name = ctx.Features.voidKey;
            mock.Settings.Returns(ctx.Settings);
            mock.Features.Returns(ctx.Features);
            mock.CurrentModel.Returns(ctx.CurrentModel);
            var visibleExternalElements = ctx.GetVisibleExternalElements();
            mock.GetVisibleExternalElements().Returns(visibleExternalElements);
            mock.GetCodeModel((string)null)
                .ReturnsForAnyArgs(x =>
                {
                    var src = x[0] as string;
                    return !string.IsNullOrEmpty(src) ? ctx.GetCodeModel(src) : null;
                });
            mock.GetCodeModel((FileModel)null)
                .ReturnsForAnyArgs(x => x[0] is FileModel src ? ctx.GetCodeModel(src) : null);
            mock.GetCodeModel(Arg.Any<string>(), Arg.Any<bool>())
                .ReturnsForAnyArgs(x =>
                {
                    var src = x[0] as string;
                    return !string.IsNullOrEmpty(src) ? ctx.GetCodeModel(src, x.ArgAt<bool>(1)) : null;
                });
            mock.GetCodeModel(Arg.Any<FileModel>(), Arg.Any<string>())
                .ReturnsForAnyArgs(x =>
                {
                    var src = x[1] as string;
                    return !string.IsNullOrEmpty(src) ? ctx.GetCodeModel(x.ArgAt<FileModel>(0), src) : null;
                });
            mock.GetCodeModel(null, null, Arg.Any<bool>())
                .ReturnsForAnyArgs(x =>
                {
                    var src = x[1] as string;
                    return !string.IsNullOrEmpty(src) ? ctx.GetCodeModel(x.ArgAt<FileModel>(0), src, x.ArgAt<bool>(2)) : null;
                });
            mock.GetFileModel(null)
                .ReturnsForAnyArgs(it => it[0] is string fileName ? ctx.GetFileModel(fileName) : null);
            mock.IsImported(null, Arg.Any<int>())
                .ReturnsForAnyArgs(it => ctx.IsImported(it.ArgAt<MemberModel>(0) ?? ClassModel.VoidClass, it.ArgAt<int>(1)));
            mock.OnCompletionInsert(Arg.Any<ScintillaControl>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<char>())
                .ReturnsForAnyArgs(it => ctx.OnCompletionInsert(it.ArgAt<ScintillaControl>(0), it.ArgAt<int>(1), it.ArgAt<string>(2), it.ArgAt<char>(3)));
            mock.ResolveImports(null)
                .ReturnsForAnyArgs(it => ctx.ResolveImports(it.ArgAt<FileModel>(0)));
            mock.ResolveType(null, null)
                .ReturnsForAnyArgs(x => ctx.ResolveType(x.ArgAt<string>(0), x.ArgAt<FileModel>(1)));
            mock.ResolveToken(null, null)
                .ReturnsForAnyArgs(x => ctx.ResolveToken(x.ArgAt<string>(0), x.ArgAt<FileModel>(1)));
            mock.ResolveDotContext(null, null, false)
                .ReturnsForAnyArgs(it => it.ArgAt<ASExpr>(1) is {} expr ? ctx.ResolveDotContext(it.ArgAt<ScintillaControl>(0), expr, it.ArgAt<bool>(2)) : null);
            mock.When(it => it.ResolveDotContext(Arg.Any<ScintillaControl>(), Arg.Any<ASResult>(), Arg.Any<MemberList>()))
                .Do(it => ctx.ResolveDotContext(it.ArgAt<ScintillaControl>(0), it.ArgAt<ASResult>(1), it.ArgAt<MemberList>(2)));
            mock.ResolvePackage(null, false)
                .ReturnsForAnyArgs(it => ctx.ResolvePackage(it.ArgAt<string>(0), it.ArgAt<bool>(1)));
            mock.TypesAffinity(null, null)
                .ReturnsForAnyArgs(it =>
                {
                    var inClass = it.ArgAt<ClassModel>(0);
                    var withClass = it.ArgAt<ClassModel>(1);
                    return inClass is null || withClass is null ? Visibility.Default : ctx.TypesAffinity(inClass, withClass);
                });
            mock.IsFileValid.Returns(ctx.IsFileValid);
            mock.GetDefaultValue(null)
                .ReturnsForAnyArgs(it => ctx.GetDefaultValue(it.ArgAt<string>(0)));
            mock.DecomposeTypes(null)
                .ReturnsForAnyArgs(it => ctx.DecomposeTypes(it.ArgAt<IEnumerable<string>>(0) ?? Array.Empty<string>()));
            mock.Classpath.Returns(ctx.Classpath);
            mock.CreateFileModel(null)
                .ReturnsForAnyArgs(it => ctx.CreateFileModel(it.ArgAt<string>(0)));
            var allProjectClasses = ctx.GetAllProjectClasses();
            mock.GetAllProjectClasses().Returns(allProjectClasses);
            mock.CodeGenerator.Returns(ctx.CodeGenerator);
            mock.DocumentationGenerator.Returns(ctx.DocumentationGenerator);
            mock.CodeComplete.Returns(ctx.CodeComplete);
        }

        static void BuildClassPath(AS3Context.Context ctx)
        {
            PlatformData.Load(Path.Combine(PathHelper.AppDir, "Settings", "Platforms"));
            if (!(PluginBase.CurrentProject is AS3Project)) PluginBase.CurrentProject = new AS3Project("as3");
            ctx.BuildClassPath();
            var intrinsicPath = $"{PathHelper.LibraryDir}{Path.DirectorySeparatorChar}AS3{Path.DirectorySeparatorChar}intrinsic";
            ctx.Classpath.AddRange(Directory.GetDirectories(intrinsicPath).Select(it => new PathModel(it, ctx)));
            foreach (var it in ctx.Classpath)
            {
                if (it.IsVirtual) ctx.ExploreVirtualPath(it);
                else
                {
                    var path = it.Path;
                    foreach (var searchPattern in ctx.GetExplorerMask())
                    {
                        foreach (var fileName in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
                        {
                            it.AddFile(ctx.GetFileModel(fileName));
                        }
                    }
                    ctx.RefreshContextCache(path);
                }
            }
        }

        static void BuildClassPath(HaXeContext.Context ctx)
        {
            PlatformData.Load(Path.Combine(PathHelper.AppDir, "Settings", "Platforms"));
            if (!(PluginBase.CurrentProject is HaxeProject))
            {
                PluginBase.CurrentProject = new HaxeProject("haxe")
                {
                    CurrentSDK = Environment.GetEnvironmentVariable("HAXEPATH")?.TrimEnd('\\', '/')
                };
            }
            ctx.BuildClassPath();
            foreach (var it in ctx.Classpath)
            {
                var path = it.Path;
                foreach (var searchPattern in ctx.GetExplorerMask())
                {
                    foreach (var fileName in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
                    {
                        it.AddFile(ctx.GetFileModel(fileName));
                    }
                }
                ctx.RefreshContextCache(path);
            }
        }
    }
}
