using UnityEngine;
using UnityEngine.UI;

public class ChapterInterfaceElement : MonoBehaviour
{
    [SerializeField] private ElementType elementType;
    public ElementType ElementType => elementType;

    private Button elementButton;

    public void Initialize(ChapterInfo chapterInfo)
    {
        elementType = chapterInfo.elementType;
        elementButton = GetComponent<Button>();
        if (elementButton != null)
        {
            elementButton.onClick.AddListener(() => OnElementClicked(chapterInfo));
        }
    }

    private void OnElementClicked(ChapterInfo chapterInfo)
    {
        Debug.Log(ElementType);
        if (elementType == ElementType.SlideShow)
        {
            SlideShow.Create(chapterInfo, null);
            return;
        }

        SceneLoadWatcher.LoadScene(chapterInfo.gameName);
    }
}