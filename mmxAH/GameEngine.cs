using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
namespace mmxAH
{ public delegate void Func();
	public delegate  void FuncWithParam( short param);
	public delegate bool FuncBoolReturn(short param);
	public class GameEngine
	{   public PairMeneger  ds, colors;
		private List<Limits> lims;
		private const string BinaryFileName="data.bin";
		private const string TextFileName="data.txt";
		private const string LangvFileName="langv_eng.txt";
		public List<Locathion> locs; 
		public Deck<GatePrototype> gates; 
		public OWMeneger ows;
		public IOClass io;
		public List<Investigator> ActiveInvistigators;
		public List<MonsterPrototype> MonsterPrototypes; 
		public List<MonsterIndivid> ActiveMonsters;
		public Deck<MonsterIndivid> MonstersCup;
		public SystemStrings sysstr;
		public PhasesClock clock;
		public MapOfCity map;
		private Limits lim;
		public Preference pref;
		private Deck<Investigator> invests; 
		public SkillTestInfos GlobalModifs;
		public List<SpecialCardText> scTexts;
		private MainMenuForm mmFrm;
		public Deck< MythosCard> mythosDeck;
		public CurStorage curs;
		public GlobalActhions ga;
		public Deck< OWEncCard> owEnc;
		public  Deck< ArcEncCard> [] archEncs;
		public GlobalStatus status;
		public List< MonsterIndivid> Outscirts; 
		public List< MonsterIndivid> Scy;



		public GameEngine ( MainMenuForm pMmFrm)
		{ 
			ds= new PairMeneger() ; 
			colors= new PairMeneger();
			lims= new List<Limits>();
			locs= new List<Locathion>();
			gates= new Deck<GatePrototype>( );
			ows= new OWMeneger(this); 
			ActiveInvistigators= new List<Investigator>();
			ActiveMonsters= new List<MonsterIndivid>();
			MonstersCup = new Deck<MonsterIndivid>(true); 
			sysstr= new SystemStrings();  
			clock= new PhasesClock(this);
			pref = new Preference ();
			invests = new Deck<Investigator> ();
			GlobalModifs = new SkillTestInfos (); 
			scTexts = new List<SpecialCardText> ();
			mmFrm = pMmFrm; 
			mythosDeck = new Deck<MythosCard> (); 
			MonsterPrototypes = new List<MonsterPrototype> (); 
			curs = new CurStorage (this);
			ga = new GlobalActhions(this);
			owEnc = new Deck<OWEncCard> ();
			status = new GlobalStatus (this);
			Outscirts= new List<MonsterIndivid>(); 
			Scy = new List<MonsterIndivid> ();


		
		}

		public bool Init ()
		{ if (File.Exists (TextFileParser.CreatePath (BinaryFileName)))
			return new BinaryWorker (this, BinaryFileName).BinaryInit ();   
			else
				return new TextWorker (this).TextInit (TextFileName, LangvFileName);   
        
			
		}


		public bool LimitsFromText( TextFileParser prs)
		{ int cn;
			if (! Int32.TryParse (prs.GetToken (), out cn))
				return false;
			Limits cl = new Limits ();
			for (int i=0; i< cn; i++)
			{
				if (! Byte.TryParse (prs.GetToken (), out cl.players))
					return false;
				if (! Byte.TryParse (prs.GetToken (), out cl.gates))
					return false;
				if (! Byte.TryParse (prs.GetToken (), out cl.mA))
					return false;
				if (! Byte.TryParse (prs.GetToken (), out cl.mO))
					return false;
				lims.Add (cl);
				cl = new Limits ();

			}
			return true;
		}

		public bool  InvestigatorsFromText(TextFileParser prs, TextFileParser text)
		{ int cn;
			if (! Int32.TryParse (prs.GetToken (), out cn)) 
			return false;
			Investigator inv;
			for (short i=0; i<cn; i++)
			{ inv = new Investigator (this, i);
				if (! inv.FromTextFile (prs, text))
					return false;
				invests.Add(inv);
			}
			return true;

		}


		private void Test ()
		{ 
		 





		}


	
	






		private struct Limits
		{ public byte players;
		   public byte gates;
			public byte mA;
			 public byte mO;

		}

		public void LimitsFromBinary (BinaryReader rd)
		{
			int limc = rd.ReadInt32 ();
			Limits l;
			for (int i=0; i< limc; i++)
			{ l= new Limits();
				l.players= rd.ReadByte();
				l.gates= rd.ReadByte();
				l.mA=rd.ReadByte ();
				l.mO=rd.ReadByte (); 
				lims.Add (l);

			}

		}


		public void LimitsToBinary (BinaryWriter  wr)
		{
			wr.Write (lims.Count);
			foreach (Limits l in lims)
			{ wr.Write ( l.players);
				wr.Write ( l.gates);
				wr.Write ( l.mA);
				wr.Write ( l.mO);

			}

		}
		public void newGame ()
		{  
			Reset (); 
			io.ServerWrite (sysstr.GetString (SSType.Setup_Started) + "  "+ DateTime.Now+"."+ Environment.NewLine );  
			List<IOOption> ioopts = new List<IOOption > ();

			for( short i=1; i<= lims.Count; i++)
				ioopts.Add( new IOOptionWithParam(i.ToString(), NewGame_NumOfInv,i));

			io.StartChoose (ioopts, sysstr.GetString (SSType.Setup_NumOfInv_Promt), sysstr.GetString (SSType.Confirm));




		}


				          
	    private void NewGame_NumOfInv(short num)
		{   lim = lims [num-1];
			io.ServerWrite (sysstr.GetString (SSType.Setup_NumOfInv_Fact) + " : " + num+"."+ Environment.NewLine);  
			status.Init (lim.gates, lim.mA, lim.mO);
			//Random inv
			for (byte i=0; i<num; i++)
			{ ActiveInvistigators.Add (invests.Draw());
				ActiveInvistigators [i].Setup (i);

			}
			Test ();
			clock.StartRandomSetup ();
		}

       

	
  	   
   

		/*private Bitmap CreateImage (Bitmap main, Bitmap newImag, int x, int y)
		{ Graphics g= Graphics.FromImage( main); 
			g.DrawImage(newImag, x, y);   
			return main;
		}
   
        */  


		public   void ToBin()
		{
			 WorkForm frm= new WorkForm(this, true);
			io.StandAloneStart(ToBin2); 
			System.Windows.Forms.Application.Run (frm);




		}

		public void ToBin2 ()
		{
			io.ClientWrite ("Convert text files to binary" + Environment.NewLine, 16, true); 
			io.ClientWrite ("Data file: " + TextFileName + Environment.NewLine); 
			io.ClientWrite ("Strings file: " + LangvFileName + Environment.NewLine); 
			io.ClientWrite ("Loading data...");  
			if (! new TextWorker (this).TextInit (TextFileName, LangvFileName))
			{ io.ClientWrite ("Error !!!",16, true);  
				return;

			}
			io.ClientWrite("OK."+ Environment.NewLine);     
			new BinaryWorker(this, "data.bin").ToBin() ; 

		}

		public byte GetPlayersNumber()
		{
			return lim.players; 
		}


		public void SetWorkForm( WorkForm frm)
		{ io = new IOClass (frm,this, mmFrm ); 

		}


		public void Reset()
		{ //Здесь сброс AO ( включая эффекты) 

			curs.Reset ();

			for (int i=0; i< ActiveInvistigators.Count; i++)
			{
				invests.Add (ActiveInvistigators [i]);
				ActiveInvistigators [i].Reset ();
			}
			ActiveInvistigators.Clear ();  
		   for (int i=0; i< ActiveMonsters .Count; i++)
				MonstersCup .Add (ActiveMonsters [i]);
			ActiveMonsters.Clear (); 
			foreach (Locathion l in locs)
				l.Reset ();
			foreach (MonsterIndivid m in Outscirts)
				MonstersCup.Add (m); 
			clock.Reset ();
			status.Reset();
			ResetOutscirts (); 

			//самое последнее действие , после всех резетов
			MonstersCup.Reset ();
			mythosDeck.Reset ();
			gates.Reset ();
			invests.Reset ();
			owEnc.Reset ();


		}

		public void ResetOutscirts()
		{ foreach (MonsterIndivid m in Outscirts)
			MonstersCup.Add (m); 
			Outscirts.Clear (); 

		}


		public void SaveActive(System.IO.BinaryWriter  wr)
		{  wr.Write (lim.players);
			foreach (Investigator inv in ActiveInvistigators)
				inv.WriteToSave (wr);
			wr.Write (ActiveMonsters.Count);
			foreach (MonsterIndivid mon in ActiveMonsters)
				mon.WriteToSave (wr); 
			wr.Write (Outscirts .Count);
			foreach (MonsterIndivid mon in Outscirts )
				mon.WriteToSave (wr); 




		}


		public void LoadActive(System.IO.BinaryReader  rd)
		{ byte numPlayers = rd.ReadByte ();
			lim = lims [numPlayers - 1];
			status.Init (lim.gates, lim.mA, lim.mO);  
			for (byte i=0; i<numPlayers; i++)
			{ ActiveInvistigators.Add (invests.GetCardById (rd.ReadInt16 ()));
				ActiveInvistigators [i].ReadFromSave (rd);  

			}

			int numMonsters = rd.ReadInt32 ();
			for (int i=0; i<numMonsters; i++)
			{ ActiveMonsters.Add (MonstersCup .GetCardById (rd.ReadInt16 ()));
				ActiveMonsters [i].ReadFromSave (rd);  

			}


		      numMonsters = rd.ReadInt32 ();
			for (int i=0; i<numMonsters; i++)
			{Outscirts .Add (MonstersCup .GetCardById (rd.ReadInt16 ()));
				Outscirts  [i].ReadFromSave (rd);  

			}

		}

	}
}

