namespace WebSharper.GlMatrix.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.GlMatrix

[<JavaScript>]
module Client =

    // The template is the whole document, loaded from the DOM, so editing
    // index.html and refreshing the browser is enough.
    type IndexTemplate = Template<"index.html", ClientLoad.FromDocument>

    // ---------------------------------------------------------------
    // State bound into the template
    // ---------------------------------------------------------------

    let notice =
        Var.Create "Every number below is produced by the gl-matrix npm package through the binding — no hand-rolled maths."

    let sceneStatus = Var.Create "Starting WebGL…"
    let vecA = Var.Create "-"
    let vecB = Var.Create "-"
    let vecResult = Var.Create "-"
    let matrix = Var.Create ""
    let matInfo = Var.Create "-"
    let quatText = Var.Create "-"
    let quat2Text = Var.Create "-"
    let quatInfo = Var.Create "-"
    let utils = Var.Create "-"
    let forEachText = Var.Create "-"
    let ldu = Var.Create "-"

    // Theme: light is the default; the choice is persisted in localStorage.
    // The storage key is shared with the bootstrap script in index.html.
    let themeStorageKey = "websharper-glmatrix-theme"
    let theme = Var.Create "light"
    let themeLabel = Var.Create "🌙 Dark mode"

    let applyTheme () =
        JS.Document.DocumentElement.SetAttribute("data-theme", theme.Value)
        themeLabel.Value <-
            if theme.Value = "dark" then "☀️ Light mode" else "🌙 Dark mode"

    let toggleTheme () =
        theme.Value <- (if theme.Value = "dark" then "light" else "dark")
        try JS.Window.LocalStorage.SetItem(themeStorageKey, theme.Value) with _ -> ()
        applyTheme ()

    // ---------------------------------------------------------------
    // Formatting helpers
    // ---------------------------------------------------------------

    let fmt (x: float) =
        let r = System.Math.Round(x * 10000.) / 10000.
        if r = 0. then "0" else string r

    /// Prints a column-major matrix as row-major text, using the binding's own
    /// indexer on the matrix type.
    let fmtMat4 (m: Mat4) =
        [ for r in 0 .. 3 ->
            "  " + ([ for c in 0 .. 3 -> fmt m.[c * 4 + r] ] |> String.concat "  ") ]
        |> String.concat "\n"

    let fmtMat3 (m: Mat3) =
        [ for r in 0 .. 2 ->
            "  " + ([ for c in 0 .. 2 -> fmt m.[c * 3 + r] ] |> String.concat "  ") ]
        |> String.concat "\n"

    let fmtMat2 (m: Mat2) =
        [ for r in 0 .. 1 ->
            "  " + ([ for c in 0 .. 1 -> fmt m.[c * 2 + r] ] |> String.concat "  ") ]
        |> String.concat "\n"

    let fmtVec list = list |> List.map fmt |> String.concat ", "

    // ---------------------------------------------------------------
    // WebGL scene: a spinning cube built from the binding's matrix and
    // quaternion types.
    // ---------------------------------------------------------------

    let vertexShader = "
precision highp float;
attribute vec3 aPosition;
attribute vec3 aNormal;
attribute vec3 aColor;
uniform mat4 uProjection;
uniform mat4 uView;
uniform mat4 uModel;
varying vec3 vNormal;
varying vec3 vColor;
void main(void) {
    vec4 world = uModel * vec4(aPosition, 1.0);
    vNormal = mat3(uModel) * aNormal;
    vColor = aColor;
    gl_Position = uProjection * uView * world;
}"

    let fragmentShader = "
precision mediump float;
varying vec3 vNormal;
varying vec3 vColor;
uniform float uFlat;
void main(void) {
    vec3 n = normalize(vNormal);
    vec3 l = normalize(vec3(0.5, 0.9, 0.7));
    float diffuse = max(dot(n, l), 0.0);
    vec3 lit = vColor * (0.35 + 0.65 * diffuse);
    gl_FragColor = vec4(mix(lit, vColor, uFlat), 1.0);
}"

    /// Builds a unit cube with per-face normals and colours, packed as
    /// position + normal + colour (9 floats per vertex, 36 vertices).
    let buildCube () =
        let data = ResizeArray<float32>()
        let push (x, y, z) = data.Add (float32 x); data.Add (float32 y); data.Add (float32 z)
        let face (n: float * float * float) (verts: (float * float * float)[]) (r, g, b) =
            let (nx, ny, nz) = n
            let vertex (x, y, z) =
                push (x, y, z)
                push (nx, ny, nz)
                data.Add (float32 r); data.Add (float32 g); data.Add (float32 b)
            vertex verts.[0]
            vertex verts.[1]
            vertex verts.[2]
            vertex verts.[0]
            vertex verts.[2]
            vertex verts.[3]
        face (0., 0., 1.) [| (-1., -1., 1.); (1., -1., 1.); (1., 1., 1.); (-1., 1., 1.) |] (0.95, 0.36, 0.34)
        face (0., 0., -1.) [| (1., -1., -1.); (-1., -1., -1.); (-1., 1., -1.); (1., 1., -1.) |] (0.24, 0.62, 0.94)
        face (0., 1., 0.) [| (-1., 1., 1.); (1., 1., 1.); (1., 1., -1.); (-1., 1., -1.) |] (0.42, 0.80, 0.45)
        face (0., -1., 0.) [| (-1., -1., -1.); (1., -1., -1.); (1., -1., 1.); (-1., -1., 1.) |] (0.98, 0.76, 0.28)
        face (1., 0., 0.) [| (1., -1., 1.); (1., -1., -1.); (1., 1., -1.); (1., 1., 1.) |] (0.66, 0.47, 0.93)
        face (-1., 0., 0.) [| (-1., -1., -1.); (-1., -1., 1.); (-1., 1., 1.); (-1., 1., -1.) |] (0.24, 0.82, 0.80)
        Float32Array(data.ToArray())

    let mutable spinAngle = 0.
    let mutable spinning = true
    let mutable flat = false
    let mutable cameraDistance = 5.

    let compileShader (gl: WebGL.RenderingContext) (kind: WebGL.Enum) (source: string) =
        let shader = gl.CreateShader(kind)
        gl.ShaderSource(shader, source)
        gl.CompileShader(shader)
        if not (As<bool>(gl.GetShaderParameter(shader, gl.COMPILE_STATUS))) then
            failwith ("Shader compile error:\n" + gl.GetShaderInfoLog(shader))
        shader

    let setupScene () =
        let canvasElement = JS.Document.GetElementById "scene"
        if isNull canvasElement then
            sceneStatus.Value <- "Canvas element not found."
        else
            let canvas = As<HTMLCanvasElement> canvasElement
            let context = canvas.GetContext "webgl"
            if isNull context then
                sceneStatus.Value <- "WebGL is not available in this browser."
            else
                let gl = As<WebGL.RenderingContext> context
                gl.Viewport(0, 0, canvas.Width, canvas.Height)
                gl.Enable(gl.DEPTH_TEST)
                gl.ClearColor(0., 0., 0., 0.)

                let vs = compileShader gl gl.VERTEX_SHADER vertexShader
                let fs = compileShader gl gl.FRAGMENT_SHADER fragmentShader
                let program = gl.CreateProgram()
                gl.AttachShader(program, vs)
                gl.AttachShader(program, fs)
                gl.LinkProgram(program)
                if not (As<bool>(gl.GetProgramParameter(program, gl.LINK_STATUS))) then
                    sceneStatus.Value <- "Program link error:\n" + gl.GetProgramInfoLog(program)
                gl.DeleteShader(vs)
                gl.DeleteShader(fs)

                gl.UseProgram(program)
                let buffer = gl.CreateBuffer()
                gl.BindBuffer(gl.ARRAY_BUFFER, buffer)
                gl.BufferData(gl.ARRAY_BUFFER, buildCube(), gl.STATIC_DRAW)

                let stride = 9 * 4
                let aPosition = gl.GetAttribLocation(program, "aPosition")
                let aNormal = gl.GetAttribLocation(program, "aNormal")
                let aColor = gl.GetAttribLocation(program, "aColor")
                gl.EnableVertexAttribArray(aPosition)
                gl.VertexAttribPointer(aPosition, 3, gl.FLOAT, false, stride, 0)
                gl.EnableVertexAttribArray(aNormal)
                gl.VertexAttribPointer(aNormal, 3, gl.FLOAT, false, stride, 3 * 4)
                gl.EnableVertexAttribArray(aColor)
                gl.VertexAttribPointer(aColor, 3, gl.FLOAT, false, stride, 6 * 4)

                let uProjection = gl.GetUniformLocation(program, "uProjection")
                let uView = gl.GetUniformLocation(program, "uView")
                let uModel = gl.GetUniformLocation(program, "uModel")
                let uFlat = gl.GetUniformLocation(program, "uFlat")

                let version =
                    match gl.GetParameter(gl.VERSION) with
                    | null -> "WebGL"
                    | v -> As<string> v

                sceneStatus.Value <-
                    version + " · 36 vertices · Float32Array uniforms straight from Vec/Mat/Quat types"

                let mutable lastTime = 0.

                let rec render (time: float) =
                    let delta = if lastTime = 0. then 0. else time - lastTime
                    lastTime <- time
                    if spinning then
                        spinAngle <- spinAngle + delta * 0.0012

                    // Projection and view come straight from mat4.
                    let projection =
                        Mat4.PerspectiveNO(
                            Mat4.Create(),
                            45.,
                            float canvas.Width / float canvas.Height,
                            0.1,
                            100.)
                    let eye = Vec3.FromValues(2.2, 1.8, cameraDistance)
                    let center = Vec3.FromValues(0., 0., 0.)
                    let up = Vec3.FromValues(0., 1., 0.)
                    let view = Mat4.LookAt(Mat4.Create(), eye, center, up)

                    // Rotation is built from a quaternion, then to a mat4.
                    let q =
                        Quat.FromEuler(
                            Quat.Create(),
                            spinAngle * 0.7,
                            spinAngle,
                            0.,
                            EulerOrder.ZYX)
                    let model = Mat4.FromQuat(Mat4.Create(), q)

                    gl.UniformMatrix4fv(uProjection, false, projection)
                    gl.UniformMatrix4fv(uView, false, view)
                    gl.UniformMatrix4fv(uModel, false, model)
                    gl.Uniform1f(uFlat, if flat then 1. else 0.)
                    gl.Clear(gl.COLOR_BUFFER_BIT ||| gl.DEPTH_BUFFER_BIT)
                    gl.DrawArrays(gl.TRIANGLES, 0, 36)

                    JS.Window.RequestAnimationFrame(render) |> ignore

                JS.Window.RequestAnimationFrame(render) |> ignore

    // ---------------------------------------------------------------
    // Vectors
    // ---------------------------------------------------------------

    let a = Vec3.FromValues(1.5, -0.5, 2.)
    let b = Vec3.FromValues(-0.5, 1., 0.75)

    let showVectors (result: string) =
        vecA.Value <- Vec3.Str a
        vecB.Value <- Vec3.Str b
        vecResult.Value <- result

    let vecAdd () =
        Vec3.Add(Vec3.Create(), a, b)
        |> fun r -> showVectors ("add → " + Vec3.Str r + "   length " + fmt (Vec3.Length r))

    let vecSubtract () =
        Vec3.Subtract(Vec3.Create(), a, b)
        |> fun r -> showVectors ("subtract → " + Vec3.Str r + "   (alias Vec3.Sub) " + Vec3.Str(Vec3.Sub(Vec3.Create(), a, b)))

    let vecScale () =
        let r = Vec3.Scale(Vec3.Create(), a, 2.5)
        showVectors ("scale a × 2.5 → " + Vec3.Str r + "   negate → " + Vec3.Str(Vec3.Negate(Vec3.Create(), a)))

    let vecDot () =
        showVectors (
            "dot " + fmt (Vec3.Dot(a, b)) +
            " · angle " + fmt (Vec3.Angle(a, b)) +
            " rad · distance " + fmt (Vec3.Distance(a, b)) +
            " · squaredDistance " + fmt (Vec3.SqrDist(a, b)) +
            " · length " + fmt (Vec3.Len(a)) +
            " · squaredLength " + fmt (Vec3.SqrLen(a)))

    let vecCross () =
        let r = Vec3.Cross(Vec3.Create(), a, b)
        let hermite = Vec3.Hermite(Vec3.Create(), a, b, a, b, 0.5)
        showVectors ("cross → " + Vec3.Str r + "   angle(cross, a) " + fmt (Vec3.Angle(r, a)) + "   hermite(…, 0.5) " + Vec3.Str hermite)

    let vecNormalize () =
        let n = Vec3.Normalize(Vec3.Create(), a)
        let inv = Vec3.Inverse(Vec3.Create(), a)
        let min = Vec3.Min(Vec3.Create(), a, b)
        let max = Vec3.Max(Vec3.Create(), a, b)
        let rounded = Vec3.Round(Vec3.Create(), Vec3.ScaleAndAdd(Vec3.Create(), a, b, 0.5))
        showVectors (
            "normalize → " + Vec3.Str n +
            "   inverse → " + Vec3.Str inv +
            "   min → " + Vec3.Str min +
            "   max → " + Vec3.Str max +
            "   round(scaleAndAdd) → " + Vec3.Str rounded)

    let vecDistance () =
        showVectors (
            "distance " + fmt (Vec3.Distance(a, b)) +
            " · dist " + fmt (Vec3.Dist(a, b)) +
            " · squaredDistance " + fmt (Vec3.SquaredDistance(a, b)) +
            " · sqrDist " + fmt (Vec3.SqrDist(a, b)) +
            " · length " + fmt (Vec3.Length b) +
            " · len " + fmt (Vec3.Len(b)))

    let vecAngle () =
        let degrees = GlMatrix.ToDegree(Vec3.Angle(a, b))
        showVectors (
            "angle(a, b) " + fmt (Vec3.Angle(a, b)) + " rad = " + fmt degrees + "°" +
            " · dot " + fmt (Vec3.Dot(a, b)) + " · equals(a, a) " +
            (if Vec3.Equals(a, a) then "yes" else "no") +
            " · exactEquals(a, clone a) " +
            (if Vec3.ExactEquals(a, Vec3.Clone a) then "yes" else "no"))

    let vecLerp () =
        let l = Vec3.Lerp(Vec3.Create(), a, b, 0.35)
        let s = Vec3.Slerp(Vec3.Create(), a, b, 0.35)
        showVectors ("lerp(0.35) → " + Vec3.Str l + "   slerp(0.35) → " + Vec3.Str s)

    let vecSlerp () =
        let c = Vec3.Bezier(Vec3.Create(), a, b, a, b, 0.4)
        showVectors ("bezier(…, 0.4) → " + Vec3.Str c)

    let vecTransform () =
        let m = Mat4.FromTranslation(Mat4.Create(), Vec3.FromValues(1., 2., -3.))
        let t = Vec3.TransformMat4(Vec3.Create(), a, m)
        let q = Quat.SetAxisAngle(Quat.Create(), Vec3.FromValues(0., 1., 0.), System.Math.PI / 4.)
        let q2 = Vec3.TransformQuat(Vec3.Create(), a, q)
        showVectors ("transformMat4 → " + Vec3.Str t + "   transformQuat → " + Vec3.Str q2)

    let vecMisc () =
        let c = Vec2.FromValues(3., 4.)
        let d = Vec2.FromValues(-1., 2.)
        let c2 = Vec2.Add(Vec2.Create(), c, d)
        let rotated = Vec2.Rotate(Vec2.Create(), c, Vec2.Create(), System.Math.PI / 3.)
        let v4 = Vec4.FromValues(1., 2., 3., 4.)
        let w4 = Vec4.FromValues(4., 3., 2., 1.)
        let v4n = Vec4.Normalize(Vec4.Create(), v4)
        let cross4 = Vec4.Cross(Vec4.Create(), v4, w4, v4n)
        // The typed constructors wrap an existing Float32Array without copying.
        let wrapped = Vec3(Float32Array([| 2.f; 4.f; 6.f |]))
        let wrappedNorm = Vec3.Normalize(Vec3.Create(), wrapped)
        showVectors (
            "vec2 add → " + Vec2.Str c2 +
            " · rotate → " + Vec2.Str rotated +
            " · dot " + fmt (Vec2.Dot(c, d)) +
            " · length " + fmt (Vec2.Length c) +
            " · cross.y " + fmt (Vec2.Cross(Vec3.Create(), c, d).Y) +
            " | vec4 normalize → " + Vec4.Str v4n +
            " · cross → " + Vec4.Str cross4 +
            " · dot " + fmt (Vec4.Dot(v4, w4)) +
            " | wrapped [" + fmt wrapped.X + ", " + fmt wrapped.Y + ", " + fmt wrapped.Z + "]" +
            " → normalize " + Vec3.Str wrappedNorm)

    // ---------------------------------------------------------------
    // Matrices
    // ---------------------------------------------------------------

    let compose () =
        let t = Mat4.FromTranslation(Mat4.Create(), Vec3.FromValues(1.5, -1., 0.5))
        let q = Quat.FromEuler(Quat.Create(), System.Math.PI / 6., System.Math.PI / 4., 0., EulerOrder.ZYX)
        let r = Mat4.FromQuat(Mat4.Create(), q)
        let s = Mat4.FromScaling(Mat4.Create(), Vec3.FromValues(0.8, 1.2, 1.))
        Mat4.Multiply(Mat4.Create(), t, Mat4.Multiply(Mat4.Create(), r, s))

    let matCompose () =
        let m = compose ()
        matrix.Value <- fmtMat4 m
        matInfo.Value <-
            "T·R·S · determinant " + fmt (Mat4.Determinant m) +
            " · trace " + fmt (m.[0] + m.[5] + m.[10] + m.[15]) +
            " · frob " + fmt (Mat4.Frob m)

    let matInvert () =
        match Optional.toOption (Mat4.Invert(Mat4.Create(), compose ())) with
        | None ->
            matrix.Value <- "-"
            matInfo.Value <- "matrix is not invertible"
        | Some inv ->
            let identity = Mat4.Multiply(Mat4.Create(), compose (), inv)
            matrix.Value <- fmtMat4 inv
            matInfo.Value <-
                "inverse · determinant " + fmt (Mat4.Determinant inv) +
                " · m·m⁻¹ ≈ identity: " + (if Mat4.Equals(identity, Mat4.Identity(Mat4.Create())) then "yes" else "no") +
                " · exact: " + (if Mat4.ExactEquals(identity, Mat4.Identity(Mat4.Create())) then "yes" else "no")

    let matTranspose () =
        let m = compose ()
        matrix.Value <- fmtMat4 (Mat4.Transpose(Mat4.Create(), m))
        matInfo.Value <- "transpose · adjoint determinant equals det² " + fmt (Mat4.Determinant(Mat4.Adjoint(Mat4.Create(), m)))

    let matDeterminant () =
        let m = compose ()
        matrix.Value <- fmtMat4 m
        matInfo.Value <-
            "determinant " + fmt (Mat4.Determinant m) +
            " · getTranslation " + Vec3.Str (Mat4.GetTranslation(Vec3.Create(), m)) +
            " · getScaling " + Vec3.Str (Mat4.GetScaling(Vec3.Create(), m)) +
            " · getRotation " + Quat.Str (Mat4.GetRotation(Quat.Create(), m))

    let matFrustum () =
        matrix.Value <- fmtMat4 (Mat4.Frustum(Mat4.Create(), -1., 1., -1., 1., 0.5, 100.))
        matInfo.Value <- "frustum(-1, 1, -1, 1, 0.5, 100)"

    let matFieldOfView () =
        let fov = FieldOfView(45., 45., 45., 45.)
        matrix.Value <- fmtMat4 (Mat4.PerspectiveFromFieldOfView(Mat4.Create(), fov, 0.5, 100.))
        matInfo.Value <- "perspectiveFromFieldOfView(45° × 45°, 0.5, 100)"

    let matOrtho () =
        matrix.Value <- fmtMat4 (Mat4.OrthoZO(Mat4.Create(), -2., 2., -1., 1., 1., 100.))
        matInfo.Value <-
            "orthoZO (WebGPU clip range) · orthoNO for WebGL: " +
            fmt (Mat4.OrthoNO(Mat4.Create(), -2., 2., -1., 1., 1., 100.).[15])

    let mat3Demo () =
        let m4 = compose ()
        let n = Mat3.NormalFromMat4(Mat3.Create(), m4)
        let projected = Mat3.Projection(Mat3.Create(), 640., 360.)
        matrix.Value <- fmtMat3 n
        matInfo.Value <-
            "normalFromMat4 · determinant " + fmt (Mat3.Determinant n) +
            " · fromMat4 upper-left " + fmt (Mat3.FromMat4(Mat3.Create(), m4).[4]) +
            " · projection aspect " + fmt projected.[0]

    let mat2dDemo () =
        let t = Mat2d.FromTranslation(Mat2d.Create(), Vec2.FromValues(3., -1.))
        let r = Mat2d.FromRotation(Mat2d.Create(), System.Math.PI / 6.)
        let m = Mat2d.Multiply(Mat2d.Create(), t, r)
        let m2 = Mat2.FromRotation(Mat2.Create(), System.Math.PI / 4.)
        let scaled = Mat2.MultiplyScalar(Mat2.Create(), m2, 2.)
        matrix.Value <- fmtMat2 scaled
        matInfo.Value <-
            "mat2d inverse determinant " + fmt (Mat2d.Determinant(m)) +
            " · mat3 fromMat2d " + fmt (Mat3.FromMat2d(Mat3.Create(), m).[0]) +
            " · mat2 frob " + fmt (Mat2.Frob scaled) +
            " · multiplyScalarAndAdd " + fmt (Mat2.MultiplyScalarAndAdd(Mat2.Create(), m2, m2, 1.).[0])

    // ---------------------------------------------------------------
    // Quaternions
    // ---------------------------------------------------------------

    let showQuat (q: Quat) (info: string) =
        quatText.Value <-
            Quat.Str q +
            "   length " + fmt (Quat.Length q) +
            " · normalized " + Quat.Str (Quat.Normalize(Quat.Create(), q))
        quatInfo.Value <- info

    let quatEuler () =
        let q = Quat.FromEuler(Quat.Create(), System.Math.PI / 6., System.Math.PI / 3., -System.Math.PI / 8., EulerOrder.ZYX)
        showQuat q ("fromEuler(ZYX) · dot with identity " + fmt (Quat.Dot(q, Quat.Identity(Quat.Create()))) + " · getAngle " + fmt (Quat.GetAngle(q, Quat.Identity(Quat.Create()))))

    let quatAxisAngle () =
        let axis = Vec3.FromValues(0., 1., 0.)
        let q = Quat.SetAxisAngle(Quat.Create(), axis, System.Math.PI / 3.)
        let out = Vec3.Create()
        let angle = Quat.GetAxisAngle(out, q)
        showQuat q ("setAxisAngle(y, 60°) · getAxisAngle → axis " + Vec3.Str out + " angle " + fmt angle)

    let quatSlerp () =
        let q1 = Quat.SetAxisAngle(Quat.Create(), Vec3.FromValues(0., 1., 0.), 0.)
        let q2 = Quat.SetAxisAngle(Quat.Create(), Vec3.FromValues(0., 1., 0.), System.Math.PI / 2.)
        let mid = Quat.Slerp(Quat.Create(), q1, q2, 0.5)
        let sq = Quat.Sqlerp(Quat.Create(), q1, q2, q1, q2, 0.5)
        showQuat mid ("slerp(0.5) · sqlerp(0.5) " + Quat.Str sq + " · lerp " + Quat.Str(Quat.Lerp(Quat.Create(), q1, q2, 0.5)))

    let quatRotate () =
        let q0 = Quat.FromEuler(Quat.Create(), 0.3, 0.5, 0.1, EulerOrder.XYZ)
        let qx = Quat.RotateX(Quat.Create(), q0, System.Math.PI / 8.)
        let qy = Quat.RotateY(Quat.Create(), qx, System.Math.PI / 8.)
        let qz = Quat.RotateZ(Quat.Create(), qy, System.Math.PI / 8.)
        showQuat qz ("rotateX · rotateY · rotateZ by 22.5° each · calculateW " + Quat.Str(Quat.CalculateW(Quat.Create(), qz)))

    let quatRotationTo () =
        let from = Vec3.FromValues(1., 0., 0.)
        let target = Vec3.FromValues(0., 1., 0.)
        let q = Quat.RotationTo(Quat.Create(), from, target)
        showQuat q ("rotationTo(x → y) · exp " + Quat.Str(Quat.Exp(Quat.Create(), q)) + " · ln " + Quat.Str(Quat.Ln(Quat.Create(), q)))

    let quatAxis () =
        let q = Quat.FromMat3(Quat.Create(), Mat3.FromRotation(Mat3.Create(), System.Math.PI / 5.))
        showQuat q ("fromMat3(rotateY 36°) · conjugate " + Quat.Str(Quat.Conjugate(Quat.Create(), q)) + " · inverse " + Quat.Str(Quat.Invert(Quat.Create(), q)))

    let quat2Demo () =
        let q = Quat.SetAxisAngle(Quat.Create(), Vec3.FromValues(0., 1., 0.), System.Math.PI / 4.)
        let translation = Vec3.FromValues(2., 0.5, -1.)
        let dq = Quat2.FromRotationTranslation(Quat2.Create(), q, translation)
        quat2Text.Value <-
            Quat2.Str dq +
            "   length " + fmt (Quat2.Length dq) +
            " · real " + Quat.Str (Quat2.GetReal(Quat.Create(), dq)) +
            " · dual " + Quat.Str (Quat2.GetDual(Quat.Create(), dq))
        quatInfo.Value <-
            "dual quat · translation " + Vec3.Str (Quat2.GetTranslation(Vec3.Create(), dq)) +
            " · dot " + fmt (Quat2.Dot(dq, dq)) +
            " · normalized length " + fmt (Quat2.Length(Quat2.Normalize(Quat2.Create(), dq)))

    let quat2Decompose () =
        let q = Quat.FromEuler(Quat.Create(), 0.4, 0.7, 0.2, EulerOrder.ZYX)
        let m =
            Mat4.FromRotationTranslationScale(
                Mat4.Create(),
                q,
                Vec3.FromValues(1., 2., 3.),
                Vec3.FromValues(2., 2., 2.))
        let outR = Quat.Create()
        let outT = Vec3.Create()
        let outS = Vec3.Create()
        Mat4.Decompose(outR, outT, outS, m) |> ignore
        quat2Text.Value <- "fromRotationTranslationScale matrix:\n" + fmtMat4 m
        quatInfo.Value <-
            "decompose → rotation " + Quat.Str outR +
            " · translation " + Vec3.Str outT +
            " · scale " + Vec3.Str outS +
            " · dual quat fromMat4 " + Quat2.Str (Quat2.FromMat4(Quat2.Create(), m))

    // ---------------------------------------------------------------
    // Utilities & traversal
    // ---------------------------------------------------------------

    let helpers () =
        let m2 = Mat2.FromValues(2., 0., 0., 4.)
        let adj = Mat2.Adjoint(Mat2.Create(), m2)
        utils.Value <-
            "epsilon " + string GlMatrix.Epsilon +
            " · angleOrder \"" + GlMatrix.AngleOrder + "\"" +
            " · round(2.5) " + fmt (GlMatrix.Round 2.5) +
            " · toRadian(180) " + fmt (GlMatrix.ToRadian 180.) +
            " · toDegree(π) " + fmt (GlMatrix.ToDegree System.Math.PI) +
            " · equals(1, 1.0000001) " + (if GlMatrix.Equals(1., 1.0000001) then "true" else "false") +
            " · equals(1, 1.1) " + (if GlMatrix.Equals(1., 1.1) then "true" else "false") +
            " · mat2 adjoint [" + fmt adj.[0] + ", " + fmt adj.[1] + ", " + fmt adj.[2] + ", " + fmt adj.[3] + "]" +
            " · mat3 fromQuat [0] " + fmt (Mat3.FromQuat(Mat3.Create(), Quat.FromEuler(Quat.Create(), 0.1, 0.2, 0.3)).[0])

    let forEachDemo () =
        let buffer = Float32Array([| 1.f; 2.f; 3.f; 4.f; 5.f; 6.f |])
        Vec3.ForEach(buffer, 3, 0, 0, fun (v, _) -> Vec3.Scale(v, v, 2.) |> ignore)
        |> ignore
        let buffer2 = Float32Array([| 1.f; 2.f; 3.f; 4.f; 5.f; 6.f |])
        Vec3.ForEach(
            buffer2, 3, 0, 0,
            (fun (v, _, factor) -> Vec3.Scale(v, v, factor) |> ignore),
            10.)
        |> ignore
        forEachText.Value <-
            "in place ×2 → " + fmtVec [ for i in 0 .. 5 -> float (buffer.Get(i)) ] +
            " · with arg 10 → " + fmtVec [ for i in 0 .. 5 -> float (buffer2.Get(i)) ]

    let lduDemo () =
        let m = Mat2.FromValues(4., 3., 6., 3.)
        let parts = Mat2.LDU(Mat2.Create(), Mat2.Create(), Mat2.Create(), m)
        let rebuilt =
            Mat2.Multiply(Mat2.Create(),
                Mat2.Multiply(Mat2.Create(), parts.[0], parts.[1]),
                parts.[2])
        ldu.Value <-
            "L " + fmtMat2 parts.[0] +
            " | D " + fmtMat2 parts.[1] +
            " | U " + fmtMat2 parts.[2] +
            " | L·D·U = m: " + (if Mat2.Equals(rebuilt, m) then "yes" else "no") +
            " (upstream mat2.LDU only fills the lower triangle and U — D is left as is)"

    // ---------------------------------------------------------------
    // Entry point
    // ---------------------------------------------------------------

    [<SPAEntryPoint>]
    let Main () =
        try
            let stored = JS.Window.LocalStorage.GetItem themeStorageKey
            if stored = "dark" || stored = "light" then
                theme.Value <- stored
        with _ -> ()
        applyTheme ()

        IndexTemplate()
            .notice(notice.V)
            .themeLabel(themeLabel.V)
            .sceneStatus(sceneStatus.V)
            .vecA(vecA.V)
            .vecB(vecB.V)
            .vecResult(vecResult.V)
            .matrix(matrix.V)
            .matInfo(matInfo.V)
            .quat(quatText.V)
            .quat2(quat2Text.V)
            .quatInfo(quatInfo.V)
            .utils(utils.V)
            .forEach(forEachText.V)
            .ldu(ldu.V)
            .ToggleTheme(fun _ -> toggleTheme ())
            .ToggleSpin(fun _ ->
                spinning <- not spinning
                sceneStatus.Value <- if spinning then "spinning" else "paused")
            .ResetSpin(fun _ ->
                spinAngle <- 0.
                sceneStatus.Value <- "rotation reset")
            .ToggleShading(fun _ ->
                flat <- not flat
                sceneStatus.Value <- if flat then "flat shading" else "lit shading")
            .ZoomIn(fun _ ->
                cameraDistance <- System.Math.Max(2.5, cameraDistance - 0.75)
                sceneStatus.Value <- "camera distance " + fmt cameraDistance)
            .ZoomOut(fun _ ->
                cameraDistance <- System.Math.Min(12., cameraDistance + 0.75)
                sceneStatus.Value <- "camera distance " + fmt cameraDistance)
            .VecAdd(fun _ -> vecAdd ())
            .VecSubtract(fun _ -> vecSubtract ())
            .VecScale(fun _ -> vecScale ())
            .VecDot(fun _ -> vecDot ())
            .VecCross(fun _ -> vecCross ())
            .VecNormalize(fun _ -> vecNormalize ())
            .VecLerp(fun _ -> vecLerp ())
            .VecSlerp(fun _ -> vecSlerp ())
            .VecDistance(fun _ -> vecDistance ())
            .VecAngle(fun _ -> vecAngle ())
            .VecTransform(fun _ -> vecTransform ())
            .VecMisc(fun _ -> vecMisc ())
            .MatCompose(fun _ -> matCompose ())
            .MatInvert(fun _ -> matInvert ())
            .MatDeterminant(fun _ -> matDeterminant ())
            .MatTranspose(fun _ -> matTranspose ())
            .MatAdjoint(fun _ -> matTranspose ())
            .MatFrustum(fun _ -> matFrustum ())
            .MatFieldOfView(fun _ -> matFieldOfView ())
            .MatOrtho(fun _ -> matOrtho ())
            .Mat3(fun _ -> mat3Demo ())
            .Mat2d(fun _ -> mat2dDemo ())
            .QuatEuler(fun _ -> quatEuler ())
            .QuatAxisAngle(fun _ -> quatAxisAngle ())
            .QuatSlerp(fun _ -> quatSlerp ())
            .QuatRotate(fun _ -> quatRotate ())
            .QuatRotationTo(fun _ -> quatRotationTo ())
            .QuatAxis(fun _ -> quatAxis ())
            .Quat2Demo(fun _ -> quat2Demo ())
            .Quat2Decompose(fun _ -> quat2Decompose ())
            .OnHelpers(fun _ -> helpers ())
            .OnForEach(fun _ -> forEachDemo ())
            .OnLdu(fun _ -> lduDemo ())
            .Bind()

        // The DOM is ready now; WebGL needs the real canvas element.
        let safe (name: string) (f: unit -> unit) =
            try f () with e ->
                Console.Log("glmatrix sample: " + name + " failed: " + e.Message)
        safe "matCompose" matCompose
        safe "vecAdd" vecAdd
        safe "quatEuler" quatEuler
        safe "quat2Demo" quat2Demo
        safe "helpers" helpers
        safe "forEachDemo" forEachDemo
        safe "lduDemo" lduDemo
        safe "setupScene" setupScene