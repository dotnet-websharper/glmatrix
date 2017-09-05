#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.GlMatrix")
        .VersionFrom("WebSharper", versionSpec = "(,4.0)")
        .WithFSharpVersion(FSharpVersion.FSharp30)
        .WithFramework(fun fw -> fw.Net40)
        .References(fun r -> [r.Assembly "System.Web"])

let main =
    bt.WebSharper.Extension("WebSharper.GlMatrix")
        .Embed(["gl-matrix-min.js"])
        .SourcesFromProject()

let test =
    bt.WebSharper.HtmlWebsite("WebSharper.GlMatrix.Tests")
        .SourcesFromProject()
        .References(fun r ->
            [
                r.Project main
                r.NuGet("WebSharper.Html").Version("(,4.0)").Reference()
            ])

bt.Solution [
    main
    test

    bt.NuGet.CreatePackage()
        .Configure(fun c ->
            { c with
                Title = Some "WebSharper.GlMatrix-2.2.0"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://github.com/intellifactory/websharper.glmatrix"
                Description = "WebSharper Extensions for glMatrix 2.2.0"
                RequiresLicenseAcceptance = true })
        .Add(main)

]
|> bt.Dispatch
