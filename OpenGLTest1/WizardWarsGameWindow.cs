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


        int textureIdHouse = 0;
        int textureIdCastle = 0;
        int textureIdGrid = 0;
        int textureIdGrass = 0;
        lib.HexMap hexMap = null;

        public WizardWarsGameWindow() : base(800, 600, GraphicsMode.Default, "WizardWars 0.0.1")
        {
            VSync = VSyncMode.On;

            lib.Texture _texGrass = new lib.Texture("G:\\Archive\\Pictures\\2D\\Textures\\grass_64x64.png");
            lib.Texture _texGrassWater = new lib.Texture("G:\\Archive\\Pictures\\2D\\Textures\\grass_water_64x64.png");
            lib.Texture _texWater = new lib.Texture("G:\\Archive\\Pictures\\2D\\Textures\\water_64x64.png");
            lib.Texture _texHouse = new lib.Texture("G:\\Archive\\Pictures\\2D\\Textures\\house_64x64_2.png");
            lib.Texture _texGrid = new lib.Texture("G:\\Archive\\Pictures\\2D\\Textures\\grid_64x64.png");
            lib.Texture _hexGrid = new lib.Texture("G:\\Archive\\Pictures\\2D\\Textures\\hex_64x64.png");
            textureIdGrid = _hexGrid.GetId();

            List<lib.MapLayer> _mapLayers = new List<lib.MapLayer>();
            lib.Texture[,] terrainTextures = new lib.Texture[9,9];
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (x % 2 == 0 && y % 8 == 0)
                        terrainTextures[x, y] = _texGrass;
                    else
                        terrainTextures[x, y] = _texGrass;
                }
            }
            lib.MapLayer terrainLayer = new lib.MapLayer(terrainTextures);
            _mapLayers.Add(terrainLayer);


            // Resources layer
            lib.Texture[,] resourcesTextures = new lib.Texture[9, 9];
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    //if (x == 3 && y == 2)
                        resourcesTextures[x, y] = _texHouse;
                    //else
                       // resourcesTextures[x, y] = _texGrass;
                }
            }
            lib.MapLayer resourcesLayer = new lib.MapLayer(resourcesTextures);
            _mapLayers.Add(resourcesLayer);

            // GridLayer
            lib.Texture[,] gridTextures = new lib.Texture[9, 9];
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    gridTextures[x, y] = _texGrid;
                }
            }
            lib.MapLayer gridLayer = new lib.MapLayer(gridTextures);
            _mapLayers.Add(gridLayer);


            hexMap = new lib.HexMap(new lib.MapDimension(9), new lib.MapDimension(9), _mapLayers);
        }
        
    
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            textureIdHouse = new lib.Texture("G:\\Archive\\Pictures\\2D\\Textures\\house_64x64_2.png").GetId();
            textureIdGrass = new lib.Texture("G:\\Archive\\Pictures\\2D\\Textures\\grass_16x16.png").GetId();
            textureIdCastle = new lib.Texture("G:\\Archive\\Pictures\\2D\\Tilesets\\King\\castle32x32.png").GetId();

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            Matrix4 orthoProjection = Matrix4.CreateOrthographic(40, 40, 1, 500);
            orthoProjection = Matrix4.CreateOrthographicOffCenter(0, 800, 600, 0, 1, 100);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref orthoProjection);
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

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            

            GL.BindTexture(TextureTarget.Texture2D, textureIdCastle);
            GL.Begin(BeginMode.Quads);

            /*GL.Color3(1.0f, 1.0f, 1.0f);*/ GL.TexCoord2(0, 1); GL.Vertex3(-1.0f, -2.0f, 2.0f);
            /*GL.Color3(1.0f, 1.0f, 1.0f);*/ GL.TexCoord2(1, 1); GL.Vertex3(1.0f, -2.0f, 2.0f);
            /*GL.Color3(1.0f, 1.0f, 1.0f);*/ GL.TexCoord2(1, 0); GL.Vertex3(1.0f, 2.0f, 2.0f);
            /*GL.Color3(1.0f, 1.0f, 1.0f);*/ GL.TexCoord2(0, 0); GL.Vertex3(-1.0f, 2.0f, 2.0f);

            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, textureIdGrid);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.Begin(BeginMode.Quads);

            //for (int i = 10; i > -5; i -= 2)
            //{
            //    GL.TexCoord2(0, 1); GL.Vertex3((-i), -1.0f, 3.9f);
            //    GL.TexCoord2(1, 1); GL.Vertex3((-i) + 2, -1.0f, 3.9f);
            //    GL.TexCoord2(1, 0); GL.Vertex3((-i) + 2, 1.0f, 3.9f);
            //    GL.TexCoord2(0, 0); GL.Vertex3((-i), 1.0f, 3.9f);
            //}

            //for (int i = 10; i > -5; i -= 2)
            //{
            //    GL.TexCoord2(0, 1); GL.Vertex3((-i) + 1, -2.5f, 4.9f);
            //    GL.TexCoord2(1, 1); GL.Vertex3((-i) + 2 + 1, -2.5f, 4.9f);
            //    GL.TexCoord2(1, 0); GL.Vertex3((-i) + 2 + 1, -0.5f, 4.9f);
            //    GL.TexCoord2(0, 0); GL.Vertex3((-i) + 1, -0.5f, 4.9f);
            //}
            
            //for (int i = 10; i > -5; i -= 2)
            //{
            //    GL.TexCoord2(0, 1); GL.Vertex3((-i), -4.0f, 5.9f);
            //    GL.TexCoord2(1, 1); GL.Vertex3((-i) + 2, -4.0f, 5.9f);
            //    GL.TexCoord2(1, 0); GL.Vertex3((-i) + 2, -2.0f, 5.9f);
            //    GL.TexCoord2(0, 0); GL.Vertex3((-i), -2.0f, 5.9f);
            //}

            //for (int i = 10; i > -5; i -= 2)
            //{
            //    GL.TexCoord2(0, 1); GL.Vertex3((-i) + 1, -5.5f, 6.9f);
            //    GL.TexCoord2(1, 1); GL.Vertex3((-i) + 2 + 1, -5.5f, 6.9f);
            //    GL.TexCoord2(1, 0); GL.Vertex3((-i) + 2 + 1, -3.5f, 6.9f);
            //    GL.TexCoord2(0, 0); GL.Vertex3((-i) + 1, -3.5f, 6.9f);
            //}


            //GL.End();

            GL.BindTexture(TextureTarget.Texture2D, textureIdHouse);
            GL.Begin(BeginMode.Quads);
            for (int i = 10; i > -5; i -= 2)
            {
                GL.TexCoord2(0, 1); GL.Vertex3((-i), -4.0f, 6.9f);
                GL.TexCoord2(1, 1); GL.Vertex3((-i) + 2, -4.0f, 6.9f);
                GL.TexCoord2(1, 0); GL.Vertex3((-i) + 2, -2.0f, 6.9f);
                GL.TexCoord2(0, 0); GL.Vertex3((-i), -2.0f, 6.9f);
            }

            GL.End();

            float h = 4;
            float r = 0.866025f;
            float s = 8;

            for (int row = 0; row < 12; row++)
            {
                    int v = row & 1;
                    GL.BindTexture(TextureTarget.Texture2D, textureIdGrass);
                    GL.Begin(PrimitiveType.Quads);
                
                    for (int i = 0; i < 20; i += 1)
                    {
                        GL.TexCoord2(0, 1); GL.Vertex3((i * 34 * r + (row & 1) * r*16),         row * (h+s)*2,          6.9f + row*0.1);
                        GL.TexCoord2(1, 1); GL.Vertex3((i * 34 * r + (row & 1) * r*16 + 32),    row * (h+s)*2,          6.9f + row*0.1);
                        GL.TexCoord2(1, 0); GL.Vertex3((i * 34 * r + (row & 1) * r*16 + 32),    row * (h+s)*2 + 32,     6.9f + row*0.1);
                        GL.TexCoord2(0, 0); GL.Vertex3((i * 34 * r + (row & 1) * r*16),         row * (h+s)*2 + 32,     6.9f + row*0.1);
                    }
               
                    GL.End();
                
            }

            GL.BindTexture(TextureTarget.Texture2D, textureIdHouse);
            GL.Begin(BeginMode.Quads);
            //for (int i = 0; i < 10; i += 1)
            //{
            //    GL.TexCoord2(0, 1); GL.Vertex3((i * 64),      64.0f, 17.9f);
            //    GL.TexCoord2(1, 1); GL.Vertex3((i * 64) + 64, 64.0f, 17.9f);
            //    GL.TexCoord2(1, 0); GL.Vertex3((i * 64) + 64, 0.0f, 17.9f);
            //    GL.TexCoord2(0, 0); GL.Vertex3((i * 64),      0.0f, 17.9f);
            //}

            GL.End();


        }

        void drawPixels()
        {
            byte[] rasters =
            {
                0xff, 0xff, 0xff, 0xff,
                0xff, 0xff, 0xff, 0xff,
                0xcf, 0xcf, 0xff, 0xff,
                0xff, 0xcf, 0xff, 0xff,
                0xc0, 0xcf, 0xcf, 0xc0,
                0xcf, 0xcf, 0xff, 0xc0,
                0xff, 0xcf, 0xcf, 0xff,
                0xff, 0xff, 0xff, 0xc0,
            };
            GL.RasterPos2(10, 10);
            GL.Bitmap(4, 8, 0, 0, 0, 0, rasters);
        }

        void update(double delta)
        {
            
            if (Keyboard.GetState().IsKeyDown(Key.A)) {
                moveH += 20f * (float)delta;
            }

            if (Keyboard.GetState().IsKeyDown(Key.D))
            {
                moveH -= 20f * (float)delta;
            }

            if (Keyboard.GetState().IsKeyDown(Key.W))
            {
                moveV -= 20f * (float)delta;
            }

            if (Keyboard.GetState().IsKeyDown(Key.S))
            {
                moveV += 20f * (float)delta;
            }

        }

        private float moveH, moveV;
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            update(e.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.UnitZ, Vector3.UnitY);
            Matrix4 scale = Matrix4.Scale(2.0f);
            Matrix4 transl = Matrix4.Translation(moveH, moveV, 0);
            Matrix4 model = Matrix4.Mult(transl, scale);
            Matrix4 modelview = Matrix4.Mult(view, model);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            //hexMap.Draw(new Vector3(-8, 6, 0));
            drawTexturedQuads();
            //drawPixels();

            SwapBuffers();
        }

        

    }
}
