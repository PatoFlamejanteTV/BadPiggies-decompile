//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Hidden/FrameDebuggerRenderTargetDisplay" {
Properties {
_MainTex ("", any) = "white" { }
}
SubShader {
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 51439
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
attribute highp vec3 in_TEXCOORD0;
varying highp vec3 vs_TEXCOORD0;
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
    vs_TEXCOORD0.xyz = in_TEXCOORD0.xyz;
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
uniform 	mediump vec4 _Channels;
uniform 	mediump vec4 _Levels;
uniform 	int _UndoOutputSRGB;
uniform highp sampler2D _MainTex;
varying highp vec3 vs_TEXCOORD0;
#define SV_Target0 gl_FragData[0]
vec4 u_xlat0;
mediump vec4 u_xlat16_0;
mediump vec3 u_xlat16_1;
vec3 u_xlat2;
bool u_xlatb2;
mediump float u_xlat16_4;
void main()
{
    u_xlat0 = texture2D(_MainTex, vs_TEXCOORD0.xy);
    u_xlat16_0 = u_xlat0 + (-_Levels.xxxx);
    u_xlat16_1.x = (-_Levels.x) + _Levels.y;
    u_xlat16_0 = u_xlat16_0 / u_xlat16_1.xxxx;
    u_xlat16_0 = u_xlat16_0 * _Channels;
    u_xlat16_1.x = dot(u_xlat16_0, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlat16_4 = dot(_Channels, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlatb2 = u_xlat16_4==1.0;
    u_xlat16_0 = (bool(u_xlatb2)) ? u_xlat16_1.xxxx : u_xlat16_0;
    u_xlat16_1.xyz = u_xlat16_0.xyz;
    u_xlat16_1.xyz = clamp(u_xlat16_1.xyz, 0.0, 1.0);
    u_xlat2.xyz = u_xlat16_1.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat2.xyz = u_xlat16_1.xyz * u_xlat2.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat2.xyz = u_xlat16_1.xyz * u_xlat2.xyz;
    SV_Target0.xyz = (_UndoOutputSRGB != 0) ? u_xlat2.xyz : u_xlat16_0.xyz;
    SV_Target0.w = u_xlat16_0.w;
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
in highp vec3 in_TEXCOORD0;
out highp vec3 vs_TEXCOORD0;
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
    vs_TEXCOORD0.xyz = in_TEXCOORD0.xyz;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
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
uniform 	mediump vec4 _Channels;
uniform 	mediump vec4 _Levels;
uniform 	int _UndoOutputSRGB;
UNITY_LOCATION(0) uniform highp sampler2D _MainTex;
in highp vec3 vs_TEXCOORD0;
layout(location = 0) out mediump vec4 SV_Target0;
vec4 u_xlat0;
mediump vec4 u_xlat16_0;
mediump vec3 u_xlat16_1;
vec3 u_xlat2;
bool u_xlatb2;
mediump float u_xlat16_4;
void main()
{
    u_xlat0 = texture(_MainTex, vs_TEXCOORD0.xy);
    u_xlat16_0 = u_xlat0 + (-_Levels.xxxx);
    u_xlat16_1.x = (-_Levels.x) + _Levels.y;
    u_xlat16_0 = u_xlat16_0 / u_xlat16_1.xxxx;
    u_xlat16_0 = u_xlat16_0 * _Channels;
    u_xlat16_1.x = dot(u_xlat16_0, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlat16_4 = dot(_Channels, vec4(1.0, 1.0, 1.0, 1.0));
#ifdef UNITY_ADRENO_ES3
    u_xlatb2 = !!(u_xlat16_4==1.0);
#else
    u_xlatb2 = u_xlat16_4==1.0;
#endif
    u_xlat16_0 = (bool(u_xlatb2)) ? u_xlat16_1.xxxx : u_xlat16_0;
    u_xlat16_1.xyz = u_xlat16_0.xyz;
#ifdef UNITY_ADRENO_ES3
    u_xlat16_1.xyz = min(max(u_xlat16_1.xyz, 0.0), 1.0);
#else
    u_xlat16_1.xyz = clamp(u_xlat16_1.xyz, 0.0, 1.0);
#endif
    u_xlat2.xyz = u_xlat16_1.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat2.xyz = u_xlat16_1.xyz * u_xlat2.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat2.xyz = u_xlat16_1.xyz * u_xlat2.xyz;
    SV_Target0.xyz = (_UndoOutputSRGB != 0) ? u_xlat2.xyz : u_xlat16_0.xyz;
    SV_Target0.w = u_xlat16_0.w;
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
    float3 TEXCOORD0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float3 TEXCOORD0 [[ user(TEXCOORD0) ]];
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
    output.TEXCOORD0.xyz = input.TEXCOORD0.xyz;
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
in  vec3 in_TEXCOORD0;
out vec3 vs_TEXCOORD0;
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
    vs_TEXCOORD0.xyz = in_TEXCOORD0.xyz;
    return;
}

#endif
#ifdef FRAGMENT
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
uniform 	vec4 _Channels;
uniform 	vec4 _Levels;
uniform 	int _UndoOutputSRGB;
UNITY_LOCATION(0) uniform  sampler2D _MainTex;
in  vec3 vs_TEXCOORD0;
layout(location = 0) out vec4 SV_Target0;
vec4 u_xlat0;
vec3 u_xlat1;
vec3 u_xlat2;
float u_xlat4;
bool u_xlatb4;
void main()
{
    u_xlat0 = texture(_MainTex, vs_TEXCOORD0.xy);
    u_xlat0 = u_xlat0 + (-_Levels.xxxx);
    u_xlat1.x = (-_Levels.x) + _Levels.y;
    u_xlat0 = u_xlat0 / u_xlat1.xxxx;
    u_xlat0 = u_xlat0 * _Channels;
    u_xlat1.x = dot(u_xlat0, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlat4 = dot(_Channels, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlatb4 = u_xlat4==1.0;
    u_xlat0 = (bool(u_xlatb4)) ? u_xlat1.xxxx : u_xlat0;
    u_xlat1.xyz = u_xlat0.xyz;
    u_xlat1.xyz = clamp(u_xlat1.xyz, 0.0, 1.0);
    u_xlat2.xyz = u_xlat1.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat2.xyz = u_xlat1.xyz * u_xlat2.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat1.xyz = u_xlat1.xyz * u_xlat2.xyz;
    SV_Target0.xyz = (_UndoOutputSRGB != 0) ? u_xlat1.xyz : u_xlat0.xyz;
    SV_Target0.w = u_xlat0.w;
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
                                              OpEntryPoint Vertex %4 "main" %11 %72 %84 %86 
                                              OpName vs_TEXCOORD0 "vs_TEXCOORD0" 
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
                                              OpDecorate vs_TEXCOORD0 Location 84 
                                              OpDecorate %86 Location 86 
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
                                      %82 = OpTypeVector %6 3 
                                      %83 = OpTypePointer Output %82 
               Output f32_3* vs_TEXCOORD0 = OpVariable Output 
                                      %85 = OpTypePointer Input %82 
                         Input f32_3* %86 = OpVariable Input 
                                      %88 = OpTypePointer Output %6 
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
                                f32_3 %87 = OpLoad %86 
                                              OpStore vs_TEXCOORD0 %87 
                          Output f32* %89 = OpAccessChain %72 %22 %68 
                                  f32 %90 = OpLoad %89 
                                  f32 %91 = OpFNegate %90 
                          Output f32* %92 = OpAccessChain %72 %22 %68 
                                              OpStore %92 %91 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 134
; Schema: 0
                                             OpCapability Shader 
                                      %1 = OpExtInstImport "GLSL.std.450" 
                                             OpMemoryModel Logical GLSL450 
                                             OpEntryPoint Fragment %4 "main" %22 %111 
                                             OpExecutionMode %4 OriginUpperLeft 
                                             OpName vs_TEXCOORD0 "vs_TEXCOORD0" 
                                             OpDecorate %12 DescriptorSet 12 
                                             OpDecorate %12 Binding 12 
                                             OpDecorate %16 DescriptorSet 16 
                                             OpDecorate %16 Binding 16 
                                             OpDecorate vs_TEXCOORD0 Location 22 
                                             OpMemberDecorate %29 0 Offset 29 
                                             OpMemberDecorate %29 1 Offset 29 
                                             OpMemberDecorate %29 2 Offset 29 
                                             OpDecorate %29 Block 
                                             OpDecorate %31 DescriptorSet 31 
                                             OpDecorate %31 Binding 31 
                                             OpDecorate %111 Location 111 
                                      %2 = OpTypeVoid 
                                      %3 = OpTypeFunction %2 
                                      %6 = OpTypeFloat 32 
                                      %7 = OpTypeVector %6 4 
                                      %8 = OpTypePointer Private %7 
                       Private f32_4* %9 = OpVariable Private 
                                     %10 = OpTypeImage %6 Dim2D 0 0 0 1 Unknown 
                                     %11 = OpTypePointer UniformConstant %10 
UniformConstant read_only Texture2D* %12 = OpVariable UniformConstant 
                                     %14 = OpTypeSampler 
                                     %15 = OpTypePointer UniformConstant %14 
            UniformConstant sampler* %16 = OpVariable UniformConstant 
                                     %18 = OpTypeSampledImage %10 
                                     %20 = OpTypeVector %6 3 
                                     %21 = OpTypePointer Input %20 
               Input f32_3* vs_TEXCOORD0 = OpVariable Input 
                                     %23 = OpTypeVector %6 2 
                                     %28 = OpTypeInt 32 1 
                                     %29 = OpTypeStruct %7 %7 %28 
                                     %30 = OpTypePointer Uniform %29 
Uniform struct {f32_4; f32_4; i32;}* %31 = OpVariable Uniform 
                                 i32 %32 = OpConstant 1 
                                     %33 = OpTypePointer Uniform %7 
                                     %39 = OpTypePointer Private %20 
                      Private f32_3* %40 = OpVariable Private 
                                     %41 = OpTypeInt 32 0 
                                 u32 %42 = OpConstant 0 
                                     %43 = OpTypePointer Uniform %6 
                                 u32 %47 = OpConstant 1 
                                     %51 = OpTypePointer Private %6 
                                 i32 %58 = OpConstant 0 
                                 f32 %63 = OpConstant 3.674022E-40 
                               f32_4 %64 = OpConstantComposite %63 %63 %63 %63 
                        Private f32* %67 = OpVariable Private 
                                     %71 = OpTypeBool 
                                     %72 = OpTypePointer Private %71 
                       Private bool* %73 = OpVariable Private 
                                     %77 = OpTypePointer Function %7 
                                 f32 %89 = OpConstant 3.674022E-40 
                      Private f32_3* %93 = OpVariable Private 
                                 f32 %95 = OpConstant 3.674022E-40 
                               f32_3 %96 = OpConstantComposite %95 %95 %95 
                                 f32 %98 = OpConstant 3.674022E-40 
                               f32_3 %99 = OpConstantComposite %98 %98 %98 
                                f32 %104 = OpConstant 3.674022E-40 
                              f32_3 %105 = OpConstantComposite %104 %104 %104 
                                    %110 = OpTypePointer Output %7 
                      Output f32_4* %111 = OpVariable Output 
                                i32 %112 = OpConstant 2 
                                    %113 = OpTypePointer Uniform %28 
                                    %117 = OpTypePointer Function %20 
                                u32 %128 = OpConstant 3 
                                    %131 = OpTypePointer Output %6 
                                 void %4 = OpFunction None %3 
                                      %5 = OpLabel 
                     Function f32_4* %78 = OpVariable Function 
                    Function f32_3* %118 = OpVariable Function 
                 read_only Texture2D %13 = OpLoad %12 
                             sampler %17 = OpLoad %16 
          read_only Texture2DSampled %19 = OpSampledImage %13 %17 
                               f32_3 %24 = OpLoad vs_TEXCOORD0 
                               f32_2 %25 = OpVectorShuffle %24 %24 0 1 
                               f32_4 %26 = OpImageSampleImplicitLod %19 %25 
                                             OpStore %9 %26 
                               f32_4 %27 = OpLoad %9 
                      Uniform f32_4* %34 = OpAccessChain %31 %32 
                               f32_4 %35 = OpLoad %34 
                               f32_4 %36 = OpVectorShuffle %35 %35 0 0 0 0 
                               f32_4 %37 = OpFNegate %36 
                               f32_4 %38 = OpFAdd %27 %37 
                                             OpStore %9 %38 
                        Uniform f32* %44 = OpAccessChain %31 %32 %42 
                                 f32 %45 = OpLoad %44 
                                 f32 %46 = OpFNegate %45 
                        Uniform f32* %48 = OpAccessChain %31 %32 %47 
                                 f32 %49 = OpLoad %48 
                                 f32 %50 = OpFAdd %46 %49 
                        Private f32* %52 = OpAccessChain %40 %42 
                                             OpStore %52 %50 
                               f32_4 %53 = OpLoad %9 
                               f32_3 %54 = OpLoad %40 
                               f32_4 %55 = OpVectorShuffle %54 %54 0 0 0 0 
                               f32_4 %56 = OpFDiv %53 %55 
                                             OpStore %9 %56 
                               f32_4 %57 = OpLoad %9 
                      Uniform f32_4* %59 = OpAccessChain %31 %58 
                               f32_4 %60 = OpLoad %59 
                               f32_4 %61 = OpFMul %57 %60 
                                             OpStore %9 %61 
                               f32_4 %62 = OpLoad %9 
                                 f32 %65 = OpDot %62 %64 
                        Private f32* %66 = OpAccessChain %40 %42 
                                             OpStore %66 %65 
                      Uniform f32_4* %68 = OpAccessChain %31 %58 
                               f32_4 %69 = OpLoad %68 
                                 f32 %70 = OpDot %69 %64 
                                             OpStore %67 %70 
                                 f32 %74 = OpLoad %67 
                                bool %75 = OpFOrdEqual %74 %63 
                                             OpStore %73 %75 
                                bool %76 = OpLoad %73 
                                             OpSelectionMerge %80 None 
                                             OpBranchConditional %76 %79 %83 
                                     %79 = OpLabel 
                               f32_3 %81 = OpLoad %40 
                               f32_4 %82 = OpVectorShuffle %81 %81 0 0 0 0 
                                             OpStore %78 %82 
                                             OpBranch %80 
                                     %83 = OpLabel 
                               f32_4 %84 = OpLoad %9 
                                             OpStore %78 %84 
                                             OpBranch %80 
                                     %80 = OpLabel 
                               f32_4 %85 = OpLoad %78 
                                             OpStore %9 %85 
                               f32_4 %86 = OpLoad %9 
                               f32_3 %87 = OpVectorShuffle %86 %86 0 1 2 
                                             OpStore %40 %87 
                               f32_3 %88 = OpLoad %40 
                               f32_3 %90 = OpCompositeConstruct %89 %89 %89 
                               f32_3 %91 = OpCompositeConstruct %63 %63 %63 
                               f32_3 %92 = OpExtInst %1 43 %88 %90 %91 
                                             OpStore %40 %92 
                               f32_3 %94 = OpLoad %40 
                               f32_3 %97 = OpFMul %94 %96 
                              f32_3 %100 = OpFAdd %97 %99 
                                             OpStore %93 %100 
                              f32_3 %101 = OpLoad %40 
                              f32_3 %102 = OpLoad %93 
                              f32_3 %103 = OpFMul %101 %102 
                              f32_3 %106 = OpFAdd %103 %105 
                                             OpStore %93 %106 
                              f32_3 %107 = OpLoad %40 
                              f32_3 %108 = OpLoad %93 
                              f32_3 %109 = OpFMul %107 %108 
                                             OpStore %40 %109 
                       Uniform i32* %114 = OpAccessChain %31 %112 
                                i32 %115 = OpLoad %114 
                               bool %116 = OpINotEqual %115 %58 
                                             OpSelectionMerge %120 None 
                                             OpBranchConditional %116 %119 %122 
                                    %119 = OpLabel 
                              f32_3 %121 = OpLoad %40 
                                             OpStore %118 %121 
                                             OpBranch %120 
                                    %122 = OpLabel 
                              f32_4 %123 = OpLoad %9 
                              f32_3 %124 = OpVectorShuffle %123 %123 0 1 2 
                                             OpStore %118 %124 
                                             OpBranch %120 
                                    %120 = OpLabel 
                              f32_3 %125 = OpLoad %118 
                              f32_4 %126 = OpLoad %111 
                              f32_4 %127 = OpVectorShuffle %126 %125 4 5 6 3 
                                             OpStore %111 %127 
                       Private f32* %129 = OpAccessChain %9 %128 
                                f32 %130 = OpLoad %129 
                        Output f32* %132 = OpAccessChain %111 %128 
                                             OpStore %132 %130 
                                             OpReturn
                                             OpFunctionEnd
"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
}
SubProgram "gles " {
""
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
struct FGlobals_Type
{
    float4 _Channels;
    float4 _Levels;
    int _UndoOutputSRGB;
};

struct Mtl_FragmentIn
{
    float3 TEXCOORD0 [[ user(TEXCOORD0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    constant FGlobals_Type& FGlobals [[ buffer(0) ]],
    sampler sampler_MainTex [[ sampler (0) ]],
    texture2d<float, access::sample > _MainTex [[ texture(0) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float4 u_xlat0;
    float3 u_xlat1;
    float3 u_xlat2;
    float u_xlat4;
    bool u_xlatb4;
    u_xlat0 = _MainTex.sample(sampler_MainTex, input.TEXCOORD0.xy);
    u_xlat0 = u_xlat0 + (-FGlobals._Levels.xxxx);
    u_xlat1.x = (-FGlobals._Levels.x) + FGlobals._Levels.y;
    u_xlat0 = u_xlat0 / u_xlat1.xxxx;
    u_xlat0 = u_xlat0 * FGlobals._Channels;
    u_xlat1.x = dot(u_xlat0, float4(1.0, 1.0, 1.0, 1.0));
    u_xlat4 = dot(FGlobals._Channels, float4(1.0, 1.0, 1.0, 1.0));
    u_xlatb4 = u_xlat4==1.0;
    u_xlat0 = (bool(u_xlatb4)) ? u_xlat1.xxxx : u_xlat0;
    u_xlat1.xyz = u_xlat0.xyz;
    u_xlat1.xyz = clamp(u_xlat1.xyz, 0.0f, 1.0f);
    u_xlat2.xyz = fma(u_xlat1.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat2.xyz = fma(u_xlat1.xyz, u_xlat2.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = u_xlat1.xyz * u_xlat2.xyz;
    output.SV_Target0.xyz = (FGlobals._UndoOutputSRGB != 0) ? u_xlat1.xyz : u_xlat0.xyz;
    output.SV_Target0.w = u_xlat0.w;
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
}
}
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 114367
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
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
    float3 TEXCOORD0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float3 TEXCOORD0 [[ user(TEXCOORD0) ]];
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
    output.TEXCOORD0.xyz = input.TEXCOORD0.xyz;
    return output;
}
"
}
SubProgram "gles " {
"#ifdef VERTEX
#version 100

uniform 	vec4 hlslcc_mtx4x4unity_ObjectToWorld[4];
uniform 	vec4 hlslcc_mtx4x4unity_MatrixVP[4];
attribute highp vec4 in_POSITION0;
attribute highp vec3 in_TEXCOORD0;
varying highp vec3 vs_TEXCOORD0;
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
    vs_TEXCOORD0.xyz = in_TEXCOORD0.xyz;
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
uniform 	mediump vec4 _Channels;
uniform 	mediump vec4 _Levels;
uniform 	int _UndoOutputSRGB;
uniform highp samplerCube _MainTex;
varying highp vec3 vs_TEXCOORD0;
#define SV_Target0 gl_FragData[0]
vec4 u_xlat0;
mediump vec4 u_xlat16_0;
mediump vec3 u_xlat16_1;
vec3 u_xlat2;
bool u_xlatb2;
mediump float u_xlat16_4;
void main()
{
    u_xlat0 = textureCube(_MainTex, vs_TEXCOORD0.xyz);
    u_xlat16_0 = u_xlat0 + (-_Levels.xxxx);
    u_xlat16_1.x = (-_Levels.x) + _Levels.y;
    u_xlat16_0 = u_xlat16_0 / u_xlat16_1.xxxx;
    u_xlat16_0 = u_xlat16_0 * _Channels;
    u_xlat16_1.x = dot(u_xlat16_0, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlat16_4 = dot(_Channels, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlatb2 = u_xlat16_4==1.0;
    u_xlat16_0 = (bool(u_xlatb2)) ? u_xlat16_1.xxxx : u_xlat16_0;
    u_xlat16_1.xyz = u_xlat16_0.xyz;
    u_xlat16_1.xyz = clamp(u_xlat16_1.xyz, 0.0, 1.0);
    u_xlat2.xyz = u_xlat16_1.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat2.xyz = u_xlat16_1.xyz * u_xlat2.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat2.xyz = u_xlat16_1.xyz * u_xlat2.xyz;
    SV_Target0.xyz = (_UndoOutputSRGB != 0) ? u_xlat2.xyz : u_xlat16_0.xyz;
    SV_Target0.w = u_xlat16_0.w;
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
in highp vec3 in_TEXCOORD0;
out highp vec3 vs_TEXCOORD0;
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
    vs_TEXCOORD0.xyz = in_TEXCOORD0.xyz;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
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
uniform 	mediump vec4 _Channels;
uniform 	mediump vec4 _Levels;
uniform 	int _UndoOutputSRGB;
UNITY_LOCATION(0) uniform highp samplerCube _MainTex;
in highp vec3 vs_TEXCOORD0;
layout(location = 0) out mediump vec4 SV_Target0;
vec4 u_xlat0;
mediump vec4 u_xlat16_0;
mediump vec3 u_xlat16_1;
vec3 u_xlat2;
bool u_xlatb2;
mediump float u_xlat16_4;
void main()
{
    u_xlat0 = texture(_MainTex, vs_TEXCOORD0.xyz);
    u_xlat16_0 = u_xlat0 + (-_Levels.xxxx);
    u_xlat16_1.x = (-_Levels.x) + _Levels.y;
    u_xlat16_0 = u_xlat16_0 / u_xlat16_1.xxxx;
    u_xlat16_0 = u_xlat16_0 * _Channels;
    u_xlat16_1.x = dot(u_xlat16_0, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlat16_4 = dot(_Channels, vec4(1.0, 1.0, 1.0, 1.0));
#ifdef UNITY_ADRENO_ES3
    u_xlatb2 = !!(u_xlat16_4==1.0);
#else
    u_xlatb2 = u_xlat16_4==1.0;
#endif
    u_xlat16_0 = (bool(u_xlatb2)) ? u_xlat16_1.xxxx : u_xlat16_0;
    u_xlat16_1.xyz = u_xlat16_0.xyz;
#ifdef UNITY_ADRENO_ES3
    u_xlat16_1.xyz = min(max(u_xlat16_1.xyz, 0.0), 1.0);
#else
    u_xlat16_1.xyz = clamp(u_xlat16_1.xyz, 0.0, 1.0);
#endif
    u_xlat2.xyz = u_xlat16_1.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat2.xyz = u_xlat16_1.xyz * u_xlat2.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat2.xyz = u_xlat16_1.xyz * u_xlat2.xyz;
    SV_Target0.xyz = (_UndoOutputSRGB != 0) ? u_xlat2.xyz : u_xlat16_0.xyz;
    SV_Target0.w = u_xlat16_0.w;
    return;
}

#endif
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
in  vec3 in_TEXCOORD0;
out vec3 vs_TEXCOORD0;
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
    vs_TEXCOORD0.xyz = in_TEXCOORD0.xyz;
    return;
}

#endif
#ifdef FRAGMENT
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
uniform 	vec4 _Channels;
uniform 	vec4 _Levels;
uniform 	int _UndoOutputSRGB;
UNITY_LOCATION(0) uniform  samplerCube _MainTex;
in  vec3 vs_TEXCOORD0;
layout(location = 0) out vec4 SV_Target0;
vec4 u_xlat0;
vec3 u_xlat1;
vec3 u_xlat2;
float u_xlat4;
bool u_xlatb4;
void main()
{
    u_xlat0 = texture(_MainTex, vs_TEXCOORD0.xyz);
    u_xlat0 = u_xlat0 + (-_Levels.xxxx);
    u_xlat1.x = (-_Levels.x) + _Levels.y;
    u_xlat0 = u_xlat0 / u_xlat1.xxxx;
    u_xlat0 = u_xlat0 * _Channels;
    u_xlat1.x = dot(u_xlat0, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlat4 = dot(_Channels, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlatb4 = u_xlat4==1.0;
    u_xlat0 = (bool(u_xlatb4)) ? u_xlat1.xxxx : u_xlat0;
    u_xlat1.xyz = u_xlat0.xyz;
    u_xlat1.xyz = clamp(u_xlat1.xyz, 0.0, 1.0);
    u_xlat2.xyz = u_xlat1.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat2.xyz = u_xlat1.xyz * u_xlat2.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat1.xyz = u_xlat1.xyz * u_xlat2.xyz;
    SV_Target0.xyz = (_UndoOutputSRGB != 0) ? u_xlat1.xyz : u_xlat0.xyz;
    SV_Target0.w = u_xlat0.w;
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
                                              OpEntryPoint Vertex %4 "main" %11 %72 %84 %86 
                                              OpName vs_TEXCOORD0 "vs_TEXCOORD0" 
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
                                              OpDecorate vs_TEXCOORD0 Location 84 
                                              OpDecorate %86 Location 86 
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
                                      %82 = OpTypeVector %6 3 
                                      %83 = OpTypePointer Output %82 
               Output f32_3* vs_TEXCOORD0 = OpVariable Output 
                                      %85 = OpTypePointer Input %82 
                         Input f32_3* %86 = OpVariable Input 
                                      %88 = OpTypePointer Output %6 
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
                                f32_3 %87 = OpLoad %86 
                                              OpStore vs_TEXCOORD0 %87 
                          Output f32* %89 = OpAccessChain %72 %22 %68 
                                  f32 %90 = OpLoad %89 
                                  f32 %91 = OpFNegate %90 
                          Output f32* %92 = OpAccessChain %72 %22 %68 
                                              OpStore %92 %91 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 132
; Schema: 0
                                               OpCapability Shader 
                                        %1 = OpExtInstImport "GLSL.std.450" 
                                               OpMemoryModel Logical GLSL450 
                                               OpEntryPoint Fragment %4 "main" %22 %109 
                                               OpExecutionMode %4 OriginUpperLeft 
                                               OpName vs_TEXCOORD0 "vs_TEXCOORD0" 
                                               OpDecorate %12 DescriptorSet 12 
                                               OpDecorate %12 Binding 12 
                                               OpDecorate %16 DescriptorSet 16 
                                               OpDecorate %16 Binding 16 
                                               OpDecorate vs_TEXCOORD0 Location 22 
                                               OpMemberDecorate %27 0 Offset 27 
                                               OpMemberDecorate %27 1 Offset 27 
                                               OpMemberDecorate %27 2 Offset 27 
                                               OpDecorate %27 Block 
                                               OpDecorate %29 DescriptorSet 29 
                                               OpDecorate %29 Binding 29 
                                               OpDecorate %109 Location 109 
                                        %2 = OpTypeVoid 
                                        %3 = OpTypeFunction %2 
                                        %6 = OpTypeFloat 32 
                                        %7 = OpTypeVector %6 4 
                                        %8 = OpTypePointer Private %7 
                         Private f32_4* %9 = OpVariable Private 
                                       %10 = OpTypeImage %6 Cube 0 0 0 1 Unknown 
                                       %11 = OpTypePointer UniformConstant %10 
UniformConstant read_only TextureCube* %12 = OpVariable UniformConstant 
                                       %14 = OpTypeSampler 
                                       %15 = OpTypePointer UniformConstant %14 
              UniformConstant sampler* %16 = OpVariable UniformConstant 
                                       %18 = OpTypeSampledImage %10 
                                       %20 = OpTypeVector %6 3 
                                       %21 = OpTypePointer Input %20 
                 Input f32_3* vs_TEXCOORD0 = OpVariable Input 
                                       %26 = OpTypeInt 32 1 
                                       %27 = OpTypeStruct %7 %7 %26 
                                       %28 = OpTypePointer Uniform %27 
  Uniform struct {f32_4; f32_4; i32;}* %29 = OpVariable Uniform 
                                   i32 %30 = OpConstant 1 
                                       %31 = OpTypePointer Uniform %7 
                                       %37 = OpTypePointer Private %20 
                        Private f32_3* %38 = OpVariable Private 
                                       %39 = OpTypeInt 32 0 
                                   u32 %40 = OpConstant 0 
                                       %41 = OpTypePointer Uniform %6 
                                   u32 %45 = OpConstant 1 
                                       %49 = OpTypePointer Private %6 
                                   i32 %56 = OpConstant 0 
                                   f32 %61 = OpConstant 3.674022E-40 
                                 f32_4 %62 = OpConstantComposite %61 %61 %61 %61 
                          Private f32* %65 = OpVariable Private 
                                       %69 = OpTypeBool 
                                       %70 = OpTypePointer Private %69 
                         Private bool* %71 = OpVariable Private 
                                       %75 = OpTypePointer Function %7 
                                   f32 %87 = OpConstant 3.674022E-40 
                        Private f32_3* %91 = OpVariable Private 
                                   f32 %93 = OpConstant 3.674022E-40 
                                 f32_3 %94 = OpConstantComposite %93 %93 %93 
                                   f32 %96 = OpConstant 3.674022E-40 
                                 f32_3 %97 = OpConstantComposite %96 %96 %96 
                                  f32 %102 = OpConstant 3.674022E-40 
                                f32_3 %103 = OpConstantComposite %102 %102 %102 
                                      %108 = OpTypePointer Output %7 
                        Output f32_4* %109 = OpVariable Output 
                                  i32 %110 = OpConstant 2 
                                      %111 = OpTypePointer Uniform %26 
                                      %115 = OpTypePointer Function %20 
                                  u32 %126 = OpConstant 3 
                                      %129 = OpTypePointer Output %6 
                                   void %4 = OpFunction None %3 
                                        %5 = OpLabel 
                       Function f32_4* %76 = OpVariable Function 
                      Function f32_3* %116 = OpVariable Function 
                 read_only TextureCube %13 = OpLoad %12 
                               sampler %17 = OpLoad %16 
          read_only TextureCubeSampled %19 = OpSampledImage %13 %17 
                                 f32_3 %23 = OpLoad vs_TEXCOORD0 
                                 f32_4 %24 = OpImageSampleImplicitLod %19 %23 
                                               OpStore %9 %24 
                                 f32_4 %25 = OpLoad %9 
                        Uniform f32_4* %32 = OpAccessChain %29 %30 
                                 f32_4 %33 = OpLoad %32 
                                 f32_4 %34 = OpVectorShuffle %33 %33 0 0 0 0 
                                 f32_4 %35 = OpFNegate %34 
                                 f32_4 %36 = OpFAdd %25 %35 
                                               OpStore %9 %36 
                          Uniform f32* %42 = OpAccessChain %29 %30 %40 
                                   f32 %43 = OpLoad %42 
                                   f32 %44 = OpFNegate %43 
                          Uniform f32* %46 = OpAccessChain %29 %30 %45 
                                   f32 %47 = OpLoad %46 
                                   f32 %48 = OpFAdd %44 %47 
                          Private f32* %50 = OpAccessChain %38 %40 
                                               OpStore %50 %48 
                                 f32_4 %51 = OpLoad %9 
                                 f32_3 %52 = OpLoad %38 
                                 f32_4 %53 = OpVectorShuffle %52 %52 0 0 0 0 
                                 f32_4 %54 = OpFDiv %51 %53 
                                               OpStore %9 %54 
                                 f32_4 %55 = OpLoad %9 
                        Uniform f32_4* %57 = OpAccessChain %29 %56 
                                 f32_4 %58 = OpLoad %57 
                                 f32_4 %59 = OpFMul %55 %58 
                                               OpStore %9 %59 
                                 f32_4 %60 = OpLoad %9 
                                   f32 %63 = OpDot %60 %62 
                          Private f32* %64 = OpAccessChain %38 %40 
                                               OpStore %64 %63 
                        Uniform f32_4* %66 = OpAccessChain %29 %56 
                                 f32_4 %67 = OpLoad %66 
                                   f32 %68 = OpDot %67 %62 
                                               OpStore %65 %68 
                                   f32 %72 = OpLoad %65 
                                  bool %73 = OpFOrdEqual %72 %61 
                                               OpStore %71 %73 
                                  bool %74 = OpLoad %71 
                                               OpSelectionMerge %78 None 
                                               OpBranchConditional %74 %77 %81 
                                       %77 = OpLabel 
                                 f32_3 %79 = OpLoad %38 
                                 f32_4 %80 = OpVectorShuffle %79 %79 0 0 0 0 
                                               OpStore %76 %80 
                                               OpBranch %78 
                                       %81 = OpLabel 
                                 f32_4 %82 = OpLoad %9 
                                               OpStore %76 %82 
                                               OpBranch %78 
                                       %78 = OpLabel 
                                 f32_4 %83 = OpLoad %76 
                                               OpStore %9 %83 
                                 f32_4 %84 = OpLoad %9 
                                 f32_3 %85 = OpVectorShuffle %84 %84 0 1 2 
                                               OpStore %38 %85 
                                 f32_3 %86 = OpLoad %38 
                                 f32_3 %88 = OpCompositeConstruct %87 %87 %87 
                                 f32_3 %89 = OpCompositeConstruct %61 %61 %61 
                                 f32_3 %90 = OpExtInst %1 43 %86 %88 %89 
                                               OpStore %38 %90 
                                 f32_3 %92 = OpLoad %38 
                                 f32_3 %95 = OpFMul %92 %94 
                                 f32_3 %98 = OpFAdd %95 %97 
                                               OpStore %91 %98 
                                 f32_3 %99 = OpLoad %38 
                                f32_3 %100 = OpLoad %91 
                                f32_3 %101 = OpFMul %99 %100 
                                f32_3 %104 = OpFAdd %101 %103 
                                               OpStore %91 %104 
                                f32_3 %105 = OpLoad %38 
                                f32_3 %106 = OpLoad %91 
                                f32_3 %107 = OpFMul %105 %106 
                                               OpStore %38 %107 
                         Uniform i32* %112 = OpAccessChain %29 %110 
                                  i32 %113 = OpLoad %112 
                                 bool %114 = OpINotEqual %113 %56 
                                               OpSelectionMerge %118 None 
                                               OpBranchConditional %114 %117 %120 
                                      %117 = OpLabel 
                                f32_3 %119 = OpLoad %38 
                                               OpStore %116 %119 
                                               OpBranch %118 
                                      %120 = OpLabel 
                                f32_4 %121 = OpLoad %9 
                                f32_3 %122 = OpVectorShuffle %121 %121 0 1 2 
                                               OpStore %116 %122 
                                               OpBranch %118 
                                      %118 = OpLabel 
                                f32_3 %123 = OpLoad %116 
                                f32_4 %124 = OpLoad %109 
                                f32_4 %125 = OpVectorShuffle %124 %123 4 5 6 3 
                                               OpStore %109 %125 
                         Private f32* %127 = OpAccessChain %9 %126 
                                  f32 %128 = OpLoad %127 
                          Output f32* %130 = OpAccessChain %109 %126 
                                               OpStore %130 %128 
                                               OpReturn
                                               OpFunctionEnd
"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
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
struct FGlobals_Type
{
    float4 _Channels;
    float4 _Levels;
    int _UndoOutputSRGB;
};

struct Mtl_FragmentIn
{
    float3 TEXCOORD0 [[ user(TEXCOORD0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    constant FGlobals_Type& FGlobals [[ buffer(0) ]],
    sampler sampler_MainTex [[ sampler (0) ]],
    texturecube<float, access::sample > _MainTex [[ texture(0) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float4 u_xlat0;
    float3 u_xlat1;
    float3 u_xlat2;
    float u_xlat4;
    bool u_xlatb4;
    u_xlat0 = _MainTex.sample(sampler_MainTex, input.TEXCOORD0.xyz);
    u_xlat0 = u_xlat0 + (-FGlobals._Levels.xxxx);
    u_xlat1.x = (-FGlobals._Levels.x) + FGlobals._Levels.y;
    u_xlat0 = u_xlat0 / u_xlat1.xxxx;
    u_xlat0 = u_xlat0 * FGlobals._Channels;
    u_xlat1.x = dot(u_xlat0, float4(1.0, 1.0, 1.0, 1.0));
    u_xlat4 = dot(FGlobals._Channels, float4(1.0, 1.0, 1.0, 1.0));
    u_xlatb4 = u_xlat4==1.0;
    u_xlat0 = (bool(u_xlatb4)) ? u_xlat1.xxxx : u_xlat0;
    u_xlat1.xyz = u_xlat0.xyz;
    u_xlat1.xyz = clamp(u_xlat1.xyz, 0.0f, 1.0f);
    u_xlat2.xyz = fma(u_xlat1.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat2.xyz = fma(u_xlat1.xyz, u_xlat2.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = u_xlat1.xyz * u_xlat2.xyz;
    output.SV_Target0.xyz = (FGlobals._UndoOutputSRGB != 0) ? u_xlat1.xyz : u_xlat0.xyz;
    output.SV_Target0.w = u_xlat0.w;
    return output;
}
"
}
SubProgram "gles " {
""
}
SubProgram "gles3 " {
""
}
SubProgram "glcore " {
""
}
SubProgram "vulkan " {
""
}
}
}
}
SubShader {
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 175888
Program "vp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
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
    float3 TEXCOORD0 [[ attribute(1) ]] ;
};

struct Mtl_VertexOut
{
    float4 mtl_Position [[ position ]];
    float3 TEXCOORD0 [[ user(TEXCOORD0) ]];
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
    output.TEXCOORD0.xyz = input.TEXCOORD0.xyz;
    return output;
}
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
in highp vec3 in_TEXCOORD0;
out highp vec3 vs_TEXCOORD0;
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
    vs_TEXCOORD0.xyz = in_TEXCOORD0.xyz;
    return;
}

#endif
#ifdef FRAGMENT
#version 300 es

precision highp float;
precision highp int;
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
uniform 	mediump vec4 _Channels;
uniform 	mediump vec4 _Levels;
uniform 	int _UndoOutputSRGB;
UNITY_LOCATION(0) uniform mediump sampler2DArray _MainTex;
in highp vec3 vs_TEXCOORD0;
layout(location = 0) out mediump vec4 SV_Target0;
vec4 u_xlat0;
mediump vec4 u_xlat16_0;
mediump vec3 u_xlat16_1;
vec3 u_xlat2;
bool u_xlatb2;
mediump float u_xlat16_4;
void main()
{
    u_xlat0 = texture(_MainTex, vs_TEXCOORD0.xyz);
    u_xlat16_0 = u_xlat0 + (-_Levels.xxxx);
    u_xlat16_1.x = (-_Levels.x) + _Levels.y;
    u_xlat16_0 = u_xlat16_0 / u_xlat16_1.xxxx;
    u_xlat16_0 = u_xlat16_0 * _Channels;
    u_xlat16_1.x = dot(u_xlat16_0, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlat16_4 = dot(_Channels, vec4(1.0, 1.0, 1.0, 1.0));
#ifdef UNITY_ADRENO_ES3
    u_xlatb2 = !!(u_xlat16_4==1.0);
#else
    u_xlatb2 = u_xlat16_4==1.0;
#endif
    u_xlat16_0 = (bool(u_xlatb2)) ? u_xlat16_1.xxxx : u_xlat16_0;
    u_xlat16_1.xyz = u_xlat16_0.xyz;
#ifdef UNITY_ADRENO_ES3
    u_xlat16_1.xyz = min(max(u_xlat16_1.xyz, 0.0), 1.0);
#else
    u_xlat16_1.xyz = clamp(u_xlat16_1.xyz, 0.0, 1.0);
#endif
    u_xlat2.xyz = u_xlat16_1.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat2.xyz = u_xlat16_1.xyz * u_xlat2.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat2.xyz = u_xlat16_1.xyz * u_xlat2.xyz;
    SV_Target0.xyz = (_UndoOutputSRGB != 0) ? u_xlat2.xyz : u_xlat16_0.xyz;
    SV_Target0.w = u_xlat16_0.w;
    return;
}

#endif
"
}
SubProgram "glcore " {
"#ifdef VERTEX
#version 330
#extension GL_ARB_explicit_attrib_location : require

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
in  vec3 in_TEXCOORD0;
out vec3 vs_TEXCOORD0;
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
    vs_TEXCOORD0.xyz = in_TEXCOORD0.xyz;
    return;
}

#endif
#ifdef FRAGMENT
#version 330
#extension GL_ARB_explicit_attrib_location : require

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
uniform 	vec4 _Channels;
uniform 	vec4 _Levels;
uniform 	int _UndoOutputSRGB;
UNITY_LOCATION(0) uniform  sampler2DArray _MainTex;
in  vec3 vs_TEXCOORD0;
layout(location = 0) out vec4 SV_Target0;
vec4 u_xlat0;
vec3 u_xlat1;
vec3 u_xlat2;
float u_xlat4;
bool u_xlatb4;
void main()
{
    u_xlat0 = texture(_MainTex, vs_TEXCOORD0.xyz);
    u_xlat0 = u_xlat0 + (-_Levels.xxxx);
    u_xlat1.x = (-_Levels.x) + _Levels.y;
    u_xlat0 = u_xlat0 / u_xlat1.xxxx;
    u_xlat0 = u_xlat0 * _Channels;
    u_xlat1.x = dot(u_xlat0, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlat4 = dot(_Channels, vec4(1.0, 1.0, 1.0, 1.0));
    u_xlatb4 = u_xlat4==1.0;
    u_xlat0 = (bool(u_xlatb4)) ? u_xlat1.xxxx : u_xlat0;
    u_xlat1.xyz = u_xlat0.xyz;
    u_xlat1.xyz = clamp(u_xlat1.xyz, 0.0, 1.0);
    u_xlat2.xyz = u_xlat1.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
    u_xlat2.xyz = u_xlat1.xyz * u_xlat2.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
    u_xlat1.xyz = u_xlat1.xyz * u_xlat2.xyz;
    SV_Target0.xyz = (_UndoOutputSRGB != 0) ? u_xlat1.xyz : u_xlat0.xyz;
    SV_Target0.w = u_xlat0.w;
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
                                              OpEntryPoint Vertex %4 "main" %11 %72 %84 %86 
                                              OpName vs_TEXCOORD0 "vs_TEXCOORD0" 
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
                                              OpDecorate vs_TEXCOORD0 Location 84 
                                              OpDecorate %86 Location 86 
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
                                      %82 = OpTypeVector %6 3 
                                      %83 = OpTypePointer Output %82 
               Output f32_3* vs_TEXCOORD0 = OpVariable Output 
                                      %85 = OpTypePointer Input %82 
                         Input f32_3* %86 = OpVariable Input 
                                      %88 = OpTypePointer Output %6 
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
                                f32_3 %87 = OpLoad %86 
                                              OpStore vs_TEXCOORD0 %87 
                          Output f32* %89 = OpAccessChain %72 %22 %68 
                                  f32 %90 = OpLoad %89 
                                  f32 %91 = OpFNegate %90 
                          Output f32* %92 = OpAccessChain %72 %22 %68 
                                              OpStore %92 %91 
                                              OpReturn
                                              OpFunctionEnd
; SPIR-V
; Version: 1.0
; Generator: Khronos Glslang Reference Front End; 6
; Bound: 132
; Schema: 0
                                                  OpCapability Shader 
                                           %1 = OpExtInstImport "GLSL.std.450" 
                                                  OpMemoryModel Logical GLSL450 
                                                  OpEntryPoint Fragment %4 "main" %22 %109 
                                                  OpExecutionMode %4 OriginUpperLeft 
                                                  OpName vs_TEXCOORD0 "vs_TEXCOORD0" 
                                                  OpDecorate %12 DescriptorSet 12 
                                                  OpDecorate %12 Binding 12 
                                                  OpDecorate %16 DescriptorSet 16 
                                                  OpDecorate %16 Binding 16 
                                                  OpDecorate vs_TEXCOORD0 Location 22 
                                                  OpMemberDecorate %27 0 Offset 27 
                                                  OpMemberDecorate %27 1 Offset 27 
                                                  OpMemberDecorate %27 2 Offset 27 
                                                  OpDecorate %27 Block 
                                                  OpDecorate %29 DescriptorSet 29 
                                                  OpDecorate %29 Binding 29 
                                                  OpDecorate %109 Location 109 
                                           %2 = OpTypeVoid 
                                           %3 = OpTypeFunction %2 
                                           %6 = OpTypeFloat 32 
                                           %7 = OpTypeVector %6 4 
                                           %8 = OpTypePointer Private %7 
                            Private f32_4* %9 = OpVariable Private 
                                          %10 = OpTypeImage %6 Dim2D 0 1 0 1 Unknown 
                                          %11 = OpTypePointer UniformConstant %10 
UniformConstant read_only Texture2DArray* %12 = OpVariable UniformConstant 
                                          %14 = OpTypeSampler 
                                          %15 = OpTypePointer UniformConstant %14 
                 UniformConstant sampler* %16 = OpVariable UniformConstant 
                                          %18 = OpTypeSampledImage %10 
                                          %20 = OpTypeVector %6 3 
                                          %21 = OpTypePointer Input %20 
                    Input f32_3* vs_TEXCOORD0 = OpVariable Input 
                                          %26 = OpTypeInt 32 1 
                                          %27 = OpTypeStruct %7 %7 %26 
                                          %28 = OpTypePointer Uniform %27 
     Uniform struct {f32_4; f32_4; i32;}* %29 = OpVariable Uniform 
                                      i32 %30 = OpConstant 1 
                                          %31 = OpTypePointer Uniform %7 
                                          %37 = OpTypePointer Private %20 
                           Private f32_3* %38 = OpVariable Private 
                                          %39 = OpTypeInt 32 0 
                                      u32 %40 = OpConstant 0 
                                          %41 = OpTypePointer Uniform %6 
                                      u32 %45 = OpConstant 1 
                                          %49 = OpTypePointer Private %6 
                                      i32 %56 = OpConstant 0 
                                      f32 %61 = OpConstant 3.674022E-40 
                                    f32_4 %62 = OpConstantComposite %61 %61 %61 %61 
                             Private f32* %65 = OpVariable Private 
                                          %69 = OpTypeBool 
                                          %70 = OpTypePointer Private %69 
                            Private bool* %71 = OpVariable Private 
                                          %75 = OpTypePointer Function %7 
                                      f32 %87 = OpConstant 3.674022E-40 
                           Private f32_3* %91 = OpVariable Private 
                                      f32 %93 = OpConstant 3.674022E-40 
                                    f32_3 %94 = OpConstantComposite %93 %93 %93 
                                      f32 %96 = OpConstant 3.674022E-40 
                                    f32_3 %97 = OpConstantComposite %96 %96 %96 
                                     f32 %102 = OpConstant 3.674022E-40 
                                   f32_3 %103 = OpConstantComposite %102 %102 %102 
                                         %108 = OpTypePointer Output %7 
                           Output f32_4* %109 = OpVariable Output 
                                     i32 %110 = OpConstant 2 
                                         %111 = OpTypePointer Uniform %26 
                                         %115 = OpTypePointer Function %20 
                                     u32 %126 = OpConstant 3 
                                         %129 = OpTypePointer Output %6 
                                      void %4 = OpFunction None %3 
                                           %5 = OpLabel 
                          Function f32_4* %76 = OpVariable Function 
                         Function f32_3* %116 = OpVariable Function 
                 read_only Texture2DArray %13 = OpLoad %12 
                                  sampler %17 = OpLoad %16 
          read_only Texture2DArraySampled %19 = OpSampledImage %13 %17 
                                    f32_3 %23 = OpLoad vs_TEXCOORD0 
                                    f32_4 %24 = OpImageSampleImplicitLod %19 %23 
                                                  OpStore %9 %24 
                                    f32_4 %25 = OpLoad %9 
                           Uniform f32_4* %32 = OpAccessChain %29 %30 
                                    f32_4 %33 = OpLoad %32 
                                    f32_4 %34 = OpVectorShuffle %33 %33 0 0 0 0 
                                    f32_4 %35 = OpFNegate %34 
                                    f32_4 %36 = OpFAdd %25 %35 
                                                  OpStore %9 %36 
                             Uniform f32* %42 = OpAccessChain %29 %30 %40 
                                      f32 %43 = OpLoad %42 
                                      f32 %44 = OpFNegate %43 
                             Uniform f32* %46 = OpAccessChain %29 %30 %45 
                                      f32 %47 = OpLoad %46 
                                      f32 %48 = OpFAdd %44 %47 
                             Private f32* %50 = OpAccessChain %38 %40 
                                                  OpStore %50 %48 
                                    f32_4 %51 = OpLoad %9 
                                    f32_3 %52 = OpLoad %38 
                                    f32_4 %53 = OpVectorShuffle %52 %52 0 0 0 0 
                                    f32_4 %54 = OpFDiv %51 %53 
                                                  OpStore %9 %54 
                                    f32_4 %55 = OpLoad %9 
                           Uniform f32_4* %57 = OpAccessChain %29 %56 
                                    f32_4 %58 = OpLoad %57 
                                    f32_4 %59 = OpFMul %55 %58 
                                                  OpStore %9 %59 
                                    f32_4 %60 = OpLoad %9 
                                      f32 %63 = OpDot %60 %62 
                             Private f32* %64 = OpAccessChain %38 %40 
                                                  OpStore %64 %63 
                           Uniform f32_4* %66 = OpAccessChain %29 %56 
                                    f32_4 %67 = OpLoad %66 
                                      f32 %68 = OpDot %67 %62 
                                                  OpStore %65 %68 
                                      f32 %72 = OpLoad %65 
                                     bool %73 = OpFOrdEqual %72 %61 
                                                  OpStore %71 %73 
                                     bool %74 = OpLoad %71 
                                                  OpSelectionMerge %78 None 
                                                  OpBranchConditional %74 %77 %81 
                                          %77 = OpLabel 
                                    f32_3 %79 = OpLoad %38 
                                    f32_4 %80 = OpVectorShuffle %79 %79 0 0 0 0 
                                                  OpStore %76 %80 
                                                  OpBranch %78 
                                          %81 = OpLabel 
                                    f32_4 %82 = OpLoad %9 
                                                  OpStore %76 %82 
                                                  OpBranch %78 
                                          %78 = OpLabel 
                                    f32_4 %83 = OpLoad %76 
                                                  OpStore %9 %83 
                                    f32_4 %84 = OpLoad %9 
                                    f32_3 %85 = OpVectorShuffle %84 %84 0 1 2 
                                                  OpStore %38 %85 
                                    f32_3 %86 = OpLoad %38 
                                    f32_3 %88 = OpCompositeConstruct %87 %87 %87 
                                    f32_3 %89 = OpCompositeConstruct %61 %61 %61 
                                    f32_3 %90 = OpExtInst %1 43 %86 %88 %89 
                                                  OpStore %38 %90 
                                    f32_3 %92 = OpLoad %38 
                                    f32_3 %95 = OpFMul %92 %94 
                                    f32_3 %98 = OpFAdd %95 %97 
                                                  OpStore %91 %98 
                                    f32_3 %99 = OpLoad %38 
                                   f32_3 %100 = OpLoad %91 
                                   f32_3 %101 = OpFMul %99 %100 
                                   f32_3 %104 = OpFAdd %101 %103 
                                                  OpStore %91 %104 
                                   f32_3 %105 = OpLoad %38 
                                   f32_3 %106 = OpLoad %91 
                                   f32_3 %107 = OpFMul %105 %106 
                                                  OpStore %38 %107 
                            Uniform i32* %112 = OpAccessChain %29 %110 
                                     i32 %113 = OpLoad %112 
                                    bool %114 = OpINotEqual %113 %56 
                                                  OpSelectionMerge %118 None 
                                                  OpBranchConditional %114 %117 %120 
                                         %117 = OpLabel 
                                   f32_3 %119 = OpLoad %38 
                                                  OpStore %116 %119 
                                                  OpBranch %118 
                                         %120 = OpLabel 
                                   f32_4 %121 = OpLoad %9 
                                   f32_3 %122 = OpVectorShuffle %121 %121 0 1 2 
                                                  OpStore %116 %122 
                                                  OpBranch %118 
                                         %118 = OpLabel 
                                   f32_3 %123 = OpLoad %116 
                                   f32_4 %124 = OpLoad %109 
                                   f32_4 %125 = OpVectorShuffle %124 %123 4 5 6 3 
                                                  OpStore %109 %125 
                            Private f32* %127 = OpAccessChain %9 %126 
                                     f32 %128 = OpLoad %127 
                             Output f32* %130 = OpAccessChain %109 %126 
                                                  OpStore %130 %128 
                                                  OpReturn
                                                  OpFunctionEnd
"
}
}
Program "fp" {
SubProgram "d3d11 " {
"// shader disassembly not supported on DXBC"
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
struct FGlobals_Type
{
    float4 _Channels;
    float4 _Levels;
    int _UndoOutputSRGB;
};

struct Mtl_FragmentIn
{
    float3 TEXCOORD0 [[ user(TEXCOORD0) ]] ;
};

struct Mtl_FragmentOut
{
    float4 SV_Target0 [[ color(xlt_remap_o[0]) ]];
};

fragment Mtl_FragmentOut xlatMtlMain(
    constant FGlobals_Type& FGlobals [[ buffer(0) ]],
    sampler sampler_MainTex [[ sampler (0) ]],
    texture2d_array<float, access::sample > _MainTex [[ texture(0) ]] ,
    Mtl_FragmentIn input [[ stage_in ]])
{
    Mtl_FragmentOut output;
    float4 u_xlat0;
    float3 u_xlat1;
    float3 u_xlat2;
    float u_xlat4;
    bool u_xlatb4;
    u_xlat0 = _MainTex.sample(sampler_MainTex, input.TEXCOORD0.xy, round(input.TEXCOORD0.z));
    u_xlat0 = u_xlat0 + (-FGlobals._Levels.xxxx);
    u_xlat1.x = (-FGlobals._Levels.x) + FGlobals._Levels.y;
    u_xlat0 = u_xlat0 / u_xlat1.xxxx;
    u_xlat0 = u_xlat0 * FGlobals._Channels;
    u_xlat1.x = dot(u_xlat0, float4(1.0, 1.0, 1.0, 1.0));
    u_xlat4 = dot(FGlobals._Channels, float4(1.0, 1.0, 1.0, 1.0));
    u_xlatb4 = u_xlat4==1.0;
    u_xlat0 = (bool(u_xlatb4)) ? u_xlat1.xxxx : u_xlat0;
    u_xlat1.xyz = u_xlat0.xyz;
    u_xlat1.xyz = clamp(u_xlat1.xyz, 0.0f, 1.0f);
    u_xlat2.xyz = fma(u_xlat1.xyz, float3(0.305306017, 0.305306017, 0.305306017), float3(0.682171106, 0.682171106, 0.682171106));
    u_xlat2.xyz = fma(u_xlat1.xyz, u_xlat2.xyz, float3(0.0125228781, 0.0125228781, 0.0125228781));
    u_xlat1.xyz = u_xlat1.xyz * u_xlat2.xyz;
    output.SV_Target0.xyz = (FGlobals._UndoOutputSRGB != 0) ? u_xlat1.xyz : u_xlat0.xyz;
    output.SV_Target0.w = u_xlat0.w;
    return output;
}
"
}
SubProgram "gles3 " {
""
}
SubProgram "glcore " {
""
}
SubProgram "vulkan " {
""
}
}
}
}
}