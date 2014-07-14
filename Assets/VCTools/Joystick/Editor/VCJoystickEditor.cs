using UnityEditor;
using UnityEngine;


//[CustomEditor(typeof (VCJoystick))]
public class VCJoystickEditor : Editor
{
    #region PUBLIC_METHODS
	public bool asd;

    #endregion //PUBLIC_METHODS



    #region UNITY_EDITOR_METHODS
    public void OnEnable()
    {
       
    }


    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();// LookLikeControls();
		//EditorGUILayout.LabelField("VCJoystick");
		//EditorGUILayout.InspectorTitlebar(true, new UnityEngine.Object() );
		EditorGUILayout.HelpBox ( "VCJoystick",MessageType.None);
		
		//EditorGUILayout.BeginToggleGroup( "Debug" , true );
		//EditorGUILayout.LabelField("asdasd");
		//EditorGUILayout.EndToggleGroup();
		
		
		Vector4 t = new Vector4( 
			((VCJoystick)this.target).workingArea.x , 
			((VCJoystick)this.target).workingArea.y ,
			((VCJoystick)this.target).workingArea.x + ((VCJoystick)this.target).workingArea.width , 
			((VCJoystick)this.target).workingArea.y + ((VCJoystick)this.target).workingArea.height );
		
		//EditorGUI.Vector4Field( new Rect(0f,250f,80f,30f) , "asdasd" , t );
		//EditorGUI.DrawRect( new Rect( 10 , 250 , 30 , 30 ) , Color.grey );
		
        DrawDefaultInspector();
    }
    #endregion //UNITY_EDITOR_METHODS
	
	#region PRIVATE_METHODS


    #endregion //PUBLIC_METHODS
	
}