using UnityEngine;

public abstract class AbstractScreen : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    
    public bool IsVisible => _root && _root.activeSelf;

    public virtual void Show()
    {
        Debug.Log("Showing screen: " + GetType().Name);
        if (_root)
        {
            _root.SetActive(true);
        }
    }

    public virtual void Hide()
    {
        Debug.Log("Hiding screen: " + GetType().Name);
        if (_root)
        {
            _root.SetActive(false);
        }
    }
}