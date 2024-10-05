//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Hidden/Internal-Colored" {
Properties {
_Color ("Color", Color) = (1,1,1,1)
_SrcBlend ("SrcBlend", Float) = 5
_DstBlend ("DstBlend", Float) = 10
_ZWrite ("ZWrite", Float) = 1
_ZTest ("ZTest", Float) = 4
_Cull ("Cull", Float) = 0
_ZBias ("ZBias", Float) = 0
}
SubShader {
 Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
 Pass {
  Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
  Blend Zero Zero, Zero Zero
  ZTest Off
  ZWrite Off
  Cull Off
  GpuProgramID 18665
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
uniform 	vec4 _Color;
attribute highp vec4 in_POSITION0;
attribute highp vec4 in_COLOR0;
varying mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_COLOR0 * _Color;
    vs_COLOR0 = u_xlat0;
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
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
uniform 	vec4 _Color;
in highp vec4 in_POSITION0;
in highp vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    u_xlat0 = in_COLOR0 * _Color;
    vs_COLOR0 = u_xlat0;
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
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
    float4 _Color;
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 COLOR0 [[ user(COLOR0) ]];
    float4 mtl_Position [[ position ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    output.COLOR0 = input.COLOR0 * VGlobals._Color;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
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
uniform 	vec4 _Color;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
out vec4 vs_COLOR0;
vec4 u_xlat0;
vec4 u_xlat1;
void main()
{
    vs_COLOR0 = in_COLOR0 * _Color;
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
    gl_Position = hlslcc_mtx4x4unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
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
; Bound: 94
; Schema: 0
                                                     OpCapability Shader 
                                              %1 = OpExtInstImport "GLSL.std.450" 
                                                     OpMemoryModel Logical GLSL450 
                                                     OpEntryPoint Vertex %4 "main" %9 %11 %28 %79 
                                                     OpDecorate %9 Location 9 
                                                     OpDecorate %11 Location 11 
                                                     OpDecorate %15 ArrayStride 15 
                                                     OpDecorate %16 ArrayStride 16 
                                                     OpMemberDecorate %17 0 Offset 17 
                                                     OpMemberDecorate %17 1 Offset 17 
                                                     OpMemberDecorate %17 2 Offset 17 
                                                     OpDecorate %17 Block 
                                                     OpDecorate %19 DescriptorSet 19 
                                                     OpDecorate %19 Binding 19 
                                                     OpDecorate %28 Location 28 
                                                     OpMemberDecorate %77 0 BuiltIn 77 
                                                     OpMemberDecorate %77 1 BuiltIn 77 
                                                     OpMemberDecorate %77 2 BuiltIn 77 
                                                     OpDecorate %77 Block 
                                              %2 = OpTypeVoid 
                                              %3 = OpTypeFunction %2 
                                              %6 = OpTypeFloat 32 
                                              %7 = OpTypeVector %6 4 
                                              %8 = OpTypePointer Output %7 
                                Output f32_4* %9 = OpVariable Output 
                                             %10 = OpTypePointer Input %7 
                                Input f32_4* %11 = OpVariable Input 
                                             %13 = OpTypeInt 32 0 
                                         u32 %14 = OpConstant 4 
                                             %15 = OpTypeArray %7 %14 
                                             %16 = OpTypeArray %7 %14 
                                             %17 = OpTypeStruct %15 %16 %7 
                                             %18 = OpTypePointer Uniform %17 
Uniform struct {f32_4[4]; f32_4[4]; f32_4;}* %19 = OpVariable Uniform 
                                             %20 = OpTypeInt 32 1 
                                         i32 %21 = OpConstant 2 
                                             %22 = OpTypePointer Uniform %7 
                                             %26 = OpTypePointer Private %7 
                              Private f32_4* %27 = OpVariable Private 
                                Input f32_4* %28 = OpVariable Input 
                                         i32 %31 = OpConstant 0 
                                         i32 %32 = OpConstant 1 
                                         i32 %51 = OpConstant 3 
                              Private f32_4* %55 = OpVariable Private 
                                         u32 %75 = OpConstant 1 
                                             %76 = OpTypeArray %6 %75 
                                             %77 = OpTypeStruct %7 %6 %76 
                                             %78 = OpTypePointer Output %77 
        Output struct {f32_4; f32; f32[1];}* %79 = OpVariable Output 
                                             %88 = OpTypePointer Output %6 
                                         void %4 = OpFunction None %3 
                                              %5 = OpLabel 
                                       f32_4 %12 = OpLoad %11 
                              Uniform f32_4* %23 = OpAccessChain %19 %21 
                                       f32_4 %24 = OpLoad %23 
                                       f32_4 %25 = OpFMul %12 %24 
                                                     OpStore %9 %25 
                                       f32_4 %29 = OpLoad %28 
                                       f32_4 %30 = OpVectorShuffle %29 %29 1 1 1 1 
                              Uniform f32_4* %33 = OpAccessChain %19 %31 %32 
                                       f32_4 %34 = OpLoad %33 
                                       f32_4 %35 = OpFMul %30 %34 
                                                     OpStore %27 %35 
                              Uniform f32_4* %36 = OpAccessChain %19 %31 %31 
                                       f32_4 %37 = OpLoad %36 
                                       f32_4 %38 = OpLoad %28 
                                       f32_4 %39 = OpVectorShuffle %38 %38 0 0 0 0 
                                       f32_4 %40 = OpFMul %37 %39 
                                       f32_4 %41 = OpLoad %27 
                                       f32_4 %42 = OpFAdd %40 %41 
                                                     OpStore %27 %42 
                              Uniform f32_4* %43 = OpAccessChain %19 %31 %21 
                                       f32_4 %44 = OpLoad %43 
                                       f32_4 %45 = OpLoad %28 
                                       f32_4 %46 = OpVectorShuffle %45 %45 2 2 2 2 
                                       f32_4 %47 = OpFMul %44 %46 
                                       f32_4 %48 = OpLoad %27 
                                       f32_4 %49 = OpFAdd %47 %48 
                                                     OpStore %27 %49 
                                       f32_4 %50 = OpLoad %27 
                              Uniform f32_4* %52 = OpAccessChain %19 %31 %51 
                                       f32_4 %53 = OpLoad %52 
                                       f32_4 %54 = OpFAdd %50 %53 
                                                     OpStore %27 %54 
                                       f32_4 %56 = OpLoad %27 
                                       f32_4 %57 = OpVectorShuffle %56 %56 1 1 1 1 
                              Uniform f32_4* %58 = OpAccessChain %19 %32 %32 
                                       f32_4 %59 = OpLoad %58 
                                       f32_4 %60 = OpFMul %57 %59 
                                                     OpStore %55 %60 
                              Uniform f32_4* %61 = OpAccessChain %19 %32 %31 
                                       f32_4 %62 = OpLoad %61 
                                       f32_4 %63 = OpLoad %27 
                                       f32_4 %64 = OpVectorShuffle %63 %63 0 0 0 0 
                                       f32_4 %65 = OpFMul %62 %64 
                                       f32_4 %66 = OpLoad %55 
                                       f32_4 %67 = OpFAdd %65 %66 
                                                     OpStore %55 %67 
                              Uniform f32_4* %68 = OpAccessChain %19 %32 %21 
                                       f32_4 %69 = OpLoad %68 
                                       f32_4 %70 = OpLoad %27 
                                       f32_4 %71 = OpVectorShuffle %70 %70 2 2 2 2 
                                       f32_4 %72 = OpFMul %69 %71 
                                       f32_4 %73 = OpLoad %55 
                                       f32_4 %74 = OpFAdd %72 %73 
                                                     OpStore %55 %74 
                              Uniform f32_4* %80 = OpAccessChain %19 %32 %51 
                                       f32_4 %81 = OpLoad %80 
                                       f32_4 %82 = OpLoad %27 
                                       f32_4 %83 = OpVectorShuffle %82 %82 3 3 3 3 
                                       f32_4 %84 = OpFMul %81 %83 
                                       f32_4 %85 = OpLoad %55 
                                       f32_4 %86 = OpFAdd %84 %85 
                               Output f32_4* %87 = OpAccessChain %79 %31 
                                                     OpStore %87 %86 
                                 Output f32* %89 = OpAccessChain %79 %31 %75 
                                         f32 %90 = OpLoad %89 
                                         f32 %91 = OpFNegate %90 
                                 Output f32* %92 = OpAccessChain %79 %31 %75 
                                                     OpStore %92 %91 
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
uniform 	vec4 _Color;
attribute highp vec4 in_POSITION0;
attribute highp vec4 in_COLOR0;
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
    u_xlat0 = in_COLOR0 * _Color;
    vs_COLOR0 = u_xlat0;
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = op_shl(unity_StereoEyeIndex, 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
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
uniform 	vec4 _Color;
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
in highp vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
vec4 u_xlat0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_COLOR0 * _Color;
    vs_COLOR0 = u_xlat0;
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(unity_StereoEyeIndex << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
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
    float4 _Color;
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
    float4 COLOR0 [[ user(COLOR0) ]];
    float4 mtl_Position [[ position ]];
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
    output.COLOR0 = input.COLOR0 * VGlobals._Color;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = UnityStereoEyeIndex.unity_StereoEyeIndex << 0x2;
    u_xlat2 = u_xlat0.yyyy * UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1], u_xlat0.xxxx, u_xlat2);
    u_xlat2 = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)], u_xlat0.zzzz, u_xlat2);
    output.mtl_Position = fma(UnityStereoGlobals.hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)], u_xlat0.wwww, u_xlat2);
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
uniform 	vec4 _Color;
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
    vs_COLOR0 = in_COLOR0 * _Color;
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = unity_StereoEyeIndex << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
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
; Bound: 108
; Schema: 0
                                                      OpCapability Shader 
                                               %1 = OpExtInstImport "GLSL.std.450" 
                                                      OpMemoryModel Logical GLSL450 
                                                      OpEntryPoint Vertex %4 "main" %9 %11 %29 %91 
                                                      OpDecorate %9 Location 9 
                                                      OpDecorate %11 Location 11 
                                                      OpDecorate %15 ArrayStride 15 
                                                      OpDecorate %17 ArrayStride 17 
                                                      OpMemberDecorate %19 0 Offset 19 
                                                      OpMemberDecorate %19 1 Offset 19 
                                                      OpMemberDecorate %19 2 Offset 19 
                                                      OpMemberDecorate %19 3 Offset 19 
                                                      OpDecorate %19 Block 
                                                      OpDecorate %21 DescriptorSet 21 
                                                      OpDecorate %21 Binding 21 
                                                      OpDecorate %29 Location 29 
                                                      OpMemberDecorate %89 0 BuiltIn 89 
                                                      OpMemberDecorate %89 1 BuiltIn 89 
                                                      OpMemberDecorate %89 2 BuiltIn 89 
                                                      OpDecorate %89 Block 
                                               %2 = OpTypeVoid 
                                               %3 = OpTypeFunction %2 
                                               %6 = OpTypeFloat 32 
                                               %7 = OpTypeVector %6 4 
                                               %8 = OpTypePointer Output %7 
                                 Output f32_4* %9 = OpVariable Output 
                                              %10 = OpTypePointer Input %7 
                                 Input f32_4* %11 = OpVariable Input 
                                              %13 = OpTypeInt 32 0 
                                          u32 %14 = OpConstant 4 
                                              %15 = OpTypeArray %7 %14 
                                          u32 %16 = OpConstant 8 
                                              %17 = OpTypeArray %7 %16 
                                              %18 = OpTypeInt 32 1 
                                              %19 = OpTypeStruct %15 %17 %18 %7 
                                              %20 = OpTypePointer Uniform %19 
Uniform struct {f32_4[4]; f32_4[8]; i32; f32_4;}* %21 = OpVariable Uniform 
                                          i32 %22 = OpConstant 3 
                                              %23 = OpTypePointer Uniform %7 
                                              %27 = OpTypePointer Private %7 
                               Private f32_4* %28 = OpVariable Private 
                                 Input f32_4* %29 = OpVariable Input 
                                          i32 %32 = OpConstant 0 
                                          i32 %33 = OpConstant 1 
                                          i32 %44 = OpConstant 2 
                                              %56 = OpTypePointer Private %18 
                                 Private i32* %57 = OpVariable Private 
                                              %58 = OpTypePointer Uniform %18 
                               Private f32_4* %62 = OpVariable Private 
                                          u32 %87 = OpConstant 1 
                                              %88 = OpTypeArray %6 %87 
                                              %89 = OpTypeStruct %7 %6 %88 
                                              %90 = OpTypePointer Output %89 
         Output struct {f32_4; f32; f32[1];}* %91 = OpVariable Output 
                                             %102 = OpTypePointer Output %6 
                                          void %4 = OpFunction None %3 
                                               %5 = OpLabel 
                                        f32_4 %12 = OpLoad %11 
                               Uniform f32_4* %24 = OpAccessChain %21 %22 
                                        f32_4 %25 = OpLoad %24 
                                        f32_4 %26 = OpFMul %12 %25 
                                                      OpStore %9 %26 
                                        f32_4 %30 = OpLoad %29 
                                        f32_4 %31 = OpVectorShuffle %30 %30 1 1 1 1 
                               Uniform f32_4* %34 = OpAccessChain %21 %32 %33 
                                        f32_4 %35 = OpLoad %34 
                                        f32_4 %36 = OpFMul %31 %35 
                                                      OpStore %28 %36 
                               Uniform f32_4* %37 = OpAccessChain %21 %32 %32 
                                        f32_4 %38 = OpLoad %37 
                                        f32_4 %39 = OpLoad %29 
                                        f32_4 %40 = OpVectorShuffle %39 %39 0 0 0 0 
                                        f32_4 %41 = OpFMul %38 %40 
                                        f32_4 %42 = OpLoad %28 
                                        f32_4 %43 = OpFAdd %41 %42 
                                                      OpStore %28 %43 
                               Uniform f32_4* %45 = OpAccessChain %21 %32 %44 
                                        f32_4 %46 = OpLoad %45 
                                        f32_4 %47 = OpLoad %29 
                                        f32_4 %48 = OpVectorShuffle %47 %47 2 2 2 2 
                                        f32_4 %49 = OpFMul %46 %48 
                                        f32_4 %50 = OpLoad %28 
                                        f32_4 %51 = OpFAdd %49 %50 
                                                      OpStore %28 %51 
                                        f32_4 %52 = OpLoad %28 
                               Uniform f32_4* %53 = OpAccessChain %21 %32 %22 
                                        f32_4 %54 = OpLoad %53 
                                        f32_4 %55 = OpFAdd %52 %54 
                                                      OpStore %28 %55 
                                 Uniform i32* %59 = OpAccessChain %21 %44 
                                          i32 %60 = OpLoad %59 
                                          i32 %61 = OpShiftLeftLogical %60 %44 
                                                      OpStore %57 %61 
                                        f32_4 %63 = OpLoad %28 
                                        f32_4 %64 = OpVectorShuffle %63 %63 1 1 1 1 
                                          i32 %65 = OpLoad %57 
                                          i32 %66 = OpIAdd %65 %33 
                               Uniform f32_4* %67 = OpAccessChain %21 %33 %66 
                                        f32_4 %68 = OpLoad %67 
                                        f32_4 %69 = OpFMul %64 %68 
                                                      OpStore %62 %69 
                                          i32 %70 = OpLoad %57 
                               Uniform f32_4* %71 = OpAccessChain %21 %33 %70 
                                        f32_4 %72 = OpLoad %71 
                                        f32_4 %73 = OpLoad %28 
                                        f32_4 %74 = OpVectorShuffle %73 %73 0 0 0 0 
                                        f32_4 %75 = OpFMul %72 %74 
                                        f32_4 %76 = OpLoad %62 
                                        f32_4 %77 = OpFAdd %75 %76 
                                                      OpStore %62 %77 
                                          i32 %78 = OpLoad %57 
                                          i32 %79 = OpIAdd %78 %44 
                               Uniform f32_4* %80 = OpAccessChain %21 %33 %79 
                                        f32_4 %81 = OpLoad %80 
                                        f32_4 %82 = OpLoad %28 
                                        f32_4 %83 = OpVectorShuffle %82 %82 2 2 2 2 
                                        f32_4 %84 = OpFMul %81 %83 
                                        f32_4 %85 = OpLoad %62 
                                        f32_4 %86 = OpFAdd %84 %85 
                                                      OpStore %62 %86 
                                          i32 %92 = OpLoad %57 
                                          i32 %93 = OpIAdd %92 %22 
                               Uniform f32_4* %94 = OpAccessChain %21 %33 %93 
                                        f32_4 %95 = OpLoad %94 
                                        f32_4 %96 = OpLoad %28 
                                        f32_4 %97 = OpVectorShuffle %96 %96 3 3 3 3 
                                        f32_4 %98 = OpFMul %95 %97 
                                        f32_4 %99 = OpLoad %62 
                                       f32_4 %100 = OpFAdd %98 %99 
                               Output f32_4* %101 = OpAccessChain %91 %32 
                                                      OpStore %101 %100 
                                 Output f32* %103 = OpAccessChain %91 %32 %87 
                                         f32 %104 = OpLoad %103 
                                         f32 %105 = OpFNegate %104 
                                 Output f32* %106 = OpAccessChain %91 %32 %87 
                                                      OpStore %106 %105 
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
uniform 	vec4 _Color;
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
in highp vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
flat out highp uint vs_BLENDINDICES0;
vec4 u_xlat0;
uint u_xlatu0;
vec4 u_xlat1;
vec4 u_xlat2;
int u_xlati3;
void main()
{
    u_xlat0 = in_COLOR0 * _Color;
    vs_COLOR0 = u_xlat0;
    u_xlat0.x = float(uint(gl_InstanceID));
    u_xlat0.x = u_xlat0.x * 0.5;
    u_xlat0.x = fract(u_xlat0.x);
    u_xlat0.x = u_xlat0.x + u_xlat0.x;
    u_xlat0.x = roundEven(u_xlat0.x);
    u_xlatu0 = uint(u_xlat0.x);
    u_xlati3 = int(int(u_xlatu0) << 2);
    u_xlat1 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat1;
    u_xlat1 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
    u_xlat1 = u_xlat1 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat2 = u_xlat1.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati3] * u_xlat1.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 2)] * u_xlat1.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati3 + 3)] * u_xlat1.wwww + u_xlat2;
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
    float4 _Color;
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 COLOR0 [[ user(COLOR0) ]];
    float4 mtl_Position [[ position ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    output.COLOR0 = input.COLOR0 * VGlobals._Color;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
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
uniform 	vec4 _Color;
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
    vs_COLOR0 = in_COLOR0 * _Color;
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
; Bound: 114
; Schema: 0
                                                     OpCapability Shader 
                                                     OpCapability Geometry 
                                                     OpCapability ShaderViewportIndexLayerEXT 
                                                     OpExtension "SPV_EXT_shader_viewport_index_layer" 
                                              %1 = OpExtInstImport "GLSL.std.450" 
                                                     OpMemoryModel Logical GLSL450 
                                                     OpEntryPoint Vertex %4 "main" %9 %11 %29 %59 %91 %103 
                                                     OpDecorate %9 Location 9 
                                                     OpDecorate %11 Location 11 
                                                     OpDecorate %15 ArrayStride 15 
                                                     OpDecorate %17 ArrayStride 17 
                                                     OpMemberDecorate %18 0 Offset 18 
                                                     OpMemberDecorate %18 1 Offset 18 
                                                     OpMemberDecorate %18 2 Offset 18 
                                                     OpDecorate %18 Block 
                                                     OpDecorate %20 DescriptorSet 20 
                                                     OpDecorate %20 Binding 20 
                                                     OpDecorate %29 Location 29 
                                                     OpDecorate %59 BuiltIn 59 
                                                     OpMemberDecorate %89 0 BuiltIn 89 
                                                     OpMemberDecorate %89 1 BuiltIn 89 
                                                     OpMemberDecorate %89 2 BuiltIn 89 
                                                     OpDecorate %89 Block 
                                                     OpDecorate %103 BuiltIn 103 
                                              %2 = OpTypeVoid 
                                              %3 = OpTypeFunction %2 
                                              %6 = OpTypeFloat 32 
                                              %7 = OpTypeVector %6 4 
                                              %8 = OpTypePointer Output %7 
                                Output f32_4* %9 = OpVariable Output 
                                             %10 = OpTypePointer Input %7 
                                Input f32_4* %11 = OpVariable Input 
                                             %13 = OpTypeInt 32 0 
                                         u32 %14 = OpConstant 4 
                                             %15 = OpTypeArray %7 %14 
                                         u32 %16 = OpConstant 8 
                                             %17 = OpTypeArray %7 %16 
                                             %18 = OpTypeStruct %15 %17 %7 
                                             %19 = OpTypePointer Uniform %18 
Uniform struct {f32_4[4]; f32_4[8]; f32_4;}* %20 = OpVariable Uniform 
                                             %21 = OpTypeInt 32 1 
                                         i32 %22 = OpConstant 2 
                                             %23 = OpTypePointer Uniform %7 
                                             %27 = OpTypePointer Private %7 
                              Private f32_4* %28 = OpVariable Private 
                                Input f32_4* %29 = OpVariable Input 
                                         i32 %32 = OpConstant 0 
                                         i32 %33 = OpConstant 1 
                                         i32 %52 = OpConstant 3 
                                             %56 = OpTypePointer Private %21 
                                Private i32* %57 = OpVariable Private 
                                             %58 = OpTypePointer Input %21 
                                  Input i32* %59 = OpVariable Input 
                              Private f32_4* %62 = OpVariable Private 
                                         u32 %87 = OpConstant 1 
                                             %88 = OpTypeArray %6 %87 
                                             %89 = OpTypeStruct %7 %6 %88 
                                             %90 = OpTypePointer Output %89 
        Output struct {f32_4; f32; f32[1];}* %91 = OpVariable Output 
                                            %102 = OpTypePointer Output %21 
                                Output i32* %103 = OpVariable Output 
                                            %108 = OpTypePointer Output %6 
                                         void %4 = OpFunction None %3 
                                              %5 = OpLabel 
                                       f32_4 %12 = OpLoad %11 
                              Uniform f32_4* %24 = OpAccessChain %20 %22 
                                       f32_4 %25 = OpLoad %24 
                                       f32_4 %26 = OpFMul %12 %25 
                                                     OpStore %9 %26 
                                       f32_4 %30 = OpLoad %29 
                                       f32_4 %31 = OpVectorShuffle %30 %30 1 1 1 1 
                              Uniform f32_4* %34 = OpAccessChain %20 %32 %33 
                                       f32_4 %35 = OpLoad %34 
                                       f32_4 %36 = OpFMul %31 %35 
                                                     OpStore %28 %36 
                              Uniform f32_4* %37 = OpAccessChain %20 %32 %32 
                                       f32_4 %38 = OpLoad %37 
                                       f32_4 %39 = OpLoad %29 
                                       f32_4 %40 = OpVectorShuffle %39 %39 0 0 0 0 
                                       f32_4 %41 = OpFMul %38 %40 
                                       f32_4 %42 = OpLoad %28 
                                       f32_4 %43 = OpFAdd %41 %42 
                                                     OpStore %28 %43 
                              Uniform f32_4* %44 = OpAccessChain %20 %32 %22 
                                       f32_4 %45 = OpLoad %44 
                                       f32_4 %46 = OpLoad %29 
                                       f32_4 %47 = OpVectorShuffle %46 %46 2 2 2 2 
                                       f32_4 %48 = OpFMul %45 %47 
                                       f32_4 %49 = OpLoad %28 
                                       f32_4 %50 = OpFAdd %48 %49 
                                                     OpStore %28 %50 
                                       f32_4 %51 = OpLoad %28 
                              Uniform f32_4* %53 = OpAccessChain %20 %32 %52 
                                       f32_4 %54 = OpLoad %53 
                                       f32_4 %55 = OpFAdd %51 %54 
                                                     OpStore %28 %55 
                                         i32 %60 = OpLoad %59 
                                         i32 %61 = OpBitFieldInsert %32 %60 %22 %33 
                                                     OpStore %57 %61 
                                       f32_4 %63 = OpLoad %28 
                                       f32_4 %64 = OpVectorShuffle %63 %63 1 1 1 1 
                                         i32 %65 = OpLoad %57 
                                         i32 %66 = OpIAdd %65 %33 
                              Uniform f32_4* %67 = OpAccessChain %20 %33 %66 
                                       f32_4 %68 = OpLoad %67 
                                       f32_4 %69 = OpFMul %64 %68 
                                                     OpStore %62 %69 
                                         i32 %70 = OpLoad %57 
                              Uniform f32_4* %71 = OpAccessChain %20 %33 %70 
                                       f32_4 %72 = OpLoad %71 
                                       f32_4 %73 = OpLoad %28 
                                       f32_4 %74 = OpVectorShuffle %73 %73 0 0 0 0 
                                       f32_4 %75 = OpFMul %72 %74 
                                       f32_4 %76 = OpLoad %62 
                                       f32_4 %77 = OpFAdd %75 %76 
                                                     OpStore %62 %77 
                                         i32 %78 = OpLoad %57 
                                         i32 %79 = OpIAdd %78 %22 
                              Uniform f32_4* %80 = OpAccessChain %20 %33 %79 
                                       f32_4 %81 = OpLoad %80 
                                       f32_4 %82 = OpLoad %28 
                                       f32_4 %83 = OpVectorShuffle %82 %82 2 2 2 2 
                                       f32_4 %84 = OpFMul %81 %83 
                                       f32_4 %85 = OpLoad %62 
                                       f32_4 %86 = OpFAdd %84 %85 
                                                     OpStore %62 %86 
                                         i32 %92 = OpLoad %57 
                                         i32 %93 = OpIAdd %92 %52 
                              Uniform f32_4* %94 = OpAccessChain %20 %33 %93 
                                       f32_4 %95 = OpLoad %94 
                                       f32_4 %96 = OpLoad %28 
                                       f32_4 %97 = OpVectorShuffle %96 %96 3 3 3 3 
                                       f32_4 %98 = OpFMul %95 %97 
                                       f32_4 %99 = OpLoad %62 
                                      f32_4 %100 = OpFAdd %98 %99 
                              Output f32_4* %101 = OpAccessChain %91 %32 
                                                     OpStore %101 %100 
                                        i32 %104 = OpLoad %59 
                                        u32 %105 = OpBitcast %104 
                                        u32 %106 = OpBitwiseAnd %105 %87 
                                        i32 %107 = OpBitcast %106 
                                                     OpStore %103 %107 
                                Output f32* %109 = OpAccessChain %91 %32 %87 
                                        f32 %110 = OpLoad %109 
                                        f32 %111 = OpFNegate %110 
                                Output f32* %112 = OpAccessChain %91 %32 %87 
                                                     OpStore %112 %111 
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
uniform 	vec4 _Color;
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
in highp vec4 in_COLOR0;
out mediump vec4 vs_COLOR0;
out highp float vs_BLENDWEIGHT0;
vec4 u_xlat0;
uint u_xlatu0;
int u_xlati1;
vec4 u_xlat2;
void main()
{
    u_xlat0 = in_COLOR0 * _Color;
    vs_COLOR0 = u_xlat0;
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(int(gl_ViewID_OVR) << 2);
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
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
    float4 _Color;
};

struct Mtl_VertexIn
{
    float4 POSITION0 [[ attribute(0) ]] ;
    float4 COLOR0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 COLOR0 [[ user(COLOR0) ]];
    float4 mtl_Position [[ position ]];
};

vertex Mtl_VertexOut xlatMtlMain(
    constant VGlobals_Type& VGlobals [[ buffer(0) ]],
    Mtl_VertexIn input [[ stage_in ]])
{
    Mtl_VertexOut output;
    float4 u_xlat0;
    float4 u_xlat1;
    output.COLOR0 = input.COLOR0 * VGlobals._Color;
    u_xlat0 = input.POSITION0.yyyy * VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[0], input.POSITION0.xxxx, u_xlat0);
    u_xlat0 = fma(VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[2], input.POSITION0.zzzz, u_xlat0);
    u_xlat0 = u_xlat0 + VGlobals.hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlat1 = u_xlat0.yyyy * VGlobals.hlslcc_mtx4x4unity_MatrixVP[1];
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[0], u_xlat0.xxxx, u_xlat1);
    u_xlat1 = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[2], u_xlat0.zzzz, u_xlat1);
    output.mtl_Position = fma(VGlobals.hlslcc_mtx4x4unity_MatrixVP[3], u_xlat0.wwww, u_xlat1);
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
uniform 	vec4 _Color;
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
    vs_COLOR0 = in_COLOR0 * _Color;
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4x4unity_ObjectToWorld[1];
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4x4unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4x4unity_ObjectToWorld[3];
    u_xlati1 = int(gl_ViewID_OVR) << 2;
    u_xlat2 = u_xlat0.yyyy * hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 1)];
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[u_xlati1] * u_xlat0.xxxx + u_xlat2;
    u_xlat2 = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 2)] * u_xlat0.zzzz + u_xlat2;
    gl_Position = hlslcc_mtx4x4unity_StereoMatrixVP[(u_xlati1 + 3)] * u_xlat0.wwww + u_xlat2;
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
; Bound: 137
; Schema: 0
                                                      OpCapability Shader 
                                                      OpCapability MultiView 
                                                      OpExtension "SPV_KHR_multiview" 
                                               %1 = OpExtInstImport "GLSL.std.450" 
                                                      OpMemoryModel Logical GLSL450 
                                                      OpEntryPoint Vertex %4 "main" %9 %11 %27 %57 %106 %121 
                                                      OpDecorate %9 Location 9 
                                                      OpDecorate %11 Location 11 
                                                      OpDecorate %15 ArrayStride 15 
                                                      OpMemberDecorate %16 0 Offset 16 
                                                      OpMemberDecorate %16 1 Offset 16 
                                                      OpDecorate %16 Block 
                                                      OpDecorate %18 DescriptorSet 18 
                                                      OpDecorate %18 Binding 18 
                                                      OpDecorate %27 Location 27 
                                                      OpDecorate %57 BuiltIn 57 
                                                      OpDecorate %65 ArrayStride 65 
                                                      OpDecorate %66 ArrayStride 66 
                                                      OpDecorate %67 ArrayStride 67 
                                                      OpDecorate %68 ArrayStride 68 
                                                      OpDecorate %69 ArrayStride 69 
                                                      OpDecorate %70 ArrayStride 70 
                                                      OpDecorate %71 ArrayStride 71 
                                                      OpDecorate %72 ArrayStride 72 
                                                      OpDecorate %75 ArrayStride 75 
                                                      OpDecorate %76 ArrayStride 76 
                                                      OpMemberDecorate %77 0 Offset 77 
                                                      OpMemberDecorate %77 1 Offset 77 
                                                      OpMemberDecorate %77 2 Offset 77 
                                                      OpMemberDecorate %77 3 Offset 77 
                                                      OpMemberDecorate %77 4 Offset 77 
                                                      OpMemberDecorate %77 5 Offset 77 
                                                      OpMemberDecorate %77 6 Offset 77 
                                                      OpMemberDecorate %77 7 Offset 77 
                                                      OpMemberDecorate %77 8 Offset 77 
                                                      OpMemberDecorate %77 9 Offset 77 
                                                      OpDecorate %77 Block 
                                                      OpDecorate %79 DescriptorSet 79 
                                                      OpDecorate %79 Binding 79 
                                                      OpMemberDecorate %104 0 BuiltIn 104 
                                                      OpMemberDecorate %104 1 BuiltIn 104 
                                                      OpMemberDecorate %104 2 BuiltIn 104 
                                                      OpDecorate %104 Block 
                                                      OpDecorate %121 Location 121 
                                                      OpDecorate %122 ArrayStride 122 
                                                      OpMemberDecorate %123 0 Offset 123 
                                                      OpDecorate %123 Block 
                                                      OpDecorate %125 DescriptorSet 125 
                                                      OpDecorate %125 Binding 125 
                                               %2 = OpTypeVoid 
                                               %3 = OpTypeFunction %2 
                                               %6 = OpTypeFloat 32 
                                               %7 = OpTypeVector %6 4 
                                               %8 = OpTypePointer Output %7 
                                 Output f32_4* %9 = OpVariable Output 
                                              %10 = OpTypePointer Input %7 
                                 Input f32_4* %11 = OpVariable Input 
                                              %13 = OpTypeInt 32 0 
                                          u32 %14 = OpConstant 4 
                                              %15 = OpTypeArray %7 %14 
                                              %16 = OpTypeStruct %15 %7 
                                              %17 = OpTypePointer Uniform %16 
           Uniform struct {f32_4[4]; f32_4;}* %18 = OpVariable Uniform 
                                              %19 = OpTypeInt 32 1 
                                          i32 %20 = OpConstant 1 
                                              %21 = OpTypePointer Uniform %7 
                                              %25 = OpTypePointer Private %7 
                               Private f32_4* %26 = OpVariable Private 
                                 Input f32_4* %27 = OpVariable Input 
                                          i32 %30 = OpConstant 0 
                                          i32 %41 = OpConstant 2 
                                          i32 %50 = OpConstant 3 
                                              %54 = OpTypePointer Private %19 
                                 Private i32* %55 = OpVariable Private 
                                              %56 = OpTypePointer Input %13 
                                   Input u32* %57 = OpVariable Input 
                               Private f32_4* %61 = OpVariable Private 
                                          u32 %64 = OpConstant 8 
                                              %65 = OpTypeArray %7 %64 
                                              %66 = OpTypeArray %7 %64 
                                              %67 = OpTypeArray %7 %64 
                                              %68 = OpTypeArray %7 %64 
                                              %69 = OpTypeArray %7 %64 
                                              %70 = OpTypeArray %7 %64 
                                              %71 = OpTypeArray %7 %64 
                                              %72 = OpTypeArray %7 %64 
                                              %73 = OpTypeVector %6 3 
                                          u32 %74 = OpConstant 2 
                                              %75 = OpTypeArray %73 %74 
                                              %76 = OpTypeArray %7 %74 
                                              %77 = OpTypeStruct %65 %66 %67 %68 %69 %70 %71 %72 %75 %76 
                                              %78 = OpTypePointer Uniform %77 
Uniform struct {f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_4[8]; f32_3[2]; f32_4[2];}* %79 = OpVariable Uniform 
                                         u32 %102 = OpConstant 1 
                                             %103 = OpTypeArray %6 %102 
                                             %104 = OpTypeStruct %7 %6 %103 
                                             %105 = OpTypePointer Output %104 
        Output struct {f32_4; f32; f32[1];}* %106 = OpVariable Output 
                                             %117 = OpTypePointer Private %13 
                                Private u32* %118 = OpVariable Private 
                                             %120 = OpTypePointer Output %6 
                                 Output f32* %121 = OpVariable Output 
                                             %122 = OpTypeArray %7 %74 
                                             %123 = OpTypeStruct %122 
                                             %124 = OpTypePointer Uniform %123 
                 Uniform struct {f32_4[2];}* %125 = OpVariable Uniform 
                                         u32 %128 = OpConstant 0 
                                             %129 = OpTypePointer Uniform %6 
                                          void %4 = OpFunction None %3 
                                               %5 = OpLabel 
                                        f32_4 %12 = OpLoad %11 
                               Uniform f32_4* %22 = OpAccessChain %18 %20 
                                        f32_4 %23 = OpLoad %22 
                                        f32_4 %24 = OpFMul %12 %23 
                                                      OpStore %9 %24 
                                        f32_4 %28 = OpLoad %27 
                                        f32_4 %29 = OpVectorShuffle %28 %28 1 1 1 1 
                               Uniform f32_4* %31 = OpAccessChain %18 %30 %20 
                                        f32_4 %32 = OpLoad %31 
                                        f32_4 %33 = OpFMul %29 %32 
                                                      OpStore %26 %33 
                               Uniform f32_4* %34 = OpAccessChain %18 %30 %30 
                                        f32_4 %35 = OpLoad %34 
                                        f32_4 %36 = OpLoad %27 
                                        f32_4 %37 = OpVectorShuffle %36 %36 0 0 0 0 
                                        f32_4 %38 = OpFMul %35 %37 
                                        f32_4 %39 = OpLoad %26 
                                        f32_4 %40 = OpFAdd %38 %39 
                                                      OpStore %26 %40 
                               Uniform f32_4* %42 = OpAccessChain %18 %30 %41 
                                        f32_4 %43 = OpLoad %42 
                                        f32_4 %44 = OpLoad %27 
                                        f32_4 %45 = OpVectorShuffle %44 %44 2 2 2 2 
                                        f32_4 %46 = OpFMul %43 %45 
                                        f32_4 %47 = OpLoad %26 
                                        f32_4 %48 = OpFAdd %46 %47 
                                                      OpStore %26 %48 
                                        f32_4 %49 = OpLoad %26 
                               Uniform f32_4* %51 = OpAccessChain %18 %30 %50 
                                        f32_4 %52 = OpLoad %51 
                                        f32_4 %53 = OpFAdd %49 %52 
                                                      OpStore %26 %53 
                                          u32 %58 = OpLoad %57 
                                          i32 %59 = OpBitcast %58 
                                          i32 %60 = OpShiftLeftLogical %59 %41 
                                                      OpStore %55 %60 
                                        f32_4 %62 = OpLoad %26 
                                        f32_4 %63 = OpVectorShuffle %62 %62 1 1 1 1 
                                          i32 %80 = OpLoad %55 
                                          i32 %81 = OpIAdd %80 %20 
                               Uniform f32_4* %82 = OpAccessChain %79 %50 %81 
                                        f32_4 %83 = OpLoad %82 
                                        f32_4 %84 = OpFMul %63 %83 
                                                      OpStore %61 %84 
                                          i32 %85 = OpLoad %55 
                               Uniform f32_4* %86 = OpAccessChain %79 %50 %85 
                                        f32_4 %87 = OpLoad %86 
                                        f32_4 %88 = OpLoad %26 
                                        f32_4 %89 = OpVectorShuffle %88 %88 0 0 0 0 
                                        f32_4 %90 = OpFMul %87 %89 
                                        f32_4 %91 = OpLoad %61 
                                        f32_4 %92 = OpFAdd %90 %91 
                                                      OpStore %61 %92 
                                          i32 %93 = OpLoad %55 
                                          i32 %94 = OpIAdd %93 %41 
                               Uniform f32_4* %95 = OpAccessChain %79 %50 %94 
                                        f32_4 %96 = OpLoad %95 
                                        f32_4 %97 = OpLoad %26 
                                        f32_4 %98 = OpVectorShuffle %97 %97 2 2 2 2 
                                        f32_4 %99 = OpFMul %96 %98 
                                       f32_4 %100 = OpLoad %61 
                                       f32_4 %101 = OpFAdd %99 %100 
                                                      OpStore %61 %101 
                                         i32 %107 = OpLoad %55 
                                         i32 %108 = OpIAdd %107 %50 
                              Uniform f32_4* %109 = OpAccessChain %79 %50 %108 
                                       f32_4 %110 = OpLoad %109 
                                       f32_4 %111 = OpLoad %26 
                                       f32_4 %112 = OpVectorShuffle %111 %111 3 3 3 3 
                                       f32_4 %113 = OpFMul %110 %112 
                                       f32_4 %114 = OpLoad %61 
                                       f32_4 %115 = OpFAdd %113 %114 
                               Output f32_4* %116 = OpAccessChain %106 %30 
                                                      OpStore %116 %115 
                                         u32 %119 = OpLoad %57 
                                                      OpStore %118 %119 
                                         u32 %126 = OpLoad %118 
                                         i32 %127 = OpBitcast %126 
                                Uniform f32* %130 = OpAccessChain %125 %30 %127 %128 
                                         f32 %131 = OpLoad %130 
                                                      OpStore %121 %131 
                                 Output f32* %132 = OpAccessChain %106 %30 %102 
                                         f32 %133 = OpLoad %132 
                                         f32 %134 = OpFNegate %133 
                                 Output f32* %135 = OpAccessChain %106 %30 %102 
                                                      OpStore %135 %134 
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