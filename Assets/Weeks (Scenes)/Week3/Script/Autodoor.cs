using UnityEngine;
using System.Collections;

public class AutoDoor : MonoBehaviour
{
    [Tooltip("How high the door moves up when it opens")]
    [SerializeField] private float openHeight = 4f;

    [SerializeField] private float interval = 3f;
    [SerializeField] private float moveDuration = 0.5f;

    private Vector3 closedPosition;
    private Vector3 openedPosition;
    private bool isOpen = false;

    private void Start()
    {
        // Automatically lock in the targets based on where the door is placed in the editor
        closedPosition = transform.position;
        openedPosition = closedPosition + new Vector3(0, openHeight, 0);

        StartCoroutine(ToggleDoorRoutine());
    }

    private IEnumerator ToggleDoorRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            isOpen = !isOpen;
            Vector3 targetPos = isOpen ? openedPosition : closedPosition;
            Vector3 startPos = transform.position;
            float time = 0;

            while (time < 1f)
            {
                time += Time.deltaTime / moveDuration;
                transform.position = Vector3.Lerp(startPos, targetPos, time);
                yield return null;
            }
        }
    }
}