Shader "Unlit/Cyber" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Red ("洋红-红色-橙色", Range(-1, 0.5)) = 0
		_Orange ("红色-橙色-黄色", Range(-0.5, 0.5)) = 0
		_Yellow ("橙色-黄色-绿色", Range(-0.5, 1)) = 0
		_Green ("黄色-绿色-靛色", Range(-1, 1)) = 0
		_Cyan ("绿色-靛色-蓝色", Range(-1, 1)) = 0
		_Blue ("靛色-蓝色-紫色", Range(-1, 0.5)) = 0
		_Purple ("蓝色-紫色-洋红", Range(-0.5, 0.5)) = 0
		_Magenta ("紫色-洋红-红色", Range(-0.5, 1)) = 0
		_Hue ("Hue", Float) = 0
		_Saturation ("Saturation", Float) = 1
		_Value ("Value", Float) = 1
	}
	SubShader {
		LOD 100
		Tags { "QUEUE" = "Transparent" }
		GrabPass {
		}
		Pass {
			LOD 100
			Tags { "QUEUE" = "Transparent" }
			GpuProgramID 33141
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
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						vec4 unused_0_1[7];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_1_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_1_2[2];
					};
					in  vec4 in_POSITION0;
					out vec4 vs_TEXCOORD0;
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
					    u_xlat1.xyz = u_xlat0.xwy * vec3(0.5, 0.5, -0.5);
					    vs_TEXCOORD0.xy = u_xlat1.yy + u_xlat1.xz;
					    vs_TEXCOORD0.zw = u_xlat0.zw;
					    gl_Position = u_xlat0;
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
						float _Red;
						float _Orange;
						float _Yellow;
						float _Green;
						float _Cyan;
						float _Blue;
						float _Purple;
						float _Magenta;
						float _Hue;
						float _Saturation;
						float _Value;
					};
					uniform  sampler2D _GrabTexture;
					in  vec4 vs_TEXCOORD0;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					bool u_xlatb1;
					vec4 u_xlat2;
					bool u_xlatb2;
					vec4 u_xlat3;
					bvec4 u_xlatb3;
					vec4 u_xlat4;
					float u_xlat5;
					bool u_xlatb5;
					float u_xlat6;
					bool u_xlatb6;
					bool u_xlatb7;
					float u_xlat10;
					bool u_xlatb10;
					vec2 u_xlat11;
					bool u_xlatb11;
					float u_xlat12;
					float u_xlat16;
					float u_xlat17;
					void main()
					{
					    u_xlat0.xy = vs_TEXCOORD0.xy / vs_TEXCOORD0.ww;
					    u_xlat0 = texture(_GrabTexture, u_xlat0.xy);
					    u_xlatb1 = u_xlat0.y>=u_xlat0.z;
					    u_xlat1.x = u_xlatb1 ? 1.0 : float(0.0);
					    u_xlat2.xy = u_xlat0.zy;
					    u_xlat2.z = float(-1.0);
					    u_xlat2.w = float(0.666666687);
					    u_xlat3.xy = u_xlat0.yz + (-u_xlat2.xy);
					    u_xlat3.z = float(1.0);
					    u_xlat3.w = float(-1.0);
					    u_xlat1 = u_xlat1.xxxx * u_xlat3 + u_xlat2;
					    u_xlatb5 = u_xlat0.x>=u_xlat1.x;
					    u_xlat5 = u_xlatb5 ? 1.0 : float(0.0);
					    u_xlat2.xyz = u_xlat1.xyw;
					    u_xlat2.w = u_xlat0.x;
					    u_xlat1.xyw = u_xlat2.wyx;
					    u_xlat1 = (-u_xlat2) + u_xlat1;
					    u_xlat1 = vec4(u_xlat5) * u_xlat1 + u_xlat2;
					    u_xlat0.x = min(u_xlat1.y, u_xlat1.w);
					    u_xlat0.x = (-u_xlat0.x) + u_xlat1.x;
					    u_xlat5 = (-u_xlat1.y) + u_xlat1.w;
					    u_xlat10 = u_xlat0.x * 6.0 + 1.00000001e-10;
					    u_xlat5 = u_xlat5 / u_xlat10;
					    u_xlat5 = u_xlat5 + u_xlat1.z;
					    u_xlat10 = u_xlat1.x + 1.00000001e-10;
					    u_xlat0.x = u_xlat0.x / u_xlat10;
					    u_xlat10 = _Red * 0.166666701;
					    u_xlat6 = _Magenta * 0.166666701 + 0.833333313;
					    u_xlatb11 = 0.833333313<abs(u_xlat5);
					    if(u_xlatb11){
					        u_xlat11.x = _Red * 0.166666701 + 1.0;
					        u_xlat16 = abs(u_xlat5) + -0.833333313;
					        u_xlat16 = u_xlat16 * 5.99999952;
					        u_xlat11.x = (-u_xlat6) + u_xlat11.x;
					        u_xlat11.x = u_xlat16 * u_xlat11.x + u_xlat6;
					    } else {
					        u_xlat16 = _Purple * 0.166666701 + 0.75;
					        u_xlatb2 = 0.75<abs(u_xlat5);
					        if(u_xlatb2){
					            u_xlat2.x = abs(u_xlat5) + -0.75;
					            u_xlat2.x = u_xlat2.x * 12.0000029;
					            u_xlat6 = (-u_xlat16) + u_xlat6;
					            u_xlat11.x = u_xlat2.x * u_xlat6 + u_xlat16;
					        } else {
					            u_xlat2.xyz = vec3(_Green, _Yellow, _Orange) * vec3(0.166666701, 0.166666701, 0.166666701) + vec3(0.333333313, 0.166666701, 0.0833332986);
					            u_xlat3.xy = vec2(_Blue, _Cyan) * vec2(0.166666701, 0.166666701) + vec2(0.666666687, 0.5);
					            u_xlat4 = abs(vec4(u_xlat5)) + vec4(-0.666666687, -0.5, -0.333333313, -0.166666701);
					            u_xlat4 = u_xlat4 * vec4(12.0000029, 5.99999952, 5.99999952, 6.00000191);
					            u_xlat6 = u_xlat16 + (-u_xlat3.x);
					            u_xlat6 = u_xlat4.x * u_xlat6 + u_xlat3.x;
					            u_xlat16 = (-u_xlat3.y) + u_xlat3.x;
					            u_xlat16 = u_xlat4.y * u_xlat16 + u_xlat3.y;
					            u_xlat17 = (-u_xlat2.x) + u_xlat3.y;
					            u_xlat17 = u_xlat4.z * u_xlat17 + u_xlat2.x;
					            u_xlatb3 = lessThan(vec4(0.666666687, 0.5, 0.333333313, 0.166666701), abs(vec4(u_xlat5)));
					            u_xlat4.xy = (-u_xlat2.yz) + u_xlat2.xy;
					            u_xlat2.x = u_xlat4.w * u_xlat4.x + u_xlat2.y;
					            u_xlatb7 = 0.0833332986<abs(u_xlat5);
					            u_xlat4.x = abs(u_xlat5) + -0.0833332986;
					            u_xlat4.x = u_xlat4.x * 11.9999905;
					            u_xlat4.x = u_xlat4.x * u_xlat4.y + u_xlat2.z;
					            u_xlat5 = abs(u_xlat5) * 12.0000048;
					            u_xlat12 = (-_Red) * 0.166666701 + u_xlat2.z;
					            u_xlat5 = u_xlat5 * u_xlat12 + u_xlat10;
					            u_xlat5 = (u_xlatb7) ? u_xlat4.x : u_xlat5;
					            u_xlat5 = (u_xlatb3.w) ? u_xlat2.x : u_xlat5;
					            u_xlat5 = (u_xlatb3.z) ? u_xlat17 : u_xlat5;
					            u_xlat5 = (u_xlatb3.y) ? u_xlat16 : u_xlat5;
					            u_xlat11.x = (u_xlatb3.x) ? u_xlat6 : u_xlat5;
					        }
					    }
					    u_xlat5 = u_xlat11.x + _Hue;
					    u_xlatb10 = 1.0>=u_xlat5;
					    u_xlatb6 = u_xlat5<0.0;
					    u_xlat11.xy = vec2(u_xlat5) + vec2(-1.0, 1.0);
					    u_xlat5 = (u_xlatb6) ? u_xlat11.y : u_xlat5;
					    u_xlat5 = (u_xlatb10) ? u_xlat5 : u_xlat11.x;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * _Saturation;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat10 = log2(u_xlat1.x);
					    u_xlat10 = u_xlat10 * _Value;
					    u_xlat10 = exp2(u_xlat10);
					    u_xlat1.xyz = vec3(u_xlat5) + vec3(1.0, 0.666666687, 0.333333343);
					    u_xlat1.xyz = fract(u_xlat1.xyz);
					    u_xlat1.xyz = u_xlat1.xyz * vec3(6.0, 6.0, 6.0) + vec3(-3.0, -3.0, -3.0);
					    u_xlat1.xyz = abs(u_xlat1.xyz) + vec3(-1.0, -1.0, -1.0);
					    u_xlat1.xyz = clamp(u_xlat1.xyz, 0.0, 1.0);
					    u_xlat1.xyz = u_xlat1.xyz + vec3(-1.0, -1.0, -1.0);
					    u_xlat1.xyz = u_xlat0.xxx * u_xlat1.xyz + vec3(1.0, 1.0, 1.0);
					    SV_Target0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    SV_Target0.w = u_xlat0.w;
					    return;
					}"
				}
			}
		}
	}
}