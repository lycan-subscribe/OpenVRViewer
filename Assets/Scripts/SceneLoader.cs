using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SceneLoader
{
	public Scene current_scene;
	private ViewerScene world_component;

	private ViewerRig parent;
	//private Transform default_spawn = new GameObject().transform;

	public SceneLoader(ViewerRig parent_t){
		parent = parent_t;
	}
	
    public IEnumerator DownloadAndLoad(string uri){
		
		
		// We want https, not x-world
		try{
			uri = "https:" + uri.Split(char.Parse(":"))[1];
		}
		catch(Exception e){
			UnityEngine.Debug.Log(e);
			yield break;
		}
		
		
		// Download World (if needed)
        Debug.Log("Getting asset bundle...");
		var uwr = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET);
		uwr.downloadHandler = new DownloadHandlerAssetBundle(uri, 0); // add checksums?
        yield return uwr.SendWebRequest();
		Debug.Log( "Response code: " + uwr.responseCode );
		Debug.Log( "Downloaded bytes: " + uwr.downloadedBytes );
		Debug.Log( string.Join("; ", uwr.GetResponseHeaders().Select(x => x.Key + "=" + x.Value).ToArray()) );
		
		if (uwr.isNetworkError) {
            Debug.Log(uwr.error);
        }
		
		
		// Get the asset bundle; not sure what this means, files might get overwritten?
		AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
        string[] scenePaths = bundle.GetAllScenePaths();
		if( scenePaths.Length < 1 ){
			Debug.Log("No scene found in asset bundle. Cancelled loading.");
			yield break;
		}
		
		
		// Filter scene, what components are dangerous? Scripts don't seem to be allowed? (Does nothing yet)
		WorldFilter.FilterWorldFile(scenePaths[0]);
		
		
		// Destroy / deload previous asset bundle ? untested
		if( current_scene.IsValid() ){
			yield return SceneManager.UnloadSceneAsync(current_scene);
		}
		
		
		// Load scene completely
		Debug.Log("Retrieved asset bundle. Loading world " + scenePaths[0] + " ...");
		yield return SceneManager.LoadSceneAsync(scenePaths[0]);
		current_scene = SceneManager.GetSceneByPath(scenePaths[0]);
		Debug.Log("World loaded: " + scenePaths[0]);
		
		
		OnWorldLoad();
	}
	
	private void OnWorldLoad(){
		
		// Process and teleport to spawn
		foreach(var obj in current_scene.GetRootGameObjects() ){
			if( obj.GetComponent(typeof(ViewerScene)) is ViewerScene vw ){
				if(vw != null){
					world_component = vw;
					parent.ChangeAvatar( world_component.avatar );
				}
			}
			else if( obj.GetComponent(typeof(AudioListener)) is AudioListener al ){
				if( al != null )
					al.enabled = false;
			}
		}
		
		if( world_component == null ){
			Debug.Log("No world component found. Teleporting to (0,0,0)");
			//parent.SetTransform(default_spawn); //Untested, might break
		}
		else if( world_component.poses.Length > 0 ){
			// Start them off in the default pose
			Debug.Log("Spawning in pose 0...");
			parent.ChangePose(world_component.poses[0]);
		}
		else if( world_component.spawn != null ){
			Debug.Log("No poses found, moving to spawn point...");
			parent.SetTransform(world_component.spawn.transform);
		}
		else{
			Debug.Log("No spawn point found. Teleporting to (0,0,0)");
			//parent.SetTransform(default_spawn); //Untested, might break
		}
	}
}
