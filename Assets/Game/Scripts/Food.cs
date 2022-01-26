using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] private int fat = 1;

    public int Fat { get => fat; }
}
