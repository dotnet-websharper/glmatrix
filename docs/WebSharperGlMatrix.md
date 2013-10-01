# Overview

This WebSharper™ Extension provides a set of classes and functions almost
identical to the ones documented in [glMatrix](http://glmatrix.googlecode.com). When used
in WebSharper™ projects, these stub classes delegate the work to the
actual classes implemented by the browser.

After adding the reference to the project all the classes can be found
under the `IntelliFactory.WebSharper.GlMatrix` module.

# Usage

The GlMatrix WebSharper extension is as far as possible a one-to-one
mapping of glMatrix. Therefore, this documentation will only discuss
the differences between glMatrix and the WebSharper extension. For a
complete reference, see [the glMatrix homepage](http://glmatrix.googlecode.com).

## Differences with the JavaScript API

In order to enforce strong typing and take advantage of F# features such as
method overloading, we made a few modifications over the glMatrix API.

  * While glMatrix works on `Float32Array` objects, the WebSharper
  extension defines classes for each one of `Mat2`, `Mat3`, `Mat4`,
  `Vec2`, `Vec2d`, `Vec3`, `Vec4` and `Quad`. These classes inherit
  from the standard `Float32Array`, which allows you to pass them
  directly to WebGL methods such as the `Uniform` series.

## Using matrix transformations with WebGL

As the name suggests, glMatrix is mainly intended as a toolbox
for coordinate transformation for WebGL. The following example
shows a typical use to set the projection and modelview matrices
of a given WebGL shader program.

    #fsharp
    let SetupView (gl : WebGLRenderingContext, program : WebGLProgram) =
        // Set the projection matrix
        let proj = Mat4.Perspective(Mat4.Create(), 45., 4./3., 0.1, 1000.)
        let u_projection = gl.GetUniformLocation(program, "projectionMatrix")
        gl.UniformMatrix4fv(u_projection, false, proj)

        // Set the modelview matrix
        let cameraPosition = Mat4.Identity(Mat4.Create())
        Mat4.Translate(cameraPosition, cameraPosition, Vec3.FromValues(0.2, 2.3, -5.2)) |> ignore
        let u_modelview = gl.GetUniformLocation(program, "modelviewMatrix")
        gl.UniformMatrix4fv(u_modelview, false, cameraPosition)
