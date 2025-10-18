using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    public GameObject[] vehicles;
    public Transform[] spawnPoints;
    public Transform[] dropPoints;
    [HideInInspector]
    public Vector2[] startCoordinates;
    [HideInInspector]
    public Transform[] assignedDropPoints;
    public Canvas can;
    public AudioSource effects;
    public AudioClip[] audioCli;
    [HideInInspector]
    public bool rightPlace = false;
    public static GameObject lastDragged = null;
    public static bool drag = false;
    public bool locked = false;


    void Awake()
    {
        startCoordinates = new Vector2[vehicles.Length];
        assignedDropPoints = new Transform[vehicles.Length];

        // Рандомим стартовые точки
        Transform[] availableSpawn = (Transform[])spawnPoints.Clone();
        for (int i = 0; i < vehicles.Length; i++)
        {
            int randomIndex = Random.Range(0, availableSpawn.Length);
            Transform point = availableSpawn[randomIndex];

            RectTransform rect = vehicles[i].GetComponent<RectTransform>();
            rect.localPosition = point.localPosition;
            startCoordinates[i] = point.localPosition;

            // Если индекс от 10 до 19, рандомим масштаб и поворот
            if (i >= 10 && i <= 19)
            {
                float randomScale = Random.Range(0.6f, 1f);
                rect.localScale = new Vector3(randomScale, randomScale, 1f);

                float randomRotation = Random.Range(0f, 90f);
                rect.localRotation = Quaternion.Euler(0f, 0f, randomRotation);
            }

            availableSpawn = RemoveAt(availableSpawn, randomIndex);
        }

        // Рандомим dropPlaces
        assignedDropPoints = ShuffleArray(dropPoints);
    }




    private Transform[] RemoveAt(Transform[] array, int index)
    {
        Transform[] newArray = new Transform[array.Length - 1];
        for (int i = 0, j = 0; i < array.Length; i++)
        {
            if (i == index) continue;
            newArray[j++] = array[i];
        }
        return newArray;
    }

    private Transform[] ShuffleArray(Transform[] array)
    {
        Transform[] newArray = (Transform[])array.Clone();
        for (int i = 0; i < newArray.Length; i++)
        {
            int rnd = Random.Range(i, newArray.Length);
            (newArray[i], newArray[rnd]) = (newArray[rnd], newArray[i]);
        }
        return newArray;
    }

}
