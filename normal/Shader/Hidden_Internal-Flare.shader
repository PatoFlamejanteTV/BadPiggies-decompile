//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Hidden/Internal-Flare" {
Properties {
}
SubShader {
 Tags { "RenderType" = "Overlay" }
 Pass {
  Tags { "RenderType" = "Overlay" }
  Blend One One, One One
  ColorMask RGB 0
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 9990
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