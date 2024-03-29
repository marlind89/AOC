﻿using System.Diagnostics;
using System.Text;

namespace AOC.Helpers;

public abstract class Puzzle<T> : Puzzle<T,T>
{ 
}

public abstract class Puzzle<T1, T2> : IPuzzle
{
    public int PuzzleNumber { get; }

    protected T1? One { get; set; }
    protected T2? Two { get; set; }

    private readonly string[] _lines;
    private long _solveTime;

    public Puzzle() 
    {
        PuzzleNumber = int.Parse(GetType().Name.Replace("Puzzle", ""));
         _lines = File.ReadAllLines($"Inputs/Puzzle{PuzzleNumber}.txt");
    }

    public void Solve() 
    {
        var sw = new Stopwatch();
        sw.Start();
        Solve(_lines);
        sw.Stop();
        _solveTime = sw.ElapsedMilliseconds;
    }
    
    protected abstract void Solve(string[] lines);

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Puzzle {PuzzleNumber} ({_solveTime} ms)");
        builder.AppendLine();
        builder.AppendLine($"  Part One: {One}");
        builder.AppendLine($"  Part Two: {Two}");
        return builder.ToString();
    }
}
