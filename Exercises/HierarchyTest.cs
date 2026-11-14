namespace Exercises
{
    public class HierarchyTest
    {
        public static void Starter()
        {
            //HierarchyTest.Circle_starter();
            HierarchyTest.Circle2_starter();
            //HierarchyTest.Line2_starter();
            //Console.WriteLine(IPolygon.GetInterfaceName());
            //Console.WriteLine(ICircle.GetInterfaceName());
        }
        public static void Circle_starter()
        {
            Circle circle_1 = new Circle(5);
            Console.WriteLine(circle_1.ToString());
            Console.WriteLine();
            Console.WriteLine($"Radius: {circle_1.GetRadius()}");
            Console.WriteLine($"Length: {circle_1.GetLength()}");
            Console.WriteLine($"Square: {circle_1.GetSquare()}");
        }
        public static void Circle2_starter()
        {
            Circle2 circle_1 = new Circle2(2, 0, 0);
            Console.WriteLine(circle_1.ToString());
            Console.WriteLine();
            Console.WriteLine($"Radius: {circle_1.GetRadius()}");
            Console.WriteLine($"Length: {circle_1.GetLength()}");
            Console.WriteLine($"Square: {circle_1.GetSquare()}");
            Console.WriteLine($"Point on circle: {circle_1.IsPointOnCircle(new Point2(2, 2))}");
            Console.WriteLine($"{circle_1.IsPointOnCircleVerbose(2, 2)}");
            Console.WriteLine();
            Console.WriteLine($"{circle_1.IntersectsCircleVerbous(new Circle2(2, 3, 0))}");
        }
        public static void Line2_starter()
        {
            //Line2 line2_1 = new Line2(new Coordinates2(5, 6), new Coordinates2(0, 1));
            //Line2 line2_1 = new Line2(new Point2(5, 6), new Point2(0, 1));
            Line2 line2_1 = Line2.NewLine(1, 1, 2, 2);
            Console.WriteLine(line2_1.Name());
            Console.WriteLine($"Line general formula: {line2_1.EquationFormula()}");
            Console.WriteLine($"Line formula: {line2_1.Equation()}");
            Console.WriteLine(line2_1.IsPointInLine(2, 2));
            Console.WriteLine(line2_1.IsPointInLine(2, 3));
            Console.WriteLine(line2_1.IsPointInLineVerbose(2, 2));
            Console.WriteLine(line2_1.IsPointInLineVerbose(2, 3));
            Console.WriteLine(line2_1.ToString());
        }
    }
    public enum IntersectState
    {
        Undefined = -1,
        NoCommonPoints = 0,
        Touch = 1,
        Intersect = 2
    }
    public enum IsPointInside
    {
        Undefined = -1,
        Outside = 0,
        OnBorder = 1,
        Inside = 2
    }
    public interface ICoordinates2
    {
        public double getXCoord();
        public double getYCoord();
        public static string GetInterfaceName() => "2 dimension coordinates interface";
    }
    public interface ICoordinates3 : ICoordinates2
    {
        public double getZCoord();
        public static string GetInterfaceName() => "3 dimension coordinates interface";
    }
    public interface ICircle
    {
        public string Name();
        public Double GetSquare();
        public Double GetRadius();
        public Double GetLength();
        public static string GetInterfaceName() => "Circle interface";
        //public static string GetInterfaceName()
        //{
        //    return "Circle interface";
        //}
    }
    public interface ICircle2 : ICircle
    {
        public string Equation();
        public string EquationFormula();
        public IsPointInside IsPointOnCircle(Point2 _point);
        public IntersectState IntersectsCircle(Circle2 _circle);
    }
    public interface IPoint
    {
        //public int NumberOfDimensions();
        public static string GetInterfaceName() => "Point interface";
    }
    public interface IPoint2 : IPoint
    {
        public double GetXCoord();
        public double GetYCoord();
        public Coordinates2 GetCoordinates();
        public static string GetInterfaceName() => "2 coordinates point interface";
    }
    public interface IPoint3 : IPoint2
    {
        public double GetZCoord();
        public Coordinates3 GetCoordinates();
        public static string GetInterfaceName() => "3 coordinates dot interface";
    }
    public interface ILine2
    {
        public string Name();
        public string Equation();
        public string EquationFormula();
        public bool IsPointInLine(Point2 _point);
        public static string GetInterfaceName() => "Line interface";
    }
    public interface IPolygon 
    {
        public int GetNumberOfSides(); // OOP
        public int NumberOfSides { get; init; } // Procedural
        public Double GetSquare();
        public Double GetPerimeter();
        public bool CheckSides();
        public void SetSide(int _sideId, double _length)
        {
            if (!this.CheckSides())
            {
                throw new Exception($"Side lengths are incorrect!");
            }
        }
        public static string GetInterfaceName() => "Polygon interface";
    }
    public class Coordinates2 : ICoordinates2
    {
        double x, y;
        public double X
        {
            set { this.x = X; }
            get { return this.x; }
        }
        public Coordinates2(double _x, double _y)
        {
            this.x = _x;
            this.y = _y;
        }
        public double getXCoord() => this.x;
        public double getYCoord() => this.y;
        //public double getXCoord()
        //{
        //    return this.x;
        //}
        //public double getYCoord()
        //{
        //    return this.y;
        //}
        public void Deconstruct(out double _x, out double _y)
        {
            _x = this.x;
            _y = this.y;
        }
    }
    public class Coordinates3 : ICoordinates3
    {
        double x, y, z;
        public Coordinates3(double _x, double _y, double _z)
        {
            this.x = _x;
            this.y = _y;
            this.z = _z;
        }
        public double getXCoord() => this.x;
        public double getYCoord() => this.y;
        public double getZCoord() => this.z;
        public void Deconstruct(out double _x, out double _y, out double _z)
        {
            _x =this.x;
            _y =this.y;
            _z =this.z;
        }
    }
    public class Point2 : IPoint2
    {
        protected Coordinates2 pointCoordinates;
        public Point2(Coordinates2 _pointCoordinates)
        {
            this.pointCoordinates = _pointCoordinates;
        }
        public Point2(double _x, double _y) : this(new Coordinates2(_x, _y))
        {
        }
        //public int NumberOfDimensions() => 2;
        public double GetXCoord() => this.pointCoordinates.getXCoord();
        public double GetYCoord() => this.pointCoordinates.getYCoord();
        public Coordinates2 GetCoordinates() => pointCoordinates;
        public static Point2 FromCoord(double _x, double _y)
        {
            return new Point2(_x, _y);
        }
    }
    public abstract class Shape // Base class for all shapes
    {
        public abstract string Name();
    }
    public class Circle : Shape, ICircle 
    {
        protected double radius;
        //public double Radius { get; init; }
        public Circle(double _radius)
        {
            if (_radius < 0)
            {
                throw new ArgumentException("Radius cannot be negative.\nRadius provided: {_radius}");
            }
            if (_radius == 0)
            {
                throw new ArgumentException("Radius cannot be zero. Use Point object.");
            }
            this.radius = _radius;
        }
        public double GetRadius()
        {
            return this.radius;
        }
        public Double GetLength()
        {
            return 2 * Math.PI * this.radius;
        }
        public Double GetSquare()
        {
            return Math.PI * Math.Pow(this.radius, 2);
        }
        public string ToString() => $"Circle in plane\nRadius: {this.radius}";
        public override string Name() => Circle.NameStatic();
        public static string NameStatic() => "Circle in plane";
    }
    public class Circle2 : Circle, ICircle2
    {
        Coordinates2 centerCoord;
        private double centerX;
        private double centerY;
        public double CenterX { get { return this.centerX; } }
        public double CenterY { get { return this.centerY; } }
        public Circle2(double _radius, Coordinates2 _centerCoord) : base(_radius)
        {
            this.centerCoord = _centerCoord;
            this.Initialize();
        }
        public Circle2(double _radius, Point2 _centerPoint) : this(_radius, new Coordinates2(_centerPoint.GetXCoord(), _centerPoint.GetYCoord()))
        {
        }
        public Circle2(double _radius, double _centerX, double _centerY) : this(_radius, new Coordinates2(_centerX, _centerY))
        {
        }
        protected void Initialize()
        {
            this.centerX = this.centerCoord.getXCoord();
            this.centerY = this.centerCoord.getYCoord();
        }
        public string EquationFormula() => "R^2 = (X - A)^2 + (Y - B)^2";
        public string Equation() => $"{this.radius}^2 =  (X - {this.centerX})^2 + (Y - {this.centerY})^2";
        public IsPointInside IsPointOnCircle(Point2 _point)
        {
            IsPointInside ret = IsPointInside.Undefined;
            double distance = Math.Sqrt(Math.Pow(_point.GetXCoord() - this.centerX, 2) + Math.Pow(_point.GetYCoord() - this.centerY, 2));
            if (distance > radius)
                ret = IsPointInside.Outside;
            if (distance == radius)
                ret = IsPointInside.OnBorder;
            if (distance < radius)
                ret = IsPointInside.Inside;
            return ret;
        }
        public string IsPointOnCircleVerbose(double _x, double _y)
        {
            Point2 point_1 = new Point2(_x, _y);
            string result = "";
            switch (this.IsPointOnCircle(point_1))
            {
                case IsPointInside.OnBorder:
                    result = "on";
                    break;
                case IsPointInside.Inside:
                    result = "inside";
                    break;
                case IsPointInside.Outside:
                    result = "outside";
                    break;
            }
            return $"The point ({_x},{_y}) is {result} the circle: {this.Equation()}";
        }
        public IntersectState IntersectsCircle(Circle2 _circle)
        {
            IntersectState ret = IntersectState.Undefined;
            double centerDistance = Math.Sqrt(Math.Pow(this.centerX - _circle.centerX, 2) + Math.Pow(this.centerY - _circle.centerY, 2) );
            double radiusSum = this.GetRadius() + _circle.GetRadius();
            if (centerDistance == radiusSum)
                ret = IntersectState.Touch;
            if (centerDistance < radiusSum)
                ret = IntersectState.Intersect;
            if (centerDistance > radiusSum)
                ret = IntersectState.NoCommonPoints;
            return ret;
        }
        public string IntersectsCircleVerbous(Circle2 _circle)
        {
            string result = "";
            switch (this.IntersectsCircle(_circle))
            {
                case IntersectState.Touch:
                    result = "touches";
                    break;
                case IntersectState.Intersect:
                    result = "intersects";
                    break;
                case IntersectState.NoCommonPoints:
                    result = "does not intersect";
                    break;
            }
            return $"The circle {this.Equation()} {result} the circle: {_circle.Equation()}";
        }
        public new string ToString() => $"{Circle2.NameStatic()}\nRadius: {this.radius}";
        public override string Name() => Circle2.NameStatic();
        public new static string NameStatic() => "Circle in plane with decart coordinates (2 dimensions)";
    }
    public class Line2 : ILine2
    {
        Coordinates2 coord_1, coord_2;
        double x1, x2, y1, y2;
        double? a, b;
        public Line2(Coordinates2 _coord_1, Coordinates2 _coord_2)
        {
            this.coord_1 = _coord_1;
            this.coord_2 = _coord_2;
            this.Initialize();
        }
        public Line2(Point2 _point_1, Point2 _point_2)
            : this(_point_1.GetCoordinates(), _point_2.GetCoordinates())
        {
        }
        protected void Initialize()
        {
            this.x1 = this.coord_1.getXCoord();
            this.y1 = this.coord_1.getYCoord();
            this.x2 = this.coord_2.getXCoord();
            this.y2 = this.coord_2.getYCoord();
            if (this.x2 != this.x1)
            {
                this.a = (this.y2 - this.y1) / (this.x2 - this.x1);
                this.b = this.y1 - this.a * this.x1;
            }
        }
        public string Name() => Line2.NameStatic();
        public string EquationFormula() => "Y = A * X + B";
        public string Equation()
        {
            string ret;
            if (this.x2 == this.x1)
            {
                ret = $"X = {this.x1}";
            }
            else if (this.y2 == this.y1)
            {
                ret = $"Y = {this.y1}";
            }
            else
            {
                ret = $"Y = {this.a} * X + {this.b}";
            }
            return ret;
        }
        public bool IsPointInLine(Point2 _point) => (_point.GetYCoord() == _point.GetXCoord() * this.a + this.b);
        //public bool IsPointInLine(Point2 _point)
        //{
        //    bool ret = false;
        //    if (_point.GetYCoord() == _point.GetXCoord() * this.a + this.b)
        //    {
        //        ret = true;
        //    }
        //    return ret;
        //}
        public bool IsPointInLine(double _x, double _y) => this.IsPointInLine(new Point2(_x, _y));
        public string IsPointInLineVerbose(double _x, double _y)
            => $"Point ({_x},{_y}) is {(!this.IsPointInLine(_x, _y) ? "not " : "")}in line {this.Equation()}.";
        public string ToString() => $"Line in plane: {this.Equation()}\nPoint 1: ({this.x1}, {this.y1})\nPoint 1: ({this.x2}, {this.y2})";
        public static Line2 NewLine(int _x1, int _y1, int _x2, int _y2) => new Line2(new Point2(_x1, _y1), new Point2(_x2, _y2));
        public static string NameStatic() => "Line in plane with decart coordinates (2 dimensions)";
    }
    public abstract class Polygon : Shape, IPolygon
    {
        protected int numberOfSides;
        public int NumberOfSides { get; init; }
        protected double[] sides;
        public Polygon(int _numOfSides)
        {
            if (_numOfSides <= 0)
            {
                throw new ArgumentException($"Number of sides must be more than 2!\nNumber of sides provided: {_numOfSides}");
            }
            this.NumberOfSides = _numOfSides;
            this.numberOfSides = _numOfSides;
            sides = new double[_numOfSides];
        }
        public int GetNumberOfSides()
        {
            return this.numberOfSides;
            //return this.NumberOfSides;
        }
        public abstract Double GetSquare();
        public abstract Double GetPerimeter();
        public bool CheckSides()
        {
            bool ret = true;
            return ret;
        }
        public void SetSide(int _sideId, double _length)
        {
            if (_sideId < 0)
            {
                throw new ArgumentException($"Side ID must be positive!\nWrond sides ID: {_sideId}");
            }
            if (_sideId > numberOfSides)
            {
                throw new ArgumentException($"Side ID must not exceed number of sides {numberOfSides}!\nWrond sides Id: {_sideId}");
            }
            if (_length <= 0)
            {
                throw new ArgumentException($"Side length must be positive!\nWrond side length: {_length}");
            }
            sides[_sideId] = _length;
            if (!this.CheckSides())
            {
                throw new Exception($"Side lengths are incorrect!");
            }
        }

    }
    public class Triangle : Polygon
    {
        public const int TRIANGLE_NUMBER_OF_SIDES = 3;
        public Triangle()
            : base(Triangle.TRIANGLE_NUMBER_OF_SIDES)
        {
        }
        public override string Name() => Triangle.NameStatic();
        public static string NameStatic() => "Triangle";
        public int GetNumberOfSides()
        {
            return this.numberOfSides; 
        }
        public bool CheckSides()
        {
            bool ret = true;
            if (this.sides[0] < 0)
            {
                ret = false;
            }
            return ret;
        }
        public override Double GetSquare()
        {
            return 0;
        }
        public override Double GetPerimeter()
        {
            return 0;
        }
    }
    public class Quadrangle : Polygon
    {
        public const int QUADRANGLE_NUMBER_OF_SIDES = 3;
        public Quadrangle()
            : base(Quadrangle.QUADRANGLE_NUMBER_OF_SIDES)
        {
        }
        public override string Name() => Quadrangle.NameStatic();
        public static string NameStatic() => "Quadrangle";
        public int GetNumberOfSides()
        {
            return this.numberOfSides;
        }
        public bool CheckSides()
        {
            bool ret = true;
            if (this.sides[0] < 0)
            {
                ret = false;
            }
            return ret;
        }
        public override Double GetSquare()
        {
            return 0;
        }
        public override Double GetPerimeter()
        {
            return 0;
        }
    }
    public class Parallelogram : Quadrangle
    {
        public Parallelogram()
            : base()
        {
        }
        public override string Name() => Parallelogram.NameStatic();
        public static string NameStatic() => "Parallelogram";
        public int GetNumberOfSides()
        {
            return this.numberOfSides;
        }
        public bool CheckSides()
        {
            bool ret = true;
            return ret;
        }
        public Double GetSquare()
        {
            return 0;
        }
    }
    public class Rhomb : Parallelogram
    {
        public Rhomb()
            : base()
        {
        }
        public override string Name() => Rhomb.NameStatic();
        public static string NameStatic() => "Rhomb";
        public Double GetSquare()
        {
            return 0;
        }
    }
    public class Rectangle : Parallelogram
    {
        public Rectangle()
            : base()
        {
        }
        public Double GetSquare()
        {
            return 0;
        }
    }
    public class Square : Rectangle
    {
        public Square()
            : base()
        {
        }
        public Double GetSquare()
        {
            return 0;
        }
    }
}
