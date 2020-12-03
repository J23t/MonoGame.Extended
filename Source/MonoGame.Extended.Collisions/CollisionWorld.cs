using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Collisions
{
    public class CollisionWorld : IDisposable, IUpdate
    {
        private readonly List<CollisionActor> _actors;

        private readonly Vector2 _gravity;
        private CollisionGrid _grid;

        public CollisionWorld(Vector2 gravity)
        {
            _gravity = gravity;
            _actors = new List<CollisionActor>();
        }

        public void Dispose()
        {
        }

        public void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var actor in _actors)
            {
                actor.Velocity += _gravity * deltaTime;
                actor.Position += actor.Velocity * deltaTime;

                if (_grid != null)
                    foreach (ICollidable collidable in _grid.GetCollidables(actor.BoundingBox))
                    {
                        RectangleF intersection = RectangleF.Intersection(collidable.BoundingBox, actor.BoundingBox);

                        if (intersection.IsEmpty)
                            continue;

                        CollisionInfo info = GetCollisionInfo(actor, collidable, intersection);
                        actor.OnCollision(info);
                    }
            }
        }

        public CollisionActor CreateActor(IActorTarget target)
        {
            var actor = new CollisionActor(target);
            _actors.Add(actor);
            return actor;
        }

        public void RemoveActor(IActorTarget target)
        {
            var actor = new CollisionActor(target);
            _actors.Remove(actor);
        }

        public CollisionGrid CreateGrid(int[] data, int columns, int rows, int cellWidth, int cellHeight)
        {
            if (_grid != null)
                throw new InvalidOperationException("Only one collision grid can be created per world");

            _grid = new CollisionGrid(data, columns, rows, cellWidth, cellHeight);
            return _grid;
        }

        internal CollisionGrid CreateGrid(int[] data, TiledMap map)
        {
            if (_grid != null)
                throw new InvalidOperationException("Only one collision grid can be created per world");

            _grid = new CollisionGrid(data, map.Width, map.Height, map.TileWidth, map.TileHeight);
            return _grid;
        }

        private CollisionInfo GetCollisionInfo(ICollidable first, ICollidable second, RectangleF intersectingRectangle)
        {
            var info = new CollisionInfo
            {
                Other = second
            };

            if (intersectingRectangle.Width < intersectingRectangle.Height)
            {
                var d = first.BoundingBox.Center.X < second.BoundingBox.Center.X
                    ? intersectingRectangle.Width
                    : -intersectingRectangle.Width;
                info.PenetrationVector = new Vector2(d, 0);
            }
            else
            {
                var d = first.BoundingBox.Center.Y < second.BoundingBox.Center.Y
                    ? intersectingRectangle.Height
                    : -intersectingRectangle.Height;
                info.PenetrationVector = new Vector2(0, d);
            }

            return info;
        }
    }
}