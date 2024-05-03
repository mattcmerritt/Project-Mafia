using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent unityEvent;
    [SerializeField] private bool triggerOnce;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 7) { return; }
        else
        {
            unityEvent?.Invoke();
            if (triggerOnce) { this.enabled = false; }
        }
    }
}
