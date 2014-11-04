using System;

namespace mmxAH
{
	public class PhasesClock
	{ private GameEngine en;
		private Phases curPhase;
		private byte curTurn;
		private byte curPlayer;
		private byte firstPlayer;

		public PhasesClock (GameEngine eng)
		{en=eng;
			Reset ();


		}


		public void Reset()
		{  curPhase= Phases.SetupFix ;
			curTurn=0;
			curPlayer=0;
			firstPlayer=0; 

		}

		public void WriteToSave(System.IO.BinaryWriter wr)
		{ wr.Write((byte) curPhase);
			wr.Write (curTurn);
			wr.Write (curPlayer);
			wr.Write (firstPlayer);

		}


		public void ReadFromSave(System.IO.BinaryReader rd)
		{ curPhase= (Phases) rd.ReadByte();
		  curTurn=  rd.ReadByte(); 
			curPlayer= rd.ReadByte(); 
			firstPlayer =  rd.ReadByte();
		}

		public void EndMovementSegment()
		{ 

			short locnum = en.ActiveInvistigators [curPlayer].GetLocathion ();
			LocathionType t = en.locs [locnum ].GetLocType ();
			if( t != LocathionType.LiTaS  && t != LocathionType.OW)     
				((ArchamArea) en.locs [locnum]).EndMovementClues(curPlayer);   
		}


		public void NextPlayer()
		{  byte tempCurPlayer= curPlayer;
			tempCurPlayer++;
			if (tempCurPlayer == en.GetPlayersNumber() )
				tempCurPlayer = 0;
			if ((tempCurPlayer == firstPlayer && en.pref.StopOnPhase) || en.pref.StopOnSegment)
			{
				en.io.SetSaveEnable (true); 
				en.io.Pause (NextPlayer2); 
			} else
				NextPlayer2 (); 
		}


		private void NextPlayer2()
		{ en.io.SetSaveEnable(false); 
			curPlayer++;
			if (curPlayer == en.GetPlayersNumber() )
				curPlayer = 0;
			if (curPlayer == firstPlayer)
			{
				NextPhase ();
			} else
				NextSegment(); 



		}

		public void EndTurn()
		{if (en.pref.StopOnTurn)
			{
				en.io.SetSaveEnable (true);
				en.io.Pause (EndTurn2); 
			} else
			{
				EndTurn2 (); 
			}

		}

		private  void EndTurn2()
		{ en.io.SetSaveEnable (false);

			if (curTurn != 0)
			{ 	firstPlayer++;
				if (firstPlayer == en.GetPlayersNumber ())
					firstPlayer = 0;
				curPlayer = firstPlayer; 
				en.io.ServerWrite (Environment.NewLine +en.sysstr.GetString (SSType.Turn) + "  " + curTurn + " " + en.sysstr.GetString (SSType.TurnEndMessage), 14);
				en.io.ServerWrite ("  " + en.sysstr.GetString (SSType.FirstPlayer) + "  : ", 14, true);
				en.io.ServerWrite (en.ActiveInvistigators [firstPlayer].GetTitle () + Environment.NewLine + Environment.NewLine + Environment.NewLine, 14, false, true);
			}
			curTurn++;
			
			curPhase =Phases.Upkeep;  
			en.ActiveInvistigators [firstPlayer].Upkeep (); 

		}

		private void NextPhase()
		{


			switch (curPhase)
		{   case Phases.SetupFix : {curPhase = Phases.SetupRandom ;} break; 
			case Phases.SetupRandom  : {curPhase = Phases.SetupSliders; } break; 
			case Phases.SetupSliders   : {curPhase = Phases.SetupMythos;} break; 
			case Phases.SetupMythos: {curTurn = 1;curPhase = Phases.Upkeep;} break; 
			case Phases.Upkeep: {curPhase = Phases.Movement;} break; 
			case Phases.Movement : {curPhase = Phases.AhEnc ;} break; 
			case Phases.AhEnc  : {curPhase = Phases.OwEnc  ;} break;
			case Phases.OwEnc  : {curPhase = Phases.Mythos  ;} break;
			case Phases.Mythos:
				{ // только при загрузке из сейва
					EndTurn2 ();
					return; 
				}break;



			}
			NextSegment ();

		}

		private void NextSegment()
		{ Investigator invest = en.ActiveInvistigators [curPlayer ];
			switch (curPhase)
		{  case Phases.SetupRandom:{invest.SetupRandom (); } break;
			case Phases.SetupSliders: {invest.Setup_Sliders ();}break;
			case Phases.SetupMythos: {en.ga.SetupMythos (); }break;  
				case Phases.Upkeep: {invest.Upkeep  ();} break; 
				case Phases.Movement:{
					foreach (MonsterIndivid mon in en.ActiveMonsters)
						mon.isEncountred = false;

				invest.Movement ();

			    }break; 
				case Phases.AhEnc:{invest.AhEnc ();}  break;
				case Phases.OwEnc  : {invest.OwEnc (); } break;
			case Phases.Mythos: {en.mythosDeck.Draw ().Execute() ; } break;



			}

		}

		public byte GetCurPlayer()
		{ return curPlayer; 

		}
		public byte GetFirstPlayer()
		{
			return firstPlayer; 
		}

		public void PrintCurPhase(bool isServer=true )
		{ 
			string str = Environment.NewLine+ "==";
			if ((byte)curPhase <= 4)
				str += GetSetupPhaseString ();
			else
			{
				str += en.sysstr.GetString (SSType.Turn) + " " + curTurn + " . ";
				if (curPhase == Phases.Mythos)
					str += en.sysstr.GetString (SSType.Mythos);
				else
				{ switch (curPhase)
				{ case Phases.Upkeep: str += en.sysstr.GetString (SSType.Upkeep); break;   
				  case Phases.Movement : str += en.sysstr.GetString (SSType.Movement); break;
				  case Phases.AhEnc: str += en.sysstr.GetString (SSType.ArcEncPhase); break; 
				  case Phases.OwEnc: str += en.sysstr.GetString (SSType.OwEncPhase); break;   
					}

					str += " . " + en.ActiveInvistigators [curPlayer ].GetTitle ();

				}
			}
			str+= "==" + Environment.NewLine;
			if( isServer) 
			en.io.ServerWrite (str, 14, true); 
			else
			en.io.ClientWrite (str, 14, true); 
		}

		private string  GetSetupPhaseString()
		{ switch (curPhase)
		{ case Phases.SetupFix: return en.sysstr.GetString (SSType.Setup); 
			case Phases.SetupRandom: return  en.sysstr.GetString (SSType.SetupRandomStep) + " . " + en.ActiveInvistigators [curPlayer ].GetTitle (); 
			case Phases.SetupSliders: return  en.sysstr.GetString (SSType.SetupSlidersStep)  + " . " + en.ActiveInvistigators [curPlayer ].GetTitle ();
			case Phases.SetupMythos: return   en.sysstr.GetString (SSType.SetupMythosStep);

			}
			return "";
		}


		public void ResumeFromSave()
		{ if (curPlayer == firstPlayer)
			{
				NextPhase ();
				return;
			}

			//сохранение в конце сегмента
			NextSegment (); 

		}


		public void StartRandomSetup()
		{ curPlayer=0;
			curPhase = Phases.SetupRandom;
			NextSegment (); 

		}


	}


	public enum Phases
	{ SetupFix=1,
	  SetupRandom,
      SetupSliders,
	  SetupMythos,
      Upkeep,
      Movement,
      AhEnc,
      OwEnc,
      Mythos 

	}
}

