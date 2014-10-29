using System;
using System.Collections.Generic;  

namespace mmxAH
{
	public class Investigator: Card
	{ private string DisplayName;
		private short locnum;
		private byte investnum;
		private byte clues;
		private string Pronoun;
		private string Occupashion;
		private GameEngine en;
		public TrigerCollrecthion myTrigers;
		public byte MovementPoints=0;
		private ArcSlider SpeedSneak, FightWill, LoreLuck;
		public SkillTestInfos STInfo;
		private byte maxSanity;
		private byte maxStamina;
		private byte curSanity;
		private  byte curStamina;
		private SetupInfo setupi;
		private byte Money;
		private byte Focus, CurFocus;
		private List<SpecialCard> specCards; 
		private byte STTresh=5;
		private bool isDelayed=false;
	    public bool isCanMove=true;
		private List< MonsterIndivid> monsterTrofies;
		private List<GatePrototype> gateTrofies;


		private struct SetupInfo
		{ public short StartLoc;
			public byte StartClues;
			public byte StartMoney;
			public byte DrawCommon;
			public byte DrawUnice;
			public byte DrawSpells;
			public byte DrawSkils; 


		}


		private class ArcSlider
		{ private const byte slider_places=4;
			private byte [] charA, charB;
			private byte curPos;
			private string Title;
			private GameEngine en;
			public ArcSlider(GameEngine eng, string tit)
			{ charA= new byte[slider_places] ;
				charB= new byte[slider_places] ;
				curPos=0; 
				Title=tit;
				en=eng;
			}

		

			public byte GetFirstChar()
			{
				return charA [curPos];
			}

			public byte GetSecondChar()
			{
				return charB [curPos];
			}

			public bool FromTextFile(TextFileParser data)
			{ if (! Byte.TryParse (data.GetToken (), out charA[0])) 
				return false;
			  if (! Byte.TryParse (data.GetToken (), out charA[1])) 
					return false;
			  if (! Byte.TryParse (data.GetToken (), out charA[2])) 
					return false;
			  if (! Byte.TryParse (data.GetToken (), out charA[3])) 
					return false;
			  if (! Byte.TryParse (data.GetToken (), out charB[0])) 
					return false;
			   if (! Byte.TryParse (data.GetToken (), out charB[1])) 
					return false;
			   if (! Byte.TryParse (data.GetToken (), out charB[2])) 
					return false;
			   if (! Byte.TryParse (data.GetToken (), out charB[3])) 
					return false; 
				return true;
			}

			public string PrintCharA()
			{  string res="";
				for (int i=0; i<slider_places; i++)
				{
					res += charA [i];
					if (i == curPos)
						res += "*";
					res += "\t";
				}
				return res;

			}


			public string PrintCharB()
			{ string res="";
				for (int i=0; i<slider_places; i++)
				{
					res += charB [i];
					if (i == curPos)
						res += "*";
					res += "\t";
				}
				return res;



			}

			public void SetToPlace(byte place)
			{ if (place < 0 && place >= slider_places)
					return;
				curPos = place; 
				PrintPlacement (); 

			}

			public void IncriseA()
			{ if (curPos < slider_places - 1) 
					curPos++;
				PrintPlacement ();  
				en.ActiveInvistigators [en.clock.GetCurPlayer()].DecreaseCurFocus(); 
				en.ActiveInvistigators [en.clock.GetCurPlayer()].Change_Sliders ();  


			}

			public void IncriseB()
			{ if( curPos >= 1) 
				curPos--;
				PrintPlacement ();
				en.ActiveInvistigators [en.clock.GetCurPlayer()].DecreaseCurFocus(); 
				en.ActiveInvistigators [en.clock.GetCurPlayer()].Change_Sliders (); 

			}

			private void PrintPlacement()
			{
				en.io.ServerWrite (Title, 12, true);
				en.io.ServerWrite (" "+ en.sysstr.GetString (SSType.isPlacedTo)+ " " );
				en.io.ServerWrite (charA [curPos] + "/" + charB[curPos]+ Environment.NewLine , 12, false, true);    

			}

			public bool  CanIncreseA()
			{ return  curPos < slider_places - 1;

			}

			public bool CanIncreseB()
			{ return (curPos >= 1); 

			}
			public List<IOOption> SetupChoses( FuncWithParam rp)
			{ List<IOOption> res = new List<IOOption> ();

				for (byte i=0; i<slider_places; i++)
					res.Add( new IOOptionWithParam(charA[i]+"/"+charB[i], rp, i)); 
				return res;

			}

			public string GetTitle()
			{
				return Title;
			}


			public void WriteToSave(System.IO.BinaryWriter wr)
			{  wr.Write (curPos); 

			}


			public void ReadFromSave(System.IO.BinaryReader rd)
			{ curPos = rd.ReadByte (); 
			}

			public void Reset()
			{
				curPos = 0; 
			}
		}


		public Investigator (GameEngine eng, short pId)
		{ en=eng;
			ID = pId;
			myTrigers = new TrigerCollrecthion();
			SpeedSneak = new ArcSlider (en, en.sysstr.GetCharekteresticName(SkillTestType.Speed)+ "/"+ en.sysstr.GetCharekteresticName(SkillTestType.Sneak )  );
			FightWill =  new ArcSlider (en, en.sysstr.GetCharekteresticName(SkillTestType.Fight)+ "/"+ en.sysstr.GetCharekteresticName(SkillTestType.Will )  );
			LoreLuck =   new ArcSlider (en, en.sysstr.GetCharekteresticName(SkillTestType.Lore)+ "/"+ en.sysstr.GetCharekteresticName(SkillTestType.Luck )  );
			STInfo= new SkillTestInfos(); 
			specCards = new List< SpecialCard> (); 
			monsterTrofies = new List<MonsterIndivid> ();
			gateTrofies = new List<GatePrototype> (); 




		}

		public string GetTitle()
		{ return DisplayName;  

		}
		public void SetLocathion( short newLoc)
		{
			en.locs[locnum].RemoveInvestigator(investnum);
			locnum= newLoc;
			en.locs[locnum].AddInvestigator(investnum);
		}

		public short GetLocathion()
		{
			return locnum;
		}

		public void Setup(byte newInvestNum)
		{
			//Нельзя автодобовления из-за вожможной замены после devour
			investnum= newInvestNum;

			locnum = setupi.StartLoc;
			en.locs [locnum].AddInvestigator (investnum);

			clues = setupi.StartClues;
			Money = setupi.StartMoney;  
			curSanity = maxSanity;
			curStamina = maxStamina;  
			//ЗДЕСЬ ДОБАВЛЕНИЕ СПОСОБНОСТЕЙ В ТРИГЕРНЫЕ СПИСКИ, пОЛУЧЕНИЕ ВЕЩЕЙ ИТП


			

		}

		public void  SetupRandom()
		{

			en.clock.NextPlayer (); 

		}

		public void MoveChicle()
		{
			en.locs[locnum].Move ();

		}

		public void MoveTo (short locindex)
		{ MovementPoints -= ((ArchamArea) en.locs[locindex]).GetCost();

			en.io.ServerWrite  ( DisplayName+ "  "+ en.sysstr.GetString( SSType.MoveToFact)+ " ");
			en.io.ServerWrite (en.locs[locindex].GetMoveToTitle(), 12, false, true);
			en.io.ServerWrite (" .  " +en.sysstr.GetString(SSType.RemainMP) + "  "+ MovementPoints+ Environment.NewLine);
			SetLocathion (locindex);
			en.map.Print(); 
			en.locs[locindex].Move ();

		}

		public byte GetCluesValue()
		{ return clues;

		}

		public void AddClues( byte count)
		{  
				clues += count;

				
			en.io.ServerWrite  (Pronoun + "  "+ en.sysstr.GetString (SSType.NowHas) +  "  " + en.sysstr.GetNumberClueToken (clues) + "."+ Environment.NewLine );      
           

		}

		public void RemoveClues( byte count)
		{  


			if (count > clues)
				clues = 0;
			else
				clues -= count;

			en.io.ServerWrite (DisplayName + "  " + en.sysstr.GetString (SSType.DiscardFact) + " ");  
			 en.io.ServerWrite (en.sysstr.GetNumberClueToken(count),12,false,false,1,"Green");
			en.io.ServerWrite (" ( " + clues + " " + en.sysstr.GetString (SSType.Remain)+"). " );


		}


		public short GetCharValue(SkillTestType t)
		{ 
			switch (t)
		{ 	case SkillTestType.Speed:  return  SpeedSneak.GetFirstChar ();   
			case SkillTestType.Sneak : return SpeedSneak.GetSecondChar ();   
			case SkillTestType.Fight : return FightWill.GetFirstChar (); 
			case SkillTestType.Will: return FightWill.GetSecondChar ();  
			case SkillTestType.Lore: return  LoreLuck .GetFirstChar ();  
			case SkillTestType.Luck: return  LoreLuck .GetSecondChar ();   
			default: return 0;
			}

		
		}

		public bool FromTextFile( TextFileParser data, TextFileParser text)
		{ DisplayName= text.GetCurString();
		  Pronoun = text.GetCurString();
		  Occupashion = text.GetCurString ();
			if (! Byte.TryParse (data.GetToken (), out maxSanity)) 
				return false;
			if (! Byte.TryParse (data.GetToken (), out maxStamina)) 
				return false;

			string sl = data.GetToken ();
			setupi.StartLoc = en.map.GetNumberByCodeName (sl);
			if (setupi.StartLoc == -1)
				return false;

			if (! Byte.TryParse (data.GetToken (), out setupi.StartClues )) 
				return false;
			if (! Byte.TryParse (data.GetToken (), out setupi.StartMoney )) 
				return false;


			//ЗАГЛУШКА ОСОБЕНЫХ FIX POSESSHION
			data.GetToken ();

			if (! Byte.TryParse (data.GetToken (), out setupi.DrawCommon )) 
				return false;
			if (! Byte.TryParse (data.GetToken (), out setupi.DrawUnice )) 
				return false;
			if (! Byte.TryParse (data.GetToken (), out setupi.DrawSpells )) 
				return false;
			if (! Byte.TryParse (data.GetToken (), out setupi.DrawSkils )) 
				return false;

			if (! Byte.TryParse (data.GetToken (), out Focus)) 
				return false;

			if (! SpeedSneak.FromTextFile (data))
				return false;
			if (! FightWill.FromTextFile (data))
				return false;
			if (! LoreLuck.FromTextFile (data))
				return false;

			//ЗАГЛУШКА S. A.
			data.GetToken (); 
			return true;

		}

		public void Print()
		{  en.io.ClientWrite(DisplayName,12, false, true);
			en.io.ClientWrite ("," + Occupashion );
			if(isDelayed) 
				en.io.ClientWrite ("    "+en.sysstr.GetString( SSType.Delayed)   , 12, false,true); 
			if (investnum == en.clock.GetFirstPlayer ())
				en.io.ClientWrite ("    "+en.sysstr.GetString( SSType.FirstPlayer)   , 12, true); 
			en.io.ClientWrite (Environment.NewLine+ en.sysstr.GetNumberClueToken (clues),12, false,false,1,"Green");     
			en.io.ClientWrite ("  $" + Money + Environment.NewLine);   
			en.io.ClientWrite (en.sysstr.GetString(SSType.Sanity)    + ": ", 12, true,false,1, "Blue");
			en.io.ClientWrite ( curSanity + "/ " + maxSanity + Environment.NewLine,12,false,false,1,"Blue"); 
			en.io.ClientWrite (en.sysstr.GetString(SSType.Stamina)    + ": ", 12, true, false,1, "Red");
			en.io.ClientWrite ( curStamina + "/ " + maxStamina + Environment.NewLine,12,false, false,1, "Red");
			en.io.ClientWrite (en.sysstr.GetString(SSType.Focus)   + ": ", 12, true);
			en.io.ClientWrite (Focus+ Environment.NewLine );  


			en.io.ClientWrite  (en.sysstr.GetString(SSType.Speed )  + "/" +en.sysstr.GetString(SSType.Sneak ) + Environment.NewLine); 
			en.io.ClientWrite (SpeedSneak.PrintCharA () + STInfo.PrintInfo (SkillTestType.Speed, en.sysstr));
			en.io.ClientWrite(en.GlobalModifs.PrintInfo(SkillTestType.Speed, en.sysstr) +Environment.NewLine,12,true  ); 
			en.io.ClientWrite (SpeedSneak.PrintCharB()+STInfo.PrintInfo(SkillTestType.Sneak,en.sysstr)+STInfo.PrintInfo(SkillTestType.Evade, en.sysstr) ); 
			en.io.ClientWrite(en.GlobalModifs.PrintInfo(SkillTestType.Sneak,en.sysstr) +en.GlobalModifs.PrintInfo(SkillTestType.Evade, en.sysstr)+ Environment.NewLine,12,true  );
			en.io.ClientWrite  (Environment.NewLine + en.sysstr.GetString(SSType.Fight )  + "/" +en.sysstr.GetString(SSType.Will ) + Environment.NewLine); 
			en.io.ClientWrite (FightWill.PrintCharA()+STInfo.PrintInfo(SkillTestType.Fight, en.sysstr)+STInfo.PrintInfo(SkillTestType.Combat,en.sysstr)  ); 
			en.io.ClientWrite (en.GlobalModifs.PrintInfo(SkillTestType.Fight, en.sysstr)+en.GlobalModifs.PrintInfo(SkillTestType.Combat,en.sysstr)+Environment.NewLine,12,true  ); 
			en.io.ClientWrite (FightWill.PrintCharB()+STInfo.PrintInfo(SkillTestType.Will,en.sysstr)+STInfo.PrintInfo(SkillTestType.Horror, en.sysstr) ); 
			en.io.ClientWrite (en.GlobalModifs.PrintInfo(SkillTestType.Will,en.sysstr)+en.GlobalModifs.PrintInfo(SkillTestType.Horror, en.sysstr)+Environment.NewLine,12,true  ); 
			en.io.ClientWrite  (Environment.NewLine + en.sysstr.GetString(SSType.Lore )  + "/" +en.sysstr.GetString(SSType.Luck ) + Environment.NewLine); 
			en.io.ClientWrite (LoreLuck.PrintCharA()+STInfo.PrintInfo(SkillTestType.Lore,en.sysstr)+STInfo.PrintInfo(SkillTestType.SpellCast , en.sysstr)  ); 
			en.io.ClientWrite (en.GlobalModifs.PrintInfo(SkillTestType.Lore,en.sysstr)+en.GlobalModifs.PrintInfo(SkillTestType.SpellCast , en.sysstr)+Environment.NewLine,12,true  ); 
			en.io.ClientWrite (LoreLuck.PrintCharB()+STInfo.PrintInfo(SkillTestType.Luck, en.sysstr)  ); 
			en.io.ClientWrite (en.GlobalModifs.PrintInfo(SkillTestType.Luck, en.sysstr)+Environment.NewLine  ); 


			if (specCards.Count != 0)
			{
				en.io.ClientWrite (Environment.NewLine + en.sysstr.GetString(SSType.SpecialCards)  + Environment.NewLine, 12, true);
				foreach (SpecialCard sc in specCards)
					sc.Print ();
			}

			if (monsterTrofies .Count != 0)
			{
				en.io.ClientWrite (Environment.NewLine + en.sysstr.GetString(SSType.MonsterTrofies )  + Environment.NewLine, 12, true);
				for (int i = 0; i < monsterTrofies.Count; i++)
				{ if (i != 0)
						en.io.ClientWrite (","); 

				   monsterTrofies [i].PrintAsTrofy() ;

				}
			}



		}

		public void AddSpecCards( SpecialCard sc)
		{  specCards.Add (sc); 

		}

		public bool isSpecCards( string codeName)
		{  foreach (SpecialCard sc in specCards)
				if (sc.GetCodeName () == codeName)
					return true;
			return false;

		}


		public bool RemoveSpecCards( string codeName, bool isPrint=true)
		{  foreach (SpecialCard sc in specCards)
				if (sc.GetCodeName () == codeName)
				{
					
						 
					specCards.Remove (sc);
				sc.Discard (isPrint);
					return true;

				}

			return false;

		}

		public void SetSTTresh( byte value)
		{
			STTresh = value; 
		}

		public byte GetSTTresh()
		{
			return STTresh;
		}


		public void Upkeep()
		{   //здесь так как эфекты могут тратить
			CurFocus = Focus;

			en.clock.PrintCurPhase (); 
			myTrigers.ExecuteMandeoratory (TrigerEvent.Upkeep); 
			UpkeepChicle (); 

		}


		public void UpkeepChicle()
		{ List<IOOption> opts = myTrigers.GetChooseOpthions (TrigerEvent.Upkeep, 0);
			if (opts.Count == 0)
				Change_Sliders ();
			else
			{   opts.Add( new IOOpthionWithoutParam(en.sysstr.GetString(SSType.SetupSliders), Change_Sliders));   
				en.io.StartChoose (opts, en.sysstr.GetString (SSType.ChooseActhioPromt), en.sysstr.GetString (SSType.ChooseActhionButton));   
			}
		}

		private void Change_Sliders()
		{  if (CurFocus == 0)
			{
				en.clock.NextPlayer  ();
				return;
			}


			List<IOOption> opts = new List<IOOption> ();
			if( SpeedSneak.CanIncreseA())
				opts.Add( new IOOpthionWithoutParam ("+1 "+ en.sysstr.GetCharekteresticName(SkillTestType.Speed), SpeedSneak.IncriseA));  
			if( SpeedSneak.CanIncreseB())
				opts.Add( new IOOpthionWithoutParam ("+1 "+ en.sysstr.GetCharekteresticName(SkillTestType.Sneak ), SpeedSneak.IncriseB));     
			if( FightWill.CanIncreseA())
				opts.Add( new IOOpthionWithoutParam ("+1 "+ en.sysstr.GetCharekteresticName(SkillTestType.Fight), FightWill.IncriseA));     
			if( FightWill.CanIncreseB())
				opts.Add( new IOOpthionWithoutParam ("+1 "+ en.sysstr.GetCharekteresticName(SkillTestType.Will), FightWill.IncriseB));     
			if( LoreLuck.CanIncreseA())
				opts.Add( new IOOpthionWithoutParam ("+1 "+ en.sysstr.GetCharekteresticName(SkillTestType.Lore), LoreLuck.IncriseA));     
			if( LoreLuck.CanIncreseB())
				opts.Add( new IOOpthionWithoutParam ("+1 "+ en.sysstr.GetCharekteresticName(SkillTestType.Luck), LoreLuck.IncriseB)); 

			opts.Add( new IOOpthionWithoutParam ("End Upkeep",en.clock.NextPlayer ));

			en.io.SetServerFormMode (FormMode.Investigators);  
			en.io.StartChoose (opts, en.sysstr.GetString (SSType.ChooseActhioPromt), en.sysstr.GetString (SSType.ChooseActhionButton)); 
		}


		public void GainMoney( byte count)
		{  
			Money  += count;



			en.io.ServerWrite (DisplayName + " " + en.sysstr.GetString (SSType.Gain) + " $  " + count + ", ");
		    en.io.ServerWrite ( Pronoun + "  "+ en.sysstr.GetString (SSType.NowHas) +  "  $  " + Money + ".");      


		}

		public void LoseMoney( byte count)
		{  


			if (count > Money)
				Money = 0;
			else
				Money -= count;

			en.io.ServerWrite (DisplayName  + "  "+ en.sysstr.GetString (SSType.Lose)+ "  $ "+  count);
			en.io.ServerWrite (" ( $ " + Money + " " + en.sysstr.GetString (SSType.Remain)+"). " );


		}


		
		public bool LoseSanity( byte count)
		{  


			if (count > curSanity )
				curSanity  = 0;
			else
				curSanity  -= count;

			en.io.ServerWrite (DisplayName + "  " + en.sysstr.GetString (SSType.Lose) + " ");
			en.io.ServerWrite (count+  " " + en.sysstr.GetString(SSType.Sanity),12, false,false,1,"Blue");
			en.io.ServerWrite (" ( " +curSanity + " " + en.sysstr.GetString(SSType.Sanity)+ " "+ en.sysstr.GetString (SSType.Remain)+"). " );

			if( curSanity == 0)
			{
				// обработка безумия
				en.clock.NextPlayer ();
				return false;
			}
			return true;
		}


		
		public bool LoseStamina( byte count)
		{  


			if (count > curStamina )
				curStamina  = 0;
			else
				curStamina  -= count;

			en.io.ServerWrite (DisplayName + "  " + en.sysstr.GetString (SSType.Lose) + " ");
			en.io.ServerWrite (count+  " " + en.sysstr.GetString(SSType.Stamina ),12, false,false,1,"Red");
			en.io.ServerWrite (" ( " +curStamina + " " + en.sysstr.GetString(SSType.Stamina)+ " "+ en.sysstr.GetString (SSType.Remain)+"). " );

			if( curStamina == 0)
			{
				// обработка бессознательности
				en.clock.NextPlayer (); 
				return false;
			}
			return true;
		}


		public void Setup_Sliders()
		{  en.io.SetServerFormMode (FormMode.Investigators);  
			en.io.StartChoose (SpeedSneak.SetupChoses (Setup_Slider_1), en.sysstr.GetString (SSType.SetYourSlider) + " " + SpeedSneak.GetTitle (),  en.sysstr.GetString( SSType.ChooseActhionButton ));    
		}

		private void Setup_Slider_1(short p)
		{
			SpeedSneak.SetToPlace ((byte)p);  
			en.io.SetServerFormMode (FormMode.Investigators);  
			en.io.StartChoose (FightWill.SetupChoses (Setup_Slider_2), en.sysstr.GetString (SSType.SetYourSlider) + " " + FightWill.GetTitle (),  en.sysstr.GetString( SSType.ChooseActhionButton ));    

		}

		private void Setup_Slider_2(short p)
		{ FightWill.SetToPlace ((byte)p);  
			en.io.SetServerFormMode (FormMode.Investigators);  
			en.io.StartChoose (LoreLuck.SetupChoses (Setup_Slider_3), en.sysstr.GetString (SSType.SetYourSlider) + " " + LoreLuck.GetTitle (),  en.sysstr.GetString( SSType.ChooseActhionButton ));    


		}

		private void Setup_Slider_3(short p)
		{ LoreLuck.SetToPlace ((byte)p);  
			en.clock.NextPlayer  (); 

		}

		public void DecreaseCurFocus()
		{ if( CurFocus >= 1)
			CurFocus--; 
		}


		public void Movement()
		{   
			en.clock.PrintCurPhase (); 
			if (isDelayed)
			{ isDelayed = false;
				isCanMove = false; 
			  en.io.ServerWrite (DisplayName + " " + en.sysstr.GetString (SSType.DelaydEnd)+ Environment.NewLine);   
				MovementPoints = 0; 
			} else
			{ isCanMove = true;
				short mp;
				mp = (short)(SpeedSneak.GetFirstChar () + STInfo.GetInfo (SkillTestType.Speed).CharModif);
				mp += en.GlobalModifs.GetInfo (SkillTestType.Speed).CharModif; 
				mp += STInfo.GetMPModif ();
				mp += en.GlobalModifs.GetMPModif (); 
				if (mp < 0)
					mp = 0;
				MovementPoints = (byte)mp; 
			}
			en.io.SetServerFormMode (FormMode.Map );  
			en.locs [locnum].Move(); 
		

		}

		public void Delayed()
		{ isDelayed = true;
			en.io.ServerWrite (DisplayName + " " + en.sysstr.GetString (SSType.DelayedStart)+ Environment.NewLine);   

		}

	

		public override  void WriteToSave(System.IO.BinaryWriter wr)
		{  
			base.WriteToSave (wr);
			SpeedSneak.WriteToSave (wr);
			FightWill.WriteToSave (wr);
			LoreLuck.WriteToSave (wr);
			wr.Write (curSanity);
			wr.Write (curStamina);
			wr.Write (clues);
			wr.Write (Money);
			wr.Write (isDelayed); 
			wr.Write (locnum); 
			wr.Write (investnum); 
			//Вещи

			//Особые карты
			wr.Write ((byte)specCards.Count);
			foreach (SpecialCard  s in specCards)
				s.WriteToSave (wr);

			//трофеи
			wr.Write ((byte)monsterTrofies.Count);
			foreach (MonsterIndivid  m in monsterTrofies)
				wr.Write (m.GetID());

			wr.Write ((byte)gateTrofies .Count);
			foreach (GatePrototype   g in gateTrofies )
				wr.Write (g.GetID());



		}


		public  override void ReadFromSave(System.IO.BinaryReader rd)
		{ 
			SpeedSneak.ReadFromSave (rd);
			FightWill.ReadFromSave (rd);
			LoreLuck.ReadFromSave (rd);
			curSanity=rd.ReadByte() ;
			curStamina=rd.ReadByte();
			clues=rd.ReadByte();
			Money=rd.ReadByte();
			isDelayed=rd.ReadBoolean() ; 
			locnum = rd.ReadInt16(); 
			investnum = rd.ReadByte ();  
			en.locs [locnum].AddInvestigator (investnum);  
			//Вещи

			//Особые карты
			specCards.Clear ();  
			byte count = rd.ReadByte ();
			SpecialCard s;
			for (int i=0; i<count; i++)
			{s = SpecialCard.ReadFromSave (en,rd);
				//порядок важен
				s.Init (this, false);
				s.ReadFromSaveIndivid (rd);


			}

			monsterTrofies.Clear ();  
		    count = rd.ReadByte ();
			for (int i=0; i<count; i++)
			  monsterTrofies.Add( en.MonstersCup.GetCardById( rd.ReadInt16()));   

			gateTrofies.Clear ();  
			count = rd.ReadByte ();
			for (int i=0; i<count; i++)
				gateTrofies .Add( en.gates.GetCardById( rd.ReadInt16()));   


			
		}

		public void Reset()
		{ 
			SpeedSneak.Reset (); 
			FightWill.Reset ();
			LoreLuck.Reset ();
			curSanity = maxSanity; 
			curStamina=maxStamina ;
			clues=0;
			Money=0 ;
			locnum = 0;
			isCanMove = true; 
			isDelayed=false ; 
			specCards.Clear ();
			foreach (MonsterIndivid m in monsterTrofies)
				en.MonstersCup.Add (m);
			foreach (GatePrototype g in gateTrofies )
				en.gates .Add (g);
			monsterTrofies.Clear ();
			gateTrofies.Clear (); 

		}

		public void AhEnc()
		{ en.curs.DiscardEncounters();
			if (en.locs [locnum].GetLocType () != LocathionType.OW)
			    en.locs [locnum].Encounter (); 
             else
				en.clock.NextPlayer (); 

		}


		public void OwEnc()
		{ en.curs.DiscardEncounters();
			if (en.locs [locnum].GetLocType () == LocathionType.OW)
				en.locs [locnum].Encounter(); 
			else
				en.clock.NextPlayer (); 

		}


		public void AddTrophy( MonsterIndivid mons)
		{
			monsterTrofies.Add (mons); 
		}


		public void BeforeCombatCheck()
		{   
			en.curs.curFight.DoCombatCheck ();  
		}


		public string PrintToMap()
		{ string res = DisplayName;
			if (isDelayed)
				res += "( " + en.sysstr.GetString (SSType.Delayed)+ ")";
			return res;

		}

	}



}

