using UnityEngine;

[ExecuteAlways]
public class Hoop : MonoBehaviour
{
    public bool upme;

    public float h;

    public float angle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (upme)
        {
            transform.position = transform.position.normalized * h;
            transform.forward = transform.position.normalized;
            transform.Rotate(transform.forward, angle, Space.World);
        }
    }
}
