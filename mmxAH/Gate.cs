using System;

namespace mmxAH
{
	public class GatePrototype: Card
	{
		private byte owindex;
		private byte dsindex; 
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

		public  byte GetOW()
		{ return owindex;
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

	}
}

