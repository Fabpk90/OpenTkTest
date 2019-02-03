using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace test
{
    public static class ShaderHandler
    {

        public static int CreateShader(string path)
        {
            string fragmentShader, vertexShader;

            int shaderProgram = GL.CreateProgram();
            
            ParseShader(path, out vertexShader, out fragmentShader);

            int fs = CompileShader(fragmentShader, ShaderType.FragmentShader);
            int vs = CompileShader(vertexShader, ShaderType.VertexShader);
            
            GL.AttachShader(shaderProgram, fs);
            GL.AttachShader(shaderProgram, vs);

            
            GL.LinkProgram(shaderProgram);
            GL.ValidateProgram(shaderProgram);

            //clean intermediates
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);

            return shaderProgram;
        }
        public static void ParseShader(string path, out string shaderVertex, out string shaderFragment)
        {
            StreamReader r = new StreamReader(path);

            string[] buffers = new string[2];
            int typeParsing = -1; //0 == vertex 1 == fragment

            while (!r.EndOfStream)
            {
                var line = r.ReadLine();
                if (line.Contains("#shader"))
                {
                    if (line.Contains("vertex"))
                    {
                        typeParsing = 0;
                    }
                    else if (line.Contains("fragment"))
                    {
                        typeParsing = 1;
                    }
                }
                else
                {
                    buffers[typeParsing] += line + "\n";
                }
            }

            shaderVertex = buffers[0];
            shaderFragment = buffers[1];
        }
        
        public static int CompileShader(string source, ShaderType type)
        {
            int id = GL.CreateShader(type);

            GL.ShaderSource(id, source);
            GL.CompileShader(id);

            string log = "";
            GL.GetShaderInfoLog(id, out log);

            if (log != "")
            {
                Console.WriteLine(log);
                GL.DeleteShader(id);
            }

            return id;
        }
    }
}