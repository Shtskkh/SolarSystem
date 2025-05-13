using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SolarSystem.Classes;

public class Shader
{
    private readonly int _program;

    public Shader(string vertexSource, string fragmentSource)
    {
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexSource);
        GL.CompileShader(vertexShader);
        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
            throw new Exception("Vertex shader compilation failed: " + GL.GetShaderInfoLog(vertexShader));

        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentSource);
        GL.CompileShader(fragmentShader);
        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
        if (success == 0)
            throw new Exception("Fragment shader compilation failed: " + GL.GetShaderInfoLog(fragmentShader));

        _program = GL.CreateProgram();
        GL.AttachShader(_program, vertexShader);
        GL.AttachShader(_program, fragmentShader);
        GL.LinkProgram(_program);
        GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out success);
        if (success == 0)
            throw new Exception("Program linking failed: " + GL.GetProgramInfoLog(_program));

        GL.DetachShader(_program, vertexShader);
        GL.DetachShader(_program, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use() => GL.UseProgram(_program);

    public void SetMatrix4(string name, Matrix4 matrix) => 
        GL.UniformMatrix4(GL.GetUniformLocation(_program, name), false, ref matrix);
    public void SetVector3(string name, Vector3 vector) => 
        GL.Uniform3(GL.GetUniformLocation(_program, name), vector);

    public void Dispose() => GL.DeleteProgram(_program);
}