using System;
using System.Diagnostics;
using UnityEngine;

class OnLaunch
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void OnAssembliesLoad()
    {
		// Check if already running (WINDOWS SPECIFIC?)
		// This doesn't work because it finds the current process lol
		/*Process currentProcess = Process.GetCurrentProcess();
		Process[] runningProcesses = Process.GetProcessesByName(currentProcess.ProcessName);
		if( runningProcesses.Length > 0 ){
			UnityEngine.Debug.Log( "Other process already running with pid " + runningProcesses[0].Id );
		}*/
		
        UnityEngine.Debug.Log("Assemblies Loaded");
    }
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static void AfterSceneLoad(){
		// Check if there's a URI in the commandline args
		try {
			string uri = GetArg("-uri");
			if(uri != null){
				string uri_https = uri.Replace("x-world://", "https://");
				
				UnityEngine.Debug.Log("URI provided in args, opening " + uri_https);
				WorldLoader[] wl = UnityEngine.Object.FindObjectsOfType(typeof(WorldLoader)) as WorldLoader[];
				wl[0].LoadWorldAsync(uri_https);
			}
			else{
				// DEBUG SHIT
				
				WorldLoader[] wl = UnityEngine.Object.FindObjectsOfType(typeof(WorldLoader)) as WorldLoader[];
				wl[0].LoadWorldAsync("https://public-vr-test-scenes.s3.us-west-2.amazonaws.com/samplescene.xworld");
			}
		}
		catch (Exception e){
			UnityEngine.Debug.Log(e);
		}
	}
	
	private static string GetArg(string name){
		string[] args = System.Environment.GetCommandLineArgs();
		
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i] == name && args.Length > i + 1)
			{
				return args[i + 1];
			}
		}
		return null;
	}

}