namespace AOC2021.Puzzles;

internal class Puzzle17 : Puzzle<int>
{
    private ((int x, int y) from, (int x, int y) to) _targetArea;

    protected override void Solve(string[] lines)
    {
        var x = lines[0][(2 + lines[0].IndexOf("x="))..lines[0].IndexOf(",")].Split("..").Select(int.Parse).ToArray();
        var y = lines[0][(2 + lines[0].IndexOf("y="))..].Split("..").Select(int.Parse).ToArray();

        _targetArea = (
            (x[0], y[0]),
            (x[1], y[1])
        );

        var hits = 0;
        var topYShot = int.MinValue;
        foreach (var forward in Enumerable.Range(0, 300))
        {
            foreach (var upward in Enumerable.Range(-200, 400))
            {
                if (Shoot(forward, upward, out var topY))
                {
                    hits++;
                    topYShot = Math.Max(topYShot, topY);
                }
            }
        }

        One = topYShot;
        Two = hits;
    }

    private bool Shoot(int forward, int upward, out int topY)
    {
        topY = int.MinValue;
        var currentPosition = (x: 0, y: 0);

        while (true)
        {
            currentPosition.x += forward;
            currentPosition.y += upward;
            topY = Math.Max(topY, currentPosition.y);
            upward--;
            if (forward > 0)
            {
                forward -= 1;
            }
            else if (forward < 0)
            {
                forward += 1;
            }

            if (IsWithinTarget(currentPosition))
            {
                return true;
            }
            if (MissedTarget(currentPosition))
            {
                return false;
            }
        }
    }

    private bool IsWithinTarget((int x, int y) coord) => 
        _targetArea.from.x <= coord.x && _targetArea.to.x >= coord.x &&
            _targetArea.from.y <= coord.y && _targetArea.to.y >= coord.y;

    private bool MissedTarget((int x, int y) coord) => 
        coord.x > _targetArea.to.x || coord.y < _targetArea.from.y;
}
