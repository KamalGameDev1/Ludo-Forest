using DG.Tweening;
using LudoMGP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LudoMGP
{
    public class PawnScript : MonoBehaviour
    {
        public bool InHouse;
        public GameObject pawnObj;
        public Button pawnbtn;

        public int pawnNo;
        public string pawnColor;
        public Transform homeTransform;

        // For selection logic
        private bool isSelectable = false;
        private int diceNumber;
        private bool keyValue;
        public string playerId;

        private readonly HashSet<int> safePositions = new HashSet<int>()
        {
             1, 9, 14, 22, 27, 35, 40, 48 // example Ludo safe tiles (adjust as per your path)
        };
        public int currentBoardPosition = -1; // updated after every move

        private void OnEnable()
        {
            pawnObj.transform.localEulerAngles = new Vector3(0, 0, -GameManagerLudo.instance.rotationZ);
        }

        private void OnDisable()
        {
            pawnObj.transform.localEulerAngles = new Vector3(0, 0, 0);
            pawnObj.SetActive(false);
        }

        private void Start()
        {
            if (playerId == GameManagerLudo.playerID)
            {
                pawnbtn.onClick.AddListener(OnPawnClicked);
            }
        }

        public void StartSelectedAnim()
        {

        }

        public void SetColorAndNum(string color, int i)
        {
            pawnColor = color.ToLower();
            pawnNo = i;
        }

        // Called during eligible pawn selection
        public void SetSelectable(bool selectable)
        {
                                                                 // or:
            this.isSelectable = selectable;
            pawnbtn.interactable = selectable;
            pawnbtn.enabled = isSelectable;
           
        }

        public void SetSelectionData(int diceNo, bool key)
        {
            this.diceNumber = diceNo;
            this.keyValue = key;
        }

        private void OnPawnClicked()
        {

            if (!isSelectable)
            {
                Debug.Log("Pawn not selected");
                return;
            }
              
            
            SetSelectable(false); // Prevent duplicate click
            Debug.Log($"pawn color :{pawnColor}//pawn no.{pawnNo} ");
            // Emit selected pawn
            LudoSocketManager.instance.SelectedPawn(pawnNo, pawnColor, diceNumber, keyValue.ToString());
            diceNumber = 0;
            Debug.Log($"<color=cyan>Pawn clicked: {pawnColor} #{pawnNo}, Dice: {diceNumber}, Key: {keyValue}</color>");
        }
        public void ResetPawn(Vector3 homePos)
        {
            currentBoardPosition = 0;
            transform.position = homePos;
            transform.localScale = Vector3.one;
        }

    }
}
