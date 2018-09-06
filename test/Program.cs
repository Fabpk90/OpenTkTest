using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using OpenTK.Input;

namespace test
{
    class Program : GameWindow
    {
        private float[] vertices =
        {
            -0.5f, -0.5f,
            0.0f, 0.5f,
            0.5f, -0.5f,
        };

        private int buffer;
        private int prog;

        private string shaderVertex;
        private string shaderFragment;

        public Program() : base(1280, // initial width
            720, // initial height
            GraphicsMode.Default,
            "Test OpenTk", // initial title
            GameWindowFlags.Default,
            DisplayDevice.Default,
            4, // OpenGL major version
            0, // OpenGL minor version
            GraphicsContextFlags.ForwardCompatible)
        {
        }


        [STAThread]
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void ParseShader(string path, out string shaderVertex, out string shaderFragment)
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CursorVisible = true;

            GL.GenBuffers(1, out buffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, sizeof(float) * 2, 0);

            prog = GL.CreateProgram();

            ParseShader("Shaders/basic.shader", out shaderVertex, out shaderFragment);
            
            //compile shader
            int vs = CompileShader(shaderVertex, ShaderType.VertexShader);
            int fs = CompileShader(shaderFragment, ShaderType.FragmentShader);

            GL.AttachShader(prog, vs);
            GL.AttachShader(prog, fs);


            GL.LinkProgram(prog);
            GL.ValidateProgram(prog);

            //clean intermediates
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);
            
            GL.UseProgram(prog);
        }

        private int CompileShader(string source, ShaderType type)
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

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
                Exit();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            GL.DeleteProgram(prog);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Title = $"FPS: {1f / e.Time:0}";

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            SwapBuffers();
        }
    }
}