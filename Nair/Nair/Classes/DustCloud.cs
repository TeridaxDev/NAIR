using Microsoft.Xna.Framework;

namespace Nair.Classes
{
    class DustCloud
    {

        private Point location;
        private Direction direction;
        private int age;

        public Point Location { get => location; }
        public Direction Facing { get => direction; }
        public int Age { get => age; }


        public DustCloud(Point location, Direction direction)
        {
            age = 0;
            this.direction = direction;
            this.location = location;
        }

        public int UpAge()
        {
            age++;
            return age;
        }

    }
}
