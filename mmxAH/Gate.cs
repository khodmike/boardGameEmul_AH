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



		public string  ShortDiscripthion()
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
			if (isPrint)
			{
				en.io.ServerWrite (en.sysstr.GetString (SSType.GateTo) + "  ");
				en.io.ServerWrite (en.locs [owindex].GetTitle (), 12, true);
				en.io.ServerWrite ("  " + en.sysstr.GetString (SSType.GateOpenVerb));
				en.io.ServerWrite ("  " + ((ArchemArea)en.locs [ArchemLoc]).GetGateAndClueTitle () + ".", 12, false, true); 
			}
		}

		public void MoveTo(byte invnum, bool isDelayed)
		{
			en.ActiveInvistigators [invnum].SetLocathion (owindex);

			en.io.ServerWrite (en.ActiveInvistigators [invnum].GetTitle (), 12, false, true);
			if (isDelayed)
			{
				en.io.ServerWrite ("  " + en.sysstr.GetString (SSType.GateDrawn) + " ");
				en.io.ServerWrite (en.locs [owindex].GetTitle () + ".  "+ Environment.NewLine , 12, true);
				en.ActiveInvistigators [invnum].Delayed (); 
			} else
			{
				en.io.ServerWrite ("  " + en.sysstr.GetString (SSType.MoveToFact) + " ");
				en.io.ServerWrite (en.locs [owindex].GetTitle () + ".  "+ Environment.NewLine, 12, true);
			}




		}

	}
}

