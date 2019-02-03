#shader vertex
#version 330 core
layout(location = 0) in vec4 position;
layout(location = 1) in vec3 color;

uniform mat4 transform;
uniform mat4 projection;
uniform mat4 view;

out vec3 aColor;

void main()
{
    gl_Position = projection * view * transform * vec4(position.xyz, 1.0);
    aColor = color;
}

#shader fragment
#version 330 core

in vec3 aColor;

out vec4 color;

void main()
{
    color = vec4(aColor, 1.0);
}
