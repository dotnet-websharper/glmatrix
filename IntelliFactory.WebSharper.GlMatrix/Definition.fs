namespace IntelliFactory.WebSharper.GlMatrixExtension

open IntelliFactory.WebSharper.Dom
open IntelliFactory.WebSharper.Html5
open IntelliFactory.WebSharper.Html5.WebGL
open IntelliFactory.WebSharper.InterfaceGenerator

module Definition =

    let Vec2 = Type.New()
    let Vec3 = Type.New()
    let Vec4 = Type.New()
    let Mat4 = Type.New()
    let Mat3 = Type.New()
    let Mat2 = Type.New()
    let Mat2d = Type.New()
    let Quat = Type.New()

    module VecMixin =

        let Vec (c : CodeModel.Class) =
            c
            |+> [
                Constructor T<Float32Array>?arr
                |> WithInline "$arr"

                Constructor T<float[]>?arr
                |> WithInline "$arr"

                "add" => c?out * c?a * c?b ^-> c
                |> WithComment "Adds two vectors"

                "clone" => c?a ^-> c
                |> WithComment "Creates a new vector initialized with values from an existing vector"

                "copy" => c?out * c?a ^-> c
                |> WithComment "Copy the values from one vector to another"

                "create" => T<unit> ^-> c
                |> WithComment "Creates a new, empty vector"

                "dist" => c?a * c?b ^-> T<float>
                |> WithComment "Calculates the euclidian distance between two vectors"

                "distance" => c?a * c?b ^-> T<float>
                |> WithComment "Calculates the euclidian distance between two vectors"

                "div" => c?out * c?a * c?b ^-> c
                |> WithComment "Divides two vectors"

                "divide" => c?out * c?a * c?b ^-> c
                |> WithComment "Divides two vectors"

                "dot" => c?a * c?b ^-> c
                |> WithComment "Calculates the dot product of two vectors"

                "forEach" => T<Float32Array>?a * T<int>?stride * T<int>?offset * T<int>?count * (c ^-> T<unit>)?f ^-> T<Float32Array>
                |> WithComment "Perform some operation over an array of vectors."

                Generic - fun t -> "forEach" => T<Float32Array>?a * T<int>?stride * T<int>?offset * T<int>?count * (c * T<unit> * t ^-> T<unit>)?f ^-> T<Float32Array>
                |> WithComment "Perform some operation over an array of vectors."

                "len" => c?a ^-> T<float>
                |> WithComment "Calculates the length of a vector"

                "length" => c?a ^-> T<float>
                |> WithComment "Calculates the length of a vector"

                "lerp" => c?out * c?a * c?b * T<float>?t ^-> c
                |> WithComment "Performs a linear interpolation between two vectors"

                "max" => c?out * c?a * c?b ^-> c
                |> WithComment "Returns the maximum of two vectors"

                "min" => c?out * c?a * c?b ^-> c
                |> WithComment "Returns the minimum of two vectors"

                "mul" => c?out * c?a * c?b ^-> c
                |> WithComment "Multiplies two vectors"

                "multiply" => c?out * c?a * c?b ^-> c
                |> WithComment "Multiplies two vectors"

                "negate" => c?out * c?a ^-> c
                |> WithComment "Negates the components of a vector"

                "normalize" => c?out * c?a ^-> c
                |> WithComment "Normalize a vector"

                "random" => c?out * !?T<float>?scale ^-> c
                |> WithComment "Generates a random vector with the given scale. If scale is ommitted, a unit vector will be returned."

                "scale" => c?out * c?a * T<float>?b ^-> c
                |> WithComment "Scales a vector by a scalar number"

                "scaleAndAdd" => c?out * c?a * c?b * T<float>?scale ^-> c
                |> WithComment "Adds two vectors after scaling the second operand by a scalar value"

                "sqrDist" => c?a * c?b ^-> T<float>
                |> WithComment "Calculates the squared euclidian distance between two vectors"

                "squaredDistance" => c?a * c?b ^-> T<float>
                |> WithComment "Calculates the squared euclidian distance between two vectors"

                "sqrLen" => c?a ^-> T<float>
                |> WithComment "Calculates the squared length of a vector"

                "squareLength" => c?a ^-> T<float>
                |> WithComment "Calculates the squared length of a vector"

                "str" => c?a ^-> T<string>
                |> WithComment "Returns a string representation of a vector"

                "sub" => c?out * c?a * c?b ^-> c
                |> WithComment "Subtracts vector b from vector a"

                "substract" => c?out * c?a * c?b ^-> c
                |> WithComment "Subtracts vector b from vector a"
            ]

        let Cross  (c : CodeModel.Class) =
            c
            |+> [
                "cross" => Vec3?out * c?a * c?b ^-> Vec3
                |> WithComment "Computes the cross product of two vectors. Note that the cross product must by definition produce a 3D vector"
            ]

    let Vec2Class =
        Class "vec2"
        |=> Vec2
        |> VecMixin.Vec
        |> VecMixin.Cross
        |+> [
            Constructor T<float * float>?tup
            |> WithInline "$tup"

            "fromValues" => T<float>?x * T<float>?y ^-> Vec2
            |> WithComment "Creates a new vec2 initialized with the given values"

            "transformMat2" => Vec2?out * Vec2?a * Mat2?m ^-> Vec2
            |> WithComment "Transforms the vec2 with a mat2"

            "transformMat2d" => Vec2?out * Vec2?a * Mat2d?m ^-> Vec2
            |> WithComment "Transforms the vec2 with a mat2d"

            "transformMat3" => Vec2?out * Vec2?a * Mat3?m ^-> Vec2
            |> WithComment "Transforms the vec2 with a mat3. 3rd vector component is implicitly 1."

            "transformMat4" => Vec2?out * Vec2?a * Mat4?m ^-> Vec2
            |> WithComment "Transforms the vec2 with a mat4. 3rd vector component is implicitly 0. 4th vector component is implicitly 1."
        ]
        |+> Protocol [
            "x" =% T<float>
            |> WithSetterInline "$this[0]=$value"
            |> WithGetterInline "$this[0]"

            "y" =% T<float>
            |> WithSetterInline "$this[1]=$value"
            |> WithGetterInline "$this[1]"
        ]

    let Vec3Class =
        Class "vec3"
        |=> Vec3
        |> VecMixin.Vec
        |> VecMixin.Cross
        |+> [
            Constructor T<float * float * float>?tup
            |> WithInline "$tup"

            "fromValues" => T<float>?x * T<float>?y * T<float>?z ^-> Vec3
            |> WithComment "Creates a new vec3 initialized with the given values"

            "transformMat3" => Vec3?out * Vec3?a * Mat3?m ^-> Vec3
            |> WithComment "Transforms the vec3 with a mat3."

            "transformMat4" => Vec3?out * Vec3?a * Mat4?m ^-> Vec3
            |> WithComment "Transforms the vec3 with a mat4. 4th vector component is implicitly 1."

            "transformQuat" => Vec3?out * Vec3?a * Quat?q ^-> Vec3
            |> WithComment "Transforms the vec3 with a quat"
        ]
        |+> Protocol [
            "x" =% T<float>
            |> WithSetterInline "$this[0]=$value"
            |> WithGetterInline "$this[0]"

            "y" =% T<float>
            |> WithSetterInline "$this[1]=$value"
            |> WithGetterInline "$this[1]"

            "z" =% T<float>
            |> WithSetterInline "$this[2]=$value"
            |> WithGetterInline "$this[2]"
        ]

    let Vec4Class =
        Class "vec4"
        |=> Vec4
        |> VecMixin.Vec
        |> VecMixin.Cross
        |+> [
            Constructor T<float * float * float * float>?tup
            |> WithInline "$tup"

            "fromValues" => T<float>?x * T<float>?y * T<float>?z * T<float>?w ^-> Vec4
            |> WithComment "Creates a new vec4 initialized with the given values"

            "transformMat4" => Vec4?out * Vec4?a * Mat4?m ^-> Vec4
            |> WithComment "Transforms the vec4 with a mat4. 4th vector component is implicitly 1."

            "transformQuat" => Vec4?out * Vec4?a * Quat?q ^-> Vec4
            |> WithComment "Transforms the vec4 with a quat"
        ]
        |+> Protocol [
            "x" =% T<float>
            |> WithSetterInline "$this[0]=$value"
            |> WithGetterInline "$this[0]"

            "y" =% T<float>
            |> WithSetterInline "$this[1]=$value"
            |> WithGetterInline "$this[1]"

            "z" =% T<float>
            |> WithSetterInline "$this[2]=$value"
            |> WithGetterInline "$this[2]"

            "w" =% T<float>
            |> WithSetterInline "$this[3]=$value"
            |> WithGetterInline "$this[3]"
        ]

    module MatMixin =

        let Mat scaleV (c : CodeModel.Class) =
            c
            |=> Inherits T<Float32Array>
            |+> [
                Constructor T<Float32Array>?arr
                |> WithInline "$arr"

                Constructor T<float[]>?arr
                |> WithInline "$arr"

                "clone" => c?a ^-> c
                |> WithComment "Creates a new matrix initialized with values from an existing matrix"

                "copy" => c?out * c?a ^-> c
                |> WithComment "Copy the values from one matrix to another"

                "create" => T<unit> ^-> c
                |> WithComment "Creates a new identity matrix"

                "determinant" => c?a ^-> c
                |> WithComment "Calculates the determinant of a matrix"

                "identity" => c?out ^-> c
                |> WithComment "Set a matrix to the identity matrix"

                "invert" => c?out * c?a ^-> c
                |> WithComment "Inverts a matrix"

                "mul" => c?out * c?a * c?b ^-> c
                |> WithComment "Multiplies two matrices"

                "multiply" => c?out * c?a * c?b ^-> c
                |> WithComment "Multiplies two matrices"

                "scale" => c?out * c?a * scaleV?v ^-> c
                |> WithComment "Scales the matrix by the dimensions in the given vector"

                "str" => c?a ^-> T<string>
                |> WithComment "Returns a string representation of a matrix"

            ]

        let Square (c : CodeModel.Class) =
            c
            |+> [

                "adjoint" => c?out * c?a ^-> c
                |> WithComment "Calculates the adjugate of a matrix"

                "transpose" => c?out * c?a ^-> c
                |> WithComment "Transpose the values of a matrix"

            ]

        let Rotate (c : CodeModel.Class) =
            c
            |+> [
                "rotate" => c?out * c?a * T<float>?rad ^-> c
                |> WithComment "Rotates a matrix by the given angle"
            ]

        let Translate v (c : CodeModel.Class) =
            c
            |+> [
                "translate" => c?out * c?a * v?v ^-> c
                |> WithComment "Translate a matrix by the given vector"
            ]

    let Mat2Class =
        Class "mat2"
        |=> Mat2
        |> MatMixin.Mat Vec2
        |> MatMixin.Square
        |> MatMixin.Rotate

    let Mat2dClass =
        Class "mat2d"
        |=> Mat2d
        |> MatMixin.Mat Vec2
        |> MatMixin.Rotate
        |> MatMixin.Translate Vec2

    let Mat3Class =
        Class "mat3"
        |=> Mat3
        |> MatMixin.Mat Vec2
        |> MatMixin.Square
        |> MatMixin.Rotate
        |> MatMixin.Translate Vec2
        |+> [
            "fromMat2d" => Mat3?out * Mat2d?a ^-> Mat3
            |> WithComment "Copies the values from a mat2d into a mat3"

            "fromMat4" => Mat3?out * Mat4?a ^-> Mat3
            |> WithComment "Copies the upper-left 3x3 values into the given mat3."

            "fromQuat" => Mat3?out * Quat?q ^-> Mat3
            |> WithComment "Calculates a 3x3 matrix from the given quaternion"

            "normalFromMat4" => Mat3?out * Mat4?a ^-> Mat3
            |> WithComment "Calculates a 3x3 normal matrix (transpose inverse) from the 4x4 matrix"
        ]

    let Mat4Class =
        Class "mat4"
        |=> Mat4
        |> MatMixin.Mat Vec3
        |> MatMixin.Square
        |> MatMixin.Translate Vec3
        |+> [
            "fromQuat" => Mat4?out * Quat?q ^-> Mat4
            |> WithComment "Calculates a 4x4 matrix from the given quaternion"

            "fromRotationTranslation" => Mat4?out * Quat?q * Vec3?v ^-> Mat4
            |> WithComment "Creates a matrix from a quaternion rotation and vector translation This is equivalent to (but much faster than): mat4.identity(dest); mat4.translate(dest, vec); var quatMat = mat4.create(); quat4.toMat4(quat, quatMat); mat4.multiply(dest, quatMat);"

            "frustum" => Mat4?out * T<float>?left * T<float>?right * T<float>?bottom * T<float>?top * T<float>?near * T<float>?far ^-> Mat4
            |> WithComment "Generates a frustum matrix with the given bounds"

            "lookAt" => Mat4?out * Vec3?eye * Vec3?center * Vec3?up ^-> Mat4
            |> WithComment "Generates a look-at matrix with the given eye position, focal point, and up axis"

            "ortho" => Mat4?out * T<float>?left * T<float>?right * T<float>?bottom * T<float>?top * T<float>?near * T<float>?far ^-> Mat4
            |> WithComment "Generates a orthogonal projection matrix with the given bounds"

            "perspective" => Mat4?out * T<float>?fovy * T<float>?aspect * T<float>?near * T<float>?far ^-> Mat4
            |> WithComment "Generates a perspective projection matrix with the given bounds"

            "rotate" => Mat4?out * Mat4?a * T<float>?rad * Vec3?axis ^-> Mat4
            |> WithComment "Rotates a mat4 by the given angle"

            "rotateX" => Mat4?out * Mat4?a * T<float>?rad ^-> Mat4
            |> WithComment "Rotates a matrix by the given angle around the X axis"

            "rotateY" => Mat4?out * Mat4?a * T<float>?rad ^-> Mat4
            |> WithComment "Rotates a matrix by the given angle around the Y axis"

            "rotateZ" => Mat4?out * Mat4?a * T<float>?rad ^-> Mat4
            |> WithComment "Rotates a matrix by the given angle around the Z axis"
        ]

    let QuatClass =
        Class "quat"
        |=> Quat
        |=> Inherits T<Float32Array>
        |+> [
                Constructor T<Float32Array>?arr
                |> WithInline "$arr"

                "add" => Quat?out * Quat?a * Quat?b ^-> Quat
                |> WithComment "Adds two quat's"

                "calculateW" => Quat?out * Quat?a ^-> Quat
                |> WithComment "Calculates the W component of a quat from the X, Y, and Z components. Assumes that quaternion is 1 unit in length. Any existing W component will be ignored."

                "clone" => Quat?a ^-> Quat
                |> WithComment "Creates a new quat initialized with values from an existing quaternion"

                "conjugate" => Quat?out * Quat?a ^-> Quat
                |> WithComment "Calculates the conjugate of a quat If the quaternion is normalized, this function is faster than quat.inverse and produces the same result."

                "copy" => Quat?out * Quat?a ^-> Quat
                |> WithComment "Copy the values from one quat to another"

                "create" => T<unit> ^-> Quat
                |> WithComment "Creates a new identity quat"

                "dot" => Quat?a * Quat?b ^-> T<float>
                |> WithComment "Calculates the dot product of two quat's"

                "fromMat3" => Quat?out * Mat3?m ^-> Quat
                |> WithComment "Creates a quaternion from the given 3x3 rotation matrix. NOTE: The resultant quaternion is not normalized, so you should be sure to renormalize the quaternion yourself where necessary."

                "fromValues" => T<float>?x * T<float>?y * T<float>?z * T<float>?w ^-> Quat
                |> WithComment "Creates a new quat initialized with the given values"

                "identity" => Quat?out ^-> Quat
                |> WithComment "Set a quat to the identity quaternion"

                "invert" => Quat?out * Quat?a ^-> Quat
                |> WithComment "Calculates the inverse of a quat"

                "len" => Quat ^-> T<float>
                |> WithComment "Calculates the length of a quat"

                "length" => Quat ^-> T<float>
                |> WithComment "Calculates the length of a quat"

                "lerp" => Quat?out * Quat?a * Quat?b * T<float>?t ^-> Quat
                |> WithComment "Performs a linear interpolation between two quat's"

                "mul" => Quat?out * Quat?a * Quat?b ^-> Quat
                |> WithComment "Multiplies two quat's"

                "multiply" => Quat?out * Quat?a * Quat?b ^-> Quat
                |> WithComment "Multiplies two quat's"

                "normalize" => Quat?out * Quat?a ^-> Quat
                |> WithComment "Normalize a quat"

                "rotateX" => Quat?out * Quat?a * T<float>?rad ^-> Quat
                |> WithComment "Rotates a quaternion by the given angle about the X axis"

                "rotateY" => Quat?out * Quat?a * T<float>?rad ^-> Quat
                |> WithComment "Rotates a quaternion by the given angle about the Y axis"

                "rotateZ" => Quat?out * Quat?a * T<float>?rad ^-> Quat
                |> WithComment "Rotates a quaternion by the given angle about the Z axis"

                "scale" => Quat?out * Quat?a * T<float>?b ^-> Quat
                |> WithComment "Scales a quat by a scalar number"

                "set" => Quat?out * T<float>?x * T<float>?y * T<float>?z * T<float>?w ^-> Quat
                |> WithComment "Set the components of a quat to the given values"

                "setAxisAngle" => Quat?out * Vec3?axis * T<float>?rad ^-> Quat
                |> WithComment "Sets a quat from the given angle and rotation axis, then returns it."

                "slerp" => Quat?out * Quat?a * Quat?b * T<float>?t ^-> Quat
                |> WithComment "Performs a spherical linear interpolation between two quat"

                "sqrLen" => Quat?a ^-> T<float>
                |> WithComment "Calculates the squared length of a quat"

                "squaredLength" => Quat?a ^-> T<float>
                |> WithComment "Calculates the squared length of a quat"

                "str" => Quat?a ^-> T<string>
                |> WithComment "Returns a string representation of a quatenion"
            ]

    let Assembly =
        Assembly [
            Namespace "IntelliFactory.WebSharper.GlMatrix.Resources" [
                (Resource "Js" "gl-matrix-min.js").AssemblyWide()
            ]
            Namespace "IntelliFactory.WebSharper.GlMatrix" [
                Vec2Class
                Vec3Class
                Vec4Class
                Mat2Class
                Mat2dClass
                Mat3Class
                Mat4Class
                QuatClass
            ]
        ]


[<Sealed>]
type GlMatrixExtension() =
    interface IExtension with
        member x.Assembly = Definition.Assembly

[<assembly: Extension(typeof<GlMatrixExtension>)>]
do ()
