﻿namespace DigitalDoor.Reporting.Entities.ValueObjects;

public class Dimension
{
    public double Height { get; set; }
    public double Width { get; set; }

    public Dimension(double width, double height)
    {
        this.Width = width;
        this.Height = height;
    }
    public Dimension(Dimension dimension)
    {
        this.Width = dimension.Width;
        this.Height = dimension.Height;
    }

    public Dimension() { }

}
