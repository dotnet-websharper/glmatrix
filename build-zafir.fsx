#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("Zafir.GlMatrix")
        .VersionFrom("Zafir")
        .WithFSharpVersion(FSharpVersion.FSharp30)
        .WithFramework(fun fw -> fw.Net40)
        .References(fun r -> [r.Assembly "System.Web"])

let main =
    bt.Zafir.Extension("WebSharper.GlMatrix")
        .Embed(["gl-matrix-min.js"])
        .SourcesFromProject()

let test =
    bt.Zafir.HtmlWebsite("WebSharper.GlMatrix.Tests")
        .SourcesFromProject()
        .References(fun r ->
            [
                r.Project main
                r.NuGet("Zafir.Html").Latest(true).ForceFoundVersion().Reference()
            ])

bt.Solution [
    main
    test

    bt.NuGet.CreatePackage()
        .Configure(fun c ->
            { c with
                Title = Some "Zafir.GlMatrix-2.2.0"
                LicenseUrl = Some "http://websharper.com/licensing"
                ProjectUrl = Some "https://github.com/intellifactory/websharper.glmatrix"
                Description = "Zafir Extensions for glMatrix 2.2.0"
                RequiresLicenseAcceptance = true })
        .Add(main)

]
|> bt.Dispatch
