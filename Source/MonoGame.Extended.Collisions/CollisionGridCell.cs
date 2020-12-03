namespace MonoGame.Extended.Collisions
{

    /// <summary>
    /// Represents a single cell in a collision grid.
    /// </summary>
    public class CollisionGridCell : ICollidable
    {
        private readonly CollisionGrid _parentGrid;

        /// <summary>
        /// Creates a collision grid cell at a location in the parent grid.
        /// </summary>
        /// <param name="parentGrid">The collision grid which this cell is a part of.</param>
        /// <param name="column">The column position of this cell.</param>
        /// <param name="row">The row position of this cell.</param>
        /// <param name="data"></param>
        public CollisionGridCell(CollisionGrid parentGrid, int column, int row, int data)
        {
            _parentGrid = parentGrid;
            Column = column;
            Row = row;
            Data = data;
            Flag = data == 0 ? CollisionGridCellFlag.Empty : (data == 1 ? CollisionGridCellFlag.Solid : CollisionGridCellFlag.Interesting);
        }

        /// <summary>
        /// Gets the Column in the parent grid that this cell represents.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Gets the Row in the parent grid that this cell represents.
        /// </summary>
        public int Row { get; }
        public int Data { get; private set; }
        public object Tag { get; set; }
        public CollisionGridCellFlag Flag { get; set; }

        /// <summary>
        /// Gets the bounding box of the cell.
        /// </summary>
        public RectangleF BoundingBox
        {
            get
            {
                if (CollisionWorldExtensions.UsingTiled && Flag is CollisionGridCellFlag.Interesting)
                    return _parentGrid.GetCellRectangleFromObject(Column, Row).ToRectangleF();
                else
                    return _parentGrid.GetCellRectangle(Column, Row).ToRectangleF();
            }
        }

    }
}