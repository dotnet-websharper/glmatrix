namespace IntelliFactory.WebSharper.JQueryExtension

open IntelliFactory.WebSharper.Dom
open IntelliFactory.WebSharper.WebGL

module JQuery =
    open IntelliFactory.WebSharper.InterfaceGenerator

    let Vec3 = Type.New()
    let Mat4 = Type.New()
    let Mat3 = Type.New()
    let Quat4 = Type.New()

    let Vec3Class =
        Class "vec3"
        |=> Vec3
        |=> Inherits T<Float32Array>
        |+> [
                "create" => T<unit> ^-> Vec3
                "create" => Vec3 ^-> Vec3
                "create" => Type.ArrayOf T<float> ^-> Vec3
                "set" => T<Float32Array> * Vec3 ^-> Vec3
                "set" => Type.ArrayOf T<float> * Vec3 ^-> Vec3
                "add" => Vec3 * Vec3 ^-> Vec3
                "add" => Vec3 * Vec3 * Vec3 ^-> Vec3
                "substract" => Vec3 * Vec3 ^-> Vec3
                "substract" => Vec3 * Vec3 * Vec3 ^-> Vec3
                "negate" => Vec3 ^-> Vec3
                "negate" => Vec3 * Vec3 ^-> Vec3
                "scale" => Vec3 * T<float> ^-> Vec3
                "scale" => Vec3 * T<float> * Vec3 ^-> Vec3
                "normalize" => Vec3 ^-> Vec3
                "normalize" => Vec3 * Vec3 ^-> Vec3
                "cross" => Vec3 * Vec3 ^-> Vec3
                "cross" => Vec3 * Vec3 * Vec3 ^-> Vec3
                "length" => Vec3 ^-> T<float>
                "dot" => Vec3 * Vec3 ^-> T<float>
                "direction" => Vec3 * Vec3 ^-> Vec3
                "direction" => Vec3 * Vec3 * Vec3 ^-> Vec3
                "lerp" => Vec3 * Vec3 * T<float> ^-> Vec3
                "lerp" => Vec3 * Vec3 * T<float> * Vec3 ^-> Vec3
                "str" => Vec3 ^-> T<string>
            ]

    let Mat3Class =
        Class "mat3"
        |=> Mat3
        |=> Inherits T<Float32Array>
        |+> [
                Constructor T<Float32Array>?arr
                |> WithInline "$arr"
                "create" => T<unit> ^-> Mat3
                "create" => Mat3 ^-> Mat3
                "create" => Type.ArrayOf T<float> ^-> Mat3
                "set" => T<Float32Array> * Mat3 ^-> T<unit>
                "set" => Type.ArrayOf T<float> * Mat3 ^-> T<unit>
                "identity" => Mat3 ^-> Mat3
                "transpose" => Mat3 ^-> Mat3
                "transpose" => Mat3 * Mat3 ^-> Mat3
                "toMat4" => Mat3 ^-> Mat4
                "toMat4" => Mat3 * Mat4 ^-> Mat4
                "str" => Mat4 ^-> T<string>
            ]

    let Mat4Class =
        Class "mat4"
        |=> Mat4
        |=> Inherits T<Float32Array>
        |+> [
                Constructor T<Float32Array>?arr
                |> WithInline "$arr"
                "create" => T<unit> ^-> Mat4
                "create" => Mat4 ^-> Mat4
                "create" => Type.ArrayOf T<float> ^-> Mat4
                "set" => Mat4 * Mat4 ^-> Mat4
                "set" => Type.ArrayOf T<float> * Mat4 ^-> Mat4
                "identity" => Mat4 ^-> Mat4
                "transpose" => Mat4 ^-> Mat4
                "transpose" => Mat4 * Mat4 ^-> Mat4
                "determinant" => Mat4 ^-> T<float>
                "inverse" => Mat4 ^-> Mat4
                "inverse" => Mat4 * Mat4 ^-> Mat4
                "toRotationMat" => Mat4 ^-> Mat4
                "toRotationMat" => Mat4 * Mat4 ^-> Mat4
                "toMat3" => Mat4 ^-> Mat3
                "toMat3" => Mat4 * Mat3 ^-> Mat3
                "toInverseMat3" => Mat4 ^-> Mat3
                "toInverseMat3" => Mat4 * Mat3 ^-> Mat3
                "multiply" => Mat4 * Mat4 ^-> Mat4
                "multiply" => Mat4 * Mat4 * Mat4 ^-> Mat4
                "multiply" => Mat4?mat * Vec3?vec ^-> Vec3
                |> WithInline "mat4.multiplyVec3($mat, $vec)"
                "multiply" => Mat4?mat * Vec3?vec * Vec3?dst ^-> Vec3
                |> WithInline "mat4.multiplyVec3($mat, $vec, $dst)"
                "multiply" => Mat4?mat * T<Float32Array>?vec ^-> T<Float32Array>
                |> WithInline "mat4.multiplyVec4($mat, $vec)"
                "multiply" => Mat4?mat * T<Float32Array>?vec * T<Float32Array>?dst ^-> T<Float32Array>
                |> WithInline "mat4.multiplyVec4($mat, $vec)"
                "translate" => Mat4 * Vec3 ^-> Mat4
                "translate" => Mat4 * Type.ArrayOf T<float> ^-> Mat4
                "translate" => Mat4 * Vec3 * Mat4 ^-> Mat4
                "translate" => Mat4 * Type.ArrayOf T<float> * Mat4 ^-> Mat4
                "scale" => Mat4 * Vec3 ^-> Mat4
                "scale" => Mat4 * Type.ArrayOf T<float> ^-> Mat4
                "scale" => Mat4 * Vec3 * Mat4 ^-> Mat4
                "scale" => Mat4 * Type.ArrayOf T<float> * Mat4 ^-> Mat4
                "rotate" => Mat4 * Vec3 * T<float> ^-> Mat4
                "rotate" => Mat4 * Type.ArrayOf T<float> * T<float> ^-> Mat4
                "rotate" => Mat4 * Vec3 * T<float> * Mat4 ^-> Mat4
                "rotate" => Mat4 * Type.ArrayOf T<float> * T<float> * Mat4 ^-> Mat4
                "rotateX" => Mat4 * T<float> ^-> Mat4
                "rotateX" => Mat4 * T<float> * Mat4 ^-> Mat4
                "rotateY" => Mat4 * T<float> ^-> Mat4
                "rotateY" => Mat4 * T<float> * Mat4 ^-> Mat4
                "rotateZ" => Mat4 * T<float> ^-> Mat4
                "rotateZ" => Mat4 * T<float> * Mat4 ^-> Mat4
                "frustum" => T<float> * T<float> * T<float> * T<float> * T<float> * T<float> ^-> Mat4
                "frustum" => T<float> * T<float> * T<float> * T<float> * T<float> * T<float> * Mat4 ^-> Mat4
                "perspective" => T<float> * T<float> * T<float> * T<float> ^-> Mat4
                "perspective" => T<float> * T<float> * T<float> * T<float> * Mat4 ^-> Mat4
                "ortho" => T<float> * T<float> * T<float> * T<float> * T<float> * T<float> ^-> Mat4
                "ortho" => T<float> * T<float> * T<float> * T<float> * T<float> * T<float> * Mat4 ^-> Mat4
                "lookAt" => Vec3 * Vec3 * Vec3 ^-> Mat4
                "lookAt" => Vec3 * Vec3 * Vec3 * Mat4 ^-> Mat4
                "str" => Mat4 ^-> T<string>
            ]

    let Quat4Class =
        Class "quat4"
        |=> Quat4
        |=> Inherits T<Float32Array>
        |+> [
                Constructor T<Float32Array>?arr
                |> WithInline "$arr"
                "create" => T<unit> ^-> Quat4
                "create" => Quat4 ^-> Quat4
                "create" => Type.ArrayOf T<float> ^-> Quat4
                "set" => Quat4 * Quat4 ^-> Quat4
                "set" => Type.ArrayOf T<float> * Quat4 ^-> Quat4
                "calculateW" => Quat4 ^-> Quat4
                "calculateW" => Quat4 * Quat4 ^-> Quat4
                "inverse" => Quat4 ^-> Quat4
                "inverse" => Quat4 * Quat4 ^-> Quat4
                "length" => Quat4 ^-> T<float>
                "normalize" => Quat4 ^-> Quat4
                "normalize" => Quat4 * Quat4 ^-> Quat4
                "multiply" => Quat4 * Quat4 ^-> Quat4
                "multiply" => Quat4 * Quat4 * Quat4 ^-> Quat4
                "multiply" => Quat4?quat * Vec3?vec ^-> Vec3
                |> WithInline "quat4.multiplyVec3($quat, $vec)"
                "multiplyVec3" => Quat4?quat * Vec3?vec * Vec3?dst ^-> Vec3
                |> WithInline "quat4.multiplyVec3($quat, $vec, $dst)"
                "toMat3" => Quat4 ^-> Mat3
                "toMat3" => Quat4 * Mat3 ^-> Mat3
                "toMat4" => Quat4 ^-> Mat4
                "toMat4" => Quat4 * Mat4 ^-> Mat4
                "slerp" => Quat4 * Quat4 * T<float> ^-> Quat4
                "slerp" => Quat4 * Quat4 * T<float> * Quat4 ^-> Quat4
                "str" => Quat4 ^-> T<string>
            ]


    let Assembly =
        Assembly [
            Namespace "IntelliFactory.WebSharper.GlMatrix" [
                Vec3Class
                Mat3Class
                Mat4Class
                Quat4Class
            ]
        ]

