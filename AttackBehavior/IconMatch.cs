using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconMatch : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Sprite L;
    [SerializeField] private Sprite R;
    [SerializeField] public string behaviorName;
    [SerializeField] private RectTransform _parentRectTransform;
    private List<Image> _icons=new List<Image>();  
    [HideInInspector] public int matchCount=0;
    private void Visible(int index)
    {
        Color temp = _icons[index].color;    
        temp.a=1;
        _icons[index].color= temp;
    }
    private void Invisible(int index)
    {
        Color temp = _icons[index].color;    
        temp.a= 170f/255f;
        _icons[index].color= temp;
    }
    public void DisMatch()
    {
        for (int i = 0; i <behaviorName.Length; i++)
        {
            Invisible(i);
        }
        matchCount = 0; 
    }
    public bool Match(string input)
    {
        if (input.Length == 0)
        {
            DisMatch();
            return false;
        }
        for(int i = matchCount; i < input.Length; i++)
        {
            if (matchCount==behaviorName.Length)
            {
                DisMatch();
            }
            if (behaviorName[matchCount] == input[i])
            {
                Visible(matchCount);
                matchCount++;
            }
            else
            {
                DisMatch();
                return false;
            }
        }
        return true;
    }
    private void Start()
    {
        for (int i = 0; i < behaviorName.Length; i++)
        {
            Image newIcon = Instantiate(icon, _parentRectTransform);
            RectTransform rectTransform = newIcon.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = new Vector3(i * 100f, 0, 0);

            if (behaviorName[i] == 'L')
            {
                newIcon.sprite = L;
            }
            else if (behaviorName[i] == 'R')
            {
                newIcon.sprite = R;
            }
            // register 
            _icons.Add(newIcon);
        }
    }

    private void Update()
    {
        
    }
}
