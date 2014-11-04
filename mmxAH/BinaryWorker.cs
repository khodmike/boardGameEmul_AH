using System;
using System.IO;
namespace mmxAH
{
	public class BinaryWorker
	{ private string filePath;
		private GameEngine en;
		private const string TitleString="===Mmx Game Emulator Data File Game: Archem Horror===";
		private const string SaveTitleString="===Mmx Game Emulator Game: Archem Horror  Save File===";
		private int ver=1;
		public BinaryWorker (GameEngine eng, string fp)
		{ filePath=System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar+fp;
			en=eng;
		}

		public  void ToBin()
		{ en.io.ClientWrite("Output file name : "+ filePath+ Environment.NewLine);
		BinaryWriter wr;
			try
			{
			 wr= new BinaryWriter( new FileStream(filePath, FileMode.Create)); 
				wr.Write (TitleString);
				wr.Write (ver);
				en.sysstr.ToBinary(wr);
				en.colors.ToBinary(wr);
				en.ds.ToBinary(wr); 
				en.LimitsToBinary(wr);
				en.ows.ToBin (wr); 
				en.map.ToBin (wr); 
				en.gates.ToBin (wr);




				wr.Write( en.archEncs.Length);   
				wr.Close ();
			}
			catch( Exception e)
			{
				en.io.ClientWrite("Error:"+ e.Message,16,true);
			}

			en.io.ClientWrite("Binary Converthion succesfully complete. "+ Environment.NewLine);

		}


		
		public  void ToSave()
		{
			BinaryWriter wr;
			wr= new BinaryWriter( new FileStream(filePath, FileMode.Create )); 
			wr.Write (SaveTitleString);
			wr.Write (ver);
			en.clock.WriteToSave (wr); 
			en.SaveActive (wr);
			foreach (Locathion loc in en.locs)
				loc.WriteToSave (wr);

			en.mythosDeck.ToSave (wr);
			en.owEnc.ToSave (wr); 
			en.status.ToSave (wr);  
			wr.Close ();
				

		}

		public 	bool  BinaryInit ()
		{ BinaryReader rd;
			try
			{
				 rd = new BinaryReader (new FileStream (filePath, FileMode.Open));  
			} catch
			{
				return false;
			}

			en.locs.Add ( new Litas(en));
			if( rd.ReadString() != TitleString )
				return false;
			if (rd.ReadInt32 () != ver)
				return false;
			en.sysstr.FromBinary(rd); 
			en.colors.FromBinary(rd);
			en.ds.FromBinary(rd);
			en.LimitsFromBinary(rd); 
			en.ows.FromBin (rd);
			en.map = new MapOfCity (en);
			en.map.FromBin (rd);
			InitGates (rd);

			int cn = rd.ReadInt32 ();
			en.archEncs = new Deck<ArcEncCard> [cn] ;
			for (int i=0; i<cn; i++)
				en.archEncs [i] = new Deck<ArcEncCard> (true); 
			rd.Close(); 
			return true;

		}


		public bool  FromSave()
		{ BinaryReader rd;
			try
			{
				rd = new BinaryReader (new FileStream (filePath, FileMode.Open));
				if( rd.ReadString() != SaveTitleString )
					return false;
				if (rd.ReadInt32 () != ver)
					return false;
				en.Reset ();
				en.clock.ReadFromSave(rd); 
				en.LoadActive (rd); 
				foreach( Locathion loc in en.locs)
					loc.ReadFromSave(rd); 

				en.mythosDeck.FromSave(rd);
				en.owEnc.FromSave(rd);
				en.status.FromSave(rd); 
				rd.Close(); 
			} catch ( Exception e)
			{ System.Windows.Forms.MessageBox.Show (e.Message); 
				return false;
			}




			return true;

		}


		private void InitGates(BinaryReader rd)
		{ int ng = rd.ReadInt32();
			GatePrototype g;
			for (int i=0; i<ng; i++)
			{ g = new GatePrototype (en);
				g.FromBin (rd);
				en.gates.Add (g); 

			}
			en.gates.Shuffle (); 
		}


	}
}

