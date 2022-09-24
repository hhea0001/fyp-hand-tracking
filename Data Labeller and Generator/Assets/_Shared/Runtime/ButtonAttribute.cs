using System;

public class ButtonAttribute : Attribute
{
	public string text;
	public ButtonAttribute(string text)
	{
		this.text = text;
	}
}