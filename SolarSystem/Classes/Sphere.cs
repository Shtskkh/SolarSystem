using OpenTK.Mathematics;

namespace SolarSystem.Classes;

public class Sphere
{
    public float[] Vertices { get; private set; }
    public uint[] Indices { get; private set; }
    public int VertexCount => Vertices.Length / 3;
    public int IndexCount => Indices.Length;

    public Sphere(float radius, int sectors, int stacks)
    {
        GenerateSphere(radius, sectors, stacks);
    }

    private void GenerateSphere(float radius, int sectors, int stacks)
    {
        var vertices = new List<float>();
        var indices = new List<uint>();

        var sectorStep = 2 * MathHelper.Pi / sectors;
        var stackStep = MathHelper.Pi / stacks;

        for (var i = 0; i <= stacks; ++i)
        {
            var stackAngle = MathHelper.Pi / 2 - i * stackStep;
            var xy = radius * (float)Math.Cos(stackAngle);
            var z = radius * (float)Math.Sin(stackAngle);

            for (var j = 0; j <= sectors; ++j)
            {
                var sectorAngle = j * sectorStep;

                var x = xy * (float)Math.Cos(sectorAngle);
                var y = xy * (float)Math.Sin(sectorAngle);

                vertices.Add(x);
                vertices.Add(y);
                vertices.Add(z);
            }
        }

        for (var i = 0; i < stacks; ++i)
        {
            var k1 = i * (sectors + 1);
            var k2 = k1 + sectors + 1;

            for (var j = 0; j < sectors; ++j, ++k1, ++k2)
            {
                if (i != 0)
                {
                    indices.Add((uint)k1);
                    indices.Add((uint)k2);
                    indices.Add((uint)(k1 + 1));
                }

                if (i == (stacks - 1)) continue;

                indices.Add((uint)(k1 + 1));
                indices.Add((uint)k2);
                indices.Add((uint)(k2 + 1));
            }
        }

        Vertices = vertices.ToArray();
        Indices = indices.ToArray();
    }
}