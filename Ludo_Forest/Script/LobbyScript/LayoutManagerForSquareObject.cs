using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LudoMGP
{


    public class LayoutManagerForSquareObject : MonoBehaviour
    {


        RectTransform rt;

        public static System.Action isPortrateLudo;
        private void OnEnable()
        {
            changeLudoGameBoardHeight();
            isPortrateLudo += changeLudoGameBoardHeight;
        }
        public void changeLudoGameBoardHeight()
        {
            StartCoroutine(waitForLudoBoardSetup());
        }

        public IEnumerator waitForLudoBoardSetup()
        {
            //MyInfo.isPortraitOrientation = true;
            yield return new WaitForSeconds(1f);

            rt = (RectTransform)this.transform;
            float width = rt.rect.width;
            float height = rt.rect.height;
            //float rectWidth =  Screen.width;

            Debug.Log("width: " + width + " and height: " + height);

            rt.sizeDelta = new Vector2(width, width);

            if (this.gameObject.GetComponent<VerticalLayoutGroup>())
            {
                this.gameObject.GetComponent<VerticalLayoutGroup>().enabled = false;
                this.gameObject.GetComponent<VerticalLayoutGroup>().enabled = true;
            }
        }

        private void OnDisable()
        {
            isPortrateLudo -= changeLudoGameBoardHeight;
        }
    }
}