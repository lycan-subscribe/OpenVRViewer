using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;

public class VRPointer : SteamVR_LaserPointer
{
	
    public override void OnPointerClick(PointerEventArgs e)
     {
        Debug.Log("Clicking...");
        
        if(e.target.GetComponent(typeof(ViewerPose)) is ViewerPose vp){
            if(vp != null){
                
            }
        }
     }
 
     public override void OnPointerOut(PointerEventArgs e)
     {
        Debug.Log("Exited");
     }
 
     public override void OnPointerIn(PointerEventArgs e)
     {
        Debug.Log("Entered");
     }
 }
