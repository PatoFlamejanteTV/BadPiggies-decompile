//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Hidden/Compositing" {
Properties {
_MainTex ("_MainTex (A)", 2D) = "black" { }
_SecondTex ("_SecondTex (A)", 2D) = "black" { }
}
SubShader {
 Pass {
  Name "Mix_RGBA_To_RGBA"
  Cull Off
  GpuProgramID 64389
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