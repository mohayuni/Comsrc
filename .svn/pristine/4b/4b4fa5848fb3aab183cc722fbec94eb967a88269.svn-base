
//#define	ITEMINDEX2
//#define	MESSAGE2

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;					//	filestream
using System.Xml;					//	XmlSerializer
using System.Xml.Schema;			//	XmlSerializer
using System.Xml.Serialization;		//	XmlSerializer


namespace WinDialogTest
{
	public partial class DialogTest1 : Form
	{
		private const string myXmlName = "TestDialog.xml";
		private string[] combobox1Items = {"Combo0","Combo1","Combo2","Combo3","Combo4","Combo5",
												"Combo6","Combo7","Combo8","Combo9"};


		public DialogTest1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
#if NOP
			XmlSerializer serializer = new XmlSerializer(typeof(DialogTestObj));
			TextWriter writer = new StreamWriter();
			PurchaseOrder po = new PurchaseOrder();
#endif
			XmlSerializer _serializer = new XmlSerializer(typeof(DialogTestObj));
			_serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
			_serializer.UnknownAttribute += new	XmlAttributeEventHandler(serializer_UnknownAttribute);

			FileStream _fs = null;
			DialogTestObj _dialogTestObj;
			_dialogTestObj = new DialogTestObj();
			_dialogTestObj.itemIndex1 = 1;
			_dialogTestObj.Message1 = "Load時のテキスト";
#if ITEMINDEX2
			_dialogTestObj.itemIndex2 = 9;
#endif
#if MESSAGE2
			_dialogTestObj.Message2 = "Message2テキスト";
#endif

			_dialogTestObj.combobox1Items = combobox1Items;
			int _ii;

			_dialogTestObj.VaueObj1 = new ValueObj();
			_dialogTestObj.VaueObj1.min = 1;
			_dialogTestObj.VaueObj1.value = 5;
			_dialogTestObj.VaueObj1.max = 100;
			try
			{
				_fs = new FileStream(myXmlName, FileMode.Open);
				_dialogTestObj = (DialogTestObj)_serializer.Deserialize(_fs);
//				fs.Close();
			}
			catch (Exception _eeee)
			{
				MessageBox.Show("{" + myXmlName + "}xml file error (Form1_Load) error" + _eeee.Message);
				
			}
			if (_fs != null) _fs.Close();

			textBox1.Text = _dialogTestObj.Message1;
#if MESSAGE2
			textBox2.Text = _dialogTestObj.Message2;
#endif
			for (_ii = 0; _ii < 10; _ii++)
			{
//				comboBox1.Items.Add("Combo " + _ii.ToString());
				if ( (_dialogTestObj.combobox1Items != null) 
					&& (_dialogTestObj.combobox1Items[_ii] != null) )
 					comboBox1.Items.Add(_dialogTestObj.combobox1Items[_ii]);
				comboBox2.Items.Add("Combo " + _ii.ToString());
			}
			if (comboBox1.Items.Count > _dialogTestObj.itemIndex1)
				comboBox1.SelectedIndex = _dialogTestObj.itemIndex1;
#if ITEMINDEX2
			comboBox2.SelectedIndex = _dialogTestObj.itemIndex2;
#endif

		}

		private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
		{
			MessageBox.Show("Unknown Node:" + e.Name + "\t" + e.Text);
		}

		private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			System.Xml.XmlAttribute attr = e.Attr;
			MessageBox.Show("Unknown attribute " + attr.Name + "='" + attr.Value + "'");
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
#if NOP
			MessageBox.Show("comboBox1 Select Index = " + comboBox1.SelectedIndex.ToString() + "\r\n"
							+ "comboBox1 Select Text = " + comboBox1.SelectedItem+ "\r\n"
							+"textBox1 Text = " + textBox1.Text+"\r\n");
#endif
			XmlSerializer _serializer = new XmlSerializer(typeof(DialogTestObj));
			TextWriter _writer = null;
			try
			{
				_writer = new StreamWriter(myXmlName);
				DialogTestObj _dialogTestObj = new DialogTestObj();

				_dialogTestObj.Message1 = textBox1.Text;
#if MESSAGE2
				_dialogTestObj.Message2 = textBox2.Text;
#endif
				_dialogTestObj.itemIndex1 = comboBox1.SelectedIndex;
#if ITEMINDEX2
				_dialogTestObj.itemIndex2 = comboBox2.SelectedIndex;
#endif
				_dialogTestObj.VaueObj1 = new ValueObj();
				_dialogTestObj.VaueObj1.min = 1;
				_dialogTestObj.VaueObj1.value = 5;
				_dialogTestObj.VaueObj1.max = 100;
				_dialogTestObj.combobox1Items = combobox1Items;

				_serializer.Serialize(_writer, _dialogTestObj);

			}
			catch (Exception _eeee)
			{
				MessageBox.Show("{" + myXmlName + "}xml file error (buttonOK_Click) error" + _eeee.Message);
			}
			if (_writer != null) _writer.Close();

			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void textBox1_Leave(object sender, EventArgs e)
		{
			if (textBox1.Text.IndexOf("テキスト") != -1)
			{
				MessageBox.Show("???文字列　テキストはダメ");
				textBox1.Focus();			
			}

		}
	}
}

[XmlRoot("TestDialog")]
public class DialogTestObj
{
	[System.Xml.Serialization.XmlElement("ComboBox1Index")]
	public int itemIndex1;
#if	ITEMINDEX2
	[System.Xml.Serialization.XmlElement("ComboBox2Index")]
	public int itemIndex2;
#endif
	[System.Xml.Serialization.XmlElement("TestBox1String")]
	public string Message1;
#if MESSAGE2
	[System.Xml.Serialization.XmlElement("TestBox2String")]
	public string Message2;
#endif
	[System.Xml.Serialization.XmlElement("ValueObj1")]
	public ValueObj VaueObj1;
	[System.Xml.Serialization.XmlElement("ComboBox1Items")]
	public string[] combobox1Items;
}

[XmlRoot("ValueObj")]
public class ValueObj
{
	[System.Xml.Serialization.XmlElement("min")]
	public int min;
	[System.Xml.Serialization.XmlElement("value")]
	public int value;
	[System.Xml.Serialization.XmlElement("max")]
	public int	max;
}
