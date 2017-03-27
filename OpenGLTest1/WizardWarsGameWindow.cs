using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace OpenGLTest1
{
    class WizardWarsGameWindow : OpenTK.GameWindow
    {
        public WizardWarsGameWindow() : base(800, 600, GraphicsMode.Default, "WizardWars 0.0.1")
        {
            VSync = VSyncMode.On;
        }

        int textureIdHouse = 0;
        int textureIdCastle = 0;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            textureIdHouse = loadImage("G:\\Archive\\Pictures\\2D\\Tilesets\\King\\House32x32.png");
            textureIdCastle = loadImage("G:\\Archive\\Pictures\\2D\\Tilesets\\King\\castle32x32.png");

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (Keyboard[Key.Escape])
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(new Vector3(0, 0, 30), Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, textureIdCastle);
            GL.Begin(BeginMode.Quads);

            GL.Color3(1.0f, 1.0f, 1.0f); GL.TexCoord2(0, 1); GL.Vertex3(-1.0f, -1.0f, 4.0f); 
            GL.Color3(1.0f, 1.0f, 1.0f); GL.TexCoord2(1, 1); GL.Vertex3(1.0f, -1.0f, 4.0f);
            GL.Color3(1.0f, 1.0f, 1.0f); GL.TexCoord2(1, 0); GL.Vertex3(1.0f, 1.0f, 4.0f);
            GL.Color3(1.0f, 1.0f, 1.0f); GL.TexCoord2(0, 0); GL.Vertex3(-1.0f, 1.0f, 4.0f);

            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, textureIdHouse);
            GL.Begin(BeginMode.Quads);

            for (int i = 10; i > 0; i-=2)
            {
                GL.Color3(1.0f, 1.0f, 1.0f); GL.TexCoord2(0, 1); GL.Vertex3((-i), -1.0f, 4.0f);
                GL.Color3(1.0f, 1.0f, 1.0f); GL.TexCoord2(1, 1); GL.Vertex3((-i)+2, -1.0f, 4.0f);
                GL.Color3(1.0f, 1.0f, 1.0f); GL.TexCoord2(1, 0); GL.Vertex3((-i)+2, 1.0f, 4.0f);
                GL.Color3(1.0f, 1.0f, 1.0f); GL.TexCoord2(0, 0); GL.Vertex3((-i), 1.0f, 4.0f);
            }

            GL.End();

            

            

            SwapBuffers();
        }

        private int loadImage(Bitmap image)
        {
            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);
            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }

        private int loadImage(string filename)
        {
            try
            {
                Image file = Image.FromFile(filename);
                return loadImage(new Bitmap(file));
            }
            catch (FileNotFoundException e)
            {
                return -1;
            }
        }

    }
}
