using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelBarDisplay : MonoBehaviour
{

	#region Attributes
	// Enter your attributes which are used to specify this single instance of this class
	#endregion
	#region Connections
	public Transform targetTransform;
	public TextMeshProUGUI levelText;
	public Slider slider;
	
	#endregion
	#region State Variables
		// Enter your state variables used to store data in your algorithm
	#endregion
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void DisplayProgress(float progress)
    {
		//targetTransform.localScale = new Vector3(
		//	progress,
		//	targetTransform.localScale.y,
		//	targetTransform.localScale.z);
		slider.value = progress;

	}

	public void SetLevel(int levelIndex)
    {
		levelText.text = "" + (levelIndex+1);

	}

	#region Init Functions
	void InitState(){
		// Ali veli hasan hüseyin
	}
	void InitConnections(){
	}
	
	#endregion
	
	
}
