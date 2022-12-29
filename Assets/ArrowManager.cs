using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ArrowManager : MonoBehaviour
{
    //Settings
    public List<TaskManager> taskList;
    float radius;
    // Connections
    public Transform arrow;
    // State Variables
    public int taskIndex = 0;
    bool active = true;
    // Start is called before the first frame update
	void Awake()
	{
		//InitConnections();
	}
    void Start()
    {
        InitState();
    }
    void InitConnections(){
    }
    void InitState()
    {
        radius = Vector3.Distance(transform.position, arrow.position);

        taskIndex = PlayerPrefs.GetInt(nameof(taskIndex), 0);
        if(PlayerPrefs.GetInt("FirstLaunch", 1) == 0)
        {
            arrow.gameObject.SetActive(false);
            active = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(taskIndex < taskList.Count && active)
        {
            Vector3 lookPos = taskList[taskIndex].transform.position;
            Vector3 directionVector = taskList[taskIndex].transform.position - transform.position;
            directionVector.y = 0;

            directionVector = directionVector / directionVector.magnitude;
            directionVector = directionVector * radius + transform.position;
            arrow.position = new Vector3(directionVector.x, arrow.position.y, directionVector.z);

            lookPos.y = 0;
            arrow.LookAt(lookPos);
            
        }
    }

    public void SetArrowManager(List<TaskManager> targets)
    {
        taskList = targets;
    }

    public void ChangeTarget()
    {
        if(taskIndex < taskList.Count)
        {
            taskIndex++;
            PlayerPrefs.SetInt(nameof(taskIndex), taskIndex);
        }

        if(taskIndex >= taskList.Count)
        {
            arrow.gameObject.SetActive(false);
            PlayerPrefs.SetInt("FirstLaunch", 0);
            active = false;
        }

    }
}

