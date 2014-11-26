using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts; 
using System.IO;

namespace mmxAH
{  


	public enum IOMode 
	{ Pause=1,
	  YesNo,
	  Choose,
	  Stop,
      StandAloneActhion

	}


	public enum FormMode
	{ Log=1,
      Map,
      Status,
      Monsters,
      Ao,
      Investigators,
      Ow   

	}

	public  abstract class IOOption
	{ public string Title;
	  public virtual void Execute ()
		{

		}

	}

	public class IOOptionWithParam: IOOption
	{ 	private FuncWithParam chooseFunc;
		private short Param;
		public IOOptionWithParam( string T, FuncWithParam  f, short p)
		{Title=T;
		chooseFunc=f;
		 Param=p;  
		}
		public override void Execute ()
		{
			chooseFunc.BeginInvoke (Param, null,null);
		}

	}

	public class IOOpthionWithoutParam : IOOption
	{  private Func fu;
		public IOOpthionWithoutParam( string tit,Func fp)
		{ Title = tit;
			fu = fp;

		}
		public override  void Execute()
		{
			fu.BeginInvoke(null,null) ;
		}


	}


	[Synchronization] 
	public class IOClass
	{   public IOMode mode;
		public string PauseTitle, ChooseEndTitle, YesTitle, NoTitle, QuestTitle;
		public List<IOOption> options; 
		private Func FAfterPause, FYes, FNo;
		 private WorkForm frm;
		private FormMode frmMode;
		private List< LogEntry > log; 
		private GameEngine en;
		private byte sideCode=0; //0-netral, 1-Server, 2-server
		private bool isServerUpdate = false;
		private MainMenuForm mmFrm;


		private FormMode serverFormMode;
		public IOClass (WorkForm frmp, GameEngine eng, MainMenuForm frmMM)
		{
		  mode= IOMode.Stop;
			frmMode = FormMode.Log;
			frm = frmp;
			log = new List<LogEntry> (); 
			en = eng;
			mmFrm = frmMM; 
		}


		public void Pause( Func f, string title="Next")
		{  FAfterPause=f;
		   PauseTitle=title; 
		   mode=IOMode.Pause; 
			isServerUpdate = true;  

         

		}

		public void YesNoStart (string q, string aYes, string aNo, Func y, Func n)
		{ FYes=y;
		  FNo=n;
		  YesTitle=aYes;
		  NoTitle=aNo;
		  QuestTitle=q; 
		  mode= IOMode.YesNo; 
			isServerUpdate = true; 

		}


		public void PauseEnd ()
		{ mode= IOMode.Stop; 
			FAfterPause.BeginInvoke (null, null); 
		}

		public void Answer( bool a)
		{  mode=IOMode.Stop; 
			if(a)
				FYes.BeginInvoke(null, null) ;
			else
				FNo.BeginInvoke(null, null);

		}

		public void StartChoose( List<IOOption> opts, string question="Select acthion", string buttonName="Confirm")
		{
			mode= IOMode.Choose;
			QuestTitle=question;
			ChooseEndTitle=buttonName;
		    options =opts;
			isServerUpdate = true; 


		}

		public void ChooseEnd (int selectedOption)
		{mode= IOMode.Stop;
		 options[selectedOption].Execute();


		}

		public void StandAloneStart( Func f)
		{ FYes=f; 
			mode= IOMode.StandAloneActhion; 

		}

		public void StandAloneResponse()
		{ mode= IOMode.Stop; 
			FYes.BeginInvoke(null,null) ;
		}

		public void ClientWrite (string Text, byte fontsize=12, bool isBold=false, bool isItalic=false, byte  label=1, string ChooseColor="Black")
		{  if (sideCode != 1) 
				return;


			frm.ShowLabelText (Text, fontsize, isBold, isItalic, label, ChooseColor); 
			if ( frmMode == FormMode.Log)
				log.Add ( new LogEntry(Text, fontsize, isBold,isItalic, label, ChooseColor ));   
		}



		public void ServerWrite (string Text, byte fontsize=12, bool isBold=false, bool isItalic=false, byte  label=1, string ChooseColor="Black")
		{ do
			{

			} while( sideCode != 0);

			sideCode = 2;
			SetServerFormMode (FormMode.Log);  
		




		
			log.Add ( new LogEntry(Text, fontsize, isBold,isItalic, label, ChooseColor ));   
			sideCode = 0; 

		}

		private  void ShowLog ()
		{   
			frm.mapMode(false);

		

			LogEntry le;
			for (int i=0; i< log.Count; i++)
			{ le= log[i];
				frm.ShowLabelText (le.Text, le.size, le.isBold, le.isItalic, le.label, le.cl);    
			}

			frmMode = FormMode.Log; 
		}

		private struct LogEntry
		{ public string Text;
			public byte size;
			public bool isBold;
			public bool isItalic;
			public byte label;
			public string cl;
			public LogEntry( string t, byte s, bool ib, bool ii, byte l, string c)
			{ Text=t;
				size=s;
				isBold=ib;
				isItalic=ii;
				label=l;
				cl=c;

			}


		}

		public void SetFormMode( FormMode md)
		{  if (sideCode != 0)
				return;
			sideCode = 1;
			frmMode = md;
			switch (md)
			{ case FormMode.Log: ShowLog (); break; 
			case FormMode.Map: en.map.Print ();  break;
			case FormMode.Investigators: PrintInvist (); break;
			case FormMode.Ow:{frm.mapMode (false); en.ows.Print (); } break;   
			case FormMode.Monsters: PrintMonsters (); break; 
			case FormMode.Status:{frm.mapMode (false); en.status.Print (); } break;   
			}
			sideCode = 0;

		}

		public void Set3LabelView()
		{  if (sideCode != 1)
				return;
			frmMode = FormMode.Map;
			frm.mapMode (true); 

		}

		public void  ArrengeLabels()
		{
			frm.ArrengeLabels (); 
		}

		private void PrintInvist()
		{ frm.mapMode (false);
			byte i = en.clock.GetCurPlayer (); 
			do
			{
				en.ActiveInvistigators [i]. Print (); 
				ClientWrite (Environment.NewLine);
				ClientWrite (Environment.NewLine);
				i++;
				if( i== en.GetPlayersNumber())
					i=0;
			}
			while( i != en.clock.GetCurPlayer ()); 
		} 


		private void PrintMonsters()
		{ frm.mapMode (false);
			foreach (MonsterPrototype  pr in en.MonsterPrototypes)
				pr.isPrinted = false; 
			foreach (MonsterIndivid m  in en.ActiveMonsters)
				m.PrintClient (); 
		} 



		public void CheckServerUpdate()
		{ //вызываеться из таймера Основной формы

				
				


			if (isServerUpdate)
			{
			 
				frm.RefreshControls ();
				SetFormMode (serverFormMode);
				isServerUpdate = false;
			}


			
				
		





		}

		public void SetServerFormMode( FormMode mode)
		{
			serverFormMode = mode;

		}

	
		public void SetSaveEnable(bool mode)
		{
			mmFrm.SaveEnabled (mode); 
		}


		public void CreateSaveFile( string SaveName="")
		{ 
	

			short maxNum = 0;
			DirectoryInfo di=  new DirectoryInfo(TextFileParser.CreatePath("Saves"));
			if (! di.Exists)
			{ di.Create (); 
				if( SaveName  == "")
				SaveName  = "AH_save_1";
			} else
			{  if (SaveName == "")
				{
					foreach (FileInfo fi in di.GetFiles())
					{
						if (fi.Name.StartsWith ("AH_save_"))
						{
							string numstr = fi.Name.Substring (8);
							numstr = numstr.Substring (0, numstr.Length - 4);
							short num;
							if (short.TryParse (numstr, out num) && num > maxNum)
								maxNum = num;

						}

					}

					SaveName  = "AH_save_" + (maxNum + 1); 
				}

			}

			
			SaveName = System.IO.Path.Combine("Saves", SaveName+".xge");
			new BinaryWorker (en, SaveName).ToSave ();   


		}

		public bool LoadSaveFile(string SaveName)
		{
		SaveName=System.IO.Path.Combine("Saves", SaveName+".xge");
			if (!  new BinaryWorker (en, SaveName).FromSave ())
				return false;
			frm.Show ();
			en.clock.ResumeFromSave ();
			return true;
		              
		}

		public void ShowMainMenu()
		{
			mmFrm.Show (); 
		}


		public void ClientPrintTag( string str)
		{ string str_print = "";
			int bi, ii;
			do
			{ bi=str.IndexOf ("{b}");
				ii=str.IndexOf ("{i}");
				if ( bi== ii) // только если обоих нету
				{
					ClientWrite (str);
					str = "";
				}
				else if( bi != -1 && ii != -1)
					{
						if( bi> ii) //курсив раньше
					{ str_print= str.Substring(0,ii); 
						en.io.ClientWrite(str_print);
						str= str.Substring( ii+3);
						ii=str.IndexOf ("{i}");
						str_print= str.Substring(0,ii); 
						en.io.ClientWrite(str_print,12,false,true);
						str= str.Substring( ii+3);
						}
						else  //жирный раньше
					{str_print= str.Substring(0,bi); 
						en.io.ClientWrite(str_print);
						str= str.Substring( bi+3);
						ii=str.IndexOf ("{b}");
						str_print= str.Substring(0,ii); 
						en.io.ClientWrite(str_print,12,true);
						str= str.Substring( ii+3);

						}
					}
					else if( bi != -1)
					{ // только жирность
					str_print= str.Substring(0,bi); 
					en.io.ClientWrite(str_print);
					str= str.Substring( bi+3);
					ii=str.IndexOf ("{b}");
					str_print= str.Substring(0,ii); 
					en.io.ClientWrite(str_print,12,true);
					str= str.Substring( ii+3);
					}
					else
					{ // только курсив
					str_print= str.Substring(0,ii); 
					en.io.ClientWrite(str_print);
					str= str.Substring( ii+3);
					ii=str.IndexOf ("{i}");
					str_print= str.Substring(0,ii); 
					en.io.ClientWrite(str_print,12,false,true);
					str= str.Substring( ii+3);
					}




			} while(str != "");
		}

		public void ServerPrintTag( string str)
		{ string str_print = "";
			int bi, ii;
			do
			{ bi=str.IndexOf ("{b}");
				ii=str.IndexOf ("{i}");
				if ( bi== ii) // только если обоих нету
				{
					ServerWrite (str);
					str = "";
				}
				else if( bi != -1 && ii != -1)
				{
					if( bi> ii) //курсив раньше
					{ str_print= str.Substring(0,ii); 
						en.io.ServerWrite(str_print);
						str= str.Substring( ii+3);
						ii=str.IndexOf ("{i}");
						str_print= str.Substring(0,ii); 
						en.io.ServerWrite(str_print,12,false,true);
						str= str.Substring( ii+3);
					}
					else  //жирный раньше
					{str_print= str.Substring(0,bi); 
						en.io.ServerWrite(str_print);
						str= str.Substring( bi+3);
						ii=str.IndexOf ("{b}");
						str_print= str.Substring(0,ii); 
						en.io.ServerWrite(str_print,12,true);
						str= str.Substring( ii+3);

					}
				}
				else if( bi != -1)
				{ // только жирность
					str_print= str.Substring(0,bi); 
					en.io.ServerWrite(str_print);
					str= str.Substring( bi+3);
					ii=str.IndexOf ("{b}");
					str_print= str.Substring(0,ii); 
					en.io.ServerWrite(str_print,12,true);
					str= str.Substring( ii+3);
				}
				else
				{ // только курсив
					str_print= str.Substring(0,ii); 
					en.io.ServerWrite(str_print);
					str= str.Substring( ii+3);
					ii=str.IndexOf ("{i}");
					str_print= str.Substring(0,ii); 
					en.io.ServerWrite(str_print,12,false,true);
					str= str.Substring( ii+3);
				}




			} while(str != "");
		}

	}
}


/*Вхоимодействие клиента и сервера
  Весь вывод клиента- из кэшиирющих списков LogEntry( по одному для каждого режима).
  При запросе пользователя ( Pause, YesNo, Choose)  посылаються новые содержания этимх листов.
  Они сохраняються в ОТДЕЛЬНЫЕ вспомогательные списки. 
  Только поосле формированиия этих списков на клиенте ставиться isServerUpdate.
В функции вызываемой из таймера основной формы(CheckServerUpdate) вспомогательные списки копируються в основные, и 
отображается режим запрошеный с сервера ( храниться в отдельной переменой).
*/
