#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec3 color;
layout(location = 2) in vec2 texCoord;

out vec3 aColor;
out vec2 aTexCoord;

void main()
{
    gl_Position = position;
    aColor = color;
    aTexCoord = texCoord;
}

#shader fragment
#version 330 core

in vec3 aColor;
in vec2 aTexCoord;

uniform sampler2D tex0;
uniform sampler2D tex1;

out vec4 color;

void main()
{
    color = mix(texture(tex0, aTexCoord) , texture(tex1, aTexCoord), 0.5 ) * vec4(aColor, 1.0);
}