﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using NSubstitute;
using PluginCore;
using PluginCore.Helpers;
using ProjectManager.Projects.Haxe;
using ScintillaNet;

namespace ASCompletion.TestUtils
{
    public static class ContextExtensions
    {
        public static void SetAs3Features(this IASContext mock)
        {
            var context = new AS3Context.Context(new AS3Context.AS3Settings());
            ASContext.RegisterLanguage(context, "as3");
            BuildClassPath(context);
            context.CurrentModel = new FileModel {Context = mock, Version = 3};
            SetFeatures(mock, context);
        }

        public static void SetHaxeFeatures(this IASContext mock)
        {
            var context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
            ASContext.RegisterLanguage(context, "haxe");
            BuildClassPath(context);
            context.CurrentModel = new FileModel {Context = mock, Version = 4, haXe = true};
            SetFeatures(mock, context);
        }

        static void SetFeatures(IASContext mock, IASContext context)
        {
            mock.Settings.Returns(context.Settings);
            mock.Features.Returns(context.Features);
            mock.CurrentModel.Returns(context.CurrentModel);
            var visibleExternalElements = context.GetVisibleExternalElements();
            mock.GetVisibleExternalElements().Returns(visibleExternalElements);
            mock.GetCodeModel(null).ReturnsForAnyArgs(x =>
            {
                var src = x[0] as string;
                return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(src);
            });
            mock.GetCodeModel(Arg.Any<string>(), Arg.Any<bool>()).ReturnsForAnyArgs(x =>
            {
                var src = x[0] as string;
                return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(src, x.ArgAt<bool>(1));
            });
            mock.GetCodeModel(Arg.Any<FileModel>(), Arg.Any<string>()).ReturnsForAnyArgs(x =>
            {
                var src = x[1] as string;
                return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(x.ArgAt<FileModel>(0), src);
            });
            mock.GetCodeModel(null, null, Arg.Any<bool>()).ReturnsForAnyArgs(x =>
            {
                var src = x[1] as string;
                return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(x.ArgAt<FileModel>(0), src, x.ArgAt<bool>(2));
            });
            mock.GetFileModel(null).ReturnsForAnyArgs(it =>
            {
                var fileName = it[0] as string;
                return fileName == null ? null : context.GetFileModel(fileName);
            });
            mock.IsImported(null, Arg.Any<int>()).ReturnsForAnyArgs(it =>
            {
                var member = it.ArgAt<MemberModel>(0) ?? ClassModel.VoidClass;
                return context.IsImported(member, it.ArgAt<int>(1));
            });
            mock.ResolveType(null, null).ReturnsForAnyArgs(x => context.ResolveType(x.ArgAt<string>(0), x.ArgAt<FileModel>(1)));
            mock.ResolveToken(null, null).ReturnsForAnyArgs(x => context.ResolveToken(x.ArgAt<string>(0), x.ArgAt<FileModel>(1)));
            mock.ResolveDotContext(null, null, false).ReturnsForAnyArgs(it =>
            {
                var expr = it.ArgAt<ASExpr>(1);
                return expr == null ? null : context.ResolveDotContext(it.ArgAt<ScintillaControl>(0), expr, it.ArgAt<bool>(2));
            });
            mock.TypesAffinity(null, null).ReturnsForAnyArgs(it =>
            {
                var inClass = it.ArgAt<ClassModel>(0);
                var withClass = it.ArgAt<ClassModel>(1);
                return inClass == null || withClass == null ? Visibility.Default : context.TypesAffinity(inClass, withClass);
            });
            mock.IsFileValid.Returns(context.IsFileValid);
            mock.GetDefaultValue(null).ReturnsForAnyArgs(it => context.GetDefaultValue(it.ArgAt<string>(0)));
            mock.DecomposeTypes(null).ReturnsForAnyArgs(it => context.DecomposeTypes(it.ArgAt<IEnumerable<string>>(0) ?? new string[0]));
            mock.Classpath.Returns(context.Classpath);
            mock.CreateFileModel(null).ReturnsForAnyArgs(it => context.CreateFileModel(it.ArgAt<string>(0)));
            var allProjectClasses = context.GetAllProjectClasses();
            mock.GetAllProjectClasses().Returns(allProjectClasses);
            mock.CodeGenerator.Returns(context.CodeGenerator);
            mock.DocumentationGenerator.Returns(context.DocumentationGenerator);
            mock.CodeComplete.Returns(context.CodeComplete);
        }

        static void BuildClassPath(AS3Context.Context context)
        {
            context.BuildClassPath();
            var intrinsicPath = $"{PathHelper.LibraryDir}{Path.DirectorySeparatorChar}AS3{Path.DirectorySeparatorChar}intrinsic";
            context.Classpath.AddRange(Directory.GetDirectories(intrinsicPath).Select(it => new PathModel(it, context)));
            foreach (var it in context.Classpath)
            {
                if (it.IsVirtual) context.ExploreVirtualPath(it);
                else
                {
                    var path = it.Path;
                    foreach (var searchPattern in context.GetExplorerMask())
                    {
                        foreach (var fileName in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
                        {
                            it.AddFile(context.GetFileModel(fileName));
                        }
                    }
                    context.RefreshContextCache(path);
                }
            }
        }

        static void BuildClassPath(HaXeContext.Context context)
        {
            var platformsFile = Path.Combine("Settings", "Platforms");
            PlatformData.Load(Path.Combine(PathHelper.AppDir, platformsFile));
            PluginBase.CurrentProject = new HaxeProject("haxe")
            {
                CurrentSDK = Environment.GetEnvironmentVariable("HAXEPATH")
            };
            context.BuildClassPath();
            foreach (var it in context.Classpath)
            {
                var path = it.Path;
                foreach (var searchPattern in context.GetExplorerMask())
                {
                    foreach (var fileName in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
                    {
                        it.AddFile(context.GetFileModel(fileName));
                    }
                }
                context.RefreshContextCache(path);
            }
        }
    }
}
