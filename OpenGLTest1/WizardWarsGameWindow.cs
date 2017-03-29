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

        enum HexTriangleType
        {
            Top, 
            TopRight, 
            TopLeft, 
            BotLeft, 
            BotRight, 
            Bot
        }


        public WizardWarsGameWindow() : base(800, 600, GraphicsMode.Default, "WizardWars 0.0.1")
        {
            VSync = VSyncMode.On;
        }

        int textureIdHouse = 0;
        int textureIdCastle = 0;

        float outerRadius()
        {
            return 1;
            
        }

        float innerRadius()
        {
            return (float)Math.Sqrt(3) / 2 * outerRadius();
        }

        Vector3[] getHexVectors()
        {
            Vector3[] vecs = new Vector3[7];

            vecs[0] =new Vector3(0, 0, 0);
            vecs[1] = new Vector3(0.5f * outerRadius(), innerRadius(), 0);
            vecs[2] = new Vector3(-0.5f * outerRadius(), innerRadius(), 0);
            vecs[3] = new Vector3(-outerRadius(), 0, 0);
            vecs[4] = new Vector3(-0.5f * outerRadius(), -innerRadius(), 0);
            vecs[5] = new Vector3(0.5f * outerRadius(), -innerRadius(), 0);
            vecs[6] = new Vector3(outerRadius(), 0, 0);
            
            return vecs;
        }

        int[] getHexTriangleIndices(HexTriangleType hexTriangleType)
        {
            switch (hexTriangleType)
            {
                case HexTriangleType.Top: return new int[3]{ 0, 1, 2};
                case HexTriangleType.TopRight: return new int[3] { 0, 6, 1 };
                case HexTriangleType.TopLeft: return new int[3] { 0, 2, 3 };
                case HexTriangleType.BotLeft: return new int[3] { 0, 3, 4 };
                case HexTriangleType.Bot: return new int[3] { 0, 4, 5 };
                case HexTriangleType.BotRight: return new int[3] { 0, 5, 6 };
                default: return null;
            }
        }
        
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

        private void drawTexturedQuads()
        {
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

            for (int i = 10; i > 0; i -= 2)
            {
                GL.Color3(1.0f, 1.0f, 1.0f); GL.TexCoord2(0, 1); GL.Vertex3((-i), -1.0f, 4.0f);
                GL.Color3(1.0f, 1.0f, 1.0f); GL.TexCoord2(1, 1); GL.Vertex3((-i) + 2, -1.0f, 4.0f);
                GL.Color3(1.0f, 1.0f, 1.0f); GL.TexCoord2(1, 0); GL.Vertex3((-i) + 2, 1.0f, 4.0f);
                GL.Color3(1.0f, 1.0f, 1.0f); GL.TexCoord2(0, 0); GL.Vertex3((-i), 1.0f, 4.0f);
            }

            GL.End();
        }

        protected void drawHex(Vector3 position, Vector3 color)
        {
            Vector3[] hexVectors = getHexVectors();
            int[] top = getHexTriangleIndices(HexTriangleType.Top);
            int[] topRight = getHexTriangleIndices(HexTriangleType.TopRight);
            int[] topLeft = getHexTriangleIndices(HexTriangleType.TopLeft);
            int[] botLeft = getHexTriangleIndices(HexTriangleType.BotLeft);
            int[] bot = getHexTriangleIndices(HexTriangleType.Bot);
            int[] botRight = getHexTriangleIndices(HexTriangleType.BotRight);

            for (int i = 0; i < 3; i++) { GL.Color3(color); GL.Vertex3((position + (hexVectors[top[i]])).X, (position + hexVectors[top[i]]).Y, (position + hexVectors[top[i]]).Z); }
            
            for (int i = 0; i < 3; i++) { GL.Color3(color); GL.Vertex3((position + hexVectors[topRight[i]]).X, (position + hexVectors[topRight[i]]).Y, (position + hexVectors[topRight[i]]).Z); }

            for (int i = 0; i < 3; i++) { GL.Color3(color); GL.Vertex3((position + hexVectors[topLeft[i]]).X, (position + hexVectors[topLeft[i]]).Y, (position + hexVectors[topLeft[i]]).Z); }

            for (int i = 0; i < 3; i++) { GL.Color3(color); GL.Vertex3((position + hexVectors[botLeft[i]]).X, (position + hexVectors[botLeft[i]]).Y, (position + hexVectors[botLeft[i]]).Z); }

            for (int i = 0; i < 3; i++) { GL.Color3(color); GL.Vertex3((position + hexVectors[bot[i]]).X, (position + hexVectors[bot[i]]).Y, (position + hexVectors[bot[i]]).Z); }

            for (int i = 0; i < 3; i++) { GL.Color3(color); GL.Vertex3((position + hexVectors[botRight[i]]).X, (position + hexVectors[botRight[i]]).Y, (position + hexVectors[botRight[i]]).Z); }

        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(new Vector3(0, 0, 30), Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

          

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            // Draw a single hex in the center of the screen
            GL.Begin(BeginMode.Triangles);

            // Just draw one hex
            // 1st row, 1st col
            Vector3 white = new Vector3(1, 1, 1);
            //drawHex(new Vector3(0 * outerRadius(), 0, 0), white);
            //drawHex(new Vector3(1.5f * outerRadius(), -innerRadius(), 0), white);
            
            //drawHex(new Vector3(3f * outerRadius(), 0, 0), white);
            //drawHex(new Vector3(0 * outerRadius(), 2f* -innerRadius(), 0), white);
            //drawHex(new Vector3(3f * outerRadius(), 2f * -innerRadius(), 0), white);

            Vector3 vertOffset = new Vector3(-12, 8, 0);
            Vector3 baseOffset = new Vector3(1.5f, 0, 0);

            float downVal = 0;
            for (int row = 0; row < 45; row++)
            {
                for (int col = 0; col < 45; col++)
                {
                    if (row == 0 || row % 2 == 0)
                    {
                        if (col % 2 == 1)
                        {
                            downVal = -innerRadius();
                            //drawHex(new Vector3(col * outerRadius() * 1.5f * 2, downVal, 0) + vertOffset, new Vector3(1, 0, 0));
                        } else
                        {
                            drawHex(new Vector3(col * outerRadius() * 1.5f, -innerRadius() * row, 0) + vertOffset, white);
                        }
                    }

                    if (row % 2 == 1)
                    {
                        if (col % 2 == 0)
                            drawHex(new Vector3((outerRadius() * 1.5f * col) + 1.5f, -innerRadius() * row, 0) + vertOffset, new Vector3(0, 1, 1));
                    }
                    

                }
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
