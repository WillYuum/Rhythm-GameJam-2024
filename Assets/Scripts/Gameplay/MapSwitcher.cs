using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSwitcher : MonoBehaviour
{
    [SerializeField] private Tilemap _beachMap;
    [SerializeField] private Tilemap _robotMap;


    private Tilemap _currentMapType;

    public enum MapType
    {
        Beach,
        Robot,
        None
    }

    private void Awake()
    {
        _currentMapType = _robotMap;
        _beachMap.gameObject.SetActive(false);
        _robotMap.gameObject.SetActive(false);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchTo(_currentMapType == _beachMap ? MapType.Robot : MapType.Beach);
        }
#endif
    }



    public void SwitchTo(MapType mapType)
    {
        Debug.Log("Switching to " + mapType);

        _currentMapType.gameObject.SetActive(false);

        switch (mapType)
        {
            case MapType.Beach:
                _currentMapType = _beachMap;
                break;
            case MapType.Robot:
                _currentMapType = _robotMap;
                break;
            case MapType.None:
                return;
        }

        _currentMapType.gameObject.SetActive(true);
    }
}
