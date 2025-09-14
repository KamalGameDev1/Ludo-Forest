using LudoMGP;
using UnityEngine;

public class PlayerWinningAnimation : MonoBehaviour
{
    public GameObject winObj;
    private void OnEnable()
    {
        winObj.transform.localEulerAngles = new Vector3(0, 0, -GameManagerLudo.instance.rotationZ);
    }

    private void OnDisable()
    {
        winObj.transform.localEulerAngles = new Vector3(0, 0, 0);
        winObj.SetActive(false);
    }
}
