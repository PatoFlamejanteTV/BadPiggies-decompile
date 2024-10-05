//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Unlit/Cyber" {
Properties {
_MainTex ("Texture", 2D) = "white" { }
_Red ("洋红-红色-橙色", Range(-1, 0.5)) = 0
_Orange ("红色-橙色-黄色", Range(-0.5, 0.5)) = 0
_Yellow ("橙色-黄色-绿色", Range(-0.5, 1)) = 0
_Green ("黄色-绿色-靛色", Range(-1, 1)) = 0
_Cyan ("绿色-靛色-蓝色", Range(-1, 1)) = 0
_Blue ("靛色-蓝色-紫色", Range(-1, 0.5)) = 0
_Purple ("蓝色-紫色-洋红", Range(-0.5, 0.5)) = 0
_Magenta ("紫色-洋红-红色", Range(-0.5, 1)) = 0
_Hue ("Hue", Float) = 0
_Saturation ("Saturation", Float) = 1
_Value ("Value", Float) = 1
}
SubShader {
 LOD 100
 Tags { "QUEUE" = "Transparent" }
 GrabPass {
}
 Pass {
  LOD 100
  Tags { "QUEUE" = "Transparent" }
  GpuProgramID 33141
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
}
}
}
}