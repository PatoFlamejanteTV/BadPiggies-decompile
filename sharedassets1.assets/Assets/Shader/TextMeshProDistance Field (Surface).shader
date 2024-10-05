Shader "TextMeshPro/Distance Field (Surface)" {
	Properties {
		_FaceTex ("Fill Texture", 2D) = "white" {}
		_FaceUVSpeedX ("Face UV Speed X", Range(-5, 5)) = 0
		_FaceUVSpeedY ("Face UV Speed Y", Range(-5, 5)) = 0
		[HDR] _FaceColor ("Fill Color", Vector) = (1,1,1,1)
		_FaceDilate ("Face Dilate", Range(-1, 1)) = 0
		[HDR] _OutlineColor ("Outline Color", Vector) = (0,0,0,1)
		_OutlineTex ("Outline Texture", 2D) = "white" {}
		_OutlineUVSpeedX ("Outline UV Speed X", Range(-5, 5)) = 0
		_OutlineUVSpeedY ("Outline UV Speed Y", Range(-5, 5)) = 0
		_OutlineWidth ("Outline Thickness", Range(0, 1)) = 0
		_OutlineSoftness ("Outline Softness", Range(0, 1)) = 0
		_Bevel ("Bevel", Range(0, 1)) = 0.5
		_BevelOffset ("Bevel Offset", Range(-0.5, 0.5)) = 0
		_BevelWidth ("Bevel Width", Range(-0.5, 0.5)) = 0
		_BevelClamp ("Bevel Clamp", Range(0, 1)) = 0
		_BevelRoundness ("Bevel Roundness", Range(0, 1)) = 0
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_BumpOutline ("Bump Outline", Range(0, 1)) = 0.5
		_BumpFace ("Bump Face", Range(0, 1)) = 0.5
		_ReflectFaceColor ("Face Color", Vector) = (0,0,0,1)
		_ReflectOutlineColor ("Outline Color", Vector) = (0,0,0,1)
		_Cube ("Reflection Cubemap", Cube) = "black" {}
		_EnvMatrixRotation ("Texture Rotation", Vector) = (0,0,0,0)
		[HDR] _SpecColor ("Specular Color", Vector) = (0,0,0,1)
		_FaceShininess ("Face Shininess", Range(0, 1)) = 0
		_OutlineShininess ("Outline Shininess", Range(0, 1)) = 0
		[HDR] _GlowColor ("Color", Vector) = (0,1,0,0.5)
		_GlowOffset ("Offset", Range(-1, 1)) = 0
		_GlowInner ("Inner", Range(0, 1)) = 0.05
		_GlowOuter ("Outer", Range(0, 1)) = 0.05
		_GlowPower ("Falloff", Range(1, 0)) = 0.75
		_WeightNormal ("Weight Normal", Float) = 0
		_WeightBold ("Weight Bold", Float) = 0.5
		_ShaderFlags ("Flags", Float) = 0
		_ScaleRatioA ("Scale RatioA", Float) = 1
		_ScaleRatioB ("Scale RatioB", Float) = 1
		_ScaleRatioC ("Scale RatioC", Float) = 1
		_MainTex ("Font Atlas", 2D) = "white" {}
		_TextureWidth ("Texture Width", Float) = 512
		_TextureHeight ("Texture Height", Float) = 512
		_GradientScale ("Gradient Scale", Float) = 5
		_ScaleX ("Scale X", Float) = 1
		_ScaleY ("Scale Y", Float) = 1
		_PerspectiveFilter ("Perspective Correction", Range(0, 1)) = 0.875
		_Sharpness ("Sharpness", Range(-1, 1)) = 0
		_VertexOffsetX ("Vertex OffsetX", Float) = 0
		_VertexOffsetY ("Vertex OffsetY", Float) = 0
		_CullMode ("Cull Mode", Float) = 0
	}
	SubShader {
		LOD 300
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			Name "FORWARD"
			LOD 300
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ColorMask RGB -1
			ZWrite Off
			Cull Off
			GpuProgramID 31442
			Program "vp" {
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[6];
						float _FaceDilate;
						vec4 unused_0_2[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_4[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_10[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_2_0[42];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_2_5[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_3_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_3_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_4_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_4_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD5;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD7;
					vec4 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					int u_xlati5;
					vec3 u_xlat7;
					float u_xlat15;
					bool u_xlatb15;
					float u_xlat16;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat16 = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat16 + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD5.x = u_xlat15 * 0.5;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat16 = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat2.x = _Sharpness + 1.0;
					    u_xlat16 = u_xlat16 * u_xlat2.x;
					    u_xlat2.x = u_xlat15 * u_xlat16;
					    u_xlat7.x = (-_PerspectiveFilter) + 1.0;
					    u_xlat2.x = u_xlat7.x * u_xlat2.x;
					    u_xlat15 = u_xlat15 * u_xlat16 + (-u_xlat2.x);
					    u_xlat7.xyz = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat7.xyz = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat7.xyz;
					    u_xlat7.xyz = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat7.xyz;
					    u_xlat7.xyz = u_xlat7.xyz + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat7.xyz;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat3 = u_xlat0.xxxx * u_xlat3.xyzz;
					    u_xlat0.xyz = (-u_xlat1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat16 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat16 = inversesqrt(u_xlat16);
					    u_xlat7.xyz = u_xlat0.xyz * vec3(u_xlat16);
					    u_xlat16 = dot(u_xlat3.xyw, u_xlat7.xyz);
					    vs_TEXCOORD5.y = abs(u_xlat16) * u_xlat15 + u_xlat2.x;
					    vs_TEXCOORD2.w = u_xlat1.x;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat2.xyz * u_xlat3.wxy;
					    u_xlat4.xyz = u_xlat3.ywx * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat15 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat15) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.z = u_xlat3.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.w = u_xlat1.y;
					    vs_TEXCOORD4.w = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_TEXCOORD4.z = u_xlat3.w;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat1.xyz = u_xlat0.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyw = _EnvMatrix[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    vs_TEXCOORD6.xyz = _EnvMatrix[2].xyz * u_xlat0.zzz + u_xlat0.xyw;
					    u_xlat0.x = u_xlat3.y * u_xlat3.y;
					    u_xlat0.x = u_xlat3.x * u_xlat3.x + (-u_xlat0.x);
					    u_xlat1 = u_xlat3.ywzx * u_xlat3;
					    u_xlat2.x = dot(unity_SHBr, u_xlat1);
					    u_xlat2.y = dot(unity_SHBg, u_xlat1);
					    u_xlat2.z = dot(unity_SHBb, u_xlat1);
					    vs_TEXCOORD7.xyz = unity_SHC.xyz * u_xlat0.xxx + u_xlat2.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "GLOW_ON" "LIGHTPROBE_SH" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[6];
						float _FaceDilate;
						vec4 unused_0_2[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_4[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_10[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_2_0[42];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_2_5[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_3_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_3_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_4_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_4_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD5;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD7;
					vec4 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					int u_xlati5;
					vec3 u_xlat7;
					float u_xlat15;
					bool u_xlatb15;
					float u_xlat16;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat16 = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat16 + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD5.x = u_xlat15 * 0.5;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat16 = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat2.x = _Sharpness + 1.0;
					    u_xlat16 = u_xlat16 * u_xlat2.x;
					    u_xlat2.x = u_xlat15 * u_xlat16;
					    u_xlat7.x = (-_PerspectiveFilter) + 1.0;
					    u_xlat2.x = u_xlat7.x * u_xlat2.x;
					    u_xlat15 = u_xlat15 * u_xlat16 + (-u_xlat2.x);
					    u_xlat7.xyz = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat7.xyz = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat7.xyz;
					    u_xlat7.xyz = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat7.xyz;
					    u_xlat7.xyz = u_xlat7.xyz + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat7.xyz;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat3 = u_xlat0.xxxx * u_xlat3.xyzz;
					    u_xlat0.xyz = (-u_xlat1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat16 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat16 = inversesqrt(u_xlat16);
					    u_xlat7.xyz = u_xlat0.xyz * vec3(u_xlat16);
					    u_xlat16 = dot(u_xlat3.xyw, u_xlat7.xyz);
					    vs_TEXCOORD5.y = abs(u_xlat16) * u_xlat15 + u_xlat2.x;
					    vs_TEXCOORD2.w = u_xlat1.x;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat2.xyz * u_xlat3.wxy;
					    u_xlat4.xyz = u_xlat3.ywx * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat15 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat15) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.z = u_xlat3.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.w = u_xlat1.y;
					    vs_TEXCOORD4.w = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_TEXCOORD4.z = u_xlat3.w;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat1.xyz = u_xlat0.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyw = _EnvMatrix[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    vs_TEXCOORD6.xyz = _EnvMatrix[2].xyz * u_xlat0.zzz + u_xlat0.xyw;
					    u_xlat0.x = u_xlat3.y * u_xlat3.y;
					    u_xlat0.x = u_xlat3.x * u_xlat3.x + (-u_xlat0.x);
					    u_xlat1 = u_xlat3.ywzx * u_xlat3;
					    u_xlat2.x = dot(unity_SHBr, u_xlat1);
					    u_xlat2.y = dot(unity_SHBg, u_xlat1);
					    u_xlat2.z = dot(unity_SHBb, u_xlat1);
					    vs_TEXCOORD7.xyz = unity_SHC.xyz * u_xlat0.xxx + u_xlat2.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "LIGHTPROBE_SH" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[6];
						float _FaceDilate;
						vec4 unused_0_2[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_4[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_10[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_2_0[42];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_2_5[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_3_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_3_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_4_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_4_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD5;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD7;
					vec4 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					int u_xlati5;
					vec3 u_xlat7;
					float u_xlat15;
					bool u_xlatb15;
					float u_xlat16;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat16 = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat16 + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD5.x = u_xlat15 * 0.5;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat16 = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat2.x = _Sharpness + 1.0;
					    u_xlat16 = u_xlat16 * u_xlat2.x;
					    u_xlat2.x = u_xlat15 * u_xlat16;
					    u_xlat7.x = (-_PerspectiveFilter) + 1.0;
					    u_xlat2.x = u_xlat7.x * u_xlat2.x;
					    u_xlat15 = u_xlat15 * u_xlat16 + (-u_xlat2.x);
					    u_xlat7.xyz = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat7.xyz = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat7.xyz;
					    u_xlat7.xyz = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat7.xyz;
					    u_xlat7.xyz = u_xlat7.xyz + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat7.xyz;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat3 = u_xlat0.xxxx * u_xlat3.xyzz;
					    u_xlat0.xyz = (-u_xlat1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat16 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat16 = inversesqrt(u_xlat16);
					    u_xlat7.xyz = u_xlat0.xyz * vec3(u_xlat16);
					    u_xlat16 = dot(u_xlat3.xyw, u_xlat7.xyz);
					    vs_TEXCOORD5.y = abs(u_xlat16) * u_xlat15 + u_xlat2.x;
					    vs_TEXCOORD2.w = u_xlat1.x;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat2.xyz * u_xlat3.wxy;
					    u_xlat4.xyz = u_xlat3.ywx * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat15 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat15) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.z = u_xlat3.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.w = u_xlat1.y;
					    vs_TEXCOORD4.w = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_TEXCOORD4.z = u_xlat3.w;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat1.xyz = u_xlat0.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyw = _EnvMatrix[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    vs_TEXCOORD6.xyz = _EnvMatrix[2].xyz * u_xlat0.zzz + u_xlat0.xyw;
					    u_xlat0.x = u_xlat3.y * u_xlat3.y;
					    u_xlat0.x = u_xlat3.x * u_xlat3.x + (-u_xlat0.x);
					    u_xlat1 = u_xlat3.ywzx * u_xlat3;
					    u_xlat2.x = dot(unity_SHBr, u_xlat1);
					    u_xlat2.y = dot(unity_SHBg, u_xlat1);
					    u_xlat2.z = dot(unity_SHBb, u_xlat1);
					    vs_TEXCOORD7.xyz = unity_SHC.xyz * u_xlat0.xxx + u_xlat2.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "GLOW_ON" "LIGHTPROBE_SH" "VERTEXLIGHT_ON" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[6];
						float _FaceDilate;
						vec4 unused_0_2[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_4[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_10[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_2_0[3];
						vec4 unity_4LightPosX0;
						vec4 unity_4LightPosY0;
						vec4 unity_4LightPosZ0;
						vec4 unity_4LightAtten0;
						vec4 unity_LightColor[8];
						vec4 unused_2_6[34];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_2_11[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_3_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_3_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_4_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_4_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD5;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD7;
					vec4 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					int u_xlati5;
					vec3 u_xlat7;
					float u_xlat15;
					bool u_xlatb15;
					float u_xlat16;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat16 = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat16 + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD5.x = u_xlat15 * 0.5;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat16 = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat2.x = _Sharpness + 1.0;
					    u_xlat16 = u_xlat16 * u_xlat2.x;
					    u_xlat2.x = u_xlat15 * u_xlat16;
					    u_xlat7.x = (-_PerspectiveFilter) + 1.0;
					    u_xlat2.x = u_xlat7.x * u_xlat2.x;
					    u_xlat15 = u_xlat15 * u_xlat16 + (-u_xlat2.x);
					    u_xlat7.xyz = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat7.xyz = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat7.xyz;
					    u_xlat7.xyz = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat7.xyz;
					    u_xlat7.xyz = u_xlat7.xyz + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat7.xyz;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat3 = u_xlat0.xxxx * u_xlat3.xyzz;
					    u_xlat0.xyz = (-u_xlat1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat16 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat16 = inversesqrt(u_xlat16);
					    u_xlat7.xyz = u_xlat0.xyz * vec3(u_xlat16);
					    u_xlat16 = dot(u_xlat3.xyw, u_xlat7.xyz);
					    vs_TEXCOORD5.y = abs(u_xlat16) * u_xlat15 + u_xlat2.x;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat2.xyz * u_xlat3.wxy;
					    u_xlat4.xyz = u_xlat3.ywx * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat15 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat15) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.z = u_xlat3.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD2.w = u_xlat1.x;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.z = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_TEXCOORD3.w = u_xlat1.y;
					    vs_TEXCOORD4.z = u_xlat3.w;
					    vs_TEXCOORD4.w = u_xlat1.z;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat2.xyz = u_xlat0.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyw = _EnvMatrix[0].xyz * u_xlat0.xxx + u_xlat2.xyz;
					    vs_TEXCOORD6.xyz = _EnvMatrix[2].xyz * u_xlat0.zzz + u_xlat0.xyw;
					    u_xlat0 = (-u_xlat1.yyyy) + unity_4LightPosY0;
					    u_xlat2 = u_xlat3.yyyy * u_xlat0;
					    u_xlat0 = u_xlat0 * u_xlat0;
					    u_xlat4 = (-u_xlat1.xxxx) + unity_4LightPosX0;
					    u_xlat1 = (-u_xlat1.zzzz) + unity_4LightPosZ0;
					    u_xlat2 = u_xlat4 * u_xlat3.xxxx + u_xlat2;
					    u_xlat0 = u_xlat4 * u_xlat4 + u_xlat0;
					    u_xlat0 = u_xlat1 * u_xlat1 + u_xlat0;
					    u_xlat1 = u_xlat1 * u_xlat3.wwzw + u_xlat2;
					    u_xlat0 = max(u_xlat0, vec4(9.99999997e-07, 9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
					    u_xlat2 = inversesqrt(u_xlat0);
					    u_xlat0 = u_xlat0 * unity_4LightAtten0 + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat0 = vec4(1.0, 1.0, 1.0, 1.0) / u_xlat0;
					    u_xlat1 = u_xlat1 * u_xlat2;
					    u_xlat1 = max(u_xlat1, vec4(0.0, 0.0, 0.0, 0.0));
					    u_xlat0 = u_xlat0 * u_xlat1;
					    u_xlat1.xyz = u_xlat0.yyy * unity_LightColor[1].xyz;
					    u_xlat1.xyz = unity_LightColor[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_LightColor[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    u_xlat0.xyz = unity_LightColor[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    u_xlat1.xyz = u_xlat0.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
					    u_xlat1.xyz = u_xlat0.xyz * u_xlat1.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
					    u_xlat15 = u_xlat3.y * u_xlat3.y;
					    u_xlat15 = u_xlat3.x * u_xlat3.x + (-u_xlat15);
					    u_xlat2 = u_xlat3.ywzx * u_xlat3;
					    u_xlat3.x = dot(unity_SHBr, u_xlat2);
					    u_xlat3.y = dot(unity_SHBg, u_xlat2);
					    u_xlat3.z = dot(unity_SHBb, u_xlat2);
					    u_xlat2.xyz = unity_SHC.xyz * vec3(u_xlat15) + u_xlat3.xyz;
					    vs_TEXCOORD7.xyz = u_xlat0.xyz * u_xlat1.xyz + u_xlat2.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "LIGHTPROBE_SH" "VERTEXLIGHT_ON" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[6];
						float _FaceDilate;
						vec4 unused_0_2[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_4[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_10[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_2_0[3];
						vec4 unity_4LightPosX0;
						vec4 unity_4LightPosY0;
						vec4 unity_4LightPosZ0;
						vec4 unity_4LightAtten0;
						vec4 unity_LightColor[8];
						vec4 unused_2_6[34];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_2_11[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_3_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_3_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_4_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_4_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD5;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD7;
					vec4 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					int u_xlati5;
					vec3 u_xlat7;
					float u_xlat15;
					bool u_xlatb15;
					float u_xlat16;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat16 = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat16 + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD5.x = u_xlat15 * 0.5;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat16 = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat2.x = _Sharpness + 1.0;
					    u_xlat16 = u_xlat16 * u_xlat2.x;
					    u_xlat2.x = u_xlat15 * u_xlat16;
					    u_xlat7.x = (-_PerspectiveFilter) + 1.0;
					    u_xlat2.x = u_xlat7.x * u_xlat2.x;
					    u_xlat15 = u_xlat15 * u_xlat16 + (-u_xlat2.x);
					    u_xlat7.xyz = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat7.xyz = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat7.xyz;
					    u_xlat7.xyz = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat7.xyz;
					    u_xlat7.xyz = u_xlat7.xyz + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat7.xyz;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat3 = u_xlat0.xxxx * u_xlat3.xyzz;
					    u_xlat0.xyz = (-u_xlat1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat16 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat16 = inversesqrt(u_xlat16);
					    u_xlat7.xyz = u_xlat0.xyz * vec3(u_xlat16);
					    u_xlat16 = dot(u_xlat3.xyw, u_xlat7.xyz);
					    vs_TEXCOORD5.y = abs(u_xlat16) * u_xlat15 + u_xlat2.x;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat2.xyz * u_xlat3.wxy;
					    u_xlat4.xyz = u_xlat3.ywx * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat15 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat15) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.z = u_xlat3.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD2.w = u_xlat1.x;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.z = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_TEXCOORD3.w = u_xlat1.y;
					    vs_TEXCOORD4.z = u_xlat3.w;
					    vs_TEXCOORD4.w = u_xlat1.z;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat2.xyz = u_xlat0.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyw = _EnvMatrix[0].xyz * u_xlat0.xxx + u_xlat2.xyz;
					    vs_TEXCOORD6.xyz = _EnvMatrix[2].xyz * u_xlat0.zzz + u_xlat0.xyw;
					    u_xlat0 = (-u_xlat1.yyyy) + unity_4LightPosY0;
					    u_xlat2 = u_xlat3.yyyy * u_xlat0;
					    u_xlat0 = u_xlat0 * u_xlat0;
					    u_xlat4 = (-u_xlat1.xxxx) + unity_4LightPosX0;
					    u_xlat1 = (-u_xlat1.zzzz) + unity_4LightPosZ0;
					    u_xlat2 = u_xlat4 * u_xlat3.xxxx + u_xlat2;
					    u_xlat0 = u_xlat4 * u_xlat4 + u_xlat0;
					    u_xlat0 = u_xlat1 * u_xlat1 + u_xlat0;
					    u_xlat1 = u_xlat1 * u_xlat3.wwzw + u_xlat2;
					    u_xlat0 = max(u_xlat0, vec4(9.99999997e-07, 9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
					    u_xlat2 = inversesqrt(u_xlat0);
					    u_xlat0 = u_xlat0 * unity_4LightAtten0 + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat0 = vec4(1.0, 1.0, 1.0, 1.0) / u_xlat0;
					    u_xlat1 = u_xlat1 * u_xlat2;
					    u_xlat1 = max(u_xlat1, vec4(0.0, 0.0, 0.0, 0.0));
					    u_xlat0 = u_xlat0 * u_xlat1;
					    u_xlat1.xyz = u_xlat0.yyy * unity_LightColor[1].xyz;
					    u_xlat1.xyz = unity_LightColor[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_LightColor[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    u_xlat0.xyz = unity_LightColor[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    u_xlat1.xyz = u_xlat0.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
					    u_xlat1.xyz = u_xlat0.xyz * u_xlat1.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
					    u_xlat15 = u_xlat3.y * u_xlat3.y;
					    u_xlat15 = u_xlat3.x * u_xlat3.x + (-u_xlat15);
					    u_xlat2 = u_xlat3.ywzx * u_xlat3;
					    u_xlat3.x = dot(unity_SHBr, u_xlat2);
					    u_xlat3.y = dot(unity_SHBg, u_xlat2);
					    u_xlat3.z = dot(unity_SHBb, u_xlat2);
					    u_xlat2.xyz = unity_SHC.xyz * vec3(u_xlat15) + u_xlat3.xyz;
					    vs_TEXCOORD7.xyz = u_xlat0.xyz * u_xlat1.xyz + u_xlat2.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "GLOW_ON" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[6];
						float _FaceDilate;
						vec4 unused_0_2[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_4[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_10[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_2_0[42];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_2_5[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_3_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_3_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_4_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_4_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD5;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD7;
					vec4 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					int u_xlati5;
					vec3 u_xlat7;
					float u_xlat15;
					bool u_xlatb15;
					float u_xlat16;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat16 = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat16 + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD5.x = u_xlat15 * 0.5;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat16 = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat2.x = _Sharpness + 1.0;
					    u_xlat16 = u_xlat16 * u_xlat2.x;
					    u_xlat2.x = u_xlat15 * u_xlat16;
					    u_xlat7.x = (-_PerspectiveFilter) + 1.0;
					    u_xlat2.x = u_xlat7.x * u_xlat2.x;
					    u_xlat15 = u_xlat15 * u_xlat16 + (-u_xlat2.x);
					    u_xlat7.xyz = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat7.xyz = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat7.xyz;
					    u_xlat7.xyz = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat7.xyz;
					    u_xlat7.xyz = u_xlat7.xyz + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat7.xyz;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat3 = u_xlat0.xxxx * u_xlat3.xyzz;
					    u_xlat0.xyz = (-u_xlat1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat16 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat16 = inversesqrt(u_xlat16);
					    u_xlat7.xyz = u_xlat0.xyz * vec3(u_xlat16);
					    u_xlat16 = dot(u_xlat3.xyw, u_xlat7.xyz);
					    vs_TEXCOORD5.y = abs(u_xlat16) * u_xlat15 + u_xlat2.x;
					    vs_TEXCOORD2.w = u_xlat1.x;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat2.xyz * u_xlat3.wxy;
					    u_xlat4.xyz = u_xlat3.ywx * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat15 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat15) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.z = u_xlat3.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.w = u_xlat1.y;
					    vs_TEXCOORD4.w = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_TEXCOORD4.z = u_xlat3.w;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat1.xyz = u_xlat0.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyw = _EnvMatrix[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    vs_TEXCOORD6.xyz = _EnvMatrix[2].xyz * u_xlat0.zzz + u_xlat0.xyw;
					    u_xlat0.x = u_xlat3.y * u_xlat3.y;
					    u_xlat0.x = u_xlat3.x * u_xlat3.x + (-u_xlat0.x);
					    u_xlat1 = u_xlat3.ywzx * u_xlat3;
					    u_xlat2.x = dot(unity_SHBr, u_xlat1);
					    u_xlat2.y = dot(unity_SHBg, u_xlat1);
					    u_xlat2.z = dot(unity_SHBb, u_xlat1);
					    vs_TEXCOORD7.xyz = unity_SHC.xyz * u_xlat0.xxx + u_xlat2.xyz;
					    return;
					}"
				}
			}
			Program "fp" {
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 _ReflectFaceColor;
						vec4 _ReflectOutlineColor;
						vec4 unused_0_20[12];
						float _ShaderFlags;
						float _ScaleRatioA;
						vec4 unused_0_23[4];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_27;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_30[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[38];
						vec4 unity_SHAr;
						vec4 unity_SHAg;
						vec4 unity_SHAb;
						vec4 unused_2_5[4];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_7;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						vec4 unused_3_1[7];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  samplerCube _Cube;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD5;
					in  vec4 vs_TEXCOORD2;
					in  vec4 vs_TEXCOORD3;
					in  vec4 vs_TEXCOORD4;
					in  vec4 vs_COLOR0;
					in  vec3 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD7;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					bool u_xlatb4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec3 u_xlat9;
					vec3 u_xlat10;
					float u_xlat11;
					float u_xlat13;
					bool u_xlatb13;
					float u_xlat14;
					float u_xlat18;
					float u_xlat20;
					float u_xlat27;
					float u_xlat28;
					float u_xlat29;
					float u_xlat30;
					bool u_xlatb30;
					float u_xlat31;
					bool u_xlatb31;
					void main()
					{
					    u_xlat9.x = vs_TEXCOORD2.w;
					    u_xlat9.y = vs_TEXCOORD3.w;
					    u_xlat9.z = vs_TEXCOORD4.w;
					    u_xlat1.xyz = (-u_xlat9.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat28 = (-u_xlat2.w) + 0.5;
					    u_xlat28 = u_xlat28 + (-vs_TEXCOORD5.x);
					    u_xlat28 = u_xlat28 * vs_TEXCOORD5.y + 0.5;
					    u_xlat2.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat2.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat2.xz = u_xlat2.xy * vs_TEXCOORD5.yy;
					    u_xlat3 = vs_COLOR0 * _FaceColor;
					    u_xlat29 = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat4.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3 * u_xlat4;
					    u_xlat4.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat4.xy);
					    u_xlat4.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat5.w = u_xlat29 * u_xlat4.w;
					    u_xlat29 = (-u_xlat2.x) * 0.5 + u_xlat28;
					    u_xlat20 = u_xlat2.z * 0.5 + u_xlat29;
					    u_xlat11 = u_xlat2.y * vs_TEXCOORD5.y + 1.0;
					    u_xlat11 = u_xlat20 / u_xlat11;
					    u_xlat11 = clamp(u_xlat11, 0.0, 1.0);
					    u_xlat11 = (-u_xlat11) + 1.0;
					    u_xlat28 = u_xlat2.x * 0.5 + u_xlat28;
					    u_xlat28 = clamp(u_xlat28, 0.0, 1.0);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat2.x = u_xlat28 * u_xlat2.x;
					    u_xlat3.xyz = u_xlat3.www * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat4.xyz * u_xlat5.www;
					    u_xlat4 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = u_xlat2.xxxx * u_xlat4 + u_xlat3;
					    u_xlat2 = vec4(u_xlat11) * u_xlat3;
					    u_xlat3.x = max(u_xlat2.w, 9.99999975e-05);
					    u_xlat2.xyz = u_xlat2.xyz / u_xlat3.xxx;
					    u_xlat3.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat3.z = 0.0;
					    u_xlat4 = (-u_xlat3.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat5 = texture(_MainTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat6 = texture(_MainTex, u_xlat3.xy);
					    u_xlat4 = texture(_MainTex, u_xlat4.zw);
					    u_xlat3 = texture(_MainTex, u_xlat3.zw);
					    u_xlat4.x = _ShaderFlags * 0.5;
					    u_xlatb13 = u_xlat4.x>=(-u_xlat4.x);
					    u_xlat4.x = fract(abs(u_xlat4.x));
					    u_xlat4.x = (u_xlatb13) ? u_xlat4.x : (-u_xlat4.x);
					    u_xlatb4 = u_xlat4.x>=0.5;
					    u_xlat13 = vs_TEXCOORD5.x + _BevelOffset;
					    u_xlat3.x = u_xlat5.w;
					    u_xlat3.y = u_xlat6.w;
					    u_xlat3.z = u_xlat4.w;
					    u_xlat3 = vec4(u_xlat13) + u_xlat3;
					    u_xlat13 = _BevelWidth + _OutlineWidth;
					    u_xlat13 = max(u_xlat13, 0.00999999978);
					    u_xlat3 = u_xlat3 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat3 = u_xlat3 / vec4(u_xlat13);
					    u_xlat3 = u_xlat3 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat3 = clamp(u_xlat3, 0.0, 1.0);
					    u_xlat5 = u_xlat3 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat5 = -abs(u_xlat5) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat3 = (bool(u_xlatb4)) ? u_xlat5 : u_xlat3;
					    u_xlat5 = u_xlat3 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat5 = sin(u_xlat5);
					    u_xlat5 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat5 + u_xlat3;
					    u_xlat4.x = (-_BevelClamp) + 1.0;
					    u_xlat3 = min(u_xlat3, u_xlat4.xxxx);
					    u_xlat4.x = u_xlat13 * _Bevel;
					    u_xlat4.x = u_xlat4.x * _GradientScale;
					    u_xlat4.x = u_xlat4.x * -2.0;
					    u_xlat3.xz = u_xlat3.xz * u_xlat4.xx;
					    u_xlat3.yz = u_xlat3.wy * u_xlat4.xx + (-u_xlat3.zx);
					    u_xlat3.x = float(-1.0);
					    u_xlat3.w = float(1.0);
					    u_xlat30 = dot(u_xlat3.zw, u_xlat3.zw);
					    u_xlat30 = inversesqrt(u_xlat30);
					    u_xlat4.yz = vec2(u_xlat30) * vec2(1.0, 0.0);
					    u_xlat4.x = u_xlat30 * u_xlat3.z;
					    u_xlat30 = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat30 = inversesqrt(u_xlat30);
					    u_xlat3.z = 0.0;
					    u_xlat3.xyz = vec3(u_xlat30) * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat4.xyz;
					    u_xlat3.xyz = u_xlat4.zxy * u_xlat3.yzx + (-u_xlat5.xyz);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat30 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat30 = min(u_xlat30, 1.0);
					    u_xlat30 = (-u_xlat30) + 1.0;
					    u_xlat4.z = sqrt(u_xlat30);
					    u_xlat30 = (-_BumpFace) + _BumpOutline;
					    u_xlat30 = u_xlat28 * u_xlat30 + _BumpFace;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat30) + vec3(-0.0, -0.0, -1.0);
					    u_xlat4.xyz = u_xlat2.www * u_xlat4.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat3.xyz = u_xlat3.xyz + (-u_xlat4.xyz);
					    u_xlat30 = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat30 = inversesqrt(u_xlat30);
					    u_xlat3.xyz = vec3(u_xlat30) * u_xlat3.xyz;
					    u_xlat4.xyz = u_xlat3.yyy * unity_ObjectToWorld[1].xyz;
					    u_xlat4.xyz = unity_ObjectToWorld[0].xyz * u_xlat3.xxx + u_xlat4.xyz;
					    u_xlat4.xyz = unity_ObjectToWorld[2].xyz * u_xlat3.zzz + u_xlat4.xyz;
					    u_xlat30 = dot(vs_TEXCOORD6.xyz, u_xlat4.xyz);
					    u_xlat30 = u_xlat30 + u_xlat30;
					    u_xlat4.xyz = u_xlat4.xyz * (-vec3(u_xlat30)) + vs_TEXCOORD6.xyz;
					    u_xlat4 = texture(_Cube, u_xlat4.xyz);
					    u_xlat5.xyz = (-_ReflectFaceColor.xyz) + _ReflectOutlineColor.xyz;
					    u_xlat5.xyz = vec3(u_xlat28) * u_xlat5.xyz + _ReflectFaceColor.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * u_xlat5.xyz;
					    u_xlat30 = (-unused_0_27.y) + unused_0_27.z;
					    u_xlat28 = u_xlat28 * u_xlat30 + unused_0_27.y;
					    u_xlatb30 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb30){
					        u_xlatb31 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD3.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD2.www + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.www + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb31)) ? u_xlat5.xyz : u_xlat9.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat31 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat14 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat31, u_xlat14);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat31 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat31 = clamp(u_xlat31, 0.0, 1.0);
					    u_xlat5.x = dot(vs_TEXCOORD2.xyz, (-u_xlat3.xyz));
					    u_xlat5.y = dot(vs_TEXCOORD3.xyz, (-u_xlat3.xyz));
					    u_xlat5.z = dot(vs_TEXCOORD4.xyz, (-u_xlat3.xyz));
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat3.x = inversesqrt(u_xlat3.x);
					    u_xlat5.xyz = u_xlat3.xxx * u_xlat5.xyz;
					    u_xlat3.xyz = vec3(u_xlat31) * _LightColor0.xyz;
					    if(u_xlatb30){
					        u_xlatb30 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD3.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD2.www + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.www + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat9.xyz = (bool(u_xlatb30)) ? u_xlat6.xyz : u_xlat9.xyz;
					        u_xlat9.xyz = u_xlat9.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat9.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat9.x = u_xlat6.y * 0.25;
					        u_xlat18 = unity_ProbeVolumeParams.z * 0.5;
					        u_xlat27 = (-unity_ProbeVolumeParams.z) * 0.5 + 0.25;
					        u_xlat9.x = max(u_xlat18, u_xlat9.x);
					        u_xlat6.x = min(u_xlat27, u_xlat9.x);
					        u_xlat7 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					        u_xlat9.xyz = u_xlat6.xzw + vec3(0.25, 0.0, 0.0);
					        u_xlat8 = texture(unity_ProbeVolumeSH, u_xlat9.xyz);
					        u_xlat9.xyz = u_xlat6.xzw + vec3(0.5, 0.0, 0.0);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat9.xyz);
					        u_xlat5.w = 1.0;
					        u_xlat7.x = dot(u_xlat7, u_xlat5);
					        u_xlat7.y = dot(u_xlat8, u_xlat5);
					        u_xlat7.z = dot(u_xlat6, u_xlat5);
					    } else {
					        u_xlat5.w = 1.0;
					        u_xlat7.x = dot(unity_SHAr, u_xlat5);
					        u_xlat7.y = dot(unity_SHAg, u_xlat5);
					        u_xlat7.z = dot(unity_SHAb, u_xlat5);
					    }
					    u_xlat9.xyz = u_xlat7.xyz + vs_TEXCOORD7.xyz;
					    u_xlat9.xyz = max(u_xlat9.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat9.xyz = log2(u_xlat9.xyz);
					    u_xlat9.xyz = u_xlat9.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
					    u_xlat9.xyz = exp2(u_xlat9.xyz);
					    u_xlat9.xyz = u_xlat9.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    u_xlat9.xyz = max(u_xlat9.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat1.xyz = u_xlat1.xyz * u_xlat0.xxx + _WorldSpaceLightPos0.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat1.xyz = u_xlat0.xxx * u_xlat1.xyz;
					    u_xlat0.x = dot(u_xlat5.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat0.x = max(u_xlat0.x, 0.0);
					    u_xlat1.x = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat1.x = max(u_xlat1.x, 0.0);
					    u_xlat10.x = u_xlat28 * 128.0;
					    u_xlat1.x = log2(u_xlat1.x);
					    u_xlat1.x = u_xlat1.x * u_xlat10.x;
					    u_xlat1.x = exp2(u_xlat1.x);
					    u_xlat10.xyz = u_xlat2.xyz * u_xlat3.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * _SpecColor.xyz;
					    u_xlat3.xyz = u_xlat1.xxx * u_xlat3.xyz;
					    u_xlat1.xyz = u_xlat10.xyz * u_xlat0.xxx + u_xlat3.xyz;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat9.xyz + u_xlat1.xyz;
					    SV_Target0.xyz = u_xlat4.xyz * u_xlat2.www + u_xlat0.xyz;
					    SV_Target0.w = u_xlat2.w;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "GLOW_ON" "LIGHTPROBE_SH" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 _ReflectFaceColor;
						vec4 _ReflectOutlineColor;
						vec4 unused_0_20[10];
						vec4 _GlowColor;
						float _GlowOffset;
						float _GlowOuter;
						float _GlowInner;
						float _GlowPower;
						float _ShaderFlags;
						float _ScaleRatioA;
						float _ScaleRatioB;
						vec4 unused_0_29[3];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_33;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_36[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[38];
						vec4 unity_SHAr;
						vec4 unity_SHAg;
						vec4 unity_SHAb;
						vec4 unused_2_5[4];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_7;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						vec4 unused_3_1[7];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  samplerCube _Cube;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD5;
					in  vec4 vs_TEXCOORD2;
					in  vec4 vs_TEXCOORD3;
					in  vec4 vs_TEXCOORD4;
					in  vec4 vs_COLOR0;
					in  vec3 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD7;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					bool u_xlatb2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec3 u_xlat9;
					float u_xlat10;
					float u_xlat11;
					vec3 u_xlat12;
					bool u_xlatb12;
					float u_xlat18;
					float u_xlat20;
					float u_xlat21;
					float u_xlat27;
					float u_xlat28;
					float u_xlat29;
					bool u_xlatb29;
					void main()
					{
					    u_xlat9.x = vs_TEXCOORD2.w;
					    u_xlat9.y = vs_TEXCOORD3.w;
					    u_xlat9.z = vs_TEXCOORD4.w;
					    u_xlat1.xyz = (-u_xlat9.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat28 = (-u_xlat2.w) + 0.5;
					    u_xlat28 = u_xlat28 + (-vs_TEXCOORD5.x);
					    u_xlat28 = u_xlat28 * vs_TEXCOORD5.y + 0.5;
					    u_xlat2.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat2.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat2.xz = u_xlat2.xy * vs_TEXCOORD5.yy;
					    u_xlat3 = vs_COLOR0 * _FaceColor;
					    u_xlat29 = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat4.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3 * u_xlat4;
					    u_xlat4.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat4.xy);
					    u_xlat4.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat5.w = u_xlat29 * u_xlat4.w;
					    u_xlat29 = (-u_xlat2.x) * 0.5 + u_xlat28;
					    u_xlat20 = u_xlat2.z * 0.5 + u_xlat29;
					    u_xlat11 = u_xlat2.y * vs_TEXCOORD5.y + 1.0;
					    u_xlat11 = u_xlat20 / u_xlat11;
					    u_xlat11 = clamp(u_xlat11, 0.0, 1.0);
					    u_xlat11 = (-u_xlat11) + 1.0;
					    u_xlat20 = u_xlat2.x * 0.5 + u_xlat28;
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat2.x = u_xlat2.x * u_xlat20;
					    u_xlat3.xyz = u_xlat3.www * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat4.xyz * u_xlat5.www;
					    u_xlat4 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = u_xlat2.xxxx * u_xlat4 + u_xlat3;
					    u_xlat4 = vec4(u_xlat11) * u_xlat3;
					    u_xlat2.x = max(u_xlat4.w, 9.99999975e-05);
					    u_xlat3.xyz = u_xlat4.xyz / u_xlat2.xxx;
					    u_xlat4.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat4.z = 0.0;
					    u_xlat5 = (-u_xlat4.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat6 = texture(_MainTex, u_xlat5.xy);
					    u_xlat7 = u_xlat4.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat8 = texture(_MainTex, u_xlat7.xy);
					    u_xlat5 = texture(_MainTex, u_xlat5.zw);
					    u_xlat7 = texture(_MainTex, u_xlat7.zw);
					    u_xlat2.x = _ShaderFlags * 0.5;
					    u_xlatb29 = u_xlat2.x>=(-u_xlat2.x);
					    u_xlat2.x = fract(abs(u_xlat2.x));
					    u_xlat2.x = (u_xlatb29) ? u_xlat2.x : (-u_xlat2.x);
					    u_xlatb2 = u_xlat2.x>=0.5;
					    u_xlat29 = vs_TEXCOORD5.x + _BevelOffset;
					    u_xlat7.x = u_xlat6.w;
					    u_xlat7.y = u_xlat8.w;
					    u_xlat7.z = u_xlat5.w;
					    u_xlat5 = vec4(u_xlat29) + u_xlat7;
					    u_xlat29 = _BevelWidth + _OutlineWidth;
					    u_xlat29 = max(u_xlat29, 0.00999999978);
					    u_xlat5 = u_xlat5 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat5 = u_xlat5 / vec4(u_xlat29);
					    u_xlat5 = u_xlat5 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat5 = clamp(u_xlat5, 0.0, 1.0);
					    u_xlat6 = u_xlat5 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat6 = -abs(u_xlat6) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat5 = (bool(u_xlatb2)) ? u_xlat6 : u_xlat5;
					    u_xlat6 = u_xlat5 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat6 = sin(u_xlat6);
					    u_xlat6 = (-u_xlat5) + u_xlat6;
					    u_xlat5 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat6 + u_xlat5;
					    u_xlat2.x = (-_BevelClamp) + 1.0;
					    u_xlat5 = min(u_xlat2.xxxx, u_xlat5);
					    u_xlat2.x = u_xlat29 * _Bevel;
					    u_xlat2.x = u_xlat2.x * _GradientScale;
					    u_xlat2.x = u_xlat2.x * -2.0;
					    u_xlat4.xy = u_xlat2.xx * u_xlat5.xz;
					    u_xlat5.yz = u_xlat5.wy * u_xlat2.xx + (-u_xlat4.yx);
					    u_xlat5.x = float(-1.0);
					    u_xlat5.w = float(1.0);
					    u_xlat2.x = dot(u_xlat5.zw, u_xlat5.zw);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat4.yz = u_xlat2.xx * vec2(1.0, 0.0);
					    u_xlat4.x = u_xlat2.x * u_xlat5.z;
					    u_xlat2.x = dot(u_xlat5.xy, u_xlat5.xy);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat5.z = 0.0;
					    u_xlat5.xyz = u_xlat2.xxx * u_xlat5.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat5.xyz;
					    u_xlat4.xyz = u_xlat4.zxy * u_xlat5.yzx + (-u_xlat6.xyz);
					    u_xlat5 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat5.x = u_xlat5.w * u_xlat5.x;
					    u_xlat5.xy = u_xlat5.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat2.x = dot(u_xlat5.xy, u_xlat5.xy);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = (-u_xlat2.x) + 1.0;
					    u_xlat5.z = sqrt(u_xlat2.x);
					    u_xlat2.x = (-_BumpFace) + _BumpOutline;
					    u_xlat2.x = u_xlat20 * u_xlat2.x + _BumpFace;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat2.xxx + vec3(-0.0, -0.0, -1.0);
					    u_xlat5.xyz = u_xlat4.www * u_xlat5.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat4.xyz = u_xlat4.xyz + (-u_xlat5.xyz);
					    u_xlat2.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat4.xyz = u_xlat2.xxx * u_xlat4.xyz;
					    u_xlat5.xyz = u_xlat4.yyy * unity_ObjectToWorld[1].xyz;
					    u_xlat5.xyz = unity_ObjectToWorld[0].xyz * u_xlat4.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_ObjectToWorld[2].xyz * u_xlat4.zzz + u_xlat5.xyz;
					    u_xlat2.x = dot(vs_TEXCOORD6.xyz, u_xlat5.xyz);
					    u_xlat2.x = u_xlat2.x + u_xlat2.x;
					    u_xlat5.xyz = u_xlat5.xyz * (-u_xlat2.xxx) + vs_TEXCOORD6.xyz;
					    u_xlat5 = texture(_Cube, u_xlat5.xyz);
					    u_xlat6.xyz = (-_ReflectFaceColor.xyz) + _ReflectOutlineColor.xyz;
					    u_xlat6.xyz = vec3(u_xlat20) * u_xlat6.xyz + _ReflectFaceColor.xyz;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat6.xyz;
					    u_xlat2.x = _GlowOffset * _ScaleRatioB;
					    u_xlat2.x = u_xlat2.x * vs_TEXCOORD5.y;
					    u_xlat28 = (-u_xlat2.x) * 0.5 + u_xlat28;
					    u_xlatb2 = u_xlat28>=0.0;
					    u_xlat2.x = u_xlatb2 ? 1.0 : float(0.0);
					    u_xlat29 = _GlowOuter * _ScaleRatioB + (-_GlowInner);
					    u_xlat2.x = u_xlat2.x * u_xlat29 + _GlowInner;
					    u_xlat2.x = u_xlat2.x * vs_TEXCOORD5.y;
					    u_xlat29 = u_xlat2.x * 0.5;
					    u_xlat2.x = u_xlat2.x * 0.5 + 1.0;
					    u_xlat28 = u_xlat28 / u_xlat2.x;
					    u_xlat28 = min(abs(u_xlat28), 1.0);
					    u_xlat28 = log2(u_xlat28);
					    u_xlat28 = u_xlat28 * _GlowPower;
					    u_xlat28 = exp2(u_xlat28);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat2.x = min(u_xlat29, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat28 = u_xlat28 * u_xlat2.x;
					    u_xlat28 = dot(_GlowColor.ww, vec2(u_xlat28));
					    u_xlat28 = clamp(u_xlat28, 0.0, 1.0);
					    u_xlat2.x = u_xlat28 * vs_COLOR0.w;
					    u_xlat6.xyz = u_xlat2.xxx * _GlowColor.xyz;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat4.www + u_xlat6.xyz;
					    u_xlat3.xyz = u_xlat4.www * u_xlat3.xyz;
					    u_xlat28 = (-u_xlat28) * vs_COLOR0.w + 1.0;
					    u_xlat3.xyz = vec3(u_xlat28) * u_xlat3.xyz + u_xlat6.xyz;
					    u_xlat28 = (-u_xlat3.w) * u_xlat11 + 1.0;
					    u_xlat28 = u_xlat28 * u_xlat2.x + u_xlat4.w;
					    u_xlat2.x = max(u_xlat28, 9.99999975e-05);
					    u_xlat2.xyw = u_xlat3.xyz / u_xlat2.xxx;
					    u_xlat3.x = (-unused_0_33.y) + unused_0_33.z;
					    u_xlat20 = u_xlat20 * u_xlat3.x + unused_0_33.y;
					    u_xlatb3 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb3){
					        u_xlatb12 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD3.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD2.www + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.www + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat12.xyz = (bool(u_xlatb12)) ? u_xlat6.xyz : u_xlat9.xyz;
					        u_xlat12.xyz = u_xlat12.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat12.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat12.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat21 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat21, u_xlat12.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat12.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat12.x = clamp(u_xlat12.x, 0.0, 1.0);
					    u_xlat6.x = dot(vs_TEXCOORD2.xyz, (-u_xlat4.xyz));
					    u_xlat6.y = dot(vs_TEXCOORD3.xyz, (-u_xlat4.xyz));
					    u_xlat6.z = dot(vs_TEXCOORD4.xyz, (-u_xlat4.xyz));
					    u_xlat21 = dot(u_xlat6.xyz, u_xlat6.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat4.xyz = vec3(u_xlat21) * u_xlat6.xyz;
					    u_xlat12.xyz = u_xlat12.xxx * _LightColor0.xyz;
					    if(u_xlatb3){
					        u_xlatb3 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD3.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD2.www + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.www + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat9.xyz = (bool(u_xlatb3)) ? u_xlat6.xyz : u_xlat9.xyz;
					        u_xlat9.xyz = u_xlat9.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat9.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat9.x = u_xlat6.y * 0.25;
					        u_xlat18 = unity_ProbeVolumeParams.z * 0.5;
					        u_xlat27 = (-unity_ProbeVolumeParams.z) * 0.5 + 0.25;
					        u_xlat9.x = max(u_xlat18, u_xlat9.x);
					        u_xlat6.x = min(u_xlat27, u_xlat9.x);
					        u_xlat7 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					        u_xlat9.xyz = u_xlat6.xzw + vec3(0.25, 0.0, 0.0);
					        u_xlat8 = texture(unity_ProbeVolumeSH, u_xlat9.xyz);
					        u_xlat9.xyz = u_xlat6.xzw + vec3(0.5, 0.0, 0.0);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat9.xyz);
					        u_xlat4.w = 1.0;
					        u_xlat7.x = dot(u_xlat7, u_xlat4);
					        u_xlat7.y = dot(u_xlat8, u_xlat4);
					        u_xlat7.z = dot(u_xlat6, u_xlat4);
					    } else {
					        u_xlat4.w = 1.0;
					        u_xlat7.x = dot(unity_SHAr, u_xlat4);
					        u_xlat7.y = dot(unity_SHAg, u_xlat4);
					        u_xlat7.z = dot(unity_SHAb, u_xlat4);
					    }
					    u_xlat9.xyz = u_xlat7.xyz + vs_TEXCOORD7.xyz;
					    u_xlat9.xyz = max(u_xlat9.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat9.xyz = log2(u_xlat9.xyz);
					    u_xlat9.xyz = u_xlat9.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
					    u_xlat9.xyz = exp2(u_xlat9.xyz);
					    u_xlat9.xyz = u_xlat9.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    u_xlat9.xyz = max(u_xlat9.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat1.xyz = u_xlat1.xyz * u_xlat0.xxx + _WorldSpaceLightPos0.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat1.xyz = u_xlat0.xxx * u_xlat1.xyz;
					    u_xlat0.x = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat0.x = max(u_xlat0.x, 0.0);
					    u_xlat1.x = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat1.x = max(u_xlat1.x, 0.0);
					    u_xlat10 = u_xlat20 * 128.0;
					    u_xlat1.x = log2(u_xlat1.x);
					    u_xlat1.x = u_xlat1.x * u_xlat10;
					    u_xlat1.x = exp2(u_xlat1.x);
					    u_xlat4.xyz = u_xlat2.xyw * u_xlat12.xyz;
					    u_xlat3.xyz = u_xlat12.xyz * _SpecColor.xyz;
					    u_xlat1.xyz = u_xlat1.xxx * u_xlat3.xyz;
					    u_xlat1.xyz = u_xlat4.xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = u_xlat2.xyw * u_xlat9.xyz + u_xlat1.xyz;
					    SV_Target0.xyz = u_xlat5.xyz + u_xlat0.xyz;
					    SV_Target0.w = u_xlat28;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "LIGHTPROBE_SH" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 _ReflectFaceColor;
						vec4 _ReflectOutlineColor;
						vec4 unused_0_20[12];
						float _ShaderFlags;
						float _ScaleRatioA;
						vec4 unused_0_23[4];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_27;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_30[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[38];
						vec4 unity_SHAr;
						vec4 unity_SHAg;
						vec4 unity_SHAb;
						vec4 unused_2_5[4];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_7;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						vec4 unused_3_1[7];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  samplerCube _Cube;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD5;
					in  vec4 vs_TEXCOORD2;
					in  vec4 vs_TEXCOORD3;
					in  vec4 vs_TEXCOORD4;
					in  vec4 vs_COLOR0;
					in  vec3 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD7;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					bool u_xlatb4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec3 u_xlat9;
					vec3 u_xlat10;
					float u_xlat11;
					float u_xlat13;
					bool u_xlatb13;
					float u_xlat14;
					float u_xlat18;
					float u_xlat20;
					float u_xlat27;
					float u_xlat28;
					float u_xlat29;
					float u_xlat30;
					bool u_xlatb30;
					float u_xlat31;
					bool u_xlatb31;
					void main()
					{
					    u_xlat9.x = vs_TEXCOORD2.w;
					    u_xlat9.y = vs_TEXCOORD3.w;
					    u_xlat9.z = vs_TEXCOORD4.w;
					    u_xlat1.xyz = (-u_xlat9.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat28 = (-u_xlat2.w) + 0.5;
					    u_xlat28 = u_xlat28 + (-vs_TEXCOORD5.x);
					    u_xlat28 = u_xlat28 * vs_TEXCOORD5.y + 0.5;
					    u_xlat2.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat2.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat2.xz = u_xlat2.xy * vs_TEXCOORD5.yy;
					    u_xlat3 = vs_COLOR0 * _FaceColor;
					    u_xlat29 = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat4.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3 * u_xlat4;
					    u_xlat4.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat4.xy);
					    u_xlat4.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat5.w = u_xlat29 * u_xlat4.w;
					    u_xlat29 = (-u_xlat2.x) * 0.5 + u_xlat28;
					    u_xlat20 = u_xlat2.z * 0.5 + u_xlat29;
					    u_xlat11 = u_xlat2.y * vs_TEXCOORD5.y + 1.0;
					    u_xlat11 = u_xlat20 / u_xlat11;
					    u_xlat11 = clamp(u_xlat11, 0.0, 1.0);
					    u_xlat11 = (-u_xlat11) + 1.0;
					    u_xlat28 = u_xlat2.x * 0.5 + u_xlat28;
					    u_xlat28 = clamp(u_xlat28, 0.0, 1.0);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat2.x = u_xlat28 * u_xlat2.x;
					    u_xlat3.xyz = u_xlat3.www * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat4.xyz * u_xlat5.www;
					    u_xlat4 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = u_xlat2.xxxx * u_xlat4 + u_xlat3;
					    u_xlat2 = vec4(u_xlat11) * u_xlat3;
					    u_xlat3.x = max(u_xlat2.w, 9.99999975e-05);
					    u_xlat2.xyz = u_xlat2.xyz / u_xlat3.xxx;
					    u_xlat3.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat3.z = 0.0;
					    u_xlat4 = (-u_xlat3.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat5 = texture(_MainTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat6 = texture(_MainTex, u_xlat3.xy);
					    u_xlat4 = texture(_MainTex, u_xlat4.zw);
					    u_xlat3 = texture(_MainTex, u_xlat3.zw);
					    u_xlat4.x = _ShaderFlags * 0.5;
					    u_xlatb13 = u_xlat4.x>=(-u_xlat4.x);
					    u_xlat4.x = fract(abs(u_xlat4.x));
					    u_xlat4.x = (u_xlatb13) ? u_xlat4.x : (-u_xlat4.x);
					    u_xlatb4 = u_xlat4.x>=0.5;
					    u_xlat13 = vs_TEXCOORD5.x + _BevelOffset;
					    u_xlat3.x = u_xlat5.w;
					    u_xlat3.y = u_xlat6.w;
					    u_xlat3.z = u_xlat4.w;
					    u_xlat3 = vec4(u_xlat13) + u_xlat3;
					    u_xlat13 = _BevelWidth + _OutlineWidth;
					    u_xlat13 = max(u_xlat13, 0.00999999978);
					    u_xlat3 = u_xlat3 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat3 = u_xlat3 / vec4(u_xlat13);
					    u_xlat3 = u_xlat3 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat3 = clamp(u_xlat3, 0.0, 1.0);
					    u_xlat5 = u_xlat3 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat5 = -abs(u_xlat5) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat3 = (bool(u_xlatb4)) ? u_xlat5 : u_xlat3;
					    u_xlat5 = u_xlat3 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat5 = sin(u_xlat5);
					    u_xlat5 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat5 + u_xlat3;
					    u_xlat4.x = (-_BevelClamp) + 1.0;
					    u_xlat3 = min(u_xlat3, u_xlat4.xxxx);
					    u_xlat4.x = u_xlat13 * _Bevel;
					    u_xlat4.x = u_xlat4.x * _GradientScale;
					    u_xlat4.x = u_xlat4.x * -2.0;
					    u_xlat3.xz = u_xlat3.xz * u_xlat4.xx;
					    u_xlat3.yz = u_xlat3.wy * u_xlat4.xx + (-u_xlat3.zx);
					    u_xlat3.x = float(-1.0);
					    u_xlat3.w = float(1.0);
					    u_xlat30 = dot(u_xlat3.zw, u_xlat3.zw);
					    u_xlat30 = inversesqrt(u_xlat30);
					    u_xlat4.yz = vec2(u_xlat30) * vec2(1.0, 0.0);
					    u_xlat4.x = u_xlat30 * u_xlat3.z;
					    u_xlat30 = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat30 = inversesqrt(u_xlat30);
					    u_xlat3.z = 0.0;
					    u_xlat3.xyz = vec3(u_xlat30) * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat4.xyz;
					    u_xlat3.xyz = u_xlat4.zxy * u_xlat3.yzx + (-u_xlat5.xyz);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat30 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat30 = min(u_xlat30, 1.0);
					    u_xlat30 = (-u_xlat30) + 1.0;
					    u_xlat4.z = sqrt(u_xlat30);
					    u_xlat30 = (-_BumpFace) + _BumpOutline;
					    u_xlat30 = u_xlat28 * u_xlat30 + _BumpFace;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat30) + vec3(-0.0, -0.0, -1.0);
					    u_xlat4.xyz = u_xlat2.www * u_xlat4.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat3.xyz = u_xlat3.xyz + (-u_xlat4.xyz);
					    u_xlat30 = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat30 = inversesqrt(u_xlat30);
					    u_xlat3.xyz = vec3(u_xlat30) * u_xlat3.xyz;
					    u_xlat4.xyz = u_xlat3.yyy * unity_ObjectToWorld[1].xyz;
					    u_xlat4.xyz = unity_ObjectToWorld[0].xyz * u_xlat3.xxx + u_xlat4.xyz;
					    u_xlat4.xyz = unity_ObjectToWorld[2].xyz * u_xlat3.zzz + u_xlat4.xyz;
					    u_xlat30 = dot(vs_TEXCOORD6.xyz, u_xlat4.xyz);
					    u_xlat30 = u_xlat30 + u_xlat30;
					    u_xlat4.xyz = u_xlat4.xyz * (-vec3(u_xlat30)) + vs_TEXCOORD6.xyz;
					    u_xlat4 = texture(_Cube, u_xlat4.xyz);
					    u_xlat5.xyz = (-_ReflectFaceColor.xyz) + _ReflectOutlineColor.xyz;
					    u_xlat5.xyz = vec3(u_xlat28) * u_xlat5.xyz + _ReflectFaceColor.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * u_xlat5.xyz;
					    u_xlat30 = (-unused_0_27.y) + unused_0_27.z;
					    u_xlat28 = u_xlat28 * u_xlat30 + unused_0_27.y;
					    u_xlatb30 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb30){
					        u_xlatb31 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD3.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD2.www + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.www + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb31)) ? u_xlat5.xyz : u_xlat9.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat31 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat14 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat31, u_xlat14);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat31 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat31 = clamp(u_xlat31, 0.0, 1.0);
					    u_xlat5.x = dot(vs_TEXCOORD2.xyz, (-u_xlat3.xyz));
					    u_xlat5.y = dot(vs_TEXCOORD3.xyz, (-u_xlat3.xyz));
					    u_xlat5.z = dot(vs_TEXCOORD4.xyz, (-u_xlat3.xyz));
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat3.x = inversesqrt(u_xlat3.x);
					    u_xlat5.xyz = u_xlat3.xxx * u_xlat5.xyz;
					    u_xlat3.xyz = vec3(u_xlat31) * _LightColor0.xyz;
					    if(u_xlatb30){
					        u_xlatb30 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD3.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD2.www + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.www + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat9.xyz = (bool(u_xlatb30)) ? u_xlat6.xyz : u_xlat9.xyz;
					        u_xlat9.xyz = u_xlat9.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat9.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat9.x = u_xlat6.y * 0.25;
					        u_xlat18 = unity_ProbeVolumeParams.z * 0.5;
					        u_xlat27 = (-unity_ProbeVolumeParams.z) * 0.5 + 0.25;
					        u_xlat9.x = max(u_xlat18, u_xlat9.x);
					        u_xlat6.x = min(u_xlat27, u_xlat9.x);
					        u_xlat7 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					        u_xlat9.xyz = u_xlat6.xzw + vec3(0.25, 0.0, 0.0);
					        u_xlat8 = texture(unity_ProbeVolumeSH, u_xlat9.xyz);
					        u_xlat9.xyz = u_xlat6.xzw + vec3(0.5, 0.0, 0.0);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat9.xyz);
					        u_xlat5.w = 1.0;
					        u_xlat7.x = dot(u_xlat7, u_xlat5);
					        u_xlat7.y = dot(u_xlat8, u_xlat5);
					        u_xlat7.z = dot(u_xlat6, u_xlat5);
					    } else {
					        u_xlat5.w = 1.0;
					        u_xlat7.x = dot(unity_SHAr, u_xlat5);
					        u_xlat7.y = dot(unity_SHAg, u_xlat5);
					        u_xlat7.z = dot(unity_SHAb, u_xlat5);
					    }
					    u_xlat9.xyz = u_xlat7.xyz + vs_TEXCOORD7.xyz;
					    u_xlat9.xyz = max(u_xlat9.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat9.xyz = log2(u_xlat9.xyz);
					    u_xlat9.xyz = u_xlat9.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
					    u_xlat9.xyz = exp2(u_xlat9.xyz);
					    u_xlat9.xyz = u_xlat9.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    u_xlat9.xyz = max(u_xlat9.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat1.xyz = u_xlat1.xyz * u_xlat0.xxx + _WorldSpaceLightPos0.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat1.xyz = u_xlat0.xxx * u_xlat1.xyz;
					    u_xlat0.x = dot(u_xlat5.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat0.x = max(u_xlat0.x, 0.0);
					    u_xlat1.x = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat1.x = max(u_xlat1.x, 0.0);
					    u_xlat10.x = u_xlat28 * 128.0;
					    u_xlat1.x = log2(u_xlat1.x);
					    u_xlat1.x = u_xlat1.x * u_xlat10.x;
					    u_xlat1.x = exp2(u_xlat1.x);
					    u_xlat10.xyz = u_xlat2.xyz * u_xlat3.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * _SpecColor.xyz;
					    u_xlat3.xyz = u_xlat1.xxx * u_xlat3.xyz;
					    u_xlat1.xyz = u_xlat10.xyz * u_xlat0.xxx + u_xlat3.xyz;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat9.xyz + u_xlat1.xyz;
					    SV_Target0.xyz = u_xlat4.xyz * u_xlat2.www + u_xlat0.xyz;
					    SV_Target0.w = u_xlat2.w;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "GLOW_ON" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 _ReflectFaceColor;
						vec4 _ReflectOutlineColor;
						vec4 unused_0_20[10];
						vec4 _GlowColor;
						float _GlowOffset;
						float _GlowOuter;
						float _GlowInner;
						float _GlowPower;
						float _ShaderFlags;
						float _ScaleRatioA;
						float _ScaleRatioB;
						vec4 unused_0_29[3];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_33;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_36[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[38];
						vec4 unity_SHAr;
						vec4 unity_SHAg;
						vec4 unity_SHAb;
						vec4 unused_2_5[4];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_7;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						vec4 unused_3_1[7];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  samplerCube _Cube;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD5;
					in  vec4 vs_TEXCOORD2;
					in  vec4 vs_TEXCOORD3;
					in  vec4 vs_TEXCOORD4;
					in  vec4 vs_COLOR0;
					in  vec3 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD7;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					bool u_xlatb2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec3 u_xlat9;
					float u_xlat10;
					float u_xlat11;
					vec3 u_xlat12;
					bool u_xlatb12;
					float u_xlat18;
					float u_xlat20;
					float u_xlat21;
					float u_xlat27;
					float u_xlat28;
					float u_xlat29;
					bool u_xlatb29;
					void main()
					{
					    u_xlat9.x = vs_TEXCOORD2.w;
					    u_xlat9.y = vs_TEXCOORD3.w;
					    u_xlat9.z = vs_TEXCOORD4.w;
					    u_xlat1.xyz = (-u_xlat9.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat28 = (-u_xlat2.w) + 0.5;
					    u_xlat28 = u_xlat28 + (-vs_TEXCOORD5.x);
					    u_xlat28 = u_xlat28 * vs_TEXCOORD5.y + 0.5;
					    u_xlat2.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat2.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat2.xz = u_xlat2.xy * vs_TEXCOORD5.yy;
					    u_xlat3 = vs_COLOR0 * _FaceColor;
					    u_xlat29 = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat4.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3 * u_xlat4;
					    u_xlat4.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat4.xy);
					    u_xlat4.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat5.w = u_xlat29 * u_xlat4.w;
					    u_xlat29 = (-u_xlat2.x) * 0.5 + u_xlat28;
					    u_xlat20 = u_xlat2.z * 0.5 + u_xlat29;
					    u_xlat11 = u_xlat2.y * vs_TEXCOORD5.y + 1.0;
					    u_xlat11 = u_xlat20 / u_xlat11;
					    u_xlat11 = clamp(u_xlat11, 0.0, 1.0);
					    u_xlat11 = (-u_xlat11) + 1.0;
					    u_xlat20 = u_xlat2.x * 0.5 + u_xlat28;
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat2.x = u_xlat2.x * u_xlat20;
					    u_xlat3.xyz = u_xlat3.www * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat4.xyz * u_xlat5.www;
					    u_xlat4 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = u_xlat2.xxxx * u_xlat4 + u_xlat3;
					    u_xlat4 = vec4(u_xlat11) * u_xlat3;
					    u_xlat2.x = max(u_xlat4.w, 9.99999975e-05);
					    u_xlat3.xyz = u_xlat4.xyz / u_xlat2.xxx;
					    u_xlat4.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat4.z = 0.0;
					    u_xlat5 = (-u_xlat4.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat6 = texture(_MainTex, u_xlat5.xy);
					    u_xlat7 = u_xlat4.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat8 = texture(_MainTex, u_xlat7.xy);
					    u_xlat5 = texture(_MainTex, u_xlat5.zw);
					    u_xlat7 = texture(_MainTex, u_xlat7.zw);
					    u_xlat2.x = _ShaderFlags * 0.5;
					    u_xlatb29 = u_xlat2.x>=(-u_xlat2.x);
					    u_xlat2.x = fract(abs(u_xlat2.x));
					    u_xlat2.x = (u_xlatb29) ? u_xlat2.x : (-u_xlat2.x);
					    u_xlatb2 = u_xlat2.x>=0.5;
					    u_xlat29 = vs_TEXCOORD5.x + _BevelOffset;
					    u_xlat7.x = u_xlat6.w;
					    u_xlat7.y = u_xlat8.w;
					    u_xlat7.z = u_xlat5.w;
					    u_xlat5 = vec4(u_xlat29) + u_xlat7;
					    u_xlat29 = _BevelWidth + _OutlineWidth;
					    u_xlat29 = max(u_xlat29, 0.00999999978);
					    u_xlat5 = u_xlat5 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat5 = u_xlat5 / vec4(u_xlat29);
					    u_xlat5 = u_xlat5 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat5 = clamp(u_xlat5, 0.0, 1.0);
					    u_xlat6 = u_xlat5 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat6 = -abs(u_xlat6) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat5 = (bool(u_xlatb2)) ? u_xlat6 : u_xlat5;
					    u_xlat6 = u_xlat5 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat6 = sin(u_xlat6);
					    u_xlat6 = (-u_xlat5) + u_xlat6;
					    u_xlat5 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat6 + u_xlat5;
					    u_xlat2.x = (-_BevelClamp) + 1.0;
					    u_xlat5 = min(u_xlat2.xxxx, u_xlat5);
					    u_xlat2.x = u_xlat29 * _Bevel;
					    u_xlat2.x = u_xlat2.x * _GradientScale;
					    u_xlat2.x = u_xlat2.x * -2.0;
					    u_xlat4.xy = u_xlat2.xx * u_xlat5.xz;
					    u_xlat5.yz = u_xlat5.wy * u_xlat2.xx + (-u_xlat4.yx);
					    u_xlat5.x = float(-1.0);
					    u_xlat5.w = float(1.0);
					    u_xlat2.x = dot(u_xlat5.zw, u_xlat5.zw);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat4.yz = u_xlat2.xx * vec2(1.0, 0.0);
					    u_xlat4.x = u_xlat2.x * u_xlat5.z;
					    u_xlat2.x = dot(u_xlat5.xy, u_xlat5.xy);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat5.z = 0.0;
					    u_xlat5.xyz = u_xlat2.xxx * u_xlat5.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat5.xyz;
					    u_xlat4.xyz = u_xlat4.zxy * u_xlat5.yzx + (-u_xlat6.xyz);
					    u_xlat5 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat5.x = u_xlat5.w * u_xlat5.x;
					    u_xlat5.xy = u_xlat5.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat2.x = dot(u_xlat5.xy, u_xlat5.xy);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = (-u_xlat2.x) + 1.0;
					    u_xlat5.z = sqrt(u_xlat2.x);
					    u_xlat2.x = (-_BumpFace) + _BumpOutline;
					    u_xlat2.x = u_xlat20 * u_xlat2.x + _BumpFace;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat2.xxx + vec3(-0.0, -0.0, -1.0);
					    u_xlat5.xyz = u_xlat4.www * u_xlat5.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat4.xyz = u_xlat4.xyz + (-u_xlat5.xyz);
					    u_xlat2.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat4.xyz = u_xlat2.xxx * u_xlat4.xyz;
					    u_xlat5.xyz = u_xlat4.yyy * unity_ObjectToWorld[1].xyz;
					    u_xlat5.xyz = unity_ObjectToWorld[0].xyz * u_xlat4.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_ObjectToWorld[2].xyz * u_xlat4.zzz + u_xlat5.xyz;
					    u_xlat2.x = dot(vs_TEXCOORD6.xyz, u_xlat5.xyz);
					    u_xlat2.x = u_xlat2.x + u_xlat2.x;
					    u_xlat5.xyz = u_xlat5.xyz * (-u_xlat2.xxx) + vs_TEXCOORD6.xyz;
					    u_xlat5 = texture(_Cube, u_xlat5.xyz);
					    u_xlat6.xyz = (-_ReflectFaceColor.xyz) + _ReflectOutlineColor.xyz;
					    u_xlat6.xyz = vec3(u_xlat20) * u_xlat6.xyz + _ReflectFaceColor.xyz;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat6.xyz;
					    u_xlat2.x = _GlowOffset * _ScaleRatioB;
					    u_xlat2.x = u_xlat2.x * vs_TEXCOORD5.y;
					    u_xlat28 = (-u_xlat2.x) * 0.5 + u_xlat28;
					    u_xlatb2 = u_xlat28>=0.0;
					    u_xlat2.x = u_xlatb2 ? 1.0 : float(0.0);
					    u_xlat29 = _GlowOuter * _ScaleRatioB + (-_GlowInner);
					    u_xlat2.x = u_xlat2.x * u_xlat29 + _GlowInner;
					    u_xlat2.x = u_xlat2.x * vs_TEXCOORD5.y;
					    u_xlat29 = u_xlat2.x * 0.5;
					    u_xlat2.x = u_xlat2.x * 0.5 + 1.0;
					    u_xlat28 = u_xlat28 / u_xlat2.x;
					    u_xlat28 = min(abs(u_xlat28), 1.0);
					    u_xlat28 = log2(u_xlat28);
					    u_xlat28 = u_xlat28 * _GlowPower;
					    u_xlat28 = exp2(u_xlat28);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat2.x = min(u_xlat29, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat28 = u_xlat28 * u_xlat2.x;
					    u_xlat28 = dot(_GlowColor.ww, vec2(u_xlat28));
					    u_xlat28 = clamp(u_xlat28, 0.0, 1.0);
					    u_xlat2.x = u_xlat28 * vs_COLOR0.w;
					    u_xlat6.xyz = u_xlat2.xxx * _GlowColor.xyz;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat4.www + u_xlat6.xyz;
					    u_xlat3.xyz = u_xlat4.www * u_xlat3.xyz;
					    u_xlat28 = (-u_xlat28) * vs_COLOR0.w + 1.0;
					    u_xlat3.xyz = vec3(u_xlat28) * u_xlat3.xyz + u_xlat6.xyz;
					    u_xlat28 = (-u_xlat3.w) * u_xlat11 + 1.0;
					    u_xlat28 = u_xlat28 * u_xlat2.x + u_xlat4.w;
					    u_xlat2.x = max(u_xlat28, 9.99999975e-05);
					    u_xlat2.xyw = u_xlat3.xyz / u_xlat2.xxx;
					    u_xlat3.x = (-unused_0_33.y) + unused_0_33.z;
					    u_xlat20 = u_xlat20 * u_xlat3.x + unused_0_33.y;
					    u_xlatb3 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb3){
					        u_xlatb12 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD3.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD2.www + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.www + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat12.xyz = (bool(u_xlatb12)) ? u_xlat6.xyz : u_xlat9.xyz;
					        u_xlat12.xyz = u_xlat12.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat12.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat12.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat21 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat21, u_xlat12.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat12.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat12.x = clamp(u_xlat12.x, 0.0, 1.0);
					    u_xlat6.x = dot(vs_TEXCOORD2.xyz, (-u_xlat4.xyz));
					    u_xlat6.y = dot(vs_TEXCOORD3.xyz, (-u_xlat4.xyz));
					    u_xlat6.z = dot(vs_TEXCOORD4.xyz, (-u_xlat4.xyz));
					    u_xlat21 = dot(u_xlat6.xyz, u_xlat6.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat4.xyz = vec3(u_xlat21) * u_xlat6.xyz;
					    u_xlat12.xyz = u_xlat12.xxx * _LightColor0.xyz;
					    if(u_xlatb3){
					        u_xlatb3 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD3.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD2.www + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.www + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat9.xyz = (bool(u_xlatb3)) ? u_xlat6.xyz : u_xlat9.xyz;
					        u_xlat9.xyz = u_xlat9.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat9.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat9.x = u_xlat6.y * 0.25;
					        u_xlat18 = unity_ProbeVolumeParams.z * 0.5;
					        u_xlat27 = (-unity_ProbeVolumeParams.z) * 0.5 + 0.25;
					        u_xlat9.x = max(u_xlat18, u_xlat9.x);
					        u_xlat6.x = min(u_xlat27, u_xlat9.x);
					        u_xlat7 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					        u_xlat9.xyz = u_xlat6.xzw + vec3(0.25, 0.0, 0.0);
					        u_xlat8 = texture(unity_ProbeVolumeSH, u_xlat9.xyz);
					        u_xlat9.xyz = u_xlat6.xzw + vec3(0.5, 0.0, 0.0);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat9.xyz);
					        u_xlat4.w = 1.0;
					        u_xlat7.x = dot(u_xlat7, u_xlat4);
					        u_xlat7.y = dot(u_xlat8, u_xlat4);
					        u_xlat7.z = dot(u_xlat6, u_xlat4);
					    } else {
					        u_xlat4.w = 1.0;
					        u_xlat7.x = dot(unity_SHAr, u_xlat4);
					        u_xlat7.y = dot(unity_SHAg, u_xlat4);
					        u_xlat7.z = dot(unity_SHAb, u_xlat4);
					    }
					    u_xlat9.xyz = u_xlat7.xyz + vs_TEXCOORD7.xyz;
					    u_xlat9.xyz = max(u_xlat9.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat9.xyz = log2(u_xlat9.xyz);
					    u_xlat9.xyz = u_xlat9.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
					    u_xlat9.xyz = exp2(u_xlat9.xyz);
					    u_xlat9.xyz = u_xlat9.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    u_xlat9.xyz = max(u_xlat9.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat1.xyz = u_xlat1.xyz * u_xlat0.xxx + _WorldSpaceLightPos0.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat1.xyz = u_xlat0.xxx * u_xlat1.xyz;
					    u_xlat0.x = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat0.x = max(u_xlat0.x, 0.0);
					    u_xlat1.x = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat1.x = max(u_xlat1.x, 0.0);
					    u_xlat10 = u_xlat20 * 128.0;
					    u_xlat1.x = log2(u_xlat1.x);
					    u_xlat1.x = u_xlat1.x * u_xlat10;
					    u_xlat1.x = exp2(u_xlat1.x);
					    u_xlat4.xyz = u_xlat2.xyw * u_xlat12.xyz;
					    u_xlat3.xyz = u_xlat12.xyz * _SpecColor.xyz;
					    u_xlat1.xyz = u_xlat1.xxx * u_xlat3.xyz;
					    u_xlat1.xyz = u_xlat4.xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = u_xlat2.xyw * u_xlat9.xyz + u_xlat1.xyz;
					    SV_Target0.xyz = u_xlat5.xyz + u_xlat0.xyz;
					    SV_Target0.w = u_xlat28;
					    return;
					}"
				}
			}
		}
		Pass {
			Name "FORWARD"
			LOD 300
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDADD" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha One, SrcAlpha One
			ColorMask RGB -1
			ZWrite Off
			Cull Off
			GpuProgramID 88795
			Program "vp" {
				SubProgram "d3d11 " {
					Keywords { "POINT" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						vec4 unused_0_2[2];
						float _FaceDilate;
						vec4 unused_0_4[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_6[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_12[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_3_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD7;
					out vec3 vs_TEXCOORD8;
					vec3 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					float u_xlat5;
					int u_xlati5;
					float u_xlat7;
					float u_xlat12;
					float u_xlat15;
					bool u_xlatb15;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.x = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat7 = _Sharpness + 1.0;
					    u_xlat2.x = u_xlat7 * u_xlat2.x;
					    u_xlat7 = u_xlat15 * u_xlat2.x;
					    u_xlat12 = (-_PerspectiveFilter) + 1.0;
					    u_xlat7 = u_xlat12 * u_xlat7;
					    u_xlat15 = u_xlat15 * u_xlat2.x + (-u_xlat7);
					    u_xlat2.xzw = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat2.xzw = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat2.xzw;
					    u_xlat2.xzw = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat2.xzw;
					    u_xlat2.xzw = u_xlat2.xzw + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat2.xzw;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat2.xzw = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat1 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat1;
					    u_xlat3.xyz = (-u_xlat2.xzw) + _WorldSpaceCameraPos.xyz;
					    vs_TEXCOORD5.xyz = u_xlat2.xzw;
					    u_xlat2.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat2.xzw = u_xlat2.xxx * u_xlat3.xyz;
					    u_xlat2.x = dot(u_xlat0.yzx, u_xlat2.xzw);
					    vs_TEXCOORD6.y = abs(u_xlat2.x) * u_xlat15 + u_xlat7;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat2.x = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat2.x + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD6.x = u_xlat15 * 0.5;
					    vs_TEXCOORD2.z = u_xlat0.y;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.xyz * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.zxy * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat5 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat5) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD3.z = u_xlat0.z;
					    vs_TEXCOORD4.z = u_xlat0.x;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat0.xyz = u_xlat3.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyz = _EnvMatrix[0].xyz * u_xlat3.xxx + u_xlat0.xyz;
					    vs_TEXCOORD7.xyz = _EnvMatrix[2].xyz * u_xlat3.zzz + u_xlat0.xyz;
					    u_xlat0.xyz = u_xlat1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat0.xyz = unity_WorldToLight[0].xyz * u_xlat1.xxx + u_xlat0.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat1.zzz + u_xlat0.xyz;
					    vs_TEXCOORD8.xyz = unity_WorldToLight[3].xyz * u_xlat1.www + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "GLOW_ON" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[6];
						float _FaceDilate;
						vec4 unused_0_2[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_4[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_10[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_3_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD7;
					vec3 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					float u_xlat4;
					int u_xlati4;
					vec3 u_xlat6;
					float u_xlat12;
					bool u_xlatb12;
					float u_xlat13;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat12 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat12);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat12 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat12 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat12;
					    u_xlat12 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat12;
					    u_xlat12 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat12;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat12) / u_xlat2.xy;
					    u_xlat12 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat12 = inversesqrt(u_xlat12);
					    u_xlat13 = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat2.x = _Sharpness + 1.0;
					    u_xlat13 = u_xlat13 * u_xlat2.x;
					    u_xlat2.x = u_xlat12 * u_xlat13;
					    u_xlat6.x = (-_PerspectiveFilter) + 1.0;
					    u_xlat2.x = u_xlat6.x * u_xlat2.x;
					    u_xlat12 = u_xlat12 * u_xlat13 + (-u_xlat2.x);
					    u_xlat6.xyz = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat6.xyz = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat6.xyz;
					    u_xlat6.xyz = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat6.xyz;
					    u_xlat6.xyz = u_xlat6.xyz + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat6.xyz;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati4 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati4) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat6.xyz = (-u_xlat1.xyz) + _WorldSpaceCameraPos.xyz;
					    vs_TEXCOORD5.xyz = u_xlat1.xyz;
					    u_xlat1.x = dot(u_xlat6.xyz, u_xlat6.xyz);
					    u_xlat1.x = inversesqrt(u_xlat1.x);
					    u_xlat1.xyz = u_xlat1.xxx * u_xlat6.xyz;
					    u_xlat1.x = dot(u_xlat0.yzx, u_xlat1.xyz);
					    vs_TEXCOORD6.y = abs(u_xlat1.x) * u_xlat12 + u_xlat2.x;
					    u_xlatb12 = 0.0>=in_TEXCOORD1.y;
					    u_xlat12 = u_xlatb12 ? 1.0 : float(0.0);
					    u_xlat1.x = (-_WeightNormal) + _WeightBold;
					    u_xlat12 = u_xlat12 * u_xlat1.x + _WeightNormal;
					    u_xlat12 = u_xlat12 * 0.25 + _FaceDilate;
					    u_xlat12 = u_xlat12 * _ScaleRatioA;
					    vs_TEXCOORD6.x = u_xlat12 * 0.5;
					    vs_TEXCOORD2.z = u_xlat0.y;
					    u_xlat1.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat1.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat1.xyz;
					    u_xlat1.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat1.xyz;
					    u_xlat12 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat12 = inversesqrt(u_xlat12);
					    u_xlat1.xyz = vec3(u_xlat12) * u_xlat1.xyz;
					    u_xlat3.xyz = u_xlat0.xyz * u_xlat1.xyz;
					    u_xlat3.xyz = u_xlat0.zxy * u_xlat1.yzx + (-u_xlat3.xyz);
					    u_xlat4 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat3.xyz = vec3(u_xlat4) * u_xlat3.xyz;
					    vs_TEXCOORD2.y = u_xlat3.x;
					    vs_TEXCOORD2.x = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat0.z;
					    vs_TEXCOORD4.z = u_xlat0.x;
					    vs_TEXCOORD3.x = u_xlat1.x;
					    vs_TEXCOORD4.x = u_xlat1.y;
					    vs_TEXCOORD3.y = u_xlat3.y;
					    vs_TEXCOORD4.y = u_xlat3.z;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat0.xyz = u_xlat6.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyz = _EnvMatrix[0].xyz * u_xlat6.xxx + u_xlat0.xyz;
					    vs_TEXCOORD7.xyz = _EnvMatrix[2].xyz * u_xlat6.zzz + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[6];
						float _FaceDilate;
						vec4 unused_0_2[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_4[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_10[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_3_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD7;
					vec3 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					float u_xlat4;
					int u_xlati4;
					vec3 u_xlat6;
					float u_xlat12;
					bool u_xlatb12;
					float u_xlat13;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat12 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat12);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat12 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat12 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat12;
					    u_xlat12 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat12;
					    u_xlat12 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat12;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat12) / u_xlat2.xy;
					    u_xlat12 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat12 = inversesqrt(u_xlat12);
					    u_xlat13 = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat2.x = _Sharpness + 1.0;
					    u_xlat13 = u_xlat13 * u_xlat2.x;
					    u_xlat2.x = u_xlat12 * u_xlat13;
					    u_xlat6.x = (-_PerspectiveFilter) + 1.0;
					    u_xlat2.x = u_xlat6.x * u_xlat2.x;
					    u_xlat12 = u_xlat12 * u_xlat13 + (-u_xlat2.x);
					    u_xlat6.xyz = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat6.xyz = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat6.xyz;
					    u_xlat6.xyz = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat6.xyz;
					    u_xlat6.xyz = u_xlat6.xyz + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat6.xyz;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati4 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati4) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat6.xyz = (-u_xlat1.xyz) + _WorldSpaceCameraPos.xyz;
					    vs_TEXCOORD5.xyz = u_xlat1.xyz;
					    u_xlat1.x = dot(u_xlat6.xyz, u_xlat6.xyz);
					    u_xlat1.x = inversesqrt(u_xlat1.x);
					    u_xlat1.xyz = u_xlat1.xxx * u_xlat6.xyz;
					    u_xlat1.x = dot(u_xlat0.yzx, u_xlat1.xyz);
					    vs_TEXCOORD6.y = abs(u_xlat1.x) * u_xlat12 + u_xlat2.x;
					    u_xlatb12 = 0.0>=in_TEXCOORD1.y;
					    u_xlat12 = u_xlatb12 ? 1.0 : float(0.0);
					    u_xlat1.x = (-_WeightNormal) + _WeightBold;
					    u_xlat12 = u_xlat12 * u_xlat1.x + _WeightNormal;
					    u_xlat12 = u_xlat12 * 0.25 + _FaceDilate;
					    u_xlat12 = u_xlat12 * _ScaleRatioA;
					    vs_TEXCOORD6.x = u_xlat12 * 0.5;
					    vs_TEXCOORD2.z = u_xlat0.y;
					    u_xlat1.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat1.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat1.xyz;
					    u_xlat1.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat1.xyz;
					    u_xlat12 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat12 = inversesqrt(u_xlat12);
					    u_xlat1.xyz = vec3(u_xlat12) * u_xlat1.xyz;
					    u_xlat3.xyz = u_xlat0.xyz * u_xlat1.xyz;
					    u_xlat3.xyz = u_xlat0.zxy * u_xlat1.yzx + (-u_xlat3.xyz);
					    u_xlat4 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat3.xyz = vec3(u_xlat4) * u_xlat3.xyz;
					    vs_TEXCOORD2.y = u_xlat3.x;
					    vs_TEXCOORD2.x = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat0.z;
					    vs_TEXCOORD4.z = u_xlat0.x;
					    vs_TEXCOORD3.x = u_xlat1.x;
					    vs_TEXCOORD4.x = u_xlat1.y;
					    vs_TEXCOORD3.y = u_xlat3.y;
					    vs_TEXCOORD4.y = u_xlat3.z;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat0.xyz = u_xlat6.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyz = _EnvMatrix[0].xyz * u_xlat6.xxx + u_xlat0.xyz;
					    vs_TEXCOORD7.xyz = _EnvMatrix[2].xyz * u_xlat6.zzz + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "GLOW_ON" "SPOT" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						vec4 unused_0_2[2];
						float _FaceDilate;
						vec4 unused_0_4[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_6[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_12[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_3_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD7;
					out vec4 vs_TEXCOORD8;
					vec4 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					float u_xlat5;
					int u_xlati5;
					float u_xlat7;
					float u_xlat12;
					float u_xlat15;
					bool u_xlatb15;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.x = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat7 = _Sharpness + 1.0;
					    u_xlat2.x = u_xlat7 * u_xlat2.x;
					    u_xlat7 = u_xlat15 * u_xlat2.x;
					    u_xlat12 = (-_PerspectiveFilter) + 1.0;
					    u_xlat7 = u_xlat12 * u_xlat7;
					    u_xlat15 = u_xlat15 * u_xlat2.x + (-u_xlat7);
					    u_xlat2.xzw = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat2.xzw = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat2.xzw;
					    u_xlat2.xzw = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat2.xzw;
					    u_xlat2.xzw = u_xlat2.xzw + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat2.xzw;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat2.xzw = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat1 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat1;
					    u_xlat3.xyz = (-u_xlat2.xzw) + _WorldSpaceCameraPos.xyz;
					    vs_TEXCOORD5.xyz = u_xlat2.xzw;
					    u_xlat2.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat2.xzw = u_xlat2.xxx * u_xlat3.xyz;
					    u_xlat2.x = dot(u_xlat0.yzx, u_xlat2.xzw);
					    vs_TEXCOORD6.y = abs(u_xlat2.x) * u_xlat15 + u_xlat7;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat2.x = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat2.x + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD6.x = u_xlat15 * 0.5;
					    vs_TEXCOORD2.z = u_xlat0.y;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.xyz * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.zxy * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat5 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat5) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD3.z = u_xlat0.z;
					    vs_TEXCOORD4.z = u_xlat0.x;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat0.xyz = u_xlat3.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyz = _EnvMatrix[0].xyz * u_xlat3.xxx + u_xlat0.xyz;
					    vs_TEXCOORD7.xyz = _EnvMatrix[2].xyz * u_xlat3.zzz + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_WorldToLight[1];
					    u_xlat0 = unity_WorldToLight[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_WorldToLight[2] * u_xlat1.zzzz + u_xlat0;
					    vs_TEXCOORD8 = unity_WorldToLight[3] * u_xlat1.wwww + u_xlat0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "SPOT" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						vec4 unused_0_2[2];
						float _FaceDilate;
						vec4 unused_0_4[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_6[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_12[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_3_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD7;
					out vec4 vs_TEXCOORD8;
					vec4 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					float u_xlat5;
					int u_xlati5;
					float u_xlat7;
					float u_xlat12;
					float u_xlat15;
					bool u_xlatb15;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.x = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat7 = _Sharpness + 1.0;
					    u_xlat2.x = u_xlat7 * u_xlat2.x;
					    u_xlat7 = u_xlat15 * u_xlat2.x;
					    u_xlat12 = (-_PerspectiveFilter) + 1.0;
					    u_xlat7 = u_xlat12 * u_xlat7;
					    u_xlat15 = u_xlat15 * u_xlat2.x + (-u_xlat7);
					    u_xlat2.xzw = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat2.xzw = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat2.xzw;
					    u_xlat2.xzw = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat2.xzw;
					    u_xlat2.xzw = u_xlat2.xzw + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat2.xzw;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat2.xzw = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat1 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat1;
					    u_xlat3.xyz = (-u_xlat2.xzw) + _WorldSpaceCameraPos.xyz;
					    vs_TEXCOORD5.xyz = u_xlat2.xzw;
					    u_xlat2.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat2.xzw = u_xlat2.xxx * u_xlat3.xyz;
					    u_xlat2.x = dot(u_xlat0.yzx, u_xlat2.xzw);
					    vs_TEXCOORD6.y = abs(u_xlat2.x) * u_xlat15 + u_xlat7;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat2.x = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat2.x + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD6.x = u_xlat15 * 0.5;
					    vs_TEXCOORD2.z = u_xlat0.y;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.xyz * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.zxy * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat5 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat5) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD3.z = u_xlat0.z;
					    vs_TEXCOORD4.z = u_xlat0.x;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat0.xyz = u_xlat3.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyz = _EnvMatrix[0].xyz * u_xlat3.xxx + u_xlat0.xyz;
					    vs_TEXCOORD7.xyz = _EnvMatrix[2].xyz * u_xlat3.zzz + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_WorldToLight[1];
					    u_xlat0 = unity_WorldToLight[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_WorldToLight[2] * u_xlat1.zzzz + u_xlat0;
					    vs_TEXCOORD8 = unity_WorldToLight[3] * u_xlat1.wwww + u_xlat0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "GLOW_ON" "POINT_COOKIE" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						vec4 unused_0_2[2];
						float _FaceDilate;
						vec4 unused_0_4[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_6[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_12[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_3_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD7;
					out vec3 vs_TEXCOORD8;
					vec3 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					float u_xlat5;
					int u_xlati5;
					float u_xlat7;
					float u_xlat12;
					float u_xlat15;
					bool u_xlatb15;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.x = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat7 = _Sharpness + 1.0;
					    u_xlat2.x = u_xlat7 * u_xlat2.x;
					    u_xlat7 = u_xlat15 * u_xlat2.x;
					    u_xlat12 = (-_PerspectiveFilter) + 1.0;
					    u_xlat7 = u_xlat12 * u_xlat7;
					    u_xlat15 = u_xlat15 * u_xlat2.x + (-u_xlat7);
					    u_xlat2.xzw = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat2.xzw = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat2.xzw;
					    u_xlat2.xzw = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat2.xzw;
					    u_xlat2.xzw = u_xlat2.xzw + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat2.xzw;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat2.xzw = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat1 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat1;
					    u_xlat3.xyz = (-u_xlat2.xzw) + _WorldSpaceCameraPos.xyz;
					    vs_TEXCOORD5.xyz = u_xlat2.xzw;
					    u_xlat2.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat2.xzw = u_xlat2.xxx * u_xlat3.xyz;
					    u_xlat2.x = dot(u_xlat0.yzx, u_xlat2.xzw);
					    vs_TEXCOORD6.y = abs(u_xlat2.x) * u_xlat15 + u_xlat7;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat2.x = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat2.x + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD6.x = u_xlat15 * 0.5;
					    vs_TEXCOORD2.z = u_xlat0.y;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.xyz * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.zxy * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat5 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat5) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD3.z = u_xlat0.z;
					    vs_TEXCOORD4.z = u_xlat0.x;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat0.xyz = u_xlat3.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyz = _EnvMatrix[0].xyz * u_xlat3.xxx + u_xlat0.xyz;
					    vs_TEXCOORD7.xyz = _EnvMatrix[2].xyz * u_xlat3.zzz + u_xlat0.xyz;
					    u_xlat0.xyz = u_xlat1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat0.xyz = unity_WorldToLight[0].xyz * u_xlat1.xxx + u_xlat0.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat1.zzz + u_xlat0.xyz;
					    vs_TEXCOORD8.xyz = unity_WorldToLight[3].xyz * u_xlat1.www + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT_COOKIE" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						vec4 unused_0_2[2];
						float _FaceDilate;
						vec4 unused_0_4[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_6[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_12[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_3_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD7;
					out vec3 vs_TEXCOORD8;
					vec3 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					float u_xlat5;
					int u_xlati5;
					float u_xlat7;
					float u_xlat12;
					float u_xlat15;
					bool u_xlatb15;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.x = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat7 = _Sharpness + 1.0;
					    u_xlat2.x = u_xlat7 * u_xlat2.x;
					    u_xlat7 = u_xlat15 * u_xlat2.x;
					    u_xlat12 = (-_PerspectiveFilter) + 1.0;
					    u_xlat7 = u_xlat12 * u_xlat7;
					    u_xlat15 = u_xlat15 * u_xlat2.x + (-u_xlat7);
					    u_xlat2.xzw = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat2.xzw = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat2.xzw;
					    u_xlat2.xzw = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat2.xzw;
					    u_xlat2.xzw = u_xlat2.xzw + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat2.xzw;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat2.xzw = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat1 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat1;
					    u_xlat3.xyz = (-u_xlat2.xzw) + _WorldSpaceCameraPos.xyz;
					    vs_TEXCOORD5.xyz = u_xlat2.xzw;
					    u_xlat2.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat2.xzw = u_xlat2.xxx * u_xlat3.xyz;
					    u_xlat2.x = dot(u_xlat0.yzx, u_xlat2.xzw);
					    vs_TEXCOORD6.y = abs(u_xlat2.x) * u_xlat15 + u_xlat7;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat2.x = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat2.x + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD6.x = u_xlat15 * 0.5;
					    vs_TEXCOORD2.z = u_xlat0.y;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.xyz * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.zxy * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat5 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat5) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD3.z = u_xlat0.z;
					    vs_TEXCOORD4.z = u_xlat0.x;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat0.xyz = u_xlat3.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyz = _EnvMatrix[0].xyz * u_xlat3.xxx + u_xlat0.xyz;
					    vs_TEXCOORD7.xyz = _EnvMatrix[2].xyz * u_xlat3.zzz + u_xlat0.xyz;
					    u_xlat0.xyz = u_xlat1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat0.xyz = unity_WorldToLight[0].xyz * u_xlat1.xxx + u_xlat0.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat1.zzz + u_xlat0.xyz;
					    vs_TEXCOORD8.xyz = unity_WorldToLight[3].xyz * u_xlat1.www + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" "GLOW_ON" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						vec4 unused_0_2[2];
						float _FaceDilate;
						vec4 unused_0_4[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_6[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_12[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_3_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD7;
					out vec2 vs_TEXCOORD8;
					vec3 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					float u_xlat5;
					int u_xlati5;
					float u_xlat7;
					float u_xlat12;
					float u_xlat15;
					bool u_xlatb15;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.x = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat7 = _Sharpness + 1.0;
					    u_xlat2.x = u_xlat7 * u_xlat2.x;
					    u_xlat7 = u_xlat15 * u_xlat2.x;
					    u_xlat12 = (-_PerspectiveFilter) + 1.0;
					    u_xlat7 = u_xlat12 * u_xlat7;
					    u_xlat15 = u_xlat15 * u_xlat2.x + (-u_xlat7);
					    u_xlat2.xzw = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat2.xzw = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat2.xzw;
					    u_xlat2.xzw = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat2.xzw;
					    u_xlat2.xzw = u_xlat2.xzw + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat2.xzw;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat2.xzw = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat1 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat1;
					    u_xlat3.xyz = (-u_xlat2.xzw) + _WorldSpaceCameraPos.xyz;
					    vs_TEXCOORD5.xyz = u_xlat2.xzw;
					    u_xlat2.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat2.xzw = u_xlat2.xxx * u_xlat3.xyz;
					    u_xlat2.x = dot(u_xlat0.yzx, u_xlat2.xzw);
					    vs_TEXCOORD6.y = abs(u_xlat2.x) * u_xlat15 + u_xlat7;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat2.x = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat2.x + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD6.x = u_xlat15 * 0.5;
					    vs_TEXCOORD2.z = u_xlat0.y;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.xyz * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.zxy * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat5 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat5) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD3.z = u_xlat0.z;
					    vs_TEXCOORD4.z = u_xlat0.x;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat0.xyz = u_xlat3.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyz = _EnvMatrix[0].xyz * u_xlat3.xxx + u_xlat0.xyz;
					    vs_TEXCOORD7.xyz = _EnvMatrix[2].xyz * u_xlat3.zzz + u_xlat0.xyz;
					    u_xlat0.xy = u_xlat1.yy * unity_WorldToLight[1].xy;
					    u_xlat0.xy = unity_WorldToLight[0].xy * u_xlat1.xx + u_xlat0.xy;
					    u_xlat0.xy = unity_WorldToLight[2].xy * u_xlat1.zz + u_xlat0.xy;
					    vs_TEXCOORD8.xy = unity_WorldToLight[3].xy * u_xlat1.ww + u_xlat0.xy;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						vec4 unused_0_2[2];
						float _FaceDilate;
						vec4 unused_0_4[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_6[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_12[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_3_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD7;
					out vec2 vs_TEXCOORD8;
					vec3 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					float u_xlat5;
					int u_xlati5;
					float u_xlat7;
					float u_xlat12;
					float u_xlat15;
					bool u_xlatb15;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.x = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat7 = _Sharpness + 1.0;
					    u_xlat2.x = u_xlat7 * u_xlat2.x;
					    u_xlat7 = u_xlat15 * u_xlat2.x;
					    u_xlat12 = (-_PerspectiveFilter) + 1.0;
					    u_xlat7 = u_xlat12 * u_xlat7;
					    u_xlat15 = u_xlat15 * u_xlat2.x + (-u_xlat7);
					    u_xlat2.xzw = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat2.xzw = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat2.xzw;
					    u_xlat2.xzw = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat2.xzw;
					    u_xlat2.xzw = u_xlat2.xzw + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat2.xzw;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat2.xzw = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat1 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat1;
					    u_xlat3.xyz = (-u_xlat2.xzw) + _WorldSpaceCameraPos.xyz;
					    vs_TEXCOORD5.xyz = u_xlat2.xzw;
					    u_xlat2.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat2.xzw = u_xlat2.xxx * u_xlat3.xyz;
					    u_xlat2.x = dot(u_xlat0.yzx, u_xlat2.xzw);
					    vs_TEXCOORD6.y = abs(u_xlat2.x) * u_xlat15 + u_xlat7;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat2.x = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat2.x + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD6.x = u_xlat15 * 0.5;
					    vs_TEXCOORD2.z = u_xlat0.y;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.xyz * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.zxy * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat5 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat5) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD3.z = u_xlat0.z;
					    vs_TEXCOORD4.z = u_xlat0.x;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat0.xyz = u_xlat3.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyz = _EnvMatrix[0].xyz * u_xlat3.xxx + u_xlat0.xyz;
					    vs_TEXCOORD7.xyz = _EnvMatrix[2].xyz * u_xlat3.zzz + u_xlat0.xyz;
					    u_xlat0.xy = u_xlat1.yy * unity_WorldToLight[1].xy;
					    u_xlat0.xy = unity_WorldToLight[0].xy * u_xlat1.xx + u_xlat0.xy;
					    u_xlat0.xy = unity_WorldToLight[2].xy * u_xlat1.zz + u_xlat0.xy;
					    vs_TEXCOORD8.xy = unity_WorldToLight[3].xy * u_xlat1.ww + u_xlat0.xy;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "GLOW_ON" "POINT" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						vec4 unused_0_2[2];
						float _FaceDilate;
						vec4 unused_0_4[6];
						mat4x4 _EnvMatrix;
						vec4 unused_0_6[7];
						float _WeightNormal;
						float _WeightBold;
						float _ScaleRatioA;
						float _VertexOffsetX;
						float _VertexOffsetY;
						vec4 unused_0_12[4];
						float _GradientScale;
						float _ScaleX;
						float _ScaleY;
						float _PerspectiveFilter;
						float _Sharpness;
						vec4 _MainTex_ST;
						vec4 _FaceTex_ST;
						vec4 _OutlineTex_ST;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2;
						vec4 _ScreenParams;
						vec4 unused_1_4[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[5];
						mat4x4 glstate_matrix_projection;
						vec4 unused_3_2[8];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_TEXCOORD1;
					in  vec4 in_COLOR0;
					out vec4 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD6;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					out vec4 vs_COLOR0;
					out vec3 vs_TEXCOORD7;
					out vec3 vs_TEXCOORD8;
					vec3 u_xlat0;
					int u_xlati0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					float u_xlat5;
					int u_xlati5;
					float u_xlat7;
					float u_xlat12;
					float u_xlat15;
					bool u_xlatb15;
					void main()
					{
					    u_xlat0.xy = in_POSITION0.xy + vec2(_VertexOffsetX, _VertexOffsetY);
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat1;
					    u_xlat2 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat3 = u_xlat2.yyyy * unity_MatrixVP[1];
					    u_xlat3 = unity_MatrixVP[0] * u_xlat2.xxxx + u_xlat3;
					    u_xlat3 = unity_MatrixVP[2] * u_xlat2.zzzz + u_xlat3;
					    gl_Position = unity_MatrixVP[3] * u_xlat2.wwww + u_xlat3;
					    u_xlat15 = in_TEXCOORD1.x * 0.000244140625;
					    u_xlat3.x = floor(u_xlat15);
					    u_xlat3.y = (-u_xlat3.x) * 4096.0 + in_TEXCOORD1.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(0.001953125, 0.001953125);
					    vs_TEXCOORD0.zw = u_xlat3.xy * _FaceTex_ST.xy + _FaceTex_ST.zw;
					    vs_TEXCOORD1.xy = u_xlat3.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat15 = u_xlat2.y * unity_MatrixVP[1].w;
					    u_xlat15 = unity_MatrixVP[0].w * u_xlat2.x + u_xlat15;
					    u_xlat15 = unity_MatrixVP[2].w * u_xlat2.z + u_xlat15;
					    u_xlat15 = unity_MatrixVP[3].w * u_xlat2.w + u_xlat15;
					    u_xlat2.xy = _ScreenParams.yy * glstate_matrix_projection[1].xy;
					    u_xlat2.xy = glstate_matrix_projection[0].xy * _ScreenParams.xx + u_xlat2.xy;
					    u_xlat2.xy = u_xlat2.xy * vec2(_ScaleX, _ScaleY);
					    u_xlat2.xy = vec2(u_xlat15) / u_xlat2.xy;
					    u_xlat15 = dot(u_xlat2.xy, u_xlat2.xy);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.x = abs(in_TEXCOORD1.y) * _GradientScale;
					    u_xlat7 = _Sharpness + 1.0;
					    u_xlat2.x = u_xlat7 * u_xlat2.x;
					    u_xlat7 = u_xlat15 * u_xlat2.x;
					    u_xlat12 = (-_PerspectiveFilter) + 1.0;
					    u_xlat7 = u_xlat12 * u_xlat7;
					    u_xlat15 = u_xlat15 * u_xlat2.x + (-u_xlat7);
					    u_xlat2.xzw = _WorldSpaceCameraPos.yyy * unity_WorldToObject[1].xyz;
					    u_xlat2.xzw = unity_WorldToObject[0].xyz * _WorldSpaceCameraPos.xxx + u_xlat2.xzw;
					    u_xlat2.xzw = unity_WorldToObject[2].xyz * _WorldSpaceCameraPos.zzz + u_xlat2.xzw;
					    u_xlat2.xzw = u_xlat2.xzw + unity_WorldToObject[3].xyz;
					    u_xlat0.z = in_POSITION0.z;
					    u_xlat0.xyz = (-u_xlat0.xyz) + u_xlat2.xzw;
					    u_xlat0.x = dot(in_NORMAL0.xyz, u_xlat0.xyz);
					    u_xlati5 = int((0.0<u_xlat0.x) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = int((u_xlat0.x<0.0) ? 0xFFFFFFFFu : uint(0));
					    u_xlati0 = (-u_xlati5) + u_xlati0;
					    u_xlat0.x = float(u_xlati0);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat3.y = dot(u_xlat0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat3.z = dot(u_xlat0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat3.x = dot(u_xlat0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat2.xzw = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat1.xyz;
					    u_xlat1 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat1;
					    u_xlat3.xyz = (-u_xlat2.xzw) + _WorldSpaceCameraPos.xyz;
					    vs_TEXCOORD5.xyz = u_xlat2.xzw;
					    u_xlat2.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat2.xzw = u_xlat2.xxx * u_xlat3.xyz;
					    u_xlat2.x = dot(u_xlat0.yzx, u_xlat2.xzw);
					    vs_TEXCOORD6.y = abs(u_xlat2.x) * u_xlat15 + u_xlat7;
					    u_xlatb15 = 0.0>=in_TEXCOORD1.y;
					    u_xlat15 = u_xlatb15 ? 1.0 : float(0.0);
					    u_xlat2.x = (-_WeightNormal) + _WeightBold;
					    u_xlat15 = u_xlat15 * u_xlat2.x + _WeightNormal;
					    u_xlat15 = u_xlat15 * 0.25 + _FaceDilate;
					    u_xlat15 = u_xlat15 * _ScaleRatioA;
					    vs_TEXCOORD6.x = u_xlat15 * 0.5;
					    vs_TEXCOORD2.z = u_xlat0.y;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat15 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat2.xyz = vec3(u_xlat15) * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.xyz * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat0.zxy * u_xlat2.yzx + (-u_xlat4.xyz);
					    u_xlat5 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat4.xyz = vec3(u_xlat5) * u_xlat4.xyz;
					    vs_TEXCOORD2.y = u_xlat4.x;
					    vs_TEXCOORD2.x = u_xlat2.z;
					    vs_TEXCOORD3.z = u_xlat0.z;
					    vs_TEXCOORD4.z = u_xlat0.x;
					    vs_TEXCOORD3.x = u_xlat2.x;
					    vs_TEXCOORD4.x = u_xlat2.y;
					    vs_TEXCOORD3.y = u_xlat4.y;
					    vs_TEXCOORD4.y = u_xlat4.z;
					    vs_COLOR0 = in_COLOR0;
					    u_xlat0.xyz = u_xlat3.yyy * _EnvMatrix[1].xyz;
					    u_xlat0.xyz = _EnvMatrix[0].xyz * u_xlat3.xxx + u_xlat0.xyz;
					    vs_TEXCOORD7.xyz = _EnvMatrix[2].xyz * u_xlat3.zzz + u_xlat0.xyz;
					    u_xlat0.xyz = u_xlat1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat0.xyz = unity_WorldToLight[0].xyz * u_xlat1.xxx + u_xlat0.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat1.zzz + u_xlat0.xyz;
					    vs_TEXCOORD8.xyz = unity_WorldToLight[3].xyz * u_xlat1.www + u_xlat0.xyz;
					    return;
					}"
				}
			}
			Program "fp" {
				SubProgram "d3d11 " {
					Keywords { "POINT" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						mat4x4 unity_WorldToLight;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 unused_0_19[14];
						float _ShaderFlags;
						float _ScaleRatioA;
						vec4 unused_0_22[4];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_26;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_29[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD5;
					in  vec4 vs_COLOR0;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					bool u_xlatb4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat9;
					float u_xlat11;
					bool u_xlatb11;
					float u_xlat14;
					float u_xlat16;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					float u_xlat24;
					bool u_xlatb24;
					float u_xlat25;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat1.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat21 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat22 = (-u_xlat2.w) + 0.5;
					    u_xlat22 = u_xlat22 + (-vs_TEXCOORD6.x);
					    u_xlat22 = u_xlat22 * vs_TEXCOORD6.y + 0.5;
					    u_xlat2.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat2.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat2.xz = u_xlat2.xy * vs_TEXCOORD6.yy;
					    u_xlat3 = vs_COLOR0 * _FaceColor;
					    u_xlat23 = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat4.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3 * u_xlat4;
					    u_xlat4.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat4.xy);
					    u_xlat4.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat5.w = u_xlat23 * u_xlat4.w;
					    u_xlat23 = (-u_xlat2.x) * 0.5 + u_xlat22;
					    u_xlat16 = u_xlat2.z * 0.5 + u_xlat23;
					    u_xlat9 = u_xlat2.y * vs_TEXCOORD6.y + 1.0;
					    u_xlat9 = u_xlat16 / u_xlat9;
					    u_xlat9 = clamp(u_xlat9, 0.0, 1.0);
					    u_xlat9 = (-u_xlat9) + 1.0;
					    u_xlat22 = u_xlat2.x * 0.5 + u_xlat22;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat2.x = u_xlat22 * u_xlat2.x;
					    u_xlat3.xyz = u_xlat3.www * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat4.xyz * u_xlat5.www;
					    u_xlat4 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = u_xlat2.xxxx * u_xlat4 + u_xlat3;
					    u_xlat2 = vec4(u_xlat9) * u_xlat3;
					    u_xlat3.x = max(u_xlat2.w, 9.99999975e-05);
					    u_xlat2.xyz = u_xlat2.xyz / u_xlat3.xxx;
					    u_xlat3.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat3.z = 0.0;
					    u_xlat4 = (-u_xlat3.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat5 = texture(_MainTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat6 = texture(_MainTex, u_xlat3.xy);
					    u_xlat4 = texture(_MainTex, u_xlat4.zw);
					    u_xlat3 = texture(_MainTex, u_xlat3.zw);
					    u_xlat4.x = _ShaderFlags * 0.5;
					    u_xlatb11 = u_xlat4.x>=(-u_xlat4.x);
					    u_xlat4.x = fract(abs(u_xlat4.x));
					    u_xlat4.x = (u_xlatb11) ? u_xlat4.x : (-u_xlat4.x);
					    u_xlatb4 = u_xlat4.x>=0.5;
					    u_xlat11 = vs_TEXCOORD6.x + _BevelOffset;
					    u_xlat3.x = u_xlat5.w;
					    u_xlat3.y = u_xlat6.w;
					    u_xlat3.z = u_xlat4.w;
					    u_xlat3 = vec4(u_xlat11) + u_xlat3;
					    u_xlat11 = _BevelWidth + _OutlineWidth;
					    u_xlat11 = max(u_xlat11, 0.00999999978);
					    u_xlat3 = u_xlat3 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat3 = u_xlat3 / vec4(u_xlat11);
					    u_xlat3 = u_xlat3 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat3 = clamp(u_xlat3, 0.0, 1.0);
					    u_xlat5 = u_xlat3 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat5 = -abs(u_xlat5) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat3 = (bool(u_xlatb4)) ? u_xlat5 : u_xlat3;
					    u_xlat5 = u_xlat3 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat5 = sin(u_xlat5);
					    u_xlat5 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat5 + u_xlat3;
					    u_xlat4.x = (-_BevelClamp) + 1.0;
					    u_xlat3 = min(u_xlat3, u_xlat4.xxxx);
					    u_xlat4.x = u_xlat11 * _Bevel;
					    u_xlat4.x = u_xlat4.x * _GradientScale;
					    u_xlat4.x = u_xlat4.x * -2.0;
					    u_xlat3.xz = u_xlat3.xz * u_xlat4.xx;
					    u_xlat3.yz = u_xlat3.wy * u_xlat4.xx + (-u_xlat3.zx);
					    u_xlat3.x = float(-1.0);
					    u_xlat3.w = float(1.0);
					    u_xlat24 = dot(u_xlat3.zw, u_xlat3.zw);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat4.yz = vec2(u_xlat24) * vec2(1.0, 0.0);
					    u_xlat4.x = u_xlat24 * u_xlat3.z;
					    u_xlat24 = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat3.z = 0.0;
					    u_xlat3.xyz = vec3(u_xlat24) * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat4.xyz;
					    u_xlat3.xyz = u_xlat4.zxy * u_xlat3.yzx + (-u_xlat5.xyz);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat24 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat24 = min(u_xlat24, 1.0);
					    u_xlat24 = (-u_xlat24) + 1.0;
					    u_xlat4.z = sqrt(u_xlat24);
					    u_xlat24 = (-_BumpFace) + _BumpOutline;
					    u_xlat24 = u_xlat22 * u_xlat24 + _BumpFace;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat24) + vec3(-0.0, -0.0, -1.0);
					    u_xlat4.xyz = u_xlat2.www * u_xlat4.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat3.xyz = u_xlat3.xyz + (-u_xlat4.xyz);
					    u_xlat24 = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat3.xyz = vec3(u_xlat24) * u_xlat3.xyz;
					    u_xlat24 = (-unused_0_26.y) + unused_0_26.z;
					    u_xlat22 = u_xlat22 * u_xlat24 + unused_0_26.y;
					    u_xlat4.xyz = vs_TEXCOORD5.yyy * unity_WorldToLight[1].xyz;
					    u_xlat4.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD5.xxx + u_xlat4.xyz;
					    u_xlat4.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD5.zzz + u_xlat4.xyz;
					    u_xlat4.xyz = u_xlat4.xyz + unity_WorldToLight[3].xyz;
					    u_xlatb24 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb24){
					        u_xlatb24 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD5.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD5.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD5.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb24)) ? u_xlat5.xyz : vs_TEXCOORD5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat24 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat25 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat24, u_xlat25);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat24 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat24 = clamp(u_xlat24, 0.0, 1.0);
					    u_xlat4.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat4 = texture(_LightTexture0, u_xlat4.xx);
					    u_xlat24 = u_xlat24 * u_xlat4.x;
					    u_xlat4.x = dot(vs_TEXCOORD2.xyz, (-u_xlat3.xyz));
					    u_xlat4.y = dot(vs_TEXCOORD3.xyz, (-u_xlat3.xyz));
					    u_xlat4.z = dot(vs_TEXCOORD4.xyz, (-u_xlat3.xyz));
					    u_xlat3.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat3.x = inversesqrt(u_xlat3.x);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat4.xyz;
					    u_xlat4.xyz = vec3(u_xlat24) * _LightColor0.xyz;
					    u_xlat1.xyz = u_xlat1.xyz * vec3(u_xlat21) + u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat1.xyz;
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat0.xyz);
					    u_xlat0.y = dot(u_xlat3.xyz, u_xlat1.xyz);
					    u_xlat0.xy = max(u_xlat0.xy, vec2(0.0, 0.0));
					    u_xlat14 = u_xlat22 * 128.0;
					    u_xlat7.x = log2(u_xlat0.y);
					    u_xlat7.x = u_xlat7.x * u_xlat14;
					    u_xlat7.x = exp2(u_xlat7.x);
					    u_xlat1.xyz = u_xlat2.xyz * u_xlat4.xyz;
					    u_xlat2.xyz = u_xlat4.xyz * _SpecColor.xyz;
					    u_xlat7.xyz = u_xlat7.xxx * u_xlat2.xyz;
					    SV_Target0.xyz = u_xlat1.xyz * u_xlat0.xxx + u_xlat7.xyz;
					    SV_Target0.w = u_xlat2.w;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "GLOW_ON" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 unused_0_18[12];
						vec4 _GlowColor;
						float _GlowOffset;
						float _GlowOuter;
						float _GlowInner;
						float _GlowPower;
						float _ShaderFlags;
						float _ScaleRatioA;
						float _ScaleRatioB;
						vec4 unused_0_27[3];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_31;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_34[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD5;
					in  vec4 vs_COLOR0;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					float u_xlat8;
					vec3 u_xlat9;
					bool u_xlatb9;
					vec3 u_xlat11;
					float u_xlat17;
					bool u_xlatb17;
					float u_xlat24;
					float u_xlat25;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat24 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat1 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat1.x = (-u_xlat1.w) + 0.5;
					    u_xlat1.x = u_xlat1.x + (-vs_TEXCOORD6.x);
					    u_xlat1.x = u_xlat1.x * vs_TEXCOORD6.y + 0.5;
					    u_xlat9.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat9.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat9.xz = u_xlat9.xy * vs_TEXCOORD6.yy;
					    u_xlat2 = vs_COLOR0 * _FaceColor;
					    u_xlat3.x = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat11.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat11.xy);
					    u_xlat2 = u_xlat2 * u_xlat4;
					    u_xlat11.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat11.xy);
					    u_xlat11.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat4.w = u_xlat3.x * u_xlat4.w;
					    u_xlat3.x = (-u_xlat9.x) * 0.5 + u_xlat1.x;
					    u_xlat25 = u_xlat9.z * 0.5 + u_xlat3.x;
					    u_xlat17 = u_xlat9.y * vs_TEXCOORD6.y + 1.0;
					    u_xlat17 = u_xlat25 / u_xlat17;
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlat17 = (-u_xlat17) + 1.0;
					    u_xlat25 = u_xlat9.x * 0.5 + u_xlat1.x;
					    u_xlat25 = clamp(u_xlat25, 0.0, 1.0);
					    u_xlat9.x = min(u_xlat9.x, 1.0);
					    u_xlat9.x = sqrt(u_xlat9.x);
					    u_xlat9.x = u_xlat9.x * u_xlat25;
					    u_xlat2.xyz = u_xlat2.www * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat11.xyz * u_xlat4.www;
					    u_xlat3 = (-u_xlat2) + u_xlat4;
					    u_xlat2 = u_xlat9.xxxx * u_xlat3 + u_xlat2;
					    u_xlat3 = vec4(u_xlat17) * u_xlat2;
					    u_xlat9.x = max(u_xlat3.w, 9.99999975e-05);
					    u_xlat2.xyz = u_xlat3.xyz / u_xlat9.xxx;
					    u_xlat3.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat3.z = 0.0;
					    u_xlat4 = (-u_xlat3.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat5 = texture(_MainTex, u_xlat4.xy);
					    u_xlat6 = u_xlat3.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat7 = texture(_MainTex, u_xlat6.xy);
					    u_xlat4 = texture(_MainTex, u_xlat4.zw);
					    u_xlat6 = texture(_MainTex, u_xlat6.zw);
					    u_xlat9.x = _ShaderFlags * 0.5;
					    u_xlatb3 = u_xlat9.x>=(-u_xlat9.x);
					    u_xlat9.x = fract(abs(u_xlat9.x));
					    u_xlat9.x = (u_xlatb3) ? u_xlat9.x : (-u_xlat9.x);
					    u_xlatb9 = u_xlat9.x>=0.5;
					    u_xlat3.x = vs_TEXCOORD6.x + _BevelOffset;
					    u_xlat6.x = u_xlat5.w;
					    u_xlat6.y = u_xlat7.w;
					    u_xlat6.z = u_xlat4.w;
					    u_xlat4 = u_xlat3.xxxx + u_xlat6;
					    u_xlat3.x = _BevelWidth + _OutlineWidth;
					    u_xlat3.x = max(u_xlat3.x, 0.00999999978);
					    u_xlat4 = u_xlat4 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat4 = u_xlat4 / u_xlat3.xxxx;
					    u_xlat4 = u_xlat4 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat4 = clamp(u_xlat4, 0.0, 1.0);
					    u_xlat5 = u_xlat4 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat5 = -abs(u_xlat5) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat4 = (bool(u_xlatb9)) ? u_xlat5 : u_xlat4;
					    u_xlat5 = u_xlat4 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat5 = sin(u_xlat5);
					    u_xlat5 = (-u_xlat4) + u_xlat5;
					    u_xlat4 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat5 + u_xlat4;
					    u_xlat9.x = (-_BevelClamp) + 1.0;
					    u_xlat4 = min(u_xlat9.xxxx, u_xlat4);
					    u_xlat9.x = u_xlat3.x * _Bevel;
					    u_xlat9.x = u_xlat9.x * _GradientScale;
					    u_xlat9.x = u_xlat9.x * -2.0;
					    u_xlat3.xy = u_xlat9.xx * u_xlat4.xz;
					    u_xlat4.yz = u_xlat4.wy * u_xlat9.xx + (-u_xlat3.yx);
					    u_xlat4.x = float(-1.0);
					    u_xlat4.w = float(1.0);
					    u_xlat9.x = dot(u_xlat4.zw, u_xlat4.zw);
					    u_xlat9.x = inversesqrt(u_xlat9.x);
					    u_xlat3.yz = u_xlat9.xx * vec2(1.0, 0.0);
					    u_xlat3.x = u_xlat9.x * u_xlat4.z;
					    u_xlat9.x = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat9.x = inversesqrt(u_xlat9.x);
					    u_xlat4.z = 0.0;
					    u_xlat4.xyz = u_xlat9.xxx * u_xlat4.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat4.xyz;
					    u_xlat3.xyz = u_xlat3.zxy * u_xlat4.yzx + (-u_xlat5.xyz);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat9.x = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat9.x = min(u_xlat9.x, 1.0);
					    u_xlat9.x = (-u_xlat9.x) + 1.0;
					    u_xlat4.z = sqrt(u_xlat9.x);
					    u_xlat9.x = (-_BumpFace) + _BumpOutline;
					    u_xlat9.x = u_xlat25 * u_xlat9.x + _BumpFace;
					    u_xlat4.xyz = u_xlat4.xyz * u_xlat9.xxx + vec3(-0.0, -0.0, -1.0);
					    u_xlat4.xyz = u_xlat3.www * u_xlat4.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat3.xyz = u_xlat3.xyz + (-u_xlat4.xyz);
					    u_xlat9.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat9.x = inversesqrt(u_xlat9.x);
					    u_xlat3.xyz = u_xlat9.xxx * u_xlat3.xyz;
					    u_xlat9.x = _GlowOffset * _ScaleRatioB;
					    u_xlat9.x = u_xlat9.x * vs_TEXCOORD6.y;
					    u_xlat1.x = (-u_xlat9.x) * 0.5 + u_xlat1.x;
					    u_xlatb9 = u_xlat1.x>=0.0;
					    u_xlat9.x = u_xlatb9 ? 1.0 : float(0.0);
					    u_xlat4.x = _GlowOuter * _ScaleRatioB + (-_GlowInner);
					    u_xlat9.x = u_xlat9.x * u_xlat4.x + _GlowInner;
					    u_xlat9.x = u_xlat9.x * vs_TEXCOORD6.y;
					    u_xlat4.x = u_xlat9.x * 0.5;
					    u_xlat9.x = u_xlat9.x * 0.5 + 1.0;
					    u_xlat1.x = u_xlat1.x / u_xlat9.x;
					    u_xlat1.x = min(abs(u_xlat1.x), 1.0);
					    u_xlat1.x = log2(u_xlat1.x);
					    u_xlat1.x = u_xlat1.x * _GlowPower;
					    u_xlat1.x = exp2(u_xlat1.x);
					    u_xlat1.x = (-u_xlat1.x) + 1.0;
					    u_xlat9.x = min(u_xlat4.x, 1.0);
					    u_xlat9.x = sqrt(u_xlat9.x);
					    u_xlat1.x = u_xlat9.x * u_xlat1.x;
					    u_xlat1.x = dot(_GlowColor.ww, u_xlat1.xx);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat9.x = u_xlat1.x * vs_COLOR0.w;
					    u_xlat2.xyz = u_xlat3.www * u_xlat2.xyz;
					    u_xlat1.x = (-u_xlat1.x) * vs_COLOR0.w + 1.0;
					    u_xlat2.xyz = u_xlat2.xyz * u_xlat1.xxx;
					    u_xlat2.xyz = _GlowColor.xyz * u_xlat9.xxx + u_xlat2.xyz;
					    u_xlat1.x = (-u_xlat2.w) * u_xlat17 + 1.0;
					    u_xlat1.x = u_xlat1.x * u_xlat9.x + u_xlat3.w;
					    u_xlat9.x = max(u_xlat1.x, 9.99999975e-05);
					    u_xlat2.xyz = u_xlat2.xyz / u_xlat9.xxx;
					    u_xlat9.x = (-unused_0_31.y) + unused_0_31.z;
					    u_xlat9.x = u_xlat25 * u_xlat9.x + unused_0_31.y;
					    u_xlatb17 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb17){
					        u_xlatb17 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD5.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD5.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD5.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat4.xyz = (bool(u_xlatb17)) ? u_xlat4.xyz : vs_TEXCOORD5.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat4.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat17 = u_xlat4.y * 0.25 + 0.75;
					        u_xlat25 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat25, u_xlat17);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat17 = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlat4.x = dot(vs_TEXCOORD2.xyz, (-u_xlat3.xyz));
					    u_xlat4.y = dot(vs_TEXCOORD3.xyz, (-u_xlat3.xyz));
					    u_xlat4.z = dot(vs_TEXCOORD4.xyz, (-u_xlat3.xyz));
					    u_xlat25 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat25 = inversesqrt(u_xlat25);
					    u_xlat3.xyz = vec3(u_xlat25) * u_xlat4.xyz;
					    u_xlat4.xyz = vec3(u_xlat17) * _LightColor0.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat24) + _WorldSpaceLightPos0.xyz;
					    u_xlat24 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat0.xyz = vec3(u_xlat24) * u_xlat0.xyz;
					    u_xlat0.w = dot(u_xlat3.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat0.xyz);
					    u_xlat0.xw = max(u_xlat0.xw, vec2(0.0, 0.0));
					    u_xlat8 = u_xlat9.x * 128.0;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * u_xlat8;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat9.xyz = u_xlat2.xyz * u_xlat4.xyz;
					    u_xlat2.xyz = u_xlat4.xyz * _SpecColor.xyz;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat2.xyz;
					    SV_Target0.xyz = u_xlat9.xyz * u_xlat0.www + u_xlat0.xyz;
					    SV_Target0.w = u_xlat1.x;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 unused_0_18[14];
						float _ShaderFlags;
						float _ScaleRatioA;
						vec4 unused_0_21[4];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_25;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_28[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD5;
					in  vec4 vs_COLOR0;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					bool u_xlatb2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					float u_xlat7;
					vec3 u_xlat8;
					vec2 u_xlat9;
					bool u_xlatb9;
					vec3 u_xlat10;
					float u_xlat15;
					float u_xlat21;
					float u_xlat22;
					float u_xlat24;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat1.x = (-u_xlat1.w) + 0.5;
					    u_xlat1.x = u_xlat1.x + (-vs_TEXCOORD6.x);
					    u_xlat1.x = u_xlat1.x * vs_TEXCOORD6.y + 0.5;
					    u_xlat8.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat8.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat8.xz = u_xlat8.xy * vs_TEXCOORD6.yy;
					    u_xlat2 = vs_COLOR0 * _FaceColor;
					    u_xlat3.x = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat10.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat10.xy);
					    u_xlat2 = u_xlat2 * u_xlat4;
					    u_xlat10.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat10.xy);
					    u_xlat10.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat4.w = u_xlat3.x * u_xlat4.w;
					    u_xlat3.x = (-u_xlat8.x) * 0.5 + u_xlat1.x;
					    u_xlat22 = u_xlat8.z * 0.5 + u_xlat3.x;
					    u_xlat15 = u_xlat8.y * vs_TEXCOORD6.y + 1.0;
					    u_xlat15 = u_xlat22 / u_xlat15;
					    u_xlat15 = clamp(u_xlat15, 0.0, 1.0);
					    u_xlat15 = (-u_xlat15) + 1.0;
					    u_xlat1.x = u_xlat8.x * 0.5 + u_xlat1.x;
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat8.x = min(u_xlat8.x, 1.0);
					    u_xlat8.x = sqrt(u_xlat8.x);
					    u_xlat8.x = u_xlat8.x * u_xlat1.x;
					    u_xlat2.xyz = u_xlat2.www * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat10.xyz * u_xlat4.www;
					    u_xlat3 = (-u_xlat2) + u_xlat4;
					    u_xlat2 = u_xlat8.xxxx * u_xlat3 + u_xlat2;
					    u_xlat2 = vec4(u_xlat15) * u_xlat2;
					    u_xlat8.x = max(u_xlat2.w, 9.99999975e-05);
					    u_xlat8.xyz = u_xlat2.xyz / u_xlat8.xxx;
					    u_xlat2.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat2.z = 0.0;
					    u_xlat3 = (-u_xlat2.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat4 = texture(_MainTex, u_xlat3.xy);
					    u_xlat5 = u_xlat2.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat6 = texture(_MainTex, u_xlat5.xy);
					    u_xlat3 = texture(_MainTex, u_xlat3.zw);
					    u_xlat5 = texture(_MainTex, u_xlat5.zw);
					    u_xlat2.x = _ShaderFlags * 0.5;
					    u_xlatb9 = u_xlat2.x>=(-u_xlat2.x);
					    u_xlat2.x = fract(abs(u_xlat2.x));
					    u_xlat2.x = (u_xlatb9) ? u_xlat2.x : (-u_xlat2.x);
					    u_xlatb2 = u_xlat2.x>=0.5;
					    u_xlat9.x = vs_TEXCOORD6.x + _BevelOffset;
					    u_xlat5.x = u_xlat4.w;
					    u_xlat5.y = u_xlat6.w;
					    u_xlat5.z = u_xlat3.w;
					    u_xlat3 = u_xlat9.xxxx + u_xlat5;
					    u_xlat9.x = _BevelWidth + _OutlineWidth;
					    u_xlat9.x = max(u_xlat9.x, 0.00999999978);
					    u_xlat3 = u_xlat3 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat3 = u_xlat3 / u_xlat9.xxxx;
					    u_xlat3 = u_xlat3 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat3 = clamp(u_xlat3, 0.0, 1.0);
					    u_xlat4 = u_xlat3 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat4 = -abs(u_xlat4) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat3 = (bool(u_xlatb2)) ? u_xlat4 : u_xlat3;
					    u_xlat4 = u_xlat3 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat4 = sin(u_xlat4);
					    u_xlat4 = (-u_xlat3) + u_xlat4;
					    u_xlat3 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat4 + u_xlat3;
					    u_xlat2.x = (-_BevelClamp) + 1.0;
					    u_xlat3 = min(u_xlat2.xxxx, u_xlat3);
					    u_xlat2.x = u_xlat9.x * _Bevel;
					    u_xlat2.x = u_xlat2.x * _GradientScale;
					    u_xlat2.x = u_xlat2.x * -2.0;
					    u_xlat9.xy = u_xlat2.xx * u_xlat3.xz;
					    u_xlat3.yz = u_xlat3.wy * u_xlat2.xx + (-u_xlat9.yx);
					    u_xlat3.x = float(-1.0);
					    u_xlat3.w = float(1.0);
					    u_xlat2.x = dot(u_xlat3.zw, u_xlat3.zw);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat4.yz = u_xlat2.xx * vec2(1.0, 0.0);
					    u_xlat4.x = u_xlat2.x * u_xlat3.z;
					    u_xlat2.x = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat3.z = 0.0;
					    u_xlat2.xyz = u_xlat2.xxx * u_xlat3.xyz;
					    u_xlat3.xyz = u_xlat2.xyz * u_xlat4.xyz;
					    u_xlat2.xyz = u_xlat4.zxy * u_xlat2.yzx + (-u_xlat3.xyz);
					    u_xlat3 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat3.x = u_xlat3.w * u_xlat3.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat24 = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat24 = min(u_xlat24, 1.0);
					    u_xlat24 = (-u_xlat24) + 1.0;
					    u_xlat3.z = sqrt(u_xlat24);
					    u_xlat24 = (-_BumpFace) + _BumpOutline;
					    u_xlat24 = u_xlat1.x * u_xlat24 + _BumpFace;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(u_xlat24) + vec3(-0.0, -0.0, -1.0);
					    u_xlat3.xyz = u_xlat2.www * u_xlat3.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat2.xyz = u_xlat2.xyz + (-u_xlat3.xyz);
					    u_xlat3.x = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat3.x = inversesqrt(u_xlat3.x);
					    u_xlat2.xyz = u_xlat2.xyz * u_xlat3.xxx;
					    u_xlat3.x = (-unused_0_25.y) + unused_0_25.z;
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + unused_0_25.y;
					    u_xlatb3 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb3){
					        u_xlatb3 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat10.xyz = vs_TEXCOORD5.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat10.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD5.xxx + u_xlat10.xyz;
					        u_xlat10.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD5.zzz + u_xlat10.xyz;
					        u_xlat10.xyz = u_xlat10.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat3.xyz = (bool(u_xlatb3)) ? u_xlat10.xyz : vs_TEXCOORD5.xyz;
					        u_xlat3.xyz = u_xlat3.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat3.yzw = u_xlat3.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat10.x = u_xlat3.y * 0.25 + 0.75;
					        u_xlat4.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat3.x = max(u_xlat10.x, u_xlat4.x);
					        u_xlat3 = texture(unity_ProbeVolumeSH, u_xlat3.xzw);
					    } else {
					        u_xlat3.x = float(1.0);
					        u_xlat3.y = float(1.0);
					        u_xlat3.z = float(1.0);
					        u_xlat3.w = float(1.0);
					    }
					    u_xlat3.x = dot(u_xlat3, unity_OcclusionMaskSelector);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat4.x = dot(vs_TEXCOORD2.xyz, (-u_xlat2.xyz));
					    u_xlat4.y = dot(vs_TEXCOORD3.xyz, (-u_xlat2.xyz));
					    u_xlat4.z = dot(vs_TEXCOORD4.xyz, (-u_xlat2.xyz));
					    u_xlat2.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat2.xyz = u_xlat2.xxx * u_xlat4.xyz;
					    u_xlat3.xyz = u_xlat3.xxx * _LightColor0.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat0.w = dot(u_xlat2.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat0.x = dot(u_xlat2.xyz, u_xlat0.xyz);
					    u_xlat0.xw = max(u_xlat0.xw, vec2(0.0, 0.0));
					    u_xlat7 = u_xlat1.x * 128.0;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * u_xlat7;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat1.xyz = u_xlat8.xyz * u_xlat3.xyz;
					    u_xlat2.xyz = u_xlat3.xyz * _SpecColor.xyz;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat2.xyz;
					    SV_Target0.xyz = u_xlat1.xyz * u_xlat0.www + u_xlat0.xyz;
					    SV_Target0.w = u_xlat2.w;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "GLOW_ON" "SPOT" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						mat4x4 unity_WorldToLight;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 unused_0_19[12];
						vec4 _GlowColor;
						float _GlowOffset;
						float _GlowOuter;
						float _GlowInner;
						float _GlowPower;
						float _ShaderFlags;
						float _ScaleRatioA;
						float _ScaleRatioB;
						vec4 unused_0_28[3];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_32;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_35[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTexture0;
					uniform  sampler2D _LightTextureB0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD5;
					in  vec4 vs_COLOR0;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					bool u_xlatb2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					bool u_xlatb5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec3 u_xlat9;
					float u_xlat11;
					vec3 u_xlat12;
					vec2 u_xlat14;
					float u_xlat18;
					float u_xlat20;
					float u_xlat27;
					float u_xlat28;
					float u_xlat29;
					bool u_xlatb29;
					float u_xlat30;
					float u_xlat31;
					bool u_xlatb31;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat0.xyz = vec3(u_xlat27) * u_xlat0.xyz;
					    u_xlat1.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat27 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat28 = (-u_xlat2.w) + 0.5;
					    u_xlat28 = u_xlat28 + (-vs_TEXCOORD6.x);
					    u_xlat28 = u_xlat28 * vs_TEXCOORD6.y + 0.5;
					    u_xlat2.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat2.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat2.xz = u_xlat2.xy * vs_TEXCOORD6.yy;
					    u_xlat3 = vs_COLOR0 * _FaceColor;
					    u_xlat29 = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat4.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3 * u_xlat4;
					    u_xlat4.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat4.xy);
					    u_xlat4.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat5.w = u_xlat29 * u_xlat4.w;
					    u_xlat29 = (-u_xlat2.x) * 0.5 + u_xlat28;
					    u_xlat20 = u_xlat2.z * 0.5 + u_xlat29;
					    u_xlat11 = u_xlat2.y * vs_TEXCOORD6.y + 1.0;
					    u_xlat11 = u_xlat20 / u_xlat11;
					    u_xlat11 = clamp(u_xlat11, 0.0, 1.0);
					    u_xlat11 = (-u_xlat11) + 1.0;
					    u_xlat20 = u_xlat2.x * 0.5 + u_xlat28;
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat2.x = u_xlat2.x * u_xlat20;
					    u_xlat3.xyz = u_xlat3.www * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat4.xyz * u_xlat5.www;
					    u_xlat4 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = u_xlat2.xxxx * u_xlat4 + u_xlat3;
					    u_xlat4 = vec4(u_xlat11) * u_xlat3;
					    u_xlat2.x = max(u_xlat4.w, 9.99999975e-05);
					    u_xlat3.xyz = u_xlat4.xyz / u_xlat2.xxx;
					    u_xlat4.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat4.z = 0.0;
					    u_xlat5 = (-u_xlat4.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat6 = texture(_MainTex, u_xlat5.xy);
					    u_xlat7 = u_xlat4.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat8 = texture(_MainTex, u_xlat7.xy);
					    u_xlat5 = texture(_MainTex, u_xlat5.zw);
					    u_xlat7 = texture(_MainTex, u_xlat7.zw);
					    u_xlat2.x = _ShaderFlags * 0.5;
					    u_xlatb29 = u_xlat2.x>=(-u_xlat2.x);
					    u_xlat2.x = fract(abs(u_xlat2.x));
					    u_xlat2.x = (u_xlatb29) ? u_xlat2.x : (-u_xlat2.x);
					    u_xlatb2 = u_xlat2.x>=0.5;
					    u_xlat29 = vs_TEXCOORD6.x + _BevelOffset;
					    u_xlat7.x = u_xlat6.w;
					    u_xlat7.y = u_xlat8.w;
					    u_xlat7.z = u_xlat5.w;
					    u_xlat5 = vec4(u_xlat29) + u_xlat7;
					    u_xlat29 = _BevelWidth + _OutlineWidth;
					    u_xlat29 = max(u_xlat29, 0.00999999978);
					    u_xlat5 = u_xlat5 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat5 = u_xlat5 / vec4(u_xlat29);
					    u_xlat5 = u_xlat5 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat5 = clamp(u_xlat5, 0.0, 1.0);
					    u_xlat6 = u_xlat5 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat6 = -abs(u_xlat6) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat5 = (bool(u_xlatb2)) ? u_xlat6 : u_xlat5;
					    u_xlat6 = u_xlat5 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat6 = sin(u_xlat6);
					    u_xlat6 = (-u_xlat5) + u_xlat6;
					    u_xlat5 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat6 + u_xlat5;
					    u_xlat2.x = (-_BevelClamp) + 1.0;
					    u_xlat5 = min(u_xlat2.xxxx, u_xlat5);
					    u_xlat2.x = u_xlat29 * _Bevel;
					    u_xlat2.x = u_xlat2.x * _GradientScale;
					    u_xlat2.x = u_xlat2.x * -2.0;
					    u_xlat4.xy = u_xlat2.xx * u_xlat5.xz;
					    u_xlat5.yz = u_xlat5.wy * u_xlat2.xx + (-u_xlat4.yx);
					    u_xlat5.x = float(-1.0);
					    u_xlat5.w = float(1.0);
					    u_xlat2.x = dot(u_xlat5.zw, u_xlat5.zw);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat4.yz = u_xlat2.xx * vec2(1.0, 0.0);
					    u_xlat4.x = u_xlat2.x * u_xlat5.z;
					    u_xlat2.x = dot(u_xlat5.xy, u_xlat5.xy);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat5.z = 0.0;
					    u_xlat5.xyz = u_xlat2.xxx * u_xlat5.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat5.xyz;
					    u_xlat4.xyz = u_xlat4.zxy * u_xlat5.yzx + (-u_xlat6.xyz);
					    u_xlat5 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat5.x = u_xlat5.w * u_xlat5.x;
					    u_xlat5.xy = u_xlat5.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat2.x = dot(u_xlat5.xy, u_xlat5.xy);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = (-u_xlat2.x) + 1.0;
					    u_xlat5.z = sqrt(u_xlat2.x);
					    u_xlat2.x = (-_BumpFace) + _BumpOutline;
					    u_xlat2.x = u_xlat20 * u_xlat2.x + _BumpFace;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat2.xxx + vec3(-0.0, -0.0, -1.0);
					    u_xlat5.xyz = u_xlat4.www * u_xlat5.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat4.xyz = u_xlat4.xyz + (-u_xlat5.xyz);
					    u_xlat2.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat4.xyz = u_xlat2.xxx * u_xlat4.xyz;
					    u_xlat2.x = _GlowOffset * _ScaleRatioB;
					    u_xlat2.x = u_xlat2.x * vs_TEXCOORD6.y;
					    u_xlat28 = (-u_xlat2.x) * 0.5 + u_xlat28;
					    u_xlatb2 = u_xlat28>=0.0;
					    u_xlat2.x = u_xlatb2 ? 1.0 : float(0.0);
					    u_xlat29 = _GlowOuter * _ScaleRatioB + (-_GlowInner);
					    u_xlat2.x = u_xlat2.x * u_xlat29 + _GlowInner;
					    u_xlat2.x = u_xlat2.x * vs_TEXCOORD6.y;
					    u_xlat29 = u_xlat2.x * 0.5;
					    u_xlat2.x = u_xlat2.x * 0.5 + 1.0;
					    u_xlat28 = u_xlat28 / u_xlat2.x;
					    u_xlat28 = min(abs(u_xlat28), 1.0);
					    u_xlat28 = log2(u_xlat28);
					    u_xlat28 = u_xlat28 * _GlowPower;
					    u_xlat28 = exp2(u_xlat28);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat2.x = min(u_xlat29, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat28 = u_xlat28 * u_xlat2.x;
					    u_xlat28 = dot(_GlowColor.ww, vec2(u_xlat28));
					    u_xlat28 = clamp(u_xlat28, 0.0, 1.0);
					    u_xlat2.x = u_xlat28 * vs_COLOR0.w;
					    u_xlat3.xyz = u_xlat4.www * u_xlat3.xyz;
					    u_xlat28 = (-u_xlat28) * vs_COLOR0.w + 1.0;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(u_xlat28);
					    u_xlat3.xyz = _GlowColor.xyz * u_xlat2.xxx + u_xlat3.xyz;
					    u_xlat28 = (-u_xlat3.w) * u_xlat11 + 1.0;
					    u_xlat28 = u_xlat28 * u_xlat2.x + u_xlat4.w;
					    u_xlat2.x = max(u_xlat28, 9.99999975e-05);
					    u_xlat2.xyw = u_xlat3.xyz / u_xlat2.xxx;
					    u_xlat3.x = (-unused_0_32.y) + unused_0_32.z;
					    u_xlat20 = u_xlat20 * u_xlat3.x + unused_0_32.y;
					    u_xlat3 = vs_TEXCOORD5.yyyy * unity_WorldToLight[1];
					    u_xlat3 = unity_WorldToLight[0] * vs_TEXCOORD5.xxxx + u_xlat3;
					    u_xlat3 = unity_WorldToLight[2] * vs_TEXCOORD5.zzzz + u_xlat3;
					    u_xlat3 = u_xlat3 + unity_WorldToLight[3];
					    u_xlatb31 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb31){
					        u_xlatb31 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD5.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD5.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD5.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb31)) ? u_xlat5.xyz : vs_TEXCOORD5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat31 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat14.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat31, u_xlat14.x);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat31 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat31 = clamp(u_xlat31, 0.0, 1.0);
					    u_xlatb5 = 0.0<u_xlat3.z;
					    u_xlat5.x = u_xlatb5 ? 1.0 : float(0.0);
					    u_xlat14.xy = u_xlat3.xy / u_xlat3.ww;
					    u_xlat14.xy = u_xlat14.xy + vec2(0.5, 0.5);
					    u_xlat6 = texture(_LightTexture0, u_xlat14.xy);
					    u_xlat30 = u_xlat5.x * u_xlat6.w;
					    u_xlat3.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat5 = texture(_LightTextureB0, u_xlat3.xx);
					    u_xlat3.x = u_xlat30 * u_xlat5.x;
					    u_xlat3.x = u_xlat31 * u_xlat3.x;
					    u_xlat5.x = dot(vs_TEXCOORD2.xyz, (-u_xlat4.xyz));
					    u_xlat5.y = dot(vs_TEXCOORD3.xyz, (-u_xlat4.xyz));
					    u_xlat5.z = dot(vs_TEXCOORD4.xyz, (-u_xlat4.xyz));
					    u_xlat12.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat12.x = inversesqrt(u_xlat12.x);
					    u_xlat12.xyz = u_xlat12.xxx * u_xlat5.xyz;
					    u_xlat4.xyz = u_xlat3.xxx * _LightColor0.xyz;
					    u_xlat1.xyz = u_xlat1.xyz * vec3(u_xlat27) + u_xlat0.xyz;
					    u_xlat27 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat1.xyz = vec3(u_xlat27) * u_xlat1.xyz;
					    u_xlat0.x = dot(u_xlat12.xyz, u_xlat0.xyz);
					    u_xlat0.y = dot(u_xlat12.xyz, u_xlat1.xyz);
					    u_xlat0.xy = max(u_xlat0.xy, vec2(0.0, 0.0));
					    u_xlat18 = u_xlat20 * 128.0;
					    u_xlat9.x = log2(u_xlat0.y);
					    u_xlat9.x = u_xlat9.x * u_xlat18;
					    u_xlat9.x = exp2(u_xlat9.x);
					    u_xlat1.xyz = u_xlat2.xyw * u_xlat4.xyz;
					    u_xlat2.xyz = u_xlat4.xyz * _SpecColor.xyz;
					    u_xlat9.xyz = u_xlat9.xxx * u_xlat2.xyz;
					    SV_Target0.xyz = u_xlat1.xyz * u_xlat0.xxx + u_xlat9.xyz;
					    SV_Target0.w = u_xlat28;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "SPOT" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						mat4x4 unity_WorldToLight;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 unused_0_19[14];
						float _ShaderFlags;
						float _ScaleRatioA;
						vec4 unused_0_22[4];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_26;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_29[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTexture0;
					uniform  sampler2D _LightTextureB0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD5;
					in  vec4 vs_COLOR0;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					bool u_xlatb4;
					vec4 u_xlat5;
					bool u_xlatb5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat9;
					float u_xlat11;
					bool u_xlatb11;
					vec2 u_xlat12;
					float u_xlat14;
					float u_xlat16;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					float u_xlat24;
					bool u_xlatb24;
					float u_xlat25;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat1.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat21 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat22 = (-u_xlat2.w) + 0.5;
					    u_xlat22 = u_xlat22 + (-vs_TEXCOORD6.x);
					    u_xlat22 = u_xlat22 * vs_TEXCOORD6.y + 0.5;
					    u_xlat2.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat2.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat2.xz = u_xlat2.xy * vs_TEXCOORD6.yy;
					    u_xlat3 = vs_COLOR0 * _FaceColor;
					    u_xlat23 = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat4.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3 * u_xlat4;
					    u_xlat4.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat4.xy);
					    u_xlat4.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat5.w = u_xlat23 * u_xlat4.w;
					    u_xlat23 = (-u_xlat2.x) * 0.5 + u_xlat22;
					    u_xlat16 = u_xlat2.z * 0.5 + u_xlat23;
					    u_xlat9 = u_xlat2.y * vs_TEXCOORD6.y + 1.0;
					    u_xlat9 = u_xlat16 / u_xlat9;
					    u_xlat9 = clamp(u_xlat9, 0.0, 1.0);
					    u_xlat9 = (-u_xlat9) + 1.0;
					    u_xlat22 = u_xlat2.x * 0.5 + u_xlat22;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat2.x = u_xlat22 * u_xlat2.x;
					    u_xlat3.xyz = u_xlat3.www * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat4.xyz * u_xlat5.www;
					    u_xlat4 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = u_xlat2.xxxx * u_xlat4 + u_xlat3;
					    u_xlat2 = vec4(u_xlat9) * u_xlat3;
					    u_xlat3.x = max(u_xlat2.w, 9.99999975e-05);
					    u_xlat2.xyz = u_xlat2.xyz / u_xlat3.xxx;
					    u_xlat3.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat3.z = 0.0;
					    u_xlat4 = (-u_xlat3.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat5 = texture(_MainTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat6 = texture(_MainTex, u_xlat3.xy);
					    u_xlat4 = texture(_MainTex, u_xlat4.zw);
					    u_xlat3 = texture(_MainTex, u_xlat3.zw);
					    u_xlat4.x = _ShaderFlags * 0.5;
					    u_xlatb11 = u_xlat4.x>=(-u_xlat4.x);
					    u_xlat4.x = fract(abs(u_xlat4.x));
					    u_xlat4.x = (u_xlatb11) ? u_xlat4.x : (-u_xlat4.x);
					    u_xlatb4 = u_xlat4.x>=0.5;
					    u_xlat11 = vs_TEXCOORD6.x + _BevelOffset;
					    u_xlat3.x = u_xlat5.w;
					    u_xlat3.y = u_xlat6.w;
					    u_xlat3.z = u_xlat4.w;
					    u_xlat3 = vec4(u_xlat11) + u_xlat3;
					    u_xlat11 = _BevelWidth + _OutlineWidth;
					    u_xlat11 = max(u_xlat11, 0.00999999978);
					    u_xlat3 = u_xlat3 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat3 = u_xlat3 / vec4(u_xlat11);
					    u_xlat3 = u_xlat3 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat3 = clamp(u_xlat3, 0.0, 1.0);
					    u_xlat5 = u_xlat3 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat5 = -abs(u_xlat5) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat3 = (bool(u_xlatb4)) ? u_xlat5 : u_xlat3;
					    u_xlat5 = u_xlat3 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat5 = sin(u_xlat5);
					    u_xlat5 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat5 + u_xlat3;
					    u_xlat4.x = (-_BevelClamp) + 1.0;
					    u_xlat3 = min(u_xlat3, u_xlat4.xxxx);
					    u_xlat4.x = u_xlat11 * _Bevel;
					    u_xlat4.x = u_xlat4.x * _GradientScale;
					    u_xlat4.x = u_xlat4.x * -2.0;
					    u_xlat3.xz = u_xlat3.xz * u_xlat4.xx;
					    u_xlat3.yz = u_xlat3.wy * u_xlat4.xx + (-u_xlat3.zx);
					    u_xlat3.x = float(-1.0);
					    u_xlat3.w = float(1.0);
					    u_xlat24 = dot(u_xlat3.zw, u_xlat3.zw);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat4.yz = vec2(u_xlat24) * vec2(1.0, 0.0);
					    u_xlat4.x = u_xlat24 * u_xlat3.z;
					    u_xlat24 = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat3.z = 0.0;
					    u_xlat3.xyz = vec3(u_xlat24) * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat4.xyz;
					    u_xlat3.xyz = u_xlat4.zxy * u_xlat3.yzx + (-u_xlat5.xyz);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat24 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat24 = min(u_xlat24, 1.0);
					    u_xlat24 = (-u_xlat24) + 1.0;
					    u_xlat4.z = sqrt(u_xlat24);
					    u_xlat24 = (-_BumpFace) + _BumpOutline;
					    u_xlat24 = u_xlat22 * u_xlat24 + _BumpFace;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat24) + vec3(-0.0, -0.0, -1.0);
					    u_xlat4.xyz = u_xlat2.www * u_xlat4.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat3.xyz = u_xlat3.xyz + (-u_xlat4.xyz);
					    u_xlat24 = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat3.xyz = vec3(u_xlat24) * u_xlat3.xyz;
					    u_xlat24 = (-unused_0_26.y) + unused_0_26.z;
					    u_xlat22 = u_xlat22 * u_xlat24 + unused_0_26.y;
					    u_xlat4 = vs_TEXCOORD5.yyyy * unity_WorldToLight[1];
					    u_xlat4 = unity_WorldToLight[0] * vs_TEXCOORD5.xxxx + u_xlat4;
					    u_xlat4 = unity_WorldToLight[2] * vs_TEXCOORD5.zzzz + u_xlat4;
					    u_xlat4 = u_xlat4 + unity_WorldToLight[3];
					    u_xlatb24 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb24){
					        u_xlatb24 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD5.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD5.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD5.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb24)) ? u_xlat5.xyz : vs_TEXCOORD5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat24 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat12.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat24, u_xlat12.x);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat24 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat24 = clamp(u_xlat24, 0.0, 1.0);
					    u_xlatb5 = 0.0<u_xlat4.z;
					    u_xlat5.x = u_xlatb5 ? 1.0 : float(0.0);
					    u_xlat12.xy = u_xlat4.xy / u_xlat4.ww;
					    u_xlat12.xy = u_xlat12.xy + vec2(0.5, 0.5);
					    u_xlat6 = texture(_LightTexture0, u_xlat12.xy);
					    u_xlat25 = u_xlat5.x * u_xlat6.w;
					    u_xlat4.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat5 = texture(_LightTextureB0, u_xlat4.xx);
					    u_xlat4.x = u_xlat25 * u_xlat5.x;
					    u_xlat24 = u_xlat24 * u_xlat4.x;
					    u_xlat4.x = dot(vs_TEXCOORD2.xyz, (-u_xlat3.xyz));
					    u_xlat4.y = dot(vs_TEXCOORD3.xyz, (-u_xlat3.xyz));
					    u_xlat4.z = dot(vs_TEXCOORD4.xyz, (-u_xlat3.xyz));
					    u_xlat3.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat3.x = inversesqrt(u_xlat3.x);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat4.xyz;
					    u_xlat4.xyz = vec3(u_xlat24) * _LightColor0.xyz;
					    u_xlat1.xyz = u_xlat1.xyz * vec3(u_xlat21) + u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat1.xyz;
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat0.xyz);
					    u_xlat0.y = dot(u_xlat3.xyz, u_xlat1.xyz);
					    u_xlat0.xy = max(u_xlat0.xy, vec2(0.0, 0.0));
					    u_xlat14 = u_xlat22 * 128.0;
					    u_xlat7.x = log2(u_xlat0.y);
					    u_xlat7.x = u_xlat7.x * u_xlat14;
					    u_xlat7.x = exp2(u_xlat7.x);
					    u_xlat1.xyz = u_xlat2.xyz * u_xlat4.xyz;
					    u_xlat2.xyz = u_xlat4.xyz * _SpecColor.xyz;
					    u_xlat7.xyz = u_xlat7.xxx * u_xlat2.xyz;
					    SV_Target0.xyz = u_xlat1.xyz * u_xlat0.xxx + u_xlat7.xyz;
					    SV_Target0.w = u_xlat2.w;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "GLOW_ON" "POINT_COOKIE" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						mat4x4 unity_WorldToLight;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 unused_0_19[12];
						vec4 _GlowColor;
						float _GlowOffset;
						float _GlowOuter;
						float _GlowInner;
						float _GlowPower;
						float _ShaderFlags;
						float _ScaleRatioA;
						float _ScaleRatioB;
						vec4 unused_0_28[3];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_32;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_35[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTextureB0;
					uniform  samplerCube _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD5;
					in  vec4 vs_COLOR0;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					bool u_xlatb2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec3 u_xlat9;
					float u_xlat11;
					vec3 u_xlat12;
					float u_xlat18;
					float u_xlat20;
					float u_xlat27;
					float u_xlat28;
					float u_xlat29;
					bool u_xlatb29;
					float u_xlat30;
					bool u_xlatb30;
					float u_xlat31;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat0.xyz = vec3(u_xlat27) * u_xlat0.xyz;
					    u_xlat1.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat27 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat28 = (-u_xlat2.w) + 0.5;
					    u_xlat28 = u_xlat28 + (-vs_TEXCOORD6.x);
					    u_xlat28 = u_xlat28 * vs_TEXCOORD6.y + 0.5;
					    u_xlat2.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat2.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat2.xz = u_xlat2.xy * vs_TEXCOORD6.yy;
					    u_xlat3 = vs_COLOR0 * _FaceColor;
					    u_xlat29 = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat4.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3 * u_xlat4;
					    u_xlat4.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat4.xy);
					    u_xlat4.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat5.w = u_xlat29 * u_xlat4.w;
					    u_xlat29 = (-u_xlat2.x) * 0.5 + u_xlat28;
					    u_xlat20 = u_xlat2.z * 0.5 + u_xlat29;
					    u_xlat11 = u_xlat2.y * vs_TEXCOORD6.y + 1.0;
					    u_xlat11 = u_xlat20 / u_xlat11;
					    u_xlat11 = clamp(u_xlat11, 0.0, 1.0);
					    u_xlat11 = (-u_xlat11) + 1.0;
					    u_xlat20 = u_xlat2.x * 0.5 + u_xlat28;
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat2.x = u_xlat2.x * u_xlat20;
					    u_xlat3.xyz = u_xlat3.www * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat4.xyz * u_xlat5.www;
					    u_xlat4 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = u_xlat2.xxxx * u_xlat4 + u_xlat3;
					    u_xlat4 = vec4(u_xlat11) * u_xlat3;
					    u_xlat2.x = max(u_xlat4.w, 9.99999975e-05);
					    u_xlat3.xyz = u_xlat4.xyz / u_xlat2.xxx;
					    u_xlat4.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat4.z = 0.0;
					    u_xlat5 = (-u_xlat4.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat6 = texture(_MainTex, u_xlat5.xy);
					    u_xlat7 = u_xlat4.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat8 = texture(_MainTex, u_xlat7.xy);
					    u_xlat5 = texture(_MainTex, u_xlat5.zw);
					    u_xlat7 = texture(_MainTex, u_xlat7.zw);
					    u_xlat2.x = _ShaderFlags * 0.5;
					    u_xlatb29 = u_xlat2.x>=(-u_xlat2.x);
					    u_xlat2.x = fract(abs(u_xlat2.x));
					    u_xlat2.x = (u_xlatb29) ? u_xlat2.x : (-u_xlat2.x);
					    u_xlatb2 = u_xlat2.x>=0.5;
					    u_xlat29 = vs_TEXCOORD6.x + _BevelOffset;
					    u_xlat7.x = u_xlat6.w;
					    u_xlat7.y = u_xlat8.w;
					    u_xlat7.z = u_xlat5.w;
					    u_xlat5 = vec4(u_xlat29) + u_xlat7;
					    u_xlat29 = _BevelWidth + _OutlineWidth;
					    u_xlat29 = max(u_xlat29, 0.00999999978);
					    u_xlat5 = u_xlat5 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat5 = u_xlat5 / vec4(u_xlat29);
					    u_xlat5 = u_xlat5 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat5 = clamp(u_xlat5, 0.0, 1.0);
					    u_xlat6 = u_xlat5 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat6 = -abs(u_xlat6) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat5 = (bool(u_xlatb2)) ? u_xlat6 : u_xlat5;
					    u_xlat6 = u_xlat5 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat6 = sin(u_xlat6);
					    u_xlat6 = (-u_xlat5) + u_xlat6;
					    u_xlat5 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat6 + u_xlat5;
					    u_xlat2.x = (-_BevelClamp) + 1.0;
					    u_xlat5 = min(u_xlat2.xxxx, u_xlat5);
					    u_xlat2.x = u_xlat29 * _Bevel;
					    u_xlat2.x = u_xlat2.x * _GradientScale;
					    u_xlat2.x = u_xlat2.x * -2.0;
					    u_xlat4.xy = u_xlat2.xx * u_xlat5.xz;
					    u_xlat5.yz = u_xlat5.wy * u_xlat2.xx + (-u_xlat4.yx);
					    u_xlat5.x = float(-1.0);
					    u_xlat5.w = float(1.0);
					    u_xlat2.x = dot(u_xlat5.zw, u_xlat5.zw);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat4.yz = u_xlat2.xx * vec2(1.0, 0.0);
					    u_xlat4.x = u_xlat2.x * u_xlat5.z;
					    u_xlat2.x = dot(u_xlat5.xy, u_xlat5.xy);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat5.z = 0.0;
					    u_xlat5.xyz = u_xlat2.xxx * u_xlat5.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat5.xyz;
					    u_xlat4.xyz = u_xlat4.zxy * u_xlat5.yzx + (-u_xlat6.xyz);
					    u_xlat5 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat5.x = u_xlat5.w * u_xlat5.x;
					    u_xlat5.xy = u_xlat5.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat2.x = dot(u_xlat5.xy, u_xlat5.xy);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = (-u_xlat2.x) + 1.0;
					    u_xlat5.z = sqrt(u_xlat2.x);
					    u_xlat2.x = (-_BumpFace) + _BumpOutline;
					    u_xlat2.x = u_xlat20 * u_xlat2.x + _BumpFace;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat2.xxx + vec3(-0.0, -0.0, -1.0);
					    u_xlat5.xyz = u_xlat4.www * u_xlat5.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat4.xyz = u_xlat4.xyz + (-u_xlat5.xyz);
					    u_xlat2.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat4.xyz = u_xlat2.xxx * u_xlat4.xyz;
					    u_xlat2.x = _GlowOffset * _ScaleRatioB;
					    u_xlat2.x = u_xlat2.x * vs_TEXCOORD6.y;
					    u_xlat28 = (-u_xlat2.x) * 0.5 + u_xlat28;
					    u_xlatb2 = u_xlat28>=0.0;
					    u_xlat2.x = u_xlatb2 ? 1.0 : float(0.0);
					    u_xlat29 = _GlowOuter * _ScaleRatioB + (-_GlowInner);
					    u_xlat2.x = u_xlat2.x * u_xlat29 + _GlowInner;
					    u_xlat2.x = u_xlat2.x * vs_TEXCOORD6.y;
					    u_xlat29 = u_xlat2.x * 0.5;
					    u_xlat2.x = u_xlat2.x * 0.5 + 1.0;
					    u_xlat28 = u_xlat28 / u_xlat2.x;
					    u_xlat28 = min(abs(u_xlat28), 1.0);
					    u_xlat28 = log2(u_xlat28);
					    u_xlat28 = u_xlat28 * _GlowPower;
					    u_xlat28 = exp2(u_xlat28);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat2.x = min(u_xlat29, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat28 = u_xlat28 * u_xlat2.x;
					    u_xlat28 = dot(_GlowColor.ww, vec2(u_xlat28));
					    u_xlat28 = clamp(u_xlat28, 0.0, 1.0);
					    u_xlat2.x = u_xlat28 * vs_COLOR0.w;
					    u_xlat3.xyz = u_xlat4.www * u_xlat3.xyz;
					    u_xlat28 = (-u_xlat28) * vs_COLOR0.w + 1.0;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(u_xlat28);
					    u_xlat3.xyz = _GlowColor.xyz * u_xlat2.xxx + u_xlat3.xyz;
					    u_xlat28 = (-u_xlat3.w) * u_xlat11 + 1.0;
					    u_xlat28 = u_xlat28 * u_xlat2.x + u_xlat4.w;
					    u_xlat2.x = max(u_xlat28, 9.99999975e-05);
					    u_xlat2.xyw = u_xlat3.xyz / u_xlat2.xxx;
					    u_xlat3.x = (-unused_0_32.y) + unused_0_32.z;
					    u_xlat20 = u_xlat20 * u_xlat3.x + unused_0_32.y;
					    u_xlat3.xyz = vs_TEXCOORD5.yyy * unity_WorldToLight[1].xyz;
					    u_xlat3.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD5.xxx + u_xlat3.xyz;
					    u_xlat3.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD5.zzz + u_xlat3.xyz;
					    u_xlat3.xyz = u_xlat3.xyz + unity_WorldToLight[3].xyz;
					    u_xlatb30 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb30){
					        u_xlatb30 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD5.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD5.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD5.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb30)) ? u_xlat5.xyz : vs_TEXCOORD5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat30 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat31 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat30, u_xlat31);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat30 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat30 = clamp(u_xlat30, 0.0, 1.0);
					    u_xlat31 = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat5 = texture(_LightTextureB0, vec2(u_xlat31));
					    u_xlat6 = texture(_LightTexture0, u_xlat3.xyz);
					    u_xlat3.x = u_xlat5.x * u_xlat6.w;
					    u_xlat3.x = u_xlat30 * u_xlat3.x;
					    u_xlat5.x = dot(vs_TEXCOORD2.xyz, (-u_xlat4.xyz));
					    u_xlat5.y = dot(vs_TEXCOORD3.xyz, (-u_xlat4.xyz));
					    u_xlat5.z = dot(vs_TEXCOORD4.xyz, (-u_xlat4.xyz));
					    u_xlat12.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat12.x = inversesqrt(u_xlat12.x);
					    u_xlat12.xyz = u_xlat12.xxx * u_xlat5.xyz;
					    u_xlat4.xyz = u_xlat3.xxx * _LightColor0.xyz;
					    u_xlat1.xyz = u_xlat1.xyz * vec3(u_xlat27) + u_xlat0.xyz;
					    u_xlat27 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat1.xyz = vec3(u_xlat27) * u_xlat1.xyz;
					    u_xlat0.x = dot(u_xlat12.xyz, u_xlat0.xyz);
					    u_xlat0.y = dot(u_xlat12.xyz, u_xlat1.xyz);
					    u_xlat0.xy = max(u_xlat0.xy, vec2(0.0, 0.0));
					    u_xlat18 = u_xlat20 * 128.0;
					    u_xlat9.x = log2(u_xlat0.y);
					    u_xlat9.x = u_xlat9.x * u_xlat18;
					    u_xlat9.x = exp2(u_xlat9.x);
					    u_xlat1.xyz = u_xlat2.xyw * u_xlat4.xyz;
					    u_xlat2.xyz = u_xlat4.xyz * _SpecColor.xyz;
					    u_xlat9.xyz = u_xlat9.xxx * u_xlat2.xyz;
					    SV_Target0.xyz = u_xlat1.xyz * u_xlat0.xxx + u_xlat9.xyz;
					    SV_Target0.w = u_xlat28;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT_COOKIE" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						mat4x4 unity_WorldToLight;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 unused_0_19[14];
						float _ShaderFlags;
						float _ScaleRatioA;
						vec4 unused_0_22[4];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_26;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_29[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTextureB0;
					uniform  samplerCube _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD5;
					in  vec4 vs_COLOR0;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					bool u_xlatb4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat9;
					float u_xlat11;
					bool u_xlatb11;
					float u_xlat14;
					float u_xlat16;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					float u_xlat24;
					bool u_xlatb24;
					float u_xlat25;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat1.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat21 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat22 = (-u_xlat2.w) + 0.5;
					    u_xlat22 = u_xlat22 + (-vs_TEXCOORD6.x);
					    u_xlat22 = u_xlat22 * vs_TEXCOORD6.y + 0.5;
					    u_xlat2.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat2.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat2.xz = u_xlat2.xy * vs_TEXCOORD6.yy;
					    u_xlat3 = vs_COLOR0 * _FaceColor;
					    u_xlat23 = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat4.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3 * u_xlat4;
					    u_xlat4.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat4.xy);
					    u_xlat4.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat5.w = u_xlat23 * u_xlat4.w;
					    u_xlat23 = (-u_xlat2.x) * 0.5 + u_xlat22;
					    u_xlat16 = u_xlat2.z * 0.5 + u_xlat23;
					    u_xlat9 = u_xlat2.y * vs_TEXCOORD6.y + 1.0;
					    u_xlat9 = u_xlat16 / u_xlat9;
					    u_xlat9 = clamp(u_xlat9, 0.0, 1.0);
					    u_xlat9 = (-u_xlat9) + 1.0;
					    u_xlat22 = u_xlat2.x * 0.5 + u_xlat22;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat2.x = u_xlat22 * u_xlat2.x;
					    u_xlat3.xyz = u_xlat3.www * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat4.xyz * u_xlat5.www;
					    u_xlat4 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = u_xlat2.xxxx * u_xlat4 + u_xlat3;
					    u_xlat2 = vec4(u_xlat9) * u_xlat3;
					    u_xlat3.x = max(u_xlat2.w, 9.99999975e-05);
					    u_xlat2.xyz = u_xlat2.xyz / u_xlat3.xxx;
					    u_xlat3.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat3.z = 0.0;
					    u_xlat4 = (-u_xlat3.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat5 = texture(_MainTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat6 = texture(_MainTex, u_xlat3.xy);
					    u_xlat4 = texture(_MainTex, u_xlat4.zw);
					    u_xlat3 = texture(_MainTex, u_xlat3.zw);
					    u_xlat4.x = _ShaderFlags * 0.5;
					    u_xlatb11 = u_xlat4.x>=(-u_xlat4.x);
					    u_xlat4.x = fract(abs(u_xlat4.x));
					    u_xlat4.x = (u_xlatb11) ? u_xlat4.x : (-u_xlat4.x);
					    u_xlatb4 = u_xlat4.x>=0.5;
					    u_xlat11 = vs_TEXCOORD6.x + _BevelOffset;
					    u_xlat3.x = u_xlat5.w;
					    u_xlat3.y = u_xlat6.w;
					    u_xlat3.z = u_xlat4.w;
					    u_xlat3 = vec4(u_xlat11) + u_xlat3;
					    u_xlat11 = _BevelWidth + _OutlineWidth;
					    u_xlat11 = max(u_xlat11, 0.00999999978);
					    u_xlat3 = u_xlat3 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat3 = u_xlat3 / vec4(u_xlat11);
					    u_xlat3 = u_xlat3 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat3 = clamp(u_xlat3, 0.0, 1.0);
					    u_xlat5 = u_xlat3 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat5 = -abs(u_xlat5) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat3 = (bool(u_xlatb4)) ? u_xlat5 : u_xlat3;
					    u_xlat5 = u_xlat3 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat5 = sin(u_xlat5);
					    u_xlat5 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat5 + u_xlat3;
					    u_xlat4.x = (-_BevelClamp) + 1.0;
					    u_xlat3 = min(u_xlat3, u_xlat4.xxxx);
					    u_xlat4.x = u_xlat11 * _Bevel;
					    u_xlat4.x = u_xlat4.x * _GradientScale;
					    u_xlat4.x = u_xlat4.x * -2.0;
					    u_xlat3.xz = u_xlat3.xz * u_xlat4.xx;
					    u_xlat3.yz = u_xlat3.wy * u_xlat4.xx + (-u_xlat3.zx);
					    u_xlat3.x = float(-1.0);
					    u_xlat3.w = float(1.0);
					    u_xlat24 = dot(u_xlat3.zw, u_xlat3.zw);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat4.yz = vec2(u_xlat24) * vec2(1.0, 0.0);
					    u_xlat4.x = u_xlat24 * u_xlat3.z;
					    u_xlat24 = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat3.z = 0.0;
					    u_xlat3.xyz = vec3(u_xlat24) * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat4.xyz;
					    u_xlat3.xyz = u_xlat4.zxy * u_xlat3.yzx + (-u_xlat5.xyz);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat24 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat24 = min(u_xlat24, 1.0);
					    u_xlat24 = (-u_xlat24) + 1.0;
					    u_xlat4.z = sqrt(u_xlat24);
					    u_xlat24 = (-_BumpFace) + _BumpOutline;
					    u_xlat24 = u_xlat22 * u_xlat24 + _BumpFace;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat24) + vec3(-0.0, -0.0, -1.0);
					    u_xlat4.xyz = u_xlat2.www * u_xlat4.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat3.xyz = u_xlat3.xyz + (-u_xlat4.xyz);
					    u_xlat24 = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat3.xyz = vec3(u_xlat24) * u_xlat3.xyz;
					    u_xlat24 = (-unused_0_26.y) + unused_0_26.z;
					    u_xlat22 = u_xlat22 * u_xlat24 + unused_0_26.y;
					    u_xlat4.xyz = vs_TEXCOORD5.yyy * unity_WorldToLight[1].xyz;
					    u_xlat4.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD5.xxx + u_xlat4.xyz;
					    u_xlat4.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD5.zzz + u_xlat4.xyz;
					    u_xlat4.xyz = u_xlat4.xyz + unity_WorldToLight[3].xyz;
					    u_xlatb24 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb24){
					        u_xlatb24 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD5.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD5.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD5.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb24)) ? u_xlat5.xyz : vs_TEXCOORD5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat24 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat25 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat24, u_xlat25);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat24 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat24 = clamp(u_xlat24, 0.0, 1.0);
					    u_xlat25 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat5 = texture(_LightTextureB0, vec2(u_xlat25));
					    u_xlat4 = texture(_LightTexture0, u_xlat4.xyz);
					    u_xlat4.x = u_xlat4.w * u_xlat5.x;
					    u_xlat24 = u_xlat24 * u_xlat4.x;
					    u_xlat4.x = dot(vs_TEXCOORD2.xyz, (-u_xlat3.xyz));
					    u_xlat4.y = dot(vs_TEXCOORD3.xyz, (-u_xlat3.xyz));
					    u_xlat4.z = dot(vs_TEXCOORD4.xyz, (-u_xlat3.xyz));
					    u_xlat3.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat3.x = inversesqrt(u_xlat3.x);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat4.xyz;
					    u_xlat4.xyz = vec3(u_xlat24) * _LightColor0.xyz;
					    u_xlat1.xyz = u_xlat1.xyz * vec3(u_xlat21) + u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat1.xyz;
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat0.xyz);
					    u_xlat0.y = dot(u_xlat3.xyz, u_xlat1.xyz);
					    u_xlat0.xy = max(u_xlat0.xy, vec2(0.0, 0.0));
					    u_xlat14 = u_xlat22 * 128.0;
					    u_xlat7.x = log2(u_xlat0.y);
					    u_xlat7.x = u_xlat7.x * u_xlat14;
					    u_xlat7.x = exp2(u_xlat7.x);
					    u_xlat1.xyz = u_xlat2.xyz * u_xlat4.xyz;
					    u_xlat2.xyz = u_xlat4.xyz * _SpecColor.xyz;
					    u_xlat7.xyz = u_xlat7.xxx * u_xlat2.xyz;
					    SV_Target0.xyz = u_xlat1.xyz * u_xlat0.xxx + u_xlat7.xyz;
					    SV_Target0.w = u_xlat2.w;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" "GLOW_ON" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						mat4x4 unity_WorldToLight;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 unused_0_19[12];
						vec4 _GlowColor;
						float _GlowOffset;
						float _GlowOuter;
						float _GlowInner;
						float _GlowPower;
						float _ShaderFlags;
						float _ScaleRatioA;
						float _ScaleRatioB;
						vec4 unused_0_28[3];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_32;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_35[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD5;
					in  vec4 vs_COLOR0;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					float u_xlat8;
					vec3 u_xlat9;
					bool u_xlatb9;
					vec3 u_xlat11;
					vec2 u_xlat17;
					float u_xlat24;
					float u_xlat25;
					float u_xlat26;
					bool u_xlatb26;
					float u_xlat27;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat24 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat1 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat1.x = (-u_xlat1.w) + 0.5;
					    u_xlat1.x = u_xlat1.x + (-vs_TEXCOORD6.x);
					    u_xlat1.x = u_xlat1.x * vs_TEXCOORD6.y + 0.5;
					    u_xlat9.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat9.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat9.xz = u_xlat9.xy * vs_TEXCOORD6.yy;
					    u_xlat2 = vs_COLOR0 * _FaceColor;
					    u_xlat3.x = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat11.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat11.xy);
					    u_xlat2 = u_xlat2 * u_xlat4;
					    u_xlat11.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat11.xy);
					    u_xlat11.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat4.w = u_xlat3.x * u_xlat4.w;
					    u_xlat3.x = (-u_xlat9.x) * 0.5 + u_xlat1.x;
					    u_xlat25 = u_xlat9.z * 0.5 + u_xlat3.x;
					    u_xlat17.x = u_xlat9.y * vs_TEXCOORD6.y + 1.0;
					    u_xlat17.x = u_xlat25 / u_xlat17.x;
					    u_xlat17.x = clamp(u_xlat17.x, 0.0, 1.0);
					    u_xlat17.x = (-u_xlat17.x) + 1.0;
					    u_xlat25 = u_xlat9.x * 0.5 + u_xlat1.x;
					    u_xlat25 = clamp(u_xlat25, 0.0, 1.0);
					    u_xlat9.x = min(u_xlat9.x, 1.0);
					    u_xlat9.x = sqrt(u_xlat9.x);
					    u_xlat9.x = u_xlat9.x * u_xlat25;
					    u_xlat2.xyz = u_xlat2.www * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat11.xyz * u_xlat4.www;
					    u_xlat3 = (-u_xlat2) + u_xlat4;
					    u_xlat2 = u_xlat9.xxxx * u_xlat3 + u_xlat2;
					    u_xlat3 = u_xlat17.xxxx * u_xlat2;
					    u_xlat9.x = max(u_xlat3.w, 9.99999975e-05);
					    u_xlat2.xyz = u_xlat3.xyz / u_xlat9.xxx;
					    u_xlat3.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat3.z = 0.0;
					    u_xlat4 = (-u_xlat3.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat5 = texture(_MainTex, u_xlat4.xy);
					    u_xlat6 = u_xlat3.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat7 = texture(_MainTex, u_xlat6.xy);
					    u_xlat4 = texture(_MainTex, u_xlat4.zw);
					    u_xlat6 = texture(_MainTex, u_xlat6.zw);
					    u_xlat9.x = _ShaderFlags * 0.5;
					    u_xlatb3 = u_xlat9.x>=(-u_xlat9.x);
					    u_xlat9.x = fract(abs(u_xlat9.x));
					    u_xlat9.x = (u_xlatb3) ? u_xlat9.x : (-u_xlat9.x);
					    u_xlatb9 = u_xlat9.x>=0.5;
					    u_xlat3.x = vs_TEXCOORD6.x + _BevelOffset;
					    u_xlat6.x = u_xlat5.w;
					    u_xlat6.y = u_xlat7.w;
					    u_xlat6.z = u_xlat4.w;
					    u_xlat4 = u_xlat3.xxxx + u_xlat6;
					    u_xlat3.x = _BevelWidth + _OutlineWidth;
					    u_xlat3.x = max(u_xlat3.x, 0.00999999978);
					    u_xlat4 = u_xlat4 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat4 = u_xlat4 / u_xlat3.xxxx;
					    u_xlat4 = u_xlat4 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat4 = clamp(u_xlat4, 0.0, 1.0);
					    u_xlat5 = u_xlat4 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat5 = -abs(u_xlat5) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat4 = (bool(u_xlatb9)) ? u_xlat5 : u_xlat4;
					    u_xlat5 = u_xlat4 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat5 = sin(u_xlat5);
					    u_xlat5 = (-u_xlat4) + u_xlat5;
					    u_xlat4 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat5 + u_xlat4;
					    u_xlat9.x = (-_BevelClamp) + 1.0;
					    u_xlat4 = min(u_xlat9.xxxx, u_xlat4);
					    u_xlat9.x = u_xlat3.x * _Bevel;
					    u_xlat9.x = u_xlat9.x * _GradientScale;
					    u_xlat9.x = u_xlat9.x * -2.0;
					    u_xlat3.xy = u_xlat9.xx * u_xlat4.xz;
					    u_xlat4.yz = u_xlat4.wy * u_xlat9.xx + (-u_xlat3.yx);
					    u_xlat4.x = float(-1.0);
					    u_xlat4.w = float(1.0);
					    u_xlat9.x = dot(u_xlat4.zw, u_xlat4.zw);
					    u_xlat9.x = inversesqrt(u_xlat9.x);
					    u_xlat3.yz = u_xlat9.xx * vec2(1.0, 0.0);
					    u_xlat3.x = u_xlat9.x * u_xlat4.z;
					    u_xlat9.x = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat9.x = inversesqrt(u_xlat9.x);
					    u_xlat4.z = 0.0;
					    u_xlat4.xyz = u_xlat9.xxx * u_xlat4.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat4.xyz;
					    u_xlat3.xyz = u_xlat3.zxy * u_xlat4.yzx + (-u_xlat5.xyz);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat9.x = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat9.x = min(u_xlat9.x, 1.0);
					    u_xlat9.x = (-u_xlat9.x) + 1.0;
					    u_xlat4.z = sqrt(u_xlat9.x);
					    u_xlat9.x = (-_BumpFace) + _BumpOutline;
					    u_xlat9.x = u_xlat25 * u_xlat9.x + _BumpFace;
					    u_xlat4.xyz = u_xlat4.xyz * u_xlat9.xxx + vec3(-0.0, -0.0, -1.0);
					    u_xlat4.xyz = u_xlat3.www * u_xlat4.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat3.xyz = u_xlat3.xyz + (-u_xlat4.xyz);
					    u_xlat9.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat9.x = inversesqrt(u_xlat9.x);
					    u_xlat3.xyz = u_xlat9.xxx * u_xlat3.xyz;
					    u_xlat9.x = _GlowOffset * _ScaleRatioB;
					    u_xlat9.x = u_xlat9.x * vs_TEXCOORD6.y;
					    u_xlat1.x = (-u_xlat9.x) * 0.5 + u_xlat1.x;
					    u_xlatb9 = u_xlat1.x>=0.0;
					    u_xlat9.x = u_xlatb9 ? 1.0 : float(0.0);
					    u_xlat4.x = _GlowOuter * _ScaleRatioB + (-_GlowInner);
					    u_xlat9.x = u_xlat9.x * u_xlat4.x + _GlowInner;
					    u_xlat9.x = u_xlat9.x * vs_TEXCOORD6.y;
					    u_xlat4.x = u_xlat9.x * 0.5;
					    u_xlat9.x = u_xlat9.x * 0.5 + 1.0;
					    u_xlat1.x = u_xlat1.x / u_xlat9.x;
					    u_xlat1.x = min(abs(u_xlat1.x), 1.0);
					    u_xlat1.x = log2(u_xlat1.x);
					    u_xlat1.x = u_xlat1.x * _GlowPower;
					    u_xlat1.x = exp2(u_xlat1.x);
					    u_xlat1.x = (-u_xlat1.x) + 1.0;
					    u_xlat9.x = min(u_xlat4.x, 1.0);
					    u_xlat9.x = sqrt(u_xlat9.x);
					    u_xlat1.x = u_xlat9.x * u_xlat1.x;
					    u_xlat1.x = dot(_GlowColor.ww, u_xlat1.xx);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat9.x = u_xlat1.x * vs_COLOR0.w;
					    u_xlat2.xyz = u_xlat3.www * u_xlat2.xyz;
					    u_xlat1.x = (-u_xlat1.x) * vs_COLOR0.w + 1.0;
					    u_xlat2.xyz = u_xlat2.xyz * u_xlat1.xxx;
					    u_xlat2.xyz = _GlowColor.xyz * u_xlat9.xxx + u_xlat2.xyz;
					    u_xlat1.x = (-u_xlat2.w) * u_xlat17.x + 1.0;
					    u_xlat1.x = u_xlat1.x * u_xlat9.x + u_xlat3.w;
					    u_xlat9.x = max(u_xlat1.x, 9.99999975e-05);
					    u_xlat2.xyz = u_xlat2.xyz / u_xlat9.xxx;
					    u_xlat9.x = (-unused_0_32.y) + unused_0_32.z;
					    u_xlat9.x = u_xlat25 * u_xlat9.x + unused_0_32.y;
					    u_xlat17.xy = vs_TEXCOORD5.yy * unity_WorldToLight[1].xy;
					    u_xlat17.xy = unity_WorldToLight[0].xy * vs_TEXCOORD5.xx + u_xlat17.xy;
					    u_xlat17.xy = unity_WorldToLight[2].xy * vs_TEXCOORD5.zz + u_xlat17.xy;
					    u_xlat17.xy = u_xlat17.xy + unity_WorldToLight[3].xy;
					    u_xlatb26 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb26){
					        u_xlatb26 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD5.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD5.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD5.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat4.xyz = (bool(u_xlatb26)) ? u_xlat4.xyz : vs_TEXCOORD5.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat4.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat26 = u_xlat4.y * 0.25 + 0.75;
					        u_xlat27 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat26, u_xlat27);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat26 = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat26 = clamp(u_xlat26, 0.0, 1.0);
					    u_xlat4 = texture(_LightTexture0, u_xlat17.xy);
					    u_xlat17.x = u_xlat26 * u_xlat4.w;
					    u_xlat4.x = dot(vs_TEXCOORD2.xyz, (-u_xlat3.xyz));
					    u_xlat4.y = dot(vs_TEXCOORD3.xyz, (-u_xlat3.xyz));
					    u_xlat4.z = dot(vs_TEXCOORD4.xyz, (-u_xlat3.xyz));
					    u_xlat25 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat25 = inversesqrt(u_xlat25);
					    u_xlat3.xyz = vec3(u_xlat25) * u_xlat4.xyz;
					    u_xlat4.xyz = u_xlat17.xxx * _LightColor0.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat24) + _WorldSpaceLightPos0.xyz;
					    u_xlat24 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat0.xyz = vec3(u_xlat24) * u_xlat0.xyz;
					    u_xlat0.w = dot(u_xlat3.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat0.x = dot(u_xlat3.xyz, u_xlat0.xyz);
					    u_xlat0.xw = max(u_xlat0.xw, vec2(0.0, 0.0));
					    u_xlat8 = u_xlat9.x * 128.0;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * u_xlat8;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat9.xyz = u_xlat2.xyz * u_xlat4.xyz;
					    u_xlat2.xyz = u_xlat4.xyz * _SpecColor.xyz;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat2.xyz;
					    SV_Target0.xyz = u_xlat9.xyz * u_xlat0.www + u_xlat0.xyz;
					    SV_Target0.w = u_xlat1.x;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						mat4x4 unity_WorldToLight;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 unused_0_19[14];
						float _ShaderFlags;
						float _ScaleRatioA;
						vec4 unused_0_22[4];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_26;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_29[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD5;
					in  vec4 vs_COLOR0;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					bool u_xlatb2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					float u_xlat7;
					vec3 u_xlat8;
					vec2 u_xlat9;
					bool u_xlatb9;
					vec3 u_xlat10;
					float u_xlat15;
					float u_xlat17;
					bool u_xlatb17;
					float u_xlat21;
					float u_xlat22;
					float u_xlat24;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat1.x = (-u_xlat1.w) + 0.5;
					    u_xlat1.x = u_xlat1.x + (-vs_TEXCOORD6.x);
					    u_xlat1.x = u_xlat1.x * vs_TEXCOORD6.y + 0.5;
					    u_xlat8.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat8.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat8.xz = u_xlat8.xy * vs_TEXCOORD6.yy;
					    u_xlat2 = vs_COLOR0 * _FaceColor;
					    u_xlat3.x = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat10.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat10.xy);
					    u_xlat2 = u_xlat2 * u_xlat4;
					    u_xlat10.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat10.xy);
					    u_xlat10.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat4.w = u_xlat3.x * u_xlat4.w;
					    u_xlat3.x = (-u_xlat8.x) * 0.5 + u_xlat1.x;
					    u_xlat22 = u_xlat8.z * 0.5 + u_xlat3.x;
					    u_xlat15 = u_xlat8.y * vs_TEXCOORD6.y + 1.0;
					    u_xlat15 = u_xlat22 / u_xlat15;
					    u_xlat15 = clamp(u_xlat15, 0.0, 1.0);
					    u_xlat15 = (-u_xlat15) + 1.0;
					    u_xlat1.x = u_xlat8.x * 0.5 + u_xlat1.x;
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat8.x = min(u_xlat8.x, 1.0);
					    u_xlat8.x = sqrt(u_xlat8.x);
					    u_xlat8.x = u_xlat8.x * u_xlat1.x;
					    u_xlat2.xyz = u_xlat2.www * u_xlat2.xyz;
					    u_xlat4.xyz = u_xlat10.xyz * u_xlat4.www;
					    u_xlat3 = (-u_xlat2) + u_xlat4;
					    u_xlat2 = u_xlat8.xxxx * u_xlat3 + u_xlat2;
					    u_xlat2 = vec4(u_xlat15) * u_xlat2;
					    u_xlat8.x = max(u_xlat2.w, 9.99999975e-05);
					    u_xlat8.xyz = u_xlat2.xyz / u_xlat8.xxx;
					    u_xlat2.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat2.z = 0.0;
					    u_xlat3 = (-u_xlat2.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat4 = texture(_MainTex, u_xlat3.xy);
					    u_xlat5 = u_xlat2.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat6 = texture(_MainTex, u_xlat5.xy);
					    u_xlat3 = texture(_MainTex, u_xlat3.zw);
					    u_xlat5 = texture(_MainTex, u_xlat5.zw);
					    u_xlat2.x = _ShaderFlags * 0.5;
					    u_xlatb9 = u_xlat2.x>=(-u_xlat2.x);
					    u_xlat2.x = fract(abs(u_xlat2.x));
					    u_xlat2.x = (u_xlatb9) ? u_xlat2.x : (-u_xlat2.x);
					    u_xlatb2 = u_xlat2.x>=0.5;
					    u_xlat9.x = vs_TEXCOORD6.x + _BevelOffset;
					    u_xlat5.x = u_xlat4.w;
					    u_xlat5.y = u_xlat6.w;
					    u_xlat5.z = u_xlat3.w;
					    u_xlat3 = u_xlat9.xxxx + u_xlat5;
					    u_xlat9.x = _BevelWidth + _OutlineWidth;
					    u_xlat9.x = max(u_xlat9.x, 0.00999999978);
					    u_xlat3 = u_xlat3 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat3 = u_xlat3 / u_xlat9.xxxx;
					    u_xlat3 = u_xlat3 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat3 = clamp(u_xlat3, 0.0, 1.0);
					    u_xlat4 = u_xlat3 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat4 = -abs(u_xlat4) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat3 = (bool(u_xlatb2)) ? u_xlat4 : u_xlat3;
					    u_xlat4 = u_xlat3 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat4 = sin(u_xlat4);
					    u_xlat4 = (-u_xlat3) + u_xlat4;
					    u_xlat3 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat4 + u_xlat3;
					    u_xlat2.x = (-_BevelClamp) + 1.0;
					    u_xlat3 = min(u_xlat2.xxxx, u_xlat3);
					    u_xlat2.x = u_xlat9.x * _Bevel;
					    u_xlat2.x = u_xlat2.x * _GradientScale;
					    u_xlat2.x = u_xlat2.x * -2.0;
					    u_xlat9.xy = u_xlat2.xx * u_xlat3.xz;
					    u_xlat3.yz = u_xlat3.wy * u_xlat2.xx + (-u_xlat9.yx);
					    u_xlat3.x = float(-1.0);
					    u_xlat3.w = float(1.0);
					    u_xlat2.x = dot(u_xlat3.zw, u_xlat3.zw);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat4.yz = u_xlat2.xx * vec2(1.0, 0.0);
					    u_xlat4.x = u_xlat2.x * u_xlat3.z;
					    u_xlat2.x = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat3.z = 0.0;
					    u_xlat2.xyz = u_xlat2.xxx * u_xlat3.xyz;
					    u_xlat3.xyz = u_xlat2.xyz * u_xlat4.xyz;
					    u_xlat2.xyz = u_xlat4.zxy * u_xlat2.yzx + (-u_xlat3.xyz);
					    u_xlat3 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat3.x = u_xlat3.w * u_xlat3.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat24 = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat24 = min(u_xlat24, 1.0);
					    u_xlat24 = (-u_xlat24) + 1.0;
					    u_xlat3.z = sqrt(u_xlat24);
					    u_xlat24 = (-_BumpFace) + _BumpOutline;
					    u_xlat24 = u_xlat1.x * u_xlat24 + _BumpFace;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(u_xlat24) + vec3(-0.0, -0.0, -1.0);
					    u_xlat3.xyz = u_xlat2.www * u_xlat3.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat2.xyz = u_xlat2.xyz + (-u_xlat3.xyz);
					    u_xlat3.x = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat3.x = inversesqrt(u_xlat3.x);
					    u_xlat2.xyz = u_xlat2.xyz * u_xlat3.xxx;
					    u_xlat3.x = (-unused_0_26.y) + unused_0_26.z;
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + unused_0_26.y;
					    u_xlat3.xy = vs_TEXCOORD5.yy * unity_WorldToLight[1].xy;
					    u_xlat3.xy = unity_WorldToLight[0].xy * vs_TEXCOORD5.xx + u_xlat3.xy;
					    u_xlat3.xy = unity_WorldToLight[2].xy * vs_TEXCOORD5.zz + u_xlat3.xy;
					    u_xlat3.xy = u_xlat3.xy + unity_WorldToLight[3].xy;
					    u_xlatb17 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb17){
					        u_xlatb17 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD5.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD5.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD5.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat4.xyz = (bool(u_xlatb17)) ? u_xlat4.xyz : vs_TEXCOORD5.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat4.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat17 = u_xlat4.y * 0.25 + 0.75;
					        u_xlat24 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat24, u_xlat17);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat17 = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlat4 = texture(_LightTexture0, u_xlat3.xy);
					    u_xlat3.x = u_xlat17 * u_xlat4.w;
					    u_xlat4.x = dot(vs_TEXCOORD2.xyz, (-u_xlat2.xyz));
					    u_xlat4.y = dot(vs_TEXCOORD3.xyz, (-u_xlat2.xyz));
					    u_xlat4.z = dot(vs_TEXCOORD4.xyz, (-u_xlat2.xyz));
					    u_xlat2.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat2.xyz = u_xlat2.xxx * u_xlat4.xyz;
					    u_xlat3.xyz = u_xlat3.xxx * _LightColor0.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat0.w = dot(u_xlat2.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat0.x = dot(u_xlat2.xyz, u_xlat0.xyz);
					    u_xlat0.xw = max(u_xlat0.xw, vec2(0.0, 0.0));
					    u_xlat7 = u_xlat1.x * 128.0;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * u_xlat7;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat1.xyz = u_xlat8.xyz * u_xlat3.xyz;
					    u_xlat2.xyz = u_xlat3.xyz * _SpecColor.xyz;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat2.xyz;
					    SV_Target0.xyz = u_xlat1.xyz * u_xlat0.www + u_xlat0.xyz;
					    SV_Target0.w = u_xlat2.w;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "GLOW_ON" "POINT" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _LightColor0;
						vec4 _SpecColor;
						mat4x4 unity_WorldToLight;
						float _FaceUVSpeedX;
						float _FaceUVSpeedY;
						vec4 _FaceColor;
						float _OutlineSoftness;
						float _OutlineUVSpeedX;
						float _OutlineUVSpeedY;
						vec4 _OutlineColor;
						float _OutlineWidth;
						float _Bevel;
						float _BevelOffset;
						float _BevelWidth;
						float _BevelClamp;
						float _BevelRoundness;
						float _BumpOutline;
						float _BumpFace;
						vec4 unused_0_19[12];
						vec4 _GlowColor;
						float _GlowOffset;
						float _GlowOuter;
						float _GlowInner;
						float _GlowPower;
						float _ShaderFlags;
						float _ScaleRatioA;
						float _ScaleRatioB;
						vec4 unused_0_28[3];
						float _TextureWidth;
						float _TextureHeight;
						float _GradientScale;
						vec4 unused_0_32;
						float _FaceShininess;
						float _OutlineShininess;
						vec4 unused_0_35[3];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _FaceTex;
					uniform  sampler2D _OutlineTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec4 vs_TEXCOORD0;
					in  vec2 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD5;
					in  vec4 vs_COLOR0;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					bool u_xlatb2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec3 u_xlat9;
					float u_xlat11;
					vec3 u_xlat12;
					float u_xlat18;
					float u_xlat20;
					float u_xlat27;
					float u_xlat28;
					float u_xlat29;
					bool u_xlatb29;
					float u_xlat30;
					bool u_xlatb30;
					float u_xlat31;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat0.xyz = vec3(u_xlat27) * u_xlat0.xyz;
					    u_xlat1.xyz = (-vs_TEXCOORD5.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat27 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat28 = (-u_xlat2.w) + 0.5;
					    u_xlat28 = u_xlat28 + (-vs_TEXCOORD6.x);
					    u_xlat28 = u_xlat28 * vs_TEXCOORD6.y + 0.5;
					    u_xlat2.x = _OutlineWidth * _ScaleRatioA;
					    u_xlat2.y = _OutlineSoftness * _ScaleRatioA;
					    u_xlat2.xz = u_xlat2.xy * vs_TEXCOORD6.yy;
					    u_xlat3 = vs_COLOR0 * _FaceColor;
					    u_xlat29 = vs_COLOR0.w * _OutlineColor.w;
					    u_xlat4.xy = vec2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + vs_TEXCOORD0.zw;
					    u_xlat4 = texture(_FaceTex, u_xlat4.xy);
					    u_xlat3 = u_xlat3 * u_xlat4;
					    u_xlat4.xy = vec2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + vs_TEXCOORD1.xy;
					    u_xlat4 = texture(_OutlineTex, u_xlat4.xy);
					    u_xlat4.xyz = u_xlat4.xyz * _OutlineColor.xyz;
					    u_xlat5.w = u_xlat29 * u_xlat4.w;
					    u_xlat29 = (-u_xlat2.x) * 0.5 + u_xlat28;
					    u_xlat20 = u_xlat2.z * 0.5 + u_xlat29;
					    u_xlat11 = u_xlat2.y * vs_TEXCOORD6.y + 1.0;
					    u_xlat11 = u_xlat20 / u_xlat11;
					    u_xlat11 = clamp(u_xlat11, 0.0, 1.0);
					    u_xlat11 = (-u_xlat11) + 1.0;
					    u_xlat20 = u_xlat2.x * 0.5 + u_xlat28;
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat2.x = u_xlat2.x * u_xlat20;
					    u_xlat3.xyz = u_xlat3.www * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat4.xyz * u_xlat5.www;
					    u_xlat4 = (-u_xlat3) + u_xlat5;
					    u_xlat3 = u_xlat2.xxxx * u_xlat4 + u_xlat3;
					    u_xlat4 = vec4(u_xlat11) * u_xlat3;
					    u_xlat2.x = max(u_xlat4.w, 9.99999975e-05);
					    u_xlat3.xyz = u_xlat4.xyz / u_xlat2.xxx;
					    u_xlat4.xy = vec2(1.0, 1.0) / vec2(_TextureWidth, _TextureHeight);
					    u_xlat4.z = 0.0;
					    u_xlat5 = (-u_xlat4.xzzy) + vs_TEXCOORD0.xyxy;
					    u_xlat6 = texture(_MainTex, u_xlat5.xy);
					    u_xlat7 = u_xlat4.xzzy + vs_TEXCOORD0.xyxy;
					    u_xlat8 = texture(_MainTex, u_xlat7.xy);
					    u_xlat5 = texture(_MainTex, u_xlat5.zw);
					    u_xlat7 = texture(_MainTex, u_xlat7.zw);
					    u_xlat2.x = _ShaderFlags * 0.5;
					    u_xlatb29 = u_xlat2.x>=(-u_xlat2.x);
					    u_xlat2.x = fract(abs(u_xlat2.x));
					    u_xlat2.x = (u_xlatb29) ? u_xlat2.x : (-u_xlat2.x);
					    u_xlatb2 = u_xlat2.x>=0.5;
					    u_xlat29 = vs_TEXCOORD6.x + _BevelOffset;
					    u_xlat7.x = u_xlat6.w;
					    u_xlat7.y = u_xlat8.w;
					    u_xlat7.z = u_xlat5.w;
					    u_xlat5 = vec4(u_xlat29) + u_xlat7;
					    u_xlat29 = _BevelWidth + _OutlineWidth;
					    u_xlat29 = max(u_xlat29, 0.00999999978);
					    u_xlat5 = u_xlat5 + vec4(-0.5, -0.5, -0.5, -0.5);
					    u_xlat5 = u_xlat5 / vec4(u_xlat29);
					    u_xlat5 = u_xlat5 + vec4(0.5, 0.5, 0.5, 0.5);
					    u_xlat5 = clamp(u_xlat5, 0.0, 1.0);
					    u_xlat6 = u_xlat5 * vec4(2.0, 2.0, 2.0, 2.0) + vec4(-1.0, -1.0, -1.0, -1.0);
					    u_xlat6 = -abs(u_xlat6) + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat5 = (bool(u_xlatb2)) ? u_xlat6 : u_xlat5;
					    u_xlat6 = u_xlat5 * vec4(1.57079601, 1.57079601, 1.57079601, 1.57079601);
					    u_xlat6 = sin(u_xlat6);
					    u_xlat6 = (-u_xlat5) + u_xlat6;
					    u_xlat5 = vec4(vec4(_BevelRoundness, _BevelRoundness, _BevelRoundness, _BevelRoundness)) * u_xlat6 + u_xlat5;
					    u_xlat2.x = (-_BevelClamp) + 1.0;
					    u_xlat5 = min(u_xlat2.xxxx, u_xlat5);
					    u_xlat2.x = u_xlat29 * _Bevel;
					    u_xlat2.x = u_xlat2.x * _GradientScale;
					    u_xlat2.x = u_xlat2.x * -2.0;
					    u_xlat4.xy = u_xlat2.xx * u_xlat5.xz;
					    u_xlat5.yz = u_xlat5.wy * u_xlat2.xx + (-u_xlat4.yx);
					    u_xlat5.x = float(-1.0);
					    u_xlat5.w = float(1.0);
					    u_xlat2.x = dot(u_xlat5.zw, u_xlat5.zw);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat4.yz = u_xlat2.xx * vec2(1.0, 0.0);
					    u_xlat4.x = u_xlat2.x * u_xlat5.z;
					    u_xlat2.x = dot(u_xlat5.xy, u_xlat5.xy);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat5.z = 0.0;
					    u_xlat5.xyz = u_xlat2.xxx * u_xlat5.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat5.xyz;
					    u_xlat4.xyz = u_xlat4.zxy * u_xlat5.yzx + (-u_xlat6.xyz);
					    u_xlat5 = texture(_BumpMap, vs_TEXCOORD0.zw);
					    u_xlat5.x = u_xlat5.w * u_xlat5.x;
					    u_xlat5.xy = u_xlat5.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat2.x = dot(u_xlat5.xy, u_xlat5.xy);
					    u_xlat2.x = min(u_xlat2.x, 1.0);
					    u_xlat2.x = (-u_xlat2.x) + 1.0;
					    u_xlat5.z = sqrt(u_xlat2.x);
					    u_xlat2.x = (-_BumpFace) + _BumpOutline;
					    u_xlat2.x = u_xlat20 * u_xlat2.x + _BumpFace;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat2.xxx + vec3(-0.0, -0.0, -1.0);
					    u_xlat5.xyz = u_xlat4.www * u_xlat5.xyz + vec3(0.0, 0.0, 1.0);
					    u_xlat4.xyz = u_xlat4.xyz + (-u_xlat5.xyz);
					    u_xlat2.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat2.x = inversesqrt(u_xlat2.x);
					    u_xlat4.xyz = u_xlat2.xxx * u_xlat4.xyz;
					    u_xlat2.x = _GlowOffset * _ScaleRatioB;
					    u_xlat2.x = u_xlat2.x * vs_TEXCOORD6.y;
					    u_xlat28 = (-u_xlat2.x) * 0.5 + u_xlat28;
					    u_xlatb2 = u_xlat28>=0.0;
					    u_xlat2.x = u_xlatb2 ? 1.0 : float(0.0);
					    u_xlat29 = _GlowOuter * _ScaleRatioB + (-_GlowInner);
					    u_xlat2.x = u_xlat2.x * u_xlat29 + _GlowInner;
					    u_xlat2.x = u_xlat2.x * vs_TEXCOORD6.y;
					    u_xlat29 = u_xlat2.x * 0.5;
					    u_xlat2.x = u_xlat2.x * 0.5 + 1.0;
					    u_xlat28 = u_xlat28 / u_xlat2.x;
					    u_xlat28 = min(abs(u_xlat28), 1.0);
					    u_xlat28 = log2(u_xlat28);
					    u_xlat28 = u_xlat28 * _GlowPower;
					    u_xlat28 = exp2(u_xlat28);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat2.x = min(u_xlat29, 1.0);
					    u_xlat2.x = sqrt(u_xlat2.x);
					    u_xlat28 = u_xlat28 * u_xlat2.x;
					    u_xlat28 = dot(_GlowColor.ww, vec2(u_xlat28));
					    u_xlat28 = clamp(u_xlat28, 0.0, 1.0);
					    u_xlat2.x = u_xlat28 * vs_COLOR0.w;
					    u_xlat3.xyz = u_xlat4.www * u_xlat3.xyz;
					    u_xlat28 = (-u_xlat28) * vs_COLOR0.w + 1.0;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(u_xlat28);
					    u_xlat3.xyz = _GlowColor.xyz * u_xlat2.xxx + u_xlat3.xyz;
					    u_xlat28 = (-u_xlat3.w) * u_xlat11 + 1.0;
					    u_xlat28 = u_xlat28 * u_xlat2.x + u_xlat4.w;
					    u_xlat2.x = max(u_xlat28, 9.99999975e-05);
					    u_xlat2.xyw = u_xlat3.xyz / u_xlat2.xxx;
					    u_xlat3.x = (-unused_0_32.y) + unused_0_32.z;
					    u_xlat20 = u_xlat20 * u_xlat3.x + unused_0_32.y;
					    u_xlat3.xyz = vs_TEXCOORD5.yyy * unity_WorldToLight[1].xyz;
					    u_xlat3.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD5.xxx + u_xlat3.xyz;
					    u_xlat3.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD5.zzz + u_xlat3.xyz;
					    u_xlat3.xyz = u_xlat3.xyz + unity_WorldToLight[3].xyz;
					    u_xlatb30 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb30){
					        u_xlatb30 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD5.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD5.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD5.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb30)) ? u_xlat5.xyz : vs_TEXCOORD5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat30 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat31 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat30, u_xlat31);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat30 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat30 = clamp(u_xlat30, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat5 = texture(_LightTexture0, u_xlat3.xx);
					    u_xlat3.x = u_xlat30 * u_xlat5.x;
					    u_xlat5.x = dot(vs_TEXCOORD2.xyz, (-u_xlat4.xyz));
					    u_xlat5.y = dot(vs_TEXCOORD3.xyz, (-u_xlat4.xyz));
					    u_xlat5.z = dot(vs_TEXCOORD4.xyz, (-u_xlat4.xyz));
					    u_xlat12.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat12.x = inversesqrt(u_xlat12.x);
					    u_xlat12.xyz = u_xlat12.xxx * u_xlat5.xyz;
					    u_xlat4.xyz = u_xlat3.xxx * _LightColor0.xyz;
					    u_xlat1.xyz = u_xlat1.xyz * vec3(u_xlat27) + u_xlat0.xyz;
					    u_xlat27 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat1.xyz = vec3(u_xlat27) * u_xlat1.xyz;
					    u_xlat0.x = dot(u_xlat12.xyz, u_xlat0.xyz);
					    u_xlat0.y = dot(u_xlat12.xyz, u_xlat1.xyz);
					    u_xlat0.xy = max(u_xlat0.xy, vec2(0.0, 0.0));
					    u_xlat18 = u_xlat20 * 128.0;
					    u_xlat9.x = log2(u_xlat0.y);
					    u_xlat9.x = u_xlat9.x * u_xlat18;
					    u_xlat9.x = exp2(u_xlat9.x);
					    u_xlat1.xyz = u_xlat2.xyw * u_xlat4.xyz;
					    u_xlat2.xyz = u_xlat4.xyz * _SpecColor.xyz;
					    u_xlat9.xyz = u_xlat9.xxx * u_xlat2.xyz;
					    SV_Target0.xyz = u_xlat1.xyz * u_xlat0.xxx + u_xlat9.xyz;
					    SV_Target0.w = u_xlat28;
					    return;
					}"
				}
			}
		}
		Pass {
			Name "Caster"
			LOD 300
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "SHADOWCASTER" "QUEUE" = "Transparent" "RenderType" = "Transparent" "SHADOWSUPPORT" = "true" }
			ColorMask RGB -1
			Cull Off
			Offset 1, 1
			Fog {
				Mode Off
			}
			GpuProgramID 145332
			Program "vp" {
				SubProgram "d3d11 " {
					Keywords { "SHADOWS_DEPTH" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[2];
						vec4 _MainTex_ST;
						vec4 _OutlineTex_ST;
						float _OutlineWidth;
						float _FaceDilate;
						float _ScaleRatioA;
					};
					layout(std140) uniform UnityShadows {
						vec4 unused_1_0[5];
						vec4 unity_LightShadowBias;
						vec4 unused_1_2[20];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						vec4 unused_2_1[7];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD3;
					out float vs_TEXCOORD2;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat4;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat0 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat1.x = unity_LightShadowBias.x / u_xlat0.w;
					    u_xlat1.x = min(u_xlat1.x, 0.0);
					    u_xlat1.x = max(u_xlat1.x, -1.0);
					    u_xlat4 = u_xlat0.z + u_xlat1.x;
					    u_xlat1.x = min(u_xlat0.w, u_xlat4);
					    gl_Position.xyw = u_xlat0.xyw;
					    u_xlat0.x = (-u_xlat4) + u_xlat1.x;
					    gl_Position.z = unity_LightShadowBias.y * u_xlat0.x + u_xlat4;
					    vs_TEXCOORD1.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    vs_TEXCOORD3.xy = in_TEXCOORD0.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    u_xlat0.x = (-_OutlineWidth) * _ScaleRatioA + 1.0;
					    u_xlat0.x = (-_FaceDilate) * _ScaleRatioA + u_xlat0.x;
					    vs_TEXCOORD2 = u_xlat0.x * 0.5;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "SHADOWS_CUBE" }
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
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
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[2];
						vec4 _MainTex_ST;
						vec4 _OutlineTex_ST;
						float _OutlineWidth;
						float _FaceDilate;
						float _ScaleRatioA;
					};
					layout(std140) uniform UnityShadows {
						vec4 unused_1_0[5];
						vec4 unity_LightShadowBias;
						vec4 unused_1_2[20];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						vec4 unused_2_1[7];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD3;
					out float vs_TEXCOORD2;
					vec4 u_xlat0;
					vec4 u_xlat1;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat0 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat1.x = min(u_xlat0.w, u_xlat0.z);
					    u_xlat1.x = (-u_xlat0.z) + u_xlat1.x;
					    gl_Position.z = unity_LightShadowBias.y * u_xlat1.x + u_xlat0.z;
					    gl_Position.xyw = u_xlat0.xyw;
					    vs_TEXCOORD1.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    vs_TEXCOORD3.xy = in_TEXCOORD0.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
					    u_xlat0.x = (-_OutlineWidth) * _ScaleRatioA + 1.0;
					    u_xlat0.x = (-_FaceDilate) * _ScaleRatioA + u_xlat0.x;
					    vs_TEXCOORD2 = u_xlat0.x * 0.5;
					    return;
					}"
				}
			}
			Program "fp" {
				SubProgram "d3d11 " {
					Keywords { "SHADOWS_DEPTH" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
					#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
					#if UNITY_SUPPORTS_UNIFORM_LOCATION
					#define UNITY_LOCATION(x) layout(location = x)
					#define UNITY_BINDING(x) layout(binding = x, std140)
					#else
					#define UNITY_LOCATION(x)
					#define UNITY_BINDING(x) layout(std140)
					#endif
					uniform  sampler2D _MainTex;
					in  vec2 vs_TEXCOORD1;
					in  float vs_TEXCOORD2;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					bool u_xlatb0;
					void main()
					{
					    u_xlat0 = texture(_MainTex, vs_TEXCOORD1.xy);
					    u_xlat0.x = u_xlat0.w + (-vs_TEXCOORD2);
					    u_xlatb0 = u_xlat0.x<0.0;
					    if(((int(u_xlatb0) * int(0xffffffffu)))!=0){discard;}
					    SV_Target0 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "SHADOWS_CUBE" }
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
					#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
					#if UNITY_SUPPORTS_UNIFORM_LOCATION
					#define UNITY_LOCATION(x) layout(location = x)
					#define UNITY_BINDING(x) layout(binding = x, std140)
					#else
					#define UNITY_LOCATION(x)
					#define UNITY_BINDING(x) layout(std140)
					#endif
					uniform  sampler2D _MainTex;
					in  vec2 vs_TEXCOORD1;
					in  float vs_TEXCOORD2;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					bool u_xlatb0;
					void main()
					{
					    u_xlat0 = texture(_MainTex, vs_TEXCOORD1.xy);
					    u_xlat0.x = u_xlat0.w + (-vs_TEXCOORD2);
					    u_xlatb0 = u_xlat0.x<0.0;
					    if(((int(u_xlatb0) * int(0xffffffffu)))!=0){discard;}
					    SV_Target0 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
			}
		}
	}
	CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}