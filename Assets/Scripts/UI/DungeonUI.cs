
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class DungeonUI : MonoBehaviour
{
    [SerializeField]
    DungeonGenerator generator;


    [SerializeField]
    Slider RoomSlider;
    [SerializeField]
    TextMeshProUGUI RoomNumber ;
    int InitNumber = 5;

    [SerializeField]
    Slider maxXSlider;
    [SerializeField]
    TextMeshProUGUI maxX;

    [SerializeField]
    Slider maxYSlider;
    [SerializeField]
    TextMeshProUGUI maxY;

    [SerializeField]
    Slider maxZSlider;
    [SerializeField]
    TextMeshProUGUI maxZ;

    [SerializeField]
    Slider minDistanceSlider;
    [SerializeField]
    TextMeshProUGUI distance;

    [SerializeField]
    Slider GridXSlider;
    [SerializeField]
    TextMeshProUGUI gridX;

    [SerializeField]
    Slider GridYSlider;
    [SerializeField]
    TextMeshProUGUI gridY;

    [SerializeField]
    Slider GridZSlider;
    [SerializeField]
    TextMeshProUGUI gridZ;

    Vector3Int newMaxSize = new Vector3Int();
    Vector3Int newGridSize = new Vector3Int(10,10,10);

    // Start is called before the first frame update
    void Start()
    {
        Init();

        RoomSlider.onValueChanged.AddListener((v) =>
        {
            RoomNumber.text = v.ToString();
            generator.SetRoomNumber((int)v);

        });


        maxXSlider.onValueChanged.AddListener((v) =>
        {
            maxX.text = v.ToString();
            newMaxSize.x = (int)v;
            generator.setMaxSize(newMaxSize);
        });

        maxYSlider.onValueChanged.AddListener((v) =>
        {
            maxY.text = v.ToString();
            newMaxSize.y = (int)v;
            generator.setMaxSize(newMaxSize);
        });
        maxZSlider.onValueChanged.AddListener((v) =>
        {
            maxZ.text = v.ToString();
            newMaxSize.z = (int)v;
            generator.setMaxSize(newMaxSize);
        });
        minDistanceSlider.onValueChanged.AddListener((v) =>
        {
            distance.text = v.ToString();
            generator.minDistance = (int)v;
        });

        GridXSlider.onValueChanged.AddListener((v) =>
        {
            gridX.text = v.ToString();
            newGridSize.x = (int)v;
            generator.boundsRadius = newGridSize;
        });

        GridYSlider.onValueChanged.AddListener((v) =>
        {
            gridY.text = v.ToString();
            newGridSize.y = (int)v;
            generator.boundsRadius = newGridSize;
        });
        GridZSlider.onValueChanged.AddListener((v) =>
        {
            gridZ.text = v.ToString();
            newGridSize.z = (int)v;
            generator.boundsRadius = newGridSize;
        });
    }

    public void ChangeDungeonDimension(int selectedIndex)
    {
        generator.type = (GenerationType)selectedIndex;
    }

    private void Init()
    {
        generator.SetRoomNumber(InitNumber);
        RoomNumber.text = InitNumber.ToString();
        RoomSlider.value = InitNumber;

        gridX.text = newGridSize.x.ToString();
        gridY.text = newGridSize.y.ToString();
        gridZ.text = newGridSize.z.ToString();
        generator.boundsRadius = newGridSize;
        GridXSlider.value = newGridSize.x;
        GridYSlider.value = newGridSize.y;
        GridZSlider.value = newGridSize.z;
    }

}
