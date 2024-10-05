//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Hidden/InternalClear" {
Properties {
}
SubShader {
 Pass {
  ColorMask 0 0
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 40659
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 91
; Schema: 0
                                              OpCapability Shader 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %72 %82 %83 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %17 ArrayStride 17 
                                              OpMemberDecorate %18 0 Offset 18 
                                              OpMemberDecorate %18 1 Offset 18 
                                              OpDecorate %18 Block 
                                              OpDecorate %20 DescriptorSet 20 
                                              OpDecorate %20 Binding 20 
                                              OpMemberDecorate %70 0 BuiltIn 70 
                                              OpMemberDecorate %70 1 BuiltIn 70 
                                              OpMemberDecorate %70 2 BuiltIn 70 
                                              OpDecorate %70 Block 
                                              OpDecorate %82 Location 82 
                                              OpDecorate %83 Location 83 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                      %17 = OpTypeArray %7 %15 
                                      %18 = OpTypeStruct %16 %17 
                                      %19 = OpTypePointer Uniform %18 
Uniform struct {f32_4[4]; f32_4[4];}* %20 = OpVariable Uniform 
                                      %21 = OpTypeInt 32 1 
                                  i32 %22 = OpConstant 0 
                                  i32 %23 = OpConstant 1 
                                      %24 = OpTypePointer Uniform %7 
                                  i32 %35 = OpConstant 2 
                                  i32 %44 = OpConstant 3 
                       Private f32_4* %48 = OpVariable Private 
                                  u32 %68 = OpConstant 1 
                                      %69 = OpTypeArray %6 %68 
                                      %70 = OpTypeStruct %7 %6 %69 
                                      %71 = OpTypePointer Output %70 
 Output struct {f32_4; f32; f32[1];}* %72 = OpVariable Output 
                                      %80 = OpTypePointer Output %7 
                        Output f32_4* %82 = OpVariable Output 
                         Input f32_4* %83 = OpVariable Input 
                                      %85 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %25 = OpAccessChain %20 %22 %23 
                                f32_4 %26 = OpLoad %25 
                                f32_4 %27 = OpFMul %13 %26 
                                              OpStore %9 %27 
                       Uniform f32_4* %28 = OpAccessChain %20 %22 %22 
                                f32_4 %29 = OpLoad %28 
                                f32_4 %30 = OpLoad %11 
                                f32_4 %31 = OpVectorShuffle %30 %30 0 0 0 0 
                                f32_4 %32 = OpFMul %29 %31 
                                f32_4 %33 = OpLoad %9 
                                f32_4 %34 = OpFAdd %32 %33 
                                              OpStore %9 %34 
                       Uniform f32_4* %36 = OpAccessChain %20 %22 %35 
                                f32_4 %37 = OpLoad %36 
                                f32_4 %38 = OpLoad %11 
                                f32_4 %39 = OpVectorShuffle %38 %38 2 2 2 2 
                                f32_4 %40 = OpFMul %37 %39 
                                f32_4 %41 = OpLoad %9 
                                f32_4 %42 = OpFAdd %40 %41 
                                              OpStore %9 %42 
                                f32_4 %43 = OpLoad %9 
                       Uniform f32_4* %45 = OpAccessChain %20 %22 %44 
                                f32_4 %46 = OpLoad %45 
                                f32_4 %47 = OpFAdd %43 %46 
                                              OpStore %9 %47 
                                f32_4 %49 = OpLoad %9 
                                f32_4 %50 = OpVectorShuffle %49 %49 1 1 1 1 
                       Uniform f32_4* %51 = OpAccessChain %20 %23 %23 
                                f32_4 %52 = OpLoad %51 
                                f32_4 %53 = OpFMul %50 %52 
                                              OpStore %48 %53 
                       Uniform f32_4* %54 = OpAccessChain %20 %23 %22 
                                f32_4 %55 = OpLoad %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 0 0 0 0 
                                f32_4 %58 = OpFMul %55 %57 
                                f32_4 %59 = OpLoad %48 
                                f32_4 %60 = OpFAdd %58 %59 
                                              OpStore %48 %60 
                       Uniform f32_4* %61 = OpAccessChain %20 %23 %35 
                                f32_4 %62 = OpLoad %61 
                                f32_4 %63 = OpLoad %9 
                                f32_4 %64 = OpVectorShuffle %63 %63 2 2 2 2 
                                f32_4 %65 = OpFMul %62 %64 
                                f32_4 %66 = OpLoad %48 
                                f32_4 %67 = OpFAdd %65 %66 
                                              OpStore %48 %67 
                       Uniform f32_4* %73 = OpAccessChain %20 %23 %44 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 3 3 3 3 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %48 
                                f32_4 %79 = OpFAdd %77 %78 
                        Output f32_4* %81 = OpAccessChain %72 %22 
                                              OpStore %81 %79 
                                f32_4 %84 = OpLoad %83 
                                              OpStore %82 %84 
                          Output f32* %86 = OpAccessChain %72 %22 %68 
                                  f32 %87 = OpLoad %86 
                                  f32 %88 = OpFNegate %87 
                          Output f32* %89 = OpAccessChain %72 %22 %68 
                                              OpStore %89 %88 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
uniform 	int unity_StereoEyeIndex;
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
int op_shl(int a, int b) { return int(floor(float(a) * pow(2.0, float(b)))); }
ivec2 op_shl(ivec2 a, ivec2 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); return a; }
ivec3 op_shl(ivec3 a, ivec3 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); return a; }
ivec4 op_shl(ivec4 a, ivec4 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); a.w = op_shl(a.w, b.w); return a; }

void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = op_shl(unity_StereoEyeIndex, 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
#endif
	UNITY_UNIFORM int unity_StereoEyeIndex;
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(unity_StereoEyeIndex << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
};

struct UnityStereoGlobals_Type
{
    float4 hlslcc_mtx4x4unity_StereoMatrixP[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
    float4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
    float4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
    float4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
    float4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
    float3 unity_StereoWorldSpaceCameraPos[2];
    float4 unity_StereoScaleOffset[2];
};

struct UnityStereoEyeIndex_Type
{
    int unity_StereoEyeIndex;
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    constant UnityStereoGlobals_Type& UnityStereoGlobals [[ buffer(1) ]],
    constant UnityStereoEyeIndex_Type& UnityStereoEyeIndex [[ buffer(2) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    int u_xlati1;
    float4 u_xlat2;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = UnityStereoEyeIndex.unity_StereoEyeIndex << 0x2;
    u_xlat2 = u_xlat0.yyyy * UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1], u_xlat0.xxxx, u_xlat2);
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)], u_xlat0.zzzz, u_xlat2);
    output.mtl_Position = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)], u_xlat0.wwww, u_xlat2);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
	int unity_StereoEyeIndex;
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = unity_StereoEyeIndex << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 105
; Schema: 0
                                                   OpCapability Shader 
                                            %1 = OpExtInstImport "GLSL.std.450" 
                                                   OpMemoryModel Logical GLSL450 
                                                   OpEntryPoint Vertex %4 "main" %11 %84 %96 %97 
                                                   OpDecorate %11 Location 11 
                                                   OpDecorate %16 ArrayStride 16 
                                                   OpDecorate %18 ArrayStride 18 
                                                   OpMemberDecorate %20 0 Offset 20 
                                                   OpMemberDecorate %20 1 Offset 20 
                                                   OpMemberDecorate %20 2 Offset 20 
                                                   OpDecorate %20 Block 
                                                   OpDecorate %22 DescriptorSet 22 
                                                   OpDecorate %22 Binding 22 
                                                   OpMemberDecorate %82 0 BuiltIn 82 
                                                   OpMemberDecorate %82 1 BuiltIn 82 
                                                   OpMemberDecorate %82 2 BuiltIn 82 
                                                   OpDecorate %82 Block 
                                                   OpDecorate %96 Location 96 
                                                   OpDecorate %97 Location 97 
                                            %2 = OpTypeVoid 
                                            %3 = OpTypeFunction %2 
                                            %6 = OpTypeFloat 32 
                                            %7 = OpTypeVector %6 4 
                                            %8 = OpTypePointer Private %7 
                             Private f32_4* %9 = OpVariable Private 
                                           %10 = OpTypePointer Input %7 
                              Input f32_4* %11 = OpVariable Input 
                                           %14 = OpTypeInt 32 0 
                                       u32 %15 = OpConstant 4 
                                           %16 = OpTypeArray %7 %15 
                                       u32 %17 = OpConstant 8 
                                           %18 = OpTypeArray %7 %17 
                                           %19 = OpTypeInt 32 1 
                                           %20 = OpTypeStruct %16 %18 %19 
                                           %21 = OpTypePointer Uniform %20 
Uniform struct {f32_4[4]; f32_4[8]; i32;}* %22 = OpVariable Uniform 
                                       i32 %23 = OpConstant 0 
                                       i32 %24 = OpConstant 1 
                                           %25 = OpTypePointer Uniform %7 
                                       i32 %36 = OpConstant 2 
                                       i32 %45 = OpConstant 3 
                                           %49 = OpTypePointer Private %19 
                              Private i32* %50 = OpVariable Private 
                                           %51 = OpTypePointer Uniform %19 
                            Private f32_4* %55 = OpVariable Private 
                                       u32 %80 = OpConstant 1 
                                           %81 = OpTypeArray %6 %80 
                                           %82 = OpTypeStruct %7 %6 %81 
                                           %83 = OpTypePointer Output %82 
      Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                           %94 = OpTypePointer Output %7 
                             Output f32_4* %96 = OpVariable Output 
                              Input f32_4* %97 = OpVariable Input 
                                           %99 = OpTypePointer Output %6 
                                       void %4 = OpFunction None %3 
                                            %5 = OpLabel 
                                     f32_4 %12 = OpLoad %11 
                                     f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                            Uniform f32_4* %26 = OpAccessChain %22 %23 %24 
                                     f32_4 %27 = OpLoad %26 
                                     f32_4 %28 = OpFMul %13 %27 
                                                   OpStore %9 %28 
                            Uniform f32_4* %29 = OpAccessChain %22 %23 %23 
                                     f32_4 %30 = OpLoad %29 
                                     f32_4 %31 = OpLoad %11 
                                     f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                     f32_4 %33 = OpFMul %30 %32 
                                     f32_4 %34 = OpLoad %9 
                                     f32_4 %35 = OpFAdd %33 %34 
                                                   OpStore %9 %35 
                            Uniform f32_4* %37 = OpAccessChain %22 %23 %36 
                                     f32_4 %38 = OpLoad %37 
                                     f32_4 %39 = OpLoad %11 
                                     f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                     f32_4 %41 = OpFMul %38 %40 
                                     f32_4 %42 = OpLoad %9 
                                     f32_4 %43 = OpFAdd %41 %42 
                                                   OpStore %9 %43 
                                     f32_4 %44 = OpLoad %9 
                            Uniform f32_4* %46 = OpAccessChain %22 %23 %45 
                                     f32_4 %47 = OpLoad %46 
                                     f32_4 %48 = OpFAdd %44 %47 
                                                   OpStore %9 %48 
                              Uniform i32* %52 = OpAccessChain %22 %36 
                                       i32 %53 = OpLoad %52 
                                       i32 %54 = OpShiftLeftLogical %53 %36 
                                                   OpStore %50 %54 
                                     f32_4 %56 = OpLoad %9 
                                     f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                       i32 %58 = OpLoad %50 
                                       i32 %59 = OpIAdd %58 %24 
                            Uniform f32_4* %60 = OpAccessChain %22 %24 %59 
                                     f32_4 %61 = OpLoad %60 
                                     f32_4 %62 = OpFMul %57 %61 
                                                   OpStore %55 %62 
                                       i32 %63 = OpLoad %50 
                            Uniform f32_4* %64 = OpAccessChain %22 %24 %63 
                                     f32_4 %65 = OpLoad %64 
                                     f32_4 %66 = OpLoad %9 
                                     f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                     f32_4 %68 = OpFMul %65 %67 
                                     f32_4 %69 = OpLoad %55 
                                     f32_4 %70 = OpFAdd %68 %69 
                                                   OpStore %55 %70 
                                       i32 %71 = OpLoad %50 
                                       i32 %72 = OpIAdd %71 %36 
                            Uniform f32_4* %73 = OpAccessChain %22 %24 %72 
                                     f32_4 %74 = OpLoad %73 
                                     f32_4 %75 = OpLoad %9 
                                     f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                     f32_4 %77 = OpFMul %74 %76 
                                     f32_4 %78 = OpLoad %55 
                                     f32_4 %79 = OpFAdd %77 %78 
                                                   OpStore %55 %79 
                                       i32 %85 = OpLoad %50 
                                       i32 %86 = OpIAdd %85 %45 
                            Uniform f32_4* %87 = OpAccessChain %22 %24 %86 
                                     f32_4 %88 = OpLoad %87 
                                     f32_4 %89 = OpLoad %9 
                                     f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                     f32_4 %91 = OpFMul %88 %90 
                                     f32_4 %92 = OpLoad %55 
                                     f32_4 %93 = OpFAdd %91 %92 
                             Output f32_4* %95 = OpAccessChain %84 %23 
                                                   OpStore %95 %93 
                                     f32_4 %98 = OpLoad %97 
                                                   OpStore %96 %98 
                              Output f32* %100 = OpAccessChain %84 %23 %80 
                                      f32 %101 = OpLoad %100 
                                      f32 %102 = OpFNegate %101 
                              Output f32* %103 = OpAccessChain %84 %23 %80 
                                                   OpStore %103 %102 
                                                   OpReturn
                                                   OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
flat out highp uint vs_BLENDINDICES0;
float u_xlat0;
uint u_xlatu0;
vec4 u_xlat1;
vec4 u_xlat2;
int u_xlati3;
void main()
{
    u_xlat0 = float(uint(gl_InstanceID));
    u_xlat0 = u_xlat0 * 0.5;
    u_xlat0 = fract(u_xlat0);
    u_xlat0 = u_xlat0 + u_xlat0;
    u_xlat0 = roundEven(u_xlat0);
    u_xlatu0 = uint(u_xlat0);
    u_xlati3 = int(int(u_xlatu0) << 2);
    u_xlat1 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
    u_xlat1 = u_xlat1 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat2 = u_xlat1.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati3] * u_xlat1.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 2)] * u_xlat1.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 3)] * u_xlat1.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu0;
    gl_Layer = int(u_xlatu0);
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
flat out uint vs_BLENDINDICES0;
vec4 u_xlat0;
uint u_xlatu1;
vec4 u_xlat2;
int u_xlati4;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlatu1 = uint(gl_InstanceID) & 1u;
    u_xlati4 = int(u_xlatu1) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati4] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu1;
    gl_Layer = int(u_xlatu1);
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 111
; Schema: 0
                                              OpCapability Shader 
                                              OpCapability Geometry 
                                              OpCapability ShaderViewportIndexLayerEXT 
                                              OpExtension "SPV_EXT_shader_viewport_index_layer" 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %52 %84 %96 %97 %100 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %18 ArrayStride 18 
                                              OpMemberDecorate %19 0 Offset 19 
                                              OpMemberDecorate %19 1 Offset 19 
                                              OpDecorate %19 Block 
                                              OpDecorate %21 DescriptorSet 21 
                                              OpDecorate %21 Binding 21 
                                              OpDecorate %52 BuiltIn 52 
                                              OpMemberDecorate %82 0 BuiltIn 82 
                                              OpMemberDecorate %82 1 BuiltIn 82 
                                              OpMemberDecorate %82 2 BuiltIn 82 
                                              OpDecorate %82 Block 
                                              OpDecorate %96 Location 96 
                                              OpDecorate %97 Location 97 
                                              OpDecorate %100 BuiltIn 100 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                  u32 %17 = OpConstant 8 
                                      %18 = OpTypeArray %7 %17 
                                      %19 = OpTypeStruct %16 %18 
                                      %20 = OpTypePointer Uniform %19 
Uniform struct {f32_4[4]; f32_4[8];}* %21 = OpVariable Uniform 
                                      %22 = OpTypeInt 32 1 
                                  i32 %23 = OpConstant 0 
                                  i32 %24 = OpConstant 1 
                                      %25 = OpTypePointer Uniform %7 
                                  i32 %36 = OpConstant 2 
                                  i32 %45 = OpConstant 3 
                                      %49 = OpTypePointer Private %22 
                         Private i32* %50 = OpVariable Private 
                                      %51 = OpTypePointer Input %22 
                           Input i32* %52 = OpVariable Input 
                       Private f32_4* %55 = OpVariable Private 
                                  u32 %80 = OpConstant 1 
                                      %81 = OpTypeArray %6 %80 
                                      %82 = OpTypeStruct %7 %6 %81 
                                      %83 = OpTypePointer Output %82 
 Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                      %94 = OpTypePointer Output %7 
                        Output f32_4* %96 = OpVariable Output 
                         Input f32_4* %97 = OpVariable Input 
                                      %99 = OpTypePointer Output %22 
                         Output i32* %100 = OpVariable Output 
                                     %105 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %26 = OpAccessChain %21 %23 %24 
                                f32_4 %27 = OpLoad %26 
                                f32_4 %28 = OpFMul %13 %27 
                                              OpStore %9 %28 
                       Uniform f32_4* %29 = OpAccessChain %21 %23 %23 
                                f32_4 %30 = OpLoad %29 
                                f32_4 %31 = OpLoad %11 
                                f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                f32_4 %33 = OpFMul %30 %32 
                                f32_4 %34 = OpLoad %9 
                                f32_4 %35 = OpFAdd %33 %34 
                                              OpStore %9 %35 
                       Uniform f32_4* %37 = OpAccessChain %21 %23 %36 
                                f32_4 %38 = OpLoad %37 
                                f32_4 %39 = OpLoad %11 
                                f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                f32_4 %41 = OpFMul %38 %40 
                                f32_4 %42 = OpLoad %9 
                                f32_4 %43 = OpFAdd %41 %42 
                                              OpStore %9 %43 
                                f32_4 %44 = OpLoad %9 
                       Uniform f32_4* %46 = OpAccessChain %21 %23 %45 
                                f32_4 %47 = OpLoad %46 
                                f32_4 %48 = OpFAdd %44 %47 
                                              OpStore %9 %48 
                                  i32 %53 = OpLoad %52 
                                  i32 %54 = OpBitFieldInsert %23 %53 %36 %24 
                                              OpStore %50 %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                  i32 %58 = OpLoad %50 
                                  i32 %59 = OpIAdd %58 %24 
                       Uniform f32_4* %60 = OpAccessChain %21 %24 %59 
                                f32_4 %61 = OpLoad %60 
                                f32_4 %62 = OpFMul %57 %61 
                                              OpStore %55 %62 
                                  i32 %63 = OpLoad %50 
                       Uniform f32_4* %64 = OpAccessChain %21 %24 %63 
                                f32_4 %65 = OpLoad %64 
                                f32_4 %66 = OpLoad %9 
                                f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                f32_4 %68 = OpFMul %65 %67 
                                f32_4 %69 = OpLoad %55 
                                f32_4 %70 = OpFAdd %68 %69 
                                              OpStore %55 %70 
                                  i32 %71 = OpLoad %50 
                                  i32 %72 = OpIAdd %71 %36 
                       Uniform f32_4* %73 = OpAccessChain %21 %24 %72 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %55 
                                f32_4 %79 = OpFAdd %77 %78 
                                              OpStore %55 %79 
                                  i32 %85 = OpLoad %50 
                                  i32 %86 = OpIAdd %85 %45 
                       Uniform f32_4* %87 = OpAccessChain %21 %24 %86 
                                f32_4 %88 = OpLoad %87 
                                f32_4 %89 = OpLoad %9 
                                f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                f32_4 %91 = OpFMul %88 %90 
                                f32_4 %92 = OpLoad %55 
                                f32_4 %93 = OpFAdd %91 %92 
                        Output f32_4* %95 = OpAccessChain %84 %23 
                                              OpStore %95 %93 
                                f32_4 %98 = OpLoad %97 
                                              OpStore %96 %98 
                                 i32 %101 = OpLoad %52 
                                 u32 %102 = OpBitcast %101 
                                 u32 %103 = OpBitwiseAnd %102 %80 
                                 i32 %104 = OpBitcast %103 
                                              OpStore %100 %104 
                         Output f32* %106 = OpAccessChain %84 %23 %80 
                                 f32 %107 = OpLoad %106 
                                 f32 %108 = OpFNegate %107 
                         Output f32* %109 = OpAccessChain %84 %23 %80 
                                              OpStore %109 %108 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
#endif
	UNITY_UNIFORM vec4 unity_StereoEyeIndices[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
layout(num_views = 2) in;
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
out highp float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(int(gl_ViewID_OVR) << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
	vec4 unity_StereoEyeIndices[2];
};
layout(num_views = 2) in;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
out float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(gl_ViewID_OVR) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 134
; Schema: 0
                                                      OpCapability Shader 
                                                      OpCapability MultiView 
                                                      OpExtension "SPV_KHR_multiview" 
                                               %1 = OpExtInstImport "GLSL.std.450" 
                                                      OpMemoryModel Logical GLSL450 
                                                      OpEntryPoint Vertex %4 "main" %11 %50 %99 %111 %112 %118 
                                                      OpDecorate %11 Location 11 
                                                      OpDecorate %16 ArrayStride 16 
                                                      OpMemberDecorate %17 0 Offset 17 
                                                      OpDecorate %17 Block 
                                                      OpDecorate %19 DescriptorSet 19 
                                                      OpDecorate %19 Binding 19 
                                                      OpDecorate %50 BuiltIn 50 
                                                      OpDecorate %58 ArrayStride 58 
                                                      OpDecorate %59 ArrayStride 59 
                                                      OpDecorate %60 ArrayStride 60 
                                                      OpDecorate %61 ArrayStride 61 
                                                      OpDecorate %62 ArrayStride 62 
                                                      OpDecorate %63 ArrayStride 63 
                                                      OpDecorate %64 ArrayStride 64 
                                                      OpDecorate %65 ArrayStride 65 
                                                      OpDecorate %68 ArrayStride 68 
                                                      OpDecorate %69 ArrayStride 69 
                                                      OpMemberDecorate %70 0 Offset 70 
                                                      OpMemberDecorate %70 1 Offset 70 
                                                      OpMemberDecorate %70 2 Offset 70 
                                                      OpMemberDecorate %70 3 Offset 70 
                                                      OpMemberDecorate %70 4 Offset 70 
                                                      OpMemberDecorate %70 5 Offset 70 
                                                      OpMemberDecorate %70 6 Offset 70 
                                                      OpMemberDecorate %70 7 Offset 70 
                                                      OpMemberDecorate %70 8 Offset 70 
                                                      OpMemberDecorate %70 9 Offset 70 
                                                      OpDecorate %70 Block 
                                                      OpDecorate %72 DescriptorSet 72 
                                                      OpDecorate %72 Binding 72 
                                                      OpMemberDecorate %97 0 BuiltIn 97 
                                                      OpMemberDecorate %97 1 BuiltIn 97 
                                                      OpMemberDecorate %97 2 BuiltIn 97 
                                                      OpDecorate %97 Block 
                                                      OpDecorate %111 Location 111 
                                                      OpDecorate %112 Location 112 
                                                      OpDecorate %118 Location 118 
                                                      OpDecorate %119 ArrayStride 119 
                                                      OpMemberDecorate %120 0 Offset 120 
                                                      OpDecorate %120 Block 
                                                      OpDecorate %122 DescriptorSet 122 
                                                      OpDecorate %122 Binding 122 
                                               %2 = OpTypeVoid 
                                               %3 = OpTypeFunction %2 
                                               %6 = OpTypeFloat 32 
                                               %7 = OpTypeVector %6 4 
                                               %8 = OpTypePointer Private %7 
                                Private f32_4* %9 = OpVariable Private 
                                              %10 = OpTypePointer Input %7 
                                 Input f32_4* %11 = OpVariable Input 
                                              %14 = OpTypeInt 32 0 
                                          u32 %15 = OpConstant 4 
                                              %16 = OpTypeArray %7 %15 
                                              %17 = OpTypeStruct %16 
                                              %18 = OpTypePointer Uniform %17 
                  Uniform struct {f32_4[4];}* %19 = OpVariable Uniform 
                                              %20 = OpTypeInt 32 1 
                                          i32 %21 = OpConstant 0 
                                          i32 %22 = OpConstant 1 
                                              %23 = OpTypePointer Uniform %7 
                                          i32 %34 = OpConstant 2 
                                          i32 %43 = OpConstant 3 
                                              %47 = OpTypePointer Private %20 
                                 Private i32* %48 = OpVariable Private 
                                              %49 = OpTypePointer Input %14 
                                   Input u32* %50 = OpVariable Input 
                               Private f32_4* %54 = OpVariable Private 
                                          u32 %57 = OpConstant 8 
                                              %58 = OpTypeArray %7 %57 
                                              %59 = OpTypeArray %7 %57 
                                              %60 = OpTypeArray %7 %57 
                                              %61 = OpTypeArray %7 %57 
                                              %62 = OpTypeArray %7 %57 
                                              %63 = OpTypeArray %7 %57 
                                              %64 = OpTypeArray %7 %57 
                                              %65 = OpTypeArray %7 %57 
                                              %66 = OpTypeVector %6 3 
                                          u32 %67 = OpConstant 2 
                                              %68 = OpTypeArray %66 %67 
                                              %69 = OpTypeArray %7 %67 
                                              %70 = OpTypeStruct %58 %59 %60 %61 %62 %63 %64 %65 %68 %69 
                                              %71 = OpTypePointer Uniform %70 
Uniform struct {f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_3[2]; f32_4[2];}* %72 = OpVariable Uniform 
                                          u32 %95 = OpConstant 1 
                                              %96 = OpTypeArray %6 %95 
                                              %97 = OpTypeStruct %7 %6 %96 
                                              %98 = OpTypePointer Output %97 
         Output struct {f32_4; f32; f32[1];}* %99 = OpVariable Output 
                                             %109 = OpTypePointer Output %7 
                               Output f32_4* %111 = OpVariable Output 
                                Input f32_4* %112 = OpVariable Input 
                                             %114 = OpTypePointer Private %14 
                                Private u32* %115 = OpVariable Private 
                                             %117 = OpTypePointer Output %6 
                                 Output f32* %118 = OpVariable Output 
                                             %119 = OpTypeArray %7 %67 
                                             %120 = OpTypeStruct %119 
                                             %121 = OpTypePointer Uniform %120 
                 Uniform struct {f32_4[2];}* %122 = OpVariable Uniform 
                                         u32 %125 = OpConstant 0 
                                             %126 = OpTypePointer Uniform %6 
                                          void %4 = OpFunction None %3 
                                               %5 = OpLabel 
                                        f32_4 %12 = OpLoad %11 
                                        f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                               Uniform f32_4* %24 = OpAccessChain %19 %21 %22 
                                        f32_4 %25 = OpLoad %24 
                                        f32_4 %26 = OpFMul %13 %25 
                                                      OpStore %9 %26 
                               Uniform f32_4* %27 = OpAccessChain %19 %21 %21 
                                        f32_4 %28 = OpLoad %27 
                                        f32_4 %29 = OpLoad %11 
                                        f32_4 %30 = OpVectorShuffle %29 %29 0 0 0 0 
                                        f32_4 %31 = OpFMul %28 %30 
                                        f32_4 %32 = OpLoad %9 
                                        f32_4 %33 = OpFAdd %31 %32 
                                                      OpStore %9 %33 
                               Uniform f32_4* %35 = OpAccessChain %19 %21 %34 
                                        f32_4 %36 = OpLoad %35 
                                        f32_4 %37 = OpLoad %11 
                                        f32_4 %38 = OpVectorShuffle %37 %37 2 2 2 2 
                                        f32_4 %39 = OpFMul %36 %38 
                                        f32_4 %40 = OpLoad %9 
                                        f32_4 %41 = OpFAdd %39 %40 
                                                      OpStore %9 %41 
                                        f32_4 %42 = OpLoad %9 
                               Uniform f32_4* %44 = OpAccessChain %19 %21 %43 
                                        f32_4 %45 = OpLoad %44 
                                        f32_4 %46 = OpFAdd %42 %45 
                                                      OpStore %9 %46 
                                          u32 %51 = OpLoad %50 
                                          i32 %52 = OpBitcast %51 
                                          i32 %53 = OpShiftLeftLogical %52 %34 
                                                      OpStore %48 %53 
                                        f32_4 %55 = OpLoad %9 
                                        f32_4 %56 = OpVectorShuffle %55 %55 1 1 1 1 
                                          i32 %73 = OpLoad %48 
                                          i32 %74 = OpIAdd %73 %22 
                               Uniform f32_4* %75 = OpAccessChain %72 %43 %74 
                                        f32_4 %76 = OpLoad %75 
                                        f32_4 %77 = OpFMul %56 %76 
                                                      OpStore %54 %77 
                                          i32 %78 = OpLoad %48 
                               Uniform f32_4* %79 = OpAccessChain %72 %43 %78 
                                        f32_4 %80 = OpLoad %79 
                                        f32_4 %81 = OpLoad %9 
                                        f32_4 %82 = OpVectorShuffle %81 %81 0 0 0 0 
                                        f32_4 %83 = OpFMul %80 %82 
                                        f32_4 %84 = OpLoad %54 
                                        f32_4 %85 = OpFAdd %83 %84 
                                                      OpStore %54 %85 
                                          i32 %86 = OpLoad %48 
                                          i32 %87 = OpIAdd %86 %34 
                               Uniform f32_4* %88 = OpAccessChain %72 %43 %87 
                                        f32_4 %89 = OpLoad %88 
                                        f32_4 %90 = OpLoad %9 
                                        f32_4 %91 = OpVectorShuffle %90 %90 2 2 2 2 
                                        f32_4 %92 = OpFMul %89 %91 
                                        f32_4 %93 = OpLoad %54 
                                        f32_4 %94 = OpFAdd %92 %93 
                                                      OpStore %54 %94 
                                         i32 %100 = OpLoad %48 
                                         i32 %101 = OpIAdd %100 %43 
                              Uniform f32_4* %102 = OpAccessChain %72 %43 %101 
                                       f32_4 %103 = OpLoad %102 
                                       f32_4 %104 = OpLoad %9 
                                       f32_4 %105 = OpVectorShuffle %104 %104 3 3 3 3 
                                       f32_4 %106 = OpFMul %103 %105 
                                       f32_4 %107 = OpLoad %54 
                                       f32_4 %108 = OpFAdd %106 %107 
                               Output f32_4* %110 = OpAccessChain %99 %21 
                                                      OpStore %110 %108 
                                       f32_4 %113 = OpLoad %112 
                                                      OpStore %111 %113 
                                         u32 %116 = OpLoad %50 
                                                      OpStore %115 %116 
                                         u32 %123 = OpLoad %115 
                                         i32 %124 = OpBitcast %123 
                                Uniform f32* %127 = OpAccessChain %122 %21 %124 %125 
                                         f32 %128 = OpLoad %127 
                                                      OpStore %118 %128 
                                 Output f32* %129 = OpAccessChain %99 %21 %95 
                                         f32 %130 = OpLoad %129 
                                         f32 %131 = OpFNegate %130 
                                 Output f32* %132 = OpAccessChain %99 %21 %95 
                                                      OpStore %132 %131 
                                                      OpReturn
                                                      OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
""
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
""
}
SubProgram "vulkan " {
""
}
SubProgram "gles " {
""
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
}
}
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 113352
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 91
; Schema: 0
                                              OpCapability Shader 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %72 %82 %83 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %17 ArrayStride 17 
                                              OpMemberDecorate %18 0 Offset 18 
                                              OpMemberDecorate %18 1 Offset 18 
                                              OpDecorate %18 Block 
                                              OpDecorate %20 DescriptorSet 20 
                                              OpDecorate %20 Binding 20 
                                              OpMemberDecorate %70 0 BuiltIn 70 
                                              OpMemberDecorate %70 1 BuiltIn 70 
                                              OpMemberDecorate %70 2 BuiltIn 70 
                                              OpDecorate %70 Block 
                                              OpDecorate %82 Location 82 
                                              OpDecorate %83 Location 83 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                      %17 = OpTypeArray %7 %15 
                                      %18 = OpTypeStruct %16 %17 
                                      %19 = OpTypePointer Uniform %18 
Uniform struct {f32_4[4]; f32_4[4];}* %20 = OpVariable Uniform 
                                      %21 = OpTypeInt 32 1 
                                  i32 %22 = OpConstant 0 
                                  i32 %23 = OpConstant 1 
                                      %24 = OpTypePointer Uniform %7 
                                  i32 %35 = OpConstant 2 
                                  i32 %44 = OpConstant 3 
                       Private f32_4* %48 = OpVariable Private 
                                  u32 %68 = OpConstant 1 
                                      %69 = OpTypeArray %6 %68 
                                      %70 = OpTypeStruct %7 %6 %69 
                                      %71 = OpTypePointer Output %70 
 Output struct {f32_4; f32; f32[1];}* %72 = OpVariable Output 
                                      %80 = OpTypePointer Output %7 
                        Output f32_4* %82 = OpVariable Output 
                         Input f32_4* %83 = OpVariable Input 
                                      %85 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %25 = OpAccessChain %20 %22 %23 
                                f32_4 %26 = OpLoad %25 
                                f32_4 %27 = OpFMul %13 %26 
                                              OpStore %9 %27 
                       Uniform f32_4* %28 = OpAccessChain %20 %22 %22 
                                f32_4 %29 = OpLoad %28 
                                f32_4 %30 = OpLoad %11 
                                f32_4 %31 = OpVectorShuffle %30 %30 0 0 0 0 
                                f32_4 %32 = OpFMul %29 %31 
                                f32_4 %33 = OpLoad %9 
                                f32_4 %34 = OpFAdd %32 %33 
                                              OpStore %9 %34 
                       Uniform f32_4* %36 = OpAccessChain %20 %22 %35 
                                f32_4 %37 = OpLoad %36 
                                f32_4 %38 = OpLoad %11 
                                f32_4 %39 = OpVectorShuffle %38 %38 2 2 2 2 
                                f32_4 %40 = OpFMul %37 %39 
                                f32_4 %41 = OpLoad %9 
                                f32_4 %42 = OpFAdd %40 %41 
                                              OpStore %9 %42 
                                f32_4 %43 = OpLoad %9 
                       Uniform f32_4* %45 = OpAccessChain %20 %22 %44 
                                f32_4 %46 = OpLoad %45 
                                f32_4 %47 = OpFAdd %43 %46 
                                              OpStore %9 %47 
                                f32_4 %49 = OpLoad %9 
                                f32_4 %50 = OpVectorShuffle %49 %49 1 1 1 1 
                       Uniform f32_4* %51 = OpAccessChain %20 %23 %23 
                                f32_4 %52 = OpLoad %51 
                                f32_4 %53 = OpFMul %50 %52 
                                              OpStore %48 %53 
                       Uniform f32_4* %54 = OpAccessChain %20 %23 %22 
                                f32_4 %55 = OpLoad %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 0 0 0 0 
                                f32_4 %58 = OpFMul %55 %57 
                                f32_4 %59 = OpLoad %48 
                                f32_4 %60 = OpFAdd %58 %59 
                                              OpStore %48 %60 
                       Uniform f32_4* %61 = OpAccessChain %20 %23 %35 
                                f32_4 %62 = OpLoad %61 
                                f32_4 %63 = OpLoad %9 
                                f32_4 %64 = OpVectorShuffle %63 %63 2 2 2 2 
                                f32_4 %65 = OpFMul %62 %64 
                                f32_4 %66 = OpLoad %48 
                                f32_4 %67 = OpFAdd %65 %66 
                                              OpStore %48 %67 
                       Uniform f32_4* %73 = OpAccessChain %20 %23 %44 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 3 3 3 3 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %48 
                                f32_4 %79 = OpFAdd %77 %78 
                        Output f32_4* %81 = OpAccessChain %72 %22 
                                              OpStore %81 %79 
                                f32_4 %84 = OpLoad %83 
                                              OpStore %82 %84 
                          Output f32* %86 = OpAccessChain %72 %22 %68 
                                  f32 %87 = OpLoad %86 
                                  f32 %88 = OpFNegate %87 
                          Output f32* %89 = OpAccessChain %72 %22 %68 
                                              OpStore %89 %88 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
uniform 	int unity_StereoEyeIndex;
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
int op_shl(int a, int b) { return int(floor(float(a) * pow(2.0, float(b)))); }
ivec2 op_shl(ivec2 a, ivec2 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); return a; }
ivec3 op_shl(ivec3 a, ivec3 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); return a; }
ivec4 op_shl(ivec4 a, ivec4 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); a.w = op_shl(a.w, b.w); return a; }

void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = op_shl(unity_StereoEyeIndex, 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
#endif
	UNITY_UNIFORM int unity_StereoEyeIndex;
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(unity_StereoEyeIndex << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
};

struct UnityStereoGlobals_Type
{
    float4 hlslcc_mtx4x4unity_StereoMatrixP[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
    float4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
    float4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
    float4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
    float4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
    float3 unity_StereoWorldSpaceCameraPos[2];
    float4 unity_StereoScaleOffset[2];
};

struct UnityStereoEyeIndex_Type
{
    int unity_StereoEyeIndex;
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    constant UnityStereoGlobals_Type& UnityStereoGlobals [[ buffer(1) ]],
    constant UnityStereoEyeIndex_Type& UnityStereoEyeIndex [[ buffer(2) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    int u_xlati1;
    float4 u_xlat2;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = UnityStereoEyeIndex.unity_StereoEyeIndex << 0x2;
    u_xlat2 = u_xlat0.yyyy * UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1], u_xlat0.xxxx, u_xlat2);
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)], u_xlat0.zzzz, u_xlat2);
    output.mtl_Position = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)], u_xlat0.wwww, u_xlat2);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
	int unity_StereoEyeIndex;
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = unity_StereoEyeIndex << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 105
; Schema: 0
                                                   OpCapability Shader 
                                            %1 = OpExtInstImport "GLSL.std.450" 
                                                   OpMemoryModel Logical GLSL450 
                                                   OpEntryPoint Vertex %4 "main" %11 %84 %96 %97 
                                                   OpDecorate %11 Location 11 
                                                   OpDecorate %16 ArrayStride 16 
                                                   OpDecorate %18 ArrayStride 18 
                                                   OpMemberDecorate %20 0 Offset 20 
                                                   OpMemberDecorate %20 1 Offset 20 
                                                   OpMemberDecorate %20 2 Offset 20 
                                                   OpDecorate %20 Block 
                                                   OpDecorate %22 DescriptorSet 22 
                                                   OpDecorate %22 Binding 22 
                                                   OpMemberDecorate %82 0 BuiltIn 82 
                                                   OpMemberDecorate %82 1 BuiltIn 82 
                                                   OpMemberDecorate %82 2 BuiltIn 82 
                                                   OpDecorate %82 Block 
                                                   OpDecorate %96 Location 96 
                                                   OpDecorate %97 Location 97 
                                            %2 = OpTypeVoid 
                                            %3 = OpTypeFunction %2 
                                            %6 = OpTypeFloat 32 
                                            %7 = OpTypeVector %6 4 
                                            %8 = OpTypePointer Private %7 
                             Private f32_4* %9 = OpVariable Private 
                                           %10 = OpTypePointer Input %7 
                              Input f32_4* %11 = OpVariable Input 
                                           %14 = OpTypeInt 32 0 
                                       u32 %15 = OpConstant 4 
                                           %16 = OpTypeArray %7 %15 
                                       u32 %17 = OpConstant 8 
                                           %18 = OpTypeArray %7 %17 
                                           %19 = OpTypeInt 32 1 
                                           %20 = OpTypeStruct %16 %18 %19 
                                           %21 = OpTypePointer Uniform %20 
Uniform struct {f32_4[4]; f32_4[8]; i32;}* %22 = OpVariable Uniform 
                                       i32 %23 = OpConstant 0 
                                       i32 %24 = OpConstant 1 
                                           %25 = OpTypePointer Uniform %7 
                                       i32 %36 = OpConstant 2 
                                       i32 %45 = OpConstant 3 
                                           %49 = OpTypePointer Private %19 
                              Private i32* %50 = OpVariable Private 
                                           %51 = OpTypePointer Uniform %19 
                            Private f32_4* %55 = OpVariable Private 
                                       u32 %80 = OpConstant 1 
                                           %81 = OpTypeArray %6 %80 
                                           %82 = OpTypeStruct %7 %6 %81 
                                           %83 = OpTypePointer Output %82 
      Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                           %94 = OpTypePointer Output %7 
                             Output f32_4* %96 = OpVariable Output 
                              Input f32_4* %97 = OpVariable Input 
                                           %99 = OpTypePointer Output %6 
                                       void %4 = OpFunction None %3 
                                            %5 = OpLabel 
                                     f32_4 %12 = OpLoad %11 
                                     f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                            Uniform f32_4* %26 = OpAccessChain %22 %23 %24 
                                     f32_4 %27 = OpLoad %26 
                                     f32_4 %28 = OpFMul %13 %27 
                                                   OpStore %9 %28 
                            Uniform f32_4* %29 = OpAccessChain %22 %23 %23 
                                     f32_4 %30 = OpLoad %29 
                                     f32_4 %31 = OpLoad %11 
                                     f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                     f32_4 %33 = OpFMul %30 %32 
                                     f32_4 %34 = OpLoad %9 
                                     f32_4 %35 = OpFAdd %33 %34 
                                                   OpStore %9 %35 
                            Uniform f32_4* %37 = OpAccessChain %22 %23 %36 
                                     f32_4 %38 = OpLoad %37 
                                     f32_4 %39 = OpLoad %11 
                                     f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                     f32_4 %41 = OpFMul %38 %40 
                                     f32_4 %42 = OpLoad %9 
                                     f32_4 %43 = OpFAdd %41 %42 
                                                   OpStore %9 %43 
                                     f32_4 %44 = OpLoad %9 
                            Uniform f32_4* %46 = OpAccessChain %22 %23 %45 
                                     f32_4 %47 = OpLoad %46 
                                     f32_4 %48 = OpFAdd %44 %47 
                                                   OpStore %9 %48 
                              Uniform i32* %52 = OpAccessChain %22 %36 
                                       i32 %53 = OpLoad %52 
                                       i32 %54 = OpShiftLeftLogical %53 %36 
                                                   OpStore %50 %54 
                                     f32_4 %56 = OpLoad %9 
                                     f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                       i32 %58 = OpLoad %50 
                                       i32 %59 = OpIAdd %58 %24 
                            Uniform f32_4* %60 = OpAccessChain %22 %24 %59 
                                     f32_4 %61 = OpLoad %60 
                                     f32_4 %62 = OpFMul %57 %61 
                                                   OpStore %55 %62 
                                       i32 %63 = OpLoad %50 
                            Uniform f32_4* %64 = OpAccessChain %22 %24 %63 
                                     f32_4 %65 = OpLoad %64 
                                     f32_4 %66 = OpLoad %9 
                                     f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                     f32_4 %68 = OpFMul %65 %67 
                                     f32_4 %69 = OpLoad %55 
                                     f32_4 %70 = OpFAdd %68 %69 
                                                   OpStore %55 %70 
                                       i32 %71 = OpLoad %50 
                                       i32 %72 = OpIAdd %71 %36 
                            Uniform f32_4* %73 = OpAccessChain %22 %24 %72 
                                     f32_4 %74 = OpLoad %73 
                                     f32_4 %75 = OpLoad %9 
                                     f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                     f32_4 %77 = OpFMul %74 %76 
                                     f32_4 %78 = OpLoad %55 
                                     f32_4 %79 = OpFAdd %77 %78 
                                                   OpStore %55 %79 
                                       i32 %85 = OpLoad %50 
                                       i32 %86 = OpIAdd %85 %45 
                            Uniform f32_4* %87 = OpAccessChain %22 %24 %86 
                                     f32_4 %88 = OpLoad %87 
                                     f32_4 %89 = OpLoad %9 
                                     f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                     f32_4 %91 = OpFMul %88 %90 
                                     f32_4 %92 = OpLoad %55 
                                     f32_4 %93 = OpFAdd %91 %92 
                             Output f32_4* %95 = OpAccessChain %84 %23 
                                                   OpStore %95 %93 
                                     f32_4 %98 = OpLoad %97 
                                                   OpStore %96 %98 
                              Output f32* %100 = OpAccessChain %84 %23 %80 
                                      f32 %101 = OpLoad %100 
                                      f32 %102 = OpFNegate %101 
                              Output f32* %103 = OpAccessChain %84 %23 %80 
                                                   OpStore %103 %102 
                                                   OpReturn
                                                   OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
flat out highp uint vs_BLENDINDICES0;
float u_xlat0;
uint u_xlatu0;
vec4 u_xlat1;
vec4 u_xlat2;
int u_xlati3;
void main()
{
    u_xlat0 = float(uint(gl_InstanceID));
    u_xlat0 = u_xlat0 * 0.5;
    u_xlat0 = fract(u_xlat0);
    u_xlat0 = u_xlat0 + u_xlat0;
    u_xlat0 = roundEven(u_xlat0);
    u_xlatu0 = uint(u_xlat0);
    u_xlati3 = int(int(u_xlatu0) << 2);
    u_xlat1 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
    u_xlat1 = u_xlat1 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat2 = u_xlat1.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati3] * u_xlat1.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 2)] * u_xlat1.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 3)] * u_xlat1.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu0;
    gl_Layer = int(u_xlatu0);
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
flat out uint vs_BLENDINDICES0;
vec4 u_xlat0;
uint u_xlatu1;
vec4 u_xlat2;
int u_xlati4;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlatu1 = uint(gl_InstanceID) & 1u;
    u_xlati4 = int(u_xlatu1) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati4] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu1;
    gl_Layer = int(u_xlatu1);
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 111
; Schema: 0
                                              OpCapability Shader 
                                              OpCapability Geometry 
                                              OpCapability ShaderViewportIndexLayerEXT 
                                              OpExtension "SPV_EXT_shader_viewport_index_layer" 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %52 %84 %96 %97 %100 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %18 ArrayStride 18 
                                              OpMemberDecorate %19 0 Offset 19 
                                              OpMemberDecorate %19 1 Offset 19 
                                              OpDecorate %19 Block 
                                              OpDecorate %21 DescriptorSet 21 
                                              OpDecorate %21 Binding 21 
                                              OpDecorate %52 BuiltIn 52 
                                              OpMemberDecorate %82 0 BuiltIn 82 
                                              OpMemberDecorate %82 1 BuiltIn 82 
                                              OpMemberDecorate %82 2 BuiltIn 82 
                                              OpDecorate %82 Block 
                                              OpDecorate %96 Location 96 
                                              OpDecorate %97 Location 97 
                                              OpDecorate %100 BuiltIn 100 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                  u32 %17 = OpConstant 8 
                                      %18 = OpTypeArray %7 %17 
                                      %19 = OpTypeStruct %16 %18 
                                      %20 = OpTypePointer Uniform %19 
Uniform struct {f32_4[4]; f32_4[8];}* %21 = OpVariable Uniform 
                                      %22 = OpTypeInt 32 1 
                                  i32 %23 = OpConstant 0 
                                  i32 %24 = OpConstant 1 
                                      %25 = OpTypePointer Uniform %7 
                                  i32 %36 = OpConstant 2 
                                  i32 %45 = OpConstant 3 
                                      %49 = OpTypePointer Private %22 
                         Private i32* %50 = OpVariable Private 
                                      %51 = OpTypePointer Input %22 
                           Input i32* %52 = OpVariable Input 
                       Private f32_4* %55 = OpVariable Private 
                                  u32 %80 = OpConstant 1 
                                      %81 = OpTypeArray %6 %80 
                                      %82 = OpTypeStruct %7 %6 %81 
                                      %83 = OpTypePointer Output %82 
 Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                      %94 = OpTypePointer Output %7 
                        Output f32_4* %96 = OpVariable Output 
                         Input f32_4* %97 = OpVariable Input 
                                      %99 = OpTypePointer Output %22 
                         Output i32* %100 = OpVariable Output 
                                     %105 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %26 = OpAccessChain %21 %23 %24 
                                f32_4 %27 = OpLoad %26 
                                f32_4 %28 = OpFMul %13 %27 
                                              OpStore %9 %28 
                       Uniform f32_4* %29 = OpAccessChain %21 %23 %23 
                                f32_4 %30 = OpLoad %29 
                                f32_4 %31 = OpLoad %11 
                                f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                f32_4 %33 = OpFMul %30 %32 
                                f32_4 %34 = OpLoad %9 
                                f32_4 %35 = OpFAdd %33 %34 
                                              OpStore %9 %35 
                       Uniform f32_4* %37 = OpAccessChain %21 %23 %36 
                                f32_4 %38 = OpLoad %37 
                                f32_4 %39 = OpLoad %11 
                                f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                f32_4 %41 = OpFMul %38 %40 
                                f32_4 %42 = OpLoad %9 
                                f32_4 %43 = OpFAdd %41 %42 
                                              OpStore %9 %43 
                                f32_4 %44 = OpLoad %9 
                       Uniform f32_4* %46 = OpAccessChain %21 %23 %45 
                                f32_4 %47 = OpLoad %46 
                                f32_4 %48 = OpFAdd %44 %47 
                                              OpStore %9 %48 
                                  i32 %53 = OpLoad %52 
                                  i32 %54 = OpBitFieldInsert %23 %53 %36 %24 
                                              OpStore %50 %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                  i32 %58 = OpLoad %50 
                                  i32 %59 = OpIAdd %58 %24 
                       Uniform f32_4* %60 = OpAccessChain %21 %24 %59 
                                f32_4 %61 = OpLoad %60 
                                f32_4 %62 = OpFMul %57 %61 
                                              OpStore %55 %62 
                                  i32 %63 = OpLoad %50 
                       Uniform f32_4* %64 = OpAccessChain %21 %24 %63 
                                f32_4 %65 = OpLoad %64 
                                f32_4 %66 = OpLoad %9 
                                f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                f32_4 %68 = OpFMul %65 %67 
                                f32_4 %69 = OpLoad %55 
                                f32_4 %70 = OpFAdd %68 %69 
                                              OpStore %55 %70 
                                  i32 %71 = OpLoad %50 
                                  i32 %72 = OpIAdd %71 %36 
                       Uniform f32_4* %73 = OpAccessChain %21 %24 %72 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %55 
                                f32_4 %79 = OpFAdd %77 %78 
                                              OpStore %55 %79 
                                  i32 %85 = OpLoad %50 
                                  i32 %86 = OpIAdd %85 %45 
                       Uniform f32_4* %87 = OpAccessChain %21 %24 %86 
                                f32_4 %88 = OpLoad %87 
                                f32_4 %89 = OpLoad %9 
                                f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                f32_4 %91 = OpFMul %88 %90 
                                f32_4 %92 = OpLoad %55 
                                f32_4 %93 = OpFAdd %91 %92 
                        Output f32_4* %95 = OpAccessChain %84 %23 
                                              OpStore %95 %93 
                                f32_4 %98 = OpLoad %97 
                                              OpStore %96 %98 
                                 i32 %101 = OpLoad %52 
                                 u32 %102 = OpBitcast %101 
                                 u32 %103 = OpBitwiseAnd %102 %80 
                                 i32 %104 = OpBitcast %103 
                                              OpStore %100 %104 
                         Output f32* %106 = OpAccessChain %84 %23 %80 
                                 f32 %107 = OpLoad %106 
                                 f32 %108 = OpFNegate %107 
                         Output f32* %109 = OpAccessChain %84 %23 %80 
                                              OpStore %109 %108 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
#endif
	UNITY_UNIFORM vec4 unity_StereoEyeIndices[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
layout(num_views = 2) in;
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
out highp float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(int(gl_ViewID_OVR) << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
	vec4 unity_StereoEyeIndices[2];
};
layout(num_views = 2) in;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
out float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(gl_ViewID_OVR) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 134
; Schema: 0
                                                      OpCapability Shader 
                                                      OpCapability MultiView 
                                                      OpExtension "SPV_KHR_multiview" 
                                               %1 = OpExtInstImport "GLSL.std.450" 
                                                      OpMemoryModel Logical GLSL450 
                                                      OpEntryPoint Vertex %4 "main" %11 %50 %99 %111 %112 %118 
                                                      OpDecorate %11 Location 11 
                                                      OpDecorate %16 ArrayStride 16 
                                                      OpMemberDecorate %17 0 Offset 17 
                                                      OpDecorate %17 Block 
                                                      OpDecorate %19 DescriptorSet 19 
                                                      OpDecorate %19 Binding 19 
                                                      OpDecorate %50 BuiltIn 50 
                                                      OpDecorate %58 ArrayStride 58 
                                                      OpDecorate %59 ArrayStride 59 
                                                      OpDecorate %60 ArrayStride 60 
                                                      OpDecorate %61 ArrayStride 61 
                                                      OpDecorate %62 ArrayStride 62 
                                                      OpDecorate %63 ArrayStride 63 
                                                      OpDecorate %64 ArrayStride 64 
                                                      OpDecorate %65 ArrayStride 65 
                                                      OpDecorate %68 ArrayStride 68 
                                                      OpDecorate %69 ArrayStride 69 
                                                      OpMemberDecorate %70 0 Offset 70 
                                                      OpMemberDecorate %70 1 Offset 70 
                                                      OpMemberDecorate %70 2 Offset 70 
                                                      OpMemberDecorate %70 3 Offset 70 
                                                      OpMemberDecorate %70 4 Offset 70 
                                                      OpMemberDecorate %70 5 Offset 70 
                                                      OpMemberDecorate %70 6 Offset 70 
                                                      OpMemberDecorate %70 7 Offset 70 
                                                      OpMemberDecorate %70 8 Offset 70 
                                                      OpMemberDecorate %70 9 Offset 70 
                                                      OpDecorate %70 Block 
                                                      OpDecorate %72 DescriptorSet 72 
                                                      OpDecorate %72 Binding 72 
                                                      OpMemberDecorate %97 0 BuiltIn 97 
                                                      OpMemberDecorate %97 1 BuiltIn 97 
                                                      OpMemberDecorate %97 2 BuiltIn 97 
                                                      OpDecorate %97 Block 
                                                      OpDecorate %111 Location 111 
                                                      OpDecorate %112 Location 112 
                                                      OpDecorate %118 Location 118 
                                                      OpDecorate %119 ArrayStride 119 
                                                      OpMemberDecorate %120 0 Offset 120 
                                                      OpDecorate %120 Block 
                                                      OpDecorate %122 DescriptorSet 122 
                                                      OpDecorate %122 Binding 122 
                                               %2 = OpTypeVoid 
                                               %3 = OpTypeFunction %2 
                                               %6 = OpTypeFloat 32 
                                               %7 = OpTypeVector %6 4 
                                               %8 = OpTypePointer Private %7 
                                Private f32_4* %9 = OpVariable Private 
                                              %10 = OpTypePointer Input %7 
                                 Input f32_4* %11 = OpVariable Input 
                                              %14 = OpTypeInt 32 0 
                                          u32 %15 = OpConstant 4 
                                              %16 = OpTypeArray %7 %15 
                                              %17 = OpTypeStruct %16 
                                              %18 = OpTypePointer Uniform %17 
                  Uniform struct {f32_4[4];}* %19 = OpVariable Uniform 
                                              %20 = OpTypeInt 32 1 
                                          i32 %21 = OpConstant 0 
                                          i32 %22 = OpConstant 1 
                                              %23 = OpTypePointer Uniform %7 
                                          i32 %34 = OpConstant 2 
                                          i32 %43 = OpConstant 3 
                                              %47 = OpTypePointer Private %20 
                                 Private i32* %48 = OpVariable Private 
                                              %49 = OpTypePointer Input %14 
                                   Input u32* %50 = OpVariable Input 
                               Private f32_4* %54 = OpVariable Private 
                                          u32 %57 = OpConstant 8 
                                              %58 = OpTypeArray %7 %57 
                                              %59 = OpTypeArray %7 %57 
                                              %60 = OpTypeArray %7 %57 
                                              %61 = OpTypeArray %7 %57 
                                              %62 = OpTypeArray %7 %57 
                                              %63 = OpTypeArray %7 %57 
                                              %64 = OpTypeArray %7 %57 
                                              %65 = OpTypeArray %7 %57 
                                              %66 = OpTypeVector %6 3 
                                          u32 %67 = OpConstant 2 
                                              %68 = OpTypeArray %66 %67 
                                              %69 = OpTypeArray %7 %67 
                                              %70 = OpTypeStruct %58 %59 %60 %61 %62 %63 %64 %65 %68 %69 
                                              %71 = OpTypePointer Uniform %70 
Uniform struct {f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_3[2]; f32_4[2];}* %72 = OpVariable Uniform 
                                          u32 %95 = OpConstant 1 
                                              %96 = OpTypeArray %6 %95 
                                              %97 = OpTypeStruct %7 %6 %96 
                                              %98 = OpTypePointer Output %97 
         Output struct {f32_4; f32; f32[1];}* %99 = OpVariable Output 
                                             %109 = OpTypePointer Output %7 
                               Output f32_4* %111 = OpVariable Output 
                                Input f32_4* %112 = OpVariable Input 
                                             %114 = OpTypePointer Private %14 
                                Private u32* %115 = OpVariable Private 
                                             %117 = OpTypePointer Output %6 
                                 Output f32* %118 = OpVariable Output 
                                             %119 = OpTypeArray %7 %67 
                                             %120 = OpTypeStruct %119 
                                             %121 = OpTypePointer Uniform %120 
                 Uniform struct {f32_4[2];}* %122 = OpVariable Uniform 
                                         u32 %125 = OpConstant 0 
                                             %126 = OpTypePointer Uniform %6 
                                          void %4 = OpFunction None %3 
                                               %5 = OpLabel 
                                        f32_4 %12 = OpLoad %11 
                                        f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                               Uniform f32_4* %24 = OpAccessChain %19 %21 %22 
                                        f32_4 %25 = OpLoad %24 
                                        f32_4 %26 = OpFMul %13 %25 
                                                      OpStore %9 %26 
                               Uniform f32_4* %27 = OpAccessChain %19 %21 %21 
                                        f32_4 %28 = OpLoad %27 
                                        f32_4 %29 = OpLoad %11 
                                        f32_4 %30 = OpVectorShuffle %29 %29 0 0 0 0 
                                        f32_4 %31 = OpFMul %28 %30 
                                        f32_4 %32 = OpLoad %9 
                                        f32_4 %33 = OpFAdd %31 %32 
                                                      OpStore %9 %33 
                               Uniform f32_4* %35 = OpAccessChain %19 %21 %34 
                                        f32_4 %36 = OpLoad %35 
                                        f32_4 %37 = OpLoad %11 
                                        f32_4 %38 = OpVectorShuffle %37 %37 2 2 2 2 
                                        f32_4 %39 = OpFMul %36 %38 
                                        f32_4 %40 = OpLoad %9 
                                        f32_4 %41 = OpFAdd %39 %40 
                                                      OpStore %9 %41 
                                        f32_4 %42 = OpLoad %9 
                               Uniform f32_4* %44 = OpAccessChain %19 %21 %43 
                                        f32_4 %45 = OpLoad %44 
                                        f32_4 %46 = OpFAdd %42 %45 
                                                      OpStore %9 %46 
                                          u32 %51 = OpLoad %50 
                                          i32 %52 = OpBitcast %51 
                                          i32 %53 = OpShiftLeftLogical %52 %34 
                                                      OpStore %48 %53 
                                        f32_4 %55 = OpLoad %9 
                                        f32_4 %56 = OpVectorShuffle %55 %55 1 1 1 1 
                                          i32 %73 = OpLoad %48 
                                          i32 %74 = OpIAdd %73 %22 
                               Uniform f32_4* %75 = OpAccessChain %72 %43 %74 
                                        f32_4 %76 = OpLoad %75 
                                        f32_4 %77 = OpFMul %56 %76 
                                                      OpStore %54 %77 
                                          i32 %78 = OpLoad %48 
                               Uniform f32_4* %79 = OpAccessChain %72 %43 %78 
                                        f32_4 %80 = OpLoad %79 
                                        f32_4 %81 = OpLoad %9 
                                        f32_4 %82 = OpVectorShuffle %81 %81 0 0 0 0 
                                        f32_4 %83 = OpFMul %80 %82 
                                        f32_4 %84 = OpLoad %54 
                                        f32_4 %85 = OpFAdd %83 %84 
                                                      OpStore %54 %85 
                                          i32 %86 = OpLoad %48 
                                          i32 %87 = OpIAdd %86 %34 
                               Uniform f32_4* %88 = OpAccessChain %72 %43 %87 
                                        f32_4 %89 = OpLoad %88 
                                        f32_4 %90 = OpLoad %9 
                                        f32_4 %91 = OpVectorShuffle %90 %90 2 2 2 2 
                                        f32_4 %92 = OpFMul %89 %91 
                                        f32_4 %93 = OpLoad %54 
                                        f32_4 %94 = OpFAdd %92 %93 
                                                      OpStore %54 %94 
                                         i32 %100 = OpLoad %48 
                                         i32 %101 = OpIAdd %100 %43 
                              Uniform f32_4* %102 = OpAccessChain %72 %43 %101 
                                       f32_4 %103 = OpLoad %102 
                                       f32_4 %104 = OpLoad %9 
                                       f32_4 %105 = OpVectorShuffle %104 %104 3 3 3 3 
                                       f32_4 %106 = OpFMul %103 %105 
                                       f32_4 %107 = OpLoad %54 
                                       f32_4 %108 = OpFAdd %106 %107 
                               Output f32_4* %110 = OpAccessChain %99 %21 
                                                      OpStore %110 %108 
                                       f32_4 %113 = OpLoad %112 
                                                      OpStore %111 %113 
                                         u32 %116 = OpLoad %50 
                                                      OpStore %115 %116 
                                         u32 %123 = OpLoad %115 
                                         i32 %124 = OpBitcast %123 
                                Uniform f32* %127 = OpAccessChain %122 %21 %124 %125 
                                         f32 %128 = OpLoad %127 
                                                      OpStore %118 %128 
                                 Output f32* %129 = OpAccessChain %99 %21 %95 
                                         f32 %130 = OpLoad %129 
                                         f32 %131 = OpFNegate %130 
                                 Output f32* %132 = OpAccessChain %99 %21 %95 
                                                      OpStore %132 %131 
                                                      OpReturn
                                                      OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
""
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
""
}
SubProgram "vulkan " {
""
}
SubProgram "gles " {
""
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
}
}
 Pass {
  ColorMask 0 0
  ZTest Always
  Cull Off
  GpuProgramID 185454
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 91
; Schema: 0
                                              OpCapability Shader 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %72 %82 %83 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %17 ArrayStride 17 
                                              OpMemberDecorate %18 0 Offset 18 
                                              OpMemberDecorate %18 1 Offset 18 
                                              OpDecorate %18 Block 
                                              OpDecorate %20 DescriptorSet 20 
                                              OpDecorate %20 Binding 20 
                                              OpMemberDecorate %70 0 BuiltIn 70 
                                              OpMemberDecorate %70 1 BuiltIn 70 
                                              OpMemberDecorate %70 2 BuiltIn 70 
                                              OpDecorate %70 Block 
                                              OpDecorate %82 Location 82 
                                              OpDecorate %83 Location 83 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                      %17 = OpTypeArray %7 %15 
                                      %18 = OpTypeStruct %16 %17 
                                      %19 = OpTypePointer Uniform %18 
Uniform struct {f32_4[4]; f32_4[4];}* %20 = OpVariable Uniform 
                                      %21 = OpTypeInt 32 1 
                                  i32 %22 = OpConstant 0 
                                  i32 %23 = OpConstant 1 
                                      %24 = OpTypePointer Uniform %7 
                                  i32 %35 = OpConstant 2 
                                  i32 %44 = OpConstant 3 
                       Private f32_4* %48 = OpVariable Private 
                                  u32 %68 = OpConstant 1 
                                      %69 = OpTypeArray %6 %68 
                                      %70 = OpTypeStruct %7 %6 %69 
                                      %71 = OpTypePointer Output %70 
 Output struct {f32_4; f32; f32[1];}* %72 = OpVariable Output 
                                      %80 = OpTypePointer Output %7 
                        Output f32_4* %82 = OpVariable Output 
                         Input f32_4* %83 = OpVariable Input 
                                      %85 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %25 = OpAccessChain %20 %22 %23 
                                f32_4 %26 = OpLoad %25 
                                f32_4 %27 = OpFMul %13 %26 
                                              OpStore %9 %27 
                       Uniform f32_4* %28 = OpAccessChain %20 %22 %22 
                                f32_4 %29 = OpLoad %28 
                                f32_4 %30 = OpLoad %11 
                                f32_4 %31 = OpVectorShuffle %30 %30 0 0 0 0 
                                f32_4 %32 = OpFMul %29 %31 
                                f32_4 %33 = OpLoad %9 
                                f32_4 %34 = OpFAdd %32 %33 
                                              OpStore %9 %34 
                       Uniform f32_4* %36 = OpAccessChain %20 %22 %35 
                                f32_4 %37 = OpLoad %36 
                                f32_4 %38 = OpLoad %11 
                                f32_4 %39 = OpVectorShuffle %38 %38 2 2 2 2 
                                f32_4 %40 = OpFMul %37 %39 
                                f32_4 %41 = OpLoad %9 
                                f32_4 %42 = OpFAdd %40 %41 
                                              OpStore %9 %42 
                                f32_4 %43 = OpLoad %9 
                       Uniform f32_4* %45 = OpAccessChain %20 %22 %44 
                                f32_4 %46 = OpLoad %45 
                                f32_4 %47 = OpFAdd %43 %46 
                                              OpStore %9 %47 
                                f32_4 %49 = OpLoad %9 
                                f32_4 %50 = OpVectorShuffle %49 %49 1 1 1 1 
                       Uniform f32_4* %51 = OpAccessChain %20 %23 %23 
                                f32_4 %52 = OpLoad %51 
                                f32_4 %53 = OpFMul %50 %52 
                                              OpStore %48 %53 
                       Uniform f32_4* %54 = OpAccessChain %20 %23 %22 
                                f32_4 %55 = OpLoad %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 0 0 0 0 
                                f32_4 %58 = OpFMul %55 %57 
                                f32_4 %59 = OpLoad %48 
                                f32_4 %60 = OpFAdd %58 %59 
                                              OpStore %48 %60 
                       Uniform f32_4* %61 = OpAccessChain %20 %23 %35 
                                f32_4 %62 = OpLoad %61 
                                f32_4 %63 = OpLoad %9 
                                f32_4 %64 = OpVectorShuffle %63 %63 2 2 2 2 
                                f32_4 %65 = OpFMul %62 %64 
                                f32_4 %66 = OpLoad %48 
                                f32_4 %67 = OpFAdd %65 %66 
                                              OpStore %48 %67 
                       Uniform f32_4* %73 = OpAccessChain %20 %23 %44 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 3 3 3 3 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %48 
                                f32_4 %79 = OpFAdd %77 %78 
                        Output f32_4* %81 = OpAccessChain %72 %22 
                                              OpStore %81 %79 
                                f32_4 %84 = OpLoad %83 
                                              OpStore %82 %84 
                          Output f32* %86 = OpAccessChain %72 %22 %68 
                                  f32 %87 = OpLoad %86 
                                  f32 %88 = OpFNegate %87 
                          Output f32* %89 = OpAccessChain %72 %22 %68 
                                              OpStore %89 %88 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
uniform 	int unity_StereoEyeIndex;
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
int op_shl(int a, int b) { return int(floor(float(a) * pow(2.0, float(b)))); }
ivec2 op_shl(ivec2 a, ivec2 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); return a; }
ivec3 op_shl(ivec3 a, ivec3 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); return a; }
ivec4 op_shl(ivec4 a, ivec4 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); a.w = op_shl(a.w, b.w); return a; }

void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = op_shl(unity_StereoEyeIndex, 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
#endif
	UNITY_UNIFORM int unity_StereoEyeIndex;
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(unity_StereoEyeIndex << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
};

struct UnityStereoGlobals_Type
{
    float4 hlslcc_mtx4x4unity_StereoMatrixP[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
    float4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
    float4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
    float4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
    float4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
    float3 unity_StereoWorldSpaceCameraPos[2];
    float4 unity_StereoScaleOffset[2];
};

struct UnityStereoEyeIndex_Type
{
    int unity_StereoEyeIndex;
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    constant UnityStereoGlobals_Type& UnityStereoGlobals [[ buffer(1) ]],
    constant UnityStereoEyeIndex_Type& UnityStereoEyeIndex [[ buffer(2) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    int u_xlati1;
    float4 u_xlat2;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = UnityStereoEyeIndex.unity_StereoEyeIndex << 0x2;
    u_xlat2 = u_xlat0.yyyy * UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1], u_xlat0.xxxx, u_xlat2);
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)], u_xlat0.zzzz, u_xlat2);
    output.mtl_Position = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)], u_xlat0.wwww, u_xlat2);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
	int unity_StereoEyeIndex;
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = unity_StereoEyeIndex << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 105
; Schema: 0
                                                   OpCapability Shader 
                                            %1 = OpExtInstImport "GLSL.std.450" 
                                                   OpMemoryModel Logical GLSL450 
                                                   OpEntryPoint Vertex %4 "main" %11 %84 %96 %97 
                                                   OpDecorate %11 Location 11 
                                                   OpDecorate %16 ArrayStride 16 
                                                   OpDecorate %18 ArrayStride 18 
                                                   OpMemberDecorate %20 0 Offset 20 
                                                   OpMemberDecorate %20 1 Offset 20 
                                                   OpMemberDecorate %20 2 Offset 20 
                                                   OpDecorate %20 Block 
                                                   OpDecorate %22 DescriptorSet 22 
                                                   OpDecorate %22 Binding 22 
                                                   OpMemberDecorate %82 0 BuiltIn 82 
                                                   OpMemberDecorate %82 1 BuiltIn 82 
                                                   OpMemberDecorate %82 2 BuiltIn 82 
                                                   OpDecorate %82 Block 
                                                   OpDecorate %96 Location 96 
                                                   OpDecorate %97 Location 97 
                                            %2 = OpTypeVoid 
                                            %3 = OpTypeFunction %2 
                                            %6 = OpTypeFloat 32 
                                            %7 = OpTypeVector %6 4 
                                            %8 = OpTypePointer Private %7 
                             Private f32_4* %9 = OpVariable Private 
                                           %10 = OpTypePointer Input %7 
                              Input f32_4* %11 = OpVariable Input 
                                           %14 = OpTypeInt 32 0 
                                       u32 %15 = OpConstant 4 
                                           %16 = OpTypeArray %7 %15 
                                       u32 %17 = OpConstant 8 
                                           %18 = OpTypeArray %7 %17 
                                           %19 = OpTypeInt 32 1 
                                           %20 = OpTypeStruct %16 %18 %19 
                                           %21 = OpTypePointer Uniform %20 
Uniform struct {f32_4[4]; f32_4[8]; i32;}* %22 = OpVariable Uniform 
                                       i32 %23 = OpConstant 0 
                                       i32 %24 = OpConstant 1 
                                           %25 = OpTypePointer Uniform %7 
                                       i32 %36 = OpConstant 2 
                                       i32 %45 = OpConstant 3 
                                           %49 = OpTypePointer Private %19 
                              Private i32* %50 = OpVariable Private 
                                           %51 = OpTypePointer Uniform %19 
                            Private f32_4* %55 = OpVariable Private 
                                       u32 %80 = OpConstant 1 
                                           %81 = OpTypeArray %6 %80 
                                           %82 = OpTypeStruct %7 %6 %81 
                                           %83 = OpTypePointer Output %82 
      Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                           %94 = OpTypePointer Output %7 
                             Output f32_4* %96 = OpVariable Output 
                              Input f32_4* %97 = OpVariable Input 
                                           %99 = OpTypePointer Output %6 
                                       void %4 = OpFunction None %3 
                                            %5 = OpLabel 
                                     f32_4 %12 = OpLoad %11 
                                     f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                            Uniform f32_4* %26 = OpAccessChain %22 %23 %24 
                                     f32_4 %27 = OpLoad %26 
                                     f32_4 %28 = OpFMul %13 %27 
                                                   OpStore %9 %28 
                            Uniform f32_4* %29 = OpAccessChain %22 %23 %23 
                                     f32_4 %30 = OpLoad %29 
                                     f32_4 %31 = OpLoad %11 
                                     f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                     f32_4 %33 = OpFMul %30 %32 
                                     f32_4 %34 = OpLoad %9 
                                     f32_4 %35 = OpFAdd %33 %34 
                                                   OpStore %9 %35 
                            Uniform f32_4* %37 = OpAccessChain %22 %23 %36 
                                     f32_4 %38 = OpLoad %37 
                                     f32_4 %39 = OpLoad %11 
                                     f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                     f32_4 %41 = OpFMul %38 %40 
                                     f32_4 %42 = OpLoad %9 
                                     f32_4 %43 = OpFAdd %41 %42 
                                                   OpStore %9 %43 
                                     f32_4 %44 = OpLoad %9 
                            Uniform f32_4* %46 = OpAccessChain %22 %23 %45 
                                     f32_4 %47 = OpLoad %46 
                                     f32_4 %48 = OpFAdd %44 %47 
                                                   OpStore %9 %48 
                              Uniform i32* %52 = OpAccessChain %22 %36 
                                       i32 %53 = OpLoad %52 
                                       i32 %54 = OpShiftLeftLogical %53 %36 
                                                   OpStore %50 %54 
                                     f32_4 %56 = OpLoad %9 
                                     f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                       i32 %58 = OpLoad %50 
                                       i32 %59 = OpIAdd %58 %24 
                            Uniform f32_4* %60 = OpAccessChain %22 %24 %59 
                                     f32_4 %61 = OpLoad %60 
                                     f32_4 %62 = OpFMul %57 %61 
                                                   OpStore %55 %62 
                                       i32 %63 = OpLoad %50 
                            Uniform f32_4* %64 = OpAccessChain %22 %24 %63 
                                     f32_4 %65 = OpLoad %64 
                                     f32_4 %66 = OpLoad %9 
                                     f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                     f32_4 %68 = OpFMul %65 %67 
                                     f32_4 %69 = OpLoad %55 
                                     f32_4 %70 = OpFAdd %68 %69 
                                                   OpStore %55 %70 
                                       i32 %71 = OpLoad %50 
                                       i32 %72 = OpIAdd %71 %36 
                            Uniform f32_4* %73 = OpAccessChain %22 %24 %72 
                                     f32_4 %74 = OpLoad %73 
                                     f32_4 %75 = OpLoad %9 
                                     f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                     f32_4 %77 = OpFMul %74 %76 
                                     f32_4 %78 = OpLoad %55 
                                     f32_4 %79 = OpFAdd %77 %78 
                                                   OpStore %55 %79 
                                       i32 %85 = OpLoad %50 
                                       i32 %86 = OpIAdd %85 %45 
                            Uniform f32_4* %87 = OpAccessChain %22 %24 %86 
                                     f32_4 %88 = OpLoad %87 
                                     f32_4 %89 = OpLoad %9 
                                     f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                     f32_4 %91 = OpFMul %88 %90 
                                     f32_4 %92 = OpLoad %55 
                                     f32_4 %93 = OpFAdd %91 %92 
                             Output f32_4* %95 = OpAccessChain %84 %23 
                                                   OpStore %95 %93 
                                     f32_4 %98 = OpLoad %97 
                                                   OpStore %96 %98 
                              Output f32* %100 = OpAccessChain %84 %23 %80 
                                      f32 %101 = OpLoad %100 
                                      f32 %102 = OpFNegate %101 
                              Output f32* %103 = OpAccessChain %84 %23 %80 
                                                   OpStore %103 %102 
                                                   OpReturn
                                                   OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
flat out highp uint vs_BLENDINDICES0;
float u_xlat0;
uint u_xlatu0;
vec4 u_xlat1;
vec4 u_xlat2;
int u_xlati3;
void main()
{
    u_xlat0 = float(uint(gl_InstanceID));
    u_xlat0 = u_xlat0 * 0.5;
    u_xlat0 = fract(u_xlat0);
    u_xlat0 = u_xlat0 + u_xlat0;
    u_xlat0 = roundEven(u_xlat0);
    u_xlatu0 = uint(u_xlat0);
    u_xlati3 = int(int(u_xlatu0) << 2);
    u_xlat1 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
    u_xlat1 = u_xlat1 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat2 = u_xlat1.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati3] * u_xlat1.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 2)] * u_xlat1.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 3)] * u_xlat1.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu0;
    gl_Layer = int(u_xlatu0);
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
flat out uint vs_BLENDINDICES0;
vec4 u_xlat0;
uint u_xlatu1;
vec4 u_xlat2;
int u_xlati4;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlatu1 = uint(gl_InstanceID) & 1u;
    u_xlati4 = int(u_xlatu1) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati4] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu1;
    gl_Layer = int(u_xlatu1);
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 111
; Schema: 0
                                              OpCapability Shader 
                                              OpCapability Geometry 
                                              OpCapability ShaderViewportIndexLayerEXT 
                                              OpExtension "SPV_EXT_shader_viewport_index_layer" 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %52 %84 %96 %97 %100 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %18 ArrayStride 18 
                                              OpMemberDecorate %19 0 Offset 19 
                                              OpMemberDecorate %19 1 Offset 19 
                                              OpDecorate %19 Block 
                                              OpDecorate %21 DescriptorSet 21 
                                              OpDecorate %21 Binding 21 
                                              OpDecorate %52 BuiltIn 52 
                                              OpMemberDecorate %82 0 BuiltIn 82 
                                              OpMemberDecorate %82 1 BuiltIn 82 
                                              OpMemberDecorate %82 2 BuiltIn 82 
                                              OpDecorate %82 Block 
                                              OpDecorate %96 Location 96 
                                              OpDecorate %97 Location 97 
                                              OpDecorate %100 BuiltIn 100 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                  u32 %17 = OpConstant 8 
                                      %18 = OpTypeArray %7 %17 
                                      %19 = OpTypeStruct %16 %18 
                                      %20 = OpTypePointer Uniform %19 
Uniform struct {f32_4[4]; f32_4[8];}* %21 = OpVariable Uniform 
                                      %22 = OpTypeInt 32 1 
                                  i32 %23 = OpConstant 0 
                                  i32 %24 = OpConstant 1 
                                      %25 = OpTypePointer Uniform %7 
                                  i32 %36 = OpConstant 2 
                                  i32 %45 = OpConstant 3 
                                      %49 = OpTypePointer Private %22 
                         Private i32* %50 = OpVariable Private 
                                      %51 = OpTypePointer Input %22 
                           Input i32* %52 = OpVariable Input 
                       Private f32_4* %55 = OpVariable Private 
                                  u32 %80 = OpConstant 1 
                                      %81 = OpTypeArray %6 %80 
                                      %82 = OpTypeStruct %7 %6 %81 
                                      %83 = OpTypePointer Output %82 
 Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                      %94 = OpTypePointer Output %7 
                        Output f32_4* %96 = OpVariable Output 
                         Input f32_4* %97 = OpVariable Input 
                                      %99 = OpTypePointer Output %22 
                         Output i32* %100 = OpVariable Output 
                                     %105 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %26 = OpAccessChain %21 %23 %24 
                                f32_4 %27 = OpLoad %26 
                                f32_4 %28 = OpFMul %13 %27 
                                              OpStore %9 %28 
                       Uniform f32_4* %29 = OpAccessChain %21 %23 %23 
                                f32_4 %30 = OpLoad %29 
                                f32_4 %31 = OpLoad %11 
                                f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                f32_4 %33 = OpFMul %30 %32 
                                f32_4 %34 = OpLoad %9 
                                f32_4 %35 = OpFAdd %33 %34 
                                              OpStore %9 %35 
                       Uniform f32_4* %37 = OpAccessChain %21 %23 %36 
                                f32_4 %38 = OpLoad %37 
                                f32_4 %39 = OpLoad %11 
                                f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                f32_4 %41 = OpFMul %38 %40 
                                f32_4 %42 = OpLoad %9 
                                f32_4 %43 = OpFAdd %41 %42 
                                              OpStore %9 %43 
                                f32_4 %44 = OpLoad %9 
                       Uniform f32_4* %46 = OpAccessChain %21 %23 %45 
                                f32_4 %47 = OpLoad %46 
                                f32_4 %48 = OpFAdd %44 %47 
                                              OpStore %9 %48 
                                  i32 %53 = OpLoad %52 
                                  i32 %54 = OpBitFieldInsert %23 %53 %36 %24 
                                              OpStore %50 %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                  i32 %58 = OpLoad %50 
                                  i32 %59 = OpIAdd %58 %24 
                       Uniform f32_4* %60 = OpAccessChain %21 %24 %59 
                                f32_4 %61 = OpLoad %60 
                                f32_4 %62 = OpFMul %57 %61 
                                              OpStore %55 %62 
                                  i32 %63 = OpLoad %50 
                       Uniform f32_4* %64 = OpAccessChain %21 %24 %63 
                                f32_4 %65 = OpLoad %64 
                                f32_4 %66 = OpLoad %9 
                                f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                f32_4 %68 = OpFMul %65 %67 
                                f32_4 %69 = OpLoad %55 
                                f32_4 %70 = OpFAdd %68 %69 
                                              OpStore %55 %70 
                                  i32 %71 = OpLoad %50 
                                  i32 %72 = OpIAdd %71 %36 
                       Uniform f32_4* %73 = OpAccessChain %21 %24 %72 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %55 
                                f32_4 %79 = OpFAdd %77 %78 
                                              OpStore %55 %79 
                                  i32 %85 = OpLoad %50 
                                  i32 %86 = OpIAdd %85 %45 
                       Uniform f32_4* %87 = OpAccessChain %21 %24 %86 
                                f32_4 %88 = OpLoad %87 
                                f32_4 %89 = OpLoad %9 
                                f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                f32_4 %91 = OpFMul %88 %90 
                                f32_4 %92 = OpLoad %55 
                                f32_4 %93 = OpFAdd %91 %92 
                        Output f32_4* %95 = OpAccessChain %84 %23 
                                              OpStore %95 %93 
                                f32_4 %98 = OpLoad %97 
                                              OpStore %96 %98 
                                 i32 %101 = OpLoad %52 
                                 u32 %102 = OpBitcast %101 
                                 u32 %103 = OpBitwiseAnd %102 %80 
                                 i32 %104 = OpBitcast %103 
                                              OpStore %100 %104 
                         Output f32* %106 = OpAccessChain %84 %23 %80 
                                 f32 %107 = OpLoad %106 
                                 f32 %108 = OpFNegate %107 
                         Output f32* %109 = OpAccessChain %84 %23 %80 
                                              OpStore %109 %108 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
#endif
	UNITY_UNIFORM vec4 unity_StereoEyeIndices[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
layout(num_views = 2) in;
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
out highp float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(int(gl_ViewID_OVR) << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
	vec4 unity_StereoEyeIndices[2];
};
layout(num_views = 2) in;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
out float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(gl_ViewID_OVR) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 134
; Schema: 0
                                                      OpCapability Shader 
                                                      OpCapability MultiView 
                                                      OpExtension "SPV_KHR_multiview" 
                                               %1 = OpExtInstImport "GLSL.std.450" 
                                                      OpMemoryModel Logical GLSL450 
                                                      OpEntryPoint Vertex %4 "main" %11 %50 %99 %111 %112 %118 
                                                      OpDecorate %11 Location 11 
                                                      OpDecorate %16 ArrayStride 16 
                                                      OpMemberDecorate %17 0 Offset 17 
                                                      OpDecorate %17 Block 
                                                      OpDecorate %19 DescriptorSet 19 
                                                      OpDecorate %19 Binding 19 
                                                      OpDecorate %50 BuiltIn 50 
                                                      OpDecorate %58 ArrayStride 58 
                                                      OpDecorate %59 ArrayStride 59 
                                                      OpDecorate %60 ArrayStride 60 
                                                      OpDecorate %61 ArrayStride 61 
                                                      OpDecorate %62 ArrayStride 62 
                                                      OpDecorate %63 ArrayStride 63 
                                                      OpDecorate %64 ArrayStride 64 
                                                      OpDecorate %65 ArrayStride 65 
                                                      OpDecorate %68 ArrayStride 68 
                                                      OpDecorate %69 ArrayStride 69 
                                                      OpMemberDecorate %70 0 Offset 70 
                                                      OpMemberDecorate %70 1 Offset 70 
                                                      OpMemberDecorate %70 2 Offset 70 
                                                      OpMemberDecorate %70 3 Offset 70 
                                                      OpMemberDecorate %70 4 Offset 70 
                                                      OpMemberDecorate %70 5 Offset 70 
                                                      OpMemberDecorate %70 6 Offset 70 
                                                      OpMemberDecorate %70 7 Offset 70 
                                                      OpMemberDecorate %70 8 Offset 70 
                                                      OpMemberDecorate %70 9 Offset 70 
                                                      OpDecorate %70 Block 
                                                      OpDecorate %72 DescriptorSet 72 
                                                      OpDecorate %72 Binding 72 
                                                      OpMemberDecorate %97 0 BuiltIn 97 
                                                      OpMemberDecorate %97 1 BuiltIn 97 
                                                      OpMemberDecorate %97 2 BuiltIn 97 
                                                      OpDecorate %97 Block 
                                                      OpDecorate %111 Location 111 
                                                      OpDecorate %112 Location 112 
                                                      OpDecorate %118 Location 118 
                                                      OpDecorate %119 ArrayStride 119 
                                                      OpMemberDecorate %120 0 Offset 120 
                                                      OpDecorate %120 Block 
                                                      OpDecorate %122 DescriptorSet 122 
                                                      OpDecorate %122 Binding 122 
                                               %2 = OpTypeVoid 
                                               %3 = OpTypeFunction %2 
                                               %6 = OpTypeFloat 32 
                                               %7 = OpTypeVector %6 4 
                                               %8 = OpTypePointer Private %7 
                                Private f32_4* %9 = OpVariable Private 
                                              %10 = OpTypePointer Input %7 
                                 Input f32_4* %11 = OpVariable Input 
                                              %14 = OpTypeInt 32 0 
                                          u32 %15 = OpConstant 4 
                                              %16 = OpTypeArray %7 %15 
                                              %17 = OpTypeStruct %16 
                                              %18 = OpTypePointer Uniform %17 
                  Uniform struct {f32_4[4];}* %19 = OpVariable Uniform 
                                              %20 = OpTypeInt 32 1 
                                          i32 %21 = OpConstant 0 
                                          i32 %22 = OpConstant 1 
                                              %23 = OpTypePointer Uniform %7 
                                          i32 %34 = OpConstant 2 
                                          i32 %43 = OpConstant 3 
                                              %47 = OpTypePointer Private %20 
                                 Private i32* %48 = OpVariable Private 
                                              %49 = OpTypePointer Input %14 
                                   Input u32* %50 = OpVariable Input 
                               Private f32_4* %54 = OpVariable Private 
                                          u32 %57 = OpConstant 8 
                                              %58 = OpTypeArray %7 %57 
                                              %59 = OpTypeArray %7 %57 
                                              %60 = OpTypeArray %7 %57 
                                              %61 = OpTypeArray %7 %57 
                                              %62 = OpTypeArray %7 %57 
                                              %63 = OpTypeArray %7 %57 
                                              %64 = OpTypeArray %7 %57 
                                              %65 = OpTypeArray %7 %57 
                                              %66 = OpTypeVector %6 3 
                                          u32 %67 = OpConstant 2 
                                              %68 = OpTypeArray %66 %67 
                                              %69 = OpTypeArray %7 %67 
                                              %70 = OpTypeStruct %58 %59 %60 %61 %62 %63 %64 %65 %68 %69 
                                              %71 = OpTypePointer Uniform %70 
Uniform struct {f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_3[2]; f32_4[2];}* %72 = OpVariable Uniform 
                                          u32 %95 = OpConstant 1 
                                              %96 = OpTypeArray %6 %95 
                                              %97 = OpTypeStruct %7 %6 %96 
                                              %98 = OpTypePointer Output %97 
         Output struct {f32_4; f32; f32[1];}* %99 = OpVariable Output 
                                             %109 = OpTypePointer Output %7 
                               Output f32_4* %111 = OpVariable Output 
                                Input f32_4* %112 = OpVariable Input 
                                             %114 = OpTypePointer Private %14 
                                Private u32* %115 = OpVariable Private 
                                             %117 = OpTypePointer Output %6 
                                 Output f32* %118 = OpVariable Output 
                                             %119 = OpTypeArray %7 %67 
                                             %120 = OpTypeStruct %119 
                                             %121 = OpTypePointer Uniform %120 
                 Uniform struct {f32_4[2];}* %122 = OpVariable Uniform 
                                         u32 %125 = OpConstant 0 
                                             %126 = OpTypePointer Uniform %6 
                                          void %4 = OpFunction None %3 
                                               %5 = OpLabel 
                                        f32_4 %12 = OpLoad %11 
                                        f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                               Uniform f32_4* %24 = OpAccessChain %19 %21 %22 
                                        f32_4 %25 = OpLoad %24 
                                        f32_4 %26 = OpFMul %13 %25 
                                                      OpStore %9 %26 
                               Uniform f32_4* %27 = OpAccessChain %19 %21 %21 
                                        f32_4 %28 = OpLoad %27 
                                        f32_4 %29 = OpLoad %11 
                                        f32_4 %30 = OpVectorShuffle %29 %29 0 0 0 0 
                                        f32_4 %31 = OpFMul %28 %30 
                                        f32_4 %32 = OpLoad %9 
                                        f32_4 %33 = OpFAdd %31 %32 
                                                      OpStore %9 %33 
                               Uniform f32_4* %35 = OpAccessChain %19 %21 %34 
                                        f32_4 %36 = OpLoad %35 
                                        f32_4 %37 = OpLoad %11 
                                        f32_4 %38 = OpVectorShuffle %37 %37 2 2 2 2 
                                        f32_4 %39 = OpFMul %36 %38 
                                        f32_4 %40 = OpLoad %9 
                                        f32_4 %41 = OpFAdd %39 %40 
                                                      OpStore %9 %41 
                                        f32_4 %42 = OpLoad %9 
                               Uniform f32_4* %44 = OpAccessChain %19 %21 %43 
                                        f32_4 %45 = OpLoad %44 
                                        f32_4 %46 = OpFAdd %42 %45 
                                                      OpStore %9 %46 
                                          u32 %51 = OpLoad %50 
                                          i32 %52 = OpBitcast %51 
                                          i32 %53 = OpShiftLeftLogical %52 %34 
                                                      OpStore %48 %53 
                                        f32_4 %55 = OpLoad %9 
                                        f32_4 %56 = OpVectorShuffle %55 %55 1 1 1 1 
                                          i32 %73 = OpLoad %48 
                                          i32 %74 = OpIAdd %73 %22 
                               Uniform f32_4* %75 = OpAccessChain %72 %43 %74 
                                        f32_4 %76 = OpLoad %75 
                                        f32_4 %77 = OpFMul %56 %76 
                                                      OpStore %54 %77 
                                          i32 %78 = OpLoad %48 
                               Uniform f32_4* %79 = OpAccessChain %72 %43 %78 
                                        f32_4 %80 = OpLoad %79 
                                        f32_4 %81 = OpLoad %9 
                                        f32_4 %82 = OpVectorShuffle %81 %81 0 0 0 0 
                                        f32_4 %83 = OpFMul %80 %82 
                                        f32_4 %84 = OpLoad %54 
                                        f32_4 %85 = OpFAdd %83 %84 
                                                      OpStore %54 %85 
                                          i32 %86 = OpLoad %48 
                                          i32 %87 = OpIAdd %86 %34 
                               Uniform f32_4* %88 = OpAccessChain %72 %43 %87 
                                        f32_4 %89 = OpLoad %88 
                                        f32_4 %90 = OpLoad %9 
                                        f32_4 %91 = OpVectorShuffle %90 %90 2 2 2 2 
                                        f32_4 %92 = OpFMul %89 %91 
                                        f32_4 %93 = OpLoad %54 
                                        f32_4 %94 = OpFAdd %92 %93 
                                                      OpStore %54 %94 
                                         i32 %100 = OpLoad %48 
                                         i32 %101 = OpIAdd %100 %43 
                              Uniform f32_4* %102 = OpAccessChain %72 %43 %101 
                                       f32_4 %103 = OpLoad %102 
                                       f32_4 %104 = OpLoad %9 
                                       f32_4 %105 = OpVectorShuffle %104 %104 3 3 3 3 
                                       f32_4 %106 = OpFMul %103 %105 
                                       f32_4 %107 = OpLoad %54 
                                       f32_4 %108 = OpFAdd %106 %107 
                               Output f32_4* %110 = OpAccessChain %99 %21 
                                                      OpStore %110 %108 
                                       f32_4 %113 = OpLoad %112 
                                                      OpStore %111 %113 
                                         u32 %116 = OpLoad %50 
                                                      OpStore %115 %116 
                                         u32 %123 = OpLoad %115 
                                         i32 %124 = OpBitcast %123 
                                Uniform f32* %127 = OpAccessChain %122 %21 %124 %125 
                                         f32 %128 = OpLoad %127 
                                                      OpStore %118 %128 
                                 Output f32* %129 = OpAccessChain %99 %21 %95 
                                         f32 %130 = OpLoad %129 
                                         f32 %131 = OpFNegate %130 
                                 Output f32* %132 = OpAccessChain %99 %21 %95 
                                                      OpStore %132 %131 
                                                      OpReturn
                                                      OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
""
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
""
}
SubProgram "vulkan " {
""
}
SubProgram "gles " {
""
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
}
}
 Pass {
  ZTest Always
  Cull Off
  GpuProgramID 202167
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 91
; Schema: 0
                                              OpCapability Shader 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %72 %82 %83 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %17 ArrayStride 17 
                                              OpMemberDecorate %18 0 Offset 18 
                                              OpMemberDecorate %18 1 Offset 18 
                                              OpDecorate %18 Block 
                                              OpDecorate %20 DescriptorSet 20 
                                              OpDecorate %20 Binding 20 
                                              OpMemberDecorate %70 0 BuiltIn 70 
                                              OpMemberDecorate %70 1 BuiltIn 70 
                                              OpMemberDecorate %70 2 BuiltIn 70 
                                              OpDecorate %70 Block 
                                              OpDecorate %82 Location 82 
                                              OpDecorate %83 Location 83 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                      %17 = OpTypeArray %7 %15 
                                      %18 = OpTypeStruct %16 %17 
                                      %19 = OpTypePointer Uniform %18 
Uniform struct {f32_4[4]; f32_4[4];}* %20 = OpVariable Uniform 
                                      %21 = OpTypeInt 32 1 
                                  i32 %22 = OpConstant 0 
                                  i32 %23 = OpConstant 1 
                                      %24 = OpTypePointer Uniform %7 
                                  i32 %35 = OpConstant 2 
                                  i32 %44 = OpConstant 3 
                       Private f32_4* %48 = OpVariable Private 
                                  u32 %68 = OpConstant 1 
                                      %69 = OpTypeArray %6 %68 
                                      %70 = OpTypeStruct %7 %6 %69 
                                      %71 = OpTypePointer Output %70 
 Output struct {f32_4; f32; f32[1];}* %72 = OpVariable Output 
                                      %80 = OpTypePointer Output %7 
                        Output f32_4* %82 = OpVariable Output 
                         Input f32_4* %83 = OpVariable Input 
                                      %85 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %25 = OpAccessChain %20 %22 %23 
                                f32_4 %26 = OpLoad %25 
                                f32_4 %27 = OpFMul %13 %26 
                                              OpStore %9 %27 
                       Uniform f32_4* %28 = OpAccessChain %20 %22 %22 
                                f32_4 %29 = OpLoad %28 
                                f32_4 %30 = OpLoad %11 
                                f32_4 %31 = OpVectorShuffle %30 %30 0 0 0 0 
                                f32_4 %32 = OpFMul %29 %31 
                                f32_4 %33 = OpLoad %9 
                                f32_4 %34 = OpFAdd %32 %33 
                                              OpStore %9 %34 
                       Uniform f32_4* %36 = OpAccessChain %20 %22 %35 
                                f32_4 %37 = OpLoad %36 
                                f32_4 %38 = OpLoad %11 
                                f32_4 %39 = OpVectorShuffle %38 %38 2 2 2 2 
                                f32_4 %40 = OpFMul %37 %39 
                                f32_4 %41 = OpLoad %9 
                                f32_4 %42 = OpFAdd %40 %41 
                                              OpStore %9 %42 
                                f32_4 %43 = OpLoad %9 
                       Uniform f32_4* %45 = OpAccessChain %20 %22 %44 
                                f32_4 %46 = OpLoad %45 
                                f32_4 %47 = OpFAdd %43 %46 
                                              OpStore %9 %47 
                                f32_4 %49 = OpLoad %9 
                                f32_4 %50 = OpVectorShuffle %49 %49 1 1 1 1 
                       Uniform f32_4* %51 = OpAccessChain %20 %23 %23 
                                f32_4 %52 = OpLoad %51 
                                f32_4 %53 = OpFMul %50 %52 
                                              OpStore %48 %53 
                       Uniform f32_4* %54 = OpAccessChain %20 %23 %22 
                                f32_4 %55 = OpLoad %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 0 0 0 0 
                                f32_4 %58 = OpFMul %55 %57 
                                f32_4 %59 = OpLoad %48 
                                f32_4 %60 = OpFAdd %58 %59 
                                              OpStore %48 %60 
                       Uniform f32_4* %61 = OpAccessChain %20 %23 %35 
                                f32_4 %62 = OpLoad %61 
                                f32_4 %63 = OpLoad %9 
                                f32_4 %64 = OpVectorShuffle %63 %63 2 2 2 2 
                                f32_4 %65 = OpFMul %62 %64 
                                f32_4 %66 = OpLoad %48 
                                f32_4 %67 = OpFAdd %65 %66 
                                              OpStore %48 %67 
                       Uniform f32_4* %73 = OpAccessChain %20 %23 %44 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 3 3 3 3 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %48 
                                f32_4 %79 = OpFAdd %77 %78 
                        Output f32_4* %81 = OpAccessChain %72 %22 
                                              OpStore %81 %79 
                                f32_4 %84 = OpLoad %83 
                                              OpStore %82 %84 
                          Output f32* %86 = OpAccessChain %72 %22 %68 
                                  f32 %87 = OpLoad %86 
                                  f32 %88 = OpFNegate %87 
                          Output f32* %89 = OpAccessChain %72 %22 %68 
                                              OpStore %89 %88 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
uniform 	int unity_StereoEyeIndex;
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
int op_shl(int a, int b) { return int(floor(float(a) * pow(2.0, float(b)))); }
ivec2 op_shl(ivec2 a, ivec2 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); return a; }
ivec3 op_shl(ivec3 a, ivec3 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); return a; }
ivec4 op_shl(ivec4 a, ivec4 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); a.w = op_shl(a.w, b.w); return a; }

void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = op_shl(unity_StereoEyeIndex, 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
#endif
	UNITY_UNIFORM int unity_StereoEyeIndex;
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(unity_StereoEyeIndex << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
};

struct UnityStereoGlobals_Type
{
    float4 hlslcc_mtx4x4unity_StereoMatrixP[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
    float4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
    float4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
    float4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
    float4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
    float3 unity_StereoWorldSpaceCameraPos[2];
    float4 unity_StereoScaleOffset[2];
};

struct UnityStereoEyeIndex_Type
{
    int unity_StereoEyeIndex;
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    constant UnityStereoGlobals_Type& UnityStereoGlobals [[ buffer(1) ]],
    constant UnityStereoEyeIndex_Type& UnityStereoEyeIndex [[ buffer(2) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    int u_xlati1;
    float4 u_xlat2;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = UnityStereoEyeIndex.unity_StereoEyeIndex << 0x2;
    u_xlat2 = u_xlat0.yyyy * UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1], u_xlat0.xxxx, u_xlat2);
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)], u_xlat0.zzzz, u_xlat2);
    output.mtl_Position = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)], u_xlat0.wwww, u_xlat2);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
	int unity_StereoEyeIndex;
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = unity_StereoEyeIndex << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 105
; Schema: 0
                                                   OpCapability Shader 
                                            %1 = OpExtInstImport "GLSL.std.450" 
                                                   OpMemoryModel Logical GLSL450 
                                                   OpEntryPoint Vertex %4 "main" %11 %84 %96 %97 
                                                   OpDecorate %11 Location 11 
                                                   OpDecorate %16 ArrayStride 16 
                                                   OpDecorate %18 ArrayStride 18 
                                                   OpMemberDecorate %20 0 Offset 20 
                                                   OpMemberDecorate %20 1 Offset 20 
                                                   OpMemberDecorate %20 2 Offset 20 
                                                   OpDecorate %20 Block 
                                                   OpDecorate %22 DescriptorSet 22 
                                                   OpDecorate %22 Binding 22 
                                                   OpMemberDecorate %82 0 BuiltIn 82 
                                                   OpMemberDecorate %82 1 BuiltIn 82 
                                                   OpMemberDecorate %82 2 BuiltIn 82 
                                                   OpDecorate %82 Block 
                                                   OpDecorate %96 Location 96 
                                                   OpDecorate %97 Location 97 
                                            %2 = OpTypeVoid 
                                            %3 = OpTypeFunction %2 
                                            %6 = OpTypeFloat 32 
                                            %7 = OpTypeVector %6 4 
                                            %8 = OpTypePointer Private %7 
                             Private f32_4* %9 = OpVariable Private 
                                           %10 = OpTypePointer Input %7 
                              Input f32_4* %11 = OpVariable Input 
                                           %14 = OpTypeInt 32 0 
                                       u32 %15 = OpConstant 4 
                                           %16 = OpTypeArray %7 %15 
                                       u32 %17 = OpConstant 8 
                                           %18 = OpTypeArray %7 %17 
                                           %19 = OpTypeInt 32 1 
                                           %20 = OpTypeStruct %16 %18 %19 
                                           %21 = OpTypePointer Uniform %20 
Uniform struct {f32_4[4]; f32_4[8]; i32;}* %22 = OpVariable Uniform 
                                       i32 %23 = OpConstant 0 
                                       i32 %24 = OpConstant 1 
                                           %25 = OpTypePointer Uniform %7 
                                       i32 %36 = OpConstant 2 
                                       i32 %45 = OpConstant 3 
                                           %49 = OpTypePointer Private %19 
                              Private i32* %50 = OpVariable Private 
                                           %51 = OpTypePointer Uniform %19 
                            Private f32_4* %55 = OpVariable Private 
                                       u32 %80 = OpConstant 1 
                                           %81 = OpTypeArray %6 %80 
                                           %82 = OpTypeStruct %7 %6 %81 
                                           %83 = OpTypePointer Output %82 
      Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                           %94 = OpTypePointer Output %7 
                             Output f32_4* %96 = OpVariable Output 
                              Input f32_4* %97 = OpVariable Input 
                                           %99 = OpTypePointer Output %6 
                                       void %4 = OpFunction None %3 
                                            %5 = OpLabel 
                                     f32_4 %12 = OpLoad %11 
                                     f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                            Uniform f32_4* %26 = OpAccessChain %22 %23 %24 
                                     f32_4 %27 = OpLoad %26 
                                     f32_4 %28 = OpFMul %13 %27 
                                                   OpStore %9 %28 
                            Uniform f32_4* %29 = OpAccessChain %22 %23 %23 
                                     f32_4 %30 = OpLoad %29 
                                     f32_4 %31 = OpLoad %11 
                                     f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                     f32_4 %33 = OpFMul %30 %32 
                                     f32_4 %34 = OpLoad %9 
                                     f32_4 %35 = OpFAdd %33 %34 
                                                   OpStore %9 %35 
                            Uniform f32_4* %37 = OpAccessChain %22 %23 %36 
                                     f32_4 %38 = OpLoad %37 
                                     f32_4 %39 = OpLoad %11 
                                     f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                     f32_4 %41 = OpFMul %38 %40 
                                     f32_4 %42 = OpLoad %9 
                                     f32_4 %43 = OpFAdd %41 %42 
                                                   OpStore %9 %43 
                                     f32_4 %44 = OpLoad %9 
                            Uniform f32_4* %46 = OpAccessChain %22 %23 %45 
                                     f32_4 %47 = OpLoad %46 
                                     f32_4 %48 = OpFAdd %44 %47 
                                                   OpStore %9 %48 
                              Uniform i32* %52 = OpAccessChain %22 %36 
                                       i32 %53 = OpLoad %52 
                                       i32 %54 = OpShiftLeftLogical %53 %36 
                                                   OpStore %50 %54 
                                     f32_4 %56 = OpLoad %9 
                                     f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                       i32 %58 = OpLoad %50 
                                       i32 %59 = OpIAdd %58 %24 
                            Uniform f32_4* %60 = OpAccessChain %22 %24 %59 
                                     f32_4 %61 = OpLoad %60 
                                     f32_4 %62 = OpFMul %57 %61 
                                                   OpStore %55 %62 
                                       i32 %63 = OpLoad %50 
                            Uniform f32_4* %64 = OpAccessChain %22 %24 %63 
                                     f32_4 %65 = OpLoad %64 
                                     f32_4 %66 = OpLoad %9 
                                     f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                     f32_4 %68 = OpFMul %65 %67 
                                     f32_4 %69 = OpLoad %55 
                                     f32_4 %70 = OpFAdd %68 %69 
                                                   OpStore %55 %70 
                                       i32 %71 = OpLoad %50 
                                       i32 %72 = OpIAdd %71 %36 
                            Uniform f32_4* %73 = OpAccessChain %22 %24 %72 
                                     f32_4 %74 = OpLoad %73 
                                     f32_4 %75 = OpLoad %9 
                                     f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                     f32_4 %77 = OpFMul %74 %76 
                                     f32_4 %78 = OpLoad %55 
                                     f32_4 %79 = OpFAdd %77 %78 
                                                   OpStore %55 %79 
                                       i32 %85 = OpLoad %50 
                                       i32 %86 = OpIAdd %85 %45 
                            Uniform f32_4* %87 = OpAccessChain %22 %24 %86 
                                     f32_4 %88 = OpLoad %87 
                                     f32_4 %89 = OpLoad %9 
                                     f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                     f32_4 %91 = OpFMul %88 %90 
                                     f32_4 %92 = OpLoad %55 
                                     f32_4 %93 = OpFAdd %91 %92 
                             Output f32_4* %95 = OpAccessChain %84 %23 
                                                   OpStore %95 %93 
                                     f32_4 %98 = OpLoad %97 
                                                   OpStore %96 %98 
                              Output f32* %100 = OpAccessChain %84 %23 %80 
                                      f32 %101 = OpLoad %100 
                                      f32 %102 = OpFNegate %101 
                              Output f32* %103 = OpAccessChain %84 %23 %80 
                                                   OpStore %103 %102 
                                                   OpReturn
                                                   OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
flat out highp uint vs_BLENDINDICES0;
float u_xlat0;
uint u_xlatu0;
vec4 u_xlat1;
vec4 u_xlat2;
int u_xlati3;
void main()
{
    u_xlat0 = float(uint(gl_InstanceID));
    u_xlat0 = u_xlat0 * 0.5;
    u_xlat0 = fract(u_xlat0);
    u_xlat0 = u_xlat0 + u_xlat0;
    u_xlat0 = roundEven(u_xlat0);
    u_xlatu0 = uint(u_xlat0);
    u_xlati3 = int(int(u_xlatu0) << 2);
    u_xlat1 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
    u_xlat1 = u_xlat1 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat2 = u_xlat1.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati3] * u_xlat1.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 2)] * u_xlat1.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 3)] * u_xlat1.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu0;
    gl_Layer = int(u_xlatu0);
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
flat out uint vs_BLENDINDICES0;
vec4 u_xlat0;
uint u_xlatu1;
vec4 u_xlat2;
int u_xlati4;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlatu1 = uint(gl_InstanceID) & 1u;
    u_xlati4 = int(u_xlatu1) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati4] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu1;
    gl_Layer = int(u_xlatu1);
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 111
; Schema: 0
                                              OpCapability Shader 
                                              OpCapability Geometry 
                                              OpCapability ShaderViewportIndexLayerEXT 
                                              OpExtension "SPV_EXT_shader_viewport_index_layer" 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %52 %84 %96 %97 %100 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %18 ArrayStride 18 
                                              OpMemberDecorate %19 0 Offset 19 
                                              OpMemberDecorate %19 1 Offset 19 
                                              OpDecorate %19 Block 
                                              OpDecorate %21 DescriptorSet 21 
                                              OpDecorate %21 Binding 21 
                                              OpDecorate %52 BuiltIn 52 
                                              OpMemberDecorate %82 0 BuiltIn 82 
                                              OpMemberDecorate %82 1 BuiltIn 82 
                                              OpMemberDecorate %82 2 BuiltIn 82 
                                              OpDecorate %82 Block 
                                              OpDecorate %96 Location 96 
                                              OpDecorate %97 Location 97 
                                              OpDecorate %100 BuiltIn 100 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                  u32 %17 = OpConstant 8 
                                      %18 = OpTypeArray %7 %17 
                                      %19 = OpTypeStruct %16 %18 
                                      %20 = OpTypePointer Uniform %19 
Uniform struct {f32_4[4]; f32_4[8];}* %21 = OpVariable Uniform 
                                      %22 = OpTypeInt 32 1 
                                  i32 %23 = OpConstant 0 
                                  i32 %24 = OpConstant 1 
                                      %25 = OpTypePointer Uniform %7 
                                  i32 %36 = OpConstant 2 
                                  i32 %45 = OpConstant 3 
                                      %49 = OpTypePointer Private %22 
                         Private i32* %50 = OpVariable Private 
                                      %51 = OpTypePointer Input %22 
                           Input i32* %52 = OpVariable Input 
                       Private f32_4* %55 = OpVariable Private 
                                  u32 %80 = OpConstant 1 
                                      %81 = OpTypeArray %6 %80 
                                      %82 = OpTypeStruct %7 %6 %81 
                                      %83 = OpTypePointer Output %82 
 Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                      %94 = OpTypePointer Output %7 
                        Output f32_4* %96 = OpVariable Output 
                         Input f32_4* %97 = OpVariable Input 
                                      %99 = OpTypePointer Output %22 
                         Output i32* %100 = OpVariable Output 
                                     %105 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %26 = OpAccessChain %21 %23 %24 
                                f32_4 %27 = OpLoad %26 
                                f32_4 %28 = OpFMul %13 %27 
                                              OpStore %9 %28 
                       Uniform f32_4* %29 = OpAccessChain %21 %23 %23 
                                f32_4 %30 = OpLoad %29 
                                f32_4 %31 = OpLoad %11 
                                f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                f32_4 %33 = OpFMul %30 %32 
                                f32_4 %34 = OpLoad %9 
                                f32_4 %35 = OpFAdd %33 %34 
                                              OpStore %9 %35 
                       Uniform f32_4* %37 = OpAccessChain %21 %23 %36 
                                f32_4 %38 = OpLoad %37 
                                f32_4 %39 = OpLoad %11 
                                f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                f32_4 %41 = OpFMul %38 %40 
                                f32_4 %42 = OpLoad %9 
                                f32_4 %43 = OpFAdd %41 %42 
                                              OpStore %9 %43 
                                f32_4 %44 = OpLoad %9 
                       Uniform f32_4* %46 = OpAccessChain %21 %23 %45 
                                f32_4 %47 = OpLoad %46 
                                f32_4 %48 = OpFAdd %44 %47 
                                              OpStore %9 %48 
                                  i32 %53 = OpLoad %52 
                                  i32 %54 = OpBitFieldInsert %23 %53 %36 %24 
                                              OpStore %50 %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                  i32 %58 = OpLoad %50 
                                  i32 %59 = OpIAdd %58 %24 
                       Uniform f32_4* %60 = OpAccessChain %21 %24 %59 
                                f32_4 %61 = OpLoad %60 
                                f32_4 %62 = OpFMul %57 %61 
                                              OpStore %55 %62 
                                  i32 %63 = OpLoad %50 
                       Uniform f32_4* %64 = OpAccessChain %21 %24 %63 
                                f32_4 %65 = OpLoad %64 
                                f32_4 %66 = OpLoad %9 
                                f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                f32_4 %68 = OpFMul %65 %67 
                                f32_4 %69 = OpLoad %55 
                                f32_4 %70 = OpFAdd %68 %69 
                                              OpStore %55 %70 
                                  i32 %71 = OpLoad %50 
                                  i32 %72 = OpIAdd %71 %36 
                       Uniform f32_4* %73 = OpAccessChain %21 %24 %72 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %55 
                                f32_4 %79 = OpFAdd %77 %78 
                                              OpStore %55 %79 
                                  i32 %85 = OpLoad %50 
                                  i32 %86 = OpIAdd %85 %45 
                       Uniform f32_4* %87 = OpAccessChain %21 %24 %86 
                                f32_4 %88 = OpLoad %87 
                                f32_4 %89 = OpLoad %9 
                                f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                f32_4 %91 = OpFMul %88 %90 
                                f32_4 %92 = OpLoad %55 
                                f32_4 %93 = OpFAdd %91 %92 
                        Output f32_4* %95 = OpAccessChain %84 %23 
                                              OpStore %95 %93 
                                f32_4 %98 = OpLoad %97 
                                              OpStore %96 %98 
                                 i32 %101 = OpLoad %52 
                                 u32 %102 = OpBitcast %101 
                                 u32 %103 = OpBitwiseAnd %102 %80 
                                 i32 %104 = OpBitcast %103 
                                              OpStore %100 %104 
                         Output f32* %106 = OpAccessChain %84 %23 %80 
                                 f32 %107 = OpLoad %106 
                                 f32 %108 = OpFNegate %107 
                         Output f32* %109 = OpAccessChain %84 %23 %80 
                                              OpStore %109 %108 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
#endif
	UNITY_UNIFORM vec4 unity_StereoEyeIndices[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
layout(num_views = 2) in;
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
out highp float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(int(gl_ViewID_OVR) << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
	vec4 unity_StereoEyeIndices[2];
};
layout(num_views = 2) in;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
out float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(gl_ViewID_OVR) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 134
; Schema: 0
                                                      OpCapability Shader 
                                                      OpCapability MultiView 
                                                      OpExtension "SPV_KHR_multiview" 
                                               %1 = OpExtInstImport "GLSL.std.450" 
                                                      OpMemoryModel Logical GLSL450 
                                                      OpEntryPoint Vertex %4 "main" %11 %50 %99 %111 %112 %118 
                                                      OpDecorate %11 Location 11 
                                                      OpDecorate %16 ArrayStride 16 
                                                      OpMemberDecorate %17 0 Offset 17 
                                                      OpDecorate %17 Block 
                                                      OpDecorate %19 DescriptorSet 19 
                                                      OpDecorate %19 Binding 19 
                                                      OpDecorate %50 BuiltIn 50 
                                                      OpDecorate %58 ArrayStride 58 
                                                      OpDecorate %59 ArrayStride 59 
                                                      OpDecorate %60 ArrayStride 60 
                                                      OpDecorate %61 ArrayStride 61 
                                                      OpDecorate %62 ArrayStride 62 
                                                      OpDecorate %63 ArrayStride 63 
                                                      OpDecorate %64 ArrayStride 64 
                                                      OpDecorate %65 ArrayStride 65 
                                                      OpDecorate %68 ArrayStride 68 
                                                      OpDecorate %69 ArrayStride 69 
                                                      OpMemberDecorate %70 0 Offset 70 
                                                      OpMemberDecorate %70 1 Offset 70 
                                                      OpMemberDecorate %70 2 Offset 70 
                                                      OpMemberDecorate %70 3 Offset 70 
                                                      OpMemberDecorate %70 4 Offset 70 
                                                      OpMemberDecorate %70 5 Offset 70 
                                                      OpMemberDecorate %70 6 Offset 70 
                                                      OpMemberDecorate %70 7 Offset 70 
                                                      OpMemberDecorate %70 8 Offset 70 
                                                      OpMemberDecorate %70 9 Offset 70 
                                                      OpDecorate %70 Block 
                                                      OpDecorate %72 DescriptorSet 72 
                                                      OpDecorate %72 Binding 72 
                                                      OpMemberDecorate %97 0 BuiltIn 97 
                                                      OpMemberDecorate %97 1 BuiltIn 97 
                                                      OpMemberDecorate %97 2 BuiltIn 97 
                                                      OpDecorate %97 Block 
                                                      OpDecorate %111 Location 111 
                                                      OpDecorate %112 Location 112 
                                                      OpDecorate %118 Location 118 
                                                      OpDecorate %119 ArrayStride 119 
                                                      OpMemberDecorate %120 0 Offset 120 
                                                      OpDecorate %120 Block 
                                                      OpDecorate %122 DescriptorSet 122 
                                                      OpDecorate %122 Binding 122 
                                               %2 = OpTypeVoid 
                                               %3 = OpTypeFunction %2 
                                               %6 = OpTypeFloat 32 
                                               %7 = OpTypeVector %6 4 
                                               %8 = OpTypePointer Private %7 
                                Private f32_4* %9 = OpVariable Private 
                                              %10 = OpTypePointer Input %7 
                                 Input f32_4* %11 = OpVariable Input 
                                              %14 = OpTypeInt 32 0 
                                          u32 %15 = OpConstant 4 
                                              %16 = OpTypeArray %7 %15 
                                              %17 = OpTypeStruct %16 
                                              %18 = OpTypePointer Uniform %17 
                  Uniform struct {f32_4[4];}* %19 = OpVariable Uniform 
                                              %20 = OpTypeInt 32 1 
                                          i32 %21 = OpConstant 0 
                                          i32 %22 = OpConstant 1 
                                              %23 = OpTypePointer Uniform %7 
                                          i32 %34 = OpConstant 2 
                                          i32 %43 = OpConstant 3 
                                              %47 = OpTypePointer Private %20 
                                 Private i32* %48 = OpVariable Private 
                                              %49 = OpTypePointer Input %14 
                                   Input u32* %50 = OpVariable Input 
                               Private f32_4* %54 = OpVariable Private 
                                          u32 %57 = OpConstant 8 
                                              %58 = OpTypeArray %7 %57 
                                              %59 = OpTypeArray %7 %57 
                                              %60 = OpTypeArray %7 %57 
                                              %61 = OpTypeArray %7 %57 
                                              %62 = OpTypeArray %7 %57 
                                              %63 = OpTypeArray %7 %57 
                                              %64 = OpTypeArray %7 %57 
                                              %65 = OpTypeArray %7 %57 
                                              %66 = OpTypeVector %6 3 
                                          u32 %67 = OpConstant 2 
                                              %68 = OpTypeArray %66 %67 
                                              %69 = OpTypeArray %7 %67 
                                              %70 = OpTypeStruct %58 %59 %60 %61 %62 %63 %64 %65 %68 %69 
                                              %71 = OpTypePointer Uniform %70 
Uniform struct {f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_3[2]; f32_4[2];}* %72 = OpVariable Uniform 
                                          u32 %95 = OpConstant 1 
                                              %96 = OpTypeArray %6 %95 
                                              %97 = OpTypeStruct %7 %6 %96 
                                              %98 = OpTypePointer Output %97 
         Output struct {f32_4; f32; f32[1];}* %99 = OpVariable Output 
                                             %109 = OpTypePointer Output %7 
                               Output f32_4* %111 = OpVariable Output 
                                Input f32_4* %112 = OpVariable Input 
                                             %114 = OpTypePointer Private %14 
                                Private u32* %115 = OpVariable Private 
                                             %117 = OpTypePointer Output %6 
                                 Output f32* %118 = OpVariable Output 
                                             %119 = OpTypeArray %7 %67 
                                             %120 = OpTypeStruct %119 
                                             %121 = OpTypePointer Uniform %120 
                 Uniform struct {f32_4[2];}* %122 = OpVariable Uniform 
                                         u32 %125 = OpConstant 0 
                                             %126 = OpTypePointer Uniform %6 
                                          void %4 = OpFunction None %3 
                                               %5 = OpLabel 
                                        f32_4 %12 = OpLoad %11 
                                        f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                               Uniform f32_4* %24 = OpAccessChain %19 %21 %22 
                                        f32_4 %25 = OpLoad %24 
                                        f32_4 %26 = OpFMul %13 %25 
                                                      OpStore %9 %26 
                               Uniform f32_4* %27 = OpAccessChain %19 %21 %21 
                                        f32_4 %28 = OpLoad %27 
                                        f32_4 %29 = OpLoad %11 
                                        f32_4 %30 = OpVectorShuffle %29 %29 0 0 0 0 
                                        f32_4 %31 = OpFMul %28 %30 
                                        f32_4 %32 = OpLoad %9 
                                        f32_4 %33 = OpFAdd %31 %32 
                                                      OpStore %9 %33 
                               Uniform f32_4* %35 = OpAccessChain %19 %21 %34 
                                        f32_4 %36 = OpLoad %35 
                                        f32_4 %37 = OpLoad %11 
                                        f32_4 %38 = OpVectorShuffle %37 %37 2 2 2 2 
                                        f32_4 %39 = OpFMul %36 %38 
                                        f32_4 %40 = OpLoad %9 
                                        f32_4 %41 = OpFAdd %39 %40 
                                                      OpStore %9 %41 
                                        f32_4 %42 = OpLoad %9 
                               Uniform f32_4* %44 = OpAccessChain %19 %21 %43 
                                        f32_4 %45 = OpLoad %44 
                                        f32_4 %46 = OpFAdd %42 %45 
                                                      OpStore %9 %46 
                                          u32 %51 = OpLoad %50 
                                          i32 %52 = OpBitcast %51 
                                          i32 %53 = OpShiftLeftLogical %52 %34 
                                                      OpStore %48 %53 
                                        f32_4 %55 = OpLoad %9 
                                        f32_4 %56 = OpVectorShuffle %55 %55 1 1 1 1 
                                          i32 %73 = OpLoad %48 
                                          i32 %74 = OpIAdd %73 %22 
                               Uniform f32_4* %75 = OpAccessChain %72 %43 %74 
                                        f32_4 %76 = OpLoad %75 
                                        f32_4 %77 = OpFMul %56 %76 
                                                      OpStore %54 %77 
                                          i32 %78 = OpLoad %48 
                               Uniform f32_4* %79 = OpAccessChain %72 %43 %78 
                                        f32_4 %80 = OpLoad %79 
                                        f32_4 %81 = OpLoad %9 
                                        f32_4 %82 = OpVectorShuffle %81 %81 0 0 0 0 
                                        f32_4 %83 = OpFMul %80 %82 
                                        f32_4 %84 = OpLoad %54 
                                        f32_4 %85 = OpFAdd %83 %84 
                                                      OpStore %54 %85 
                                          i32 %86 = OpLoad %48 
                                          i32 %87 = OpIAdd %86 %34 
                               Uniform f32_4* %88 = OpAccessChain %72 %43 %87 
                                        f32_4 %89 = OpLoad %88 
                                        f32_4 %90 = OpLoad %9 
                                        f32_4 %91 = OpVectorShuffle %90 %90 2 2 2 2 
                                        f32_4 %92 = OpFMul %89 %91 
                                        f32_4 %93 = OpLoad %54 
                                        f32_4 %94 = OpFAdd %92 %93 
                                                      OpStore %54 %94 
                                         i32 %100 = OpLoad %48 
                                         i32 %101 = OpIAdd %100 %43 
                              Uniform f32_4* %102 = OpAccessChain %72 %43 %101 
                                       f32_4 %103 = OpLoad %102 
                                       f32_4 %104 = OpLoad %9 
                                       f32_4 %105 = OpVectorShuffle %104 %104 3 3 3 3 
                                       f32_4 %106 = OpFMul %103 %105 
                                       f32_4 %107 = OpLoad %54 
                                       f32_4 %108 = OpFAdd %106 %107 
                               Output f32_4* %110 = OpAccessChain %99 %21 
                                                      OpStore %110 %108 
                                       f32_4 %113 = OpLoad %112 
                                                      OpStore %111 %113 
                                         u32 %116 = OpLoad %50 
                                                      OpStore %115 %116 
                                         u32 %123 = OpLoad %115 
                                         i32 %124 = OpBitcast %123 
                                Uniform f32* %127 = OpAccessChain %122 %21 %124 %125 
                                         f32 %128 = OpLoad %127 
                                                      OpStore %118 %128 
                                 Output f32* %129 = OpAccessChain %99 %21 %95 
                                         f32 %130 = OpLoad %129 
                                         f32 %131 = OpFNegate %130 
                                 Output f32* %132 = OpAccessChain %99 %21 %95 
                                                      OpStore %132 %131 
                                                      OpReturn
                                                      OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
""
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
""
}
SubProgram "vulkan " {
""
}
SubProgram "gles " {
""
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
}
}
 Pass {
  ColorMask 0 0
  ZTest Always
  ZWrite Off
  Cull Off
  Stencil {
   Comp Always
   Pass Zero
   Fail Keep
   ZFail Keep
  }
  GpuProgramID 272253
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 91
; Schema: 0
                                              OpCapability Shader 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %72 %82 %83 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %17 ArrayStride 17 
                                              OpMemberDecorate %18 0 Offset 18 
                                              OpMemberDecorate %18 1 Offset 18 
                                              OpDecorate %18 Block 
                                              OpDecorate %20 DescriptorSet 20 
                                              OpDecorate %20 Binding 20 
                                              OpMemberDecorate %70 0 BuiltIn 70 
                                              OpMemberDecorate %70 1 BuiltIn 70 
                                              OpMemberDecorate %70 2 BuiltIn 70 
                                              OpDecorate %70 Block 
                                              OpDecorate %82 Location 82 
                                              OpDecorate %83 Location 83 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                      %17 = OpTypeArray %7 %15 
                                      %18 = OpTypeStruct %16 %17 
                                      %19 = OpTypePointer Uniform %18 
Uniform struct {f32_4[4]; f32_4[4];}* %20 = OpVariable Uniform 
                                      %21 = OpTypeInt 32 1 
                                  i32 %22 = OpConstant 0 
                                  i32 %23 = OpConstant 1 
                                      %24 = OpTypePointer Uniform %7 
                                  i32 %35 = OpConstant 2 
                                  i32 %44 = OpConstant 3 
                       Private f32_4* %48 = OpVariable Private 
                                  u32 %68 = OpConstant 1 
                                      %69 = OpTypeArray %6 %68 
                                      %70 = OpTypeStruct %7 %6 %69 
                                      %71 = OpTypePointer Output %70 
 Output struct {f32_4; f32; f32[1];}* %72 = OpVariable Output 
                                      %80 = OpTypePointer Output %7 
                        Output f32_4* %82 = OpVariable Output 
                         Input f32_4* %83 = OpVariable Input 
                                      %85 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %25 = OpAccessChain %20 %22 %23 
                                f32_4 %26 = OpLoad %25 
                                f32_4 %27 = OpFMul %13 %26 
                                              OpStore %9 %27 
                       Uniform f32_4* %28 = OpAccessChain %20 %22 %22 
                                f32_4 %29 = OpLoad %28 
                                f32_4 %30 = OpLoad %11 
                                f32_4 %31 = OpVectorShuffle %30 %30 0 0 0 0 
                                f32_4 %32 = OpFMul %29 %31 
                                f32_4 %33 = OpLoad %9 
                                f32_4 %34 = OpFAdd %32 %33 
                                              OpStore %9 %34 
                       Uniform f32_4* %36 = OpAccessChain %20 %22 %35 
                                f32_4 %37 = OpLoad %36 
                                f32_4 %38 = OpLoad %11 
                                f32_4 %39 = OpVectorShuffle %38 %38 2 2 2 2 
                                f32_4 %40 = OpFMul %37 %39 
                                f32_4 %41 = OpLoad %9 
                                f32_4 %42 = OpFAdd %40 %41 
                                              OpStore %9 %42 
                                f32_4 %43 = OpLoad %9 
                       Uniform f32_4* %45 = OpAccessChain %20 %22 %44 
                                f32_4 %46 = OpLoad %45 
                                f32_4 %47 = OpFAdd %43 %46 
                                              OpStore %9 %47 
                                f32_4 %49 = OpLoad %9 
                                f32_4 %50 = OpVectorShuffle %49 %49 1 1 1 1 
                       Uniform f32_4* %51 = OpAccessChain %20 %23 %23 
                                f32_4 %52 = OpLoad %51 
                                f32_4 %53 = OpFMul %50 %52 
                                              OpStore %48 %53 
                       Uniform f32_4* %54 = OpAccessChain %20 %23 %22 
                                f32_4 %55 = OpLoad %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 0 0 0 0 
                                f32_4 %58 = OpFMul %55 %57 
                                f32_4 %59 = OpLoad %48 
                                f32_4 %60 = OpFAdd %58 %59 
                                              OpStore %48 %60 
                       Uniform f32_4* %61 = OpAccessChain %20 %23 %35 
                                f32_4 %62 = OpLoad %61 
                                f32_4 %63 = OpLoad %9 
                                f32_4 %64 = OpVectorShuffle %63 %63 2 2 2 2 
                                f32_4 %65 = OpFMul %62 %64 
                                f32_4 %66 = OpLoad %48 
                                f32_4 %67 = OpFAdd %65 %66 
                                              OpStore %48 %67 
                       Uniform f32_4* %73 = OpAccessChain %20 %23 %44 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 3 3 3 3 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %48 
                                f32_4 %79 = OpFAdd %77 %78 
                        Output f32_4* %81 = OpAccessChain %72 %22 
                                              OpStore %81 %79 
                                f32_4 %84 = OpLoad %83 
                                              OpStore %82 %84 
                          Output f32* %86 = OpAccessChain %72 %22 %68 
                                  f32 %87 = OpLoad %86 
                                  f32 %88 = OpFNegate %87 
                          Output f32* %89 = OpAccessChain %72 %22 %68 
                                              OpStore %89 %88 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
uniform 	int unity_StereoEyeIndex;
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
int op_shl(int a, int b) { return int(floor(float(a) * pow(2.0, float(b)))); }
ivec2 op_shl(ivec2 a, ivec2 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); return a; }
ivec3 op_shl(ivec3 a, ivec3 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); return a; }
ivec4 op_shl(ivec4 a, ivec4 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); a.w = op_shl(a.w, b.w); return a; }

void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = op_shl(unity_StereoEyeIndex, 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
#endif
	UNITY_UNIFORM int unity_StereoEyeIndex;
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(unity_StereoEyeIndex << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
};

struct UnityStereoGlobals_Type
{
    float4 hlslcc_mtx4x4unity_StereoMatrixP[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
    float4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
    float4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
    float4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
    float4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
    float3 unity_StereoWorldSpaceCameraPos[2];
    float4 unity_StereoScaleOffset[2];
};

struct UnityStereoEyeIndex_Type
{
    int unity_StereoEyeIndex;
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    constant UnityStereoGlobals_Type& UnityStereoGlobals [[ buffer(1) ]],
    constant UnityStereoEyeIndex_Type& UnityStereoEyeIndex [[ buffer(2) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    int u_xlati1;
    float4 u_xlat2;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = UnityStereoEyeIndex.unity_StereoEyeIndex << 0x2;
    u_xlat2 = u_xlat0.yyyy * UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1], u_xlat0.xxxx, u_xlat2);
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)], u_xlat0.zzzz, u_xlat2);
    output.mtl_Position = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)], u_xlat0.wwww, u_xlat2);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
	int unity_StereoEyeIndex;
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = unity_StereoEyeIndex << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 105
; Schema: 0
                                                   OpCapability Shader 
                                            %1 = OpExtInstImport "GLSL.std.450" 
                                                   OpMemoryModel Logical GLSL450 
                                                   OpEntryPoint Vertex %4 "main" %11 %84 %96 %97 
                                                   OpDecorate %11 Location 11 
                                                   OpDecorate %16 ArrayStride 16 
                                                   OpDecorate %18 ArrayStride 18 
                                                   OpMemberDecorate %20 0 Offset 20 
                                                   OpMemberDecorate %20 1 Offset 20 
                                                   OpMemberDecorate %20 2 Offset 20 
                                                   OpDecorate %20 Block 
                                                   OpDecorate %22 DescriptorSet 22 
                                                   OpDecorate %22 Binding 22 
                                                   OpMemberDecorate %82 0 BuiltIn 82 
                                                   OpMemberDecorate %82 1 BuiltIn 82 
                                                   OpMemberDecorate %82 2 BuiltIn 82 
                                                   OpDecorate %82 Block 
                                                   OpDecorate %96 Location 96 
                                                   OpDecorate %97 Location 97 
                                            %2 = OpTypeVoid 
                                            %3 = OpTypeFunction %2 
                                            %6 = OpTypeFloat 32 
                                            %7 = OpTypeVector %6 4 
                                            %8 = OpTypePointer Private %7 
                             Private f32_4* %9 = OpVariable Private 
                                           %10 = OpTypePointer Input %7 
                              Input f32_4* %11 = OpVariable Input 
                                           %14 = OpTypeInt 32 0 
                                       u32 %15 = OpConstant 4 
                                           %16 = OpTypeArray %7 %15 
                                       u32 %17 = OpConstant 8 
                                           %18 = OpTypeArray %7 %17 
                                           %19 = OpTypeInt 32 1 
                                           %20 = OpTypeStruct %16 %18 %19 
                                           %21 = OpTypePointer Uniform %20 
Uniform struct {f32_4[4]; f32_4[8]; i32;}* %22 = OpVariable Uniform 
                                       i32 %23 = OpConstant 0 
                                       i32 %24 = OpConstant 1 
                                           %25 = OpTypePointer Uniform %7 
                                       i32 %36 = OpConstant 2 
                                       i32 %45 = OpConstant 3 
                                           %49 = OpTypePointer Private %19 
                              Private i32* %50 = OpVariable Private 
                                           %51 = OpTypePointer Uniform %19 
                            Private f32_4* %55 = OpVariable Private 
                                       u32 %80 = OpConstant 1 
                                           %81 = OpTypeArray %6 %80 
                                           %82 = OpTypeStruct %7 %6 %81 
                                           %83 = OpTypePointer Output %82 
      Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                           %94 = OpTypePointer Output %7 
                             Output f32_4* %96 = OpVariable Output 
                              Input f32_4* %97 = OpVariable Input 
                                           %99 = OpTypePointer Output %6 
                                       void %4 = OpFunction None %3 
                                            %5 = OpLabel 
                                     f32_4 %12 = OpLoad %11 
                                     f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                            Uniform f32_4* %26 = OpAccessChain %22 %23 %24 
                                     f32_4 %27 = OpLoad %26 
                                     f32_4 %28 = OpFMul %13 %27 
                                                   OpStore %9 %28 
                            Uniform f32_4* %29 = OpAccessChain %22 %23 %23 
                                     f32_4 %30 = OpLoad %29 
                                     f32_4 %31 = OpLoad %11 
                                     f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                     f32_4 %33 = OpFMul %30 %32 
                                     f32_4 %34 = OpLoad %9 
                                     f32_4 %35 = OpFAdd %33 %34 
                                                   OpStore %9 %35 
                            Uniform f32_4* %37 = OpAccessChain %22 %23 %36 
                                     f32_4 %38 = OpLoad %37 
                                     f32_4 %39 = OpLoad %11 
                                     f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                     f32_4 %41 = OpFMul %38 %40 
                                     f32_4 %42 = OpLoad %9 
                                     f32_4 %43 = OpFAdd %41 %42 
                                                   OpStore %9 %43 
                                     f32_4 %44 = OpLoad %9 
                            Uniform f32_4* %46 = OpAccessChain %22 %23 %45 
                                     f32_4 %47 = OpLoad %46 
                                     f32_4 %48 = OpFAdd %44 %47 
                                                   OpStore %9 %48 
                              Uniform i32* %52 = OpAccessChain %22 %36 
                                       i32 %53 = OpLoad %52 
                                       i32 %54 = OpShiftLeftLogical %53 %36 
                                                   OpStore %50 %54 
                                     f32_4 %56 = OpLoad %9 
                                     f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                       i32 %58 = OpLoad %50 
                                       i32 %59 = OpIAdd %58 %24 
                            Uniform f32_4* %60 = OpAccessChain %22 %24 %59 
                                     f32_4 %61 = OpLoad %60 
                                     f32_4 %62 = OpFMul %57 %61 
                                                   OpStore %55 %62 
                                       i32 %63 = OpLoad %50 
                            Uniform f32_4* %64 = OpAccessChain %22 %24 %63 
                                     f32_4 %65 = OpLoad %64 
                                     f32_4 %66 = OpLoad %9 
                                     f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                     f32_4 %68 = OpFMul %65 %67 
                                     f32_4 %69 = OpLoad %55 
                                     f32_4 %70 = OpFAdd %68 %69 
                                                   OpStore %55 %70 
                                       i32 %71 = OpLoad %50 
                                       i32 %72 = OpIAdd %71 %36 
                            Uniform f32_4* %73 = OpAccessChain %22 %24 %72 
                                     f32_4 %74 = OpLoad %73 
                                     f32_4 %75 = OpLoad %9 
                                     f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                     f32_4 %77 = OpFMul %74 %76 
                                     f32_4 %78 = OpLoad %55 
                                     f32_4 %79 = OpFAdd %77 %78 
                                                   OpStore %55 %79 
                                       i32 %85 = OpLoad %50 
                                       i32 %86 = OpIAdd %85 %45 
                            Uniform f32_4* %87 = OpAccessChain %22 %24 %86 
                                     f32_4 %88 = OpLoad %87 
                                     f32_4 %89 = OpLoad %9 
                                     f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                     f32_4 %91 = OpFMul %88 %90 
                                     f32_4 %92 = OpLoad %55 
                                     f32_4 %93 = OpFAdd %91 %92 
                             Output f32_4* %95 = OpAccessChain %84 %23 
                                                   OpStore %95 %93 
                                     f32_4 %98 = OpLoad %97 
                                                   OpStore %96 %98 
                              Output f32* %100 = OpAccessChain %84 %23 %80 
                                      f32 %101 = OpLoad %100 
                                      f32 %102 = OpFNegate %101 
                              Output f32* %103 = OpAccessChain %84 %23 %80 
                                                   OpStore %103 %102 
                                                   OpReturn
                                                   OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
flat out highp uint vs_BLENDINDICES0;
float u_xlat0;
uint u_xlatu0;
vec4 u_xlat1;
vec4 u_xlat2;
int u_xlati3;
void main()
{
    u_xlat0 = float(uint(gl_InstanceID));
    u_xlat0 = u_xlat0 * 0.5;
    u_xlat0 = fract(u_xlat0);
    u_xlat0 = u_xlat0 + u_xlat0;
    u_xlat0 = roundEven(u_xlat0);
    u_xlatu0 = uint(u_xlat0);
    u_xlati3 = int(int(u_xlatu0) << 2);
    u_xlat1 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
    u_xlat1 = u_xlat1 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat2 = u_xlat1.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati3] * u_xlat1.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 2)] * u_xlat1.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 3)] * u_xlat1.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu0;
    gl_Layer = int(u_xlatu0);
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
flat out uint vs_BLENDINDICES0;
vec4 u_xlat0;
uint u_xlatu1;
vec4 u_xlat2;
int u_xlati4;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlatu1 = uint(gl_InstanceID) & 1u;
    u_xlati4 = int(u_xlatu1) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati4] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu1;
    gl_Layer = int(u_xlatu1);
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 111
; Schema: 0
                                              OpCapability Shader 
                                              OpCapability Geometry 
                                              OpCapability ShaderViewportIndexLayerEXT 
                                              OpExtension "SPV_EXT_shader_viewport_index_layer" 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %52 %84 %96 %97 %100 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %18 ArrayStride 18 
                                              OpMemberDecorate %19 0 Offset 19 
                                              OpMemberDecorate %19 1 Offset 19 
                                              OpDecorate %19 Block 
                                              OpDecorate %21 DescriptorSet 21 
                                              OpDecorate %21 Binding 21 
                                              OpDecorate %52 BuiltIn 52 
                                              OpMemberDecorate %82 0 BuiltIn 82 
                                              OpMemberDecorate %82 1 BuiltIn 82 
                                              OpMemberDecorate %82 2 BuiltIn 82 
                                              OpDecorate %82 Block 
                                              OpDecorate %96 Location 96 
                                              OpDecorate %97 Location 97 
                                              OpDecorate %100 BuiltIn 100 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                  u32 %17 = OpConstant 8 
                                      %18 = OpTypeArray %7 %17 
                                      %19 = OpTypeStruct %16 %18 
                                      %20 = OpTypePointer Uniform %19 
Uniform struct {f32_4[4]; f32_4[8];}* %21 = OpVariable Uniform 
                                      %22 = OpTypeInt 32 1 
                                  i32 %23 = OpConstant 0 
                                  i32 %24 = OpConstant 1 
                                      %25 = OpTypePointer Uniform %7 
                                  i32 %36 = OpConstant 2 
                                  i32 %45 = OpConstant 3 
                                      %49 = OpTypePointer Private %22 
                         Private i32* %50 = OpVariable Private 
                                      %51 = OpTypePointer Input %22 
                           Input i32* %52 = OpVariable Input 
                       Private f32_4* %55 = OpVariable Private 
                                  u32 %80 = OpConstant 1 
                                      %81 = OpTypeArray %6 %80 
                                      %82 = OpTypeStruct %7 %6 %81 
                                      %83 = OpTypePointer Output %82 
 Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                      %94 = OpTypePointer Output %7 
                        Output f32_4* %96 = OpVariable Output 
                         Input f32_4* %97 = OpVariable Input 
                                      %99 = OpTypePointer Output %22 
                         Output i32* %100 = OpVariable Output 
                                     %105 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %26 = OpAccessChain %21 %23 %24 
                                f32_4 %27 = OpLoad %26 
                                f32_4 %28 = OpFMul %13 %27 
                                              OpStore %9 %28 
                       Uniform f32_4* %29 = OpAccessChain %21 %23 %23 
                                f32_4 %30 = OpLoad %29 
                                f32_4 %31 = OpLoad %11 
                                f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                f32_4 %33 = OpFMul %30 %32 
                                f32_4 %34 = OpLoad %9 
                                f32_4 %35 = OpFAdd %33 %34 
                                              OpStore %9 %35 
                       Uniform f32_4* %37 = OpAccessChain %21 %23 %36 
                                f32_4 %38 = OpLoad %37 
                                f32_4 %39 = OpLoad %11 
                                f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                f32_4 %41 = OpFMul %38 %40 
                                f32_4 %42 = OpLoad %9 
                                f32_4 %43 = OpFAdd %41 %42 
                                              OpStore %9 %43 
                                f32_4 %44 = OpLoad %9 
                       Uniform f32_4* %46 = OpAccessChain %21 %23 %45 
                                f32_4 %47 = OpLoad %46 
                                f32_4 %48 = OpFAdd %44 %47 
                                              OpStore %9 %48 
                                  i32 %53 = OpLoad %52 
                                  i32 %54 = OpBitFieldInsert %23 %53 %36 %24 
                                              OpStore %50 %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                  i32 %58 = OpLoad %50 
                                  i32 %59 = OpIAdd %58 %24 
                       Uniform f32_4* %60 = OpAccessChain %21 %24 %59 
                                f32_4 %61 = OpLoad %60 
                                f32_4 %62 = OpFMul %57 %61 
                                              OpStore %55 %62 
                                  i32 %63 = OpLoad %50 
                       Uniform f32_4* %64 = OpAccessChain %21 %24 %63 
                                f32_4 %65 = OpLoad %64 
                                f32_4 %66 = OpLoad %9 
                                f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                f32_4 %68 = OpFMul %65 %67 
                                f32_4 %69 = OpLoad %55 
                                f32_4 %70 = OpFAdd %68 %69 
                                              OpStore %55 %70 
                                  i32 %71 = OpLoad %50 
                                  i32 %72 = OpIAdd %71 %36 
                       Uniform f32_4* %73 = OpAccessChain %21 %24 %72 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %55 
                                f32_4 %79 = OpFAdd %77 %78 
                                              OpStore %55 %79 
                                  i32 %85 = OpLoad %50 
                                  i32 %86 = OpIAdd %85 %45 
                       Uniform f32_4* %87 = OpAccessChain %21 %24 %86 
                                f32_4 %88 = OpLoad %87 
                                f32_4 %89 = OpLoad %9 
                                f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                f32_4 %91 = OpFMul %88 %90 
                                f32_4 %92 = OpLoad %55 
                                f32_4 %93 = OpFAdd %91 %92 
                        Output f32_4* %95 = OpAccessChain %84 %23 
                                              OpStore %95 %93 
                                f32_4 %98 = OpLoad %97 
                                              OpStore %96 %98 
                                 i32 %101 = OpLoad %52 
                                 u32 %102 = OpBitcast %101 
                                 u32 %103 = OpBitwiseAnd %102 %80 
                                 i32 %104 = OpBitcast %103 
                                              OpStore %100 %104 
                         Output f32* %106 = OpAccessChain %84 %23 %80 
                                 f32 %107 = OpLoad %106 
                                 f32 %108 = OpFNegate %107 
                         Output f32* %109 = OpAccessChain %84 %23 %80 
                                              OpStore %109 %108 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
#endif
	UNITY_UNIFORM vec4 unity_StereoEyeIndices[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
layout(num_views = 2) in;
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
out highp float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(int(gl_ViewID_OVR) << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
	vec4 unity_StereoEyeIndices[2];
};
layout(num_views = 2) in;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
out float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(gl_ViewID_OVR) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 134
; Schema: 0
                                                      OpCapability Shader 
                                                      OpCapability MultiView 
                                                      OpExtension "SPV_KHR_multiview" 
                                               %1 = OpExtInstImport "GLSL.std.450" 
                                                      OpMemoryModel Logical GLSL450 
                                                      OpEntryPoint Vertex %4 "main" %11 %50 %99 %111 %112 %118 
                                                      OpDecorate %11 Location 11 
                                                      OpDecorate %16 ArrayStride 16 
                                                      OpMemberDecorate %17 0 Offset 17 
                                                      OpDecorate %17 Block 
                                                      OpDecorate %19 DescriptorSet 19 
                                                      OpDecorate %19 Binding 19 
                                                      OpDecorate %50 BuiltIn 50 
                                                      OpDecorate %58 ArrayStride 58 
                                                      OpDecorate %59 ArrayStride 59 
                                                      OpDecorate %60 ArrayStride 60 
                                                      OpDecorate %61 ArrayStride 61 
                                                      OpDecorate %62 ArrayStride 62 
                                                      OpDecorate %63 ArrayStride 63 
                                                      OpDecorate %64 ArrayStride 64 
                                                      OpDecorate %65 ArrayStride 65 
                                                      OpDecorate %68 ArrayStride 68 
                                                      OpDecorate %69 ArrayStride 69 
                                                      OpMemberDecorate %70 0 Offset 70 
                                                      OpMemberDecorate %70 1 Offset 70 
                                                      OpMemberDecorate %70 2 Offset 70 
                                                      OpMemberDecorate %70 3 Offset 70 
                                                      OpMemberDecorate %70 4 Offset 70 
                                                      OpMemberDecorate %70 5 Offset 70 
                                                      OpMemberDecorate %70 6 Offset 70 
                                                      OpMemberDecorate %70 7 Offset 70 
                                                      OpMemberDecorate %70 8 Offset 70 
                                                      OpMemberDecorate %70 9 Offset 70 
                                                      OpDecorate %70 Block 
                                                      OpDecorate %72 DescriptorSet 72 
                                                      OpDecorate %72 Binding 72 
                                                      OpMemberDecorate %97 0 BuiltIn 97 
                                                      OpMemberDecorate %97 1 BuiltIn 97 
                                                      OpMemberDecorate %97 2 BuiltIn 97 
                                                      OpDecorate %97 Block 
                                                      OpDecorate %111 Location 111 
                                                      OpDecorate %112 Location 112 
                                                      OpDecorate %118 Location 118 
                                                      OpDecorate %119 ArrayStride 119 
                                                      OpMemberDecorate %120 0 Offset 120 
                                                      OpDecorate %120 Block 
                                                      OpDecorate %122 DescriptorSet 122 
                                                      OpDecorate %122 Binding 122 
                                               %2 = OpTypeVoid 
                                               %3 = OpTypeFunction %2 
                                               %6 = OpTypeFloat 32 
                                               %7 = OpTypeVector %6 4 
                                               %8 = OpTypePointer Private %7 
                                Private f32_4* %9 = OpVariable Private 
                                              %10 = OpTypePointer Input %7 
                                 Input f32_4* %11 = OpVariable Input 
                                              %14 = OpTypeInt 32 0 
                                          u32 %15 = OpConstant 4 
                                              %16 = OpTypeArray %7 %15 
                                              %17 = OpTypeStruct %16 
                                              %18 = OpTypePointer Uniform %17 
                  Uniform struct {f32_4[4];}* %19 = OpVariable Uniform 
                                              %20 = OpTypeInt 32 1 
                                          i32 %21 = OpConstant 0 
                                          i32 %22 = OpConstant 1 
                                              %23 = OpTypePointer Uniform %7 
                                          i32 %34 = OpConstant 2 
                                          i32 %43 = OpConstant 3 
                                              %47 = OpTypePointer Private %20 
                                 Private i32* %48 = OpVariable Private 
                                              %49 = OpTypePointer Input %14 
                                   Input u32* %50 = OpVariable Input 
                               Private f32_4* %54 = OpVariable Private 
                                          u32 %57 = OpConstant 8 
                                              %58 = OpTypeArray %7 %57 
                                              %59 = OpTypeArray %7 %57 
                                              %60 = OpTypeArray %7 %57 
                                              %61 = OpTypeArray %7 %57 
                                              %62 = OpTypeArray %7 %57 
                                              %63 = OpTypeArray %7 %57 
                                              %64 = OpTypeArray %7 %57 
                                              %65 = OpTypeArray %7 %57 
                                              %66 = OpTypeVector %6 3 
                                          u32 %67 = OpConstant 2 
                                              %68 = OpTypeArray %66 %67 
                                              %69 = OpTypeArray %7 %67 
                                              %70 = OpTypeStruct %58 %59 %60 %61 %62 %63 %64 %65 %68 %69 
                                              %71 = OpTypePointer Uniform %70 
Uniform struct {f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_3[2]; f32_4[2];}* %72 = OpVariable Uniform 
                                          u32 %95 = OpConstant 1 
                                              %96 = OpTypeArray %6 %95 
                                              %97 = OpTypeStruct %7 %6 %96 
                                              %98 = OpTypePointer Output %97 
         Output struct {f32_4; f32; f32[1];}* %99 = OpVariable Output 
                                             %109 = OpTypePointer Output %7 
                               Output f32_4* %111 = OpVariable Output 
                                Input f32_4* %112 = OpVariable Input 
                                             %114 = OpTypePointer Private %14 
                                Private u32* %115 = OpVariable Private 
                                             %117 = OpTypePointer Output %6 
                                 Output f32* %118 = OpVariable Output 
                                             %119 = OpTypeArray %7 %67 
                                             %120 = OpTypeStruct %119 
                                             %121 = OpTypePointer Uniform %120 
                 Uniform struct {f32_4[2];}* %122 = OpVariable Uniform 
                                         u32 %125 = OpConstant 0 
                                             %126 = OpTypePointer Uniform %6 
                                          void %4 = OpFunction None %3 
                                               %5 = OpLabel 
                                        f32_4 %12 = OpLoad %11 
                                        f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                               Uniform f32_4* %24 = OpAccessChain %19 %21 %22 
                                        f32_4 %25 = OpLoad %24 
                                        f32_4 %26 = OpFMul %13 %25 
                                                      OpStore %9 %26 
                               Uniform f32_4* %27 = OpAccessChain %19 %21 %21 
                                        f32_4 %28 = OpLoad %27 
                                        f32_4 %29 = OpLoad %11 
                                        f32_4 %30 = OpVectorShuffle %29 %29 0 0 0 0 
                                        f32_4 %31 = OpFMul %28 %30 
                                        f32_4 %32 = OpLoad %9 
                                        f32_4 %33 = OpFAdd %31 %32 
                                                      OpStore %9 %33 
                               Uniform f32_4* %35 = OpAccessChain %19 %21 %34 
                                        f32_4 %36 = OpLoad %35 
                                        f32_4 %37 = OpLoad %11 
                                        f32_4 %38 = OpVectorShuffle %37 %37 2 2 2 2 
                                        f32_4 %39 = OpFMul %36 %38 
                                        f32_4 %40 = OpLoad %9 
                                        f32_4 %41 = OpFAdd %39 %40 
                                                      OpStore %9 %41 
                                        f32_4 %42 = OpLoad %9 
                               Uniform f32_4* %44 = OpAccessChain %19 %21 %43 
                                        f32_4 %45 = OpLoad %44 
                                        f32_4 %46 = OpFAdd %42 %45 
                                                      OpStore %9 %46 
                                          u32 %51 = OpLoad %50 
                                          i32 %52 = OpBitcast %51 
                                          i32 %53 = OpShiftLeftLogical %52 %34 
                                                      OpStore %48 %53 
                                        f32_4 %55 = OpLoad %9 
                                        f32_4 %56 = OpVectorShuffle %55 %55 1 1 1 1 
                                          i32 %73 = OpLoad %48 
                                          i32 %74 = OpIAdd %73 %22 
                               Uniform f32_4* %75 = OpAccessChain %72 %43 %74 
                                        f32_4 %76 = OpLoad %75 
                                        f32_4 %77 = OpFMul %56 %76 
                                                      OpStore %54 %77 
                                          i32 %78 = OpLoad %48 
                               Uniform f32_4* %79 = OpAccessChain %72 %43 %78 
                                        f32_4 %80 = OpLoad %79 
                                        f32_4 %81 = OpLoad %9 
                                        f32_4 %82 = OpVectorShuffle %81 %81 0 0 0 0 
                                        f32_4 %83 = OpFMul %80 %82 
                                        f32_4 %84 = OpLoad %54 
                                        f32_4 %85 = OpFAdd %83 %84 
                                                      OpStore %54 %85 
                                          i32 %86 = OpLoad %48 
                                          i32 %87 = OpIAdd %86 %34 
                               Uniform f32_4* %88 = OpAccessChain %72 %43 %87 
                                        f32_4 %89 = OpLoad %88 
                                        f32_4 %90 = OpLoad %9 
                                        f32_4 %91 = OpVectorShuffle %90 %90 2 2 2 2 
                                        f32_4 %92 = OpFMul %89 %91 
                                        f32_4 %93 = OpLoad %54 
                                        f32_4 %94 = OpFAdd %92 %93 
                                                      OpStore %54 %94 
                                         i32 %100 = OpLoad %48 
                                         i32 %101 = OpIAdd %100 %43 
                              Uniform f32_4* %102 = OpAccessChain %72 %43 %101 
                                       f32_4 %103 = OpLoad %102 
                                       f32_4 %104 = OpLoad %9 
                                       f32_4 %105 = OpVectorShuffle %104 %104 3 3 3 3 
                                       f32_4 %106 = OpFMul %103 %105 
                                       f32_4 %107 = OpLoad %54 
                                       f32_4 %108 = OpFAdd %106 %107 
                               Output f32_4* %110 = OpAccessChain %99 %21 
                                                      OpStore %110 %108 
                                       f32_4 %113 = OpLoad %112 
                                                      OpStore %111 %113 
                                         u32 %116 = OpLoad %50 
                                                      OpStore %115 %116 
                                         u32 %123 = OpLoad %115 
                                         i32 %124 = OpBitcast %123 
                                Uniform f32* %127 = OpAccessChain %122 %21 %124 %125 
                                         f32 %128 = OpLoad %127 
                                                      OpStore %118 %128 
                                 Output f32* %129 = OpAccessChain %99 %21 %95 
                                         f32 %130 = OpLoad %129 
                                         f32 %131 = OpFNegate %130 
                                 Output f32* %132 = OpAccessChain %99 %21 %95 
                                                      OpStore %132 %131 
                                                      OpReturn
                                                      OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
""
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
""
}
SubProgram "vulkan " {
""
}
SubProgram "gles " {
""
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
}
}
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Stencil {
   Comp Always
   Pass Zero
   Fail Keep
   ZFail Keep
  }
  GpuProgramID 346237
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 91
; Schema: 0
                                              OpCapability Shader 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %72 %82 %83 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %17 ArrayStride 17 
                                              OpMemberDecorate %18 0 Offset 18 
                                              OpMemberDecorate %18 1 Offset 18 
                                              OpDecorate %18 Block 
                                              OpDecorate %20 DescriptorSet 20 
                                              OpDecorate %20 Binding 20 
                                              OpMemberDecorate %70 0 BuiltIn 70 
                                              OpMemberDecorate %70 1 BuiltIn 70 
                                              OpMemberDecorate %70 2 BuiltIn 70 
                                              OpDecorate %70 Block 
                                              OpDecorate %82 Location 82 
                                              OpDecorate %83 Location 83 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                      %17 = OpTypeArray %7 %15 
                                      %18 = OpTypeStruct %16 %17 
                                      %19 = OpTypePointer Uniform %18 
Uniform struct {f32_4[4]; f32_4[4];}* %20 = OpVariable Uniform 
                                      %21 = OpTypeInt 32 1 
                                  i32 %22 = OpConstant 0 
                                  i32 %23 = OpConstant 1 
                                      %24 = OpTypePointer Uniform %7 
                                  i32 %35 = OpConstant 2 
                                  i32 %44 = OpConstant 3 
                       Private f32_4* %48 = OpVariable Private 
                                  u32 %68 = OpConstant 1 
                                      %69 = OpTypeArray %6 %68 
                                      %70 = OpTypeStruct %7 %6 %69 
                                      %71 = OpTypePointer Output %70 
 Output struct {f32_4; f32; f32[1];}* %72 = OpVariable Output 
                                      %80 = OpTypePointer Output %7 
                        Output f32_4* %82 = OpVariable Output 
                         Input f32_4* %83 = OpVariable Input 
                                      %85 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %25 = OpAccessChain %20 %22 %23 
                                f32_4 %26 = OpLoad %25 
                                f32_4 %27 = OpFMul %13 %26 
                                              OpStore %9 %27 
                       Uniform f32_4* %28 = OpAccessChain %20 %22 %22 
                                f32_4 %29 = OpLoad %28 
                                f32_4 %30 = OpLoad %11 
                                f32_4 %31 = OpVectorShuffle %30 %30 0 0 0 0 
                                f32_4 %32 = OpFMul %29 %31 
                                f32_4 %33 = OpLoad %9 
                                f32_4 %34 = OpFAdd %32 %33 
                                              OpStore %9 %34 
                       Uniform f32_4* %36 = OpAccessChain %20 %22 %35 
                                f32_4 %37 = OpLoad %36 
                                f32_4 %38 = OpLoad %11 
                                f32_4 %39 = OpVectorShuffle %38 %38 2 2 2 2 
                                f32_4 %40 = OpFMul %37 %39 
                                f32_4 %41 = OpLoad %9 
                                f32_4 %42 = OpFAdd %40 %41 
                                              OpStore %9 %42 
                                f32_4 %43 = OpLoad %9 
                       Uniform f32_4* %45 = OpAccessChain %20 %22 %44 
                                f32_4 %46 = OpLoad %45 
                                f32_4 %47 = OpFAdd %43 %46 
                                              OpStore %9 %47 
                                f32_4 %49 = OpLoad %9 
                                f32_4 %50 = OpVectorShuffle %49 %49 1 1 1 1 
                       Uniform f32_4* %51 = OpAccessChain %20 %23 %23 
                                f32_4 %52 = OpLoad %51 
                                f32_4 %53 = OpFMul %50 %52 
                                              OpStore %48 %53 
                       Uniform f32_4* %54 = OpAccessChain %20 %23 %22 
                                f32_4 %55 = OpLoad %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 0 0 0 0 
                                f32_4 %58 = OpFMul %55 %57 
                                f32_4 %59 = OpLoad %48 
                                f32_4 %60 = OpFAdd %58 %59 
                                              OpStore %48 %60 
                       Uniform f32_4* %61 = OpAccessChain %20 %23 %35 
                                f32_4 %62 = OpLoad %61 
                                f32_4 %63 = OpLoad %9 
                                f32_4 %64 = OpVectorShuffle %63 %63 2 2 2 2 
                                f32_4 %65 = OpFMul %62 %64 
                                f32_4 %66 = OpLoad %48 
                                f32_4 %67 = OpFAdd %65 %66 
                                              OpStore %48 %67 
                       Uniform f32_4* %73 = OpAccessChain %20 %23 %44 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 3 3 3 3 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %48 
                                f32_4 %79 = OpFAdd %77 %78 
                        Output f32_4* %81 = OpAccessChain %72 %22 
                                              OpStore %81 %79 
                                f32_4 %84 = OpLoad %83 
                                              OpStore %82 %84 
                          Output f32* %86 = OpAccessChain %72 %22 %68 
                                  f32 %87 = OpLoad %86 
                                  f32 %88 = OpFNegate %87 
                          Output f32* %89 = OpAccessChain %72 %22 %68 
                                              OpStore %89 %88 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
uniform 	int unity_StereoEyeIndex;
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
int op_shl(int a, int b) { return int(floor(float(a) * pow(2.0, float(b)))); }
ivec2 op_shl(ivec2 a, ivec2 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); return a; }
ivec3 op_shl(ivec3 a, ivec3 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); return a; }
ivec4 op_shl(ivec4 a, ivec4 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); a.w = op_shl(a.w, b.w); return a; }

void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = op_shl(unity_StereoEyeIndex, 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
#endif
	UNITY_UNIFORM int unity_StereoEyeIndex;
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(unity_StereoEyeIndex << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
};

struct UnityStereoGlobals_Type
{
    float4 hlslcc_mtx4x4unity_StereoMatrixP[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
    float4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
    float4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
    float4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
    float4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
    float3 unity_StereoWorldSpaceCameraPos[2];
    float4 unity_StereoScaleOffset[2];
};

struct UnityStereoEyeIndex_Type
{
    int unity_StereoEyeIndex;
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    constant UnityStereoGlobals_Type& UnityStereoGlobals [[ buffer(1) ]],
    constant UnityStereoEyeIndex_Type& UnityStereoEyeIndex [[ buffer(2) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    int u_xlati1;
    float4 u_xlat2;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = UnityStereoEyeIndex.unity_StereoEyeIndex << 0x2;
    u_xlat2 = u_xlat0.yyyy * UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1], u_xlat0.xxxx, u_xlat2);
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)], u_xlat0.zzzz, u_xlat2);
    output.mtl_Position = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)], u_xlat0.wwww, u_xlat2);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
	int unity_StereoEyeIndex;
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = unity_StereoEyeIndex << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 105
; Schema: 0
                                                   OpCapability Shader 
                                            %1 = OpExtInstImport "GLSL.std.450" 
                                                   OpMemoryModel Logical GLSL450 
                                                   OpEntryPoint Vertex %4 "main" %11 %84 %96 %97 
                                                   OpDecorate %11 Location 11 
                                                   OpDecorate %16 ArrayStride 16 
                                                   OpDecorate %18 ArrayStride 18 
                                                   OpMemberDecorate %20 0 Offset 20 
                                                   OpMemberDecorate %20 1 Offset 20 
                                                   OpMemberDecorate %20 2 Offset 20 
                                                   OpDecorate %20 Block 
                                                   OpDecorate %22 DescriptorSet 22 
                                                   OpDecorate %22 Binding 22 
                                                   OpMemberDecorate %82 0 BuiltIn 82 
                                                   OpMemberDecorate %82 1 BuiltIn 82 
                                                   OpMemberDecorate %82 2 BuiltIn 82 
                                                   OpDecorate %82 Block 
                                                   OpDecorate %96 Location 96 
                                                   OpDecorate %97 Location 97 
                                            %2 = OpTypeVoid 
                                            %3 = OpTypeFunction %2 
                                            %6 = OpTypeFloat 32 
                                            %7 = OpTypeVector %6 4 
                                            %8 = OpTypePointer Private %7 
                             Private f32_4* %9 = OpVariable Private 
                                           %10 = OpTypePointer Input %7 
                              Input f32_4* %11 = OpVariable Input 
                                           %14 = OpTypeInt 32 0 
                                       u32 %15 = OpConstant 4 
                                           %16 = OpTypeArray %7 %15 
                                       u32 %17 = OpConstant 8 
                                           %18 = OpTypeArray %7 %17 
                                           %19 = OpTypeInt 32 1 
                                           %20 = OpTypeStruct %16 %18 %19 
                                           %21 = OpTypePointer Uniform %20 
Uniform struct {f32_4[4]; f32_4[8]; i32;}* %22 = OpVariable Uniform 
                                       i32 %23 = OpConstant 0 
                                       i32 %24 = OpConstant 1 
                                           %25 = OpTypePointer Uniform %7 
                                       i32 %36 = OpConstant 2 
                                       i32 %45 = OpConstant 3 
                                           %49 = OpTypePointer Private %19 
                              Private i32* %50 = OpVariable Private 
                                           %51 = OpTypePointer Uniform %19 
                            Private f32_4* %55 = OpVariable Private 
                                       u32 %80 = OpConstant 1 
                                           %81 = OpTypeArray %6 %80 
                                           %82 = OpTypeStruct %7 %6 %81 
                                           %83 = OpTypePointer Output %82 
      Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                           %94 = OpTypePointer Output %7 
                             Output f32_4* %96 = OpVariable Output 
                              Input f32_4* %97 = OpVariable Input 
                                           %99 = OpTypePointer Output %6 
                                       void %4 = OpFunction None %3 
                                            %5 = OpLabel 
                                     f32_4 %12 = OpLoad %11 
                                     f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                            Uniform f32_4* %26 = OpAccessChain %22 %23 %24 
                                     f32_4 %27 = OpLoad %26 
                                     f32_4 %28 = OpFMul %13 %27 
                                                   OpStore %9 %28 
                            Uniform f32_4* %29 = OpAccessChain %22 %23 %23 
                                     f32_4 %30 = OpLoad %29 
                                     f32_4 %31 = OpLoad %11 
                                     f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                     f32_4 %33 = OpFMul %30 %32 
                                     f32_4 %34 = OpLoad %9 
                                     f32_4 %35 = OpFAdd %33 %34 
                                                   OpStore %9 %35 
                            Uniform f32_4* %37 = OpAccessChain %22 %23 %36 
                                     f32_4 %38 = OpLoad %37 
                                     f32_4 %39 = OpLoad %11 
                                     f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                     f32_4 %41 = OpFMul %38 %40 
                                     f32_4 %42 = OpLoad %9 
                                     f32_4 %43 = OpFAdd %41 %42 
                                                   OpStore %9 %43 
                                     f32_4 %44 = OpLoad %9 
                            Uniform f32_4* %46 = OpAccessChain %22 %23 %45 
                                     f32_4 %47 = OpLoad %46 
                                     f32_4 %48 = OpFAdd %44 %47 
                                                   OpStore %9 %48 
                              Uniform i32* %52 = OpAccessChain %22 %36 
                                       i32 %53 = OpLoad %52 
                                       i32 %54 = OpShiftLeftLogical %53 %36 
                                                   OpStore %50 %54 
                                     f32_4 %56 = OpLoad %9 
                                     f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                       i32 %58 = OpLoad %50 
                                       i32 %59 = OpIAdd %58 %24 
                            Uniform f32_4* %60 = OpAccessChain %22 %24 %59 
                                     f32_4 %61 = OpLoad %60 
                                     f32_4 %62 = OpFMul %57 %61 
                                                   OpStore %55 %62 
                                       i32 %63 = OpLoad %50 
                            Uniform f32_4* %64 = OpAccessChain %22 %24 %63 
                                     f32_4 %65 = OpLoad %64 
                                     f32_4 %66 = OpLoad %9 
                                     f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                     f32_4 %68 = OpFMul %65 %67 
                                     f32_4 %69 = OpLoad %55 
                                     f32_4 %70 = OpFAdd %68 %69 
                                                   OpStore %55 %70 
                                       i32 %71 = OpLoad %50 
                                       i32 %72 = OpIAdd %71 %36 
                            Uniform f32_4* %73 = OpAccessChain %22 %24 %72 
                                     f32_4 %74 = OpLoad %73 
                                     f32_4 %75 = OpLoad %9 
                                     f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                     f32_4 %77 = OpFMul %74 %76 
                                     f32_4 %78 = OpLoad %55 
                                     f32_4 %79 = OpFAdd %77 %78 
                                                   OpStore %55 %79 
                                       i32 %85 = OpLoad %50 
                                       i32 %86 = OpIAdd %85 %45 
                            Uniform f32_4* %87 = OpAccessChain %22 %24 %86 
                                     f32_4 %88 = OpLoad %87 
                                     f32_4 %89 = OpLoad %9 
                                     f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                     f32_4 %91 = OpFMul %88 %90 
                                     f32_4 %92 = OpLoad %55 
                                     f32_4 %93 = OpFAdd %91 %92 
                             Output f32_4* %95 = OpAccessChain %84 %23 
                                                   OpStore %95 %93 
                                     f32_4 %98 = OpLoad %97 
                                                   OpStore %96 %98 
                              Output f32* %100 = OpAccessChain %84 %23 %80 
                                      f32 %101 = OpLoad %100 
                                      f32 %102 = OpFNegate %101 
                              Output f32* %103 = OpAccessChain %84 %23 %80 
                                                   OpStore %103 %102 
                                                   OpReturn
                                                   OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
flat out highp uint vs_BLENDINDICES0;
float u_xlat0;
uint u_xlatu0;
vec4 u_xlat1;
vec4 u_xlat2;
int u_xlati3;
void main()
{
    u_xlat0 = float(uint(gl_InstanceID));
    u_xlat0 = u_xlat0 * 0.5;
    u_xlat0 = fract(u_xlat0);
    u_xlat0 = u_xlat0 + u_xlat0;
    u_xlat0 = roundEven(u_xlat0);
    u_xlatu0 = uint(u_xlat0);
    u_xlati3 = int(int(u_xlatu0) << 2);
    u_xlat1 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
    u_xlat1 = u_xlat1 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat2 = u_xlat1.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati3] * u_xlat1.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 2)] * u_xlat1.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 3)] * u_xlat1.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu0;
    gl_Layer = int(u_xlatu0);
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
flat out uint vs_BLENDINDICES0;
vec4 u_xlat0;
uint u_xlatu1;
vec4 u_xlat2;
int u_xlati4;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlatu1 = uint(gl_InstanceID) & 1u;
    u_xlati4 = int(u_xlatu1) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati4] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu1;
    gl_Layer = int(u_xlatu1);
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 111
; Schema: 0
                                              OpCapability Shader 
                                              OpCapability Geometry 
                                              OpCapability ShaderViewportIndexLayerEXT 
                                              OpExtension "SPV_EXT_shader_viewport_index_layer" 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %52 %84 %96 %97 %100 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %18 ArrayStride 18 
                                              OpMemberDecorate %19 0 Offset 19 
                                              OpMemberDecorate %19 1 Offset 19 
                                              OpDecorate %19 Block 
                                              OpDecorate %21 DescriptorSet 21 
                                              OpDecorate %21 Binding 21 
                                              OpDecorate %52 BuiltIn 52 
                                              OpMemberDecorate %82 0 BuiltIn 82 
                                              OpMemberDecorate %82 1 BuiltIn 82 
                                              OpMemberDecorate %82 2 BuiltIn 82 
                                              OpDecorate %82 Block 
                                              OpDecorate %96 Location 96 
                                              OpDecorate %97 Location 97 
                                              OpDecorate %100 BuiltIn 100 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                  u32 %17 = OpConstant 8 
                                      %18 = OpTypeArray %7 %17 
                                      %19 = OpTypeStruct %16 %18 
                                      %20 = OpTypePointer Uniform %19 
Uniform struct {f32_4[4]; f32_4[8];}* %21 = OpVariable Uniform 
                                      %22 = OpTypeInt 32 1 
                                  i32 %23 = OpConstant 0 
                                  i32 %24 = OpConstant 1 
                                      %25 = OpTypePointer Uniform %7 
                                  i32 %36 = OpConstant 2 
                                  i32 %45 = OpConstant 3 
                                      %49 = OpTypePointer Private %22 
                         Private i32* %50 = OpVariable Private 
                                      %51 = OpTypePointer Input %22 
                           Input i32* %52 = OpVariable Input 
                       Private f32_4* %55 = OpVariable Private 
                                  u32 %80 = OpConstant 1 
                                      %81 = OpTypeArray %6 %80 
                                      %82 = OpTypeStruct %7 %6 %81 
                                      %83 = OpTypePointer Output %82 
 Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                      %94 = OpTypePointer Output %7 
                        Output f32_4* %96 = OpVariable Output 
                         Input f32_4* %97 = OpVariable Input 
                                      %99 = OpTypePointer Output %22 
                         Output i32* %100 = OpVariable Output 
                                     %105 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %26 = OpAccessChain %21 %23 %24 
                                f32_4 %27 = OpLoad %26 
                                f32_4 %28 = OpFMul %13 %27 
                                              OpStore %9 %28 
                       Uniform f32_4* %29 = OpAccessChain %21 %23 %23 
                                f32_4 %30 = OpLoad %29 
                                f32_4 %31 = OpLoad %11 
                                f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                f32_4 %33 = OpFMul %30 %32 
                                f32_4 %34 = OpLoad %9 
                                f32_4 %35 = OpFAdd %33 %34 
                                              OpStore %9 %35 
                       Uniform f32_4* %37 = OpAccessChain %21 %23 %36 
                                f32_4 %38 = OpLoad %37 
                                f32_4 %39 = OpLoad %11 
                                f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                f32_4 %41 = OpFMul %38 %40 
                                f32_4 %42 = OpLoad %9 
                                f32_4 %43 = OpFAdd %41 %42 
                                              OpStore %9 %43 
                                f32_4 %44 = OpLoad %9 
                       Uniform f32_4* %46 = OpAccessChain %21 %23 %45 
                                f32_4 %47 = OpLoad %46 
                                f32_4 %48 = OpFAdd %44 %47 
                                              OpStore %9 %48 
                                  i32 %53 = OpLoad %52 
                                  i32 %54 = OpBitFieldInsert %23 %53 %36 %24 
                                              OpStore %50 %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                  i32 %58 = OpLoad %50 
                                  i32 %59 = OpIAdd %58 %24 
                       Uniform f32_4* %60 = OpAccessChain %21 %24 %59 
                                f32_4 %61 = OpLoad %60 
                                f32_4 %62 = OpFMul %57 %61 
                                              OpStore %55 %62 
                                  i32 %63 = OpLoad %50 
                       Uniform f32_4* %64 = OpAccessChain %21 %24 %63 
                                f32_4 %65 = OpLoad %64 
                                f32_4 %66 = OpLoad %9 
                                f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                f32_4 %68 = OpFMul %65 %67 
                                f32_4 %69 = OpLoad %55 
                                f32_4 %70 = OpFAdd %68 %69 
                                              OpStore %55 %70 
                                  i32 %71 = OpLoad %50 
                                  i32 %72 = OpIAdd %71 %36 
                       Uniform f32_4* %73 = OpAccessChain %21 %24 %72 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %55 
                                f32_4 %79 = OpFAdd %77 %78 
                                              OpStore %55 %79 
                                  i32 %85 = OpLoad %50 
                                  i32 %86 = OpIAdd %85 %45 
                       Uniform f32_4* %87 = OpAccessChain %21 %24 %86 
                                f32_4 %88 = OpLoad %87 
                                f32_4 %89 = OpLoad %9 
                                f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                f32_4 %91 = OpFMul %88 %90 
                                f32_4 %92 = OpLoad %55 
                                f32_4 %93 = OpFAdd %91 %92 
                        Output f32_4* %95 = OpAccessChain %84 %23 
                                              OpStore %95 %93 
                                f32_4 %98 = OpLoad %97 
                                              OpStore %96 %98 
                                 i32 %101 = OpLoad %52 
                                 u32 %102 = OpBitcast %101 
                                 u32 %103 = OpBitwiseAnd %102 %80 
                                 i32 %104 = OpBitcast %103 
                                              OpStore %100 %104 
                         Output f32* %106 = OpAccessChain %84 %23 %80 
                                 f32 %107 = OpLoad %106 
                                 f32 %108 = OpFNegate %107 
                         Output f32* %109 = OpAccessChain %84 %23 %80 
                                              OpStore %109 %108 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
#endif
	UNITY_UNIFORM vec4 unity_StereoEyeIndices[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
layout(num_views = 2) in;
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
out highp float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(int(gl_ViewID_OVR) << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
	vec4 unity_StereoEyeIndices[2];
};
layout(num_views = 2) in;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
out float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(gl_ViewID_OVR) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 134
; Schema: 0
                                                      OpCapability Shader 
                                                      OpCapability MultiView 
                                                      OpExtension "SPV_KHR_multiview" 
                                               %1 = OpExtInstImport "GLSL.std.450" 
                                                      OpMemoryModel Logical GLSL450 
                                                      OpEntryPoint Vertex %4 "main" %11 %50 %99 %111 %112 %118 
                                                      OpDecorate %11 Location 11 
                                                      OpDecorate %16 ArrayStride 16 
                                                      OpMemberDecorate %17 0 Offset 17 
                                                      OpDecorate %17 Block 
                                                      OpDecorate %19 DescriptorSet 19 
                                                      OpDecorate %19 Binding 19 
                                                      OpDecorate %50 BuiltIn 50 
                                                      OpDecorate %58 ArrayStride 58 
                                                      OpDecorate %59 ArrayStride 59 
                                                      OpDecorate %60 ArrayStride 60 
                                                      OpDecorate %61 ArrayStride 61 
                                                      OpDecorate %62 ArrayStride 62 
                                                      OpDecorate %63 ArrayStride 63 
                                                      OpDecorate %64 ArrayStride 64 
                                                      OpDecorate %65 ArrayStride 65 
                                                      OpDecorate %68 ArrayStride 68 
                                                      OpDecorate %69 ArrayStride 69 
                                                      OpMemberDecorate %70 0 Offset 70 
                                                      OpMemberDecorate %70 1 Offset 70 
                                                      OpMemberDecorate %70 2 Offset 70 
                                                      OpMemberDecorate %70 3 Offset 70 
                                                      OpMemberDecorate %70 4 Offset 70 
                                                      OpMemberDecorate %70 5 Offset 70 
                                                      OpMemberDecorate %70 6 Offset 70 
                                                      OpMemberDecorate %70 7 Offset 70 
                                                      OpMemberDecorate %70 8 Offset 70 
                                                      OpMemberDecorate %70 9 Offset 70 
                                                      OpDecorate %70 Block 
                                                      OpDecorate %72 DescriptorSet 72 
                                                      OpDecorate %72 Binding 72 
                                                      OpMemberDecorate %97 0 BuiltIn 97 
                                                      OpMemberDecorate %97 1 BuiltIn 97 
                                                      OpMemberDecorate %97 2 BuiltIn 97 
                                                      OpDecorate %97 Block 
                                                      OpDecorate %111 Location 111 
                                                      OpDecorate %112 Location 112 
                                                      OpDecorate %118 Location 118 
                                                      OpDecorate %119 ArrayStride 119 
                                                      OpMemberDecorate %120 0 Offset 120 
                                                      OpDecorate %120 Block 
                                                      OpDecorate %122 DescriptorSet 122 
                                                      OpDecorate %122 Binding 122 
                                               %2 = OpTypeVoid 
                                               %3 = OpTypeFunction %2 
                                               %6 = OpTypeFloat 32 
                                               %7 = OpTypeVector %6 4 
                                               %8 = OpTypePointer Private %7 
                                Private f32_4* %9 = OpVariable Private 
                                              %10 = OpTypePointer Input %7 
                                 Input f32_4* %11 = OpVariable Input 
                                              %14 = OpTypeInt 32 0 
                                          u32 %15 = OpConstant 4 
                                              %16 = OpTypeArray %7 %15 
                                              %17 = OpTypeStruct %16 
                                              %18 = OpTypePointer Uniform %17 
                  Uniform struct {f32_4[4];}* %19 = OpVariable Uniform 
                                              %20 = OpTypeInt 32 1 
                                          i32 %21 = OpConstant 0 
                                          i32 %22 = OpConstant 1 
                                              %23 = OpTypePointer Uniform %7 
                                          i32 %34 = OpConstant 2 
                                          i32 %43 = OpConstant 3 
                                              %47 = OpTypePointer Private %20 
                                 Private i32* %48 = OpVariable Private 
                                              %49 = OpTypePointer Input %14 
                                   Input u32* %50 = OpVariable Input 
                               Private f32_4* %54 = OpVariable Private 
                                          u32 %57 = OpConstant 8 
                                              %58 = OpTypeArray %7 %57 
                                              %59 = OpTypeArray %7 %57 
                                              %60 = OpTypeArray %7 %57 
                                              %61 = OpTypeArray %7 %57 
                                              %62 = OpTypeArray %7 %57 
                                              %63 = OpTypeArray %7 %57 
                                              %64 = OpTypeArray %7 %57 
                                              %65 = OpTypeArray %7 %57 
                                              %66 = OpTypeVector %6 3 
                                          u32 %67 = OpConstant 2 
                                              %68 = OpTypeArray %66 %67 
                                              %69 = OpTypeArray %7 %67 
                                              %70 = OpTypeStruct %58 %59 %60 %61 %62 %63 %64 %65 %68 %69 
                                              %71 = OpTypePointer Uniform %70 
Uniform struct {f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_3[2]; f32_4[2];}* %72 = OpVariable Uniform 
                                          u32 %95 = OpConstant 1 
                                              %96 = OpTypeArray %6 %95 
                                              %97 = OpTypeStruct %7 %6 %96 
                                              %98 = OpTypePointer Output %97 
         Output struct {f32_4; f32; f32[1];}* %99 = OpVariable Output 
                                             %109 = OpTypePointer Output %7 
                               Output f32_4* %111 = OpVariable Output 
                                Input f32_4* %112 = OpVariable Input 
                                             %114 = OpTypePointer Private %14 
                                Private u32* %115 = OpVariable Private 
                                             %117 = OpTypePointer Output %6 
                                 Output f32* %118 = OpVariable Output 
                                             %119 = OpTypeArray %7 %67 
                                             %120 = OpTypeStruct %119 
                                             %121 = OpTypePointer Uniform %120 
                 Uniform struct {f32_4[2];}* %122 = OpVariable Uniform 
                                         u32 %125 = OpConstant 0 
                                             %126 = OpTypePointer Uniform %6 
                                          void %4 = OpFunction None %3 
                                               %5 = OpLabel 
                                        f32_4 %12 = OpLoad %11 
                                        f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                               Uniform f32_4* %24 = OpAccessChain %19 %21 %22 
                                        f32_4 %25 = OpLoad %24 
                                        f32_4 %26 = OpFMul %13 %25 
                                                      OpStore %9 %26 
                               Uniform f32_4* %27 = OpAccessChain %19 %21 %21 
                                        f32_4 %28 = OpLoad %27 
                                        f32_4 %29 = OpLoad %11 
                                        f32_4 %30 = OpVectorShuffle %29 %29 0 0 0 0 
                                        f32_4 %31 = OpFMul %28 %30 
                                        f32_4 %32 = OpLoad %9 
                                        f32_4 %33 = OpFAdd %31 %32 
                                                      OpStore %9 %33 
                               Uniform f32_4* %35 = OpAccessChain %19 %21 %34 
                                        f32_4 %36 = OpLoad %35 
                                        f32_4 %37 = OpLoad %11 
                                        f32_4 %38 = OpVectorShuffle %37 %37 2 2 2 2 
                                        f32_4 %39 = OpFMul %36 %38 
                                        f32_4 %40 = OpLoad %9 
                                        f32_4 %41 = OpFAdd %39 %40 
                                                      OpStore %9 %41 
                                        f32_4 %42 = OpLoad %9 
                               Uniform f32_4* %44 = OpAccessChain %19 %21 %43 
                                        f32_4 %45 = OpLoad %44 
                                        f32_4 %46 = OpFAdd %42 %45 
                                                      OpStore %9 %46 
                                          u32 %51 = OpLoad %50 
                                          i32 %52 = OpBitcast %51 
                                          i32 %53 = OpShiftLeftLogical %52 %34 
                                                      OpStore %48 %53 
                                        f32_4 %55 = OpLoad %9 
                                        f32_4 %56 = OpVectorShuffle %55 %55 1 1 1 1 
                                          i32 %73 = OpLoad %48 
                                          i32 %74 = OpIAdd %73 %22 
                               Uniform f32_4* %75 = OpAccessChain %72 %43 %74 
                                        f32_4 %76 = OpLoad %75 
                                        f32_4 %77 = OpFMul %56 %76 
                                                      OpStore %54 %77 
                                          i32 %78 = OpLoad %48 
                               Uniform f32_4* %79 = OpAccessChain %72 %43 %78 
                                        f32_4 %80 = OpLoad %79 
                                        f32_4 %81 = OpLoad %9 
                                        f32_4 %82 = OpVectorShuffle %81 %81 0 0 0 0 
                                        f32_4 %83 = OpFMul %80 %82 
                                        f32_4 %84 = OpLoad %54 
                                        f32_4 %85 = OpFAdd %83 %84 
                                                      OpStore %54 %85 
                                          i32 %86 = OpLoad %48 
                                          i32 %87 = OpIAdd %86 %34 
                               Uniform f32_4* %88 = OpAccessChain %72 %43 %87 
                                        f32_4 %89 = OpLoad %88 
                                        f32_4 %90 = OpLoad %9 
                                        f32_4 %91 = OpVectorShuffle %90 %90 2 2 2 2 
                                        f32_4 %92 = OpFMul %89 %91 
                                        f32_4 %93 = OpLoad %54 
                                        f32_4 %94 = OpFAdd %92 %93 
                                                      OpStore %54 %94 
                                         i32 %100 = OpLoad %48 
                                         i32 %101 = OpIAdd %100 %43 
                              Uniform f32_4* %102 = OpAccessChain %72 %43 %101 
                                       f32_4 %103 = OpLoad %102 
                                       f32_4 %104 = OpLoad %9 
                                       f32_4 %105 = OpVectorShuffle %104 %104 3 3 3 3 
                                       f32_4 %106 = OpFMul %103 %105 
                                       f32_4 %107 = OpLoad %54 
                                       f32_4 %108 = OpFAdd %106 %107 
                               Output f32_4* %110 = OpAccessChain %99 %21 
                                                      OpStore %110 %108 
                                       f32_4 %113 = OpLoad %112 
                                                      OpStore %111 %113 
                                         u32 %116 = OpLoad %50 
                                                      OpStore %115 %116 
                                         u32 %123 = OpLoad %115 
                                         i32 %124 = OpBitcast %123 
                                Uniform f32* %127 = OpAccessChain %122 %21 %124 %125 
                                         f32 %128 = OpLoad %127 
                                                      OpStore %118 %128 
                                 Output f32* %129 = OpAccessChain %99 %21 %95 
                                         f32 %130 = OpLoad %129 
                                         f32 %131 = OpFNegate %130 
                                 Output f32* %132 = OpAccessChain %99 %21 %95 
                                                      OpStore %132 %131 
                                                      OpReturn
                                                      OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
""
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
""
}
SubProgram "vulkan " {
""
}
SubProgram "gles " {
""
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
}
}
 Pass {
  ColorMask 0 0
  ZTest Always
  Cull Off
  Stencil {
   Comp Always
   Pass Zero
   Fail Keep
   ZFail Keep
  }
  GpuProgramID 446987
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 91
; Schema: 0
                                              OpCapability Shader 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %72 %82 %83 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %17 ArrayStride 17 
                                              OpMemberDecorate %18 0 Offset 18 
                                              OpMemberDecorate %18 1 Offset 18 
                                              OpDecorate %18 Block 
                                              OpDecorate %20 DescriptorSet 20 
                                              OpDecorate %20 Binding 20 
                                              OpMemberDecorate %70 0 BuiltIn 70 
                                              OpMemberDecorate %70 1 BuiltIn 70 
                                              OpMemberDecorate %70 2 BuiltIn 70 
                                              OpDecorate %70 Block 
                                              OpDecorate %82 Location 82 
                                              OpDecorate %83 Location 83 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                      %17 = OpTypeArray %7 %15 
                                      %18 = OpTypeStruct %16 %17 
                                      %19 = OpTypePointer Uniform %18 
Uniform struct {f32_4[4]; f32_4[4];}* %20 = OpVariable Uniform 
                                      %21 = OpTypeInt 32 1 
                                  i32 %22 = OpConstant 0 
                                  i32 %23 = OpConstant 1 
                                      %24 = OpTypePointer Uniform %7 
                                  i32 %35 = OpConstant 2 
                                  i32 %44 = OpConstant 3 
                       Private f32_4* %48 = OpVariable Private 
                                  u32 %68 = OpConstant 1 
                                      %69 = OpTypeArray %6 %68 
                                      %70 = OpTypeStruct %7 %6 %69 
                                      %71 = OpTypePointer Output %70 
 Output struct {f32_4; f32; f32[1];}* %72 = OpVariable Output 
                                      %80 = OpTypePointer Output %7 
                        Output f32_4* %82 = OpVariable Output 
                         Input f32_4* %83 = OpVariable Input 
                                      %85 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %25 = OpAccessChain %20 %22 %23 
                                f32_4 %26 = OpLoad %25 
                                f32_4 %27 = OpFMul %13 %26 
                                              OpStore %9 %27 
                       Uniform f32_4* %28 = OpAccessChain %20 %22 %22 
                                f32_4 %29 = OpLoad %28 
                                f32_4 %30 = OpLoad %11 
                                f32_4 %31 = OpVectorShuffle %30 %30 0 0 0 0 
                                f32_4 %32 = OpFMul %29 %31 
                                f32_4 %33 = OpLoad %9 
                                f32_4 %34 = OpFAdd %32 %33 
                                              OpStore %9 %34 
                       Uniform f32_4* %36 = OpAccessChain %20 %22 %35 
                                f32_4 %37 = OpLoad %36 
                                f32_4 %38 = OpLoad %11 
                                f32_4 %39 = OpVectorShuffle %38 %38 2 2 2 2 
                                f32_4 %40 = OpFMul %37 %39 
                                f32_4 %41 = OpLoad %9 
                                f32_4 %42 = OpFAdd %40 %41 
                                              OpStore %9 %42 
                                f32_4 %43 = OpLoad %9 
                       Uniform f32_4* %45 = OpAccessChain %20 %22 %44 
                                f32_4 %46 = OpLoad %45 
                                f32_4 %47 = OpFAdd %43 %46 
                                              OpStore %9 %47 
                                f32_4 %49 = OpLoad %9 
                                f32_4 %50 = OpVectorShuffle %49 %49 1 1 1 1 
                       Uniform f32_4* %51 = OpAccessChain %20 %23 %23 
                                f32_4 %52 = OpLoad %51 
                                f32_4 %53 = OpFMul %50 %52 
                                              OpStore %48 %53 
                       Uniform f32_4* %54 = OpAccessChain %20 %23 %22 
                                f32_4 %55 = OpLoad %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 0 0 0 0 
                                f32_4 %58 = OpFMul %55 %57 
                                f32_4 %59 = OpLoad %48 
                                f32_4 %60 = OpFAdd %58 %59 
                                              OpStore %48 %60 
                       Uniform f32_4* %61 = OpAccessChain %20 %23 %35 
                                f32_4 %62 = OpLoad %61 
                                f32_4 %63 = OpLoad %9 
                                f32_4 %64 = OpVectorShuffle %63 %63 2 2 2 2 
                                f32_4 %65 = OpFMul %62 %64 
                                f32_4 %66 = OpLoad %48 
                                f32_4 %67 = OpFAdd %65 %66 
                                              OpStore %48 %67 
                       Uniform f32_4* %73 = OpAccessChain %20 %23 %44 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 3 3 3 3 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %48 
                                f32_4 %79 = OpFAdd %77 %78 
                        Output f32_4* %81 = OpAccessChain %72 %22 
                                              OpStore %81 %79 
                                f32_4 %84 = OpLoad %83 
                                              OpStore %82 %84 
                          Output f32* %86 = OpAccessChain %72 %22 %68 
                                  f32 %87 = OpLoad %86 
                                  f32 %88 = OpFNegate %87 
                          Output f32* %89 = OpAccessChain %72 %22 %68 
                                              OpStore %89 %88 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
uniform 	int unity_StereoEyeIndex;
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
int op_shl(int a, int b) { return int(floor(float(a) * pow(2.0, float(b)))); }
ivec2 op_shl(ivec2 a, ivec2 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); return a; }
ivec3 op_shl(ivec3 a, ivec3 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); return a; }
ivec4 op_shl(ivec4 a, ivec4 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); a.w = op_shl(a.w, b.w); return a; }

void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = op_shl(unity_StereoEyeIndex, 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
#endif
	UNITY_UNIFORM int unity_StereoEyeIndex;
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(unity_StereoEyeIndex << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
};

struct UnityStereoGlobals_Type
{
    float4 hlslcc_mtx4x4unity_StereoMatrixP[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
    float4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
    float4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
    float4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
    float4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
    float3 unity_StereoWorldSpaceCameraPos[2];
    float4 unity_StereoScaleOffset[2];
};

struct UnityStereoEyeIndex_Type
{
    int unity_StereoEyeIndex;
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    constant UnityStereoGlobals_Type& UnityStereoGlobals [[ buffer(1) ]],
    constant UnityStereoEyeIndex_Type& UnityStereoEyeIndex [[ buffer(2) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    int u_xlati1;
    float4 u_xlat2;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = UnityStereoEyeIndex.unity_StereoEyeIndex << 0x2;
    u_xlat2 = u_xlat0.yyyy * UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1], u_xlat0.xxxx, u_xlat2);
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)], u_xlat0.zzzz, u_xlat2);
    output.mtl_Position = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)], u_xlat0.wwww, u_xlat2);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
	int unity_StereoEyeIndex;
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = unity_StereoEyeIndex << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 105
; Schema: 0
                                                   OpCapability Shader 
                                            %1 = OpExtInstImport "GLSL.std.450" 
                                                   OpMemoryModel Logical GLSL450 
                                                   OpEntryPoint Vertex %4 "main" %11 %84 %96 %97 
                                                   OpDecorate %11 Location 11 
                                                   OpDecorate %16 ArrayStride 16 
                                                   OpDecorate %18 ArrayStride 18 
                                                   OpMemberDecorate %20 0 Offset 20 
                                                   OpMemberDecorate %20 1 Offset 20 
                                                   OpMemberDecorate %20 2 Offset 20 
                                                   OpDecorate %20 Block 
                                                   OpDecorate %22 DescriptorSet 22 
                                                   OpDecorate %22 Binding 22 
                                                   OpMemberDecorate %82 0 BuiltIn 82 
                                                   OpMemberDecorate %82 1 BuiltIn 82 
                                                   OpMemberDecorate %82 2 BuiltIn 82 
                                                   OpDecorate %82 Block 
                                                   OpDecorate %96 Location 96 
                                                   OpDecorate %97 Location 97 
                                            %2 = OpTypeVoid 
                                            %3 = OpTypeFunction %2 
                                            %6 = OpTypeFloat 32 
                                            %7 = OpTypeVector %6 4 
                                            %8 = OpTypePointer Private %7 
                             Private f32_4* %9 = OpVariable Private 
                                           %10 = OpTypePointer Input %7 
                              Input f32_4* %11 = OpVariable Input 
                                           %14 = OpTypeInt 32 0 
                                       u32 %15 = OpConstant 4 
                                           %16 = OpTypeArray %7 %15 
                                       u32 %17 = OpConstant 8 
                                           %18 = OpTypeArray %7 %17 
                                           %19 = OpTypeInt 32 1 
                                           %20 = OpTypeStruct %16 %18 %19 
                                           %21 = OpTypePointer Uniform %20 
Uniform struct {f32_4[4]; f32_4[8]; i32;}* %22 = OpVariable Uniform 
                                       i32 %23 = OpConstant 0 
                                       i32 %24 = OpConstant 1 
                                           %25 = OpTypePointer Uniform %7 
                                       i32 %36 = OpConstant 2 
                                       i32 %45 = OpConstant 3 
                                           %49 = OpTypePointer Private %19 
                              Private i32* %50 = OpVariable Private 
                                           %51 = OpTypePointer Uniform %19 
                            Private f32_4* %55 = OpVariable Private 
                                       u32 %80 = OpConstant 1 
                                           %81 = OpTypeArray %6 %80 
                                           %82 = OpTypeStruct %7 %6 %81 
                                           %83 = OpTypePointer Output %82 
      Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                           %94 = OpTypePointer Output %7 
                             Output f32_4* %96 = OpVariable Output 
                              Input f32_4* %97 = OpVariable Input 
                                           %99 = OpTypePointer Output %6 
                                       void %4 = OpFunction None %3 
                                            %5 = OpLabel 
                                     f32_4 %12 = OpLoad %11 
                                     f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                            Uniform f32_4* %26 = OpAccessChain %22 %23 %24 
                                     f32_4 %27 = OpLoad %26 
                                     f32_4 %28 = OpFMul %13 %27 
                                                   OpStore %9 %28 
                            Uniform f32_4* %29 = OpAccessChain %22 %23 %23 
                                     f32_4 %30 = OpLoad %29 
                                     f32_4 %31 = OpLoad %11 
                                     f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                     f32_4 %33 = OpFMul %30 %32 
                                     f32_4 %34 = OpLoad %9 
                                     f32_4 %35 = OpFAdd %33 %34 
                                                   OpStore %9 %35 
                            Uniform f32_4* %37 = OpAccessChain %22 %23 %36 
                                     f32_4 %38 = OpLoad %37 
                                     f32_4 %39 = OpLoad %11 
                                     f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                     f32_4 %41 = OpFMul %38 %40 
                                     f32_4 %42 = OpLoad %9 
                                     f32_4 %43 = OpFAdd %41 %42 
                                                   OpStore %9 %43 
                                     f32_4 %44 = OpLoad %9 
                            Uniform f32_4* %46 = OpAccessChain %22 %23 %45 
                                     f32_4 %47 = OpLoad %46 
                                     f32_4 %48 = OpFAdd %44 %47 
                                                   OpStore %9 %48 
                              Uniform i32* %52 = OpAccessChain %22 %36 
                                       i32 %53 = OpLoad %52 
                                       i32 %54 = OpShiftLeftLogical %53 %36 
                                                   OpStore %50 %54 
                                     f32_4 %56 = OpLoad %9 
                                     f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                       i32 %58 = OpLoad %50 
                                       i32 %59 = OpIAdd %58 %24 
                            Uniform f32_4* %60 = OpAccessChain %22 %24 %59 
                                     f32_4 %61 = OpLoad %60 
                                     f32_4 %62 = OpFMul %57 %61 
                                                   OpStore %55 %62 
                                       i32 %63 = OpLoad %50 
                            Uniform f32_4* %64 = OpAccessChain %22 %24 %63 
                                     f32_4 %65 = OpLoad %64 
                                     f32_4 %66 = OpLoad %9 
                                     f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                     f32_4 %68 = OpFMul %65 %67 
                                     f32_4 %69 = OpLoad %55 
                                     f32_4 %70 = OpFAdd %68 %69 
                                                   OpStore %55 %70 
                                       i32 %71 = OpLoad %50 
                                       i32 %72 = OpIAdd %71 %36 
                            Uniform f32_4* %73 = OpAccessChain %22 %24 %72 
                                     f32_4 %74 = OpLoad %73 
                                     f32_4 %75 = OpLoad %9 
                                     f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                     f32_4 %77 = OpFMul %74 %76 
                                     f32_4 %78 = OpLoad %55 
                                     f32_4 %79 = OpFAdd %77 %78 
                                                   OpStore %55 %79 
                                       i32 %85 = OpLoad %50 
                                       i32 %86 = OpIAdd %85 %45 
                            Uniform f32_4* %87 = OpAccessChain %22 %24 %86 
                                     f32_4 %88 = OpLoad %87 
                                     f32_4 %89 = OpLoad %9 
                                     f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                     f32_4 %91 = OpFMul %88 %90 
                                     f32_4 %92 = OpLoad %55 
                                     f32_4 %93 = OpFAdd %91 %92 
                             Output f32_4* %95 = OpAccessChain %84 %23 
                                                   OpStore %95 %93 
                                     f32_4 %98 = OpLoad %97 
                                                   OpStore %96 %98 
                              Output f32* %100 = OpAccessChain %84 %23 %80 
                                      f32 %101 = OpLoad %100 
                                      f32 %102 = OpFNegate %101 
                              Output f32* %103 = OpAccessChain %84 %23 %80 
                                                   OpStore %103 %102 
                                                   OpReturn
                                                   OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
flat out highp uint vs_BLENDINDICES0;
float u_xlat0;
uint u_xlatu0;
vec4 u_xlat1;
vec4 u_xlat2;
int u_xlati3;
void main()
{
    u_xlat0 = float(uint(gl_InstanceID));
    u_xlat0 = u_xlat0 * 0.5;
    u_xlat0 = fract(u_xlat0);
    u_xlat0 = u_xlat0 + u_xlat0;
    u_xlat0 = roundEven(u_xlat0);
    u_xlatu0 = uint(u_xlat0);
    u_xlati3 = int(int(u_xlatu0) << 2);
    u_xlat1 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
    u_xlat1 = u_xlat1 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat2 = u_xlat1.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati3] * u_xlat1.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 2)] * u_xlat1.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 3)] * u_xlat1.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu0;
    gl_Layer = int(u_xlatu0);
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
flat out uint vs_BLENDINDICES0;
vec4 u_xlat0;
uint u_xlatu1;
vec4 u_xlat2;
int u_xlati4;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlatu1 = uint(gl_InstanceID) & 1u;
    u_xlati4 = int(u_xlatu1) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati4] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu1;
    gl_Layer = int(u_xlatu1);
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 111
; Schema: 0
                                              OpCapability Shader 
                                              OpCapability Geometry 
                                              OpCapability ShaderViewportIndexLayerEXT 
                                              OpExtension "SPV_EXT_shader_viewport_index_layer" 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %52 %84 %96 %97 %100 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %18 ArrayStride 18 
                                              OpMemberDecorate %19 0 Offset 19 
                                              OpMemberDecorate %19 1 Offset 19 
                                              OpDecorate %19 Block 
                                              OpDecorate %21 DescriptorSet 21 
                                              OpDecorate %21 Binding 21 
                                              OpDecorate %52 BuiltIn 52 
                                              OpMemberDecorate %82 0 BuiltIn 82 
                                              OpMemberDecorate %82 1 BuiltIn 82 
                                              OpMemberDecorate %82 2 BuiltIn 82 
                                              OpDecorate %82 Block 
                                              OpDecorate %96 Location 96 
                                              OpDecorate %97 Location 97 
                                              OpDecorate %100 BuiltIn 100 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                  u32 %17 = OpConstant 8 
                                      %18 = OpTypeArray %7 %17 
                                      %19 = OpTypeStruct %16 %18 
                                      %20 = OpTypePointer Uniform %19 
Uniform struct {f32_4[4]; f32_4[8];}* %21 = OpVariable Uniform 
                                      %22 = OpTypeInt 32 1 
                                  i32 %23 = OpConstant 0 
                                  i32 %24 = OpConstant 1 
                                      %25 = OpTypePointer Uniform %7 
                                  i32 %36 = OpConstant 2 
                                  i32 %45 = OpConstant 3 
                                      %49 = OpTypePointer Private %22 
                         Private i32* %50 = OpVariable Private 
                                      %51 = OpTypePointer Input %22 
                           Input i32* %52 = OpVariable Input 
                       Private f32_4* %55 = OpVariable Private 
                                  u32 %80 = OpConstant 1 
                                      %81 = OpTypeArray %6 %80 
                                      %82 = OpTypeStruct %7 %6 %81 
                                      %83 = OpTypePointer Output %82 
 Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                      %94 = OpTypePointer Output %7 
                        Output f32_4* %96 = OpVariable Output 
                         Input f32_4* %97 = OpVariable Input 
                                      %99 = OpTypePointer Output %22 
                         Output i32* %100 = OpVariable Output 
                                     %105 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %26 = OpAccessChain %21 %23 %24 
                                f32_4 %27 = OpLoad %26 
                                f32_4 %28 = OpFMul %13 %27 
                                              OpStore %9 %28 
                       Uniform f32_4* %29 = OpAccessChain %21 %23 %23 
                                f32_4 %30 = OpLoad %29 
                                f32_4 %31 = OpLoad %11 
                                f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                f32_4 %33 = OpFMul %30 %32 
                                f32_4 %34 = OpLoad %9 
                                f32_4 %35 = OpFAdd %33 %34 
                                              OpStore %9 %35 
                       Uniform f32_4* %37 = OpAccessChain %21 %23 %36 
                                f32_4 %38 = OpLoad %37 
                                f32_4 %39 = OpLoad %11 
                                f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                f32_4 %41 = OpFMul %38 %40 
                                f32_4 %42 = OpLoad %9 
                                f32_4 %43 = OpFAdd %41 %42 
                                              OpStore %9 %43 
                                f32_4 %44 = OpLoad %9 
                       Uniform f32_4* %46 = OpAccessChain %21 %23 %45 
                                f32_4 %47 = OpLoad %46 
                                f32_4 %48 = OpFAdd %44 %47 
                                              OpStore %9 %48 
                                  i32 %53 = OpLoad %52 
                                  i32 %54 = OpBitFieldInsert %23 %53 %36 %24 
                                              OpStore %50 %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                  i32 %58 = OpLoad %50 
                                  i32 %59 = OpIAdd %58 %24 
                       Uniform f32_4* %60 = OpAccessChain %21 %24 %59 
                                f32_4 %61 = OpLoad %60 
                                f32_4 %62 = OpFMul %57 %61 
                                              OpStore %55 %62 
                                  i32 %63 = OpLoad %50 
                       Uniform f32_4* %64 = OpAccessChain %21 %24 %63 
                                f32_4 %65 = OpLoad %64 
                                f32_4 %66 = OpLoad %9 
                                f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                f32_4 %68 = OpFMul %65 %67 
                                f32_4 %69 = OpLoad %55 
                                f32_4 %70 = OpFAdd %68 %69 
                                              OpStore %55 %70 
                                  i32 %71 = OpLoad %50 
                                  i32 %72 = OpIAdd %71 %36 
                       Uniform f32_4* %73 = OpAccessChain %21 %24 %72 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %55 
                                f32_4 %79 = OpFAdd %77 %78 
                                              OpStore %55 %79 
                                  i32 %85 = OpLoad %50 
                                  i32 %86 = OpIAdd %85 %45 
                       Uniform f32_4* %87 = OpAccessChain %21 %24 %86 
                                f32_4 %88 = OpLoad %87 
                                f32_4 %89 = OpLoad %9 
                                f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                f32_4 %91 = OpFMul %88 %90 
                                f32_4 %92 = OpLoad %55 
                                f32_4 %93 = OpFAdd %91 %92 
                        Output f32_4* %95 = OpAccessChain %84 %23 
                                              OpStore %95 %93 
                                f32_4 %98 = OpLoad %97 
                                              OpStore %96 %98 
                                 i32 %101 = OpLoad %52 
                                 u32 %102 = OpBitcast %101 
                                 u32 %103 = OpBitwiseAnd %102 %80 
                                 i32 %104 = OpBitcast %103 
                                              OpStore %100 %104 
                         Output f32* %106 = OpAccessChain %84 %23 %80 
                                 f32 %107 = OpLoad %106 
                                 f32 %108 = OpFNegate %107 
                         Output f32* %109 = OpAccessChain %84 %23 %80 
                                              OpStore %109 %108 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
#endif
	UNITY_UNIFORM vec4 unity_StereoEyeIndices[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
layout(num_views = 2) in;
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
out highp float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(int(gl_ViewID_OVR) << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
	vec4 unity_StereoEyeIndices[2];
};
layout(num_views = 2) in;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
out float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(gl_ViewID_OVR) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 134
; Schema: 0
                                                      OpCapability Shader 
                                                      OpCapability MultiView 
                                                      OpExtension "SPV_KHR_multiview" 
                                               %1 = OpExtInstImport "GLSL.std.450" 
                                                      OpMemoryModel Logical GLSL450 
                                                      OpEntryPoint Vertex %4 "main" %11 %50 %99 %111 %112 %118 
                                                      OpDecorate %11 Location 11 
                                                      OpDecorate %16 ArrayStride 16 
                                                      OpMemberDecorate %17 0 Offset 17 
                                                      OpDecorate %17 Block 
                                                      OpDecorate %19 DescriptorSet 19 
                                                      OpDecorate %19 Binding 19 
                                                      OpDecorate %50 BuiltIn 50 
                                                      OpDecorate %58 ArrayStride 58 
                                                      OpDecorate %59 ArrayStride 59 
                                                      OpDecorate %60 ArrayStride 60 
                                                      OpDecorate %61 ArrayStride 61 
                                                      OpDecorate %62 ArrayStride 62 
                                                      OpDecorate %63 ArrayStride 63 
                                                      OpDecorate %64 ArrayStride 64 
                                                      OpDecorate %65 ArrayStride 65 
                                                      OpDecorate %68 ArrayStride 68 
                                                      OpDecorate %69 ArrayStride 69 
                                                      OpMemberDecorate %70 0 Offset 70 
                                                      OpMemberDecorate %70 1 Offset 70 
                                                      OpMemberDecorate %70 2 Offset 70 
                                                      OpMemberDecorate %70 3 Offset 70 
                                                      OpMemberDecorate %70 4 Offset 70 
                                                      OpMemberDecorate %70 5 Offset 70 
                                                      OpMemberDecorate %70 6 Offset 70 
                                                      OpMemberDecorate %70 7 Offset 70 
                                                      OpMemberDecorate %70 8 Offset 70 
                                                      OpMemberDecorate %70 9 Offset 70 
                                                      OpDecorate %70 Block 
                                                      OpDecorate %72 DescriptorSet 72 
                                                      OpDecorate %72 Binding 72 
                                                      OpMemberDecorate %97 0 BuiltIn 97 
                                                      OpMemberDecorate %97 1 BuiltIn 97 
                                                      OpMemberDecorate %97 2 BuiltIn 97 
                                                      OpDecorate %97 Block 
                                                      OpDecorate %111 Location 111 
                                                      OpDecorate %112 Location 112 
                                                      OpDecorate %118 Location 118 
                                                      OpDecorate %119 ArrayStride 119 
                                                      OpMemberDecorate %120 0 Offset 120 
                                                      OpDecorate %120 Block 
                                                      OpDecorate %122 DescriptorSet 122 
                                                      OpDecorate %122 Binding 122 
                                               %2 = OpTypeVoid 
                                               %3 = OpTypeFunction %2 
                                               %6 = OpTypeFloat 32 
                                               %7 = OpTypeVector %6 4 
                                               %8 = OpTypePointer Private %7 
                                Private f32_4* %9 = OpVariable Private 
                                              %10 = OpTypePointer Input %7 
                                 Input f32_4* %11 = OpVariable Input 
                                              %14 = OpTypeInt 32 0 
                                          u32 %15 = OpConstant 4 
                                              %16 = OpTypeArray %7 %15 
                                              %17 = OpTypeStruct %16 
                                              %18 = OpTypePointer Uniform %17 
                  Uniform struct {f32_4[4];}* %19 = OpVariable Uniform 
                                              %20 = OpTypeInt 32 1 
                                          i32 %21 = OpConstant 0 
                                          i32 %22 = OpConstant 1 
                                              %23 = OpTypePointer Uniform %7 
                                          i32 %34 = OpConstant 2 
                                          i32 %43 = OpConstant 3 
                                              %47 = OpTypePointer Private %20 
                                 Private i32* %48 = OpVariable Private 
                                              %49 = OpTypePointer Input %14 
                                   Input u32* %50 = OpVariable Input 
                               Private f32_4* %54 = OpVariable Private 
                                          u32 %57 = OpConstant 8 
                                              %58 = OpTypeArray %7 %57 
                                              %59 = OpTypeArray %7 %57 
                                              %60 = OpTypeArray %7 %57 
                                              %61 = OpTypeArray %7 %57 
                                              %62 = OpTypeArray %7 %57 
                                              %63 = OpTypeArray %7 %57 
                                              %64 = OpTypeArray %7 %57 
                                              %65 = OpTypeArray %7 %57 
                                              %66 = OpTypeVector %6 3 
                                          u32 %67 = OpConstant 2 
                                              %68 = OpTypeArray %66 %67 
                                              %69 = OpTypeArray %7 %67 
                                              %70 = OpTypeStruct %58 %59 %60 %61 %62 %63 %64 %65 %68 %69 
                                              %71 = OpTypePointer Uniform %70 
Uniform struct {f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_3[2]; f32_4[2];}* %72 = OpVariable Uniform 
                                          u32 %95 = OpConstant 1 
                                              %96 = OpTypeArray %6 %95 
                                              %97 = OpTypeStruct %7 %6 %96 
                                              %98 = OpTypePointer Output %97 
         Output struct {f32_4; f32; f32[1];}* %99 = OpVariable Output 
                                             %109 = OpTypePointer Output %7 
                               Output f32_4* %111 = OpVariable Output 
                                Input f32_4* %112 = OpVariable Input 
                                             %114 = OpTypePointer Private %14 
                                Private u32* %115 = OpVariable Private 
                                             %117 = OpTypePointer Output %6 
                                 Output f32* %118 = OpVariable Output 
                                             %119 = OpTypeArray %7 %67 
                                             %120 = OpTypeStruct %119 
                                             %121 = OpTypePointer Uniform %120 
                 Uniform struct {f32_4[2];}* %122 = OpVariable Uniform 
                                         u32 %125 = OpConstant 0 
                                             %126 = OpTypePointer Uniform %6 
                                          void %4 = OpFunction None %3 
                                               %5 = OpLabel 
                                        f32_4 %12 = OpLoad %11 
                                        f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                               Uniform f32_4* %24 = OpAccessChain %19 %21 %22 
                                        f32_4 %25 = OpLoad %24 
                                        f32_4 %26 = OpFMul %13 %25 
                                                      OpStore %9 %26 
                               Uniform f32_4* %27 = OpAccessChain %19 %21 %21 
                                        f32_4 %28 = OpLoad %27 
                                        f32_4 %29 = OpLoad %11 
                                        f32_4 %30 = OpVectorShuffle %29 %29 0 0 0 0 
                                        f32_4 %31 = OpFMul %28 %30 
                                        f32_4 %32 = OpLoad %9 
                                        f32_4 %33 = OpFAdd %31 %32 
                                                      OpStore %9 %33 
                               Uniform f32_4* %35 = OpAccessChain %19 %21 %34 
                                        f32_4 %36 = OpLoad %35 
                                        f32_4 %37 = OpLoad %11 
                                        f32_4 %38 = OpVectorShuffle %37 %37 2 2 2 2 
                                        f32_4 %39 = OpFMul %36 %38 
                                        f32_4 %40 = OpLoad %9 
                                        f32_4 %41 = OpFAdd %39 %40 
                                                      OpStore %9 %41 
                                        f32_4 %42 = OpLoad %9 
                               Uniform f32_4* %44 = OpAccessChain %19 %21 %43 
                                        f32_4 %45 = OpLoad %44 
                                        f32_4 %46 = OpFAdd %42 %45 
                                                      OpStore %9 %46 
                                          u32 %51 = OpLoad %50 
                                          i32 %52 = OpBitcast %51 
                                          i32 %53 = OpShiftLeftLogical %52 %34 
                                                      OpStore %48 %53 
                                        f32_4 %55 = OpLoad %9 
                                        f32_4 %56 = OpVectorShuffle %55 %55 1 1 1 1 
                                          i32 %73 = OpLoad %48 
                                          i32 %74 = OpIAdd %73 %22 
                               Uniform f32_4* %75 = OpAccessChain %72 %43 %74 
                                        f32_4 %76 = OpLoad %75 
                                        f32_4 %77 = OpFMul %56 %76 
                                                      OpStore %54 %77 
                                          i32 %78 = OpLoad %48 
                               Uniform f32_4* %79 = OpAccessChain %72 %43 %78 
                                        f32_4 %80 = OpLoad %79 
                                        f32_4 %81 = OpLoad %9 
                                        f32_4 %82 = OpVectorShuffle %81 %81 0 0 0 0 
                                        f32_4 %83 = OpFMul %80 %82 
                                        f32_4 %84 = OpLoad %54 
                                        f32_4 %85 = OpFAdd %83 %84 
                                                      OpStore %54 %85 
                                          i32 %86 = OpLoad %48 
                                          i32 %87 = OpIAdd %86 %34 
                               Uniform f32_4* %88 = OpAccessChain %72 %43 %87 
                                        f32_4 %89 = OpLoad %88 
                                        f32_4 %90 = OpLoad %9 
                                        f32_4 %91 = OpVectorShuffle %90 %90 2 2 2 2 
                                        f32_4 %92 = OpFMul %89 %91 
                                        f32_4 %93 = OpLoad %54 
                                        f32_4 %94 = OpFAdd %92 %93 
                                                      OpStore %54 %94 
                                         i32 %100 = OpLoad %48 
                                         i32 %101 = OpIAdd %100 %43 
                              Uniform f32_4* %102 = OpAccessChain %72 %43 %101 
                                       f32_4 %103 = OpLoad %102 
                                       f32_4 %104 = OpLoad %9 
                                       f32_4 %105 = OpVectorShuffle %104 %104 3 3 3 3 
                                       f32_4 %106 = OpFMul %103 %105 
                                       f32_4 %107 = OpLoad %54 
                                       f32_4 %108 = OpFAdd %106 %107 
                               Output f32_4* %110 = OpAccessChain %99 %21 
                                                      OpStore %110 %108 
                                       f32_4 %113 = OpLoad %112 
                                                      OpStore %111 %113 
                                         u32 %116 = OpLoad %50 
                                                      OpStore %115 %116 
                                         u32 %123 = OpLoad %115 
                                         i32 %124 = OpBitcast %123 
                                Uniform f32* %127 = OpAccessChain %122 %21 %124 %125 
                                         f32 %128 = OpLoad %127 
                                                      OpStore %118 %128 
                                 Output f32* %129 = OpAccessChain %99 %21 %95 
                                         f32 %130 = OpLoad %129 
                                         f32 %131 = OpFNegate %130 
                                 Output f32* %132 = OpAccessChain %99 %21 %95 
                                                      OpStore %132 %131 
                                                      OpReturn
                                                      OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
""
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
""
}
SubProgram "vulkan " {
""
}
SubProgram "gles " {
""
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
}
}
 Pass {
  ZTest Always
  Cull Off
  Stencil {
   Comp Always
   Pass Zero
   Fail Keep
   ZFail Keep
  }
  GpuProgramID 517744
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 91
; Schema: 0
                                              OpCapability Shader 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %72 %82 %83 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %17 ArrayStride 17 
                                              OpMemberDecorate %18 0 Offset 18 
                                              OpMemberDecorate %18 1 Offset 18 
                                              OpDecorate %18 Block 
                                              OpDecorate %20 DescriptorSet 20 
                                              OpDecorate %20 Binding 20 
                                              OpMemberDecorate %70 0 BuiltIn 70 
                                              OpMemberDecorate %70 1 BuiltIn 70 
                                              OpMemberDecorate %70 2 BuiltIn 70 
                                              OpDecorate %70 Block 
                                              OpDecorate %82 Location 82 
                                              OpDecorate %83 Location 83 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                      %17 = OpTypeArray %7 %15 
                                      %18 = OpTypeStruct %16 %17 
                                      %19 = OpTypePointer Uniform %18 
Uniform struct {f32_4[4]; f32_4[4];}* %20 = OpVariable Uniform 
                                      %21 = OpTypeInt 32 1 
                                  i32 %22 = OpConstant 0 
                                  i32 %23 = OpConstant 1 
                                      %24 = OpTypePointer Uniform %7 
                                  i32 %35 = OpConstant 2 
                                  i32 %44 = OpConstant 3 
                       Private f32_4* %48 = OpVariable Private 
                                  u32 %68 = OpConstant 1 
                                      %69 = OpTypeArray %6 %68 
                                      %70 = OpTypeStruct %7 %6 %69 
                                      %71 = OpTypePointer Output %70 
 Output struct {f32_4; f32; f32[1];}* %72 = OpVariable Output 
                                      %80 = OpTypePointer Output %7 
                        Output f32_4* %82 = OpVariable Output 
                         Input f32_4* %83 = OpVariable Input 
                                      %85 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %25 = OpAccessChain %20 %22 %23 
                                f32_4 %26 = OpLoad %25 
                                f32_4 %27 = OpFMul %13 %26 
                                              OpStore %9 %27 
                       Uniform f32_4* %28 = OpAccessChain %20 %22 %22 
                                f32_4 %29 = OpLoad %28 
                                f32_4 %30 = OpLoad %11 
                                f32_4 %31 = OpVectorShuffle %30 %30 0 0 0 0 
                                f32_4 %32 = OpFMul %29 %31 
                                f32_4 %33 = OpLoad %9 
                                f32_4 %34 = OpFAdd %32 %33 
                                              OpStore %9 %34 
                       Uniform f32_4* %36 = OpAccessChain %20 %22 %35 
                                f32_4 %37 = OpLoad %36 
                                f32_4 %38 = OpLoad %11 
                                f32_4 %39 = OpVectorShuffle %38 %38 2 2 2 2 
                                f32_4 %40 = OpFMul %37 %39 
                                f32_4 %41 = OpLoad %9 
                                f32_4 %42 = OpFAdd %40 %41 
                                              OpStore %9 %42 
                                f32_4 %43 = OpLoad %9 
                       Uniform f32_4* %45 = OpAccessChain %20 %22 %44 
                                f32_4 %46 = OpLoad %45 
                                f32_4 %47 = OpFAdd %43 %46 
                                              OpStore %9 %47 
                                f32_4 %49 = OpLoad %9 
                                f32_4 %50 = OpVectorShuffle %49 %49 1 1 1 1 
                       Uniform f32_4* %51 = OpAccessChain %20 %23 %23 
                                f32_4 %52 = OpLoad %51 
                                f32_4 %53 = OpFMul %50 %52 
                                              OpStore %48 %53 
                       Uniform f32_4* %54 = OpAccessChain %20 %23 %22 
                                f32_4 %55 = OpLoad %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 0 0 0 0 
                                f32_4 %58 = OpFMul %55 %57 
                                f32_4 %59 = OpLoad %48 
                                f32_4 %60 = OpFAdd %58 %59 
                                              OpStore %48 %60 
                       Uniform f32_4* %61 = OpAccessChain %20 %23 %35 
                                f32_4 %62 = OpLoad %61 
                                f32_4 %63 = OpLoad %9 
                                f32_4 %64 = OpVectorShuffle %63 %63 2 2 2 2 
                                f32_4 %65 = OpFMul %62 %64 
                                f32_4 %66 = OpLoad %48 
                                f32_4 %67 = OpFAdd %65 %66 
                                              OpStore %48 %67 
                       Uniform f32_4* %73 = OpAccessChain %20 %23 %44 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 3 3 3 3 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %48 
                                f32_4 %79 = OpFAdd %77 %78 
                        Output f32_4* %81 = OpAccessChain %72 %22 
                                              OpStore %81 %79 
                                f32_4 %84 = OpLoad %83 
                                              OpStore %82 %84 
                          Output f32* %86 = OpAccessChain %72 %22 %68 
                                  f32 %87 = OpLoad %86 
                                  f32 %88 = OpFNegate %87 
                          Output f32* %89 = OpAccessChain %72 %22 %68 
                                              OpStore %89 %88 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
uniform 	int unity_StereoEyeIndex;
attribute highp vec4 in_POSITION0;
attribute mediump vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
int op_shl(int a, int b) { return int(floor(float(a) * pow(2.0, float(b)))); }
ivec2 op_shl(ivec2 a, ivec2 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); return a; }
ivec3 op_shl(ivec3 a, ivec3 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); return a; }
ivec4 op_shl(ivec4 a, ivec4 b) { a.x = op_shl(a.x, b.x); a.y = op_shl(a.y, b.y); a.z = op_shl(a.z, b.z); a.w = op_shl(a.w, b.w); return a; }

void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = op_shl(unity_StereoEyeIndex, 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 100

#ifdef GL_FRAGMENT_PRECISION_HIGH
    precision highp float;
#else
    precision mediump float;
#endif
precision highp int;
varying mediump vec4 vs_COLOR0;
#define SV_Target0 gl_FragData[0]
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 300 es

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
#endif
	UNITY_UNIFORM int unity_StereoEyeIndex;
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(unity_StereoEyeIndex << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
};

struct UnityStereoGlobals_Type
{
    float4 hlslcc_mtx4x4unity_StereoMatrixP[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
    float4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
    float4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
    float4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
    float4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
    float4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
    float3 unity_StereoWorldSpaceCameraPos[2];
    float4 unity_StereoScaleOffset[2];
};

struct UnityStereoEyeIndex_Type
{
    int unity_StereoEyeIndex;
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    constant UnityStereoGlobals_Type& UnityStereoGlobals [[ buffer(1) ]],
    constant UnityStereoEyeIndex_Type& UnityStereoEyeIndex [[ buffer(2) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    int u_xlati1;
    float4 u_xlat2;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = UnityStereoEyeIndex.unity_StereoEyeIndex << 0x2;
    u_xlat2 = u_xlat0.yyyy * UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1], u_xlat0.xxxx, u_xlat2);
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)], u_xlat0.zzzz, u_xlat2);
    output.mtl_Position = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)], u_xlat0.wwww, u_xlat2);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndex {
	int unity_StereoEyeIndex;
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = unity_StereoEyeIndex << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    return;
}

#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#ifdef GL_ARB_shader_bit_encoding
#extension GL_ARB_shader_bit_encoding : enable
#endif

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 105
; Schema: 0
                                                   OpCapability Shader 
                                            %1 = OpExtInstImport "GLSL.std.450" 
                                                   OpMemoryModel Logical GLSL450 
                                                   OpEntryPoint Vertex %4 "main" %11 %84 %96 %97 
                                                   OpDecorate %11 Location 11 
                                                   OpDecorate %16 ArrayStride 16 
                                                   OpDecorate %18 ArrayStride 18 
                                                   OpMemberDecorate %20 0 Offset 20 
                                                   OpMemberDecorate %20 1 Offset 20 
                                                   OpMemberDecorate %20 2 Offset 20 
                                                   OpDecorate %20 Block 
                                                   OpDecorate %22 DescriptorSet 22 
                                                   OpDecorate %22 Binding 22 
                                                   OpMemberDecorate %82 0 BuiltIn 82 
                                                   OpMemberDecorate %82 1 BuiltIn 82 
                                                   OpMemberDecorate %82 2 BuiltIn 82 
                                                   OpDecorate %82 Block 
                                                   OpDecorate %96 Location 96 
                                                   OpDecorate %97 Location 97 
                                            %2 = OpTypeVoid 
                                            %3 = OpTypeFunction %2 
                                            %6 = OpTypeFloat 32 
                                            %7 = OpTypeVector %6 4 
                                            %8 = OpTypePointer Private %7 
                             Private f32_4* %9 = OpVariable Private 
                                           %10 = OpTypePointer Input %7 
                              Input f32_4* %11 = OpVariable Input 
                                           %14 = OpTypeInt 32 0 
                                       u32 %15 = OpConstant 4 
                                           %16 = OpTypeArray %7 %15 
                                       u32 %17 = OpConstant 8 
                                           %18 = OpTypeArray %7 %17 
                                           %19 = OpTypeInt 32 1 
                                           %20 = OpTypeStruct %16 %18 %19 
                                           %21 = OpTypePointer Uniform %20 
Uniform struct {f32_4[4]; f32_4[8]; i32;}* %22 = OpVariable Uniform 
                                       i32 %23 = OpConstant 0 
                                       i32 %24 = OpConstant 1 
                                           %25 = OpTypePointer Uniform %7 
                                       i32 %36 = OpConstant 2 
                                       i32 %45 = OpConstant 3 
                                           %49 = OpTypePointer Private %19 
                              Private i32* %50 = OpVariable Private 
                                           %51 = OpTypePointer Uniform %19 
                            Private f32_4* %55 = OpVariable Private 
                                       u32 %80 = OpConstant 1 
                                           %81 = OpTypeArray %6 %80 
                                           %82 = OpTypeStruct %7 %6 %81 
                                           %83 = OpTypePointer Output %82 
      Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                           %94 = OpTypePointer Output %7 
                             Output f32_4* %96 = OpVariable Output 
                              Input f32_4* %97 = OpVariable Input 
                                           %99 = OpTypePointer Output %6 
                                       void %4 = OpFunction None %3 
                                            %5 = OpLabel 
                                     f32_4 %12 = OpLoad %11 
                                     f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                            Uniform f32_4* %26 = OpAccessChain %22 %23 %24 
                                     f32_4 %27 = OpLoad %26 
                                     f32_4 %28 = OpFMul %13 %27 
                                                   OpStore %9 %28 
                            Uniform f32_4* %29 = OpAccessChain %22 %23 %23 
                                     f32_4 %30 = OpLoad %29 
                                     f32_4 %31 = OpLoad %11 
                                     f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                     f32_4 %33 = OpFMul %30 %32 
                                     f32_4 %34 = OpLoad %9 
                                     f32_4 %35 = OpFAdd %33 %34 
                                                   OpStore %9 %35 
                            Uniform f32_4* %37 = OpAccessChain %22 %23 %36 
                                     f32_4 %38 = OpLoad %37 
                                     f32_4 %39 = OpLoad %11 
                                     f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                     f32_4 %41 = OpFMul %38 %40 
                                     f32_4 %42 = OpLoad %9 
                                     f32_4 %43 = OpFAdd %41 %42 
                                                   OpStore %9 %43 
                                     f32_4 %44 = OpLoad %9 
                            Uniform f32_4* %46 = OpAccessChain %22 %23 %45 
                                     f32_4 %47 = OpLoad %46 
                                     f32_4 %48 = OpFAdd %44 %47 
                                                   OpStore %9 %48 
                              Uniform i32* %52 = OpAccessChain %22 %36 
                                       i32 %53 = OpLoad %52 
                                       i32 %54 = OpShiftLeftLogical %53 %36 
                                                   OpStore %50 %54 
                                     f32_4 %56 = OpLoad %9 
                                     f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                       i32 %58 = OpLoad %50 
                                       i32 %59 = OpIAdd %58 %24 
                            Uniform f32_4* %60 = OpAccessChain %22 %24 %59 
                                     f32_4 %61 = OpLoad %60 
                                     f32_4 %62 = OpFMul %57 %61 
                                                   OpStore %55 %62 
                                       i32 %63 = OpLoad %50 
                            Uniform f32_4* %64 = OpAccessChain %22 %24 %63 
                                     f32_4 %65 = OpLoad %64 
                                     f32_4 %66 = OpLoad %9 
                                     f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                     f32_4 %68 = OpFMul %65 %67 
                                     f32_4 %69 = OpLoad %55 
                                     f32_4 %70 = OpFAdd %68 %69 
                                                   OpStore %55 %70 
                                       i32 %71 = OpLoad %50 
                                       i32 %72 = OpIAdd %71 %36 
                            Uniform f32_4* %73 = OpAccessChain %22 %24 %72 
                                     f32_4 %74 = OpLoad %73 
                                     f32_4 %75 = OpLoad %9 
                                     f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                     f32_4 %77 = OpFMul %74 %76 
                                     f32_4 %78 = OpLoad %55 
                                     f32_4 %79 = OpFAdd %77 %78 
                                                   OpStore %55 %79 
                                       i32 %85 = OpLoad %50 
                                       i32 %86 = OpIAdd %85 %45 
                            Uniform f32_4* %87 = OpAccessChain %22 %24 %86 
                                     f32_4 %88 = OpLoad %87 
                                     f32_4 %89 = OpLoad %9 
                                     f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                     f32_4 %91 = OpFMul %88 %90 
                                     f32_4 %92 = OpLoad %55 
                                     f32_4 %93 = OpFAdd %91 %92 
                             Output f32_4* %95 = OpAccessChain %84 %23 
                                                   OpStore %95 %93 
                                     f32_4 %98 = OpLoad %97 
                                                   OpStore %96 %98 
                              Output f32* %100 = OpAccessChain %84 %23 %80 
                                      f32 %101 = OpLoad %100 
                                      f32 %102 = OpFNegate %101 
                              Output f32* %103 = OpAccessChain %84 %23 %80 
                                                   OpStore %103 %102 
                                                   OpReturn
                                                   OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
flat out highp uint vs_BLENDINDICES0;
float u_xlat0;
uint u_xlatu0;
vec4 u_xlat1;
vec4 u_xlat2;
int u_xlati3;
void main()
{
    u_xlat0 = float(uint(gl_InstanceID));
    u_xlat0 = u_xlat0 * 0.5;
    u_xlat0 = fract(u_xlat0);
    u_xlat0 = u_xlat0 + u_xlat0;
    u_xlat0 = roundEven(u_xlat0);
    u_xlatu0 = uint(u_xlat0);
    u_xlati3 = int(int(u_xlatu0) << 2);
    u_xlat1 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
    u_xlat1 = u_xlat1 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat2 = u_xlat1.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati3] * u_xlat1.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 2)] * u_xlat1.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 3)] * u_xlat1.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu0;
    gl_Layer = int(u_xlatu0);
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_AMD_vertex_shader_layer : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
flat out uint vs_BLENDINDICES0;
vec4 u_xlat0;
uint u_xlatu1;
vec4 u_xlat2;
int u_xlati4;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlatu1 = uint(gl_InstanceID) & 1u;
    u_xlati4 = int(u_xlatu1) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati4] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati4 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    vs_BLENDINDICES0 = u_xlatu1;
    gl_Layer = int(u_xlatu1);
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 111
; Schema: 0
                                              OpCapability Shader 
                                              OpCapability Geometry 
                                              OpCapability ShaderViewportIndexLayerEXT 
                                              OpExtension "SPV_EXT_shader_viewport_index_layer" 
                                       %1 = OpExtInstImport "GLSL.std.450" 
                                              OpMemoryModel Logical GLSL450 
                                              OpEntryPoint Vertex %4 "main" %11 %52 %84 %96 %97 %100 
                                              OpDecorate %11 Location 11 
                                              OpDecorate %16 ArrayStride 16 
                                              OpDecorate %18 ArrayStride 18 
                                              OpMemberDecorate %19 0 Offset 19 
                                              OpMemberDecorate %19 1 Offset 19 
                                              OpDecorate %19 Block 
                                              OpDecorate %21 DescriptorSet 21 
                                              OpDecorate %21 Binding 21 
                                              OpDecorate %52 BuiltIn 52 
                                              OpMemberDecorate %82 0 BuiltIn 82 
                                              OpMemberDecorate %82 1 BuiltIn 82 
                                              OpMemberDecorate %82 2 BuiltIn 82 
                                              OpDecorate %82 Block 
                                              OpDecorate %96 Location 96 
                                              OpDecorate %97 Location 97 
                                              OpDecorate %100 BuiltIn 100 
                                       %2 = OpTypeVoid 
                                       %3 = OpTypeFunction %2 
                                       %6 = OpTypeFloat 32 
                                       %7 = OpTypeVector %6 4 
                                       %8 = OpTypePointer Private %7 
                        Private f32_4* %9 = OpVariable Private 
                                      %10 = OpTypePointer Input %7 
                         Input f32_4* %11 = OpVariable Input 
                                      %14 = OpTypeInt 32 0 
                                  u32 %15 = OpConstant 4 
                                      %16 = OpTypeArray %7 %15 
                                  u32 %17 = OpConstant 8 
                                      %18 = OpTypeArray %7 %17 
                                      %19 = OpTypeStruct %16 %18 
                                      %20 = OpTypePointer Uniform %19 
Uniform struct {f32_4[4]; f32_4[8];}* %21 = OpVariable Uniform 
                                      %22 = OpTypeInt 32 1 
                                  i32 %23 = OpConstant 0 
                                  i32 %24 = OpConstant 1 
                                      %25 = OpTypePointer Uniform %7 
                                  i32 %36 = OpConstant 2 
                                  i32 %45 = OpConstant 3 
                                      %49 = OpTypePointer Private %22 
                         Private i32* %50 = OpVariable Private 
                                      %51 = OpTypePointer Input %22 
                           Input i32* %52 = OpVariable Input 
                       Private f32_4* %55 = OpVariable Private 
                                  u32 %80 = OpConstant 1 
                                      %81 = OpTypeArray %6 %80 
                                      %82 = OpTypeStruct %7 %6 %81 
                                      %83 = OpTypePointer Output %82 
 Output struct {f32_4; f32; f32[1];}* %84 = OpVariable Output 
                                      %94 = OpTypePointer Output %7 
                        Output f32_4* %96 = OpVariable Output 
                         Input f32_4* %97 = OpVariable Input 
                                      %99 = OpTypePointer Output %22 
                         Output i32* %100 = OpVariable Output 
                                     %105 = OpTypePointer Output %6 
                                  void %4 = OpFunction None %3 
                                       %5 = OpLabel 
                                f32_4 %12 = OpLoad %11 
                                f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                       Uniform f32_4* %26 = OpAccessChain %21 %23 %24 
                                f32_4 %27 = OpLoad %26 
                                f32_4 %28 = OpFMul %13 %27 
                                              OpStore %9 %28 
                       Uniform f32_4* %29 = OpAccessChain %21 %23 %23 
                                f32_4 %30 = OpLoad %29 
                                f32_4 %31 = OpLoad %11 
                                f32_4 %32 = OpVectorShuffle %31 %31 0 0 0 0 
                                f32_4 %33 = OpFMul %30 %32 
                                f32_4 %34 = OpLoad %9 
                                f32_4 %35 = OpFAdd %33 %34 
                                              OpStore %9 %35 
                       Uniform f32_4* %37 = OpAccessChain %21 %23 %36 
                                f32_4 %38 = OpLoad %37 
                                f32_4 %39 = OpLoad %11 
                                f32_4 %40 = OpVectorShuffle %39 %39 2 2 2 2 
                                f32_4 %41 = OpFMul %38 %40 
                                f32_4 %42 = OpLoad %9 
                                f32_4 %43 = OpFAdd %41 %42 
                                              OpStore %9 %43 
                                f32_4 %44 = OpLoad %9 
                       Uniform f32_4* %46 = OpAccessChain %21 %23 %45 
                                f32_4 %47 = OpLoad %46 
                                f32_4 %48 = OpFAdd %44 %47 
                                              OpStore %9 %48 
                                  i32 %53 = OpLoad %52 
                                  i32 %54 = OpBitFieldInsert %23 %53 %36 %24 
                                              OpStore %50 %54 
                                f32_4 %56 = OpLoad %9 
                                f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                                  i32 %58 = OpLoad %50 
                                  i32 %59 = OpIAdd %58 %24 
                       Uniform f32_4* %60 = OpAccessChain %21 %24 %59 
                                f32_4 %61 = OpLoad %60 
                                f32_4 %62 = OpFMul %57 %61 
                                              OpStore %55 %62 
                                  i32 %63 = OpLoad %50 
                       Uniform f32_4* %64 = OpAccessChain %21 %24 %63 
                                f32_4 %65 = OpLoad %64 
                                f32_4 %66 = OpLoad %9 
                                f32_4 %67 = OpVectorShuffle %66 %66 0 0 0 0 
                                f32_4 %68 = OpFMul %65 %67 
                                f32_4 %69 = OpLoad %55 
                                f32_4 %70 = OpFAdd %68 %69 
                                              OpStore %55 %70 
                                  i32 %71 = OpLoad %50 
                                  i32 %72 = OpIAdd %71 %36 
                       Uniform f32_4* %73 = OpAccessChain %21 %24 %72 
                                f32_4 %74 = OpLoad %73 
                                f32_4 %75 = OpLoad %9 
                                f32_4 %76 = OpVectorShuffle %75 %75 2 2 2 2 
                                f32_4 %77 = OpFMul %74 %76 
                                f32_4 %78 = OpLoad %55 
                                f32_4 %79 = OpFAdd %77 %78 
                                              OpStore %55 %79 
                                  i32 %85 = OpLoad %50 
                                  i32 %86 = OpIAdd %85 %45 
                       Uniform f32_4* %87 = OpAccessChain %21 %24 %86 
                                f32_4 %88 = OpLoad %87 
                                f32_4 %89 = OpLoad %9 
                                f32_4 %90 = OpVectorShuffle %89 %89 3 3 3 3 
                                f32_4 %91 = OpFMul %88 %90 
                                f32_4 %92 = OpLoad %55 
                                f32_4 %93 = OpFAdd %91 %92 
                        Output f32_4* %95 = OpAccessChain %84 %23 
                                              OpStore %95 %93 
                                f32_4 %98 = OpLoad %97 
                                              OpStore %96 %98 
                                 i32 %101 = OpLoad %52 
                                 u32 %102 = OpBitcast %101 
                                 u32 %103 = OpBitwiseAnd %102 %80 
                                 i32 %104 = OpBitcast %103 
                                              OpStore %100 %104 
                         Output f32* %106 = OpAccessChain %84 %23 %80 
                                 f32 %107 = OpLoad %106 
                                 f32 %108 = OpFNegate %107 
                         Output f32* %109 = OpAccessChain %84 %23 %80 
                                              OpStore %109 %108 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 300 es
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(0) uniform UnityStereoGlobals {
#endif
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	UNITY_UNIFORM vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	UNITY_UNIFORM vec3 unity_StereoWorldSpaceCameraPos[2];
	UNITY_UNIFORM vec4 unity_StereoScaleOffset[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
#endif
	UNITY_UNIFORM vec4 unity_StereoEyeIndices[2];
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
};
#endif
layout(num_views = 2) in;
in highp vec4 in_POSITION0;
in mediump vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
out highp float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(int(gl_ViewID_OVR) << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
in mediump vec4 vs_COLOR0;
layout(location = 0) out mediump vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

struct VGlobals_Type
{
    float4 hlslcc_mtx4x4unity_ObjectToWorld[4];
    float4 hlslcc_mtx4x4unity_MatrixVP[4];
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float4 COLOR0 [[ user(COLOR0) ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
    output.COLOR0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require
#extension GL_OVR_multiview2 : require

#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
#if HLSLCC_ENABLE_UNIFORM_BUFFERS
#define UNITY_UNIFORM
#else
#define UNITY_UNIFORM uniform
#endif
#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
#if UNITY_SUPPORTS_UNIFORM_LOCATION
#define UNITY_LOCATION(x) layout(location = x)
#define UNITY_BINDING(x) layout(binding = x, std140)
#else
#define UNITY_LOCATION(x)
#define UNITY_BINDING(x) layout(std140)
#endif
uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
UNITY_BINDING(0) uniform UnityStereoGlobals {
	vec4 hlslcc_mtx4x4unity_StereoMatrixP[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixInvV[8];
	vec4 hlslcc_mtx4x4unity_StereoMatrixVP[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraInvProjection[8];
	vec4 hlslcc_mtx4x4unity_StereoWorldToCamera[8];
	vec4 hlslcc_mtx4x4unity_StereoCameraToWorld[8];
	vec3 unity_StereoWorldSpaceCameraPos[2];
	vec4 unity_StereoScaleOffset[2];
};
UNITY_BINDING(1) uniform UnityStereoEyeIndices {
	vec4 unity_StereoEyeIndices[2];
};
layout(num_views = 2) in;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
out float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(gl_ViewID_OVR) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
    vs_COLOR0 = in_COLOR0;
    u_xlatu0 = gl_ViewID_OVR;
    vs_BLENDWEIGHT0 = unity_StereoEyeIndices[int(u_xlatu0)].x;
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

in  vec4 vs_COLOR0;
layout(location = 0) out vec4 SV_Target0;
void main()
{
    SV_Target0 = vs_COLOR0;
    return;
}

#endif
"
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
"; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 134
; Schema: 0
                                                      OpCapability Shader 
                                                      OpCapability MultiView 
                                                      OpExtension "SPV_KHR_multiview" 
                                               %1 = OpExtInstImport "GLSL.std.450" 
                                                      OpMemoryModel Logical GLSL450 
                                                      OpEntryPoint Vertex %4 "main" %11 %50 %99 %111 %112 %118 
                                                      OpDecorate %11 Location 11 
                                                      OpDecorate %16 ArrayStride 16 
                                                      OpMemberDecorate %17 0 Offset 17 
                                                      OpDecorate %17 Block 
                                                      OpDecorate %19 DescriptorSet 19 
                                                      OpDecorate %19 Binding 19 
                                                      OpDecorate %50 BuiltIn 50 
                                                      OpDecorate %58 ArrayStride 58 
                                                      OpDecorate %59 ArrayStride 59 
                                                      OpDecorate %60 ArrayStride 60 
                                                      OpDecorate %61 ArrayStride 61 
                                                      OpDecorate %62 ArrayStride 62 
                                                      OpDecorate %63 ArrayStride 63 
                                                      OpDecorate %64 ArrayStride 64 
                                                      OpDecorate %65 ArrayStride 65 
                                                      OpDecorate %68 ArrayStride 68 
                                                      OpDecorate %69 ArrayStride 69 
                                                      OpMemberDecorate %70 0 Offset 70 
                                                      OpMemberDecorate %70 1 Offset 70 
                                                      OpMemberDecorate %70 2 Offset 70 
                                                      OpMemberDecorate %70 3 Offset 70 
                                                      OpMemberDecorate %70 4 Offset 70 
                                                      OpMemberDecorate %70 5 Offset 70 
                                                      OpMemberDecorate %70 6 Offset 70 
                                                      OpMemberDecorate %70 7 Offset 70 
                                                      OpMemberDecorate %70 8 Offset 70 
                                                      OpMemberDecorate %70 9 Offset 70 
                                                      OpDecorate %70 Block 
                                                      OpDecorate %72 DescriptorSet 72 
                                                      OpDecorate %72 Binding 72 
                                                      OpMemberDecorate %97 0 BuiltIn 97 
                                                      OpMemberDecorate %97 1 BuiltIn 97 
                                                      OpMemberDecorate %97 2 BuiltIn 97 
                                                      OpDecorate %97 Block 
                                                      OpDecorate %111 Location 111 
                                                      OpDecorate %112 Location 112 
                                                      OpDecorate %118 Location 118 
                                                      OpDecorate %119 ArrayStride 119 
                                                      OpMemberDecorate %120 0 Offset 120 
                                                      OpDecorate %120 Block 
                                                      OpDecorate %122 DescriptorSet 122 
                                                      OpDecorate %122 Binding 122 
                                               %2 = OpTypeVoid 
                                               %3 = OpTypeFunction %2 
                                               %6 = OpTypeFloat 32 
                                               %7 = OpTypeVector %6 4 
                                               %8 = OpTypePointer Private %7 
                                Private f32_4* %9 = OpVariable Private 
                                              %10 = OpTypePointer Input %7 
                                 Input f32_4* %11 = OpVariable Input 
                                              %14 = OpTypeInt 32 0 
                                          u32 %15 = OpConstant 4 
                                              %16 = OpTypeArray %7 %15 
                                              %17 = OpTypeStruct %16 
                                              %18 = OpTypePointer Uniform %17 
                  Uniform struct {f32_4[4];}* %19 = OpVariable Uniform 
                                              %20 = OpTypeInt 32 1 
                                          i32 %21 = OpConstant 0 
                                          i32 %22 = OpConstant 1 
                                              %23 = OpTypePointer Uniform %7 
                                          i32 %34 = OpConstant 2 
                                          i32 %43 = OpConstant 3 
                                              %47 = OpTypePointer Private %20 
                                 Private i32* %48 = OpVariable Private 
                                              %49 = OpTypePointer Input %14 
                                   Input u32* %50 = OpVariable Input 
                               Private f32_4* %54 = OpVariable Private 
                                          u32 %57 = OpConstant 8 
                                              %58 = OpTypeArray %7 %57 
                                              %59 = OpTypeArray %7 %57 
                                              %60 = OpTypeArray %7 %57 
                                              %61 = OpTypeArray %7 %57 
                                              %62 = OpTypeArray %7 %57 
                                              %63 = OpTypeArray %7 %57 
                                              %64 = OpTypeArray %7 %57 
                                              %65 = OpTypeArray %7 %57 
                                              %66 = OpTypeVector %6 3 
                                          u32 %67 = OpConstant 2 
                                              %68 = OpTypeArray %66 %67 
                                              %69 = OpTypeArray %7 %67 
                                              %70 = OpTypeStruct %58 %59 %60 %61 %62 %63 %64 %65 %68 %69 
                                              %71 = OpTypePointer Uniform %70 
Uniform struct {f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_3[2]; f32_4[2];}* %72 = OpVariable Uniform 
                                          u32 %95 = OpConstant 1 
                                              %96 = OpTypeArray %6 %95 
                                              %97 = OpTypeStruct %7 %6 %96 
                                              %98 = OpTypePointer Output %97 
         Output struct {f32_4; f32; f32[1];}* %99 = OpVariable Output 
                                             %109 = OpTypePointer Output %7 
                               Output f32_4* %111 = OpVariable Output 
                                Input f32_4* %112 = OpVariable Input 
                                             %114 = OpTypePointer Private %14 
                                Private u32* %115 = OpVariable Private 
                                             %117 = OpTypePointer Output %6 
                                 Output f32* %118 = OpVariable Output 
                                             %119 = OpTypeArray %7 %67 
                                             %120 = OpTypeStruct %119 
                                             %121 = OpTypePointer Uniform %120 
                 Uniform struct {f32_4[2];}* %122 = OpVariable Uniform 
                                         u32 %125 = OpConstant 0 
                                             %126 = OpTypePointer Uniform %6 
                                          void %4 = OpFunction None %3 
                                               %5 = OpLabel 
                                        f32_4 %12 = OpLoad %11 
                                        f32_4 %13 = OpVectorShuffle %12 %12 1 1 1 1 
                               Uniform f32_4* %24 = OpAccessChain %19 %21 %22 
                                        f32_4 %25 = OpLoad %24 
                                        f32_4 %26 = OpFMul %13 %25 
                                                      OpStore %9 %26 
                               Uniform f32_4* %27 = OpAccessChain %19 %21 %21 
                                        f32_4 %28 = OpLoad %27 
                                        f32_4 %29 = OpLoad %11 
                                        f32_4 %30 = OpVectorShuffle %29 %29 0 0 0 0 
                                        f32_4 %31 = OpFMul %28 %30 
                                        f32_4 %32 = OpLoad %9 
                                        f32_4 %33 = OpFAdd %31 %32 
                                                      OpStore %9 %33 
                               Uniform f32_4* %35 = OpAccessChain %19 %21 %34 
                                        f32_4 %36 = OpLoad %35 
                                        f32_4 %37 = OpLoad %11 
                                        f32_4 %38 = OpVectorShuffle %37 %37 2 2 2 2 
                                        f32_4 %39 = OpFMul %36 %38 
                                        f32_4 %40 = OpLoad %9 
                                        f32_4 %41 = OpFAdd %39 %40 
                                                      OpStore %9 %41 
                                        f32_4 %42 = OpLoad %9 
                               Uniform f32_4* %44 = OpAccessChain %19 %21 %43 
                                        f32_4 %45 = OpLoad %44 
                                        f32_4 %46 = OpFAdd %42 %45 
                                                      OpStore %9 %46 
                                          u32 %51 = OpLoad %50 
                                          i32 %52 = OpBitcast %51 
                                          i32 %53 = OpShiftLeftLogical %52 %34 
                                                      OpStore %48 %53 
                                        f32_4 %55 = OpLoad %9 
                                        f32_4 %56 = OpVectorShuffle %55 %55 1 1 1 1 
                                          i32 %73 = OpLoad %48 
                                          i32 %74 = OpIAdd %73 %22 
                               Uniform f32_4* %75 = OpAccessChain %72 %43 %74 
                                        f32_4 %76 = OpLoad %75 
                                        f32_4 %77 = OpFMul %56 %76 
                                                      OpStore %54 %77 
                                          i32 %78 = OpLoad %48 
                               Uniform f32_4* %79 = OpAccessChain %72 %43 %78 
                                        f32_4 %80 = OpLoad %79 
                                        f32_4 %81 = OpLoad %9 
                                        f32_4 %82 = OpVectorShuffle %81 %81 0 0 0 0 
                                        f32_4 %83 = OpFMul %80 %82 
                                        f32_4 %84 = OpLoad %54 
                                        f32_4 %85 = OpFAdd %83 %84 
                                                      OpStore %54 %85 
                                          i32 %86 = OpLoad %48 
                                          i32 %87 = OpIAdd %86 %34 
                               Uniform f32_4* %88 = OpAccessChain %72 %43 %87 
                                        f32_4 %89 = OpLoad %88 
                                        f32_4 %90 = OpLoad %9 
                                        f32_4 %91 = OpVectorShuffle %90 %90 2 2 2 2 
                                        f32_4 %92 = OpFMul %89 %91 
                                        f32_4 %93 = OpLoad %54 
                                        f32_4 %94 = OpFAdd %92 %93 
                                                      OpStore %54 %94 
                                         i32 %100 = OpLoad %48 
                                         i32 %101 = OpIAdd %100 %43 
                              Uniform f32_4* %102 = OpAccessChain %72 %43 %101 
                                       f32_4 %103 = OpLoad %102 
                                       f32_4 %104 = OpLoad %9 
                                       f32_4 %105 = OpVectorShuffle %104 %104 3 3 3 3 
                                       f32_4 %106 = OpFMul %103 %105 
                                       f32_4 %107 = OpLoad %54 
                                       f32_4 %108 = OpFAdd %106 %107 
                               Output f32_4* %110 = OpAccessChain %99 %21 
                                                      OpStore %110 %108 
                                       f32_4 %113 = OpLoad %112 
                                                      OpStore %111 %113 
                                         u32 %116 = OpLoad %50 
                                                      OpStore %115 %116 
                                         u32 %123 = OpLoad %115 
                                         i32 %124 = OpBitcast %123 
                                Uniform f32* %127 = OpAccessChain %122 %21 %124 %125 
                                         f32 %128 = OpLoad %127 
                                                      OpStore %118 %128 
                                 Output f32* %129 = OpAccessChain %99 %21 %95 
                                         f32 %130 = OpLoad %129 
                                         f32 %131 = OpFNegate %130 
                                 Output f32* %132 = OpAccessChain %99 %21 %95 
                                                      OpStore %132 %131 
                                                      OpReturn
                                                      OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 14
; Schema: 0
                     OpCapability Shader 
              %1 = OpExtInstImport "GLSL.std.450" 
                     OpMemoryModel Logical GLSL450 
                     OpEntryPoint Fragment %4 "main" %9 %11 
                     OpExecutionMode %4 OriginUpperLeft 
                     OpDecorate %9 Location 9 
                     OpDecorate %11 Location 11 
              %2 = OpTypeVoid 
              %3 = OpTypeFunction %2 
              %6 = OpTypeFloat 32 
              %7 = OpTypeVector %6 4 
              %8 = OpTypePointer Output %7 
Output f32_4* %9 = OpVariable Output 
             %10 = OpTypePointer Input %7 
Input f32_4* %11 = OpVariable Input 
         void %4 = OpFunction None %3 
              %5 = OpLabel 
       f32_4 %12 = OpLoad %11 
                     OpStore %9 %12 
                     OpReturn
                     OpFunctionEnd
"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
""
}
SubProgram "metal " {
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
""
}
SubProgram "vulkan " {
""
}
SubProgram "gles " {
""
}
SubProgram "d3d11 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "metal " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "vulkan " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "gles " {
Keywords { "UNITY_SINGLE_PASS_STEREO" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_INSTANCING_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_INSTANCING_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_INSTANCING_ON" }
""
}
SubProgram "d3d11 " {
Keywords { "STEREO_MULTIVIEW_ON" }
"// shader disassembly not supported on DXBC"
}
SubProgram "gles3 " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "metal " {
Keywords { "STEREO_MULTIVIEW_ON" }
"#include <metal_stdlib>
#include <metal_texture>
using namespace metal;

#if !(__HAVE_FMA__)
#define fma(a,b,c) ((a) * (b) + (c))
#endif

#ifndef XLT_REMAP_O
	#define XLT_REMAP_O {0, 1, 2, 3, 4, 5, 6, 7}
#endif
constexpr constant uint xlt_remap_o[] = XLT_REMAP_O;
struct Mtl_FragmentIn
{
    float4 COLOR0 [[ user(COLOR0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    output.SV_Target0 = input.COLOR0;
    return output;
}
"
}
SubProgram "glcore " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
SubProgram "vulkan " {
Keywords { "STEREO_MULTIVIEW_ON" }
""
}
}
}
}
}