using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracer : MonoBehaviour
{
    [SerializeField] private LineRenderer TracerLine;
    [SerializeField] private float TimeRemaining;
    [SerializeField] private Gradient OriginalGradient;

    public void SetUp(Vector3 start, Vector3 end, Gradient gradient)
    {
        TracerLine.positionCount = 2;
        TracerLine.SetPosition(0, start);
        TracerLine.SetPosition(1, end);
        TracerLine.numCapVertices = 2;

        OriginalGradient = gradient;
        
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
                OriginalGradient.colorKeys,
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
