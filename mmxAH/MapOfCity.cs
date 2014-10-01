using System;

namespace mmxAH
{
	public class MapOfCity
	{ private short StartIndex;
		private byte Count;
		private GameEngine en;
		private MapRow[] rows;
		private byte rowCount;
		public MapOfCity (GameEngine eng)
		{ en=eng;
		  StartIndex= (short)en.locs.Count; 
		}

		public bool FromText(TextFileParser prs, TextFileParser text)
		{
			if (! Byte.TryParse (prs.GetToken (), out rowCount))
				return false;
			rows= new MapRow[rowCount]; 
			if (! Byte.TryParse (prs.GetToken (), out Count))
				return false;
			//Первый этап - кодовые имена и типы
			ArchamArea aa;
			string type, codename;
			for (int i=0; i< Count; i++)
			{ type= prs.GetToken ().Trim().ToUpper();
				if(type==null)
					return false;
				codename=prs.GetToken ().Trim ().ToUpper() ;   
              if( type=="STREET")
				{aa= new ArchamStreet(en,codename);
                 //читаем положение района
				 if( ! ReadPossithionFromText(prs))
						return true;
				}
              else if( type=="STAB")
					aa= new ArchamStableLoc(en,codename);
              else if (type== "UNSTAB")
					aa= new ArchamUnstableLoc(en,codename); 
              else
				{  System.Windows.Forms.MessageBox.Show( " Unknow loc type: " + type);  
					return false;
				}
              
			 aa.SetLocIndex((short)(en.locs.Count));
				en.locs.Add(aa);
			}

			//Второй этап- спецификациии и связи
			for( short i=0; i<Count; i++)
				if ( ! en.locs[i+StartIndex].FromText(prs, text))
					return false;
			return true;
		}


		public short GetDistricktStreetNumber (byte dis)
		{ Locathion l;
			for (short i=0; i<Count; i++)
			{ l= en.locs [i + StartIndex];
				if ( l.GetLocType()== LocathionType.ArchamStreet  && ((ArchamStreet ) l).GetDistrickt ()==dis  )
					return((short) (i + StartIndex));
			}
				    return -1;
		}


		public short GetNumberByCodeName( string cn)
		{  cn= cn.ToUpper(); 
			for (short i=0; i<Count; i++)
			{
				if (  en.locs [i + StartIndex].GetCodeName ()== cn )
					return((short) (i + StartIndex));
			}
				    return -1;

		}

		private struct MapRow
		{ public short Left;
		  public short Meddium;
		  public short Right;

		}

		private bool ReadPossithionFromText (TextFileParser prs)
		{
			byte r;
			string col;
			short locnum = (short)(en.locs.Count); 
			if (! Byte.TryParse (prs.GetToken (), out r))
				return false;
			if (r >= rowCount)
				return false;
			col = prs.GetToken ().Trim ().ToUpper ();
			switch( col)
		{ case "L" : rows[r].Left= locnum; break;
		  case "M" : rows[r].Meddium = locnum; break;
          case "R" : rows[r].Right= locnum; break;
		  case "LM": { rows[r].Left= locnum; rows[r].Meddium = (short)(-locnum); } break;
          case "MR": { rows[r].Meddium = locnum; rows[r].Right = (short)(-locnum); } break;
		  default: return false; break;
			 
			}
			return true;
		}


		public void Print ()
		{
			en.io.Set3LabelView (); 
			for (byte i=0; i< rowCount; i++)
			{ if( rows[i].Left !=0 )
				((ArchamStreet) en.locs[rows[i].Left]).PrintDistrickt(1, i); 
              if( rows[i].Meddium  > 0 )
				((ArchamStreet) en.locs[rows[i].Meddium ]).PrintDistrickt(2, i); 
              if( rows[i].Meddium  < 0 )
					en.io.ClientWrite((i+1).ToString()+ ". [ "+ en.locs[-rows[i].Meddium].GetTitle()+ " ]", 16,true, false, 2);    
			    if( rows[i].Right !=0 )
				((ArchamStreet) en.locs[rows[i].Right]).PrintDistrickt(3, i); 
               
				en.io.ArrengeLabels(); 
              
			}

		}

		public void ToBin( System.IO.BinaryWriter wr)
		{ 
			wr.Write(Count);
			for (int i=StartIndex; i< StartIndex+Count; i++)
				en.locs [i].ToBin (wr);
			wr.Write (rowCount);
			foreach (MapRow r in rows)
			{ wr.Write (r.Left);
			  wr.Write (r.Meddium); 
			  wr.Write (r.Right); 

			}
          

		}

		public void FromBin( System.IO.BinaryReader rd)
		{  Count = rd.ReadByte();
			LocathionType t;
			Locathion l;
			for (int i=StartIndex; i< StartIndex+Count; i++)
			{ t = (LocathionType)rd.ReadInt32(); 
				//обеспечение дальнейшего нармального чтения
				rd.BaseStream.Position -= 4; 
				switch (t)
			{ case LocathionType.ArchamStreet: l = new ArchamStreet (en, "AAA"); break;  
			 case LocathionType.ArchamStable: l = new ArchamStableLoc (en, "AAA"); break;
			 case LocathionType.ArchamUnstable : l = new ArchamUnstableLoc  (en, "AAA"); break;
				default:
					l = new Litas (en);
					break;
			}
				l.SetLocIndex((short)(en.locs.Count));
				l.FromBin (rd); 
				en.locs.Add (l);

			}

			rowCount = rd.ReadByte (); 
			rows = new MapRow[rowCount];
			for (int i=0; i<rowCount; i++)
			{
				rows [i].Left = rd.ReadInt16 ();
				rows [i].Meddium= rd.ReadInt16 ();
				rows [i].Right = rd.ReadInt16 ();

			}

		}
	}
}

