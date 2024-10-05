Shader "Depth Mask/MaskOverlay" {
	Properties {
		_Color ("Main Color", Vector) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	SubShader {
		LOD 100
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			LOD 100
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "Vertex" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Fog {
				Mode Off
			}
			GpuProgramID 2732
			Program "vp" {
				SubProgram "d3d11 " {
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
						vec4 _Color;
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_1_0[7];
						vec4 unity_LightColor[8];
						vec4 unused_1_2[7];
						vec4 unity_LightPosition[8];
						vec4 unused_1_4[32];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 glstate_lightmodel_ambient;
						vec4 unused_3_1[12];
						mat4x4 unity_MatrixInvV;
						mat4x4 unity_MatrixVP;
						vec4 unused_3_4[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					in  vec2 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out vec4 vs_COLOR0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec3 u_xlat2;
					float u_xlat9;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat0 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat0.xyz = unity_WorldToObject[0].xyz * unity_MatrixInvV[0].xyz;
					    u_xlat0.x = dot(u_xlat0.xyz, in_NORMAL0.xyz);
					    u_xlat1.xyz = unity_WorldToObject[1].xyz * unity_MatrixInvV[1].xyz;
					    u_xlat0.y = dot(u_xlat1.xyz, in_NORMAL0.xyz);
					    u_xlat1.xyz = unity_WorldToObject[2].xyz * unity_MatrixInvV[2].xyz;
					    u_xlat0.z = dot(u_xlat1.xyz, in_NORMAL0.xyz);
					    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, unity_LightPosition[0].xyz);
					    u_xlat9 = max(u_xlat9, 0.0);
					    u_xlat1.xyz = vec3(u_xlat9) * _Color.xyz;
					    u_xlat1.xyz = u_xlat1.xyz * unity_LightColor[0].xyz;
					    u_xlat1.xyz = u_xlat1.xyz * vec3(0.5, 0.5, 0.5);
					    u_xlat1.xyz = min(u_xlat1.xyz, vec3(1.0, 1.0, 1.0));
					    u_xlat1.xyz = _Color.xyz * glstate_lightmodel_ambient.xyz + u_xlat1.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, unity_LightPosition[1].xyz);
					    u_xlat9 = max(u_xlat9, 0.0);
					    u_xlat2.xyz = vec3(u_xlat9) * _Color.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * unity_LightColor[1].xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.5, 0.5, 0.5);
					    u_xlat2.xyz = min(u_xlat2.xyz, vec3(1.0, 1.0, 1.0));
					    u_xlat1.xyz = u_xlat1.xyz + u_xlat2.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, unity_LightPosition[2].xyz);
					    u_xlat9 = max(u_xlat9, 0.0);
					    u_xlat2.xyz = vec3(u_xlat9) * _Color.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * unity_LightColor[2].xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.5, 0.5, 0.5);
					    u_xlat2.xyz = min(u_xlat2.xyz, vec3(1.0, 1.0, 1.0));
					    u_xlat1.xyz = u_xlat1.xyz + u_xlat2.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, unity_LightPosition[3].xyz);
					    u_xlat9 = max(u_xlat9, 0.0);
					    u_xlat2.xyz = vec3(u_xlat9) * _Color.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * unity_LightColor[3].xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.5, 0.5, 0.5);
					    u_xlat2.xyz = min(u_xlat2.xyz, vec3(1.0, 1.0, 1.0));
					    u_xlat1.xyz = u_xlat1.xyz + u_xlat2.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, unity_LightPosition[4].xyz);
					    u_xlat9 = max(u_xlat9, 0.0);
					    u_xlat2.xyz = vec3(u_xlat9) * _Color.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * unity_LightColor[4].xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.5, 0.5, 0.5);
					    u_xlat2.xyz = min(u_xlat2.xyz, vec3(1.0, 1.0, 1.0));
					    u_xlat1.xyz = u_xlat1.xyz + u_xlat2.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, unity_LightPosition[5].xyz);
					    u_xlat9 = max(u_xlat9, 0.0);
					    u_xlat2.xyz = vec3(u_xlat9) * _Color.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * unity_LightColor[5].xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.5, 0.5, 0.5);
					    u_xlat2.xyz = min(u_xlat2.xyz, vec3(1.0, 1.0, 1.0));
					    u_xlat1.xyz = u_xlat1.xyz + u_xlat2.xyz;
					    u_xlat0.w = dot(u_xlat0.xyz, unity_LightPosition[6].xyz);
					    u_xlat0.x = dot(u_xlat0.xyz, unity_LightPosition[7].xyz);
					    u_xlat0.xw = max(u_xlat0.xw, vec2(0.0, 0.0));
					    u_xlat0.xyz = u_xlat0.xxx * _Color.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * unity_LightColor[7].xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(0.5, 0.5, 0.5);
					    u_xlat0.xyz = min(u_xlat0.xyz, vec3(1.0, 1.0, 1.0));
					    u_xlat2.xyz = u_xlat0.www * _Color.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * unity_LightColor[6].xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.5, 0.5, 0.5);
					    u_xlat2.xyz = min(u_xlat2.xyz, vec3(1.0, 1.0, 1.0));
					    u_xlat1.xyz = u_xlat1.xyz + u_xlat2.xyz;
					    vs_COLOR0.xyz = u_xlat0.xyz + u_xlat1.xyz;
					    vs_COLOR0.xyz = clamp(vs_COLOR0.xyz, 0.0, 1.0);
					    vs_COLOR0.w = _Color.w;
					    vs_COLOR0.w = clamp(vs_COLOR0.w, 0.0, 1.0);
					    return;
					}"
				}
			}
			Program "fp" {
				SubProgram "d3d11 " {
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
					in  vec2 vs_TEXCOORD0;
					in  vec4 vs_COLOR0;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					void main()
					{
					    u_xlat0 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat0.xyz = u_xlat0.xyz * vs_COLOR0.xyz;
					    SV_Target0.w = u_xlat0.w * vs_COLOR0.w;
					    SV_Target0.xyz = u_xlat0.xyz + u_xlat0.xyz;
					    return;
					}"
				}
			}
		}
	}
}