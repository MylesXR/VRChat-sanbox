// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TsunaMoo/DJ FX"
{
	Properties
	{
		[HideInInspector]shader_is_using_thry_editor("", Float) = 0
		[HideInInspector]shader_master_label("<color=#ffffffff>Tsuna</color> <color=#000000ff>Moo</color> <color=#ffffffff>Shader</color> <color=#000000ff>Lab</color>--{texture:{name:tsuna_moo_icon,height:128}}", Float) = 0
		[HideInInspector]shader_presets("TsunaMooShaders", Float) = 0
		[HideInInspector]shader_properties_label_file("TsunaMooLabels", Float) = 0
		[Enum(Off,0,Front,1,Back,2)]_Cull("Cull", Float) = 2
		[HideInInspector]LightmapFlags("LightmapFlags", Float) = 2
		[HideInInspector]DSGI("DSGI", Float) = 0
		[HideInInspector]Instancing("Instancing", Float) = 0
		[HideInInspector]m_Main("Main", Float) = 0
		[NoScaleOffset][SingleLineTexture]_EmissionMask("Emission Mask", 2D) = "white" {}
		[SingleLineTexture]_ShiftMap("Shift Map--{reference_property:_EmissionColor}", 2D) = "white" {}
		_Speed("Hue Speed", Range( 0 , 1)) = 0
		[HideInInspector][HDR]_EmissionColor("EmissionColor", Color) = (1,1,1,0)
		[HideInInspector]footer_discord("", Float) = 0
		[HideInInspector]footer_patreon("", Float) = 0
		[HideInInspector]footer_booth("", Float) = 0
		[HideInInspector]footer_github("", Float) = 0
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull [_Cull]
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float2 uv3_texcoord3;
		};

		uniform float shader_presets;
		uniform float footer_patreon;
		uniform float footer_github;
		uniform float shader_is_using_thry_editor;
		uniform float shader_master_label;
		uniform float shader_properties_label_file;
		uniform float Instancing;
		uniform float footer_discord;
		uniform float LightmapFlags;
		uniform float _Cull;
		uniform float DSGI;
		uniform float footer_booth;
		uniform float m_Main;
		uniform sampler2D _EmissionMask;
		uniform sampler2D _ShiftMap;
		uniform float4 _EmissionColor;
		uniform float _Speed;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_EmissionMask64 = i.uv_texcoord;
			float3 hsvTorgb176 = RGBToHSV( ( tex2D( _ShiftMap, i.uv3_texcoord3 ) * _EmissionColor ).rgb );
			float mulTime205 = _Time.y * _Speed;
			float3 hsvTorgb166 = HSVToRGB( float3(( hsvTorgb176.x + mulTime205 ),hsvTorgb176.y,hsvTorgb176.z) );
			float4 temp_output_195_0 = ( tex2D( _EmissionMask, uv_EmissionMask64 ) * float4( hsvTorgb166 , 0.0 ) );
			o.Albedo = temp_output_195_0.rgb;
			o.Emission = ( temp_output_195_0 * (0.0 + (sin( mulTime205 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "Thry.ShaderEditor"
}
/*ASEBEGIN
Version=18921
529.6;340.8;1736;835;1460.937;244.2901;1.329677;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;112;-1071.188,360.5728;Inherit;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;138;-668.3048,445.8192;Inherit;False;Property;_EmissionColor;EmissionColor;12;2;[HideInInspector];[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;115;-755.907,235.4712;Inherit;True;Property;_ShiftMap;Shift Map--{reference_property:_EmissionColor};10;1;[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;206;-323.8478,518.2198;Inherit;False;Property;_Speed;Hue Speed;11;0;Create;False;0;0;0;False;0;False;0;0.383;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;180;-329.7381,205.2515;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RGBToHSVNode;176;30.78112,303.4906;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;205;53.15222,529.2198;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;152;415.681,394.7906;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;207;358.4497,668.8522;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;166;576.1032,507.4529;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;64;309.706,190.7074;Inherit;True;Property;_EmissionMask;Emission Mask;9;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;91d63e26b1d6e7b4fbbd489cc68c6750;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;195;906.3861,386.8428;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;208;928.4497,640.8522;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;224;412.171,999.0441;Inherit;False;741.2661;134.4619;;4;228;227;226;225;Settings;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;209;-774.6908,871.9523;Inherit;False;1098.176;441.6777;;9;223;222;219;218;217;216;215;212;211;ThryEditor;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;222;-28.41174,926.6533;Inherit;False;Property;m_Main;Main;8;1;[HideInInspector];Create;False;0;0;0;True;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;217;-738.4337,1156.733;Inherit;False;Property;footer_booth;;15;1;[HideInInspector];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;225;449.9904,1041.337;Inherit;False;Property;DSGI;DSGI;6;1;[HideInInspector];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;227;982.0604,1046.368;Inherit;False;Property;_Cull;Cull;4;1;[Enum];Create;False;0;3;Off;0;Front;1;Back;2;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;228;794.1719,1044.044;Inherit;False;Property;LightmapFlags;LightmapFlags;5;1;[HideInInspector];Create;False;0;0;0;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;219;-556.9443,933.5992;Inherit;False;Property;shader_master_label;<color=#ffffffff>Tsuna</color> <color=#000000ff>Moo</color> <color=#ffffffff>Shader</color> <color=#000000ff>Lab</color>--{texture:{name:tsuna_moo_icon,height:128}};1;1;[HideInInspector];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;226;617.171,1044.044;Inherit;False;Property;Instancing;Instancing;7;1;[HideInInspector];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;1235.399,504.6888;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;218;-474.0565,1034.614;Inherit;False;Property;shader_properties_label_file;TsunaMooLabels;3;1;[HideInInspector];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;212;-732.2867,934.3113;Inherit;False;Property;shader_is_using_thry_editor;;0;1;[HideInInspector];Create;False;0;0;0;True;0;False;0;69;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;216;-586.5783,1158.367;Inherit;False;Property;footer_patreon;;14;1;[HideInInspector];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;211;-735.9497,1032.483;Inherit;False;Property;shader_presets;TsunaMooShaders;2;1;[HideInInspector];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;223;-240.9865,1150.414;Inherit;False;Property;footer_discord;;13;1;[HideInInspector];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;215;-418.6273,1155.651;Inherit;False;Property;footer_github;;16;1;[HideInInspector];Create;False;0;0;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;203;1827.321,386.5184;Float;False;True;-1;2;Thry.ShaderEditor;0;0;Standard;TsunaMoo/DJ FX;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;227;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;115;1;112;0
WireConnection;180;0;115;0
WireConnection;180;1;138;0
WireConnection;176;0;180;0
WireConnection;205;0;206;0
WireConnection;152;0;176;1
WireConnection;152;1;205;0
WireConnection;207;0;205;0
WireConnection;166;0;152;0
WireConnection;166;1;176;2
WireConnection;166;2;176;3
WireConnection;195;0;64;0
WireConnection;195;1;166;0
WireConnection;208;0;207;0
WireConnection;155;0;195;0
WireConnection;155;1;208;0
WireConnection;203;0;195;0
WireConnection;203;2;155;0
ASEEND*/
//CHKSM=D7B60D49352E34587EF3F741E04DF58607ED0990