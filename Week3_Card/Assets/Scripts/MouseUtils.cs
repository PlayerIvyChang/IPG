using UnityEngine;

public static class MouseUtils
{
    private static Camera camera = Camera.main;
    public static Vector3 GetMouseWorldPosition(float z = 0f)
    {
        Plane plane = new(camera.transform.forward, new Vector3(0, 0, z));
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if(plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
