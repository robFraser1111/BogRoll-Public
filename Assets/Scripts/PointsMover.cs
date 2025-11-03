using UnityEngine;

public class PointsMover : MonoBehaviour
{
    [SerializeField] private float speed = 2;

    [SerializeField] private int startingPoint = 1;

    [SerializeField] private Transform[] points;

    private int i;

    private void Start()
    {
        // Which element in the array to start from e.g. 1
        transform.position = points[startingPoint].position;
    }

    private void Update()
    {
        // When vector gets close to i increment
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == points.Length) i = 0;
        }

        // Move object to new point in array
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }
}