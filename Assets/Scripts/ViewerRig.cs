using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class ViewerRig : MonoBehaviour
{
    SceneLoader loader;
    public GameObject head;

    ViewerAvatar current_avatar;
    bool world_avatar = true;
    ViewerPose current_pose;
    ConstraintSource head_source;
    GameObject head_constraint;
    GameObject avatar_viewpoint;

    // Hacky stuff for changing the viewpoint on load
    private bool pose_changed = false;
    private int frame_count_on_pose_change = -10;


    // Awake is called before the first frame update
    void Awake(){
        DontDestroyOnLoad(this.gameObject);
        //SceneManager.sceneLoaded += OnSceneLoaded;

        loader = new SceneLoader(this);

        // Set a default avatar?
    }


    // More hacky stuff for changing the viewpoint on load

    /*void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        frame_count_on_scene_load = Time.frameCount;
    }*/

    void Update(){
        if( pose_changed ){
            if( Time.frameCount - frame_count_on_pose_change > 1 ){ // Load on animation frame 2
                ResetCameraToPose();
                pose_changed = false;

                // Then activate IK?
            }
        }
    }


    // World stuff

    public bool LoadWorldAsync(string uri){
		StartCoroutine( loader.DownloadAndLoad( uri ) );
		return true;
	}


    // Pose stuff, moving around

    public void SetTransform(Transform t, bool move_camera = true){
        if(move_camera){
            transform.position = t.transform.position;
            transform.rotation = t.transform.rotation;
            transform.localScale = t.transform.localScale;
        }

        if( current_avatar != null ){
            current_avatar.transform.position = t.transform.position;
            current_avatar.transform.rotation = t.transform.rotation;
            current_avatar.transform.localScale = t.transform.localScale;
        }
    }

    public void ChangePose(ViewerPose pose){
        current_pose = pose;

        current_avatar.SetController( pose.pose_controller );
        SetTransform(current_pose.transform, false); // Only need to set avatar pos
        pose_changed = true;
        frame_count_on_pose_change = Time.frameCount;
    }

    public void ResetCameraToPose(){
        // Move the camera to the pose viewpoint
        // So, move the origin to viewpoint + (origin - head)
        // ROTATE FIRST (IDK WHY)
        transform.rotation = Quaternion.Euler(0, avatar_viewpoint.transform.eulerAngles.y - head.transform.localEulerAngles.y, 0 );
        transform.position = avatar_viewpoint.transform.position + (transform.position - head.transform.position);
        //Debug.Log("Head Position current / target: " + head.transform.position + " " + avatar_viewpoint.transform.position );
        //Debug.Log("Head Rotation current / target: " + head.transform.eulerAngles.y + " " + avatar_viewpoint.transform.eulerAngles.y );
        //Debug.Log("Sanity check, head local transform: " + head.transform.localPosition + " " + head.transform.localEulerAngles.y );
    }


    // Avatar stuff

    // Pass with true if the user is changing to a custom avatar
    // False if the world is changing the avatar
    public void ChangeAvatar(ViewerAvatar a, bool force = false){
        if( world_avatar || force ){
            current_avatar = a;
            world_avatar = !force;

            avatar_viewpoint = new GameObject("_VIEWPOINT");
            avatar_viewpoint.transform.parent = current_avatar.transform;
            avatar_viewpoint.transform.localPosition = current_avatar.viewpoint;
            Transform head_bone = current_avatar.animator.GetBoneTransform(HumanBodyBones.Head);
            
            // Hacky solution for hiding the head bone (BREAKS DPS PARENT CONSTRAINTS)
            head_constraint = new GameObject("_HEADBONE");
            PositionConstraint pc = head_constraint.AddComponent<PositionConstraint>() as PositionConstraint;
            RotationConstraint rc = head_constraint.AddComponent<RotationConstraint>() as RotationConstraint;
            head_source.sourceTransform = head_bone;
            head_source.weight = 1;
            pc.AddSource(head_source);
            head_constraint.transform.position = head_bone.position;
            pc.constraintActive = true;
            rc.AddSource(head_source);
            head_constraint.transform.rotation = head_bone.rotation;
            rc.constraintActive = true;

            avatar_viewpoint.transform.parent = head_constraint.transform;
            head_bone.localScale = new Vector3(0,0,0);
        }
    }
}