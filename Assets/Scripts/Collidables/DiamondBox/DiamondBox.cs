using TMPro;
using UnityEngine;

namespace Collidables
{
    public class DiamondBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public int value;

        private void OnEnable()
        {
            value = Random.Range(50, 400);
            
            text.text = value.ToString();
        }
    }
}
