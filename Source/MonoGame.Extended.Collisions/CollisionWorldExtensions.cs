using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Collisions
{
    public static class CollisionWorldExtensions
    {
        private static TiledMap _map;

        public static bool UsingTiled { get; private set; }

        public static Rectangle GetCellRectangleFromObject(this CollisionGrid grid, int column, int row)
        {
            var gid = grid.GetCellAtIndex(column, row).Data;

            TiledMapTileset Tileset = _map.GetTilesetByTileGlobalIdentifier(gid);
            TiledMapObject[] objectarray = GetBounds(Tileset, gid);

            if (objectarray.Length is 1)
            {
                TiledMapObject firstobject = objectarray[0];

                //TODO: object type

                var PositionInCell = firstobject.Position;
                var PositionOnMap = new Vector2(grid.CellWidth * column, grid.CellHeight * row) + PositionInCell;
                var SizeOfRect = new Point((int)firstobject.Size.Height, (int)firstobject.Size.Width);
                return new Rectangle(PositionOnMap.ToPoint(), SizeOfRect);
            }
            else
            {
                return new Rectangle();
            }
        }

        public static CollisionGrid CreateGrid(this CollisionWorld world, TiledMap map)
        {
            _map = map;
            var h = map.HeightInPixels;
            var w = map.WidthInPixels;
            var mapsizeinpixels = h * w;
            var gridsize = mapsizeinpixels / map.TileHeight;

            int[] data = new int[gridsize];
            int index = 0;

            foreach (var tileLayer in map.TileLayers)
            {
                for (var x = 0; x < tileLayer.Width; x++)
                {
                    for (var y = 0; y < tileLayer.Height; y++)
                    {
                        TiledMapTile tile = tileLayer.GetTile((ushort)x, (ushort)y);
                        TiledMapTileset Tileset = _map.GetTilesetByTileGlobalIdentifier(tile.GlobalIdentifier);
                        if (Tileset != null)
                        {
                            TiledMapObject[] objectarray = GetBounds(Tileset, tile.GlobalIdentifier);

                            if (objectarray != null)
                            {
                                if (objectarray.Length is 1)
                                {
                                    TiledMapObject firstobject = objectarray[0];

                                    //TODO: object type

                                    if (firstobject.Size != null)
                                    {
                                        if (firstobject.Size == new Point(tileLayer.TileWidth, tileLayer.TileHeight))
                                        {
                                            data[index] = 1; //Solid
                                        }
                                        else
                                        {
                                            data[index] = tile.GlobalIdentifier; //Interesting
                                        }
                                    }
                                }
                                else
                                {
                                    data[index] = 0; //Empty
                                }
                            }
                        }

                        index++;
                    }
                }
            }

            UsingTiled = true;

            return world.CreateGrid(data, map);
        }

        private static TiledMapObject[] GetBounds(TiledMapTileset set, int id)
        {
            var fgid = _map.GetTilesetFirstGlobalIdentifier(set);
            var localid = id - fgid;
            var results = set.Tiles.Find(cell => cell.LocalTileIdentifier.Equals(localid));

            if (results is null)
            {
                return System.Array.Empty<TiledMapObject>();
            }
            else
            {
                return results.Objects.ToArray();
            }
        }
    }
}