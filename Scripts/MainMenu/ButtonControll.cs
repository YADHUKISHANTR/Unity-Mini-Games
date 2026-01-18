using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonControll : MonoBehaviour
{
    [SerializeField] int SceneIndex;
    private void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(LoadScene);
    }
    void LoadScene()
    {
        SceneManager.LoadScene(SceneIndex);
    }
}
