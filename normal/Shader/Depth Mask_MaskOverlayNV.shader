//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Depth Mask/MaskOverlayNV" {
Properties {
_Color ("Main Color", Vector) = (1,1,1,1)
_MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }
_Radius ("Vignetting radius", Float) = 0.7
_Softness ("Vignetting softness", Float) = 0.3
}
SubShader {
 LOD 100
 Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent+10" "RenderType" = "Transparent" }
 Pass {
  LOD 100
  Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent+10" "RenderType" = "Transparent" }
  Blend SrcColor OneMinusSrcAlpha, SrcColor OneMinusSrcAlpha
  ColorMask RGB 0
  GpuProgramID 48089
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