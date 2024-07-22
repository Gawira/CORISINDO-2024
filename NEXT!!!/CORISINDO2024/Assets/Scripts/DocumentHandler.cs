using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentHandler : MonoBehaviour
{
    public List<GameObject> documentPrefabs;
    public Transform documentSpawner;
    public Transform workstationTable;
    public float slideSpeed = 0.5f; // Slower speed
    public float spawnAreaDepth = 2f; // Use depth for Z-axis spacing

    private List<GameObject> spawnedDocuments = new List<GameObject>();
    public ObjectInteractor objectInteractor; // Reference to ObjectInteractor

    // Ensure this method is public so it can be called by animation events
    public void StartGivingDocuments()
    {
        GiveDocuments();
    }

    private void GiveDocuments()
    {
        // Ensure we spawn each document only once
        int documentCount = documentPrefabs.Count;
        float spacing = spawnAreaDepth / (documentCount - 1);

        for (int i = 0; i < documentCount; i++)
        {
            GameObject documentPrefab = documentPrefabs[i];
            float offsetZ = (i - (documentCount - 1) / 2f) * spacing;

            // Calculate spawn and end positions
            Vector3 spawnPosition = documentSpawner.position + new Vector3(0, 0, offsetZ);
            Vector3 endPosition = workstationTable.position + new Vector3(0, 0, offsetZ);

            // Instantiate and add to the list
            GameObject document = Instantiate(documentPrefab, spawnPosition, Quaternion.identity);
            document.transform.rotation = documentPrefab.transform.rotation; // Set the rotation
            spawnedDocuments.Add(document);

            // Make the document kinematic during the animation
            Rigidbody rb = document.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            // Slide the document to the end position
            StartCoroutine(SlideDocument(document, spawnPosition, endPosition));
        }

        // Update the passport reference in ObjectInteractor
        objectInteractor.UpdatePassportReference();
    }

    private IEnumerator SlideDocument(GameObject document, Vector3 startPosition, Vector3 endPosition)
    {
        float elapsedTime = 0f;
        float distance = Vector3.Distance(startPosition, endPosition);
        float duration = distance / slideSpeed;

        while (elapsedTime < duration)
        {
            document.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        document.transform.position = endPosition;

        // Wait for a short moment to ensure document has settled
        yield return new WaitForSeconds(0.1f);

        // Make the document non-kinematic after the animation
        Rigidbody rb = document.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}
