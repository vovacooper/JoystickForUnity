/**********************************************************************************
 * 	VC Joystick
 * 	
 **********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;



/// <summary>
/// VC joystick.
/// </summary>
public class VCJoystick : MonoBehaviour{ 

	private static Dictionary<string , VCJoystick> joystics = new Dictionary<string, VCJoystick>();
	static public VCJoystick GetJoystick(string joysticName)
	{
		return joystics[joysticName];
	}


	/// <summary>
	/// The name of the joystick.
	/// </summary>
	public string JoystickName;
	
	public enum JoystickType{
		Binary,
		Float
	}
	public JoystickType joystickType;
	
	public enum PositionType{
		Specific,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight,
		Middle
	}
	public PositionType positionType = PositionType.Specific;
	public Rect workingArea;
	
	public Texture backgrowndTexture;
	public Vector2 backgrowndTextureSize;
	
	public Texture joystickTexture;
	public Vector2 joystickTextureSize;

	/// <summary>
	/// The position.
	/// in float mod will be [-1,1] for x and y axis where 
	/// -1 represent left and bottom 
	/// 1 represent right and top
	/// </summary>
    public Vector2 position;                                // [-1, 1] in x,y
    public int tapCount;                                    // Current tap count
	
	public float joystickRadius = 25f;
	
	/// <summary>
	/// is in debug mode?.
	/// </summary>
	public bool isDebug = true;
	
	
	private int screenWidth = Screen.width;
	private int screenHeight = Screen.height;
	
	void Update () {
		if ( this.screenWidth != Screen.width || this.screenHeight != Screen.height ) {
	        this.screenWidth = Screen.width;
	        this.screenHeight = Screen.height;
	    }
	}
	
	void DrawQuad(Rect position, Color color) {
		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0,0,color);
		texture.Apply();
		GUI.skin.box.normal.background = texture;
		GUI.Box(position, GUIContent.none);
	}

	
    public void Start(){
		if(JoystickName == ""){
			JoystickName = Guid.NewGuid().ToString();
		}
		joystics.Add(JoystickName , this);
		
		if( workingArea == new Rect(0f,0f,0f,0f) ){
			Debug.LogWarning( "Working Area is zero. " );	
		}
		switch(positionType){
		case PositionType.BottomLeft:
			workingArea = new Rect(0f , screenHeight - workingArea.height , workingArea.width , workingArea.height);
			break;
		case PositionType.BottomRight:
			workingArea = new Rect(screenWidth - workingArea.width , screenHeight - workingArea.height , workingArea.width , workingArea.height);
			break;
		case PositionType.Middle:
			workingArea = new Rect(screenWidth/2 - workingArea.width/2 , screenHeight/2 - workingArea.height/2  , workingArea.width , workingArea.height);
			break;
		case PositionType.TopLeft:
			workingArea = new Rect(0f , 0f , workingArea.width , workingArea.height);
			break;
		case PositionType.TopRight:
			workingArea = new Rect( screenWidth - workingArea.width , 0f , workingArea.width , workingArea.height);
			break;
		case PositionType.Specific:
			break;	
		}

    }
	
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER
	private Vector3 initMousePosition ;
	private bool _IsInitiated = false;
#else
	private Vector2 initTouchPosition ;
	private bool _IsInitiated = false;
	private int _fingerID = -1;
#endif
	
    public void OnGUI(){   
		if(isDebug){
			Color c =  Color.blue;
			c.a = 0.2f;
			DrawQuad(workingArea , c);
		}
		
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER

		if( _IsInitiated == false && !workingArea.Contains( new Vector2( Input.mousePosition.x ,screenHeight - Input.mousePosition.y) ) ){
			return;
		}

		if( Input.GetMouseButtonDown(0) ){
			initMousePosition = Input.mousePosition;
			initMousePosition.y = screenHeight - initMousePosition.y; //FIX
			_IsInitiated = true;
		}
		if( _IsInitiated == true && Input.GetMouseButton(0) ){
			Vector3 mouseNewPosition = Input.mousePosition;
			mouseNewPosition.y = screenHeight - mouseNewPosition.y;
			
			GUI.DrawTexture(  
				new Rect( initMousePosition.x - backgrowndTextureSize.x/2 ,
						  initMousePosition.y - backgrowndTextureSize.y/2 ,
						  backgrowndTextureSize.x ,
						  backgrowndTextureSize.y ) 
				, backgrowndTexture , ScaleMode.ScaleAndCrop , true );
			
			Vector3 joyPos;
			Vector2 newPos;
			if( (initMousePosition - mouseNewPosition).magnitude < joystickRadius ){
				joyPos = mouseNewPosition;
				newPos = ( mouseNewPosition - initMousePosition )/joystickRadius;
				if(isPosChanged(newPos)){
					position = newPos;
				}
			}else{
				joyPos = initMousePosition +  ( mouseNewPosition - initMousePosition) / ( mouseNewPosition - initMousePosition ).magnitude * joystickRadius;
				newPos = ( mouseNewPosition - initMousePosition) / (( mouseNewPosition - initMousePosition ).magnitude);
			}
			if( isPosChanged(newPos) ){
				position = newPos;
		

			}
			GUI.DrawTexture(  
				new Rect( joyPos.x - joystickTextureSize.x/2 ,
						  joyPos.y - joystickTextureSize.y/2 ,
						  joystickTextureSize.x ,
						  joystickTextureSize.y ) 
				, joystickTexture , ScaleMode.ScaleAndCrop , true );
		}
		if( Input.GetMouseButtonUp(0) ){
			_IsInitiated = false;
		}
#else 
	foreach( Touch t in Input.touches){
			
		Vector2 touchPos = t.position;
		touchPos.y = screenHeight - touchPos.y; //FIX
			
		if( _IsInitiated == false ){
			if(!workingArea.Contains( touchPos ) ){
				return;
			}
			initTouchPosition = touchPos;
			_IsInitiated = true;
			_fingerID = t.fingerId;
		}
		if( _fingerID == t.fingerId && _IsInitiated == true && ( t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary ) ){
			Vector2 touchNewPosition = touchPos;
				
			GUI.DrawTexture(  
				new Rect( initTouchPosition.x - backgrowndTextureSize.x/2 ,
						  initTouchPosition.y - backgrowndTextureSize.y/2 ,
						  backgrowndTextureSize.x ,
						  backgrowndTextureSize.y ) 
				, backgrowndTexture , ScaleMode.ScaleAndCrop , true );
			
			Vector3 joyPos;
			Vector2 newPos;
			if( (initTouchPosition - touchNewPosition).magnitude < joystickRadius ){
				joyPos = touchNewPosition;
				newPos = ( touchNewPosition - initTouchPosition )/joystickRadius;
				if(isPosChanged(newPos)){
					position = newPos;
				}
			}else{
				joyPos = initTouchPosition +  ( touchNewPosition - initTouchPosition) / ( touchNewPosition - initTouchPosition ).magnitude * joystickRadius;
				newPos = ( touchNewPosition - initTouchPosition) / (( touchNewPosition - initTouchPosition ).magnitude);
			}
			if(isPosChanged(newPos)){
				position = newPos;
				
				
			}
			GUI.DrawTexture(  
				new Rect( joyPos.x - joystickTextureSize.x/2 ,
						  joyPos.y - joystickTextureSize.y/2 ,
						  joystickTextureSize.x ,
						  joystickTextureSize.y ) 
				, joystickTexture , ScaleMode.ScaleAndCrop , true );
		}
		if(  _fingerID == t.fingerId && t.phase == TouchPhase.Ended ){
			_IsInitiated = false;
		}	
	}
#endif
	}
	
	private float MIN_DELTA = 0.0000005f;
	bool isPosChanged( Vector2 newPos ){
		return true;//Mathf.Abs( position.magnitude - newPos.magnitude) > MIN_DELTA;
	}
}