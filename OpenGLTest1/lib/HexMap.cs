using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGLTest1.lib
{

    class MapLayer
    {
        private Texture[,] _textures;
        public MapLayer(Texture[,] textures)
        {
            this._textures = textures;
        }

        public Texture At(int x, int y)
        {
            return _textures[x,y];
        }
    }

    class MapDimension
    {
        public MapDimension(UInt16 v)
        {
            Size = v;
        }

        private UInt16 _size  = 0;
        public UInt16 Size {
            get { return _size; }
            set 
            {
                if (value < 8 || value > 512)
                   throw new Exception("MapDimenstion must be between 8 and 512");
                else
                    _size = value;
            }
        }
    }

    class HexMap
    {

        private MapDimension _mapWidth, _mapHeight;
        private List<MapLayer> _mapLayers;
        private Texture _texHouse;
        private Texture _texGrass;
        private Texture _texGrassWater;

        enum HexTriangleType
        {
            Top,
            TopRight,
            TopLeft,
            BotLeft,
            BotRight,
            Bot
        }

        public HexMap(MapDimension mapWidth, MapDimension mapHeight, List<MapLayer> mapLayers)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _mapLayers = mapLayers;

            _texHouse = new Texture("G:\\Archive\\Pictures\\2D\\Tilesets\\King\\House32x32.png");
            _texGrass = new Texture("G:\\Archive\\Pictures\\2D\\Textures\\grass_16x16.png");
            _texGrassWater = new Texture("G:\\Archive\\Pictures\\2D\\Textures\\grass_water_64x64.png");
        }


        /***
         *  Draws a hexmap at the given world position.
         *  A map consists of several layers, which are drawn on top of each other.
         *  The lowest layer can be used to represent terrain, e.g. grass, water, sand etc.
         *  At the layer above, we could paint trees, streets, goldmines etc.
         * 
         */
        public void Draw(Vector3 position)
        {
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            

            // This is the offset which the uneven rows are offset to the right.
            Vector3 baseOffset = new Vector3(1.5f, 0, 0);

            float downVal = 0;

            float mapLayerNr = 0;
            foreach (MapLayer ml in _mapLayers)
            {
                for (int row = 0; row < _mapWidth.Size; row++)
                {
                    for (int col = 0; col < _mapHeight.Size; col++)
                    {
                        if (row == 0 || row % 2 == 0)
                        {
                            if (col % 2 == 1)
                            {
                                downVal = -innerRadius();
                            }
                            else
                            {
                                drawHex(new Vector3(col * outerRadius() * 1.5f, -innerRadius() * row, mapLayerNr/5000 ) + position, new Vector3(1, 1, 1), ml.At(col, row));
                            }
                        }

                        if (row % 2 == 1)
                        {
                            if (col % 2 == 0)
                                drawHex(new Vector3((outerRadius() * 1.5f * col) + 1.5f, -innerRadius() * row, mapLayerNr/5000) + position, new Vector3(1, 1, 1), ml.At(col, row));
                        }
                    }
                }
                mapLayerNr++;
            }

           
        }

        public Vector2 GetTileForWorldCoordinate(Vector3 worldCoordinate)
        {
            return new Vector2( 0, 0);
        }

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

            vecs[0] = new Vector3(0, 0, 0);
            vecs[1] = new Vector3(0.5f * outerRadius(), innerRadius(), 0);
            vecs[2] = new Vector3(-0.5f * outerRadius(), innerRadius(), 0);
            vecs[3] = new Vector3(-outerRadius(), 0, 0);
            vecs[4] = new Vector3(-0.5f * outerRadius(), -innerRadius(), 0);
            vecs[5] = new Vector3(0.5f * outerRadius(), -innerRadius(), 0);
            vecs[6] = new Vector3(outerRadius(), 0, 0);

            return vecs;
        }

        Vector2[] getHexUVs()
        {
            Vector2[] uvs = new Vector2[7];
            uvs[0] = new Vector2(0.5f, 0.5f);
            uvs[1] = new Vector2(0.99f, 0.0f);
            uvs[2] = new Vector2(0.05f, 0.0f);
            uvs[3] = new Vector2(0.0f, 0.5f);
            uvs[4] = new Vector2(0.05f, 1f);
            uvs[5] = new Vector2(0.99f, 1f);
            uvs[6] = new Vector2(1f, 0.5f);

            return uvs;
        }

        int[] getHexTriangleIndices(HexTriangleType hexTriangleType)
        {
            switch (hexTriangleType)
            {
                case HexTriangleType.Top: return new int[3] { 0, 1, 2 };
                case HexTriangleType.TopRight: return new int[3] { 0, 6, 1 };
                case HexTriangleType.TopLeft: return new int[3] { 0, 2, 3 };
                case HexTriangleType.BotLeft: return new int[3] { 0, 3, 4 };
                case HexTriangleType.Bot: return new int[3] { 0, 4, 5 };
                case HexTriangleType.BotRight: return new int[3] { 0, 5, 6 };
                default: return null;
            }
        }


        protected void drawHex(Vector3 position, Vector3 color, Texture texture)
        {
            Vector3[] hexVectors = getHexVectors();
            Vector2[] hexUVs = getHexUVs();
            int[] top = getHexTriangleIndices(HexTriangleType.Top);
            int[] topRight = getHexTriangleIndices(HexTriangleType.TopRight);
            int[] topLeft = getHexTriangleIndices(HexTriangleType.TopLeft);
            int[] botLeft = getHexTriangleIndices(HexTriangleType.BotLeft);
            int[] bot = getHexTriangleIndices(HexTriangleType.Bot);
            int[] botRight = getHexTriangleIndices(HexTriangleType.BotRight);

            GL.BindTexture(TextureTarget.Texture2D, texture.GetId());
            GL.Begin(PrimitiveType.Triangles);

            for (int i = 0; i < 3; i++) { GL.Color3(color); GL.TexCoord2(hexUVs[top[i]].X, hexUVs[top[i]].Y); GL.Vertex3((position + (hexVectors[top[i]])).X, (position + hexVectors[top[i]]).Y, (position + hexVectors[top[i]]).Z); }

            for (int i = 0; i < 3; i++) { GL.Color3(color); GL.TexCoord2(hexUVs[topRight[i]].X, hexUVs[topRight[i]].Y); GL.Vertex3((position + hexVectors[topRight[i]]).X, (position + hexVectors[topRight[i]]).Y, (position + hexVectors[topRight[i]]).Z); }

            for (int i = 0; i < 3; i++) { GL.Color3(color); GL.TexCoord2(hexUVs[topLeft[i]].X, hexUVs[topLeft[i]].Y); GL.Vertex3((position + hexVectors[topLeft[i]]).X, (position + hexVectors[topLeft[i]]).Y, (position + hexVectors[topLeft[i]]).Z); }

            for (int i = 0; i < 3; i++) { GL.Color3(color); GL.TexCoord2(hexUVs[botLeft[i]].X, hexUVs[botLeft[i]].Y); GL.Vertex3((position + hexVectors[botLeft[i]]).X, (position + hexVectors[botLeft[i]]).Y, (position + hexVectors[botLeft[i]]).Z); }

            for (int i = 0; i < 3; i++) { GL.Color3(color); GL.TexCoord2(hexUVs[bot[i]].X, hexUVs[bot[i]].Y); GL.Vertex3((position + hexVectors[bot[i]]).X, (position + hexVectors[bot[i]]).Y, (position + hexVectors[bot[i]]).Z); }

            for (int i = 0; i < 3; i++) { GL.Color3(color); GL.TexCoord2(hexUVs[botRight[i]].X, hexUVs[botRight[i]].Y); GL.Vertex3((position + hexVectors[botRight[i]]).X, (position + hexVectors[botRight[i]]).Y, (position + hexVectors[botRight[i]]).Z); }

            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, 0);

        }
    }
}
