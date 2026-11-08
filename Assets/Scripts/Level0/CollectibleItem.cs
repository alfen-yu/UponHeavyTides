using UnityEngine;

namespace UponHeavyTides.Level0
{
    public class CollectibleItem : MonoBehaviour
    {
        [SerializeField] private string itemName = "Sunlit Pearl";
        [SerializeField] private float rotateSpeed = 60f;
        [SerializeField] private float bobHeight = 0.25f;
        [SerializeField] private float bobFrequency = 2f;

        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            // Underwater gentle floating bobbing motion
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
            float yOffset = Mathf.Sin(Time.time * bobFrequency) * bobHeight;
            transform.position = startPos + new Vector3(0f, yOffset, 0f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<UponHeavyTides.Player.PlayerMovement>() != null)
            {
                Collect();
            }
        }

        private void Collect()
        {
            if (Level0Controller.Instance != null)
            {
                Level0Controller.Instance.AddCollectible(itemName);
            }
            else
            {
                Debug.Log($"Collected: {itemName}");
            }

            Destroy(gameObject);
        }
    }
}
