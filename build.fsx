#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("WebSharper.GlMatrix", "2.5")
        .References(fun r -> [r.Assembly "System.Web"])

let main =
    bt.WebSharper.Extension("IntelliFactory.WebSharper.GlMatrix")
        .Embed(["gl-matrix-min.js"])
        .SourcesFromProject()

let test =
    bt.WebSharper.HtmlWebsite("IntelliFactory.WebSharper.GlMatrix.Tests")
        .SourcesFromProject()
        .References(fun r -> [r.Project main])

bt.Solution [
    main
    test

    bt.NuGet.CreatePackage()
        .Description("WebSharper Extensions for glMatrix 2.2.0")
        .ProjectUrl("https://github.com/intellifactory/websharper.glmatrix")
        .Add(main)

]
|> bt.Dispatch
