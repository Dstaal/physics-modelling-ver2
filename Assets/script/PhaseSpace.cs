public class PhaseSpace : UnityEngine.Object
{
    public float x { get; set; }

    public float y { get; set; }

    public float z { get; set; }

    public float dx { get; set; }

    public float dy { get; set; }

    public float dz { get; set; }

    public PhaseSpace()
    {
        this.x = 0f;
        this.y = 0f;
        this.z = 0f;

        this.dx = 0f;
        this.dy = 0f;
        this.dz = 0f;
    }

}