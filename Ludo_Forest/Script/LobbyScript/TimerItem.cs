using UnityEngine;
using UnityEngine.UI;

namespace LudoMGP
{
    public class TimerItem : MonoBehaviour
    {
        public string TableName;
        public string gameVariant;
        public string gametype;
        public string TableId;
        public string maxplayerCount;
        public Text entryValueText;
        public GameObject prizePool1;
        public GameObject prizePool2;
        public Text TimerText;
        public Image maxplayerCountImage;
      

        public void CallItemTable()
        {
            LudoModesTableList.instance.classicTable.SetActive(false);
            LudoModesTableList.instance.timerTable.SetActive(true);
            LudoModesTableList.instance.CallTableApiData(TableId);
            Debug.Log("Timer item Table Called");
        }
    }
}