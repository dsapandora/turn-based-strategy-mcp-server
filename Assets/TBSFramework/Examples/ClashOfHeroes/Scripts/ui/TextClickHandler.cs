using UnityEngine;
using UnityEngine.EventSystems;

namespace TurnBasedStrategyFramework.Unity.Examples.ClashOfHeroes.UI
{
    public class TextClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string _url;
        public void OnPointerClick(PointerEventData eventData)
        {
            Application.OpenURL(_url);
        }
    }
}