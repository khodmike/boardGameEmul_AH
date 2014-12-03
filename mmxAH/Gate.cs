using System;

namespace mmxAH
{
	public class GatePrototype: Card
	{
		private byte owindex;
		private byte dsindex; 
		private short ArchemLoc=-1;
		private GameEngine en;





		public GatePrototype (GameEngine eng)
		{ 
			en=eng;
			ID = en.gates.GetCountOfCards ();
		}


		public byte GetOW()
		{
			return owindex; 
		}

		public short GetArchemLoc()
		{
			return ArchemLoc;
		}


		public override string ToString ()
		{
			return   "Gate to "+ ((OWLoc )en.locs[owindex]).GetDescripthionToGate ()+ " DS: "+ en.ds.GetTitle( dsindex);     
		}


		public bool FromText(TextFileParser prs)
		{  string str=prs.GetToken() ;
			int num;
           if( str=="" || (num= en.ows.Index(str))==-1)
				return false;
			owindex=(byte)num;
			str=prs.GetToken() ;
			if( str=="" || (num= en.ds.GetIndex(str))==-1)
				return false;
			dsindex=(byte)num;
			return true;
		}



		public string  GetShortDiscripthion()
		{ 
			return en.sysstr.GetString(SSType.GateTo)+ "  "+  en.locs[owindex].GetMoveToTitle() ;  

		}

		public string  GetLongDiscripthion()
		{ 
			return en.sysstr.GetString(SSType.GateTo)+ "  "+  ((OWLoc)en.locs[owindex]).GetDescripthionToGate()+ " "+ en.sysstr.GetString(SSType.DS)+ " : " +  en.ds.GetTitle( dsindex);  

		}

		public string  GetMediumDiscripthion()
		{ 
			return en.sysstr.GetString(SSType.GateTo)+ "  "+  ((OWLoc)en.locs[owindex]).GetShortDescripthionToGate()+ " "+ en.sysstr.GetString(SSType.DS)+ " : " +  en.ds.GetTitle( dsindex);  

		}

		public override void ToBin( System.IO.BinaryWriter wr)
		{ wr.Write(owindex);
			wr.Write (dsindex);

		}

		public  override void FromBin(System.IO.BinaryReader rd)
		{ owindex=rd.ReadByte();
		  dsindex = rd.ReadByte (); 
		}

		public void Open( short LocIndex, bool isPrint=true)
		{ ArchemLoc=LocIndex;
			en.openGates.Add (this); 
			if (isPrint)
			{
				en.io.PrintToLog (en.sysstr.GetString (SSType.GateTo) + "  ");
				en.io.PrintToLog (en.locs [owindex].GetTitle (), 12, true);
				en.io.PrintToLog ("  " + en.sysstr.GetString (SSType.GateOpenVerb));
				en.io.PrintToLog ("  " + ((ArchemArea)en.locs [ArchemLoc]).GetGateAndClueTitle () + ".", 12, false, true); 
			}
		}

		public void MoveTo(byte invnum, bool isDelayed)
		{
			en.ActiveInvistigators [invnum].SetLocathion (owindex);
			en.ActiveInvistigators [invnum].isCanMove = false; 
			en.io.PrintToLog (en.ActiveInvistigators [invnum].GetTitle (), 12, false, true);
			if (isDelayed)
			{
				en.io.PrintToLog ("  " + en.sysstr.GetString (SSType.GateDrawn) + " ");
				en.io.PrintToLog (en.locs [owindex].GetTitle () + ".  "+ Environment.NewLine , 12, true);
				en.ActiveInvistigators [invnum].Delayed (); 
			} else
			{
				en.io.PrintToLog ("  " + en.sysstr.GetString (SSType.PassThoughGate ) + " ");
				en.io.PrintToLog (en.locs [owindex].GetTitle () + ".  "+ Environment.NewLine, 12, true);
			}


		}

		public void ClosedCheck( short isLore)
		{ SkillTestType tp;
			short modif = ((OWLoc)en.locs [owindex]).GetGateModif ();
			byte dif= ((OWLoc)en.locs [owindex]).GetGateDif ();
			if (isLore == 1)
				tp = SkillTestType.Lore;
			else
				tp = SkillTestType.Fight;
			new SkillTest (en, tp, modif, ClosedAfterCheck, dif);


		}

       private void ClosedAfterCheck( short succeses)
		{ if (succeses >= ((OWLoc)en.locs [owindex]).GetGateDif ())
			{  //необходимо так как в процессе закрытия archemLOc=-1
				short loc = ArchemLoc;
				((ArchemUnstableLoc)en.locs [ArchemLoc]).CloseGate ();
				((ArchemUnstableLoc)en.locs [loc]).SealedChoose ();
			}
			else
				en.clock.NextPlayer (); 

			}


		public void Close()
		{ en.io.PrintToLog(en.sysstr.GetString(SSType.GateTo)+ " " );
			en.io.PrintToLog( en.locs[owindex].GetMoveToTitle(),12,true) ; 
			en.io.PrintToLog (" " + en.sysstr.GetString (SSType.GateClosedFact));
			en.status.ClosedGate ();
			byte i=0;
			MonsterIndivid m;
			while (i< en.ActiveMonsters.Count)
			{ m = en.ActiveMonsters [i];
				if (m.GetDs () == dsindex)
				{
					m.Discard (true);
					i = 0;
				} else
					i++;


			}
			i = 0;
			while (i< en.Outscirts.Count)
			{
				m = en.Outscirts [i];
				if (m.GetDs () == dsindex)
				{
					i = 0;
					en.MonstersCup.Add (m);
					en.Outscirts.Remove (m); 
					en.status.RemoveFromOut (); 
					en.io.PrintToLog (m.GetTitle (), 12, true, true);
					en.io.PrintToLog ("  " + en.sysstr.GetString (SSType.From) + "  ");
					en.io.PrintToLog (en.sysstr.GetString (SSType.Outscirts), 12, false, true);
					en.io.PrintToLog ("  " + en.sysstr.GetString (SSType.ReturnToTheCup) + Environment.NewLine); 
				} else
					i++;
			}        
			en.status.PrintMonserCountServer ();
			en.status.PrintMonserCountInOutServer (); 
			ArchemLoc = -1;
			en.openGates.Remove (this); 

		}

	}
}

