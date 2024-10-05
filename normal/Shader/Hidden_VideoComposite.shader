//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Hidden/VideoComposite" {
Properties {
_MainTex ("_MainTex (A)", 2D) = "black" { }
}
SubShader {
 Tags { "QUEUE" = "Transparent" }
 Pass {
  Name "Default"
  Tags { "QUEUE" = "Transparent" }
  Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
  Cull Off
  GpuProgramID 10358
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