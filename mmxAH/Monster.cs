using System;
using System.Collections.Generic;
namespace mmxAH
{
	public class MonsterIndivid: Card
	{  private byte monsternum;
		private short locnum=-1;
		private GameEngine en;
		public bool isEncountred=false;
		private Func RPAfterEncounter;
		private MonsterPrototype prot;
		private byte itemModif;
		private bool isFirstEvade;



		public MonsterIndivid (GameEngine eng, MonsterPrototype pr, short pID)
		{ en=eng;
			prot = pr;
			ID = pID;

		}

		public void AddToMap( short StartLocathion)
		{ locnum=StartLocathion;
			monsternum=(byte) en.ActiveMonsters.Count;
			en.ActiveMonsters.Add(this); 
			((ArchemArea)en.locs [StartLocathion]).AddMonster (this);  
			isEncountred=false; 

		}

		public void Discard ( bool isPrint=false)
		{ if (isPrint)
			{
				en.io.ServerWrite (prot.GetTitle() , 12, true, true);
				en.io.ServerWrite ("  " + en.sysstr.GetString (SSType.From) + "  ");
				if( locnum == 0) 
				en.io.ServerWrite (en.sysstr.GetString (SSType.Scy), 12, false, true);
				else
					en.io.ServerWrite (en.locs[locnum].GetMoveToTitle() , 12, false, true);
				en.io.ServerWrite ("  " + en.sysstr.GetString (SSType.ReturnToTheCup) + Environment.NewLine); 

			}
			if (locnum != -1)
			{
				if (locnum == 0)
					en.Scy.Remove (this);
				else
					((ArchemArea)en.locs [locnum]).RemoveMonster (this);
			en.ActiveMonsters.RemoveAt( monsternum);
			en.status.RemoveMonsterInArchem (); 
			}
			locnum = -1;

			en.MonstersCup.Add (this);

		}

		public void SetRPAfterEncounter (Func f)
		{ RPAfterEncounter =f;

		}
		public void SetLocathion( short newLoc)
		{  if( locnum== 0)
			en.Scy.Remove (this);
			else
			((ArchemArea ) en.locs[locnum]).RemoveMonster(this);
			locnum= newLoc;
			if (newLoc == 0)
				en.Scy.Add (this);
			else
			((ArchemArea ) en.locs[locnum]).AddMonster(this);
			en.io.ServerWrite (prot.GetTitle(), 12, true);
			en.io.ServerWrite ("  "+ en.sysstr.GetString (SSType.MoveToFact)+ "  "); 
			if( locnum== 0)
				en.io.ServerWrite ("  "+ en.sysstr.GetString (SSType.Scy)+ " ." + Environment.NewLine, 12, false, true); 
			else
				en.io.ServerWrite (en.locs [newLoc].GetMoveToTitle() + " ." + Environment.NewLine, 12, false, true);  
		}


		public void BeginEncounter()
		{ isEncountred=true;
			en.curs.curFight = this;
			isFirstEvade = false;
			en.io.ServerWrite ("===" +en.ActiveInvistigators [en.clock.GetCurPlayer() ].GetTitle ()+ "  "+ en.sysstr.GetString (SSType.MonsterEncounterVerb) + "  " +prot.GetTitle() + " ==="+ Environment.NewLine  , 12, true);
			prot.PrintServer ();
			string str = en.sysstr.GetString (SSType.EvadeOrFight1) + "  " + prot.GetEvadeTitle () + " " + en.sysstr.GetString (SSType.EvadeOrFight2) + " " + prot.GetFightTitle();  
			en.io.YesNoStart (str,en.sysstr.GetString(SSType.EvadeButton ) , en.sysstr.GetString(SSType.FightButton ), EvadeCheck, HorrorCheck);  

		}

		private void EvadeCheck()
		{  isFirstEvade = true;
			en.io.ServerWrite (Environment.NewLine); 
			new SkillTest (en, SkillTestType.Evade, prot.GetEvadeModif (), AfterEvadeCheck, 1, true, SkillTestType.Sneak);    

		}


		private void AfterEvadeCheck( short suc)
		{ if (suc > 0)
				EndEncounter (); 
          else
			{
				prot.DoDemage(HorrorCheck);
			

			}

		}





		private void HorrorCheck()
		{    en.ActiveInvistigators[ en.clock.GetCurPlayer()].isCanMove =false;
			en.ActiveInvistigators[ en.clock.GetCurPlayer()].MovementPoints=0;
			en.io.ServerWrite (Environment.NewLine);
			if (prot.GetIsHorror ())
			{
				new SkillTest (en, SkillTestType.Horror, prot.GetHorrorModif (), AfterHorrorCheck, 1, true, SkillTestType.Will);    
			} else
				if (isFirstEvade)
				FleeOrFight ();
			else 
				CombatCheck  (); 

		}


		private void AfterHorrorCheck( short suc)
		{ if (suc == 0)
			{
				EffSanityLose l = new EffSanityLose (en);
				l.ChangeDemageCount (prot.GetSanityDemage ());
				l.Execute (FleeOrFight); 

			} else
				if (prot.GetExtraSanity () != 0)
			{ en.io.ServerWrite (Environment.NewLine + "(" + en.sysstr.GetString (SSType.ExtraHorror) + ")", 12, true);    
				EffSanityLose l = new EffSanityLose (en);
				l.ChangeDemageCount (prot.GetExtraSanity ());
				l.Execute (FleeOrFight); 


			}
			else
				FleeOrFight (); 

		}


		private void CombatCheck()
		{  itemModif = 0;
			en.ActiveInvistigators [en.clock.GetCurPlayer ()].BeforeCombatCheck ();  

		}
		public void DoCombatCheck()
		{ en.io.ServerWrite (Environment.NewLine); 
			new SkillTest (en, SkillTestType.Combat ,  prot.GetCombatModif () , AfterCombatCheck, prot.GetTougness() , true, SkillTestType.Fight, itemModif  );    
		}


		private void AfterCombatCheck( short suc)
		{ if (suc >= prot.GetTougness ())
			{ if (prot.GetExtraCombat () != 0)
				{  MakeATrofy (); 
					en.io.ServerWrite (Environment.NewLine + "(" + en.sysstr.GetString (SSType.ExtraCombat) + ")", 12, true);    
					EffStaminaLose l = new EffStaminaLose (en);
					l.ChangeDemageCount (prot.GetExtraCombat ());
					l.Execute (EndEncounter); 


				} else
				{  MakeATrofy (); 
					EndEncounter (); 
				}
			}
			else
			{  prot.DoDemage (FleeOrFight); 

			}
			

		}

		private void FleeOrFight()
		{ if (prot.GetAmbush() )
			{
				CombatCheck ();
			} else
			{
				string str = en.sysstr.GetString (SSType.EvadeOrFight1) + "  " + prot.GetEvadeTitle () + " " + en.sysstr.GetString (SSType.EvadeOrFight2) + " " + prot.GetFightTitle (); 
				en.io.YesNoStart (str, en.sysstr.GetString (SSType.EvadeButton), en.sysstr.GetString (SSType.FightButton), FleeCheck, CombatCheck);  
			}
		}

		private void FleeCheck()
		{ 
			en.io.ServerWrite (Environment.NewLine); 
			new SkillTest (en, SkillTestType.Evade, prot.GetEvadeModif (), AfterFleeCheck, 1, true, SkillTestType.Sneak);    

		}


		private void AfterFleeCheck( short suc)
		{ if (suc > 0)
			EndEncounter (); 
			else
			{
				prot.DoDemage(FleeOrFight);


			}

		}

		public void EndEncounter()
		{ 
			en.io.ServerWrite(Environment.NewLine); 
			en.io.ServerWrite(Environment.NewLine); 
			RPAfterEncounter(); 

		}

		public string GetTitle ()
		{  return prot.GetTitle() ;

		}

		public void PrintToMap( byte label)
		{ en.io.ClientWrite(prot.GetTitle() , 12,true,true,label); 

		}

		public override void ReadFromSave (System.IO.BinaryReader rd)
		{   locnum= rd.ReadInt16();
			monsternum =(byte) (en.ActiveMonsters.Count-1); 
			if (locnum == 0)
			en.Scy.Add (this);
			else
			((ArchemArea ) en.locs[locnum]).AddMonster(this);
			isEncountred=false;

		}

		public override void WriteToSave( System.IO.BinaryWriter wr)
		{ base.WriteToSave(wr);
			wr.Write (locnum);

		}


		public void Print()
		{
			prot.PrintServer ();
		}

		public void PrintClient()
		{ prot.PrintClient (); 

		}

		public void MakeATrofy()
		{  if ( locnum!= -1)
		   {
				if (locnum == 0)
				en.Scy.Remove (this);  
				else
				((ArchemArea)en.locs [locnum]).RemoveMonster (this);  
			en.ActiveMonsters.Remove (this); 
		   }
			if ( ! prot.GetEndless ())
			{
				en.ActiveInvistigators [en.clock.GetCurPlayer ()].AddTrophy (this);

			}


		}


		public void PrintAsTrofy()
		{
			en.io.ClientWrite (prot.GetTitle () + "  (" + prot.GetTougness () + ")");  
		}

		public void AddItemModif( byte modifChange, bool isPhysical=false, bool  isMagic=false)
		{
			itemModif += prot.AddItemModif (modifChange, isPhysical, isMagic);   
		}


		public void Move( bool isWhite)
		{  short locToMove;
			if ( locnum != 0 && en.locs [locnum].GetInvestCount () != 0)
			{
				en.curs.resolvingMythos.Step3Circle ();
				return;
			}

			switch (prot.GetMoveType() )
			{ 	
			case MonsterMovementType.Inmobile:
				{
				en.curs.resolvingMythos.Step3Circle ();
				return;  }

				case MonsterMovementType.Normal:
				{  
					if (isWhite)
						locToMove = ((ArchemArea)en.locs [locnum]).GetWhiteArrow();
				   else
						locToMove = ((ArchemArea)en.locs [locnum]).GetBlackArrow();
					SetLocathion (locToMove); 
			      en.curs.resolvingMythos.Step3Circle ();
					                  return;  }

				 
				case MonsterMovementType.Fast:
				{ 
					if (isWhite)
					{
						locToMove = ((ArchemArea)en.locs [locnum]).GetWhiteArrow ();
						locToMove = ((ArchemArea)en.locs [locToMove]).GetWhiteArrow (); 
					} else
					{
						locToMove = ((ArchemArea)en.locs [locnum]).GetBlackArrow ();
						locToMove = ((ArchemArea)en.locs [locToMove]).GetBlackArrow (); 
					}
				SetLocathion (locToMove); 
				en.curs.resolvingMythos.Step3Circle ();
					return;

				}


			case MonsterMovementType.Special: 
			{ prot.ExecuteMoveEffect (); return; 

				}


			case MonsterMovementType.Fly:
			{  
					MovementFly (); 
				return;

			}
           


        }

	}
	

	private void MovementFly()
		{	List<short> potLoc= new List<short>();
			byte minSneak=100;
			Locathion loc;
			if (locnum==0) //scy
			{
				for (int i=0; i< en.locs.Count; i++)  
				{ loc = en.locs [i];
					if (loc.GetLocType () == LocathionType.ArchamStreet && loc.GetMinSneak  () <= minSneak)
					{ potLoc.Add ((short)i);
						minSneak = loc.GetMinSneak ();  
					}
			    }


			} 
			else
			{ if (en.locs [locnum].GetLocType () != LocathionType.ArchamStreet)
				{
					short streetLocNum = ((ArchemArea)en.locs [locnum]).GetWhiteArrow ();
					if (en.locs [streetLocNum].GetInvestCount () != 0)
						potLoc.Add (streetLocNum); 

				} 
				else
				{
					List<short> streets = ((ArchemStreet)en.locs [locnum]).GetLinkStreets ();  
					for (int i=0; i< streets.Count; i++)
					{
						loc = en.locs [streets [i]];
						if (loc.GetMinSneak () <= minSneak)
							potLoc.Add (streets [i]);

					}

				}

		    }


			if (potLoc.Count == 0)
			{   if (locnum!=0)
				{ SetLocathion (0);
				} 

				en.curs.resolvingMythos.Step3Circle ();
				return;
				
			}

			if (potLoc.Count == 1)
			{
				SetLocathion (potLoc [0]);
				en.curs.resolvingMythos.Step3Circle ();
				return;
			}

			//несколько вожможных локаций
			List<IOOption> iop = new List<IOOption> ();
			foreach (short li in potLoc)
			{ iop.Add( new IOOptionWithParam(en.locs[li].GetTitle(),FlyMove2, li));

			}
			en.io.StartChoose (iop, prot.GetTitle () + "  " + en.sysstr.GetString (SSType.MonsterMovePromt), en.sysstr.GetString (SSType.Confirm));     
			return;
		    

		}

		private void FlyMove2(short locind)
		{ 
			SetLocathion (locind);
			en.curs.resolvingMythos.Step3Circle ();
			return;

		}

		public byte GetDs()
		{
			return prot.GetDs ();
		}
	}


	public class MonsterPrototype
	{ private GameEngine en;
		private string Title, FightTitle, EvadeTitle, CodeName;
		private short EvadeModif, HorrorModif, CombatModif;
		private byte HorrorDemage, CombatDemage;
		private bool isHorrorCheck=true;
		private bool MR=false, MI=false, PR=false, PI=false, isEndless=false, isAmbush=false;
		private byte ExtraCombat=0, ExtraHorror=0;
		private byte Tougness=1;
		private byte dsindex;
		private Effect SpecialDemageEffect=null, SpecialMovementEffect=null, SpecialTrofyEffect=null, InsteadEncEffect=null;
		private List< String> SpecAbilText; 
		private MonsterMovementType moveType;
		private  bool isUndead=false, isMask=false, isSpawn=false;
		public string GetTitle()
		{ return Title;

		}

		public string GetEvadeTitle()
		{ return EvadeTitle;

		}

		public string GetFightTitle()
		{ return FightTitle;

		}



		public short GetEvadeModif()
		{
			return EvadeModif;
		}

		public short GetHorrorModif()
		{
			return HorrorModif; 
		}

		public short GetCombatModif()
		{
			return CombatModif; 
		}


		public string GetCodeName()
		{
			return CodeName;
		}

		public byte GetDs()
		{
			return dsindex;
		}


		public byte GetSanityDemage()
		{ return HorrorDemage; 

		}

		public byte GetTougness()
		{
			return Tougness;
		}

		public bool GetIsHorror()
		{
			return isHorrorCheck; 
		}

		public  byte GetExtraSanity()
		{ return ExtraHorror; 

		}

		public  byte GetExtraCombat()
	    {
			return ExtraCombat; 
		}

		public bool GetEndless()
		{
			return isEndless;
		}

		public bool GetAmbush()
		{ return isAmbush;

		}

		public bool GetUndead()
		{
			return isUndead;
		}

		public bool GetMask()
		{
			return isMask;
		}

		public bool GetSpawn()
		{  return isSpawn;

		}

		public MonsterMovementType GetMoveType()
		{ return moveType;

		}
		public MonsterPrototype( GameEngine eng)
		{ en=eng;
			SpecAbilText = new List<string> (); 
	

		
		}






		public void PrintServer()
		{
			en.io.ServerWrite (Title, 14, true);
			en.io.ServerWrite ("       " + en.sysstr.GetString (SSType.DS) + ": " + en.ds.GetTitle (dsindex) );
			en.io.ServerWrite ("       " + en.sysstr.GetMonsterMovementString(moveType));

			if( isUndead)
				en.io.ServerWrite ("       " + en.sysstr.GetString(SSType.Undead), 12, true,true);
			if( isSpawn)
				en.io.ServerWrite ("       " + en.sysstr.GetString(SSType.Spawn), 12, true,true);
			if( isMask)
				en.io.ServerWrite ("       " + en.sysstr.GetString(SSType.Mask), 12, true,true);
			en.io.ServerWrite (Environment.NewLine+ en.sysstr.GetString (SSType.Awerness), 12, true);
			en.io.ServerWrite ("  ");
			if (EvadeModif > 0)
				en.io.ServerWrite ("+");
			en.io.ServerWrite (EvadeModif.ToString ());
			if( isAmbush)
				en.io.ServerWrite ("       "+ en.sysstr.GetString (SSType.Ambush ), 12, true);

			en.io.ServerWrite (Environment.NewLine+ en.sysstr.GetString (SSType.HorrorModif )+ "   ", 12, true);
			if (InsteadEncEffect != null)
			{ en.io.ServerWrite ("-" + Environment.NewLine);
			  en.io.ServerWrite (en.sysstr.GetString (SSType.CombatModif) + "   ", 12, true);
			  en.io.ServerWrite ("-" + Environment.NewLine);

			} else
			{
				if (isHorrorCheck)
				{
					if (HorrorModif >= 0)
						en.io.ServerWrite ("+"); 
					en.io.ServerWrite (HorrorModif.ToString ());
					en.io.ServerWrite ("        " + en.sysstr.GetString (SSType.HorrorDemage) + "   ", 12, true);
					en.io.ServerWrite (HorrorDemage.ToString ());
					if (ExtraHorror != 0)
						en.io.ServerWrite ("        " + en.sysstr.GetString (SSType.ExtraHorror) + " " + ExtraHorror, 12, true);
					en.io.ServerWrite (Environment.NewLine);   

				} else
					en.io.ServerWrite ("-" + Environment.NewLine);

				en.io.ServerWrite (en.sysstr.GetString (SSType.CombatModif) + "   ", 12, true);
				if (CombatModif >= 0) 
					en.io.ServerWrite ("+"); 
				en.io.ServerWrite (CombatModif.ToString ());
				en.io.ServerWrite ("        " + en.sysstr.GetString (SSType.CombatDemage) + "   ", 12, true);
				if (CombatDemage == 0)
					en.io.ServerWrite ("  " + en.sysstr.GetString (SSType.MonsterDemageSpecial));
				else
					en.io.ServerWrite (CombatDemage.ToString ());
				if (ExtraCombat != 0)
					en.io.ServerWrite ("        " + en.sysstr.GetString (SSType.ExtraCombat) + " " + ExtraCombat, 12, true);
				en.io.ServerWrite (Environment.NewLine);   
			}
			en.io.ServerWrite (en.sysstr.GetString (SSType.Tougness ), 12,true );
			en.io.ServerWrite ("  ");
			en.io.ServerWrite ( Tougness.ToString());

			if( MR)
				en.io.ServerWrite ("       "+ en.sysstr.GetString (SSType.MR ), 12, true);
			if( MI)
				en.io.ServerWrite ("       "+ en.sysstr.GetString (SSType.MI ), 12, true);
			if( PR)
				en.io.ServerWrite ("       "+ en.sysstr.GetString (SSType.PR ), 12, true);
			if( PI)
				en.io.ServerWrite ("       "+ en.sysstr.GetString (SSType.PI ), 12, true);
			en.io.ServerWrite (Environment.NewLine);
			if( isEndless)
				en.io.ServerWrite ( en.sysstr.GetString (SSType.Endless )+ Environment.NewLine , 12, true);
			foreach (string str in SpecAbilText)
				en.io.ServerPrintTag (str + Environment.NewLine); 
			en.io.ServerWrite (Environment.NewLine); 


		}


		public void PrintClient()
		{
			en.io.ClientWrite (Title, 14, true);
			en.io.ClientWrite ("       " + en.sysstr.GetString (SSType.DS) + ": " + en.ds.GetTitle (dsindex) );
			en.io.ClientWrite ("       " + en.sysstr.GetMonsterMovementString(moveType));

			if( isUndead)
				en.io.ClientWrite ("       " + en.sysstr.GetString(SSType.Undead), 12, true,true);
			if( isSpawn)
				en.io.ClientWrite ("       " + en.sysstr.GetString(SSType.Spawn), 12, true,true);
			if( isMask)
				en.io.ClientWrite ("       " + en.sysstr.GetString(SSType.Mask), 12, true,true);
			en.io.ClientWrite (Environment.NewLine+ en.sysstr.GetString (SSType.Awerness), 12, true);
			en.io.ClientWrite ("  ");
			if (EvadeModif > 0)
				en.io.ClientWrite ("+");
			en.io.ClientWrite (EvadeModif.ToString ());
			if( isAmbush)
				en.io.ClientWrite ("       "+ en.sysstr.GetString (SSType.Ambush ), 12, true);

			en.io.ClientWrite (Environment.NewLine+ en.sysstr.GetString (SSType.HorrorModif )+ "   ", 12, true);
			if (InsteadEncEffect != null)
			{ en.io.ClientWrite ("-" + Environment.NewLine);
				en.io.ClientWrite (en.sysstr.GetString (SSType.CombatModif) + "   ", 12, true);
				en.io.ClientWrite ("-" + Environment.NewLine);

			} else
			{
				if (isHorrorCheck)
				{
					if (HorrorModif >= 0)
						en.io.ClientWrite ("+"); 
					en.io.ClientWrite (HorrorModif.ToString ());
					en.io.ClientWrite ("        " + en.sysstr.GetString (SSType.HorrorDemage) + "   ", 12, true);
					en.io.ClientWrite (HorrorDemage.ToString ());
					if (ExtraHorror != 0)
						en.io.ClientWrite ("        " + en.sysstr.GetString (SSType.ExtraHorror) + " " + ExtraHorror, 12, true);
					en.io.ClientWrite (Environment.NewLine);   

				} else
					en.io.ClientWrite ("-" + Environment.NewLine);

				en.io.ClientWrite (en.sysstr.GetString (SSType.CombatModif) + "   ", 12, true);
				if (CombatModif >= 0) 
					en.io.ClientWrite ("+"); 
				en.io.ClientWrite (CombatModif.ToString ());
				en.io.ClientWrite ("        " + en.sysstr.GetString (SSType.CombatDemage) + "   ", 12, true);
				if (CombatDemage == 0)
					en.io.ClientWrite ("  " + en.sysstr.GetString (SSType.MonsterDemageSpecial));
				else
					en.io.ClientWrite (CombatDemage.ToString ());
				if (ExtraCombat != 0)
					en.io.ClientWrite ("        " + en.sysstr.GetString (SSType.ExtraCombat) + " " + ExtraCombat, 12, true);
				en.io.ClientWrite (Environment.NewLine);   
			}
			en.io.ClientWrite (en.sysstr.GetString (SSType.Tougness ), 12,true );
			en.io.ClientWrite ("  ");
			en.io.ClientWrite ( Tougness.ToString());

			if( MR)
				en.io.ClientWrite ("       "+ en.sysstr.GetString (SSType.MR ), 12, true);
			if( MI)
				en.io.ClientWrite ("       "+ en.sysstr.GetString (SSType.MI ), 12, true);
			if( PR)
				en.io.ClientWrite ("       "+ en.sysstr.GetString (SSType.PR ), 12, true);
			if( PI)
				en.io.ClientWrite ("       "+ en.sysstr.GetString (SSType.PI ), 12, true);
			en.io.ClientWrite (Environment.NewLine);
			if( isEndless)
				en.io.ClientWrite ( en.sysstr.GetString (SSType.Endless )+ Environment.NewLine , 12, true);
			foreach (string str in SpecAbilText)
				en.io.ClientPrintTag (str + Environment.NewLine); 
			en.io.ClientWrite (Environment.NewLine); 


		}


		public bool FromTextFile( TextFileParser data, TextFileParser text)
		{ CodeName= data.GetToken().ToUpper();
			Title = text.GetCurString ();
			if (text.isMultiName)
			{ EvadeTitle =  text.GetCurString () ;
			  FightTitle = text.GetCurString (); 

			} else
			{ EvadeTitle = Title;
				FightTitle = Title; 
			}
			int dsindex2 = en.ds.GetIndex (data.GetToken());
			if (dsindex2 == -1)
				return false;
			else
				dsindex = (byte)dsindex2;
			switch (data.GetToken ().ToUpper ())
		{ case "NORMAL": moveType = MonsterMovementType.Normal; break;
			case "FAST": moveType = MonsterMovementType.Fast; break; 
			case "FLY": moveType = MonsterMovementType.Fly; break; 
			case "INMOB": moveType = MonsterMovementType.Inmobile; break; 
			case "SPECIAL":
				{
					moveType = MonsterMovementType.Special;
					SpecialMovementEffect = Effect.FromTextFile (data, en);
					if (SpecialMovementEffect == null)
						return false;

				} break; 

			}
			if ( ! short.TryParse(data.GetToken(), out EvadeModif))
				return false;
			string s = data.GetToken ();
			if (s.ToUpper () == "INSENC")
			{
				InsteadEncEffect = Effect.FromTextFile (data, en);
				if (InsteadEncEffect == null)
					return false;

			} else
			{
				if (s.ToUpper () == "NONE")
					isHorrorCheck = false;
				else
				{
					if (! short.TryParse (s, out HorrorModif))
						return false;
					if (! byte.TryParse (data.GetToken (), out HorrorDemage))
						return false;
				}

			 

				if (! short.TryParse (data.GetToken (), out CombatModif))
					return false;
				if (! byte.TryParse (data.GetToken (), out CombatDemage))
					return false;
				if (CombatDemage == 0)
				{
					SpecialDemageEffect = Effect.FromTextFile (data, en);
					if (SpecialDemageEffect == null)
						return false;
				}
			}
			if ( ! byte.TryParse(data.GetToken(), out Tougness ))
				return false;
			byte SpecAbilCount;
			if ( ! byte.TryParse(data.GetToken(), out SpecAbilCount  ))
				return false;
			for (byte i=0; i< SpecAbilCount; i++)
				switch (data.GetToken().ToUpper())
			{ case "NIGHT": 
				{ if ( ! byte.TryParse(data.GetToken(), out ExtraHorror))
							return false; } break;
				case "OVER": 
				{ if ( ! byte.TryParse(data.GetToken(), out ExtraCombat))
					return false; } break;

				case "MI": { MI = true;  } break;
				case "MR": { MR = true;  } break;
				case "PI": { PI = true;  } break;
				case "PR": { PR = true;  } break;
				case "ENDLESS": { isEndless  = true;  } break;
				case "AMBUSH": { isAmbush  = true;  } break;
				case "UNDEAD": { isUndead  = true;  } break;
				case "MASK": { isMask  = true;  } break;
				case "SPAWN": { isSpawn  = true;  } break;
				default: return false;
			}
			if ( ! byte.TryParse(data.GetToken(), out SpecAbilCount  ))
				return false;
			for (byte i=0; i< SpecAbilCount; i++)
				SpecAbilText.Add (text.GetCurString ());  

			byte countInCup;
			if ( ! byte.TryParse(data.GetToken(), out countInCup ))
				return false;
			for( byte i=0; i< countInCup; i++)
				en.MonstersCup.Add( new MonsterIndivid(en,this, en.MonstersCup.GetCountOfCards()));  
			return true;

		}

		public  void DoDemage( Func f)
		{ if (SpecialDemageEffect != null)
				SpecialDemageEffect.Execute (f);
			else
			{ EffStaminaLose l = new EffStaminaLose (en);
				l.ChangeDemageCount (CombatDemage);
				l.Execute (f); 

			}

		}


		public byte AddItemModif( byte modifChange, bool isPhysical, bool  isMagic)
		{ if (PR && isPhysical)
			return (byte)Math.Round ((double)modifChange/ 2);
		  if (PI && isPhysical)
				return 0;
			if (MR && isMagic)
				return (byte)Math.Round ((double)modifChange/ 2);
			if (MI && isMagic)
				return 0;

			return modifChange; 
		}


		public  void ExecuteMoveEffect()
		{ 
			SpecialMovementEffect.Execute( en.curs.resolvingMythos.Step3Circle);   
		}


        


	}

	public enum MonsterMovementType
	{ Normal=0,
      Fly,
	  Fast,
	  Special,
	  Inmobile
	}
}

