using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SolarSystem.Classes;

public class SolarSystemApp() : GameWindow(GameWindowSettings.Default,
    new NativeWindowSettings
    {
        ClientSize = new Vector2i(800, 800), 
        Title = "Вариант 1. Солнечная система"
    })
{
    private CelestialBody[] _bodies = null!;
    private int _vao, _vbo, _ebo;
    private Shader _shader = null!;
    private readonly Sphere _sphere = new Sphere(1.0f, 36, 18);

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.1f, 0.1f, 0.2f, 1.0f); // Установка цвета

        GL.Enable(EnableCap.DepthTest); // Проверка глубины

        // Инициализация небесных тел
        var sun = new CelestialBody(2.0f, new Vector3(1.0f, 1.0f, 0.0f), 0f, 0f);
        var mercury = new CelestialBody(0.2f, new Vector3(0.7f, 0.7f, 0.7f), 4f, 4.15f);
        var venus = new CelestialBody(0.3f, new Vector3(1.0f, 0.8f, 0.5f), 6f, 1.62f);
        var earth = new CelestialBody(0.4f, new Vector3(0.0f, 0.5f, 1.0f), 8f, 1.0f);
        var moon = new CelestialBody(0.1f, new Vector3(0.8f, 0.8f, 0.8f), 0.5f, 12.0f,
            earth); // Луна вращается вокруг Земли
        var mars = new CelestialBody(0.3f, new Vector3(1.0f, 0.3f, 0.3f), 10f, 0.52f);
        var jupiter = new CelestialBody(1.0f, new Vector3(0.9f, 0.6f, 0.3f), 14f, 0.08f);
        var saturn = new CelestialBody(0.8f, new Vector3(0.9f, 0.8f, 0.5f), 18f, 0.03f);
        var uranus = new CelestialBody(0.6f, new Vector3(0.5f, 0.8f, 0.9f), 22f, 0.01f);
        var neptune = new CelestialBody(0.5f, new Vector3(0.3f, 0.5f, 0.9f), 26f, 0.006f);

        _bodies = [sun, mercury, venus, earth, moon, mars, jupiter, saturn, uranus, neptune];

        // Настройка шейдеров
        _shader = new Shader(
            // Вершинный шейдер
            @"
                #version 330 core
                layout(location = 0) in vec3 aPosition;
                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;
                void main()
                {
                    gl_Position = projection * view * model * vec4(aPosition, 1.0);
                }",
            
            // Фрагментный шейдер
            @"
                #version 330 core
                out vec4 FragColor;
                uniform vec3 color;
                void main()
                {
                    FragColor = vec4(color, 1.0);
                }"
        );

        // Настройка буферов для сферы
        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _sphere.Vertices.Length * sizeof(float), _sphere.Vertices,
            BufferUsageHint.StaticDraw);

        _ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _sphere.Indices.Length * sizeof(uint), _sphere.Indices,
            BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        
        CenterWindow();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        
        foreach (var body in _bodies)
        {
            body.Update((float)e.Time);
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
            Close();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shader.Use();

        var view = Matrix4.LookAt(new Vector3(0, 0, 30), Vector3.Zero, Vector3.UnitY);
        var projection =
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)Size.X / Size.Y, 0.1f, 100f);
        _shader.SetMatrix4("view", view);
        _shader.SetMatrix4("projection", projection);

        foreach (var body in _bodies)
        {
            var model = Matrix4.CreateScale(body.Radius) * Matrix4.CreateTranslation(body.Position);
            _shader.SetMatrix4("model", model);
            _shader.SetVector3("color", body.Color);
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _sphere.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        SwapBuffers(); // Смена текущего и следующего кадра
    }
    
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        GL.DeleteBuffer(_vbo);
        GL.DeleteBuffer(_ebo);
        GL.DeleteVertexArray(_vao);
        _shader.Dispose();
    }
}