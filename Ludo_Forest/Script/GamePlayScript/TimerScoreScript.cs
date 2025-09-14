using LudoMGP;
using UnityEngine;
using UnityEngine.UI;

namespace LudoMGP
{
    public class TimerScoreScript : MonoBehaviour
    {
        public GameObject TimerObj;
        public Text scoreText;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void OnEnable()
        {
            TimerObj.transform.localEulerAngles = new Vector3(0, 0, -GameManagerLudo.instance.rotationZ);
        }

        private void OnDisable()
        {
            TimerObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            TimerObj.SetActive(false);
            scoreText.text = "0";

        }
    }
}