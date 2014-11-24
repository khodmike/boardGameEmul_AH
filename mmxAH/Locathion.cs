using System;
using System.Collections.Generic;
namespace mmxAH
{
	public  abstract class Locathion
	{   protected GameEngine en;
		protected string codeName;
		protected string displayName;
		protected string MoveToTitle;
		protected short  LocathionIndex;
		protected List<byte> investigators;
		protected LocathionType type;
		public  Locathion ()
		{ investigators =  new List<byte> ();
			codeName="";
		}
		public string GetTitle ()
		{
			return displayName;
		}
		public string GetCodeName ()
		{
			return codeName;
		}

		public void SetLocIndex( short ind)
		{
			LocathionIndex =ind;
		}

		public void AddInvestigator (byte invest)
		{ 
			investigators.Add (invest);

		}


		public void RemoveInvestigator(byte invest)
		{ int  ind = investigators.IndexOf(invest);

			if( ind != -1)
				investigators.RemoveAt(ind);

		}

		public LocathionType GetLocType()
		{ return type;

		}

		public abstract void Move();
		public abstract bool FromText( TextFileParser prs, TextFileParser text);
		public abstract void Print( byte label);
		public abstract void FromBin (System.IO.BinaryReader  rd);
	    protected void FromBin1(System.IO.BinaryReader rd)
		{  type= (LocathionType)rd.ReadInt32();  
			codeName = rd.ReadString ();
			displayName = rd.ReadString ();
			MoveToTitle = rd.ReadString ();  

		}
		public abstract void ToBin (System.IO.BinaryWriter wr);
		protected void ToBin1(System.IO.BinaryWriter wr)
		{ wr.Write ((int)type); 
		  wr.Write (codeName);
		  wr.Write (displayName);
		  wr.Write (MoveToTitle); 

		}
		protected  bool FromText3 (TextFileParser prs, TextFileParser text)
		{
			if (codeName == "")
			{
				codeName = prs.GetToken ();
				if (codeName == null)
					return false;

			}
			displayName = text.GetCurString ();
			if (displayName == null)
			{
				System.Windows.Forms.MessageBox.Show (" Could not load loc text . Code Name:" + codeName);
				return false;
			}
			if (text.isMultiName)
			{
				MoveToTitle = text.GetCurString ();
				if (MoveToTitle  == null)
				{
					System.Windows.Forms.MessageBox.Show  (" Could not load loc text . Code Name:" + codeName);
					return false;
				}
			}
			else
				MoveToTitle=displayName;  
			return true;
		}

		public string GetMoveToTitle()
		{
			return MoveToTitle;
		}

		public override string ToString ()
		{
			return "lt: "+  type.ToString()+ "  name: " + codeName;   
		}

		public virtual  void Reset()
		{ investigators.Clear (); 

		}

		public virtual  void WriteToSave(System.IO.BinaryWriter wr)
		{

		}

		public virtual void ReadFromSave( System.IO.BinaryReader rd)
		{


		}

		public virtual void Encounter()
		{
			en.clock.NextPlayer ();
		}


		public byte GetInvestCount()
		{ return (byte) investigators.Count; 

		}

		public byte GetMinSneak()
		{ byte min = 250;
			byte cur;
			Investigator inv;
			foreach( byte invnum  in investigators)
			{ inv = en.ActiveInvistigators [invnum];  
				cur = (byte) (inv.GetCharValue (SkillTestType.Sneak));  
				if (cur < min)
					min = cur;

			}
			return min;

		}
	}



	public class Litas: Locathion
	{ public Litas( GameEngine eng)
		{ LocathionIndex=0;
		type= LocathionType.LiTaS;  
			en=eng;
			codeName="LITAS"; 
		}

		public override void Move ()
		{ 


		}

	
	 public override   bool FromText( TextFileParser prs, TextFileParser text)
		{
			return true;
		}

		public override void Print (byte label)
		{

		}

		public override void ToBin (System.IO.BinaryWriter wr)
		{

		} 

		public override void FromBin (System.IO.BinaryReader rd)
		{

		}


	}


	public class OWLoc: Locathion
	{ private List<byte>  colors;
		private List<byte> investigatorsArea2;
		private short GateModif=0;
		private byte GateDif=1;

		public OWLoc (GameEngine eng)
		{ en=eng;
			colors= new List<byte>();
			investigatorsArea2=new List<byte>(); 
			type=mmxAH.LocathionType.OW;
		}

		public void SetGateDif (byte dif)
		{
			GateDif=dif;
		}
		public byte GetGateDif ()
		{
			return GateDif;
		}

		public short GetGateModif()
		{
			return GateModif; 
		}
		public override void Move ()
		{ byte invest = en.clock.GetCurPlayer ();
			en.io.SetServerFormMode (FormMode.Ow);
			if (! en.ActiveInvistigators [invest].isCanMove)
			{
				en.clock.NextPlayer ();
				return;
			}
			//A2-> RetToArch
			if (investigatorsArea2.IndexOf (invest) != -1)
			{ 	investigatorsArea2.Remove(invest);  
				en.ga.ReturnToArchem (LocathionIndex, en.ActiveInvistigators[invest].MoveChicle );
			
			}
			//A1->A2
			if (investigators.IndexOf (invest) != -1 )
			{ investigators.Remove(invest);
			 investigatorsArea2.Add(invest);
				en.io.ServerWrite (en.ActiveInvistigators [invest].GetTitle(), 12, false, true); 
				en.io.ServerWrite (" "+en.sysstr.GetString (SSType.MoveToFact)+ " ");
				en.io.ServerWrite (displayName + "  "+  en.sysstr.GetString (SSType.OWArea) + "  2.", 12, true); 
				en.clock.NextPlayer ();
			}


		}



		public  string GetDescripthionToGate ()
		{ string sd= GetShortDescripthionToGate();
			sd= sd.Remove (sd.Length-1); 

			foreach( byte cl in colors)
				sd+=","+en.colors.GetTitle(cl);
			sd+=")";
			return sd ;

		}

		public string GetShortDescripthionToGate ()
		{

			 string str= MoveToTitle ;

			str+= "(";
			if( GateModif > 0)
				str+="+";
			str+=GateModif.ToString();
			if( GateDif != 1)
				str+=",dif "+ GateDif.ToString();
			str+=")";
			return str;  
		}


		public override  bool FromText (TextFileParser prs, TextFileParser text)
		{
			 if( ! base.FromText3(prs,text))
				return false;
			byte cn;
			int c;
			if (! Byte.TryParse (prs.GetToken (), out cn)) 
				return false;
			for (int i=0; i< cn; i++)
			{ 
				c= en.colors.GetIndex( prs.GetToken());
				if( c== -1)
					return false;
				colors.Add ((byte)c); 

			}
			if (! short.TryParse (prs.GetToken (), out GateModif )) 
				return false;

			return true;
		}


		public override void Print (byte label)
		{ 
			string str = displayName + " ( ";
			for (byte i=0; i< colors.Count; i++)
			{
				if (i != 0)
					str += " , ";
				str += en.colors.GetTitle (colors [i]);
			}
			str += ")" ;
			en.io.ClientWrite(str, 14, true, false, label); 

			  str= Environment.NewLine + en.sysstr.GetString(SSType.OWArea)+ " 1: ";
               if (investigators.Count == 0)
				str+= "-";
			   else
               foreach( byte invest in investigators )
					str+= "  "+ en.ActiveInvistigators[invest].PrintToMap();  
				en.io.ClientWrite(str, 12, false, false, label); 



			  str= Environment.NewLine + en.sysstr.GetString(SSType.OWArea)+ " 2: ";
                if (investigatorsArea2.Count == 0)
				str+="-";
			    else
               foreach( byte invest in investigatorsArea2 )
					str+= "  "+ en.ActiveInvistigators[invest].GetTitle();  
				en.io.ClientWrite(str, 12, false, false, label); 


			en.io.ClientWrite(Environment.NewLine, 12, false, false, label); 

		}


		public override void ToBin (System.IO.BinaryWriter wr)
		{
			ToBin1 (wr);
			wr.Write (GateModif);
			wr.Write (colors.Count);
			foreach (byte num in colors)
				wr.Write (num);
		}

		public override void FromBin (System.IO.BinaryReader rd)
		{
			FromBin1(rd);
			GateModif = rd.ReadSByte();
			int c = rd.ReadInt32 ();
			for (int i=0; i<c; i++)
				colors.Add (rd.ReadByte());

		}

		public override void Reset()
		{ base.Reset ();
			investigatorsArea2.Clear (); 
		}

		public override void Encounter ()
		{  /*OWEncCard c;
			do
			{ c= en.owEnc.Draw();
				if( colors.IndexOf( c.GetColor()) != -1  ) 
					break;
				en.owEnc.Discard(c);   
		    } while(true);
			c.Execute(LocathionIndex );  
			*/
			en.clock.NextPlayer (); 
	   }

		public override void WriteToSave (System.IO.BinaryWriter wr)
		{ wr.Write (investigators.Count);
			foreach (byte inv in investigators)
				wr.Write (inv);
			wr.Write (investigatorsArea2.Count);
			foreach (byte inv in investigatorsArea2)
				wr.Write (inv);

		}
		public override void ReadFromSave (System.IO.BinaryReader rd)
		{
			int num = rd.ReadInt32 ();
			for (int i=0; i< num; i++)
				investigators.Add (rd.ReadByte());
			num = rd.ReadInt32 ();
			for (int i=0; i< num; i++)
				investigatorsArea2.Add (rd.ReadByte());
		}
	
	}


	public abstract  class ArchemArea : Locathion
	{ protected List<MonsterIndivid > monsters;
		protected short BlackArrow=-1;
		protected short WhiteArrow=-1;
		protected List<short> OtherLinks;
		protected byte Districkt=0;
		protected bool isClosed;

		protected byte clues;
		private byte MPcost;
protected string GateAndClueTitle;


      public ArchemArea ()
		{ monsters= new List<MonsterIndivid >();
          OtherLinks= new List<short>(); 
			MPcost=1;
		}

		public string GetGateAndClueTitle()
		{ return GateAndClueTitle;

		}

		public byte GetDistrickt ()
		{
			return Districkt;
		}

      public short GetWhiteArrow()
		{
			return WhiteArrow;
		}

		public short GetBlackArrow ()
		{ return BlackArrow;

		}



        public void AddMonster (MonsterIndivid  mon)
		{ 
			monsters.Add (mon);

		}


		public void RemoveMonster(MonsterIndivid monster)
		{ monsters.Remove (monster);  

		}


		public void RemoveAllMonsters ()
		{
			monsters.Clear() ; 
		}


		public void SetClosed (bool closedStatus)
		{
			isClosed = closedStatus;  
			if (isClosed)
			{ foreach( MonsterIndivid  m in monsters)
				m.SetLocathion(BlackArrow);
              foreach( byte invnum in investigators)
					en.ActiveInvistigators[invnum].SetLocathion(BlackArrow);    

			}
		}

		public bool GetClosed ()
		{
			return isClosed; 
		}

		public void  AddClues ( byte count=1)
		{
			clues += count;


		}

		public void RemoveClues (byte count=1)
		{ if(clues >= count) 
			clues-= count; 

		}

		public void RemoveAllClues ()
		{
			clues=0;
		}

		public byte GetClues ()
		{ 
			return clues;
		}



		protected  bool FromText2( TextFileParser prs, TextFileParser text)
		{ 
			if( ! base.FromText3(prs, text))
			return false;

			//Связи грузиться в классе улицы/локации
			if( ! byte.TryParse(prs.GetToken(), out Districkt))
				return false;


				GateAndClueTitle = text.GetCurString ();
				if (GateAndClueTitle == null)
				{
					System.Windows.Forms.MessageBox.Show (" Could not load loc text . Code Name:" + codeName);
					return false;
				}

			return true;
		}


		public override void Move ()
		{ bool noMonsters = false;
			List<IOOption> opts = new List<IOOption> ();
			Investigator inv = en.ActiveInvistigators [en.clock.GetCurPlayer() ];

			foreach ( MonsterIndivid m in monsters)
				if (! m.isEncountred)
				{
					
				m.SetRPAfterEncounter (inv.MoveChicle);
					opts.Add (new IOOpthionWithoutParam (CreateMonsterString (m), m.BeginEncounter));      
				}

			if (opts.Count == 0 && inv.isCanMove)
			{ //Нет монстров с которыми не взаимодействовал => может двигаться
				noMonsters = true;



				if (BlackArrow != -1)
					CreateMoveOpthin (BlackArrow, inv, opts);  
				if (WhiteArrow != -1 && BlackArrow != WhiteArrow)
					CreateMoveOpthin (WhiteArrow, inv, opts);
				foreach (byte loci in OtherLinks)
					CreateMoveOpthin (loci, inv, opts);


			}

			if (inv.isMonsterImunity == true) 
				noMonsters = true;
			//вещи следователя
			foreach (IOOption opt in inv.myTrigers.GetChooseOpthions(TrigerEvent.MoveAH, inv.MovementPoints))
				opts.Add (opt);

			if (opts.Count == 0)
			 //Ничего не может делать => конец фазы
				en.clock.EndMovementSegment ();
			else
			{  if( noMonsters) 
					opts.Add( new IOOpthionWithoutParam(en.sysstr.GetString(SSType.MovementEndPromt), en.clock.EndMovementSegment));    
				en.io.SetServerFormMode (FormMode.Map ); 
				en.io.StartChoose (opts,  en.sysstr.GetString (SSType.RemainMP )+ "  "+ inv.MovementPoints + Environment.NewLine +  en.sysstr.GetString (SSType.ChooseActhioPromt), en.sysstr.GetString (SSType.ChooseActhionButton));   
			}
		}

		private string CreateMonsterString (MonsterIndivid monster)
		{  return en.sysstr.GetString (SSType.MonsterEncounter) + "  " + monster.GetTitle ();

		}

		private void CreateMoveOpthin (short loci, Investigator inv, List<IOOption> opts )
		{ if( ((ArchemArea) en.locs[loci]).GetCost() > inv.MovementPoints)
			return;
			string title= en.sysstr.GetString (SSType.MoveToPropos) + "  " + en.locs[loci].GetMoveToTitle();
		  opts.Add ( new IOOptionWithParam(title, inv.MoveTo, loci)); 

		}


		public void SetCost (byte newCost)
		{
			MPcost= newCost; 
		}

		public byte GetCost()
		{
			return MPcost;
		}

		public override string ToString ()
		{
			string res =  base.ToString()+ " district: "+ Districkt.ToString()+ " ba: ";
			if( BlackArrow != -1)
				res += en.locs[BlackArrow].GetCodeName(); 
			else
				res+= "none";
			res+= " wa: ";
			if( WhiteArrow != -1)
			res+=en.locs[WhiteArrow].GetCodeName();
			else
				res+="none";

			foreach ( short loci in OtherLinks)
				res+= " other: " + en.locs[loci].GetCodeName();
			return res;

		}


		protected void PrintClosed( byte label)
		{ if(isClosed)
			en.io.ClientWrite (en.sysstr.GetString(SSType.ClosedFact), 12, true, false, label);  

		}

		protected void PrintInvestAndMonsters ( byte label)
		{ en.io.ClientWrite( Environment.NewLine, 10, false, false, label); 

			foreach( byte invnum in investigators)
				en.io.ClientWrite (en.ActiveInvistigators[invnum].PrintToMap() +Environment.NewLine, 12, false, true, label);

			foreach (MonsterIndivid m in monsters)
			{
				m.PrintToMap (label); 
				en.io.ClientWrite (Environment.NewLine,12,false,false, label); 
			}



		}


		protected  void ToBin2( System.IO.BinaryWriter wr)
		{ ToBin1 (wr);
			wr.Write (Districkt);
			wr.Write (BlackArrow);
			wr.Write (WhiteArrow);
			wr.Write (OtherLinks.Count);
			foreach (short ln in OtherLinks)
				wr.Write (ln);
			wr.Write (GateAndClueTitle); 
		}

		protected void FromBin2(System.IO.BinaryReader rd)
		{ FromBin1 (rd);
			Districkt = rd.ReadByte();
			BlackArrow = rd.ReadInt16();
			WhiteArrow= rd.ReadInt16();
			int c = rd.ReadInt32();
			for (int i=0; i<c; i++)
				OtherLinks.Add (rd.ReadInt16());
			GateAndClueTitle = rd.ReadString ();  

		}

		public override void FromBin (System.IO.BinaryReader rd)
		{
			FromBin2 (rd);
		}

		public override void ToBin (System.IO.BinaryWriter wr)
		{
			ToBin2 (wr);
		}


		public void EndMovementClues( byte invest)
		{ //ПРЕДПОЛОЖЕНИЕ пОСЛЕ ПРОВЕРКИ УЛИК КОНЕЦ сЕГМЕНТА ДВИЖЕНИЕ (ьвсе остольные в конце движения до этого)
			if (clues == 0)
			{
				en.clock.NextPlayer ();
				return;
			}


			if (en.pref.isAutoCluePickup)
			{
			   AfterMoveClues2 (clues); 
				return;
			}

			List< IOOption> opts= new List<IOOption>(); 

			for(byte i=clues; i>= 1;  i--)
				opts.Add( new IOOptionWithParam (i.ToString(), AfterMoveClues2, i)); 
			opts.Add( new IOOpthionWithoutParam("0", en.clock.NextPlayer )); 
			string str = en.sysstr.GetString(SSType.CluesPromtBegin)  + "  "+ en.sysstr.GetNumberClueToken (clues);
			str +=  "  "+ en.sysstr.GetString(SSType.CluesPromtEnd)  ;
			en.io.StartChoose (opts, str, en.sysstr.GetString(SSType.ClueActhionButtonName)  );    

		}

		public void AfterMoveClues2( short number)
		{ string str = en.ActiveInvistigators [en.clock.GetCurPlayer ()].GetTitle() + "  "+ en.sysstr.GetString(SSType.Get) ;
			en.io.ServerWrite   (str); 
			str = " " + en.sysstr.GetNumberClueToken (number);
			en.io.ServerWrite   (str,12,false,false,1,"Green"); 
			str = " "+ GateAndClueTitle+ ", " ;
			en.io.ServerWrite   (str); 
			clues -= (byte)number;
			en.ActiveInvistigators [en.clock.GetCurPlayer ()].AddClues ((byte)number);
		  en.clock.NextPlayer (); 

		}


		public virtual void MythosClues()
		{  clues++;
			en.io.ServerWrite (en.sysstr.GetString (SSType.ClueAppear));
			en.io.ServerWrite("  " + GateAndClueTitle+ "."+ Environment.NewLine  ,12,false,true);

			if (investigators.Count == 0)
			{
				en.curs.resolvingMythos.Step3 (); 
			} else if (investigators.Count == 1)
			{
				if (en.pref.isAutoCluePickup)
					MythosClues3 (investigators [0]);
				else
					en.io.YesNoStart (en.sysstr.GetString (SSType.PickupClueOnMythosPromt), en.sysstr.GetString (SSType.Yes), en.sysstr.GetString (SSType.No), MythosClues2, en.curs.resolvingMythos.Step3);   


			} else
			{ //сДЕЛАТЬ дЛЯ НЕСКЛОЬКИХ СЛЕДОВАТЕЛЕЙ	

			}
		
		}

		private void MythosClues2()
		{
			MythosClues3 (investigators [0]);
		}
	private  void MythosClues3( short invNum)
		{ string str = en.ActiveInvistigators [invNum].GetTitle() + "  "+ en.sysstr.GetString(SSType.Get) ;
			en.io.ServerWrite   (str); 
			str = " " + en.sysstr.GetNumberClueToken (1);
			en.io.ServerWrite   (str,12,false,false,1,"Green"); 
			str = " "+ GateAndClueTitle+ ", " ;
			en.io.ServerWrite   (str); 
			clues--;
			en.ActiveInvistigators [invNum].AddClues (1);
			en.curs.resolvingMythos.Step3 (); 

		}


		public override void Reset()
		{ base.Reset ();
			clues = 0;
		  MPcost = 1; 
			monsters.Clear ();

		}


		public override void WriteToSave (System.IO.BinaryWriter wr)
		{
			wr.Write (clues);
			wr.Write (isClosed); 
		}

		public override void ReadFromSave (System.IO.BinaryReader rd)
		{
			clues = rd.ReadByte ();
			isClosed = rd.ReadBoolean ();  
		}


		public bool  MonseterPlaced( )
		{  MonsterIndivid m= en.MonstersCup.Draw ();
			if (m == null)
			{
				en.ga.Awekeen ();  
				return false;
			}
			if (en.status.IsMonserToPlace (m))
			{
				m.AddToMap (LocathionIndex);
				en.io.ServerWrite (m.GetTitle (), 12, true);
				en.io.ServerWrite ("  " + en.sysstr.GetString (SSType.MonsterPlacedVerb));
				en.io.ServerWrite ("  " + GateAndClueTitle + ".  ", 12, false, true); 
				en.status.PrintMonserCountServer (); 

			}
			return true;
		}

		public List<short> GetLinkedLocs()
		{ List<short> res= new List<short>();
			if (WhiteArrow != -1)
				res.Add (WhiteArrow);
			if (BlackArrow != -1 && WhiteArrow != BlackArrow)
				res.Add (BlackArrow);
			foreach (short loc in OtherLinks)
				res.Add (loc);
			return res;

		}

	}

	public class ArchemStreet: ArchemArea
	{  private List<short> LocathionInDistr;
	   

		public ArchemStreet( GameEngine eng, string cn)
		{ type= LocathionType.ArchamStreet; 
			LocathionInDistr= new List<short>();
			codeName=cn; 
			en=eng;
		}

		public void RemoveMonstersFromDistrict()
		{ monsters.Clear ();
          foreach( byte locindex in LocathionInDistr)
				((ArchemArea)en.locs[locindex]).RemoveAllMonsters(); 

		}

		public void DistrictCloseStatus( bool closeStatus)
		{ isClosed=closeStatus;  

          foreach( byte locindex in LocathionInDistr)
				((ArchemArea)en.locs[locindex]).SetClosed(closeStatus); 

		}




		public  override bool FromText (TextFileParser prs, TextFileParser text)
		{ 
			if (! base.FromText2 (prs, text))
				return false;

			string tok;
			short loci;
			while ((tok=prs.GetToken().Trim ().ToUpper())  != "END" && tok != null)
			{ loci= en.map.GetNumberByCodeName( prs.GetToken().Trim ().ToUpper());
				if(loci == -1)
					return false;
				switch( tok)
			{ case "BLACK" : BlackArrow=loci; break;
				case "WHITE": WhiteArrow=loci; break;
				case "OTHER": OtherLinks .Add(loci); break;
				default: return false; break;
				}
			}

			return true;
		}

		public void PrintDistrickt (byte label, byte row)
		{
			en.io.ClientWrite ((row+1).ToString ()+ ". ", 16, true, false, label);
			this.Print (label);
			foreach (short li in LocathionInDistr)
			{ en.io.ClientWrite( Environment.NewLine, 10, false, false, label);   
				en.locs [li].Print (label);
			}

			en.io.ClientWrite( Environment.NewLine+ Environment.NewLine , 10, false, false, label);   

		}

		public override void Print (byte label)
		{ en.io.ClientWrite (displayName , 16, true, false, label);

			PrintInvestAndMonsters(label); 
		}



		public void AddLocInDistr ( short loc, bool isTextLoad= true)
		{
			LocathionInDistr.Add (loc);
			if( isTextLoad)
				OtherLinks.Add (loc);
		}


		public List<short> GetLinkStreets()
		{ List<short> res = new List<short> ();
			if (en.locs [BlackArrow].GetLocType() == LocathionType.ArchamStreet)
				res.Add (BlackArrow);

			if (en.locs [WhiteArrow].GetLocType ()== LocathionType.ArchamStreet)
				res.Add (WhiteArrow);
			foreach( short loci in OtherLinks)
				if (en.locs [loci].GetLocType() == LocathionType.ArchamStreet)
					res.Add (loci);
			return res;

		}


	}



	public class ArchemStableLoc: ArchemArea
	{  
	   

		public ArchemStableLoc(GameEngine eng,  string cn)
		{ 
			codeName=cn; 
			type= LocathionType.ArchamStable; 
			en=eng;

		}

		public ArchemStableLoc ()
		{

		}



 
		public  override bool FromText( TextFileParser prs, TextFileParser text)
		{
			 return FromText1 (prs, text);
		}

		protected bool FromText1 (TextFileParser prs, TextFileParser text)
		{
			if (! base.FromText2 (prs, text))
				return false;
			short a = en.map.GetDistricktStreetNumber (Districkt);  
			if (a == -1)
			{ System.Windows.Forms.MessageBox.Show (" Could not find disteckt street to loc "+ codeName);
				return false;

			}
			BlackArrow=a;
			WhiteArrow=a; 
			((ArchemStreet) en.locs[a]).AddLocInDistr(LocathionIndex);   
			return true;
		}

		public override void Print (byte label)
		{
			en.io.ClientWrite (displayName , 14, true, false, label);
			PrintClosed (label); 
			PrintInvestAndMonsters(label); 
		
		}

		public override void FromBin (System.IO.BinaryReader rd)
		{
			FromBin2 (rd);
			((ArchemStreet) en.locs[en.map.GetDistricktStreetNumber(Districkt) ]).AddLocInDistr(LocathionIndex, false);
		}

		public override void Encounter()
		{ en.clock.PrintCurPhase (); 
			ArcEncCard  c = en.archEncs [Districkt].Draw ();
			c.Execute (LocathionIndex);


		}
	}


	public class ArchemUnstableLoc: ArchemStableLoc
	{  
		private bool isSealed;
		private GatePrototype gate;

		public ArchemUnstableLoc(GameEngine eng, string cn)
		{   codeName=cn;
			type= LocathionType.ArchamUnstable; 
			en=eng; 
			clues=1;
			isSealed = false; 

		}




 
		public   bool FromText( TextFileParser prs, TextFileParser text)
	   { if( ! base.FromText1(prs, text))
			return false;



			return true;
		}

		public override void  Print( byte label )
		{
			en.io.ClientWrite (displayName , 14, true, false, label);
			if( isSealed)
				en.io.ClientWrite ( "  "+ en.sysstr.GetString(SSType.SealedFact)  , 14, false, true, label);
			PrintClosed (label); 
			if( clues != 0)
				en.io.ClientWrite (Environment.NewLine+ en.sysstr.GetNumberClueToken(clues)  , 14, false, false, label);
			if (gate != null)
				en.io.ClientWrite (Environment.NewLine + gate.GetMediumDiscripthion (), 12,false, false,label);  
			PrintInvestAndMonsters(label); 
 
		}

		public override void FromBin (System.IO.BinaryReader rd)
		{
			FromBin2 (rd);
			((ArchemStreet) en.locs[en.map.GetDistricktStreetNumber(Districkt) ]).AddLocInDistr(LocathionIndex, false);
		}

		public override void Reset()
		{
			base.Reset();
			clues = 1;
			if (gate != null)
			{ en.gates.Add (gate);
				gate = null;

			}
		}


		public override void WriteToSave (System.IO.BinaryWriter wr)
		{
			base.WriteToSave (wr);
			wr.Write (isSealed);
			if (gate != null)
				wr.Write ((short)(gate.GetID () + 1)); 
			else
				wr.Write( (short)0);
		}


		public override void ReadFromSave (System.IO.BinaryReader rd)
		{
			base.ReadFromSave (rd);
			isSealed = rd.ReadBoolean (); 
			short gateId = rd.ReadInt16 ();
			if (gateId == 0)
				gate = null;
			else
			{
				gate = en.gates.GetCardById ((short)(gateId - 1)); 
				gate.Open (LocathionIndex, false);
			}

		}

		public bool GetIsSeal()
		{
			return isSealed;

		}

		public bool isGate()
		{ return gate != null;

		}

		public GatePrototype GiveGate()
		{
			return gate;
		}


		public bool OpenGate()
		{ if (isSealed)
			{ en.io.ServerWrite (en.sysstr.GetString (SSType.GateCant1));
				en.io.ServerWrite( "  "+ GateAndClueTitle  + "  ", 12, false, true); 
				en.io.ServerPrintTag (en.sysstr.GetString (SSType.GateCant2)+ Environment.NewLine ); 
				return true;

			}

			gate = en.gates.Draw ();
			if (gate == null)
			{
				en.ga.Awekeen ();
				return false;
			}
			clues = 0;

			gate.Open (LocathionIndex);
			if (! en.status.NewGate ())
				return false;
			if (! en.status.DoomIncrise (1))
				return false;

			while( investigators.Count != 0)
			{ gate.MoveTo( investigators [0], true);
			
			} 

			if (! MonseterPlaced ())
				return false;
			if( en.GetPlayersNumber() >= 5)
				 if ( ! MonseterPlaced ()) 
					return false;

			return true;
		}




		public override void MythosClues ()
		{ if (gate == null)
				base.MythosClues ();
			else
			{ 
				en.io.ServerWrite (en.sysstr.GetString (SSType.ClueCouldNotAppear1));
				en.io.ServerWrite("  " + GateAndClueTitle+ "  " ,12,false,true);
				en.io.ServerWrite (en.sysstr.GetString (SSType.ClueCouldNotAppear2)+ Environment.NewLine );
				en.curs.resolvingMythos.Step3(); 

			}
		}


		public override void Encounter ()
		{  if (gate != null)
			{ en.clock.PrintCurPhase (); 
				byte curInv = en.clock.GetCurPlayer ();
				if (en.ActiveInvistigators [curInv].isExploredToken)
					ClosedGateChoose ();
				else
				{

					gate.MoveTo (curInv, false);
					en.clock.NextPlayer (); 
				}

			}
			else
				base.Encounter ();
		}

		private void ClosedGateChoose()
		{ List <IOOption> opts = new List<IOOption> ();
			foreach (IOOption opt in en.ActiveInvistigators[ en.clock.GetCurPlayer()].myTrigers.GetChooseOpthions(TrigerEvent.BeforeGateClosing, 0))     
				opts.Add (opt);
			opts.Add( new IOOptionWithParam(en.sysstr.GetString(SSType.ClosingWith)+ "  "+ en.sysstr.GetString(SSType.Fight), gate.ClosedCheck, 0));
			opts.Add( new IOOptionWithParam(en.sysstr.GetString(SSType.ClosingWith)+ "  " +  en.sysstr.GetString(SSType.Lore), gate.ClosedCheck, 1));
			opts.Add( new IOOpthionWithoutParam(en.sysstr.GetString(SSType.NotClosing ), en.clock.NextPlayer));
			en.io.StartChoose (opts, en.sysstr.GetString (SSType.ChooseActhioPromt), en.sysstr.GetString (SSType.ChooseActhionButton));   
		}


		public void CloseGate(byte trofyNum=40)
		{ foreach (byte invnum in investigators )
				en.ActiveInvistigators [invnum].isExploredToken = false;
			gate.Close ();
			if (trofyNum == 40)
				en.ActiveInvistigators [en.clock.GetCurPlayer()].AddTrophy (gate);
			else if (trofyNum != 60)
				en.ActiveInvistigators [trofyNum].AddTrophy (gate);   
			gate = null;

		}

		public void SealedChoose()
		{ if (en.ActiveInvistigators [en.clock.GetCurPlayer()].GetCluesValue () < en.status.GetCluesToSealed ())
			{ en.clock.NextPlayer ();
				return;

			}
			string promt = en.ActiveInvistigators [en.clock.GetCurPlayer ()].GetTitle () + " ";
				promt+= en.sysstr.GetString(SSType.Has)+ " "+ en.sysstr.GetNumberClueToken(en.ActiveInvistigators [en.clock.GetCurPlayer()].GetCluesValue())+ Environment.NewLine;  
				promt += en.sysstr.GetString (SSType.SealedPromt1) + " " + displayName +  " " + en.sysstr.GetString (SSType.ForWotld);
			promt += "  "+ en.sysstr.GetNumberClueToken (en.status.GetCluesToSealed ())+" "+  en.sysstr.GetString (SSType.SealedPromt2);
			en.io.YesNoStart (promt, en.sysstr.GetString (SSType.Yes), en.sysstr.GetString (SSType.No), SealedChooseYes, en.clock.NextPlayer);  


		}

		private  void  SealedChooseYes()
		{ en.ActiveInvistigators [en.clock.GetCurPlayer ()].RemoveClues (en.status.GetCluesToSealed ());
			Sealed ();
			en.clock.NextPlayer (); 

		}
		public void Sealed()
		{ if (gate != null)
			{
				CloseGate (40);

			}
			isSealed = true;
			en.io.ServerWrite (displayName, 12, false, true);
			en.io.ServerWrite ("  " + en.sysstr.GetString (SSType.SealedMessage));
			en.status.AddSealed (); 
		}

		public void DrawFromGate( byte invnum, bool isDelayed=true)
		{  if (gate != null)
				gate.MoveTo (invnum, isDelayed); 

			

		}
	}

	public enum LocathionType
	{ LiTaS=0,
      OW,
      ArchamStreet,
      ArchamStable,
      ArchamUnstable
      

	}
}




