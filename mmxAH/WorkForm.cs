using System;
using System.Windows.Forms;
using System.Drawing; 
 

namespace mmxAH
{  public class WorkForm: Form
	{  
	   private RichTextBox lblLeft, lblMid, lblRight;
		private Label lblQ;
		private ComboBox libChoose;
		public byte dec;
		private Button btnYes, btnNo, btnChoose, btnStatus, btnLog, btnInvest, btnMonsters, btnAo, btnOW, btnArchemMap;
		private int MaxX, MaxY;
		private GameEngine en;
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
			this.Shown += frmShow  ;  

			Timer tm = new Timer ();
			tm.Interval = 250;
			tm.Tick += tmTick; 
			tm.Start ();

		 
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


		private void tmTick( object sender, EventArgs arg)
		{ en.io.CheckServerUpdate (); 

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
			if(en.io.mode == IOMode.Pause)
				en.io.PauseEnd(); 
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
			en.io.ChooseEnd(libChoose.SelectedIndex);  



		}

		private void BtnStatusClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Status);  


		}

		private void BtnLogClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Log);  


		}

		private void BtnInvestClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Investigators );  


		}

		private void BtnAOClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Ao );  


		}

		private void BtnMonstersClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Monsters);  


		}

		private void BtnOWClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Ow);  


		}

		private void BtnArchemMapClick( object sender, EventArgs arg)
		{  en.io.SetFormMode (FormMode.Map );  


		}



			private void LibChooseSelected( object sender, EventArgs arg)
		{  
			btnChoose.Enabled=true;


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
		 btnChoose.Visible=false; 

		}

		private void frmShow( object sender, EventArgs e)
		{ RefreshControls();

		}


		public void RefreshControls ()
		{


			if (en.io.mode == IOMode.Pause)
			{
				btnYes.Visible = true;
				btnYes.Text = en.io.PauseTitle;     

			}

			if (en.io.mode == IOMode.StandAloneActhion)
			{
				en.io.StandAloneResponse(); 

			}

			if (en.io.mode == IOMode.YesNo)
			{
				btnYes.Visible = true;
				btnYes.Text = en.io.YesTitle; 
				btnNo.Visible = true;
				btnNo.Text = en.io.NoTitle;
				lblQ.Visible = true; 
				lblQ.Text = en.io.QuestTitle;  

				
			}

			if (en.io.mode == IOMode.Choose)
			{ lblQ.Visible = true; 
			  lblQ.Text = en.io.QuestTitle;
			  libChoose.Visible=true; 
			  libChoose.Items.Clear();
              foreach( IOOption opt in en.io.options)
				{
					libChoose.Items.Add (opt.Title);  
				}

				btnChoose.Text= en.io.ChooseEndTitle; 
				btnChoose.Visible=true; 
				btnChoose.Enabled=false;
			}
			this.Invalidate();
			return;


		}




	



	}












}