using UnityEngine;
using UnityEngine.UI;
public class WidgetWrap
{
    private GameObject _gameObject;

    private Text mText;
    private Slider mSlider;
    private Image mImage;
    private RectTransform mRectTrans;
    public WidgetWrap(GameObject go)
    {
        _gameObject = go;
    }
    public GameObject gameObject
    {
        get
        {
            return _gameObject;
        }
    }



    public RectTransform RectTransform
    {
        get
        {
            if (mRectTrans == null)
            {
                mRectTrans = _gameObject.GetComponent<RectTransform>();
            }
            return mRectTrans;
        }
    }


    public Text Text
    {
        get
        {
            if (mText == null)
            {
                mText = _gameObject.GetComponent<Text>();
            }
            return mText;
        }
    }

    public Button Button
    {
        get { return _gameObject.GetComponent<Button>(); }
    }

    public Toggle Toggle
    {
        get { return _gameObject.GetComponent<Toggle>(); }
    }

    public InputField InputField
    {
        get
        {
            return _gameObject.GetComponent<InputField>();
        }
    }


    public Slider Slider
    {
        get
        {
            if (mSlider == null)
            {
                mSlider = _gameObject.GetComponent<Slider>();
            }

            return mSlider;
        }
    }

    public Image Image
    {
        get
        {
            if (mImage == null)
            {
                mImage = _gameObject.GetComponent<Image>();
            }

            return mImage;
        }
    }

    public Dropdown Dropdown
    {
        get
        {
            return _gameObject.GetComponent<Dropdown>();
        }
    }
}
