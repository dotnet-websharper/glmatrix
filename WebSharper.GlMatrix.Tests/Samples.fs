namespace WebSharper.GlMatrix.Tests

open WebSharper

module Main =
    open WebSharper.Html.Client
    open WebSharper.JavaScript
    open WebSharper.GlMatrix

    [<Inline "G_vmlCanvasManager.initElement($elem)">]
    let Initialize (elem: CanvasElement) : unit = ()

    [<JavaScript>]
    let CreateContext (element : Element) =
        let canvas = As<CanvasElement> element.Dom
        if canvas?getContext = JS.Undefined then
            Initialize canvas
        canvas.Width <- 400
        canvas.Height <- 300
        ["webgl"; "experimental-webgl"]
        |> List.tryPick (fun s ->
            let gl = As<WebGL.RenderingContext> (canvas.GetContext s)
            if gl = null then None else Some gl)

    [<JavaScript>]
    let BasicContext () =
        let element = Tags.Canvas []
        match CreateContext element with
        | None -> ()
        | Some gl ->
            gl.ClearColor(0.2, 0., 0.4, 1.)
            gl.Clear(gl.COLOR_BUFFER_BIT)
            gl.Flush()
        element

    [<JavaScript>]
    let BasicVertexShader = "
precision highp float;
attribute vec3 position;
attribute vec2 texCoord;
uniform mat4 perspectiveMatrix;
uniform mat4  modelViewMatrix;
varying vec2 textureCoord;
void main(void)
{
    gl_Position = perspectiveMatrix * modelViewMatrix * vec4(position, 1.0);
    textureCoord = texCoord;
}"

    [<JavaScript>]
    let BasicFragmentShader = "
precision highp float;
void main(void)
{
    gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
}"

    [<JavaScript>]
    let TexturingFragmentShader = "
precision highp float;
uniform sampler2D tex;
varying vec2 textureCoord;
void main(void)
{
    gl_FragColor = texture2D(tex, textureCoord);
}"

    [<JavaScript>]
    let CreateProgram (gl : WebGL.RenderingContext, vertexSource, fragmentSource) =
        let vs = gl.CreateShader(gl.VERTEX_SHADER)
        gl.ShaderSource(vs, vertexSource)
        gl.CompileShader(vs)
        if not(As<bool>(gl.GetShaderParameter(vs, gl.COMPILE_STATUS))) then
            JS.Alert(
                "Couldn't compile the vertex shader:\n" +
                gl.GetShaderInfoLog(vs))
            gl.DeleteShader(vs)
        let fs = gl.CreateShader(gl.FRAGMENT_SHADER)
        gl.ShaderSource(fs, fragmentSource)
        gl.CompileShader(fs)
        if not(As<bool>(gl.GetShaderParameter(fs, gl.COMPILE_STATUS))) then
            JS.Alert(
                "Couldn't compile the fragment shader:\n" +
                gl.GetShaderInfoLog(fs))
            gl.DeleteShader(fs)
        let program = gl.CreateProgram()
        gl.AttachShader(program, vs)
        gl.AttachShader(program, fs)
        gl.LinkProgram(program)
        if not(As<bool>(gl.GetProgramParameter(program, gl.LINK_STATUS))) then
            JS.Alert(
                "Couldn't link the shader program:\n" +
                gl.GetProgramInfoLog(program))
            gl.DeleteProgram(program)
            gl.DeleteShader(fs)
            gl.DeleteShader(fs)
        program

    [<JavaScript>]
    let SetupView (gl : WebGL.RenderingContext, program) =
        let fieldOfView = 45.
        let aspectRatio = 4./3.
        let nearPlane = 1.
        let farPlane = 10000.
        let perspectiveMatrix = Mat4.Perspective(Mat4.Create(), fieldOfView, aspectRatio, nearPlane, farPlane)
        let uPerspectiveMatrix = gl.GetUniformLocation(program, "perspectiveMatrix")
        gl.UniformMatrix4fv(uPerspectiveMatrix, false, As<Float32Array> perspectiveMatrix)

    [<JavaScript>]
    let DrawRotatingObject (gl : WebGL.RenderingContext,
                            program, buf, numVertices) =
        gl.ClearColor(0., 0., 0., 0.)
        gl.UseProgram(program)
        SetupView(gl, program)
        let uModelViewMatrix = gl.GetUniformLocation(program, "modelViewMatrix")
        let rec RunFrame (i : int) () =
            let angle = 2. * float i * System.Math.PI / 1000.
            let modelViewMatrix = Mat4.Identity(Mat4.Create())
            let modelViewMatrix = Mat4.Translate(modelViewMatrix, modelViewMatrix, Vec3.FromValues(0., 0., -4.))
            let modelViewMatrix = Mat4.RotateY(modelViewMatrix, modelViewMatrix, angle)
            gl.UniformMatrix4fv(uModelViewMatrix, false, As<Float32Array> modelViewMatrix)
            gl.Clear(gl.COLOR_BUFFER_BIT ||| gl.DEPTH_BUFFER_BIT)
            gl.BindBuffer(gl.ARRAY_BUFFER, buf)
            gl.DrawArrays(gl.TRIANGLES, 0, numVertices)
            gl.Flush()
            JS.SetTimeout (RunFrame ((i + 20) % 1000)) 20 |> ignore
        RunFrame 0 ()

    [<JavaScript>]
    let DrawTriangle () =
        let canvas = Tags.Canvas []
        match CreateContext canvas with
        | None -> JS.Alert "Couldn't create WebGL context."
        | Some gl ->
            let program = CreateProgram(gl, BasicVertexShader, BasicFragmentShader)
            let vertexPosition = gl.GetAttribLocation(program, "position")
            gl.EnableVertexAttribArray(vertexPosition)
            let vertexBuffer = gl.CreateBuffer()
            let vertices = Float32Array([| 0.0f;  1.0f; 0.0f;
                                          -1.0f; -1.0f; 0.0f;
                                           1.0f; -1.0f; 0.0f; |])
            gl.BindBuffer(gl.ARRAY_BUFFER, vertexBuffer)
            gl.BufferData(gl.ARRAY_BUFFER, vertices, gl.STATIC_DRAW)
            gl.VertexAttribPointer(vertexPosition, 3, gl.FLOAT, false, 0, 0)
            DrawRotatingObject(gl, program, vertexBuffer, 3)
        canvas

    [<JavaScript>]
    let MakeAndBindTexture (gl : WebGL.RenderingContext) f =
        Img []
        |>! Events.OnLoad (fun img _ ->
            let tex = gl.CreateTexture()
            gl.ActiveTexture(gl.TEXTURE0)
            gl.BindTexture(gl.TEXTURE_2D, tex)
            gl.PixelStorei(gl.UNPACK_FLIP_Y_WEBGL, 1)
            gl.TexImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, img.Dom)
            gl.TexParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR)
            gl.TexParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR)
            f ())
        |> fun img -> img -< [Attr.Src "wslogo.jpg"]
        |> ignore

    [<JavaScript>]
    let CreateSquare (gl : WebGL.RenderingContext, program) =
        let vertexPosition = gl.GetAttribLocation(program, "position")
        gl.EnableVertexAttribArray(vertexPosition)
        let vertexTexCoord = gl.GetAttribLocation(program, "texCoord")
        gl.EnableVertexAttribArray(vertexTexCoord)
        let vertexBuffer = gl.CreateBuffer()
        let vertices = Float32Array([| -1.f; -1.f; 0.f; 0.f; 0.f;
                                       -1.f;  1.f; 0.f; 0.f; 1.f;
                                        1.f; -1.f; 0.f; 1.f; 0.f;
                                        1.f;  1.f; 0.f; 1.f; 1.f;
                                       -1.f;  1.f; 0.f; 0.f; 1.f;
                                        1.f; -1.f; 0.f; 1.f; 0.f; |])
        gl.BindBuffer(gl.ARRAY_BUFFER, vertexBuffer)
        gl.BufferData(gl.ARRAY_BUFFER, vertices, gl.STATIC_DRAW)
        let floatSize = Float32Array.BYTES_PER_ELEMENT
        gl.VertexAttribPointer(vertexPosition, 3, gl.FLOAT, false, 5*int floatSize, 0)
        gl.VertexAttribPointer(vertexTexCoord, 2, gl.FLOAT, false, 5*int floatSize, 3*int floatSize)
        (vertexBuffer, 6)

    [<JavaScript>]
    let DrawTexturedSquare () =
        let otherCanvas = Tags.Canvas []
        let canvas = Tags.Canvas [Attr.Style "position:absolute;"]
        match CreateContext canvas with
        | None -> JS.Alert "Couldn't create WebGL context."
        | Some gl ->
            let program = CreateProgram(gl, BasicVertexShader, TexturingFragmentShader)
            let vertexBuffer, numberVertices = CreateSquare(gl, program)
            MakeAndBindTexture gl <| fun () ->
                gl.UseProgram(program)
                let u_texture = gl.GetUniformLocation(program, "tex")
                gl.Uniform1i(u_texture, 0)
                DrawRotatingObject(gl, program, vertexBuffer, numberVertices)
        Div [Attr.Style "position:relative;"] -< [
            canvas
            P [Text "This sample shows how you can texture a polygon."]
            P [Text "One option is to simply grab the data from an <img> element."]
            P [Text "You can also see that a WebGL context can be placed anywhere."]]

    [<JavaScript>]
    let DrawTunnel () =
        let otherCanvas = Tags.Canvas []
        let canvas = Tags.Canvas []
        match CreateContext canvas with
        | None -> JS.Alert "Couldn't create WebGL context."
        | Some gl ->
            let program = CreateProgram(gl, BasicVertexShader, TexturingFragmentShader)
            let vertexBuffer, numberVertices = CreateSquare(gl, program)
            gl.UseProgram(program)
            let projectionMatrix = Mat4.Ortho(Mat4.Create(), -1., 1., -1., 1., 0., 1.)
            let uPerspectiveMatrix = gl.GetUniformLocation(program, "perspectiveMatrix")
            gl.UniformMatrix4fv(uPerspectiveMatrix, false, As<Float32Array> projectionMatrix)
            let uModelViewMatrix = gl.GetUniformLocation(program, "modelViewMatrix")
            gl.ClearColor(0., 0., 0., 0.)
            MakeAndBindTexture gl <| fun () ->
                let rec RunFrame (i : int) () =
                    let angle = 2. * float i * System.Math.PI / 1000.
                    let modelViewMatrix = Mat4.Identity(Mat4.Create())
                    let modelViewMatrix = Mat4.Scale(modelViewMatrix, modelViewMatrix, Vec3.FromValues(0.999**float i, 0.999**float i, 1.))
                    let modelViewMatrix = Mat4.RotateZ(modelViewMatrix, modelViewMatrix, angle)
                    gl.UniformMatrix4fv(uModelViewMatrix, false, As<Float32Array> modelViewMatrix)
                    gl.Clear(gl.DEPTH_BUFFER_BIT)
                    gl.DrawArrays(gl.TRIANGLES, 0, 6)
                    gl.Flush()
                    JS.SetTimeout (RunFrame ((i + 20) % 5000)) 20 |> ignore
                RunFrame 0 ()
        canvas

    [<JavaScript>]
    let Samples () =
        Div [
            H2 [Text "Basic 3D context"]
            BasicContext ()
            H2 [Text "Draw a triangle"]
            DrawTriangle ()
            H2 [Text "Draw a textured square"]
            DrawTunnel ()
            H2 [Text "Draw a textured square"]
            DrawTexturedSquare ()
        ]

type Samples() =
    inherit Web.Control()

    [<JavaScript>]
    override this.Body = Main.Samples() :> _


open WebSharper.Sitelets

type Action = | Index

module Site =

    open WebSharper.Html.Server

    let HomePage =
        Content.PageContent <| fun ctx ->
            { Page.Default with
                Title = Some "WebSharper glMatrix Tests"
                Body = [Div [new Samples()]] }

    let Main = Sitelet.Content "/" Index HomePage

[<Sealed>]
type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Site.Main
        member this.Actions = [Action.Index]

[<assembly: Website(typeof<Website>)>]
do ()
