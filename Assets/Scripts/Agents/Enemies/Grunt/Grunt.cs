using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grunt
{
    public class Grunt : Agent
    {
        public Coroutine HitStunCoroutine { get; private set; }
        private Color originalColor;

        protected override void Start()
        {
            base.Start();

            ActiveState = new IdleState();

            MeshRenderer rend = GetComponent<MeshRenderer>();
            originalColor = rend.material.color;
        }

        // since hit detection should occur outside of specific behaviors, it is handled here
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if (other.gameObject.GetComponent<Sword>()) 
            {
                HitStunCoroutine = StartCoroutine(ActivateHitStun());
            }
        }

        public IEnumerator ActivateHitStun()
        {
            MeshRenderer rend = GetComponent<MeshRenderer>();
            rend.material.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            rend.material.color = originalColor;
        }
    }
}