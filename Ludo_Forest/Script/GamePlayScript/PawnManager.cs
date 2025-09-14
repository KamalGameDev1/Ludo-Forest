using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace LudoMGP
{
    public class PawnManager : MonoBehaviour
    {

        [System.Serializable]
        public class HouseData
        {
            public GameObject playerChanceHighLighter;
            public GameObject playerPawnObj;
            public GameObject TimerObj;
            public GameObject playerWinHighlighterObj;
            public Transform[] housePositions;
            public Transform[] pawnPositions;

            public List<GameObject> pawns = new List<GameObject>();
            public PathData pathData; // Include path for this house
        }

        [System.Serializable]
        public class PathData
        {
            public Transform[] fullPath;       // All 57 path tiles in Ludo            
        }

        public HouseData redHouse;
        public HouseData greenHouse;
        public HouseData yellowHouse;
        public HouseData blueHouse;

        public Dictionary<string, HouseData> houseMap;

        public static PawnManager instance;

        private void Awake()
        {
            instance = this;

            houseMap = new Dictionary<string, HouseData>
            {
                { "red", redHouse },
                { "green", greenHouse },
                { "yellow", yellowHouse },
                { "blue", blueHouse }
            };
        }
        private void Start()
        {

        }
        private void OnDisable()
        {
            foreach (var kvp in houseMap)
            {
                if (kvp.Value.playerChanceHighLighter != null)
                    kvp.Value.playerChanceHighLighter.SetActive(false);
                if (kvp.Value.playerWinHighlighterObj != null)
                    kvp.Value.playerWinHighlighterObj.SetActive(false);
            }
            WinnerPanelScript.instance.WinnerpanelObj.SetActive(false);
            GameManagerLudo.instance.matchMakingPanel.SetActive(true);
        }

        public HouseData GetHouseData(string color)
        {
            color = color.ToLower();
            return houseMap.ContainsKey(color) ? houseMap[color] : null;
        }

        public IEnumerator SpawnPawn(string color, string playerId)
        {
            HouseData house = GetHouseData(color);
            if (house == null)
            {
                Debug.LogWarning($"SpawnPawn failed: Unknown color '{color}'");
                yield return null;
            }

            house.playerPawnObj.SetActive(true);
            List<Transform> fullPath = new List<Transform>(house.pathData.fullPath);
            if (GameManagerLudo.game_type == "timer")
            {
                house.TimerObj.SetActive(true);
            }
            else
            {
                house.TimerObj.SetActive(false);
            }
            house.playerWinHighlighterObj.SetActive(false);


            if (house.pawns.Count == 0)
            {
                Debug.LogWarning($"{color} house has no pawns to spawn.");
                yield return null;
            }

            for (int i = 0; i < house.pawns.Count; i++)
            {
                GameObject pawn = house.pawns[i];
                if (pawn == null) continue;

                pawn.SetActive(true);

                PawnScript pawnScript = pawn.GetComponent<PawnScript>();
                if (pawnScript != null)
                {
                    pawnScript.SetColorAndNum(color, i);
                    pawnScript.playerId = playerId;
                }

                if (GameManagerLudo.game_type == "timer")
                {
                    Transform targetSlot = fullPath[0].GetChild(0);
                    pawn.transform.SetParent(targetSlot, false);
                    pawn.transform.DOMove(targetSlot.position, 0.2f).OnComplete(() =>
                    {
                        Debug.Log("pawn Created");
                    });

                    yield return new WaitForSeconds(0.05f);
                    if (pawnScript != null)
                    {
                        pawnScript.currentBoardPosition = 1;
                        //ApplyOffsetToPawns(1, targetSlot.position);
                    }

                }
                else if (GameManagerLudo.game_type == "classic")
                {
                    if (i < house.housePositions.Length && house.housePositions[i] != null)
                    {
                        Transform startPos = house.housePositions[i];
                        Transform pawnPos = house.pawnPositions[i];

                        pawn.transform.SetParent(startPos, false);
                        pawn.transform.DOMove(pawnPos.position, 0.2f).OnComplete(() =>
                        {
                            Debug.Log("pawn Created");
                        });
                       

                        if (pawnScript != null)
                            pawnScript.currentBoardPosition = 0; // Pawn is inside house
                    }
                    else
                    {
                        Debug.LogWarning($"housePositions missing for index {i} in {color} house.");
                    }
                }

                // Store its home position for respawn if killed
                if (i < house.pawnPositions.Length && house.pawnPositions[i] != null)
                {
                    // Optional: Set home info to the pawn script if needed
                    pawnScript.homeTransform = house.pawnPositions[i];
                }
                else
                {
                    Debug.LogWarning($"pawnPositions missing for index {i} in {color} house.");
                }
            }
            StartCoroutine(SetLayout(color));
            Debug.Log($"{color} pawns spawned.");
        }


        public IEnumerator SetLayout(string color)
        {
            HouseData house = GetHouseData(color);
            List<Transform> fullPath = new List<Transform>(house.pathData.fullPath);
            if (GameManagerLudo.game_type == "timer")
            {
                Transform layoutTarget = fullPath[0].GetChild(0);
                layoutTarget.GetComponent<VerticalLayoutGroup>().enabled = false;
                yield return new WaitForSeconds(0.5f);
                layoutTarget.GetComponent<VerticalLayoutGroup>().enabled = true;

            }


            Debug.Log($"player {color} Set Layout");

        }


        public void SetPlayerScore(string color, string score)
        {
            HouseData house = GetHouseData(color);
            if (house == null)
            {
                Debug.LogWarning($"player Score failed: Unknown color '{color}'");
                return;
            }

            //if (house.TimerObj != null)                
            //    house.TimerObj.SetActive(true);

            house.TimerObj.GetComponent<TimerScoreScript>().scoreText.text = score;
            Debug.Log($"{color} player score is set");

        }

        public void ChanceHighLighter(string color)
        {
            foreach (var kvp in houseMap)
            {
                if (kvp.Value.playerChanceHighLighter != null)
                    kvp.Value.playerChanceHighLighter.SetActive(false);
            }

            HouseData house = GetHouseData(color);
            if (house == null)
            {
                Debug.LogWarning($"ChanceHighLighter failed: Unknown color '{color}'");
                return;
            }

            if (house.playerChanceHighLighter != null)
                house.playerChanceHighLighter.SetActive(true);
        }

        public void SetWinAnimation(string color)
        {
            foreach (var kvp in houseMap)
            {
                if (kvp.Value.playerWinHighlighterObj != null)
                    kvp.Value.playerWinHighlighterObj.SetActive(false);
            }

            HouseData house = GetHouseData(color);
            if (house == null)
            {
                Debug.LogWarning($"ChanceHighLighter failed: Unknown color '{color}'");
                return;
            }

            if (house.playerWinHighlighterObj != null)
                house.playerWinHighlighterObj.SetActive(true);
        }
        void DeselectableSelectablePawn(HouseData house)
        {
            foreach (GameObject pawnObj in house.pawns)
            {
                if (pawnObj != null)
                {
                    var script = pawnObj.GetComponent<PawnScript>();
                    script.SetSelectable(false);
                }
            }
        }
        public void HandleEligiblePawns(List<EligiblePawn> eligiblePawns, string color, int diceNumber, bool key)
        {
            HouseData house = GetHouseData(color);
            if (house == null)
            {
                Debug.LogWarning($"HandleEligiblePawns failed: Unknown color '{color}'");
                return;
            }

            // Reset all pawns
            foreach (GameObject pawnObj in house.pawns)
            {
                if (pawnObj != null)
                {
                    var script = pawnObj.GetComponent<PawnScript>();
                    script.SetSelectable(false);
                }
            }

            // Set only eligible pawns
            foreach (EligiblePawn ep in eligiblePawns)
            {
                if (ep.pawn_number < 0 || ep.pawn_number >= house.pawns.Count)
                {
                    Debug.LogWarning($"Invalid pawn number: {ep.pawn_number}");
                    continue;
                }

                GameObject pawnObj = house.pawns[ep.pawn_number];
                if (pawnObj != null)
                {
                    var script = pawnObj.GetComponent<PawnScript>();
                    script.SetSelectionData(diceNumber, key);
                    script.SetSelectable(true);
                    //script.StartSelectedAnim(); // optional visual feedback
                }
            }

            Debug.Log($"{eligiblePawns.Count} eligible pawns highlighted for color {color}");
        }       

        public void MovePawn(string color, int pawnNumber, int currentPos, int newPos)
        {

            HouseData house = GetHouseData(color);
            if (house == null)
            {
                Debug.Log($"MovePawn failed: Unknown color '{color}'");
                return;
            }
            
            if (pawnNumber < 0 || pawnNumber >= house.pawns.Count)
            {
                Debug.Log($"Invalid pawn number {pawnNumber} for color {color}");
                return;
            }
           
            GameObject pawn = house.pawns[pawnNumber];
            Transform pawnPosition = house.pawnPositions[pawnNumber];
            Transform pawnHomePosition = house.housePositions[pawnNumber];

            if (pawn == null)
            {
                Debug.Log("Pawn GameObject not found.");
                return;
            }
           

            List<Transform> fullPath = new List<Transform>(house.pathData.fullPath);

            List<int> colorPath = GetStartIndexForColor(color);           

            Debug.Log("Pawn movement start" + newPos);

            if (newPos == 0)
            {
                int start = colorPath.IndexOf(currentPos);
                // Send pawn back to its house (killed animation)
                List<Vector3> reversePath = new List<Vector3>();
                for (int i = start; i >= 0; i--)
                {
                    reversePath.Add(fullPath[i].position);
                }

                pawn.transform.DOPath(reversePath.ToArray(), reversePath.Count * 0.2f, PathType.Linear)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        Transform targetSlot = fullPath[0].GetChild(0);
                        pawn.transform.SetParent(targetSlot, false);
                       
                        if (GameManagerLudo.game_type == "classic")
                        {
                            Transform startPos = pawnPosition;
                            Transform pawnPos = pawnHomePosition;
                            
                            pawn.transform.DOMove(pawnPos.position, 0.2f).OnComplete(() =>
                            {
                                Debug.Log("pawn Created");
                                pawn.transform.SetParent(startPos, false);

                            });
                        }

                    });
            }
            else
            {
                int start = 0;
                int end = 0;
                int safe = 0;
                bool _safezone = false;

                if (colorPath.Contains(currentPos))
                {
                    start = colorPath.IndexOf(currentPos);
                }

                if (colorPath.Contains(newPos))
                {
                    end = colorPath.IndexOf(newPos);
                }

                if (SafeZoneList.Contains(newPos))
                {
                    safe = SafeZoneList.IndexOf(newPos);
                    _safezone = true;
                }
                Debug.Log("Start-" + start + "end-" + end);
                List<Vector3> forwardPath = new List<Vector3>();

                for (int i = start; i <= end; i++)
                {
                    forwardPath.Add(fullPath[i].position);
                }
                DeselectableSelectablePawn(house);
                pawn.transform.DOPath(forwardPath.ToArray(), forwardPath.Count * 0.2f, PathType.Linear).SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    pawn.transform.SetParent(fullPath[end].GetChild(0), false);
                    pawn.GetComponent<PawnScript>().currentBoardPosition = newPos;
                    DeselectableSelectablePawn(house);
                    Vector3 targetTilePos = fullPath[colorPath.IndexOf(newPos)].position;
                    //ApplyOffsetToPawns(newPos, targetTilePos);
                    Debug.Log("Movement Complete");
                });
            }
            Debug.Log("Pawn Movement");
        }

        private List<int> GetStartIndexForColor(string color)
        {
            switch (color.ToLower())
            {
                case "red": return red;
                case "green": return green;
                case "yellow": return yellow;
                case "blue": return blue;
                default: return null;
            }
        }
        List<int> red = new List<int>()
        {
           1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
           31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51,101, 102, 103, 104, 105, 106
        };
        List<int> green = new List<int>()
        {
            14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41,
            42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 201, 202, 203, 204, 205, 206
        };
        List<int> yellow = new List<int>()
        {
            27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 1, 2,
            3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25,301, 302, 303, 304, 305, 306
        };
        List<int> blue = new List<int>()
        {
            40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17,
            18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 401, 402, 403, 404, 405, 406
        };

        List<int> SafeZoneList = new List<int>()
        {
            1, 9, 14, 22, 27, 35, 40, 48
        };

        public void ApplyOffsetToPawns(int boardPosition, Vector3 basePosition)
        {
            List<GameObject> overlappingPawns = new List<GameObject>();

            foreach (var kvp in houseMap)
            {
                foreach (var pawn in kvp.Value.pawns)
                {
                    if (pawn != null)
                    {
                        var script = pawn.GetComponent<PawnScript>();
                        if (script != null && script.currentBoardPosition == boardPosition)
                        {
                            overlappingPawns.Add(pawn);
                        }
                    }
                }
            }

            int count = overlappingPawns.Count;
            if (count == 0) return;

            float radius = 20f; // Adjust this value for spacing
            float scaleFactor = 1f;

            if (count == 1)
            {
                overlappingPawns[0].transform.position = basePosition;
                overlappingPawns[0].transform.localScale = Vector3.one;
                return;
            }

            if (count == 2) scaleFactor = 0.9f;
            else if (count == 3) scaleFactor = 0.8f;
            else if (count == 4) scaleFactor = 0.7f;
            else scaleFactor = 0.6f;

            for (int i = 0; i < count; i++)
            {
                float angle = i * (360f / count);
                Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
                overlappingPawns[i].transform.position = basePosition + offset;
                overlappingPawns[i].transform.localScale = Vector3.one * scaleFactor;
            }
        }

        public void ResetAll()
        {
            foreach (var kvp in houseMap)
            {
                if (kvp.Value.playerChanceHighLighter != null)
                    kvp.Value.playerChanceHighLighter.SetActive(false);
                if (kvp.Value.playerWinHighlighterObj != null)
                    kvp.Value.playerWinHighlighterObj.SetActive(false);
            }
            WinnerPanelScript.instance.WinnerpanelObj.SetActive(false);

        }
    }

}