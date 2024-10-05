//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "_Custom/Unlit_Color_Geometry" {
Properties {
_MainTex ("Base (RGB)", 2D) = "white" { }
_Color ("Main Color", Vector) = (1,1,1,1)
}
SubShader {
 LOD 100
 Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Opaque" }
 Pass {
  LOD 100
  Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Opaque" }
  ZWrite Off
  Cull Off
  Fog {
   Mode Off
  }
  GpuProgramID 62309
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