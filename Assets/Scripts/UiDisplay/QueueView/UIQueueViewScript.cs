using UnityEngine;
using UnityEngine.UI;

public class UIQueueViewScript : MonoBehaviour
{
    public Button ShowBuildQueueList;
    public Button ShowServeQueueList;

    public UIBuilderQueueViewListScript BuildQueueList;
    public UISerfQueueViewListScript ServeQueueList;

    void Start()
    {
        BuildQueueList.gameObject.SetActive(false);
        ServeQueueList.gameObject.SetActive(false);

        ShowBuildQueueList.onClick.AddListener(() => { ShowBuildQueueListClicked(); });
        ShowServeQueueList.onClick.AddListener(() => { ShowServeQueueListClicked(); });
    }

    private void ShowBuildQueueListClicked()
    {
        BuildQueueList.gameObject.SetActive(!BuildQueueList.gameObject.activeSelf);
        ServeQueueList.gameObject.SetActive(false);
    }

    private void ShowServeQueueListClicked()
    {
        ServeQueueList.gameObject.SetActive(!ServeQueueList.gameObject.activeSelf);
        BuildQueueList.gameObject.SetActive(false);
    }
}