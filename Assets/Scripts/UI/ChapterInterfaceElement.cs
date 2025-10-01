using UnityEngine;
using UnityEngine.UI;

public class ChapterInterfaceElement : MonoBehaviour
{
    [SerializeField] private ElementType elementType;
    public ElementType ElementType => elementType;

    private Button elementButton;

    public void Initialize(ChapterInfo chapterInfo)
    {
        elementButton = GetComponent<Button>();
        if (elementButton != null)
        {
            elementButton.onClick.AddListener(() => OnElementClicked(chapterInfo));
        }
    }

    private static void OnElementClicked(ChapterInfo chapterInfo)
    {
        SceneLoadWatcher.LoadScene(chapterInfo.gameName);
    }
}