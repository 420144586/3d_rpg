using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// [System.Serializable]

// public class EventVector3: UnityEvent<Vector3>{

// }
public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;
    RaycastHit hitInfo;
    public event Action<Vector3> onMouseClicked;
    public event Action<GameObject> onEnemyClicked;


    protected override void Awake()
    {
       base.Awake();
       DontDestroyOnLoad(this); 
    }
    
    void Start() {
        Application.targetFrameRate = 60;
    }



    // Update is called once per frame
    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    void SetCursorTexture(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hitInfo)) {
            //切换鼠标贴图
            switch(hitInfo.collider.gameObject.tag) {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break; 
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }
    void MouseControl() {
        if(Input.GetMouseButtonDown(1) && hitInfo.collider != null) {
            if(hitInfo.collider.gameObject.CompareTag("Ground")) {
                onMouseClicked?.Invoke(hitInfo.point);
            }


            if(hitInfo.collider.gameObject.CompareTag("Enemy")) {
                onEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            
            if(hitInfo.collider.gameObject.CompareTag("Attackable")) {
                onEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }

            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                onMouseClicked?.Invoke(hitInfo.point);
            }
        }
    }
}
