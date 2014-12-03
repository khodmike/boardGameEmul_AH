using System;

namespace mmxAH
{
	public class SpecialCardText
	{ public SpecialCardText()
		{

		}
		public  string Title, InfoText, InitMessage, DiscardMessage;
		public   SpecialCardText  FromText( TextFileParser text)
		{  byte numStrings;
			Title = text.GetCurString (); 
			if( ! Byte.TryParse(text.GetToken(), out numStrings))
				return null;

			for (int i=0; i<numStrings; i++)
				InfoText += text.GetCurString () + Environment.NewLine;
			InitMessage = text.GetCurString ();
			DiscardMessage = text.GetCurString ();  
			return this;
		}

	}

	public class SpecialCard
	{ protected string CodeName;
		protected Investigator inv;
		protected GameEngine en;
		protected bool isFirstTurn;
		protected SpecialCardText text;

		public   SpecialCard ()
		{ 
		}

		public string GetCodeName()
		{ return CodeName;
		}

		public   void Print()
		{ en.io.Print(text.Title+Environment.NewLine,12,true);
			en.io.PrintTag (text.InfoText+Environment.NewLine );  

		}

		public virtual void Init( Investigator invest, bool isWrite=true)
		{  inv=invest;
			inv.AddSpecCards(this);
			if (isWrite)
			{   en.io.SetFormMode (FormMode.Log);  
				en.io.PrintTag (inv.GetTitle () + " " + text.InitMessage + Environment.NewLine);
			}

		}
		public virtual void Discard( bool isPrint)
		{ //необходим для тика
			inv.RemoveSpecCards (CodeName, false);  
			if (isPrint)
			{
				en.io.SetFormMode (FormMode.Log);  
				en.io.PrintTag (inv.GetTitle () + " " + text.DiscardMessage + Environment.NewLine); 
			}
		}

		protected virtual   void Tick ()
		{ if (isFirstTurn)
			{
				isFirstTurn = false;
				return;
			} 
			en.io.PrintToLog (text.Title+":", 12, true);
			byte d= new DiceRoller(en).RollOneDice();
			if(d==1)
				Discard(true);
			en.io.PrintToLog (Environment.NewLine);
		}



		protected void InitTriger()
		{ inv.myTrigers.AddMandarotoryTriger( new MandoratoryTriger(TrigerEvent.Upkeep, Tick, CodeName)); 

	    }



		public virtual void  WriteToSave( System.IO.BinaryWriter wr)
		{

		}
		public virtual void  ReadFromSaveIndivid( System.IO.BinaryReader  rd)
		{

		}
		
			public static SpecialCard ReadFromSave(GameEngine en, System.IO.BinaryReader rd)
		{  byte b= rd.ReadByte();
			switch (b)
		{ case 0: return new STLMembership (en); 
		  case 1: return new Bless (en); 
		  case 2: return new Curse (en); 
		  case 3: return new Retainer  (en);
		  case 4: return new DeputatiOfArchem  (en); 
		  default: return null;
			}

		}


	}


	public  class STLMembership: SpecialCard
	{   public STLMembership(GameEngine eng)
		{ en=eng;
		  CodeName = "STLMember";
			text = en.scTexts [0]; 


		}


		public override void WriteToSave (System.IO.BinaryWriter wr)
		{
			wr.Write ((byte)0);
		} 

	

	}


	public  class Bless: SpecialCard
	{   public Bless(GameEngine eng)
		{ en=eng;
			CodeName = "Bless";
			text = en.scTexts [1]; 

		}

		public override void Init (Investigator invest, bool isWrite=true)
		{   
			 
			if (invest.RemoveSpecCards ("Curse", true))
				return;

			invest.RemoveSpecCards ("Bless", false);
			isFirstTurn = true; 
			base.Init (invest, isWrite );
			InitTriger ();
			invest.SetSTTresh (4); 

		}
			 
		
      public override void Discard (bool isPrint)
		{
		    inv.SetSTTresh (5);
			inv.myTrigers.RemoveTriger (CodeName); 
				base.Discard (isPrint);
		}


		public override void WriteToSave (System.IO.BinaryWriter wr)
		{
			wr.Write ((byte)1);
			wr.Write (isFirstTurn); 
		} 

		public override void ReadFromSaveIndivid (System.IO.BinaryReader rd)
		{
			isFirstTurn = rd.ReadBoolean ();  
		}
	}

	public  class Curse: SpecialCard
	{   public Curse(GameEngine eng)
		{ en=eng;
			CodeName = "Curse";
			text = en.scTexts [2]; 

		}

		public override void Init (Investigator invest,bool isWrite=true)
		{   

			if (invest.RemoveSpecCards ("Bless", true))
				return;

			invest.RemoveSpecCards ("Curse", false);
			isFirstTurn = true; 
			base.Init (invest, isWrite );
			InitTriger ();
			invest.SetSTTresh (6); 

		}


		public override void Discard (bool isPrint)
		{
			inv.SetSTTresh (5);
			inv.myTrigers.RemoveTriger (CodeName); 
			base.Discard (isPrint);
		}

		public override void WriteToSave (System.IO.BinaryWriter wr)
		{
			wr.Write ((byte)2);
			wr.Write (isFirstTurn); 
		} 


		public override void ReadFromSaveIndivid (System.IO.BinaryReader rd)
		{
			isFirstTurn = rd.ReadBoolean ();  
		}

	}


	public  class Retainer: SpecialCard
	{   public Retainer(GameEngine eng)
		{ en=eng;
			CodeName = "Retainer";
			text = en.scTexts [3]; 

		}

		public override void Init (Investigator invest, bool isWrite=true)
		{   

			isFirstTurn = true; 
			base.Init (invest, isWrite );
			InitTriger ();


		}


		public override void Discard (bool isPrint)
		{

			inv.myTrigers.RemoveTriger (CodeName); 
			base.Discard (isPrint);
		}


		protected override void Tick()
		{ 
				en.io.PrintToLog (text.Title+":", 12, true);
			    inv.GainMoney (2); 
				if (isFirstTurn)
				{
				isFirstTurn = false;
				return;
				} 
			en.io.PrintToLog ("   "); 
				byte d= new DiceRoller(en).RollOneDice();
				if(d==1)
					Discard(true);
				en.io.PrintToLog (Environment.NewLine);

		}

		public override void WriteToSave (System.IO.BinaryWriter wr)
		{
			wr.Write ((byte)3);
			wr.Write (isFirstTurn); 
		} 

		public override void ReadFromSaveIndivid (System.IO.BinaryReader rd)
		{
			isFirstTurn = rd.ReadBoolean ();  
		}
	}


	public  class DeputatiOfArchem	: SpecialCard
	{   public  DeputatiOfArchem(GameEngine eng)
		{ en=eng;
			CodeName = "DepOfArch";
			text = en.scTexts [4]; 

		}

		public override void Init (Investigator invest, bool isWrite=true)
		{   

			// Получение пистолета и вагона
			base.Init (invest,isWrite );
			InitTriger ();


		}


		public override void Discard (bool isPrint)
		{

			inv.myTrigers.RemoveTriger (CodeName); 
			base.Discard (isPrint);
		}


		protected override void Tick()
		{ 
			en.io.PrintToLog (text.Title+":", 12, true);
			inv.GainMoney (1); 
			en.io.PrintToLog (Environment.NewLine);

		}

		public override void WriteToSave (System.IO.BinaryWriter wr)
		{
			wr.Write ((byte)4);
		} 



	}
}

