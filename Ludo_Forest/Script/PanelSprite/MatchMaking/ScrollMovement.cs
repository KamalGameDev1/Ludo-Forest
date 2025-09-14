using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LudoMGP
{
    public class ScrollMovement : MonoBehaviour
    {
        private void OnEnable()
        {
            InvokeRepeating("ScrollAnim", 0.5f, 0.01f);

        }

        public ScrollRect OppScroll;
        private float scrollSpeed = 1f;

        private void ScrollAnim()
        {
            OppScroll.verticalNormalizedPosition -= scrollSpeed * Time.deltaTime;

            if (OppScroll.verticalNormalizedPosition <= 0f)
            {
                OppScroll.verticalNormalizedPosition = 1f; // Reset scroll position to 1
            }

        }
    }
}