using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainingDummy
{
    public class TrainingDummy : Agent
    {
        public Coroutine ActiveCoroutine { get; set; }

        protected void Start()
        {
            ActiveState = new IdleState();
        }

        public IEnumerator ShowHit()
        {
            MeshRenderer rend = GetComponent<MeshRenderer>();
            Color originalColor = rend.material.color;
            rend.material.color = Color.red;
            yield return new WaitForSeconds(1);
            rend.material.color = originalColor;
        }
    }
}

