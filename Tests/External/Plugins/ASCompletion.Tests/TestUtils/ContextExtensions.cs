using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASCompletion.Context;
using ASCompletion.Model;
using NSubstitute;
using PluginCore;
using PluginCore.Helpers;
using ProjectManager.Projects.Haxe;

namespace ASCompletion.TestUtils
{
    public static class ContextExtensions
    {
        public static void SetAs3Features(this IASContext mock)
        {
            var context = new AS3Context.Context(new AS3Context.AS3Settings());
            BuildClassPath(context);
            context.CurrentModel = new FileModel {Context = mock, Version = 3};
            SetFeatures(mock, context);
        }

        public static void SetHaxeFeatures(this IASContext mock)
        {
            var context = new HaXeContext.Context(new HaXeContext.HaXeSettings());
            BuildClassPath(context);
            context.CurrentModel = new FileModel {Context = mock, Version = 4, haXe = true};
            SetFeatures(mock, context);
        }

        static void SetFeatures(IASContext mock, IASContext context)
        {
            mock.Settings.LanguageId.Returns(context.Settings.LanguageId);
            mock.Features.Returns(context.Features);
            mock.CurrentModel.Returns(context.CurrentModel);
            var visibleExternalElements = context.GetVisibleExternalElements();
            mock.GetVisibleExternalElements().Returns(visibleExternalElements);
            mock.GetCodeModel(null).ReturnsForAnyArgs(x =>
            {
                var src = x[0] as string;
                return string.IsNullOrEmpty(src) ? null : context.GetCodeModel(src);
            });
            mock.IsImported(null, Arg.Any<int>()).ReturnsForAnyArgs(it =>
            {
                var member = it.ArgAt<MemberModel>(0);
                return member != null && context.IsImported(member, it.ArgAt<int>(1));
            });
            mock.ResolveType(null, null).ReturnsForAnyArgs(x => context.ResolveType(x.ArgAt<string>(0), x.ArgAt<FileModel>(1)));
            mock.IsFileValid.Returns(context.IsFileValid);
            mock.GetDefaultValue(null).ReturnsForAnyArgs(it => context.GetDefaultValue(it.ArgAt<string>(0)));
            mock.DecomposeTypes(null).ReturnsForAnyArgs(it => context.DecomposeTypes(it.ArgAt<IEnumerable<string>>(0) ?? new string[0]));
            mock.Classpath.Returns(context.Classpath);
            mock.CreateFileModel(null).ReturnsForAnyArgs(it => context.CreateFileModel(it.ArgAt<string>(0)));
            var allProjectClasses = context.GetAllProjectClasses();
            mock.GetAllProjectClasses().Returns(allProjectClasses);
        }

        public static void BuildClassPath(this IASContext context)
        {
            if (context is AS3Context.Context) BuildClassPath((AS3Context.Context) context);
            else if (context is HaXeContext.Context) BuildClassPath((HaXeContext.Context) context);
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
                            it.AddFile(ASFileParser.ParseFile(new FileModel(fileName) {Context = context, Version = 3}));
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
                        it.AddFile(ASFileParser.ParseFile(new FileModel(fileName) {Context = context, haXe = true, Version = 4}));
                    }
                }
                context.RefreshContextCache(path);
            }
        }
    }
}
