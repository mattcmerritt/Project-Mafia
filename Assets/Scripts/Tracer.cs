using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracer : MonoBehaviour
{
    [SerializeField] private LineRenderer TracerLine;
    [SerializeField] private float TimeRemaining;
    [SerializeField] private Color Color;

    public void SetUp(Vector3 start, Vector3 end, Color color)
    {
        TracerLine.positionCount = 2;
        TracerLine.SetPosition(0, start);
        TracerLine.SetPosition(1, end);
        TracerLine.numCapVertices = 2;

        Color = color;
        
        TimeRemaining = 1f;
    }

    private void Update()
    {
        if(TimeRemaining > 0f)
        {
            // TracerLine.startColor = new Color(Color.r, Color.g, Color.b, TimeRemaining);
            // TracerLine.endColor = new Color(Color.r, Color.g, Color.b, TimeRemaining);

            Gradient colorGradient = new Gradient();
            colorGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color, 0f), new GradientColorKey(Color, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(TimeRemaining, 0f), new GradientAlphaKey(TimeRemaining, 1f) }
            );
            TracerLine.colorGradient = colorGradient;

            TimeRemaining -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
}
