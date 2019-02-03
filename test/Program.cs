using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using OpenTK.Input;
using test.Shapes;

namespace test
{
    class Program : GameWindow
    {

        public static Matrix4 view;
        public static Matrix4 projection;
        
        private Cube cube;

        public Program() : base(1280, // initial width
            720, // initial height
            GraphicsMode.Default,
            "Test OpenTk", // initial title
            GameWindowFlags.Default,
            DisplayDevice.Default,
            3, // OpenGL major version
            3, // OpenGL minor version
            GraphicsContextFlags.ForwardCompatible)
        {}


        [STAThread]
        static void Main(string[] args)
        {
            new Program().Run( 0, 75f);
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

            projection = Matrix4.CreatePerspectiveFieldOfView(OpenTK.MathHelper.DegreesToRadians(55.0f), Width / Height,
                0.1f, 100f);
            view = Matrix4.LookAt(new Vector3(0, 0, 5f), new Vector3(0, 0, -1), new Vector3(0, 1, 0));
            
            cube = new Cube();
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

            cube.Destroy();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            
            Title = $"FPS: {1f / e.Time:0}";

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            cube.Draw((float)e.Time);
            

            SwapBuffers();
        }
    }
}