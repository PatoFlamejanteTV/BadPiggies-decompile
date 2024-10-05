//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "_Custom/DailyChallengeMask" {
Properties {
_MainTex ("Texture", 2D) = "black" { }
_Mask ("Mask", 2D) = "white" { }
_Layer ("Layer", 2D) = "white" { }
_Grayness ("Grayness", Range(0, 1)) = 0
}
SubShader {
 LOD 100
 Tags { "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }
 Pass {
  LOD 100
  Tags { "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }
  Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
  ZWrite Off
  GpuProgramID 59744
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