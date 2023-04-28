using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider SliderHoriz;
    public Slider SliderVert;

    public TextMeshProUGUI TextHoriz;
    public TextMeshProUGUI TextVert;

	private float _valueHoriz;
	private float _valueVert;

	private void Awake()
	{
		SliderHoriz.value = PlayerPrefs.GetFloat("HorizontalSensitivity");
		SliderVert.value = PlayerPrefs.GetFloat("VerticalSensitivity");
	}

	public void OnValueChangedHoriz(float newValue)
	{
		_valueHoriz = Mathf.Round(newValue * 10.0f);
		TextHoriz.text = _valueHoriz.ToString();
	}

	public void OnValueChangedVert(float newValue)
	{
		_valueVert = Mathf.Round(newValue * 10.0f);
		TextVert.text = _valueVert.ToString();
	}

	public void SubmitValues()
	{
		GetComponent<MainMenu>().Menu.SetActive(true);
		GetComponent<MainMenu>().OptionsMenu.SetActive(false);

		PlayerPrefs.SetFloat("VerticalSensitivity", _valueVert);
		PlayerPrefs.SetFloat("HorizontalSensitivity", _valueHoriz);
	}
}
