//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Hidden/BlitToDepth" {
Properties {
_MainTex ("Texture", any) = "" { }
_Color ("Multiplicative color", Color) = (1,1,1,1)
}
SubShader {
 Pass {
  ColorMask 0 0
  ZTest Always
  Cull Off
  GpuProgramID 16826
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