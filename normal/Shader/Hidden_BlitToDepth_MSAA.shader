//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Hidden/BlitToDepth_MSAA" {
Properties {
_MainTex ("DepthTexture", any) = "" { }
}
SubShader {
 Pass {
  ColorMask 0 0
  ZTest Always
  Cull Off
  GpuProgramID 55646
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