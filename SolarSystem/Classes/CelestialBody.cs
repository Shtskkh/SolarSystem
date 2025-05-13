using OpenTK.Mathematics;

namespace SolarSystem.Classes;

public class CelestialBody
{
    public Vector3 Position { get; set; }
    public float Radius { get; private set; }
    public Vector3 Color { get; private set; }
    public float OrbitRadius { get; private set; }
    public float OrbitSpeed { get; private set; }
    public float Angle { get; set; }
    public CelestialBody? Parent { get; set; } = null; // Для спутников

    public CelestialBody(float radius, Vector3 color, float orbitRadius, float orbitSpeed, CelestialBody? parent = null)
    {
        Radius = radius;
        Color = color;
        OrbitRadius = orbitRadius;
        OrbitSpeed = orbitSpeed;
        Angle = 0f;
        Parent = parent;
        UpdatePosition();
    }

    public void Update(float deltaTime)
    {
        Angle += OrbitSpeed * deltaTime;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        var orbitCenter = Parent?.Position ?? Vector3.Zero;
        Position = orbitCenter + new Vector3(
            OrbitRadius * (float)Math.Cos(Angle),
            OrbitRadius * (float)Math.Sin(Angle),
            0f
        );
    }
}