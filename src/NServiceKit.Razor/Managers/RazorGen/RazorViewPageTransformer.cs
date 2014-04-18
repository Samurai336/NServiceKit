using System;
using System.Collections.Generic;
using System.Web.Razor.Generator;
using NServiceKit.Razor.Compilation;
using NServiceKit.Razor.Compilation.CodeTransformers;

namespace NServiceKit.Razor.Managers.RazorGen
{
    public class RazorViewPageTransformer : AggregateCodeTransformer
    {
        public RazorViewPageTransformer(Type pageBaseType)
        {
            this.codeTransformers.Add(new SetBaseType(pageBaseType));
        }

        private static readonly HashSet<string> namespaces = new HashSet<string>()
            {
                "System",
            };

        private readonly List<RazorCodeTransformerBase> codeTransformers = new List<RazorCodeTransformerBase>
            {
                new AddGeneratedClassAttribute(),
                new AddPageVirtualPathAttribute(),
                new SetImports( namespaces, replaceExisting: false ),
                new RemoveLineHiddenPragmas(),
                new MakeTypePartial(),
                new WebConfigTransformer()
            };

        protected override IEnumerable<RazorCodeTransformerBase> CodeTransformers
        {
            get { return this.codeTransformers; }
        }

        public override void Initialize(RazorPageHost razorHost, IDictionary<string, string> directives)
        {
            base.Initialize(razorHost, directives);

            var path = razorHost.EnableLinePragmas ? razorHost.File.RealPath : string.Empty;
            razorHost.CodeGenerator = new NServiceKitCSharpRazorCodeGenerator(razorHost.DefaultClassName, razorHost.DefaultNamespace, path, razorHost)
                {
                    GenerateLinePragmas = razorHost.EnableLinePragmas
                };

        }
        public override void ProcessGeneratedCode(System.CodeDom.CodeCompileUnit codeCompileUnit, System.CodeDom.CodeNamespace generatedNamespace, System.CodeDom.CodeTypeDeclaration generatedClass, System.CodeDom.CodeMemberMethod executeMethod)
        {
            base.ProcessGeneratedCode(codeCompileUnit, generatedNamespace, generatedClass, executeMethod);
        }
    }
}