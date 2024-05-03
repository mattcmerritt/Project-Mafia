using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private MeshRenderer mr;
    // [SerializeField] private List<Agent> EnemiesAlreadyHit;
    [SerializeField] private float ExpandDuration, ShrinkDuration, MaxSize;
    [SerializeField] private float RemainingExpansionTime, RemainingShrinkTime;

    // Start is called before the first frame update
    public void Start()
    {
        mr = GetComponent<MeshRenderer>();
        // EnemiesAlreadyHit = new List<Agent>();
        
        RemainingExpansionTime = ExpandDuration;
        RemainingShrinkTime = ShrinkDuration;
    }

    // Update is called once per frame
    public void Update()
    {
        if(RemainingExpansionTime > 0)
        {
            RemainingExpansionTime -= Time.deltaTime;
            transform.localScale = new Vector3(MaxSize - (MaxSize * (RemainingExpansionTime / ExpandDuration)), MaxSize - (MaxSize * (RemainingExpansionTime / ExpandDuration)), MaxSize - (MaxSize * (RemainingExpansionTime / ExpandDuration)));
        }
        else if (RemainingShrinkTime > 0)
        {
            RemainingShrinkTime -= Time.deltaTime;
            transform.localScale = new Vector3(MaxSize * (RemainingShrinkTime / ShrinkDuration), MaxSize * (RemainingShrinkTime / ShrinkDuration), MaxSize * (RemainingShrinkTime / ShrinkDuration));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SetMaterial(Material m)
    {
        mr.material = m;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("explosion hit" + collider.name);
        // if (collider != null && collider.gameObject.GetComponent<Agent>() != null && !EnemiesAlreadyHit.Contains(collider.gameObject.GetComponent<Agent>()))
        if (collider != null && collider.gameObject.GetComponent<Agent>() != null)
        {
            // TODO: determine damage from player and stats
            collider.gameObject.GetComponent<Agent>().TakeDamage(1);
            // EnemiesAlreadyHit.Add(collider.gameObject.GetComponent<Agent>());
        }
    }
}
