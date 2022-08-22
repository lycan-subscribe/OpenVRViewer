using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;

public class VRPointer : SteamVR_LaserPointer
{
	
    public override void OnPointerClick(PointerEventArgs e)
     {
         IPointerClickHandler clickHandler = e.target.GetComponent<IPointerClickHandler>();
         if (clickHandler == null)
         {
             return;
         }
 
		//Debug.Log("Clicking...");
         clickHandler.OnPointerClick(new PointerEventData(EventSystem.current));
     }
 
     public override void OnPointerOut(PointerEventArgs e)
     {
         IPointerExitHandler pointerExitHandler = e.target.GetComponent<IPointerExitHandler>();
         if (pointerExitHandler == null)
         {
             return;
         }
 
		//Debug.Log("Exited");
         pointerExitHandler.OnPointerExit(new PointerEventData(EventSystem.current));
     }
 
     public override void OnPointerIn(PointerEventArgs e)
     {
         IPointerEnterHandler pointerEnterHandler = e.target.GetComponent<IPointerEnterHandler>();
         if (pointerEnterHandler == null)
         {
             return;
         }
 
		//Debug.Log("Entered");
         pointerEnterHandler.OnPointerEnter(new PointerEventData(EventSystem.current));
     }
 }
