using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainingDummy
{
    public class TrainingDummy : Agent
    {
        public Coroutine ActiveCoroutine { get; set; }

        private Color originalColor;

        protected void Start()
        {
            ActiveState = new IdleState();

            MeshRenderer rend = GetComponent<MeshRenderer>();
            originalColor = rend.material.color;
        }

        public IEnumerator ShowHit()
        {
            MeshRenderer rend = GetComponent<MeshRenderer>();
            rend.material.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            rend.material.color = originalColor;
        }
    }
}

