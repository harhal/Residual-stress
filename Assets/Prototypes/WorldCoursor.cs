using UnityEngine;

public class WorldCoursor : MonoBehaviour
{
    Vector3 Location;
    Ray Ray;

    public void SetLocation(Vector3 location, Ray ray)
    {
        Location = location;
        Ray = ray;
        transform.SetPositionAndRotation(location, Quaternion.identity);
    }

    public Vector3 GetLocation()
    {
        return Location;
    }

    public Ray GetRay()
    {
        return Ray;
    }
}
