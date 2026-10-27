using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour {
    public float movingSpeed = 3f;

    CharacterController player; 
    
    void Awake() {
        player = GetComponent<CharacterController>(); 
    }

    void Update() {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        Vector3 input = Vector3.zero;
        if (kb.wKey.isPressed) input.z += 1f;
        if (kb.sKey.isPressed) input.z -= 1f;
        if (kb.aKey.isPressed) input.x -= 1f;
        if (kb.dKey.isPressed) input.x += 1f;
        
        player.Move(input.normalized * movingSpeed * Time.deltaTime);
    }
}