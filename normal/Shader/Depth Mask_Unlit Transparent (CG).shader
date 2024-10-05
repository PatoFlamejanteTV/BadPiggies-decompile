//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Depth Mask/Unlit Transparent (CG)" {
Properties {
_Color ("Main Color (A=Opacity)", Vector) = (1,1,1,1)
_MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }
}
SubShader {
 Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent+2" }
 Pass {
  Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent+2" }
  Blend DstColor Zero, DstColor Zero
  ZTest Less
  Fog {
   Mode Off
  }
  GpuProgramID 64918
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
Fallback "Diffuse"
}