using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSfx : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	public Sfx mouseOverSfx;
	
	public void OnPointerEnter(PointerEventData eventData)
	{
		GameManager.Instance.audioSource2D.pitch = 1 + Mathf.Sin(Time.time * 5) * 0.1f;
		if (mouseOverSfx.clip) GameManager.Instance.audioSource2D.PlayOneShot(mouseOverSfx.clip, mouseOverSfx.volumeT * Random.Range(0.7f, 0.9f));
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		
	}
}