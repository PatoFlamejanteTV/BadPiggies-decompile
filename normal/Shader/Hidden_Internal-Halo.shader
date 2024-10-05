//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Hidden/Internal-Halo" {
Properties {
}
SubShader {
 Tags { "RenderType" = "Overlay" }
 Pass {
  Tags { "RenderType" = "Overlay" }
  Blend OneMinusDstColor One, OneMinusDstColor One
  ColorMask RGB 0
  ZWrite Off
  Cull Off
  GpuProgramID 55344
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