using System;
using System.Windows.Forms;
using System.Drawing; 

namespace mmxAH
{
	public class MainMenuForm: Form
	{

		private  GameEngine en;
		private WorkForm frm;
		private Button btnResume, btnSave;
		private static string[] CommandLineArgs;
		private static bool isPlayMode=true;
		public MainMenuForm ()
		{
			if (CommandLineArgs.Length != 0)
				switch (CommandLineArgs [0].ToUpper())
			{ case "TOBIN" : { en= new GameEngine(this); isPlayMode= false;  en.ToBin(); } break;  

				}


			if ( ! GlobalInit())
			Application.Exit ();

			this.FormBorderStyle= System.Windows.Forms.FormBorderStyle.None;  
			this.WindowState= FormWindowState.Maximized;  
			this.Text=en.sysstr.GetString(SSType.Title); 

			int MaxX, MaxY;
			MaxX= SystemInformation.VirtualScreen.Width; 
			MaxY= SystemInformation.VirtualScreen.Height ; 

			Label lbl= new Label();
			lbl.Text= en.sysstr.GetString(SSType.Title);
			lbl.Font= new System.Drawing.Font("Times New Roman", 16, FontStyle.Bold); 
            lbl.Location= new Point( MaxX*4/10, MaxY* 15/100);
			lbl.Width= MaxX*2/10; 
			this.Controls.Add (lbl);

			Button btn= new Button();
			btn.Text=en.sysstr.GetString(SSType.Exit);
			btn.Location= new Point( MaxX*4/10, MaxY* 8/10);
			btn.Width= MaxX*2/10; 
			btn.Click +=ExitClick;
			this.Controls.Add(btn);

		    btn= new Button();
			btn.Text=en.sysstr.GetString(SSType.ResumeGame);
			btn.Location= new Point( MaxX*4/10, MaxY* 3/10);
			btn.Width= MaxX*2/10; 
			btn.Click +=ResumeClick;
			btn.Visible=false;
			btnResume=btn;
			this.Controls.Add(btn);
		
			btn= new Button();
			btn.Text=en.sysstr.GetString(SSType.NewGame);
			btn.Location= new Point( MaxX*4/10, MaxY* 4/10);
			btn.Width= MaxX*2/10; 
			btn.Click +=NewGameClick;
			this.Controls.Add(btn);

			btn= new Button();
			btn.Text=en.sysstr.GetString(SSType.LoadGame);
			btn.Location= new Point( MaxX*4/10, MaxY* 5/10);
			btn.Width= MaxX*2/10; 
			btn.Click +=LoadGameClick;
			this.Controls.Add(btn);

			btn= new Button();
			btn.Text=en.sysstr.GetString(SSType.SaveGame);
			btn.Location= new Point( MaxX*4/10, MaxY* 6/10);
			btn.Width= MaxX*2/10; 
			btn.Click +=SaveGameClick;
			this.Controls.Add(btn);
			btnSave = btn;
			btnSave.Enabled = false;

			btn= new Button();
			btn.Text=en.sysstr.GetString(SSType.Preference);
			btn.Location= new Point( MaxX*4/10, MaxY* 7/10);
			btn.Width= MaxX*2/10; 
			btn.Click +=PreferenseClick;
			this.Controls.Add(btn);





		}


		
	

		private void ResumeClick ( object sender, EventArgs arg)
		{ this.Hide ();
			frm.Show ();


		}
	
		private void ExitClick ( object sender, EventArgs arg)
		{
			Application.Exit(); 
		}



		private void NewGameClick ( object sender, EventArgs arg)
		{
			frm= new WorkForm(en);
			btnResume.Visible=true;
			en.newGame();
			this.Hide ();
			frm.Show ();


		}

		private void LoadGameClick ( object sender, EventArgs arg)
		{
			btnResume.Visible=true;
			this.Hide ();
			//порядок важен
			frm= new WorkForm(en);
			if (! en.io.LoadSaveFile ("test"))
				Application.Exit ();

		}

		private void SaveGameClick ( object sender, EventArgs arg)
		{ 
			en.io.CreateSaveFile ("test"); 

		}


		private void PreferenseClick ( object sender, EventArgs arg)
		{
			MessageBox.Show ("Pref");

		}

		public  static  void Main (string[] args)
		{ CommandLineArgs=args;
			MainMenuForm frm= new MainMenuForm();
			if(isPlayMode) 
			Application.Run (frm);

		}

		private  bool GlobalInit()
		{ en= new GameEngine(this);
			return en.Init(); 

		}

		public void SaveEnabled(bool mode)
		{ btnSave.Enabled = mode;  
		
		}

	}
}

