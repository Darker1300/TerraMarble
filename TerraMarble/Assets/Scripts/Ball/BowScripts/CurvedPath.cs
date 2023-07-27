using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedPath : MonoBehaviour
{

    public Vector3 destination;
    public float speed = 5f;
    public float curveHeight = 2f;

    private Vector3 startPosition;
    private Vector3 controlPoint;
    private float journeyLength;
    private float startTime;

    private void Start()
    {
        startPosition = transform.position;
        controlPoint = CalculateControlPoint();
        journeyLength = Vector3.Distance(startPosition, destination);
        startTime = Time.time;
    }

    private void Update()
    {
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;
        Vector3 currentPos = Vector3.Lerp(startPosition, destination, fractionOfJourney);
        currentPos.y = Mathf.Lerp(startPosition.y, destination.y, fractionOfJourney) + Mathf.Sin(fractionOfJourney * Mathf.PI) * curveHeight;
        transform.position = currentPos;

        if (fractionOfJourney >= 1f)
        {
            // Reached the destination
            // You can add additional logic here, such as destroying the object or triggering an event
        }
    }
    public void SetDestinationAndCalculate(Vector3 dest)
    {
        destination = dest;
        startPosition = transform.position;
        controlPoint = CalculateControlPoint();
        journeyLength = Vector3.Distance(startPosition, destination);
        startTime = Time.time;


    }
    private Vector3 CalculateControlPoint()
    {
        Vector3 direction = destination - startPosition;
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0f).normalized;
        float distance = Vector3.Distance(startPosition, destination) * 0.5f;
        return startPosition + perpendicular * distance;
    }
}