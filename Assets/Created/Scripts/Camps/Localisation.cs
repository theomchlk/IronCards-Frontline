using System;

public class Localisation
{
    public int row;
    public int col;
    
    public Localisation()
    {
        row = -1;
        col = -1;
    }
    
    public Localisation(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public int Row => row;
    public int Col => col;

    public override bool Equals(object obj)
    {
        return obj is Localisation other && Row == other.Row && Col == other.Col;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Col);
    }
}
