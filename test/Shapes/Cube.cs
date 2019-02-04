using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace test.Shapes
{
    public class Cube
    {
        private int vao;
        private int vbo;
        private int ebo;

        private int texture0;
        private int texture1;

        private int shaderProgram;

        private Bitmap textureLoader;
        
        
        private float[] vertex;
        private uint[] index;

        private Matrix4 transform;

        private Color4 color;

        public Cube(string texturePath)
        {
            index = new uint[]
            {
                // note that we start from 0!
                0, 1, 2,
                2, 3, 0,
                4, 5, 6,
                6, 7, 4,
                0, 4, 1,
                1, 5, 4,
                2, 6, 3,
                3, 7, 6,
                0, 4, 3,
                3, 7, 4,
                2, 6, 5,
                2, 1, 5
            };

            vertex = new float[]
            {
                //pos           //color
                0.5f, 0.5f, 0.5f, .9f, .8f, .7f, 1.0f, 1.0f, // top right
                0.5f, -0.5f, 0.5f, .5f, .6f, .4f, 1.0f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.5f, .45f, .2f, .3f, 0.0f, 0.0f, // bottom left
                -0.5f, 0.5f, 0.5f, .58f, .45f, .69f, 0.0f, 1.0f, // top left
                0.5f, 0.5f, -0.5f, .9f, .8f, .7f, 1.0f, 1.0f,
                0.5f, -0.5f, -0.5f, .45f, .2f, .3f, 1.0f, 0.0f,
                -0.5f, -0.5f, -0.5f, .5f, .6f, .4f, 0.0f, 0.0f,
                -0.5f, 0.5f, -0.5f, .58f, .45f, .69f, 0.0f, 1.0f,
            };
            
            transform = Matrix4.Identity;

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertex.Length * sizeof(float), vertex,
                BufferUsageHint.StaticDraw);

            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * index.Length, index, BufferUsageHint.StaticDraw);
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexArrayAttrib(vao, 0);
            
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexArrayAttrib(vao, 1);
            
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, true, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexArrayAttrib(vao, 2);

            shaderProgram = ShaderHandler.CreateShader("Shaders/cube.glsl");
            GL.UseProgram(shaderProgram);
            
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "projection"),false, ref Program.projection);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.MirroredRepeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.MirroredRepeat);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
            
            GL.GenTextures(1, out texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture0);
            
            textureLoader = new Bitmap(texturePath);
            BitmapData data = textureLoader.LockBits(new System.Drawing.Rectangle(0, 0, textureLoader.Width, textureLoader.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, textureLoader.Width, textureLoader.Height, 0,
                PixelFormat.Rgb, PixelType.UnsignedByte, data.Scan0);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "tex0"), 0);
            textureLoader.UnlockBits(data);
            textureLoader.Dispose();
            
           
            
            GL.GenTextures(1, out texture1);
            GL.BindTexture(TextureTarget.Texture2D, texture1);
            
            textureLoader = new Bitmap("awesomeface.png");
            textureLoader.RotateFlip(RotateFlipType.Rotate90FlipY);
            data = textureLoader.LockBits(new System.Drawing.Rectangle(0, 0, textureLoader.Width, textureLoader.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textureLoader.Width, textureLoader.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "tex1"), 1);
            textureLoader.UnlockBits(data);
            textureLoader.Dispose();
        }

        public void Draw(float deltaTime)
        {
            
            transform *= Matrix4.CreateRotationY(deltaTime);
            transform *= Matrix4.CreateRotationX(deltaTime);
            
            GL.UseProgram(shaderProgram);
            GL.BindVertexArray(vao);

            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "transform"), false, ref transform);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "view"), false, ref Program.view);
            
            GL.DrawElements(PrimitiveType.Triangles, index.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Destroy()
        {
            GL.DeleteProgram(shaderProgram);
            
            GL.DeleteVertexArray(vao);
            
            GL.DeleteBuffer(ebo);
            GL.DeleteBuffer(vbo);
            
            GL.DeleteTexture(texture0);
            GL.DeleteTexture(texture1);
        }
    }
}