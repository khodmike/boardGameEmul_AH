using System;
using System.Windows.Forms;
using System.Drawing; 
 

namespace mmxAH
{  public class WorkForm: Form
	{  
	   private RichTextBox lblLeft, lblMid, lblRight;
		private Label lblQ;
		private ComboBox libChoose;
		private CheckedListBox boxMultiChoose;
		private byte neededMultiChooseCount=0;
		public byte dec;
		private Button btnYes, btnNo, btnChoose, btnStatus, btnLog, btnInvest, btnMonsters, btnAo, btnOW, btnArchemMap;
		private int MaxX, MaxY;
		private GameEngine en;
		private bool isPauseMode=false;
		public WorkForm (GameEngine eng, bool isAloneMode=false)
		{
			en = eng;
			en.SetWorkForm (this); 
		

			MaxX = SystemInformation.VirtualScreen.Width;
			MaxY = SystemInformation.VirtualScreen.Height;

			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.WindowState = FormWindowState.Maximized;
			this.Text = en.sysstr.GetString (SSType.Title);

			lblLeft = CreateLbl ();
			lblLeft.Location = new Point (0, 0);
			lblLeft.Width = MaxX;
			lblLeft.Visible = true;

			lblMid = CreateLbl ();
			lblMid.Location = new Point (MaxX / 3, 0);
	
			lblRight = CreateLbl ();
			lblRight.Location = new Point (MaxX * 2 / 3, 0);



		

			Button btn = new Button ();
			if (isAloneMode)
			{ btn.Text= "Exit";
				btn.Click+= BtnMMClickExit;  

			}
			else
			{
			btn.Text=en.sysstr.GetString(SSType.MainMenuButton);
			btn.Click+= BtnMMClick;
			}
			btn.Location= new Point( MaxX/2, MaxY*92/100);
			btn.Width= MaxX/10;
			this.Controls.Add(btn);

		    
			btnYes = CreateButton (BtnYesClick, 20, 89, false, "Yes"); 
			btnNo=CreateButton (BtnNoClick, 40, 89, false, "No"); ;

			Label lbl= new Label();
			lbl.Visible=false;
			lbl.Location= new Point( MaxX*2/10, MaxY*83/100);
			lbl.Width=MaxX*6/10;
			lbl.Height=MaxY*3/100;
			lblQ=lbl;
			this.Controls.Add(lbl);

			libChoose= new ComboBox();
			libChoose.Location= new Point( MaxX*2/10, MaxY*87/100);
			libChoose.Width=MaxX*4/10;
			libChoose.DropDownStyle = ComboBoxStyle.DropDownList; 
			libChoose.Visible=false;
			libChoose.SelectedIndexChanged += LibChooseSelected;
			libChoose.MaximumSize = new Size (MaxX * 6 / 10, MaxY * 6 / 100);  
			this.Controls.Add(libChoose);
			btnChoose=CreateButton (BtnChooseClick, 70, 87, false, "Choose"); 
			btnStatus= CreateButton (BtnStatusClick, 10, 77, true, en.sysstr.GetString(SSType.ButtonStatus) );
			btnLog= CreateButton (BtnLogClick, 20, 77, true, en.sysstr.GetString(SSType.ButtonLog));
			btnInvest= CreateButton (BtnInvestClick, 30, 77, true, en.sysstr.GetString(SSType.ButtonInvest));
			btnAo= CreateButton (BtnAOClick, 40, 77, true, en.sysstr.GetString(SSType.ButtonAO));
			btnMonsters= CreateButton (BtnMonstersClick, 50, 77, true, en.sysstr.GetString(SSType.ButtonMonsters));
			btnOW= CreateButton (BtnOWClick, 10, 80, true, en.sysstr.GetString(SSType.ButtonOW));
			btnArchemMap= CreateButton (BtnArchemMapClick, 20, 80, true, en.sysstr.GetString(SSType.ButtonArchemMap));
			libChoose.MaxDropDownItems = 5;
			boxMultiChoose  = new CheckedListBox ();

			boxMultiChoose.Location = new Point (MaxX * 2 / 10, MaxY * 86 / 100);
			boxMultiChoose.Height = MaxY * 5/ 100;
			boxMultiChoose.Width=MaxX*4/10;
			boxMultiChoose.ItemCheck  += BoxMultiChooseSelected;   
			boxMultiChoose.Visible = false;
			this.Controls.Add (boxMultiChoose); 
		


		 
		}


		private RichTextBox CreateLbl()
		{ RichTextBox lbl= new RichTextBox();
			lbl.Width=MaxX/3;
			lbl.Height= MaxY*75/100;
			lbl.Text="";
			lbl.WordWrap=false;
			lbl.ReadOnly=true;
			lbl.ScrollBars= RichTextBoxScrollBars.Both;
			lbl.Visible=false;
			lbl.BackColor= Color.White;
			this.Controls.Add (lbl);
			return lbl;

		}


		private Button CreateButton(EventHandler  event_handler, int x, int y, bool isVisible, string Capiton) 
		{ 
			Button btn= new Button();
			btn.Click+= event_handler;
			btn.Text = Capiton;
			btn.Location= new Point( MaxX*x/100, MaxY*y/100);
			btn.Visible=isVisible; 
			btn.Width = MaxX* 7 / 100 ; 
			this.Controls.Add(btn);
			return btn;


		}



		private void BtnMMClick( object sender, EventArgs arg)
		{ this.Hide ();
			en.io.ShowMainMenu (); 
	


		}

			private void BtnMMClickExit( object sender, EventArgs arg)
		{ this.Close ();

	


		}


		private void BtnYesClick( object sender, EventArgs arg)
		{  
			ClearControls();
			if (isPauseMode)
			{
				isPauseMode = false; 
				en.io.PauseEnd (); 
			}
				else
				en.io.Answer(true);
		


		}


		private void BtnNoClick( object sender, EventArgs arg)
		{  
			ClearControls(); 
			en.io.Answer(false);



		}


		private void BtnChooseClick( object sender, EventArgs arg)
		{  
			ClearControls(); 
			if (neededMultiChooseCount != 0)
			{ neededMultiChooseCount = 0;
			 System.Collections .Generic.List<short> outCodes = new System.Collections.Generic.List<short> ();
				for (int i=0; i<boxMultiChoose.CheckedIndices.Count; i++) 
					outCodes.Add ((short)boxMultiChoose.CheckedIndices [i]);
				en.io.MultiChooseEnd (outCodes);  
				

			}
			else
			en.io.ChooseEnd(libChoose.SelectedIndex);  




		}

		private void BtnStatusClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Status); 
			en.io.UpdateForm (); 


		}

		private void BtnLogClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Log);  
			en.io.UpdateForm (); 


		}

		private void BtnInvestClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Investigators );  
			en.io.UpdateForm (); 


		}

		private void BtnAOClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Ao );  
			en.io.UpdateForm (); 


		}

		private void BtnMonstersClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Monsters);  
			en.io.UpdateForm (); 


		}

		private void BtnOWClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Ow); 
			en.io.UpdateForm (); 


		}

		private void BtnArchemMapClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Map );  
			en.io.UpdateForm (); 


		}



			private void LibChooseSelected( object sender, EventArgs arg)
		{  
			btnChoose.Enabled=true;


		}


		private void BoxMultiChooseSelected( object sender, ItemCheckEventArgs  arg)
		{  
			byte checkCount= (byte) boxMultiChoose.CheckedItems.Count;
			if (arg.NewValue == CheckState.Checked)
				checkCount ++;
			else
				checkCount--; 

			if (checkCount  == neededMultiChooseCount)   
				btnChoose.Enabled = true;
			else
				btnChoose.Enabled = false;


		}




		public void ShowLabelText (string Text,  byte fontsize=12, bool isBold=false, bool isItalic=false, byte  label=1, string chooseColor="Black")
		{
			Font f;
			lblLeft.DeselectAll ();
			lblMid.DeselectAll ();
			lblRight.DeselectAll ();
			Color col = Color.FromName (chooseColor); 
			if( isBold && isItalic)
				f = new System.Drawing.Font ("Times New Roman", fontsize, FontStyle.Bold | FontStyle.Italic );
			else if (isBold)
				f = new System.Drawing.Font ("Times New Roman", fontsize, FontStyle.Bold);
			else if (isItalic)
				f = new System.Drawing.Font ("Times New Roman", fontsize, FontStyle.Italic);
			else
				f = new System.Drawing.Font ("Times New Roman", fontsize, FontStyle.Regular);

			if (label == 1)
			{
				lblLeft.SelectionFont=f;
				lblLeft.SelectionColor = col    ; 
				lblLeft.AppendText(Text);
			 
			}
			else if (label == 2)
			{
				lblMid.SelectionFont=f;
				lblMid.SelectionColor = col    ; 
				lblMid.AppendText(Text);

			}
			else 
			{
				lblRight.SelectionFont=f;
				lblRight.SelectionColor = col    ; 
				lblRight.AppendText(Text);
			
			}




		}


	

		public void mapMode (bool mode)
		{
			ClearLabels ();

			lblMid.Visible = mode;
			lblRight.Visible = mode;
			if (mode)
			{
				lblLeft.Width = MaxX / 3;

			}
			 else
				lblLeft.Width = MaxX;



 			
          

		}

		public void ArrengeLabels ()
		{ int Max= Math.Max ( lblLeft.Lines.Length, Math.Max (lblMid.Lines.Length, lblRight.Lines.Length));
			for( int i= lblLeft.Lines.Length;i< Max; i++)
				ShowLabelText(Environment.NewLine,10, false,false,1);
			for( int i= lblMid.Lines.Length;i< Max; i++)
				ShowLabelText(Environment.NewLine, 10, false,false,2);
			for( int i= lblRight.Lines.Length;i< Max; i++)
				ShowLabelText(Environment.NewLine, 10, false,false,3);


		}
		private void ClearLabels()
		{ lblLeft.Text="";
			lblMid.Text="";
			lblRight.Text="";

		}

		private void ClearControls ()
		{ btnYes.Visible = false;
		 btnNo.Visible = false;
		 lblQ.Visible=false; 
		 libChoose.Visible=false;
			boxMultiChoose.Visible = false;  
		 btnChoose.Visible=false; 

		}


		public void StartPause( string title)
		{ btnYes.Visible = true;
			btnYes.Text = title;    
			isPauseMode = true; 

		}


		public void StartYesNo( string title, string YesAnswer, string NoAnswer)
		{
			btnYes.Visible = true;
			btnYes.Text = YesAnswer; 
			btnNo.Visible = true;
			btnNo.Text = NoAnswer;
			lblQ.Visible = true; 
			lblQ.Text = title;  

		}

		public void StartChoose( string title, System.Collections.Generic.List<string> chooses, string buttonTitle)
		{ lblQ.Visible = true; 
			lblQ.Text = title;
			libChoose.Visible=true; 
			libChoose.Items.Clear();
			foreach( string str in chooses)
			{
				libChoose.Items.Add (str);  
			}

			btnChoose.Text= buttonTitle; 
			btnChoose.Visible=true; 
			btnChoose.Enabled=false;
		
		

		}


		public void StartMultiChoose( string title, System.Collections.Generic.List<string> chooses,  byte needCount, string buttonTitle)
		{ lblQ.Visible = true; 
			lblQ.Text = title;
			boxMultiChoose.Visible=true; 
			boxMultiChoose.Items.Clear();
			foreach( string str in chooses)
			{
			 boxMultiChoose.Items.Add (str);  
			}

			btnChoose.Text= buttonTitle; 
			btnChoose.Visible=true; 
			btnChoose.Enabled=false;
			neededMultiChooseCount = needCount;  



		}







	



	}












}