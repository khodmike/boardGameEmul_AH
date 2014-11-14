using System;
using System.IO;

namespace mmxAH
{
	public class GlobalStatus
	{ GameEngine en;
		private  byte CurDoom, CurGate, CurMonsters, CurOut, CurTerror, CurSealed;
		private byte MaxDoom, MaxGate, MaxMonsters, MaxOut;
		private const byte Terror1=3, Terror2=6, Terror3=9, MaxTerror=10, MaxSealed=6;
		private short CluesToSealed;
		public GlobalStatus ( GameEngine eng)
		{ en=eng;
			Reset ();
		}

		public void Print()
		{ en.clock.PrintCurPhase(false); 
			en.io.ClientWrite (en.sysstr.GetString (SSType.DoomTrack), 12, true);
		  en.io.ClientWrite (" " + CurDoom + " / " + MaxDoom+ "."+ Environment.NewLine );  
			en.io.ClientWrite ( en.sysstr.GetString (SSType.OpenGates ), 12, true);
			en.io.ClientWrite (" " + CurGate + " / " + MaxGate+ "."+ Environment.NewLine ); 
			en.io.ClientWrite (en.sysstr.GetString (SSType.MonsterInArchem   ), 12, true);
			en.io.ClientWrite (" " + CurMonsters + " / " + MaxMonsters + "."+ Environment.NewLine );
			en.io.ClientWrite (en.sysstr.GetString (SSType.MonsterInOutscirts   ), 12, true);
			en.io.ClientWrite (" " + CurOut + " / " + MaxOut + "."+ Environment.NewLine );  
			en.io.ClientWrite (en.sysstr.GetString (SSType.TerrorTrack  ), 12, true);
			en.io.ClientWrite (" " + CurTerror + " / " + MaxTerror + "."+ Environment.NewLine ); 
			en.io.ClientWrite (Environment.NewLine+ en.sysstr.GetString (SSType.SealedLocathion   ), 12, true);
			en.io.ClientWrite (" " + CurSealed + " / " + MaxSealed + "."+ Environment.NewLine ); 

		}

		public void Init( byte pMaxGate, byte pMaxMonsters, byte pMaxOut)
		{  MaxGate=pMaxGate;
			MaxMonsters = pMaxMonsters;
			MaxOut = pMaxOut;

		}

		public void SetMaxDoom( byte pMaxDoom)
		{
			MaxDoom = pMaxDoom;
		}

		public void Reset()
		{ CurDoom=0;
			CurGate = 0; 
			CurMonsters = 0;
			CurOut = 0;
			CurTerror = 0;
			CurSealed = 0;
			MaxDoom = 0;
			MaxGate = 0;
			MaxMonsters = 0;
			MaxOut = 0;
			CluesToSealed = 5; 
			//заглушка
			MaxDoom = 10;



		}

		public bool DoomIncrise( byte count=1)
		{ CurDoom+=count;
			en.io.ServerWrite (en.sysstr.GetNumberDoomToken (count) + " " + en.sysstr.GetString (SSType.DoomInc));
			en.io.ServerWrite (" " + en.sysstr.GetString (SSType.DoomTrack), 12, true);
			en.io.ServerWrite (" " + CurDoom + " / " + MaxDoom+ "."+ Environment.NewLine );  

			if (CurDoom >= MaxDoom)
			{ en.ga.Awekeen ();
				return false;

			}
			return true;

		}


		public void TerrorIncrise()
		{ 
			CurTerror++;
			if (CurTerror > MaxTerror)
			{ CurTerror = MaxTerror;
				return;

			}
			en.io.ServerWrite (en.sysstr.GetString (SSType.TerorInc ));
			en.io.ServerWrite (" " + en.sysstr.GetString (SSType.TerrorTrack ), 12, true);
			en.io.ServerWrite (" " + CurTerror  + " / " + MaxTerror + "."+ Environment.NewLine );  



		}


		public bool NewGate()
		{ CurGate ++;
			en.io.ServerWrite (" " + en.sysstr.GetString (SSType.OpenGates ), 12, true);
			en.io.ServerWrite (" " + CurGate + " / " + MaxGate+ "."+ Environment.NewLine );  

			if (CurGate >= MaxGate)
			{ en.ga.Awekeen ();
				return false;

			}
			return true;

		}


		public bool IsMonserToPlace(MonsterIndivid m)
		{ if (CurMonsters ==  MaxMonsters)
			{   if (CurOut == MaxOut)
				{
					en.io.ServerWrite (en.sysstr.GetString (SSType.OutscirtsIsClear) + Environment.NewLine);
					CurOut = 0;
					en.MonstersCup.Add (m);
					en.ResetOutscirts ();
					TerrorIncrise (); 
				} else
				{  CurOut++;
					en.Outscirts.Add (m); 
					en.io.ServerWrite (m.GetTitle (), 12, true);
					en.io.ServerWrite ("  " + en.sysstr.GetString (SSType.PlacedToOut )+ " . ");
					en.io.ServerWrite (en.sysstr.GetString (SSType.MonsterInOutscirts   ), 12, true);
					en.io.ServerWrite (" " + CurOut + " / " + MaxOut + "."+ Environment.NewLine );
				}
				return false;

			} else
			{
				CurMonsters++;
				return true;
			}
		}


		public void PrintMonserCountServer()
		{ en.io.ServerWrite (en.sysstr.GetString (SSType.MonsterInArchem   ), 12, true);
			en.io.ServerWrite (" " + CurMonsters + " / " + MaxMonsters + "."+ Environment.NewLine );

		}

		public void PrintMonserCountInOutServer()
		{ en.io.ServerWrite (en.sysstr.GetString (SSType.MonsterInOutscirts   ), 12, true);
			en.io.ServerWrite (" " + CurOut + " / " + MaxOut + "."+ Environment.NewLine );

		}


	   
		public void ToSave(BinaryWriter wr)
		{ 
			wr.Write (CurDoom);
			wr.Write (CurGate);
			wr.Write (CurMonsters);
			wr.Write (CurOut);
			wr.Write (CurTerror);
			wr.Write (CurSealed);

		}


		public void FromSave( BinaryReader rd)
		{ CurDoom = rd.ReadByte ();
		  CurGate = rd.ReadByte ();
		  CurMonsters= rd.ReadByte ();
		  CurOut = rd.ReadByte ();
		  CurTerror = rd.ReadByte ();
		  CurSealed = rd.ReadByte ();

		}


		public  void CluesToSealedModif( short modif)
		{ CluesToSealed+= modif;
			if( CluesToSealed<0)
				CluesToSealed=0; 

		}


		public byte GetCluesToSealed()
		{  return (byte)CluesToSealed; 

		}


		public void ClosedGate()
		{ if (CurGate > 0)
				CurGate--;
			en.io.ServerWrite (en.sysstr.GetString (SSType.OpenGates    ), 12, true);
			en.io.ServerWrite (" " + CurGate + " / " + MaxGate + "."+ Environment.NewLine );

		}

		public void RemoveFromOut()
		{ if (CurOut > 0)
				CurOut--;

		}

		public void RemoveMonsterInArchem()
		{ if (CurMonsters > 0)
				CurMonsters--; 

		}
	}
}

