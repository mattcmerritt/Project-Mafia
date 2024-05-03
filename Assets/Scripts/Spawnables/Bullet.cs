using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private float initialForce;
    [SerializeField] private float lifespan;

    // Start is called before the first frame update
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    public void Update()
    {
        lr.SetPositions(new Vector3[] {transform.position, transform.position - (transform.forward * 2)});
        lifespan -= Time.deltaTime;
        if(lifespan < 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetTrail(Gradient g)
    {
        lr.colorGradient = g;
    }

    public void FireAt(Vector3 target)
    {
        transform.LookAt(target);
        rb.AddForce(transform.forward * initialForce);
    }

    private void OnTriggerEnter(Collider collider) 
    {
        if (collider != null && collider.gameObject.GetComponent<Agent>() != null)
        {
            // TODO: determine damage from player and stats
            collider.gameObject.GetComponent<Agent>().TakeDamage(1);
        }
        if (collider != null && collider.gameObject.layer != 7) // 7 = player
        {
            Debug.Log(collider.gameObject.layer);
            Destroy(this.gameObject); // destroy if not hitting self
        }
        
    }
}
