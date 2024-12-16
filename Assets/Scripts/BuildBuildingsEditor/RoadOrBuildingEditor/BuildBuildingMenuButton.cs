using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;
using System;

public class BuildBuildingMenuButton : EditorWindow
{
    [SerializeField] private int selectedIndex = -1;
    //private ScrollView rightPane;
    private VisualElement rightPane;
    private ListView leftPane;
    private BuildingPrefabItem selectedBuilding;


    [MenuItem("GameObject/Create Road or Building")]
    static void CreateRoad()
    {
        EditorWindow window = GetWindow<BuildBuildingMenuButton>();
        window.titleContent = new GUIContent("Select your building....");

        // Limit size of the window
        window.minSize = new Vector2(500, 300);
        window.maxSize = new Vector2(500, 300);

        window.Show();

    }    

    public void CreateGUI()
    {
        // Create a two-pane view with the left pane being fixed with
        var splitView = new TwoPaneSplitView(0, 150, TwoPaneSplitViewOrientation.Horizontal);

        // Add the view to the visual tree by adding it as a child to the root element
        rootVisualElement.Add(splitView);

        // A TwoPaneSplitView always needs exactly two child elements
        leftPane = new ListView();
        splitView.Add(leftPane);
        //rightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        rightPane = new VisualElement();
        splitView.Add(rightPane);        


        var allBuildings = GetRoadAndAllBuildingTypes();

        leftPane.makeItem = () => new Label();
        leftPane.bindItem = (item, index) => { (item as Label).text =
            allBuildings[index].BuildingType == BuildingType.NONE ?
            "Road " :
            allBuildings[index].BuildingType.ToString().Capitalize(); };
        leftPane.itemsSource = allBuildings;

        leftPane.onSelectionChange += OnItemSelectionChange;

        // Restore the selection index from before the hot reload
        leftPane.selectedIndex = selectedIndex;

        // Store the selection index when the selection changes
        leftPane.onSelectionChange += (items) => { selectedIndex = leftPane.selectedIndex; };        
    }   

    private void OnItemSelectionChange(IEnumerable<object> selectedItems)
    {
        var selectedBuilding = selectedItems.First() as BuildingPrefabItem;
        if (selectedBuilding == null)
            return;

        rightPane.Clear();

        // Add a new Image control and display the sprite
        var spriteImage = new Image();
        spriteImage.scaleMode = ScaleMode.ScaleToFit;
        spriteImage.sprite = selectedBuilding.Icon;

        //spriteImage.StretchToParentSize();    

        Label capturingLabelPrefabRoot = new Label("GO TO PREFAB");
        capturingLabelPrefabRoot.RegisterCallback<MouseDownEvent>((evt) =>
        {
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(selectedBuilding.BuildingPrefab)));

        });

        rightPane.Add(capturingLabelPrefabRoot);
        rightPane.Add(spriteImage);

        EditorSettings.BuildRoadOnMouseActivity_Active = selectedBuilding.BuildingType == BuildingType.NONE;
        EditorSettings.BuildBuildingOnMouseActivity_Active = selectedBuilding.BuildingType != BuildingType.NONE;
        if(selectedBuilding.BuildingType != BuildingType.NONE)
        {
            EditorSettings.SelectedBuildingType = selectedBuilding.BuildingType;
        }
        else
        {
            EditorSettings.SelectedBuildingType = null;
        }
            
              
    }

    private void CaptureMouse()
    {
        throw new NotImplementedException();
    }

    private List<BuildingPrefabItem> GetRoadAndAllBuildingTypes()
    {
        var gamemanager = FindObjectOfType<GameManager>();
        var allBuildingPrefabs = gamemanager.BuildingPrefabItems.OrderBy(x => x.BuildingType.ToString()).ToList();

        if(!allBuildingPrefabs.Any(x => x.BuildingType == BuildingType.NONE))
        {
            var roadAsBuildingPrefab = new BuildingPrefabItem
            {
                BuildingType = BuildingType.NONE,
                BuildingPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Final-Prefab/Road/Road.prefab", typeof(GameObject)),
                Icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Resources/Images/BuildingImages/StoneRoadImage.png", typeof(Sprite)),
                DisplayOffset = new Vector3(0, 0, 0)
            };

            allBuildingPrefabs.Insert(0, roadAsBuildingPrefab);
        }        

        return allBuildingPrefabs;
    }

    private void OnDestroy()
    {
        EditorSettings.BuildRoadOnMouseActivity_Active = false;
        EditorSettings.BuildBuildingOnMouseActivity_Active = false;
        EditorSettings.SelectedBuildingType = null;
    }
}