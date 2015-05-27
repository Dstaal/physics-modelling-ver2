using UnityEngine;
public class PhaseSpace : UnityEngine.Object
{
    public float x { get; set; }

    public float y { get; set; }

    public float z { get; set; }

    public float x_v { get; set; }

    public float y_v { get; set; }

    public float z_v { get; set; }

    public PhaseSpace()
    {
        this.x = 0f;
        this.y = 0f;
        this.z = 0f;

        this.x_v = 0f;
        this.y_v = 0f;
        this.z_v = 0f;
    }

    public PhaseSpace(float x, float y, float z, float x_v, float y_v, float z_v)
    {
        this.x = x;
        this.y = y;
        this.z = z;

        this.x_v = x_v;
        this.y_v = y_v;
        this.z_v = z_v;
    }

    public PhaseSpace(Vector3 position, Vector3 velocity)
    {
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;

        this.x_v = velocity.x;
        this.x_y = velocity.y;
        this.x_z = velocity.z;
    }

    public override string ToString()
    {
        return "(" + this.x + ", " + this.y + ", " + this.z + "), (" + this.x_v + ", " + this.y_v + ", " + this.z_v + ")";
    }
}