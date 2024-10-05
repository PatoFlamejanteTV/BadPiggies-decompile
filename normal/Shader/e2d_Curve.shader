//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "e2d/Curve" {
Properties {
_ControlSize ("Control Size", Float) = 1
_InvControlSize ("Inv Control Size", Float) = 1
_InvControlSizeHalf ("Half of Inv Control Size", Float) = 0.5
_Control ("Control (RGBA)", 2D) = "red" { }
_Splat0 ("Layer 0 (R)", 2D) = "white" { }
_SplatParams0 ("Splat Params 0", Vector) = (1,1,0,0)
_Splat1 ("Layer 1 (G)", 2D) = "white" { }
_SplatParams1 ("Splat Params 1", Vector) = (1,1,0,0)
_MainTex ("BaseMap (RGB)", 2D) = "white" { }
_Color ("Main Color", Color) = (1,1,1,1)
}
SubShader {
 Tags { "IGNOREPROJECTOR" = "False" "QUEUE" = "Transparent" "RenderType" = "Opaque" }
 Pass {
  Tags { "IGNOREPROJECTOR" = "False" "QUEUE" = "Transparent" "RenderType" = "Opaque" }
  Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
  GpuProgramID 56035
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
Fallback "VertexLit"
}