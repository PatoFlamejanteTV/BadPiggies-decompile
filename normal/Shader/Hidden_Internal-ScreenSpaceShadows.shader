//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Hidden/Internal-ScreenSpaceShadows" {
Properties {
_ShadowMapTexture ("", any) = "" { }
_ODSWorldTexture ("", 2D) = "" { }
}
SubShader {
 Tags { "ShadowmapFilter" = "HardShadow" }
 Pass {
  Tags { "ShadowmapFilter" = "HardShadow" }
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 62865
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
}
}
}
SubShader {
 Tags { "ShadowmapFilter" = "HardShadow_FORCE_INV_PROJECTION_IN_PS" }
 Pass {
  Tags { "ShadowmapFilter" = "HardShadow_FORCE_INV_PROJECTION_IN_PS" }
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 106743
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
}
}
}
SubShader {
 Tags { "ShadowmapFilter" = "PCF_SOFT" }
 Pass {
  Tags { "ShadowmapFilter" = "PCF_SOFT" }
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 180047
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
}
}
}
SubShader {
 Tags { "ShadowmapFilter" = "PCF_SOFT_FORCE_INV_PROJECTION_IN_PS" }
 Pass {
  Tags { "ShadowmapFilter" = "PCF_SOFT_FORCE_INV_PROJECTION_IN_PS" }
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 202046
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" }
"// shader disassembly not supported on DXBC"
}
SubProgram "d3d11 " {
Keywords { "SHADOWS_SINGLE_CASCADE" "SHADOWS_SPLIT_SPHERES" }
"// shader disassembly not supported on DXBC"
}
}
}
}
}