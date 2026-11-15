// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2026 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}
namespace WebSharper.GlMatrix

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =

    let F = T<float>
    let B = T<bool>
    let S = T<string>

    /// The four field-of-view angles used by `mat4.perspectiveFromFieldOfView`,
    /// all in degrees.
    let FieldOfView =
        Pattern.Config "FieldOfView" {
            Required = [
                "upDegrees", F
                "downDegrees", F
                "leftDegrees", F
                "rightDegrees", F
            ]
            Optional = []
        }

    // ------------------------------------------------------------------
    // Type declarations
    //
    // Every value type below is a `Float32Array` at runtime: that is what
    // gl-matrix produces and what WebGL calls such as `uniformMatrix4fv`
    // consume directly. The static members of each class mirror the
    // corresponding gl-matrix module (imported from its npm subpath).
    // ------------------------------------------------------------------

    // `gl-matrix` re-exports the per-module namespaces plus the `glMatrix`
    // namespace object, which carries the shared utilities and constants.
    let GlMatrix = Class "glMatrix" |> Import "glMatrix" "gl-matrix"
    let Vec2 = Class "vec2" |=> Inherits T<Float32Array>
    let Vec3 = Class "vec3" |=> Inherits T<Float32Array>
    let Vec4 = Class "vec4" |=> Inherits T<Float32Array>
    let Mat2 = Class "mat2" |=> Inherits T<Float32Array>
    let Mat2d = Class "mat2d" |=> Inherits T<Float32Array>
    let Mat3 = Class "mat3" |=> Inherits T<Float32Array>
    let Mat4 = Class "mat4" |=> Inherits T<Float32Array>
    let Quat = Class "quat" |=> Inherits T<Float32Array>
    let Quat2 = Class "quat2" |=> Inherits T<Float32Array>

    /// Axis order used by `quat.fromEuler`.
    let EulerOrder =
        Pattern.EnumInlines "EulerOrder" [
            "XYZ", "'xyz'"
            "XZY", "'xzy'"
            "YXZ", "'yxz'"
            "YZX", "'yzx'"
            "ZXY", "'zxy'"
            "ZYX", "'zyx'"
        ]

    // ------------------------------------------------------------------
    // glMatrix — common utilities
    // ------------------------------------------------------------------

    GlMatrix
    |+> Static [
        "EPSILON" =? F
        |> WithSourceName "Epsilon"
        |> WithComment "The epsilon used by the fuzzy comparison functions (`GlMatrix.Equals`), `0.000001`."

        "ANGLE_ORDER" =? S
        |> WithSourceName "AngleOrder"
        |> WithComment "The default order of intrinsic rotations used by `Quat.FromEuler` (defaults to `\"zyx\"`)."

        "round" => F?a ^-> F
        |> WithComment "Symmetric round: rounds half away from zero (unlike `Math.round`, which rounds towards +Infinity)."

        "setMatrixArrayType" => T<obj>?``type`` ^-> T<unit>
        |> WithComment "Sets the array type used when creating new vectors and matrices: pass the constructor `JS.Global?Float32Array` (the default) or `JS.Global?Array`."

        "toRadian" => F?a ^-> F
        |> WithComment "Converts an angle in degrees to radians."

        "toDegree" => F?a ^-> F
        |> WithComment "Converts an angle in radians to degrees."

        "equals" => F?a * F?b * !?F?tolerance ^-> B
        |> WithComment "Tests whether two numbers are approximately equal, within an absolute tolerance (defaults to `EPSILON`)."
    ]
    |> ignore

    // ------------------------------------------------------------------
    // vec2
    // ------------------------------------------------------------------

    Vec2
    |> Import "*" "gl-matrix/vec2"
    |+> Static [
        Constructor T<Float32Array>?array
        |> WithInline "$array"
        |> WithComment "Wraps an existing `Float32Array` as a vec2. The array is not copied, so it must hold at least two elements."

        "create" => T<unit> ^-> Vec2
        |> WithComment "Creates a new, empty vec2."

        "clone" => Vec2?a ^-> Vec2
        |> WithComment "Creates a new vec2 initialized with the values of an existing vector."

        "fromValues" => F?x * F?y ^-> Vec2
        |> WithComment "Creates a new vec2 initialized with the given values."

        "copy" => Vec2?``out`` * Vec2?a ^-> Vec2
        |> WithComment "Copies the values from one vec2 to another."

        "set" => Vec2?``out`` * F?x * F?y ^-> Vec2
        |> WithComment "Sets the components of a vec2 to the given values."

        "add" => Vec2?``out`` * Vec2?a * Vec2?b ^-> Vec2
        |> WithComment "Adds two vec2's."

        "subtract" => Vec2?``out`` * Vec2?a * Vec2?b ^-> Vec2
        |> WithComment "Subtracts vector b from vector a."

        "sub" => Vec2?``out`` * Vec2?a * Vec2?b ^-> Vec2
        |> WithComment "Alias for `Vec2.Subtract`: subtracts vector b from vector a."

        "multiply" => Vec2?``out`` * Vec2?a * Vec2?b ^-> Vec2
        |> WithComment "Multiplies two vec2's."

        "mul" => Vec2?``out`` * Vec2?a * Vec2?b ^-> Vec2
        |> WithComment "Alias for `Vec2.Multiply`: multiplies two vec2's."

        "divide" => Vec2?``out`` * Vec2?a * Vec2?b ^-> Vec2
        |> WithComment "Divides two vec2's."

        "div" => Vec2?``out`` * Vec2?a * Vec2?b ^-> Vec2
        |> WithComment "Alias for `Vec2.Divide`: divides two vec2's."

        "ceil" => Vec2?``out`` * Vec2?a ^-> Vec2
        |> WithComment "`Math.ceil` the components of a vec2."

        "floor" => Vec2?``out`` * Vec2?a ^-> Vec2
        |> WithComment "`Math.floor` the components of a vec2."

        "min" => Vec2?``out`` * Vec2?a * Vec2?b ^-> Vec2
        |> WithComment "Returns the minimum of two vec2's."

        "max" => Vec2?``out`` * Vec2?a * Vec2?b ^-> Vec2
        |> WithComment "Returns the maximum of two vec2's."

        "round" => Vec2?``out`` * Vec2?a ^-> Vec2
        |> WithComment "Symmetric-rounds the components of a vec2."

        "scale" => Vec2?``out`` * Vec2?a * F?b ^-> Vec2
        |> WithComment "Scales a vec2 by a scalar number."

        "scaleAndAdd" => Vec2?``out`` * Vec2?a * Vec2?b * F?scale ^-> Vec2
        |> WithComment "Adds two vec2's after scaling the second operand by a scalar value."

        "distance" => Vec2?a * Vec2?b ^-> F
        |> WithComment "Calculates the euclidian distance between two vec2's."

        "dist" => Vec2?a * Vec2?b ^-> F
        |> WithComment "Alias for `Vec2.Distance`: calculates the euclidian distance between two vec2's."

        "squaredDistance" => Vec2?a * Vec2?b ^-> F
        |> WithComment "Calculates the squared euclidian distance between two vec2's."

        "sqrDist" => Vec2?a * Vec2?b ^-> F
        |> WithComment "Alias for `Vec2.SquaredDistance`: calculates the squared euclidian distance between two vec2's."

        "length" => Vec2?a ^-> F
        |> WithComment "Calculates the length of a vec2."

        "len" => Vec2?a ^-> F
        |> WithComment "Alias for `Vec2.Length`: calculates the length of a vec2."

        "squaredLength" => Vec2?a ^-> F
        |> WithComment "Calculates the squared length of a vec2."

        "sqrLen" => Vec2?a ^-> F
        |> WithComment "Alias for `Vec2.SquaredLength`: calculates the squared length of a vec2."

        "negate" => Vec2?``out`` * Vec2?a ^-> Vec2
        |> WithComment "Negates the components of a vec2."

        "inverse" => Vec2?``out`` * Vec2?a ^-> Vec2
        |> WithComment "Returns the inverse of the components of a vec2 (`1/x`)."

        "normalize" => Vec2?``out`` * Vec2?a ^-> Vec2
        |> WithComment "Normalizes a vec2."

        "dot" => Vec2?a * Vec2?b ^-> F
        |> WithComment "Calculates the dot product of two vec2's."

        "cross" => Vec3?``out`` * Vec2?a * Vec2?b ^-> Vec3
        |> WithComment "Computes the cross product of two vec2's. Note that the cross product must by definition produce a 3D vector."

        "lerp" => Vec2?``out`` * Vec2?a * Vec2?b * F?t ^-> Vec2
        |> WithComment "Performs a linear interpolation between two vec2's."

        "random" => Vec2?``out`` * !?F?scale ^-> Vec2
        |> WithComment "Generates a random vector with the given scale. If the scale is omitted, a unit vector is returned."

        "transformMat2" => Vec2?``out`` * Vec2?a * Mat2?m ^-> Vec2
        |> WithComment "Transforms the vec2 with a mat2."

        "transformMat2d" => Vec2?``out`` * Vec2?a * Mat2d?m ^-> Vec2
        |> WithComment "Transforms the vec2 with a mat2d."

        "transformMat3" => Vec2?``out`` * Vec2?a * Mat3?m ^-> Vec2
        |> WithComment "Transforms the vec2 with a mat3. 3rd vector component is implicitly 1."

        "transformMat4" => Vec2?``out`` * Vec2?a * Mat4?m ^-> Vec2
        |> WithComment "Transforms the vec2 with a mat4. 3rd vector component is implicitly 0, 4th is implicitly 1."

        "rotate" => Vec2?``out`` * Vec2?a * Vec2?b * F?rad ^-> Vec2
        |> WithComment "Rotates a 2D vector around a given point by the given angle."

        "angle" => Vec2?a * Vec2?b ^-> F
        |> WithComment "Returns the smallest angle between two 2D vectors."

        "signedAngle" => Vec2?a * Vec2?b ^-> F
        |> WithComment "Returns the signed angle in the interval [-pi, pi] between two 2D vectors (positive if `a` is to the right of `b`)."

        "zero" => Vec2?``out`` ^-> Vec2
        |> WithComment "Sets the components of a vec2 to zero."

        "str" => Vec2?a ^-> S
        |> WithComment "Returns a string representation of a vec2."

        "exactEquals" => Vec2?a * Vec2?b ^-> B
        |> WithComment "Returns whether the vectors have exactly the same elements in the same position (compared with `===`)."

        "equals" => Vec2?a * Vec2?b ^-> B
        |> WithComment "Returns whether the vectors have approximately the same elements in the same position."

        "forEach" => T<Float32Array>?a * T<int>?stride * T<int>?offset * T<int>?count * (Vec2 * Vec2 ^-> T<unit>)?fn ^-> T<Float32Array>
        |> WithComment "Performs some operation over an array of vec2's, in place."

        Generic - fun t ->
            "forEach" => T<Float32Array>?a * T<int>?stride * T<int>?offset * T<int>?count * (Vec2 * Vec2 * t ^-> T<unit>)?fn * t?arg ^-> T<Float32Array>
        |> WithComment "Performs some operation over an array of vec2's, in place, passing an extra argument to the callback."
    ]
    |+> Instance [
        "X" =@ F
        |> WithSetterInline "$this[0]=$value"
        |> WithGetterInline "$this[0]"

        "Y" =@ F
        |> WithSetterInline "$this[1]=$value"
        |> WithGetterInline "$this[1]"

        "" =@ F
        |> Indexed T<int>
    ]
    |> ignore

    // ------------------------------------------------------------------
    // vec3
    // ------------------------------------------------------------------

    Vec3
    |> Import "*" "gl-matrix/vec3"
    |+> Static [
        Constructor T<Float32Array>?array
        |> WithInline "$array"
        |> WithComment "Wraps an existing `Float32Array` as a vec3. The array is not copied, so it must hold at least three elements."

        "create" => T<unit> ^-> Vec3
        |> WithComment "Creates a new, empty vec3."

        "clone" => Vec3?a ^-> Vec3
        |> WithComment "Creates a new vec3 initialized with the values of an existing vector."

        "fromValues" => F?x * F?y * F?z ^-> Vec3
        |> WithComment "Creates a new vec3 initialized with the given values."

        "copy" => Vec3?``out`` * Vec3?a ^-> Vec3
        |> WithComment "Copies the values from one vec3 to another."

        "set" => Vec3?``out`` * F?x * F?y * F?z ^-> Vec3
        |> WithComment "Sets the components of a vec3 to the given values."

        "add" => Vec3?``out`` * Vec3?a * Vec3?b ^-> Vec3
        |> WithComment "Adds two vec3's."

        "subtract" => Vec3?``out`` * Vec3?a * Vec3?b ^-> Vec3
        |> WithComment "Subtracts vector b from vector a."

        "sub" => Vec3?``out`` * Vec3?a * Vec3?b ^-> Vec3
        |> WithComment "Alias for `Vec3.Subtract`: subtracts vector b from vector a."

        "multiply" => Vec3?``out`` * Vec3?a * Vec3?b ^-> Vec3
        |> WithComment "Multiplies two vec3's."

        "mul" => Vec3?``out`` * Vec3?a * Vec3?b ^-> Vec3
        |> WithComment "Alias for `Vec3.Multiply`: multiplies two vec3's."

        "divide" => Vec3?``out`` * Vec3?a * Vec3?b ^-> Vec3
        |> WithComment "Divides two vec3's."

        "div" => Vec3?``out`` * Vec3?a * Vec3?b ^-> Vec3
        |> WithComment "Alias for `Vec3.Divide`: divides two vec3's."

        "ceil" => Vec3?``out`` * Vec3?a ^-> Vec3
        |> WithComment "`Math.ceil` the components of a vec3."

        "floor" => Vec3?``out`` * Vec3?a ^-> Vec3
        |> WithComment "`Math.floor` the components of a vec3."

        "min" => Vec3?``out`` * Vec3?a * Vec3?b ^-> Vec3
        |> WithComment "Returns the minimum of two vec3's."

        "max" => Vec3?``out`` * Vec3?a * Vec3?b ^-> Vec3
        |> WithComment "Returns the maximum of two vec3's."

        "round" => Vec3?``out`` * Vec3?a ^-> Vec3
        |> WithComment "Symmetric-rounds the components of a vec3."

        "scale" => Vec3?``out`` * Vec3?a * F?b ^-> Vec3
        |> WithComment "Scales a vec3 by a scalar number."

        "scaleAndAdd" => Vec3?``out`` * Vec3?a * Vec3?b * F?scale ^-> Vec3
        |> WithComment "Adds two vec3's after scaling the second operand by a scalar value."

        "distance" => Vec3?a * Vec3?b ^-> F
        |> WithComment "Calculates the euclidian distance between two vec3's."

        "dist" => Vec3?a * Vec3?b ^-> F
        |> WithComment "Alias for `Vec3.Distance`: calculates the euclidian distance between two vec3's."

        "squaredDistance" => Vec3?a * Vec3?b ^-> F
        |> WithComment "Calculates the squared euclidian distance between two vec3's."

        "sqrDist" => Vec3?a * Vec3?b ^-> F
        |> WithComment "Alias for `Vec3.SquaredDistance`: calculates the squared euclidian distance between two vec3's."

        "length" => Vec3?a ^-> F
        |> WithComment "Calculates the length of a vec3."

        "len" => Vec3?a ^-> F
        |> WithComment "Alias for `Vec3.Length`: calculates the length of a vec3."

        "squaredLength" => Vec3?a ^-> F
        |> WithComment "Calculates the squared length of a vec3."

        "sqrLen" => Vec3?a ^-> F
        |> WithComment "Alias for `Vec3.SquaredLength`: calculates the squared length of a vec3."

        "negate" => Vec3?``out`` * Vec3?a ^-> Vec3
        |> WithComment "Negates the components of a vec3."

        "inverse" => Vec3?``out`` * Vec3?a ^-> Vec3
        |> WithComment "Returns the inverse of the components of a vec3 (`1/x`)."

        "normalize" => Vec3?``out`` * Vec3?a ^-> Vec3
        |> WithComment "Normalizes a vec3."

        "dot" => Vec3?a * Vec3?b ^-> F
        |> WithComment "Calculates the dot product of two vec3's."

        "cross" => Vec3?``out`` * Vec3?a * Vec3?b ^-> Vec3
        |> WithComment "Computes the cross product of two vec3's."

        "lerp" => Vec3?``out`` * Vec3?a * Vec3?b * F?t ^-> Vec3
        |> WithComment "Performs a linear interpolation between two vec3's."

        "slerp" => Vec3?``out`` * Vec3?a * Vec3?b * F?t ^-> Vec3
        |> WithComment "Performs a spherical linear interpolation between two vec3's."

        "hermite" => Vec3?``out`` * Vec3?a * Vec3?b * Vec3?c * Vec3?d * F?t ^-> Vec3
        |> WithComment "Performs a hermite interpolation with two control points."

        "bezier" => Vec3?``out`` * Vec3?a * Vec3?b * Vec3?c * Vec3?d * F?t ^-> Vec3
        |> WithComment "Performs a bezier interpolation with two control points."

        "random" => Vec3?``out`` * !?F?scale ^-> Vec3
        |> WithComment "Generates a random vector with the given scale. If the scale is omitted, a unit vector is returned."

        "transformMat4" => Vec3?``out`` * Vec3?a * Mat4?m ^-> Vec3
        |> WithComment "Transforms the vec3 with a mat4. 4th vector component is implicitly 1."

        "transformMat3" => Vec3?``out`` * Vec3?a * Mat3?m ^-> Vec3
        |> WithComment "Transforms the vec3 with a mat3."

        "transformQuat" => Vec3?``out`` * Vec3?a * Quat?q ^-> Vec3
        |> WithComment "Transforms the vec3 with a quat."

        "rotateX" => Vec3?``out`` * Vec3?a * Vec3?b * F?rad ^-> Vec3
        |> WithComment "Rotates a 3D vector around the X axis."

        "rotateY" => Vec3?``out`` * Vec3?a * Vec3?b * F?rad ^-> Vec3
        |> WithComment "Rotates a 3D vector around the Y axis."

        "rotateZ" => Vec3?``out`` * Vec3?a * Vec3?b * F?rad ^-> Vec3
        |> WithComment "Rotates a 3D vector around the Z axis."

        "angle" => Vec3?a * Vec3?b ^-> F
        |> WithComment "Returns the angle between two 3D vectors."

        "zero" => Vec3?``out`` ^-> Vec3
        |> WithComment "Sets the components of a vec3 to zero."

        "str" => Vec3?a ^-> S
        |> WithComment "Returns a string representation of a vec3."

        "exactEquals" => Vec3?a * Vec3?b ^-> B
        |> WithComment "Returns whether the vectors have exactly the same elements in the same position (compared with `===`)."

        "equals" => Vec3?a * Vec3?b ^-> B
        |> WithComment "Returns whether the vectors have approximately the same elements in the same position."

        "forEach" => T<Float32Array>?a * T<int>?stride * T<int>?offset * T<int>?count * (Vec3 * Vec3 ^-> T<unit>)?fn ^-> T<Float32Array>
        |> WithComment "Performs some operation over an array of vec3's, in place."

        Generic - fun t ->
            "forEach" => T<Float32Array>?a * T<int>?stride * T<int>?offset * T<int>?count * (Vec3 * Vec3 * t ^-> T<unit>)?fn * t?arg ^-> T<Float32Array>
        |> WithComment "Performs some operation over an array of vec3's, in place, passing an extra argument to the callback."
    ]
    |+> Instance [
        "X" =@ F
        |> WithSetterInline "$this[0]=$value"
        |> WithGetterInline "$this[0]"

        "Y" =@ F
        |> WithSetterInline "$this[1]=$value"
        |> WithGetterInline "$this[1]"

        "Z" =@ F
        |> WithSetterInline "$this[2]=$value"
        |> WithGetterInline "$this[2]"

        "" =@ F
        |> Indexed T<int>
    ]
    |> ignore

    // ------------------------------------------------------------------
    // vec4
    // ------------------------------------------------------------------

    Vec4
    |> Import "*" "gl-matrix/vec4"
    |+> Static [
        Constructor T<Float32Array>?array
        |> WithInline "$array"
        |> WithComment "Wraps an existing `Float32Array` as a vec4. The array is not copied, so it must hold at least four elements."

        "create" => T<unit> ^-> Vec4
        |> WithComment "Creates a new, empty vec4."

        "clone" => Vec4?a ^-> Vec4
        |> WithComment "Creates a new vec4 initialized with the values of an existing vector."

        "fromValues" => F?x * F?y * F?z * F?w ^-> Vec4
        |> WithComment "Creates a new vec4 initialized with the given values."

        "copy" => Vec4?``out`` * Vec4?a ^-> Vec4
        |> WithComment "Copies the values from one vec4 to another."

        "set" => Vec4?``out`` * F?x * F?y * F?z * F?w ^-> Vec4
        |> WithComment "Sets the components of a vec4 to the given values."

        "add" => Vec4?``out`` * Vec4?a * Vec4?b ^-> Vec4
        |> WithComment "Adds two vec4's."

        "subtract" => Vec4?``out`` * Vec4?a * Vec4?b ^-> Vec4
        |> WithComment "Subtracts vector b from vector a."

        "sub" => Vec4?``out`` * Vec4?a * Vec4?b ^-> Vec4
        |> WithComment "Alias for `Vec4.Subtract`: subtracts vector b from vector a."

        "multiply" => Vec4?``out`` * Vec4?a * Vec4?b ^-> Vec4
        |> WithComment "Multiplies two vec4's."

        "mul" => Vec4?``out`` * Vec4?a * Vec4?b ^-> Vec4
        |> WithComment "Alias for `Vec4.Multiply`: multiplies two vec4's."

        "divide" => Vec4?``out`` * Vec4?a * Vec4?b ^-> Vec4
        |> WithComment "Divides two vec4's."

        "div" => Vec4?``out`` * Vec4?a * Vec4?b ^-> Vec4
        |> WithComment "Alias for `Vec4.Divide`: divides two vec4's."

        "ceil" => Vec4?``out`` * Vec4?a ^-> Vec4
        |> WithComment "`Math.ceil` the components of a vec4."

        "floor" => Vec4?``out`` * Vec4?a ^-> Vec4
        |> WithComment "`Math.floor` the components of a vec4."

        "min" => Vec4?``out`` * Vec4?a * Vec4?b ^-> Vec4
        |> WithComment "Returns the minimum of two vec4's."

        "max" => Vec4?``out`` * Vec4?a * Vec4?b ^-> Vec4
        |> WithComment "Returns the maximum of two vec4's."

        "round" => Vec4?``out`` * Vec4?a ^-> Vec4
        |> WithComment "Symmetric-rounds the components of a vec4."

        "scale" => Vec4?``out`` * Vec4?a * F?b ^-> Vec4
        |> WithComment "Scales a vec4 by a scalar number."

        "scaleAndAdd" => Vec4?``out`` * Vec4?a * Vec4?b * F?scale ^-> Vec4
        |> WithComment "Adds two vec4's after scaling the second operand by a scalar value."

        "distance" => Vec4?a * Vec4?b ^-> F
        |> WithComment "Calculates the euclidian distance between two vec4's."

        "dist" => Vec4?a * Vec4?b ^-> F
        |> WithComment "Alias for `Vec4.Distance`: calculates the euclidian distance between two vec4's."

        "squaredDistance" => Vec4?a * Vec4?b ^-> F
        |> WithComment "Calculates the squared euclidian distance between two vec4's."

        "sqrDist" => Vec4?a * Vec4?b ^-> F
        |> WithComment "Alias for `Vec4.SquaredDistance`: calculates the squared euclidian distance between two vec4's."

        "length" => Vec4?a ^-> F
        |> WithComment "Calculates the length of a vec4."

        "len" => Vec4?a ^-> F
        |> WithComment "Alias for `Vec4.Length`: calculates the length of a vec4."

        "squaredLength" => Vec4?a ^-> F
        |> WithComment "Calculates the squared length of a vec4."

        "sqrLen" => Vec4?a ^-> F
        |> WithComment "Alias for `Vec4.SquaredLength`: calculates the squared length of a vec4."

        "negate" => Vec4?``out`` * Vec4?a ^-> Vec4
        |> WithComment "Negates the components of a vec4."

        "inverse" => Vec4?``out`` * Vec4?a ^-> Vec4
        |> WithComment "Returns the inverse of the components of a vec4 (`1/x`)."

        "normalize" => Vec4?``out`` * Vec4?a ^-> Vec4
        |> WithComment "Normalizes a vec4."

        "dot" => Vec4?a * Vec4?b ^-> F
        |> WithComment "Calculates the dot product of two vec4's."

        "cross" => Vec4?``out`` * Vec4?u * Vec4?v * Vec4?w ^-> Vec4
        |> WithComment "Returns the cross-product of three vectors in a 4-dimensional space."

        "lerp" => Vec4?``out`` * Vec4?a * Vec4?b * F?t ^-> Vec4
        |> WithComment "Performs a linear interpolation between two vec4's."

        "random" => Vec4?``out`` * !?F?scale ^-> Vec4
        |> WithComment "Generates a random vector with the given scale. If the scale is omitted, a unit vector is returned."

        "transformMat4" => Vec4?``out`` * Vec4?a * Mat4?m ^-> Vec4
        |> WithComment "Transforms the vec4 with a mat4."

        "transformQuat" => Vec4?``out`` * Vec4?a * Quat?q ^-> Vec4
        |> WithComment "Transforms the vec4 with a quat."

        "zero" => Vec4?``out`` ^-> Vec4
        |> WithComment "Sets the components of a vec4 to zero."

        "str" => Vec4?a ^-> S
        |> WithComment "Returns a string representation of a vec4."

        "exactEquals" => Vec4?a * Vec4?b ^-> B
        |> WithComment "Returns whether the vectors have exactly the same elements in the same position (compared with `===`)."

        "equals" => Vec4?a * Vec4?b ^-> B
        |> WithComment "Returns whether the vectors have approximately the same elements in the same position."

        "forEach" => T<Float32Array>?a * T<int>?stride * T<int>?offset * T<int>?count * (Vec4 * Vec4 ^-> T<unit>)?fn ^-> T<Float32Array>
        |> WithComment "Performs some operation over an array of vec4's, in place."

        Generic - fun t ->
            "forEach" => T<Float32Array>?a * T<int>?stride * T<int>?offset * T<int>?count * (Vec4 * Vec4 * t ^-> T<unit>)?fn * t?arg ^-> T<Float32Array>
        |> WithComment "Performs some operation over an array of vec4's, in place, passing an extra argument to the callback."
    ]
    |+> Instance [
        "X" =@ F
        |> WithSetterInline "$this[0]=$value"
        |> WithGetterInline "$this[0]"

        "Y" =@ F
        |> WithSetterInline "$this[1]=$value"
        |> WithGetterInline "$this[1]"

        "Z" =@ F
        |> WithSetterInline "$this[2]=$value"
        |> WithGetterInline "$this[2]"

        "W" =@ F
        |> WithSetterInline "$this[3]=$value"
        |> WithGetterInline "$this[3]"

        "" =@ F
        |> Indexed T<int>
    ]
    |> ignore

    // ------------------------------------------------------------------
    // mat2
    // ------------------------------------------------------------------

    Mat2
    |> Import "*" "gl-matrix/mat2"
    |+> Static [
        Constructor T<Float32Array>?array
        |> WithInline "$array"
        |> WithComment "Wraps an existing `Float32Array` as a mat2. The array is not copied, so it must hold at least four elements."

        "create" => T<unit> ^-> Mat2
        |> WithComment "Creates a new identity mat2."

        "clone" => Mat2?a ^-> Mat2
        |> WithComment "Creates a new mat2 initialized with the values of an existing matrix."

        "fromValues" => F?m00 * F?m01 * F?m10 * F?m11 ^-> Mat2
        |> WithComment "Creates a new mat2 with the given values."

        "copy" => Mat2?``out`` * Mat2?a ^-> Mat2
        |> WithComment "Copies the values from one mat2 to another."

        "set" => Mat2?``out`` * F?m00 * F?m01 * F?m10 * F?m11 ^-> Mat2
        |> WithComment "Sets the components of a mat2 to the given values."

        "identity" => Mat2?``out`` ^-> Mat2
        |> WithComment "Sets a mat2 to the identity matrix."

        "transpose" => Mat2?``out`` * Mat2?a ^-> Mat2
        |> WithComment "Transposes the values of a mat2."

        "invert" => Mat2?``out`` * Mat2?a ^-> !?Mat2
        |> WithInline "$import.invert($out, $a) === null ? undefined : $out"
        |> WithComment "Inverts a mat2. Returns None if the source matrix is not invertible."

        "adjoint" => Mat2?``out`` * Mat2?a ^-> Mat2
        |> WithComment "Calculates the adjugate of a mat2."

        "determinant" => Mat2?a ^-> F
        |> WithComment "Calculates the determinant of a mat2."

        "multiply" => Mat2?``out`` * Mat2?a * Mat2?b ^-> Mat2
        |> WithComment "Multiplies two mat2's."

        "mul" => Mat2?``out`` * Mat2?a * Mat2?b ^-> Mat2
        |> WithComment "Alias for `Mat2.Multiply`: multiplies two mat2's."

        "rotate" => Mat2?``out`` * Mat2?a * F?rad ^-> Mat2
        |> WithComment "Rotates a mat2 by the given angle."

        "scale" => Mat2?``out`` * Mat2?a * Vec2?v ^-> Mat2
        |> WithComment "Scales the mat2 by the dimensions in the given vec2."

        "fromRotation" => Mat2?``out`` * F?rad ^-> Mat2
        |> WithComment "Creates a matrix from a given angle."

        "fromScaling" => Mat2?``out`` * Vec2?v ^-> Mat2
        |> WithComment "Creates a matrix from a vector scaling."

        "str" => Mat2?a ^-> S
        |> WithComment "Returns a string representation of a mat2."

        "frob" => Mat2?a ^-> F
        |> WithComment "Returns the Frobenius norm of a mat2."

        "LDU" => Mat2?L * Mat2?D * Mat2?U * Mat2?a ^-> !|Mat2
        |> WithComment "Factorizes the input matrix into L (lower triangular), D (diagonal) and U (upper triangular) matrices, written into the three output arguments and returned as an array."

        "add" => Mat2?``out`` * Mat2?a * Mat2?b ^-> Mat2
        |> WithComment "Adds two mat2's."

        "subtract" => Mat2?``out`` * Mat2?a * Mat2?b ^-> Mat2
        |> WithComment "Subtracts matrix b from matrix a."

        "sub" => Mat2?``out`` * Mat2?a * Mat2?b ^-> Mat2
        |> WithComment "Alias for `Mat2.Subtract`: subtracts matrix b from matrix a."

        "multiplyScalar" => Mat2?``out`` * Mat2?a * F?b ^-> Mat2
        |> WithComment "Multiplies each element of the matrix by a scalar."

        "multiplyScalarAndAdd" => Mat2?``out`` * Mat2?a * Mat2?b * F?scale ^-> Mat2
        |> WithComment "Adds two mat2's after multiplying each element of the second operand by a scalar value."

        "exactEquals" => Mat2?a * Mat2?b ^-> B
        |> WithComment "Returns whether the matrices have exactly the same elements in the same position (compared with `===`)."

        "equals" => Mat2?a * Mat2?b ^-> B
        |> WithComment "Returns whether the matrices have approximately the same elements in the same position."
    ]
    |+> Instance [
        "" =@ F
        |> Indexed T<int>
    ]
    |> ignore

    // ------------------------------------------------------------------
    // mat2d
    // ------------------------------------------------------------------

    Mat2d
    |> Import "*" "gl-matrix/mat2d"
    |+> Static [
        Constructor T<Float32Array>?array
        |> WithInline "$array"
        |> WithComment "Wraps an existing `Float32Array` as a mat2d. The array is not copied, so it must hold at least six elements."

        "create" => T<unit> ^-> Mat2d
        |> WithComment "Creates a new identity mat2d."

        "clone" => Mat2d?a ^-> Mat2d
        |> WithComment "Creates a new mat2d initialized with the values of an existing matrix."

        "fromValues" => F?a * F?b * F?c * F?d * F?tx * F?ty ^-> Mat2d
        |> WithComment "Creates a new mat2d with the given values."

        "copy" => Mat2d?``out`` * Mat2d?a ^-> Mat2d
        |> WithComment "Copies the values from one mat2d to another."

        "set" => Mat2d?``out`` * F?a * F?b * F?c * F?d * F?tx * F?ty ^-> Mat2d
        |> WithComment "Sets the components of a mat2d to the given values."

        "identity" => Mat2d?``out`` ^-> Mat2d
        |> WithComment "Sets a mat2d to the identity matrix."

        "invert" => Mat2d?``out`` * Mat2d?a ^-> !?Mat2d
        |> WithInline "$import.invert($out, $a) === null ? undefined : $out"
        |> WithComment "Inverts a mat2d. Returns None if the source matrix is not invertible."

        "determinant" => Mat2d?a ^-> F
        |> WithComment "Calculates the determinant of a mat2d."

        "multiply" => Mat2d?``out`` * Mat2d?a * Mat2d?b ^-> Mat2d
        |> WithComment "Multiplies two mat2d's."

        "mul" => Mat2d?``out`` * Mat2d?a * Mat2d?b ^-> Mat2d
        |> WithComment "Alias for `Mat2d.Multiply`: multiplies two mat2d's."

        "rotate" => Mat2d?``out`` * Mat2d?a * F?rad ^-> Mat2d
        |> WithComment "Rotates a mat2d by the given angle."

        "scale" => Mat2d?``out`` * Mat2d?a * Vec2?v ^-> Mat2d
        |> WithComment "Scales the mat2d by the dimensions in the given vec2."

        "translate" => Mat2d?``out`` * Mat2d?a * Vec2?v ^-> Mat2d
        |> WithComment "Translates the mat2d by the dimensions in the given vec2."

        "fromRotation" => Mat2d?``out`` * F?rad ^-> Mat2d
        |> WithComment "Creates a matrix from a given angle."

        "fromScaling" => Mat2d?``out`` * Vec2?v ^-> Mat2d
        |> WithComment "Creates a matrix from a vector scaling."

        "fromTranslation" => Mat2d?``out`` * Vec2?v ^-> Mat2d
        |> WithComment "Creates a matrix from a vector translation."

        "str" => Mat2d?a ^-> S
        |> WithComment "Returns a string representation of a mat2d."

        "frob" => Mat2d?a ^-> F
        |> WithComment "Returns the Frobenius norm of a mat2d."

        "add" => Mat2d?``out`` * Mat2d?a * Mat2d?b ^-> Mat2d
        |> WithComment "Adds two mat2d's."

        "subtract" => Mat2d?``out`` * Mat2d?a * Mat2d?b ^-> Mat2d
        |> WithComment "Subtracts matrix b from matrix a."

        "sub" => Mat2d?``out`` * Mat2d?a * Mat2d?b ^-> Mat2d
        |> WithComment "Alias for `Mat2d.Subtract`: subtracts matrix b from matrix a."

        "multiplyScalar" => Mat2d?``out`` * Mat2d?a * F?b ^-> Mat2d
        |> WithComment "Multiplies each element of the matrix by a scalar."

        "multiplyScalarAndAdd" => Mat2d?``out`` * Mat2d?a * Mat2d?b * F?scale ^-> Mat2d
        |> WithComment "Adds two mat2d's after multiplying each element of the second operand by a scalar value."

        "exactEquals" => Mat2d?a * Mat2d?b ^-> B
        |> WithComment "Returns whether the matrices have exactly the same elements in the same position (compared with `===`)."

        "equals" => Mat2d?a * Mat2d?b ^-> B
        |> WithComment "Returns whether the matrices have approximately the same elements in the same position."
    ]
    |+> Instance [
        "" =@ F
        |> Indexed T<int>
    ]
    |> ignore

    // ------------------------------------------------------------------
    // mat3
    // ------------------------------------------------------------------

    Mat3
    |> Import "*" "gl-matrix/mat3"
    |+> Static [
        Constructor T<Float32Array>?array
        |> WithInline "$array"
        |> WithComment "Wraps an existing `Float32Array` as a mat3. The array is not copied, so it must hold at least nine elements."

        "create" => T<unit> ^-> Mat3
        |> WithComment "Creates a new identity mat3."

        "fromMat4" => Mat3?``out`` * Mat4?a ^-> Mat3
        |> WithComment "Copies the upper-left 3x3 values of a mat4 into the given mat3."

        "clone" => Mat3?a ^-> Mat3
        |> WithComment "Creates a new mat3 initialized with the values of an existing matrix."

        "fromValues" => F?m00 * F?m01 * F?m02 * F?m10 * F?m11 * F?m12 * F?m20 * F?m21 * F?m22 ^-> Mat3
        |> WithComment "Creates a new mat3 with the given values."

        "copy" => Mat3?``out`` * Mat3?a ^-> Mat3
        |> WithComment "Copies the values from one mat3 to another."

        "set" => Mat3?``out`` * F?m00 * F?m01 * F?m02 * F?m10 * F?m11 * F?m12 * F?m20 * F?m21 * F?m22 ^-> Mat3
        |> WithComment "Sets the components of a mat3 to the given values."

        "identity" => Mat3?``out`` ^-> Mat3
        |> WithComment "Sets a mat3 to the identity matrix."

        "transpose" => Mat3?``out`` * Mat3?a ^-> Mat3
        |> WithComment "Transposes the values of a mat3."

        "invert" => Mat3?``out`` * Mat3?a ^-> !?Mat3
        |> WithInline "$import.invert($out, $a) === null ? undefined : $out"
        |> WithComment "Inverts a mat3. Returns None if the source matrix is not invertible."

        "adjoint" => Mat3?``out`` * Mat3?a ^-> Mat3
        |> WithComment "Calculates the adjugate of a mat3."

        "determinant" => Mat3?a ^-> F
        |> WithComment "Calculates the determinant of a mat3."

        "multiply" => Mat3?``out`` * Mat3?a * Mat3?b ^-> Mat3
        |> WithComment "Multiplies two mat3's."

        "mul" => Mat3?``out`` * Mat3?a * Mat3?b ^-> Mat3
        |> WithComment "Alias for `Mat3.Multiply`: multiplies two mat3's."

        "translate" => Mat3?``out`` * Mat3?a * Vec2?v ^-> Mat3
        |> WithComment "Translates a mat3 by the given vector."

        "rotate" => Mat3?``out`` * Mat3?a * F?rad ^-> Mat3
        |> WithComment "Rotates a mat3 by the given angle."

        "scale" => Mat3?``out`` * Mat3?a * Vec2?v ^-> Mat3
        |> WithComment "Scales the mat3 by the dimensions in the given vec2."

        "fromTranslation" => Mat3?``out`` * Vec2?v ^-> Mat3
        |> WithComment "Creates a matrix from a vector translation."

        "fromRotation" => Mat3?``out`` * F?rad ^-> Mat3
        |> WithComment "Creates a matrix from a given angle."

        "fromScaling" => Mat3?``out`` * Vec2?v ^-> Mat3
        |> WithComment "Creates a matrix from a vector scaling."

        "fromMat2d" => Mat3?``out`` * Mat2d?a ^-> Mat3
        |> WithComment "Copies the values from a mat2d into a mat3."

        "fromQuat" => Mat3?``out`` * Quat?q ^-> Mat3
        |> WithComment "Calculates a 3x3 matrix from the given quaternion."

        "normalFromMat4" => Mat3?``out`` * Mat4?a ^-> Mat3
        |> WithComment "Calculates a 3x3 normal matrix (transpose inverse) from the 4x4 matrix."

        "projection" => Mat3?``out`` * F?width * F?height ^-> Mat3
        |> WithComment "Generates a 2D projection matrix with the given bounds."

        "str" => Mat3?a ^-> S
        |> WithComment "Returns a string representation of a mat3."

        "frob" => Mat3?a ^-> F
        |> WithComment "Returns the Frobenius norm of a mat3."

        "add" => Mat3?``out`` * Mat3?a * Mat3?b ^-> Mat3
        |> WithComment "Adds two mat3's."

        "subtract" => Mat3?``out`` * Mat3?a * Mat3?b ^-> Mat3
        |> WithComment "Subtracts matrix b from matrix a."

        "sub" => Mat3?``out`` * Mat3?a * Mat3?b ^-> Mat3
        |> WithComment "Alias for `Mat3.Subtract`: subtracts matrix b from matrix a."

        "multiplyScalar" => Mat3?``out`` * Mat3?a * F?b ^-> Mat3
        |> WithComment "Multiplies each element of the matrix by a scalar."

        "multiplyScalarAndAdd" => Mat3?``out`` * Mat3?a * Mat3?b * F?scale ^-> Mat3
        |> WithComment "Adds two mat3's after multiplying each element of the second operand by a scalar value."

        "exactEquals" => Mat3?a * Mat3?b ^-> B
        |> WithComment "Returns whether the matrices have exactly the same elements in the same position (compared with `===`)."

        "equals" => Mat3?a * Mat3?b ^-> B
        |> WithComment "Returns whether the matrices have approximately the same elements in the same position."
    ]
    |+> Instance [
        "" =@ F
        |> Indexed T<int>
    ]
    |> ignore

    // ------------------------------------------------------------------
    // mat4
    // ------------------------------------------------------------------

    Mat4
    |> Import "*" "gl-matrix/mat4"
    |+> Static [
        Constructor T<Float32Array>?array
        |> WithInline "$array"
        |> WithComment "Wraps an existing `Float32Array` as a mat4. The array is not copied, so it must hold at least sixteen elements."

        "create" => T<unit> ^-> Mat4
        |> WithComment "Creates a new identity mat4 (column-major)."

        "clone" => Mat4?a ^-> Mat4
        |> WithComment "Creates a new mat4 initialized with the values of an existing matrix."

        "fromValues" => F?m00 * F?m01 * F?m02 * F?m03 * F?m10 * F?m11 * F?m12 * F?m13 * F?m20 * F?m21 * F?m22 * F?m23 * F?m30 * F?m31 * F?m32 * F?m33 ^-> Mat4
        |> WithComment "Creates a new mat4 with the given (column-major) values."

        "copy" => Mat4?``out`` * Mat4?a ^-> Mat4
        |> WithComment "Copies the values from one mat4 to another."

        "set" => Mat4?``out`` * F?m00 * F?m01 * F?m02 * F?m03 * F?m10 * F?m11 * F?m12 * F?m13 * F?m20 * F?m21 * F?m22 * F?m23 * F?m30 * F?m31 * F?m32 * F?m33 ^-> Mat4
        |> WithComment "Sets the components of a mat4 to the given (column-major) values."

        "identity" => Mat4?``out`` ^-> Mat4
        |> WithComment "Sets a mat4 to the identity matrix."

        "transpose" => Mat4?``out`` * Mat4?a ^-> Mat4
        |> WithComment "Transposes the values of a mat4."

        "invert" => Mat4?``out`` * Mat4?a ^-> !?Mat4
        |> WithInline "$import.invert($out, $a) === null ? undefined : $out"
        |> WithComment "Inverts a mat4. Returns None if the source matrix is not invertible."

        "adjoint" => Mat4?``out`` * Mat4?a ^-> Mat4
        |> WithComment "Calculates the adjugate of a mat4."

        "determinant" => Mat4?a ^-> F
        |> WithComment "Calculates the determinant of a mat4."

        "multiply" => Mat4?``out`` * Mat4?a * Mat4?b ^-> Mat4
        |> WithComment "Multiplies two mat4's."

        "mul" => Mat4?``out`` * Mat4?a * Mat4?b ^-> Mat4
        |> WithComment "Alias for `Mat4.Multiply`: multiplies two mat4's."

        "translate" => Mat4?``out`` * Mat4?a * Vec3?v ^-> Mat4
        |> WithComment "Translates a mat4 by the given vector."

        "scale" => Mat4?``out`` * Mat4?a * Vec3?v ^-> Mat4
        |> WithComment "Scales the mat4 by the dimensions in the given vec3 (not using vectorization)."

        "rotate" => Mat4?``out`` * Mat4?a * F?rad * Vec3?axis ^-> Mat4
        |> WithComment "Rotates a mat4 by the given angle around the given axis."

        "rotateX" => Mat4?``out`` * Mat4?a * F?rad ^-> Mat4
        |> WithComment "Rotates a matrix by the given angle around the X axis."

        "rotateY" => Mat4?``out`` * Mat4?a * F?rad ^-> Mat4
        |> WithComment "Rotates a matrix by the given angle around the Y axis."

        "rotateZ" => Mat4?``out`` * Mat4?a * F?rad ^-> Mat4
        |> WithComment "Rotates a matrix by the given angle around the Z axis."

        "fromTranslation" => Mat4?``out`` * Vec3?v ^-> Mat4
        |> WithComment "Creates a matrix from a vector translation."

        "fromScaling" => Mat4?``out`` * Vec3?v ^-> Mat4
        |> WithComment "Creates a matrix from a vector scaling."

        "fromRotation" => Mat4?``out`` * F?rad * Vec3?axis ^-> Mat4
        |> WithComment "Creates a matrix from a given angle around a given axis."

        "fromXRotation" => Mat4?``out`` * F?rad ^-> Mat4
        |> WithComment "Creates a matrix from the given angle around the X axis."

        "fromYRotation" => Mat4?``out`` * F?rad ^-> Mat4
        |> WithComment "Creates a matrix from the given angle around the Y axis."

        "fromZRotation" => Mat4?``out`` * F?rad ^-> Mat4
        |> WithComment "Creates a matrix from the given angle around the Z axis."

        "fromRotationTranslation" => Mat4?``out`` * Quat?q * Vec3?v ^-> Mat4
        |> WithComment "Creates a matrix from a quaternion rotation and vector translation. This is equivalent to (but much faster than): `Mat4.Identity(dest); Mat4.Translate(dest, dest, v); var quatMat = Mat4.Create(); Mat4.FromQuat(quatMat, q); Mat4.Multiply(dest, dest, quatMat);`"

        "fromQuat2" => Mat4?``out`` * Quat2?a ^-> Mat4
        |> WithComment "Creates a new mat4 from a dual quat."

        "getTranslation" => Vec3?``out`` * Mat4?mat ^-> Vec3
        |> WithComment "Returns the translation vector component of a transformation matrix."

        "getScaling" => Vec3?``out`` * Mat4?mat ^-> Vec3
        |> WithComment "Returns the scaling factor component of a transformation matrix."

        "getRotation" => Quat?``out`` * Mat4?mat ^-> Quat
        |> WithComment "Returns a quaternion representing the rotational component of a transformation matrix."

        "decompose" => Quat?out_r * Vec3?out_t * Vec3?out_s * Mat4?mat ^-> Quat
        |> WithComment "Decomposes a transformation matrix into its rotation, translation and scale components, written into the three output arguments."

        "fromRotationTranslationScale" => Mat4?``out`` * Quat?q * Vec3?v * Vec3?s ^-> Mat4
        |> WithComment "Creates a matrix from a quaternion rotation, vector translation and vector scale."

        "fromRotationTranslationScaleOrigin" => Mat4?``out`` * Quat?q * Vec3?v * Vec3?s * Vec3?o ^-> Mat4
        |> WithComment "Creates a matrix from a quaternion rotation, vector translation and vector scale, rotating and scaling around the given origin."

        "fromQuat" => Mat4?``out`` * Quat?q ^-> Mat4
        |> WithComment "Calculates a 4x4 matrix from the given quaternion."

        "frustum" => Mat4?``out`` * F?left * F?right * F?bottom * F?top * F?near * F?far ^-> Mat4
        |> WithComment "Generates a frustum matrix with the given bounds."

        "perspectiveNO" => Mat4?``out`` * F?fovy * F?aspect * F?near * F?far ^-> Mat4
        |> WithComment "Generates a perspective projection matrix with the given bounds, mapping the near/far clip planes to a normalized device coordinate Z range of [-1, 1] (WebGL)."

        "perspectiveZO" => Mat4?``out`` * F?fovy * F?aspect * F?near * F?far ^-> Mat4
        |> WithComment "Generates a perspective projection matrix with the given bounds, mapping the near/far clip planes to a normalized device coordinate Z range of [0, 1] (WebGPU, Vulkan, DirectX, Metal)."

        "perspectiveFromFieldOfView" => Mat4?``out`` * FieldOfView?fov * F?near * F?far ^-> Mat4
        |> WithComment "Generates a perspective projection matrix with the given field of view."

        "orthoNO" => Mat4?``out`` * F?left * F?right * F?bottom * F?top * F?near * F?far ^-> Mat4
        |> WithComment "Generates an orthogonal projection matrix with the given bounds, mapping the near/far clip planes to a normalized device coordinate Z range of [-1, 1] (WebGL)."

        "orthoZO" => Mat4?``out`` * F?left * F?right * F?bottom * F?top * F?near * F?far ^-> Mat4
        |> WithComment "Generates an orthogonal projection matrix with the given bounds, mapping the near/far clip planes to a normalized device coordinate Z range of [0, 1] (WebGPU, Vulkan, DirectX, Metal)."

        "lookAt" => Mat4?``out`` * Vec3?eye * Vec3?center * Vec3?up ^-> Mat4
        |> WithComment "Generates a look-at matrix with the given eye position, focal point and up axis."

        "targetTo" => Mat4?``out`` * Vec3?eye * Vec3?target * Vec3?up ^-> Mat4
        |> WithComment "Generates a matrix that makes something look at something else."

        "str" => Mat4?a ^-> S
        |> WithComment "Returns a string representation of a mat4."

        "frob" => Mat4?a ^-> F
        |> WithComment "Returns the Frobenius norm of a mat4."

        "add" => Mat4?``out`` * Mat4?a * Mat4?b ^-> Mat4
        |> WithComment "Adds two mat4's."

        "subtract" => Mat4?``out`` * Mat4?a * Mat4?b ^-> Mat4
        |> WithComment "Subtracts matrix b from matrix a."

        "sub" => Mat4?``out`` * Mat4?a * Mat4?b ^-> Mat4
        |> WithComment "Alias for `Mat4.Subtract`: subtracts matrix b from matrix a."

        "multiplyScalar" => Mat4?``out`` * Mat4?a * F?b ^-> Mat4
        |> WithComment "Multiplies each element of the matrix by a scalar."

        "multiplyScalarAndAdd" => Mat4?``out`` * Mat4?a * Mat4?b * F?scale ^-> Mat4
        |> WithComment "Adds two mat4's after multiplying each element of the second operand by a scalar value."

        "exactEquals" => Mat4?a * Mat4?b ^-> B
        |> WithComment "Returns whether the matrices have exactly the same elements in the same position (compared with `===`)."

        "equals" => Mat4?a * Mat4?b ^-> B
        |> WithComment "Returns whether the matrices have approximately the same elements in the same position."

        "perspective" => Mat4?``out`` * F?fovy * F?aspect * F?near * F?far ^-> Mat4
        |> ObsoleteWithMessage "Use PerspectiveNO (WebGL) or PerspectiveZO (WebGPU/Vulkan) to select the clip-space Z range explicitly."
        |> WithComment "Alias for `Mat4.PerspectiveNO`."

        "ortho" => Mat4?``out`` * F?left * F?right * F?bottom * F?top * F?near * F?far ^-> Mat4
        |> ObsoleteWithMessage "Use OrthoNO (WebGL) or OrthoZO (WebGPU/Vulkan) to select the clip-space Z range explicitly."
        |> WithComment "Alias for `Mat4.OrthoNO`."
    ]
    |+> Instance [
        "" =@ F
        |> Indexed T<int>
    ]
    |> ignore

    // ------------------------------------------------------------------
    // quat
    // ------------------------------------------------------------------

    Quat
    |> Import "*" "gl-matrix/quat"
    |+> Static [
        Constructor T<Float32Array>?array
        |> WithInline "$array"
        |> WithComment "Wraps an existing `Float32Array` as a quat. The array is not copied, so it must hold at least four elements."

        "create" => T<unit> ^-> Quat
        |> WithComment "Creates a new identity quat."

        "identity" => Quat?``out`` ^-> Quat
        |> WithComment "Sets a quat to the identity quaternion."

        "setAxisAngle" => Quat?``out`` * Vec3?axis * F?rad ^-> Quat
        |> WithComment "Sets a quat from the given angle and rotation axis, then returns it."

        "getAxisAngle" => Vec3?out_axis * Quat?q ^-> F
        |> WithComment "Gets the rotation axis and angle for a given quaternion, writing the axis into `out_axis` and returning the angle."

        "getAngle" => Quat?a * Quat?b ^-> F
        |> WithComment "Gets the angular distance between two unit quaternions."

        "multiply" => Quat?``out`` * Quat?a * Quat?b ^-> Quat
        |> WithComment "Multiplies two quat's."

        "mul" => Quat?``out`` * Quat?a * Quat?b ^-> Quat
        |> WithComment "Alias for `Quat.Multiply`: multiplies two quat's."

        "rotateX" => Quat?``out`` * Quat?a * F?rad ^-> Quat
        |> WithComment "Rotates a quaternion by the given angle about the X axis."

        "rotateY" => Quat?``out`` * Quat?a * F?rad ^-> Quat
        |> WithComment "Rotates a quaternion by the given angle about the Y axis."

        "rotateZ" => Quat?``out`` * Quat?a * F?rad ^-> Quat
        |> WithComment "Rotates a quaternion by the given angle about the Z axis."

        "calculateW" => Quat?``out`` * Quat?a ^-> Quat
        |> WithComment "Calculates the W component of a unit quat from the X, Y and Z components."

        "exp" => Quat?``out`` * Quat?a ^-> Quat
        |> WithComment "Calculates the exponential of a unit quaternion."

        "ln" => Quat?``out`` * Quat?a ^-> Quat
        |> WithComment "Calculates the natural logarithm of a unit quaternion."

        "pow" => Quat?``out`` * Quat?a * F?b ^-> Quat
        |> WithComment "Calculates the scalar power of a unit quaternion."

        "slerp" => Quat?``out`` * Quat?a * Quat?b * F?t ^-> Quat
        |> WithComment "Performs a spherical linear interpolation between two quat's."

        "sqlerp" => Quat?``out`` * Quat?a * Quat?b * Quat?c * Quat?d * F?t ^-> Quat
        |> WithComment "Performs a spherical linear interpolation with two control points."

        "random" => Quat?``out`` ^-> Quat
        |> WithComment "Generates a random unit quaternion."

        "invert" => Quat?``out`` * Quat?a ^-> Quat
        |> WithComment "Calculates the inverse of a quat."

        "conjugate" => Quat?``out`` * Quat?a ^-> Quat
        |> WithComment "Calculates the conjugate of a quat. If the quaternion is normalized, this is faster than `Quat.Invert` and produces the same result."

        "fromMat3" => Quat?``out`` * Mat3?m ^-> Quat
        |> WithComment "Creates a quaternion from the given 3x3 rotation matrix. The result is not normalized."

        "fromEuler" => Quat?``out`` * F?x * F?y * F?z * !?EulerOrder?order ^-> Quat
        |> WithComment "Creates a quaternion from the given euler angle x, y, z, using the given intrinsic rotation order (defaults to `ANGLE_ORDER`, which is `zyx`)."

        "rotationTo" => Quat?``out`` * Vec3?a * Vec3?b ^-> Quat
        |> WithComment "Sets a quaternion to represent the shortest rotation from one vector to another. Both vectors are assumed to be unit length."

        "setAxes" => Quat?``out`` * Vec3?view * Vec3?right * Vec3?up ^-> Quat
        |> WithComment "Sets the given quaternion from the axes given by the view, right and up vectors, which are expected to be unit length and perpendicular to each other."

        "str" => Quat?a ^-> S
        |> WithComment "Returns a string representation of a quat."

        "exactEquals" => Quat?a * Quat?b ^-> B
        |> WithComment "Returns whether the quaternions have exactly the same elements in the same position (compared with `===`)."

        "equals" => Quat?a * Quat?b ^-> B
        |> WithComment "Returns whether the quaternions point approximately in the same direction."

        // Aliases shared with vec4.

        "clone" => Quat?a ^-> Quat
        |> WithComment "Creates a new quat initialized with the values of an existing quaternion."

        "fromValues" => F?x * F?y * F?z * F?w ^-> Quat
        |> WithComment "Creates a new quat initialized with the given values."

        "copy" => Quat?``out`` * Quat?a ^-> Quat
        |> WithComment "Copies the values from one quat to another."

        "set" => Quat?``out`` * F?x * F?y * F?z * F?w ^-> Quat
        |> WithComment "Sets the components of a quat to the given values."

        "add" => Quat?``out`` * Quat?a * Quat?b ^-> Quat
        |> WithComment "Adds two quat's."

        "scale" => Quat?``out`` * Quat?a * F?b ^-> Quat
        |> WithComment "Scales a quat by a scalar number."

        "dot" => Quat?a * Quat?b ^-> F
        |> WithComment "Calculates the dot product of two quat's."

        "lerp" => Quat?``out`` * Quat?a * Quat?b * F?t ^-> Quat
        |> WithComment "Performs a linear interpolation between two quat's."

        "length" => Quat?a ^-> F
        |> WithComment "Calculates the length of a quat."

        "len" => Quat?a ^-> F
        |> WithComment "Alias for `Quat.Length`: calculates the length of a quat."

        "squaredLength" => Quat?a ^-> F
        |> WithComment "Calculates the squared length of a quat."

        "sqrLen" => Quat?a ^-> F
        |> WithComment "Alias for `Quat.SquaredLength`: calculates the squared length of a quat."

        "normalize" => Quat?``out`` * Quat?a ^-> Quat
        |> WithComment "Normalizes a quat."
    ]
    |+> Instance [
        "X" =@ F
        |> WithSetterInline "$this[0]=$value"
        |> WithGetterInline "$this[0]"

        "Y" =@ F
        |> WithSetterInline "$this[1]=$value"
        |> WithGetterInline "$this[1]"

        "Z" =@ F
        |> WithSetterInline "$this[2]=$value"
        |> WithGetterInline "$this[2]"

        "W" =@ F
        |> WithSetterInline "$this[3]=$value"
        |> WithGetterInline "$this[3]"

        "" =@ F
        |> Indexed T<int>
    ]
    |> ignore

    // ------------------------------------------------------------------
    // quat2 — dual quaternions
    // ------------------------------------------------------------------

    Quat2
    |> Import "*" "gl-matrix/quat2"
    |+> Static [
        Constructor T<Float32Array>?array
        |> WithInline "$array"
        |> WithComment "Wraps an existing `Float32Array` as a dual quat. The array is not copied, so it must hold at least eight elements."

        "create" => T<unit> ^-> Quat2
        |> WithComment "Creates a new identity dual quat."

        "clone" => Quat2?a ^-> Quat2
        |> WithComment "Creates a new dual quat initialized with the values of an existing dual quaternion."

        "fromValues" => F?x1 * F?y1 * F?z1 * F?w1 * F?x2 * F?y2 * F?z2 * F?w2 ^-> Quat2
        |> WithComment "Creates a new dual quat initialized with the given values."

        "fromRotationTranslationValues" => F?x1 * F?y1 * F?z1 * F?w1 * F?x2 * F?y2 * F?z2 ^-> Quat2
        |> WithComment "Creates a new dual quat from the given quaternion and translation values."

        "fromRotationTranslation" => Quat2?``out`` * Quat?q * Vec3?t ^-> Quat2
        |> WithComment "Creates a dual quat from a quaternion and a translation."

        "fromTranslation" => Quat2?``out`` * Vec3?t ^-> Quat2
        |> WithComment "Creates a dual quat from a translation."

        "fromRotation" => Quat2?``out`` * Quat?q ^-> Quat2
        |> WithComment "Creates a dual quat from a quaternion."

        "fromMat4" => Quat2?``out`` * Mat4?a ^-> Quat2
        |> WithComment "Creates a new dual quat from a 4x4 matrix."

        "copy" => Quat2?``out`` * Quat2?a ^-> Quat2
        |> WithComment "Copies the values from one dual quat to another."

        "identity" => Quat2?``out`` ^-> Quat2
        |> WithComment "Sets a dual quat to the identity dual quaternion."

        "set" => Quat2?``out`` * F?x1 * F?y1 * F?z1 * F?w1 * F?x2 * F?y2 * F?z2 * F?w2 ^-> Quat2
        |> WithComment "Sets the components of a dual quat to the given values."

        "getReal" => Quat?``out`` * Quat2?a ^-> Quat
        |> WithComment "Gets the real part of a dual quat."

        "setReal" => Quat2?``out`` * Quat?q ^-> Quat2
        |> WithComment "Sets the real component of a dual quat to the given quaternion."

        "getDual" => Quat?``out`` * Quat2?a ^-> Quat
        |> WithComment "Gets the dual part of a dual quat."

        "setDual" => Quat2?``out`` * Quat?q ^-> Quat2
        |> WithComment "Sets the dual component of a dual quat to the given quaternion."

        "getTranslation" => Vec3?``out`` * Quat2?a ^-> Vec3
        |> WithComment "Gets the translation of a normalized dual quat."

        "translate" => Quat2?``out`` * Quat2?a * Vec3?v ^-> Quat2
        |> WithComment "Translates a dual quat by the given vector."

        "rotateX" => Quat2?``out`` * Quat2?a * F?rad ^-> Quat2
        |> WithComment "Rotates a dual quat around the X axis."

        "rotateY" => Quat2?``out`` * Quat2?a * F?rad ^-> Quat2
        |> WithComment "Rotates a dual quat around the Y axis."

        "rotateZ" => Quat2?``out`` * Quat2?a * F?rad ^-> Quat2
        |> WithComment "Rotates a dual quat around the Z axis."

        "rotateByQuatAppend" => Quat2?``out`` * Quat2?a * Quat?q ^-> Quat2
        |> WithComment "Rotates a dual quat by the given quaternion (`a * q`)."

        "rotateByQuatPrepend" => Quat2?``out`` * Quat?q * Quat2?a ^-> Quat2
        |> WithComment "Rotates a dual quat by the given quaternion (`q * a`)."

        "rotateAroundAxis" => Quat2?``out`` * Quat2?a * Vec3?axis * F?rad ^-> Quat2
        |> WithComment "Rotates a dual quat around a given axis. Does the normalization automatically."

        "add" => Quat2?``out`` * Quat2?a * Quat2?b ^-> Quat2
        |> WithComment "Adds two dual quat's."

        "multiply" => Quat2?``out`` * Quat2?a * Quat2?b ^-> Quat2
        |> WithComment "Multiplies two dual quat's."

        "mul" => Quat2?``out`` * Quat2?a * Quat2?b ^-> Quat2
        |> WithComment "Alias for `Quat2.Multiply`: multiplies two dual quat's."

        "scale" => Quat2?``out`` * Quat2?a * F?b ^-> Quat2
        |> WithComment "Scales a dual quat by a scalar number."

        "dot" => Quat2?a * Quat2?b ^-> F
        |> WithComment "Calculates the dot product of two dual quat's."

        "lerp" => Quat2?``out`` * Quat2?a * Quat2?b * F?t ^-> Quat2
        |> WithComment "Performs a linear interpolation between two dual quat's."

        "invert" => Quat2?``out`` * Quat2?a ^-> Quat2
        |> WithComment "Calculates the inverse of a dual quat. If it is normalized, `Quat2.Conjugate` is cheaper."

        "conjugate" => Quat2?``out`` * Quat2?a ^-> Quat2
        |> WithComment "Calculates the conjugate of a dual quat."

        "length" => Quat2?a ^-> F
        |> WithComment "Calculates the length of a dual quat."

        "len" => Quat2?a ^-> F
        |> WithComment "Alias for `Quat2.Length`: calculates the length of a dual quat."

        "squaredLength" => Quat2?a ^-> F
        |> WithComment "Calculates the squared length of a dual quat."

        "sqrLen" => Quat2?a ^-> F
        |> WithComment "Alias for `Quat2.SquaredLength`: calculates the squared length of a dual quat."

        "normalize" => Quat2?``out`` * Quat2?a ^-> Quat2
        |> WithComment "Normalizes a dual quat."

        "str" => Quat2?a ^-> S
        |> WithComment "Returns a string representation of a dual quaternion."

        "exactEquals" => Quat2?a * Quat2?b ^-> B
        |> WithComment "Returns whether the dual quaternions have exactly the same elements in the same position (compared with `===`)."

        "equals" => Quat2?a * Quat2?b ^-> B
        |> WithComment "Returns whether the dual quaternions have approximately the same elements in the same position."
    ]
    |+> Instance [
        "" =@ F
        |> Indexed T<int>
    ]
    |> ignore

    let Assembly =
        Assembly [
            Namespace "WebSharper.GlMatrix" [
                GlMatrix
                FieldOfView
                EulerOrder
                Vec2
                Vec3
                Vec4
                Mat2
                Mat2d
                Mat3
                Mat4
                Quat
                Quat2
            ]
        ]

[<Sealed>]
type GlMatrixExtension() =
    interface IExtension with
        member x.Assembly = Definition.Assembly

[<assembly: Extension(typeof<GlMatrixExtension>)>]
do ()