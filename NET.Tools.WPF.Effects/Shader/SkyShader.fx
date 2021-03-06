sampler2D implicitInput : register(s0);
float alpha : register(c0);
float invert : register(c1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(implicitInput, uv);
	color.rgb = color.rgb * alpha;

	float gray = (color.r + color.g + color.b) / 3.0f;

	if (!invert)
		color.a = gray;
	else
		color.a = 1 - gray;

    return color;
}